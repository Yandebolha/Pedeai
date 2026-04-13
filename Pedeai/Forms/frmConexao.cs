using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using MySqlConnector;

namespace Pedeai.Forms
{
    /// <summary>
    /// Permite editar a connection string do MySQL e descobrir servidores na rede local.
    /// </summary>
    public partial class frmConexao : Form
    {
        /// <summary>Se definido, exibe uma mensagem de erro no topo da janela ao abrir.</summary>
        public string MotivoErro { get; set; }

        private bool _varrendo = false;

        public frmConexao()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            Load += FrmConexao_Load;
        }

        private void FrmConexao_Load(object sender, EventArgs e)
        {
            CarregarConexaoAtual();
            if (!string.IsNullOrWhiteSpace(MotivoErro))
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text      = "âš  " + MotivoErro;
            }
        }

        // â”€â”€ Leitura da connection string atual â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        private void CarregarConexaoAtual()
        {
            var cs = ConfigurationManager.AppSettings["ConnectionString"] ?? "";
            ParseConnectionString(cs);
        }

        private void ParseConnectionString(string cs)
        {
            // Interpreta os campos manualmente (MySqlConnectionStringBuilder)
            try
            {
                var b = new MySqlConnectionStringBuilder(cs);
                txtServidor.Text = b.Server   ?? "localhost";
                txtPorta.Text    = b.Port.ToString();
                txtBanco.Text    = b.Database ?? "pedeai";
                txtUsuario.Text  = b.UserID   ?? "root";
                txtSenha.Text    = b.Password ?? "";
            }
            catch
            {
                txtServidor.Text = "localhost";
                txtPorta.Text    = "3306";
                txtBanco.Text    = "pedeai";
                txtUsuario.Text  = "root";
                txtSenha.Text    = "";
            }
        }

        private string MontarConnectionString()
            => $"Server={txtServidor.Text.Trim()};Database={txtBanco.Text.Trim()};" +
               $"User={txtUsuario.Text.Trim()};Password={txtSenha.Text};" +
               $"Port={txtPorta.Text.Trim()};CharSet=utf8mb4;SslMode=None;AllowPublicKeyRetrieval=true;";

        // â”€â”€ Varredura de rede â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        private async void BtnVarrer_Click(object sender, EventArgs e)
        {
            if (_varrendo) return;
            _varrendo = true;
            btnVarrer.Enabled   = false;
            lstServidores.Items.Clear();
            lblStatus.Text      = "Varrendo a rede...";
            progressVarrer.Style = ProgressBarStyle.Marquee;

            int porta = 3306;
            if (!int.TryParse(txtPorta.Text.Trim(), out porta)) porta = 3306;

            var encontrados = new List<string>();
            try
            {
                // Descobre o prefixo da rede local (ex: 192.168.1.)
                var prefixos = new HashSet<string>();
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus != OperationalStatus.Up) continue;
                    foreach (var ua in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ua.Address.AddressFamily != AddressFamily.InterNetwork) continue;
                        var ip = ua.Address.ToString();
                        var parts = ip.Split('.');
                        if (parts.Length == 4 && ip != "127.0.0.1")
                            prefixos.Add($"{parts[0]}.{parts[1]}.{parts[2]}.");
                    }
                }
                if (prefixos.Count == 0) prefixos.Add("192.168.1.");

                // Testa cada IP em paralelo com timeout curto
                var tarefas = new List<Task>();
                foreach (var prefix in prefixos)
                {
                    for (int i = 1; i <= 254; i++)
                    {
                        string ip = prefix + i;
                        int portaLocal = porta;
                        tarefas.Add(Task.Run(async () =>
                        {
                            try
                            {
                                using var tcp = new TcpClient();
                                var ct = new System.Threading.CancellationTokenSource(400);
                                await tcp.ConnectAsync(ip, portaLocal);
                                lock (encontrados) encontrados.Add(ip);
                            }
                            catch { }
                        }));
                    }
                }
                await Task.WhenAll(tarefas);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Erro na varredura: " + ex.Message;
            }

            progressVarrer.Style = ProgressBarStyle.Blocks;
            progressVarrer.Value = 0;
            lstServidores.Items.Clear();
            lstServidores.Items.Add("localhost");
            encontrados.Sort();
            foreach (var ip in encontrados)
                lstServidores.Items.Add(ip);

            lblStatus.Text    = encontrados.Count == 0
                ? "Nenhum servidor MySQL encontrado na rede."
                : $"{encontrados.Count} servidor(es) encontrado(s) na porta {porta}.";
            btnVarrer.Enabled = true;
            _varrendo         = false;
        }

        // â”€â”€ Testar conexÃ£o â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        private void BtnTestar_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Testando...";
            try
            {
                using var conn = new MySqlConnection(MontarConnectionString());
                conn.Open();
                lblStatus.ForeColor = Color.Green;
                lblStatus.Text      = $"âœ” ConexÃ£o OK â€” MySQL {conn.ServerVersion}";
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text      = "âœ– Falha: " + ex.Message;
            }
        }

        // â”€â”€ Salvar no App.config â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            string novaCs = MontarConnectionString();

            // Testa antes de salvar
            try
            {
                using var conn = new MySqlConnection(novaCs);
                conn.Open();
            }
            catch (Exception ex)
            {
                var r = MessageBox.Show(
                    $"A conexÃ£o falhou:\n{ex.Message}\n\nSalvar mesmo assim?",
                    "Aviso", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (r != DialogResult.Yes) return;
            }

            try
            {
                // Edita o App.config em disco
                string exePath    = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string configPath = exePath + ".config";
                var doc = new XmlDocument();
                doc.Load(configPath);
                var node = doc.SelectSingleNode(
                    "//appSettings/add[@key='ConnectionString']");
                if (node?.Attributes != null)
                    node.Attributes["value"].Value = novaCs;
                doc.Save(configPath);

                // Atualiza em memÃ³ria tambÃ©m (sem reiniciar o app)
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["ConnectionString"].Value = novaCs;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                MessageBox.Show(
                    "ConfiguraÃ§Ã£o salva!\nAs novas conexÃµes usarÃ£o o servidor configurado.",
                    "Salvo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar configuraÃ§Ã£o:\n" + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // â”€â”€ Clique na lista de servidores descobertos â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        private void LstServidores_DoubleClick(object sender, EventArgs e)
        {
            if (lstServidores.SelectedItem is string ip)
                txtServidor.Text = ip;
        }

        // â”€â”€ Cancelar â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        private void BtnCancelar_Click(object sender, EventArgs e) { Close(); }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
