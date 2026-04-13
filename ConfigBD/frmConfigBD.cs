using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using MySqlConnector;

namespace ConfigBD
{
    public class frmConfigBD : Form
    {
        // ── Caminho do App.config do RanGoFood (mesmo diretório do ConfigBD.exe) ──
        // No .NET 5+, o arquivo de configuração se chama AppName.dll.config
        private static string ConfigPath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RanGoFood.dll.config");

        private TextBox    txtServidor;
        private TextBox    txtPorta;
        private TextBox    txtBanco;
        private TextBox    txtUsuario;
        private TextBox    txtSenha;
        private ListBox    lstServidores;
        private Button     btnVarrer;
        private Button     btnTestar;
        private Button     btnSalvar;
        private Label      lblStatus;
        private ProgressBar progress;
        private bool       _varrendo;

        public frmConfigBD()
        {
            BuildUI();
            CarregarConfig();
        }

        // ── Leitura / escrita do App.config ───────────────────────────────────
        private void CarregarConfig()
        {
            string cs = LerConnectionString();
            if (string.IsNullOrWhiteSpace(cs))
            {
                SetStatus("⚠  App.config não encontrado nesta pasta. Preencha os campos manualmente.", Color.DarkOrange);
                txtServidor.Text = "localhost";
                txtPorta.Text    = "3306";
                txtBanco.Text    = "pedeai";
                txtUsuario.Text  = "root";
                return;
            }
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
                SetStatus("⚠  Não foi possível interpretar a connection string atual.", Color.DarkOrange);
            }
        }

        private string LerConnectionString()
        {
            if (!File.Exists(ConfigPath)) return null;
            try
            {
                var doc  = new XmlDocument();
                doc.Load(ConfigPath);
                return doc.SelectSingleNode("//appSettings/add[@key='ConnectionString']")
                          ?.Attributes?["value"]?.Value ?? "";
            }
            catch { return null; }
        }

        private string MontarCS(int timeoutSecs = 15) =>
            $"Server={txtServidor.Text.Trim()};" +
            $"Database={txtBanco.Text.Trim()};" +
            $"User={txtUsuario.Text.Trim()};" +
            $"Password={txtSenha.Text};" +
            $"Port={txtPorta.Text.Trim()};" +
            $"CharSet=utf8mb4;SslMode=None;AllowPublicKeyRetrieval=true;" +
            $"ConnectionTimeout={timeoutSecs};";

        // ── Testar conexão ────────────────────────────────────────────────────
        private async void BtnTestar_Click(object sender, EventArgs e)
        {
            btnTestar.Enabled = false;
            btnSalvar.Enabled = false;
            SetStatus("Testando conexão...", Color.Gray);
            try
            {
                string versao = await Task.Run(() =>
                {
                    // Tenta primeiro com o banco especificado (mais permissivo para usuários remotos).
                    // Se falhar, tenta sem banco (útil quando o banco ainda não existe).
                    string cs = MontarCS(timeoutSecs: 5);
                    try
                    {
                        using var conn = new MySqlConnection(cs);
                        conn.Open();
                        return conn.ServerVersion;
                    }
                    catch
                    {
                        var b = new MySqlConnectionStringBuilder(cs) { Database = "" };
                        using var conn2 = new MySqlConnection(b.ToString());
                        conn2.Open();
                        return conn2.ServerVersion;
                    }
                });
                SetStatus($"✔  Servidor acessível — MySQL {versao}", Color.Green);
            }
            catch (Exception ex)
            {
                SetStatus("✖  " + ex.Message, Color.Red);
            }
            finally
            {
                btnTestar.Enabled = true;
                btnSalvar.Enabled = true;
            }
        }

        // ── Salvar no App.config ──────────────────────────────────────────────
        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            // Cria o arquivo de configuração mínimo se não existir
            if (!File.Exists(ConfigPath))
            {
                try
                {
                    var xmlNovo = new XmlDocument();
                    xmlNovo.AppendChild(xmlNovo.CreateXmlDeclaration("1.0", "utf-8", null));
                    var root = xmlNovo.AppendChild(xmlNovo.CreateElement("configuration"));
                    var appSettings = root.AppendChild(xmlNovo.CreateElement("appSettings"));
                    var add = xmlNovo.CreateElement("add");
                    add.SetAttribute("key", "ConnectionString");
                    add.SetAttribute("value", "");
                    appSettings.AppendChild(add);
                    xmlNovo.Save(ConfigPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Não foi possível criar o arquivo de configuração:\n{ConfigPath}\n\n{ex.Message}\n\n" +
                        "Verifique se o ConfigBD.exe está na mesma pasta que o RanGoFood.exe.",
                        "Erro ao criar arquivo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Testa servidor antes de salvar (não-bloqueante, apenas aviso)
            try
            {
                string cs = MontarCS(timeoutSecs: 5);
                bool ok = false;
                try { using var c = new MySqlConnection(cs); c.Open(); ok = true; } catch { }
                if (!ok)
                {
                    var b = new MySqlConnectionStringBuilder(cs) { Database = "" };
                    try { using var c2 = new MySqlConnection(b.ToString()); c2.Open(); ok = true; } catch { }
                }
                if (!ok)
                {
                    // Avisa mas não bloqueia o salvamento
                    var r = MessageBox.Show(
                        "Não foi possível verificar a conexão com o servidor informado.\n\nSalvar mesmo assim?",
                        "Aviso", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (r != DialogResult.Yes) return;
                }
            }
            catch { /* ignora erros inesperados no teste */ }

            try
            {
                var doc  = new XmlDocument();
                doc.Load(ConfigPath);
                var node = doc.SelectSingleNode("//appSettings/add[@key='ConnectionString']");
                if (node?.Attributes != null)
                    node.Attributes["value"].Value = MontarCS();
                else
                {
                    // Cria o nó se não existir
                    var appSettings = doc.SelectSingleNode("//appSettings")
                                     ?? doc.DocumentElement.AppendChild(doc.CreateElement("appSettings"));
                    var add = doc.CreateElement("add");
                    add.SetAttribute("key", "ConnectionString");
                    add.SetAttribute("value", MontarCS());
                    appSettings.AppendChild(add);
                }
                doc.Save(ConfigPath);
                SetStatus("✔  Configuração salva com sucesso!", Color.Green);
                MessageBox.Show(
                    "Configuração salva!\n\nReinicie o RanGoFood para usar a nova conexão.",
                    "Salvo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar:\n" + ex.Message, "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Varredura de rede ─────────────────────────────────────────────────
        private async void BtnVarrer_Click(object sender, EventArgs e)
        {
            if (_varrendo) return;
            _varrendo         = true;
            btnVarrer.Enabled = false;
            lstServidores.Items.Clear();
            lstServidores.Items.Add("localhost");
            progress.Style = ProgressBarStyle.Marquee;
            SetStatus("Varrendo a rede local — aguarde...", Color.Gray);

            int porta = 3306;
            int.TryParse(txtPorta.Text.Trim(), out porta);
            if (porta <= 0) porta = 3306;

            var encontrados = new List<string>();
            try
            {
                var prefixos = new HashSet<string>();
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus != OperationalStatus.Up) continue;
                    foreach (var ua in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ua.Address.AddressFamily != AddressFamily.InterNetwork) continue;
                        var partes = ua.Address.ToString().Split('.');
                        if (partes.Length == 4 && ua.Address.ToString() != "127.0.0.1")
                            prefixos.Add($"{partes[0]}.{partes[1]}.{partes[2]}.");
                    }
                }
                if (prefixos.Count == 0) prefixos.Add("192.168.1.");

                var tarefas = new List<Task>();
                foreach (var prefix in prefixos)
                    for (int i = 1; i <= 254; i++)
                    {
                        string ip       = prefix + i;
                        int    portaCpy = porta;
                        tarefas.Add(Task.Run(async () =>
                        {
                            try
                            {
                                using var tcp = new TcpClient();
                                var cts = new System.Threading.CancellationTokenSource(450);
                                await tcp.ConnectAsync(ip, portaCpy);
                                lock (encontrados) encontrados.Add(ip);
                            }
                            catch { }
                        }));
                    }
                await Task.WhenAll(tarefas);
            }
            catch (Exception ex) { SetStatus("Erro na varredura: " + ex.Message, Color.Red); }

            progress.Style = ProgressBarStyle.Blocks;
            progress.Value = 0;
            encontrados.Sort();
            foreach (var ip in encontrados)
                lstServidores.Items.Add(ip);

            SetStatus(encontrados.Count == 0
                ? $"Nenhum servidor encontrado na porta {porta}."
                : $"✔  {encontrados.Count} servidor(es) encontrado(s) na porta {porta}. Clique duplo para selecionar.",
                encontrados.Count == 0 ? Color.DarkOrange : Color.Green);

            btnVarrer.Enabled = true;
            _varrendo         = false;
        }

        private void LstServidores_DoubleClick(object sender, EventArgs e)
        {
            if (lstServidores.SelectedItem is string ip)
            {
                txtServidor.Text = ip;
                SetStatus($"Servidor selecionado: {ip}. Clique em Testar ou Salvar.", Color.DarkBlue);
            }
        }

        private void SetStatus(string msg, Color cor)
        {
            lblStatus.ForeColor = cor;
            lblStatus.Text      = msg;
        }

        // ── UI ────────────────────────────────────────────────────────────────
        private void BuildUI()
        {
            Text            = "RanGoFood — Configuração de Banco de Dados";
            ClientSize      = new Size(540, 520);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            MinimizeBox     = false;
            StartPosition   = FormStartPosition.CenterScreen;
            BackColor       = Color.FromArgb(250, 242, 225);

            // Tentar aplicar ícone do RanGoFood se existir na mesma pasta
            string ico = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RanGoFood.ico");
            if (File.Exists(ico)) try { Icon = new Icon(ico); } catch { }

            // ── Título ──
            Controls.Add(new Label
            {
                Text      = "⚙️  Configuração de Banco de Dados",
                Font      = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(120, 60, 10),
                Location  = new Point(0, 14),
                Size      = new Size(540, 34),
                TextAlign = ContentAlignment.MiddleCenter,
            });
            Controls.Add(new Label
            {
                Text      = "Configure a conexão com o servidor MySQL do RanGoFood.",
                Font      = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(80, 60, 30),
                Location  = new Point(0, 50),
                Size      = new Size(540, 20),
                TextAlign = ContentAlignment.MiddleCenter,
            });

            // ── Campos ──
            int y = 82;
            AddField("Servidor (IP / hostname):", out txtServidor, ref y, "localhost");
            AddField("Porta MySQL:",               out txtPorta,    ref y, "3306", 80);
            AddField("Nome do Banco:",             out txtBanco,    ref y, "pedeai");
            AddField("Usuário:",                   out txtUsuario,  ref y, "root");

            Controls.Add(MkLbl("Senha:", 20, y));
            txtSenha = new TextBox
            {
                Location              = new Point(170, y - 2),
                Size                  = new Size(220, 24),
                BorderStyle           = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true,
            };
            Controls.Add(txtSenha);
            y += 40;

            // ── Botões de ação ──
            btnTestar = MkBtn("🔍 Testar Conexão", 20,  y, 155, Color.FromArgb(52, 100, 160));
            btnSalvar = MkBtn("💾 Salvar",          185, y, 110, Color.FromArgb(87, 140, 38));
            var btnFechar = MkBtn("Fechar",          305, y,  90, Color.FromArgb(150, 60, 40));
            btnTestar.Click  += BtnTestar_Click;
            btnSalvar.Click  += BtnSalvar_Click;
            btnFechar.Click  += (_, __) => Close();
            Controls.AddRange(new Control[] { btnTestar, btnSalvar, btnFechar });
            y += 44;

            // ── Status ──
            lblStatus = new Label
            {
                Location  = new Point(20, y),
                Size      = new Size(500, 36),
                Font      = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(80, 60, 30),
            };
            Controls.Add(lblStatus);
            y += 44;

            // ── Separador lista ──
            Controls.Add(new Label
            {
                Text      = "Servidores MySQL encontrados na rede local:",
                Location  = new Point(20, y),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 50, 30),
            });
            y += 22;

            progress = new ProgressBar
            {
                Location = new Point(20, y),
                Size     = new Size(400, 10),
                Style    = ProgressBarStyle.Blocks,
            };
            Controls.Add(progress);
            y += 16;

            lstServidores = new ListBox
            {
                Location    = new Point(20, y),
                Size        = new Size(400, 100),
                BorderStyle = BorderStyle.FixedSingle,
                Font        = new Font("Consolas", 9.5F),
                BackColor   = Color.FromArgb(255, 252, 245),
            };
            lstServidores.DoubleClick += LstServidores_DoubleClick;
            lstServidores.Items.Add("localhost");
            Controls.Add(lstServidores);

            btnVarrer = MkBtn("🔎 Varrer\nRede", 430, y, 90, Color.FromArgb(180, 110, 20));
            btnVarrer.Size   = new Size(90, 50);
            btnVarrer.Click += BtnVarrer_Click;
            Controls.Add(btnVarrer);
        }

        private void AddField(string label, out TextBox txt, ref int y,
            string placeholder = "", int width = 220)
        {
            Controls.Add(MkLbl(label, 20, y));
            txt = new TextBox
            {
                Location    = new Point(170, y - 2),
                Size        = new Size(width, 24),
                BorderStyle = BorderStyle.FixedSingle,
            };
            Controls.Add(txt);
            y += 36;
        }

        private static Label MkLbl(string text, int x, int y) => new Label
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
