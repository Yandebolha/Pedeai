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
    public class frmConexao : Form
    {
        private TextBox   txtServidor;
        private TextBox   txtPorta;
        private TextBox   txtBanco;
        private TextBox   txtUsuario;
        private TextBox   txtSenha;
        private ListBox   lstServidores;
        private Button    btnVarrer;
        private Button    btnTestar;
        private Button    btnSalvar;
        private Button    btnCancelar;
        private Label     lblStatus;
        private ProgressBar progressVarrer;

        private bool _varrendo = false;

        public frmConexao()
        {
            BuildUI();
            CarregarConexaoAtual();
        }

        // ── Leitura da connection string atual ─────────────────────────────────
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

        // ── Varredura de rede ─────────────────────────────────────────────────
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

        // ── Testar conexão ────────────────────────────────────────────────────
        private void BtnTestar_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Testando...";
            try
            {
                using var conn = new MySqlConnection(MontarConnectionString());
                conn.Open();
                lblStatus.ForeColor = Color.Green;
                lblStatus.Text      = $"✔ Conexão OK — MySQL {conn.ServerVersion}";
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text      = "✖ Falha: " + ex.Message;
            }
        }

        // ── Salvar no App.config ──────────────────────────────────────────────
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
                    $"A conexão falhou:\n{ex.Message}\n\nSalvar mesmo assim?",
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

                // Atualiza em memória também (sem reiniciar o app)
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["ConnectionString"].Value = novaCs;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                MessageBox.Show(
                    "Configuração salva!\nAs novas conexões usarão o servidor configurado.",
                    "Salvo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar configuração:\n" + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Clique na lista de servidores descobertos ─────────────────────────
        private void LstServidores_DoubleClick(object sender, EventArgs e)
        {
            if (lstServidores.SelectedItem is string ip)
                txtServidor.Text = ip;
        }

        // ── UI ────────────────────────────────────────────────────────────────
        private void BuildUI()
        {
            Text            = "Configuração de Conexão ao Banco de Dados";
            ClientSize      = new Size(520, 500);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            MinimizeBox     = false;
            StartPosition   = FormStartPosition.CenterParent;
            BackColor       = Color.FromArgb(250, 242, 225);

            // Título
            var lblTitulo = new Label
            {
                Text      = "⚙️  Conexão ao Banco de Dados",
                Font      = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(120, 60, 10),
                Location  = new Point(0, 14),
                Size      = new Size(520, 34),
                TextAlign = ContentAlignment.MiddleCenter,
            };

            // Labels e campos
            int col1 = 20, col2 = 160, lw = 130, fw = 200;
            int y = 60;

            var lblSrv = MkLbl("Servidor (IP/hostname):", col1, y);
            txtServidor = new TextBox { Location = new Point(col2, y - 2), Size = new Size(fw, 24), BorderStyle = BorderStyle.FixedSingle };
            y += 36;

            var lblPrt = MkLbl("Porta MySQL:", col1, y);
            txtPorta = new TextBox { Location = new Point(col2, y - 2), Size = new Size(70, 24), BorderStyle = BorderStyle.FixedSingle, Text = "3306" };
            y += 36;

            var lblBd = MkLbl("Nome do Banco:", col1, y);
            txtBanco = new TextBox { Location = new Point(col2, y - 2), Size = new Size(fw, 24), BorderStyle = BorderStyle.FixedSingle };
            y += 36;

            var lblUsr = MkLbl("Usuário:", col1, y);
            txtUsuario = new TextBox { Location = new Point(col2, y - 2), Size = new Size(fw, 24), BorderStyle = BorderStyle.FixedSingle };
            y += 36;

            var lblPwd = MkLbl("Senha:", col1, y);
            txtSenha = new TextBox { Location = new Point(col2, y - 2), Size = new Size(fw, 24), BorderStyle = BorderStyle.FixedSingle, UseSystemPasswordChar = true };
            y += 44;

            // Botões testar/salvar
            btnTestar = MkBtn("🔍 Testar Conexão", col1, y, 155, Color.FromArgb(52, 100, 160));
            btnTestar.Click += BtnTestar_Click;

            btnSalvar = MkBtn("💾 Salvar", col1 + 165, y, 100, Color.FromArgb(87, 140, 38));
            btnSalvar.Click += BtnSalvar_Click;

            btnCancelar = MkBtn("Cancelar", col1 + 275, y, 90, Color.FromArgb(150, 60, 40));
            btnCancelar.Click += (_, __) => Close();

            y += 48;

            // Status
            lblStatus = new Label
            {
                Location  = new Point(col1, y),
                Size      = new Size(480, 22),
                Font      = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(80, 60, 30),
            };
            y += 30;

            // Separador
            var sep = new Label
            {
                Text      = "Servidores MySQL encontrados na rede:",
                Location  = new Point(col1, y),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 50, 30),
            };
            y += 22;

            progressVarrer = new ProgressBar
            {
                Location  = new Point(col1, y),
                Size      = new Size(380, 14),
                Style     = ProgressBarStyle.Blocks,
                Visible   = true,
            };
            y += 20;

            lstServidores = new ListBox
            {
                Location      = new Point(col1, y),
                Size          = new Size(380, 90),
                BorderStyle   = BorderStyle.FixedSingle,
                Font          = new Font("Consolas", 9.5F),
                BackColor     = Color.FromArgb(255, 252, 245),
            };
            lstServidores.DoubleClick += LstServidores_DoubleClick;
            lstServidores.Items.Add("localhost");

            btnVarrer = MkBtn("🔎 Varrer Rede", col1 + 390, y, 110, Color.FromArgb(180, 110, 20));
            btnVarrer.Click += BtnVarrer_Click;

            Controls.AddRange(new Control[]
            {
                lblTitulo,
                lblSrv, txtServidor,
                lblPrt, txtPorta,
                lblBd,  txtBanco,
                lblUsr, txtUsuario,
                lblPwd, txtSenha,
                btnTestar, btnSalvar, btnCancelar,
                lblStatus,
                sep, progressVarrer,
                lstServidores, btnVarrer,
            });
        }

        private static Label MkLbl(string text, int x, int y)
            => new Label
            {
                Text      = text,
                Location  = new Point(x, y),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 50, 30),
            };

        private static Button MkBtn(string text, int x, int y, int w, Color back)
        {
            var b = new Button
            {
                Text      = text,
                Location  = new Point(x, y),
                Size      = new Size(w, 30),
                BackColor = back,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 9F),
                Cursor    = Cursors.Hand,
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
