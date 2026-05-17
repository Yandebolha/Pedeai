using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmTurno : Form
    {
        private readonly TurnoBLL _bll = new TurnoBLL();
        private Turno _turnoAtivo;

        public frmTurno()
        {
            InitializeComponent();
        }

        private void frmTurno_Load(object sender, EventArgs e)
        {
            AtualizarEstado();
        }

        private void AtualizarEstado()
        {
            _turnoAtivo = _bll.GetAtivo();

            if (_turnoAtivo != null)
            {
                lblStatus.Text      = $"TURNO ABERTO  #{_turnoAtivo.Codigo}  |  Abertura: {_turnoAtivo.turAbertura:dd/MM/yyyy HH:mm}  |  Usu\u00e1rio: {_turnoAtivo.turUsuario}";
                lblStatus.BackColor = Color.FromArgb(39, 174, 96);
                lblCaixaIni.Text    = $"Caixa inicial: R$ {_turnoAtivo.turCaixa_Inicial:N2}";

                pnlAbrir.Visible   = false;
                pnlFechar.Visible  = true;
                numCaixaFinal.Value = _turnoAtivo.turCaixa_Inicial;
            }
            else
            {
                lblStatus.Text      = "NENHUM TURNO ABERTO";
                lblStatus.BackColor = Color.FromArgb(192, 57, 43);
                lblCaixaIni.Text    = "";

                pnlAbrir.Visible   = true;
                pnlFechar.Visible  = false;
                numCaixaInicial.Value = 0;
                txtObsAbrir.Text      = "";
            }

            CarregarHistorico();
        }

        private void BtnAbrir_Click(object sender, EventArgs e)
        {
            decimal caixaIni = numCaixaInicial.Value;
            string obs       = txtObsAbrir.Text.Trim();

            var erro = _bll.Abrir(caixaIni, obs);
            if (!string.IsNullOrEmpty(erro))
            {
                MessageBox.Show(erro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Logger.Log("frmTurno", "BtnAbrir_Click", $"Turno aberto | Caixa inicial: R$ {caixaIni:N2}");
            MessageBox.Show("Turno aberto com sucesso!", "Turno", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AtualizarEstado();
        }

        private void BtnFechar_Click(object sender, EventArgs e)
        {
            string obs    = txtObsFechar.Text.Trim();
            var resumo    = _bll.GetResumoMovimentado(_turnoAtivo);
            decimal esperado = _turnoAtivo.turCaixa_Inicial + resumo.totalVendas;

            // Abre o único diálogo de fechamento — edição inline, com ou sem diferença
            decimal? resultado = MostrarDialogFechamento(esperado, numCaixaFinal.Value, resumo);
            if (resultado == null) return;

            decimal caixaFin = resultado.Value;
            numCaixaFinal.Value = caixaFin;

            var erro = _bll.Fechar(caixaFin, obs);
            if (!string.IsNullOrEmpty(erro))
            {
                MessageBox.Show(erro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Logger.Log("frmTurno", "BtnFechar_Click", $"Turno fechado | Caixa final: R$ {caixaFin:N2}");
            MessageBox.Show("Turno fechado com sucesso!", "Turno", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AtualizarEstado();
        }

        /// <summary>
        /// Diálogo único de fechamento: resumo de pagamentos + campo editável Caixa Final.
        /// Se a diferença for > 0 exige autorização antes de confirmar.
        /// Retorna null se cancelado, ou o valor de caixa final confirmado.
        /// </summary>
        private decimal? MostrarDialogFechamento(decimal esperado, decimal informadoInicial,
            (decimal totalVendas, decimal totalDin, decimal totalCarCred, decimal totalCarDeb, decimal totalPix, int qtdPedidos) resumo)
        {
            // ── Cores do sistema ─────────────────────────────────────────────
            Color clrBg     = Color.FromArgb(245, 237, 216);
            Color clrHeader = Color.FromArgb(176, 110, 42);
            Color clrText   = Color.FromArgb(50, 40, 25);
            Color clrCap    = Color.FromArgb(100, 80, 50);
            Color clrFoot   = Color.FromArgb(235, 226, 208);
            Color clrRed    = Color.FromArgb(192, 57, 43);
            Color clrGreen  = Color.FromArgb(39, 130, 57);
            Color clrOrange = Color.FromArgb(224, 113, 42);
            Color clrBtnSec = Color.FromArgb(120, 100, 68);

            using var dlg = new Form();
            dlg.Text            = $"Fechar Turno #{_turnoAtivo?.Codigo}";
            dlg.StartPosition   = FormStartPosition.CenterParent;
            dlg.Size            = new Size(480, 570);
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox     = dlg.MinimizeBox = false;
            dlg.BackColor       = clrBg;

            // ── Cabeçalho ────────────────────────────────────────────────────
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = clrHeader };
            var lblTit = new Label
            {
                Text      = $"\U0001F512  Fechamento de Turno #{_turnoAtivo?.Codigo}",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 11F, FontStyle.Bold),
                Dock      = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter
            };
            pnlTop.Controls.Add(lblTit);

            // ── Linhas de informação ─────────────────────────────────────────
            int y = 60;
            void InfoRow(string cap, string val, Color? valClr = null)
            {
                dlg.Controls.Add(new Label
                {
                    Text = cap, Left = 24, Top = y, AutoSize = true,
                    ForeColor = clrCap, Font = new Font("Segoe UI", 9F)
                });
                dlg.Controls.Add(new Label
                {
                    Text = val, Left = 270, Top = y, AutoSize = true,
                    ForeColor = valClr ?? clrText,
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
                });
                y += 26;
            }
            void Sep()
            {
                dlg.Controls.Add(new Label
                {
                    Left = 24, Top = y, Width = 420, Height = 1,
                    BackColor = Color.FromArgb(210, 195, 170)
                });
                y += 8;
            }

            InfoRow($"Pedidos ({resumo.qtdPedidos}):", $"Total Vendas: R$ {resumo.totalVendas:N2}");
            InfoRow("  Dinheiro:",  $"R$ {resumo.totalDin:N2}");
            InfoRow("  Cr\u00e9dito:", $"R$ {resumo.totalCarCred:N2}");
            InfoRow("  D\u00e9bito:", $"R$ {resumo.totalCarDeb:N2}");
            InfoRow("  Pix:",       $"R$ {resumo.totalPix:N2}");
            Sep();
            InfoRow("Caixa esperado (Ini + Vendas):", $"R$ {esperado:N2}");

            // ── Campos editáveis: Dinheiro, Cartão, Pix recebidos ────────────
            y += 4;
            NumericUpDown MkNum(decimal val)
            {
                var n = new NumericUpDown
                {
                    Left = 270, Top = y, Width = 160, Height = 24,
                    DecimalPlaces = 2, Minimum = 0, Maximum = 9999999,
                    Value = val > 9999999 ? 9999999 : val,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    BackColor = Color.White, ForeColor = clrText,
                    BorderStyle = BorderStyle.FixedSingle, ThousandsSeparator = true
                };
                return n;
            }

            dlg.Controls.Add(new Label { Text = "Dinheiro recebido:", Left = 24, Top = y + 3,
                AutoSize = true, ForeColor = clrCap, Font = new Font("Segoe UI", 9F, FontStyle.Bold) });
            var numDin = MkNum(resumo.totalDin);
            dlg.Controls.Add(numDin);
            y += 30;

            dlg.Controls.Add(new Label { Text = "Cr\u00e9dito recebido:", Left = 24, Top = y + 3,
                AutoSize = true, ForeColor = clrCap, Font = new Font("Segoe UI", 9F, FontStyle.Bold) });
            var numCred = MkNum(resumo.totalCarCred);
            dlg.Controls.Add(numCred);
            y += 30;

            dlg.Controls.Add(new Label { Text = "D\u00e9bito recebido:", Left = 24, Top = y + 3,
                AutoSize = true, ForeColor = clrCap, Font = new Font("Segoe UI", 9F, FontStyle.Bold) });
            var numDeb = MkNum(resumo.totalCarDeb);
            dlg.Controls.Add(numDeb);
            y += 30;

            dlg.Controls.Add(new Label { Text = "Pix recebido:", Left = 24, Top = y + 3,
                AutoSize = true, ForeColor = clrCap, Font = new Font("Segoe UI", 9F, FontStyle.Bold) });
            var numPix = MkNum(resumo.totalPix);
            dlg.Controls.Add(numPix);
            y += 32;
            Sep();

            // Total informado (dinâmico)
            var lblTotalCap = new Label { Text = "Total informado:", Left = 24, Top = y,
                AutoSize = true, ForeColor = clrCap, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            var lblTotalVal = new Label { Left = 270, Top = y, AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = clrText };
            dlg.Controls.Add(lblTotalCap);
            dlg.Controls.Add(lblTotalVal);
            y += 26;
            Sep();

            // ── Linha de diferença (atualiza ao digitar) ─────────────────────
            var lblDifCap = new Label
            {
                Text = "Diferen\u00e7a:", Left = 24, Top = y, AutoSize = true,
                ForeColor = clrCap, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            var lblDifVal = new Label
            {
                Left = 270, Top = y, AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            dlg.Controls.Add(lblDifCap);
            dlg.Controls.Add(lblDifVal);

            // ── Rodapé com botões ────────────────────────────────────────────
            var pnlFoot = new Panel { Dock = DockStyle.Bottom, Height = 54, BackColor = clrFoot };
            var btnCancelar = new Button
            {
                Text = "\u2715  Cancelar", Left = 16, Top = 10, Width = 130, Height = 32,
                BackColor = clrBtnSec, ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (_, __) => { dlg.DialogResult = DialogResult.Cancel; };

            var btnConfirmar = new Button
            {
                Left = 296, Top = 10, Width = 168, Height = 32,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnConfirmar.FlatAppearance.BorderSize = 0;

            pnlFoot.Controls.Add(btnCancelar);
            pnlFoot.Controls.Add(btnConfirmar);
            dlg.Controls.Add(pnlFoot);
            dlg.Controls.Add(pnlTop);
            dlg.AcceptButton = btnConfirmar;

            // ── Atualização em tempo real da diferença ───────────────────────
            void AtualizarDif()
            {
                decimal total = numDin.Value + numCred.Value + numDeb.Value + numPix.Value;
                lblTotalVal.Text = $"R$ {total:N2}";
                decimal dif = esperado - total;
                if (Math.Abs(dif) <= 0.01m)
                {
                    lblDifVal.Text      = "Sem diferen\u00e7a \u2714";
                    lblDifVal.ForeColor = clrGreen;
                    btnConfirmar.BackColor = clrOrange;
                    btnConfirmar.Text   = "Confirmar Fechamento  \u2192";
                }
                else
                {
                    lblDifVal.Text      = $"R$ {dif:N2}  \u26a0 requer autoriza\u00e7\u00e3o";
                    lblDifVal.ForeColor = clrRed;
                    btnConfirmar.BackColor = clrRed;
                    btnConfirmar.Text   = "Confirmar c/ Autoriza\u00e7\u00e3o  \u2192";
                }
            }
            numDin.ValueChanged += (_, __) => AtualizarDif();
            numCred.ValueChanged += (_, __) => AtualizarDif();
            numDeb.ValueChanged  += (_, __) => AtualizarDif();
            numPix.ValueChanged += (_, __) => AtualizarDif();
            AtualizarDif();

            // ── Lógica do botão confirmar ─────────────────────────────────────
            btnConfirmar.Click += (_, __) =>
            {
                decimal total = numDin.Value + numCred.Value + numDeb.Value + numPix.Value;
                decimal dif   = esperado - total;
                if (Math.Abs(dif) > 0.01m)
                {
                    if (!MostrarDialogAutorizacao()) return;
                }
                dlg.DialogResult = DialogResult.OK;
            };

            if (dlg.ShowDialog(this) != DialogResult.OK) return null;
            return numDin.Value + numCred.Value + numDeb.Value + numPix.Value;
        }

        private bool MostrarDialogAutorizacao()
        {
            // ── Cores do sistema ─────────────────────────────────────────────
            Color clrBg     = Color.FromArgb(245, 237, 216);
            Color clrHeader = Color.FromArgb(176, 110, 42);
            Color clrText   = Color.FromArgb(50, 40, 25);
            Color clrCap    = Color.FromArgb(100, 80, 50);
            Color clrFoot   = Color.FromArgb(235, 226, 208);
            Color clrRed    = Color.FromArgb(192, 57, 43);
            Color clrGreen  = Color.FromArgb(87, 120, 38);
            Color clrBtnSec = Color.FromArgb(120, 100, 68);

            using var dlg = new Form();
            dlg.Text            = "Autoriza\u00e7\u00e3o para Fechamento";
            dlg.StartPosition   = FormStartPosition.CenterParent;
            dlg.Size            = new Size(420, 310);
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox     = dlg.MinimizeBox = false;
            dlg.BackColor       = clrBg;

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = clrHeader };
            var lblTit = new Label
            {
                Text      = "\U0001F512  Informe usu\u00e1rio e senha para fechar",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 10F, FontStyle.Bold),
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlTop.Controls.Add(lblTit);

            var lblLogin = new Label { Text = "Usu\u00e1rio:", Left = 30, Top = 66, AutoSize = true,
                ForeColor = clrCap, Font = new Font("Segoe UI", 9F) };
            var txtLogin = new TextBox { Left = 30, Top = 84, Width = 340, Height = 26,
                BackColor = Color.White, ForeColor = clrText,
                Font = new Font("Segoe UI", 10F), BorderStyle = BorderStyle.FixedSingle };
            var lblNomeUsu = new Label { Left = 30, Top = 114, Width = 340, Height = 18,
                Font = new Font("Segoe UI", 8.5F), Text = "" };

            var lblSenha = new Label { Text = "Senha:", Left = 30, Top = 136, AutoSize = true,
                ForeColor = clrCap, Font = new Font("Segoe UI", 9F) };
            var txtSenha = new TextBox { Left = 30, Top = 154, Width = 340, Height = 26,
                BackColor = Color.White, ForeColor = clrText,
                Font = new Font("Segoe UI", 10F), BorderStyle = BorderStyle.FixedSingle, PasswordChar = '\u25cf' };

            var lblErro = new Label { Left = 30, Top = 188, Width = 340, Height = 20,
                ForeColor = clrRed, Font = new Font("Segoe UI", 8.5F), Text = "" };

            // ── Resolve login por código ou nome (igual à tela de login) ─────
            var _bllUsu = new BLL.UsuarioBLL();
            bool ResolverLogin()
            {
                lblNomeUsu.Text = "";
                var texto = txtLogin.Text.Trim();
                if (string.IsNullOrEmpty(texto)) return false;
                try
                {
                    string nome = "";
                    if (int.TryParse(texto, out int cod) && cod > 0)
                        nome = _bllUsu.BuscarNomePorCodigo(cod);
                    if (string.IsNullOrEmpty(nome))
                        nome = _bllUsu.BuscarNomePorLogin(texto);
                    if (!string.IsNullOrEmpty(nome))
                    {
                        txtLogin.Text           = nome;
                        txtLogin.SelectionStart = nome.Length;
                        lblNomeUsu.ForeColor    = Color.FromArgb(39, 130, 57);
                        lblNomeUsu.Text         = "\u2713 Usu\u00e1rio identificado";
                        return true;
                    }
                    lblNomeUsu.ForeColor = clrRed;
                    lblNomeUsu.Text      = "Usu\u00e1rio n\u00e3o encontrado";
                    return false;
                }
                catch { return false; }
            }
            txtLogin.Leave    += (_, __) => ResolverLogin();
            txtLogin.KeyDown  += (_, e2) =>
            {
                if (e2.KeyCode == Keys.Enter)
                {
                    if (ResolverLogin()) txtSenha.Focus();
                    e2.Handled = true; e2.SuppressKeyPress = true;
                }
            };

            var pnlFoot = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = clrFoot };
            var btnVoltar = new Button
            {
                Text = "\u21E6 Voltar", Left = 16, Top = 9, Width = 120, Height = 30,
                BackColor = clrBtnSec, ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnVoltar.FlatAppearance.BorderSize = 0;
            btnVoltar.Click += (_, __) => { dlg.DialogResult = DialogResult.Cancel; };

            var btnOk = new Button
            {
                Text = "Confirmar", Left = 284, Top = 9, Width = 110, Height = 30,
                BackColor = clrGreen, ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += (_, __) =>
            {
                lblErro.Text = "";
                var usuario = _bllUsu.Autenticar(txtLogin.Text.Trim(), txtSenha.Text);
                if (usuario == null)
                {
                    lblErro.Text = "Usu\u00e1rio ou senha incorretos.";
                    txtSenha.Clear();
                    txtSenha.Focus();
                    return;
                }
                dlg.DialogResult = DialogResult.OK;
            };
            txtSenha.KeyDown += (_, e2) => { if (e2.KeyCode == Keys.Enter) btnOk.PerformClick(); };

            pnlFoot.Controls.Add(btnVoltar);
            pnlFoot.Controls.Add(btnOk);
            dlg.Controls.Add(lblErro);
            dlg.Controls.Add(txtSenha);
            dlg.Controls.Add(lblSenha);
            dlg.Controls.Add(lblNomeUsu);
            dlg.Controls.Add(txtLogin);
            dlg.Controls.Add(lblLogin);
            dlg.Controls.Add(pnlFoot);
            dlg.Controls.Add(pnlTop);
            dlg.AcceptButton = btnOk;
            dlg.Shown += (_, __) => txtLogin.Focus();

            return dlg.ShowDialog(this) == DialogResult.OK;
        }

        private void BtnRelatorio_Click(object sender, EventArgs e)
        {
            if (gridHistorico.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um turno na lista para ver as movimenta\u00e7\u00f5es.",
                                "Turno", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var row = gridHistorico.SelectedRows[0].DataBoundItem as System.Data.DataRowView;
            if (row == null) return;
            int cod = Convert.ToInt32(row.Row["Codigo"]);

            // Recarrega o modelo completo para o relatório
            var todos = _bll.Listar(new DateTime(2000, 1, 1), DateTime.Today.AddDays(1));
            Modelo.Turno turno = null;
            foreach (System.Data.DataRow r in todos.Rows)
            {
                if (Convert.ToInt32(r["Codigo"]) == cod)
                {
                    turno = new Modelo.Turno
                    {
                        Codigo          = cod,
                        turAbertura     = Convert.ToDateTime(r["turAbertura"]),
                        turFechamento   = r["turFechamento"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["turFechamento"]),
                        turUsuario      = r["turUsuario"]?.ToString() ?? "",
                        turCaixa_Inicial = Convert.ToDecimal(r["turCaixa_Inicial"]),
                        turCaixa_Final  = r["turCaixa_Final"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["turCaixa_Final"]),
                        turObservacao   = r["turObservacao"]?.ToString() ?? "",
                        turSituacao     = r["turSituacao"]?.ToString()?[0] ?? 'F'
                    };
                    break;
                }
            }
            if (turno == null) return;
            using var frm = new frmRelatorioTurno(turno);
            frm.ShowDialog(this);
        }

        private void BtnFiltrar_Click(object sender, EventArgs e)
        {
            CarregarHistorico();
        }

        private void CarregarHistorico()
        {
            try
            {
                var de  = dtpDe.Value.Date;
                var ate = dtpAte.Value.Date;
                gridHistorico.DataSource = _bll.Listar(de, ate);
                ConfigurarColunas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar hist\u00f3rico: " + ex.Message);
            }
        }

        private void ConfigurarColunas()
        {
            if (gridHistorico.Columns.Count == 0) return;

            var caps = new System.Collections.Generic.Dictionary<string, string>
            {
                ["Codigo"]           = "N\u00ba",
                ["turAbertura"]      = "Abertura",
                ["turFechamento"]    = "Fechamento",
                ["turUsuario"]       = "Usu\u00e1rio",
                ["turCaixa_Inicial"] = "Caixa Inicial",
                ["turCaixa_Final"]   = "Caixa Final",
                ["turObservacao"]    = "Observa\u00e7\u00e3o",
                ["turSituacao"]      = "Sit.",
            };
            foreach (var kv in caps)
                if (gridHistorico.Columns.Contains(kv.Key))
                    gridHistorico.Columns[kv.Key].HeaderText = kv.Value;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
