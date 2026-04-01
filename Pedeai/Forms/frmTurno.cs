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

            Logger.Log("Turno aberto", $"Caixa inicial: R$ {caixaIni:N2}");
            MessageBox.Show("Turno aberto com sucesso!", "Turno", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AtualizarEstado();
        }

        private void BtnFechar_Click(object sender, EventArgs e)
        {
            decimal caixaFin = numCaixaFinal.Value;
            string obs       = txtObsFechar.Text.Trim();

            decimal movimentado = _bll.GetTotalMovimentado(_turnoAtivo);
            decimal esperado    = _turnoAtivo.turCaixa_Inicial + movimentado;
            decimal diferenca   = esperado - caixaFin;

            if (diferenca > 0.01m)
            {
                bool autorizado = MostrarDialogDiferencaCaixa(esperado, caixaFin, diferenca);
                if (!autorizado) return;
            }
            else
            {
                if (MessageBox.Show(
                        $"Confirma o fechamento do turno #{_turnoAtivo?.Codigo}?\n\nCaixa final: R$ {caixaFin:N2}",
                        "Fechar Turno", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
            }

            var erro = _bll.Fechar(caixaFin, obs);
            if (!string.IsNullOrEmpty(erro))
            {
                MessageBox.Show(erro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Logger.Log("Turno fechado", $"Caixa final: R$ {caixaFin:N2}");
            MessageBox.Show("Turno fechado com sucesso!", "Turno", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AtualizarEstado();
        }

        private bool MostrarDialogDiferencaCaixa(decimal esperado, decimal informado, decimal diferenca)
        {
            using var dlg = new Form();
            dlg.Text            = "Diferen\u00e7a de Caixa";
            dlg.StartPosition   = FormStartPosition.CenterParent;
            dlg.Size            = new Size(460, 310);
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox     = dlg.MinimizeBox = false;
            dlg.BackColor       = Color.FromArgb(28, 37, 65);

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = Color.FromArgb(180, 50, 30) };
            var lblTit = new Label
            {
                Text      = "\u26A0  DIFEREN\u00c7A NO FECHAMENTO",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 11F, FontStyle.Bold),
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlTop.Controls.Add(lblTit);

            int y = 64;
            void AddRow(string lbl, string val, Color valColor)
            {
                dlg.Controls.Add(new Label { Text = lbl, Left = 30, Top = y, AutoSize = true,
                    ForeColor = Color.FromArgb(150, 175, 220), Font = new Font("Segoe UI", 9.5F) });
                dlg.Controls.Add(new Label { Text = val, Left = 250, Top = y, AutoSize = true,
                    ForeColor = valColor, Font = new Font("Segoe UI", 10F, FontStyle.Bold) });
                y += 34;
            }
            AddRow("Caixa esperado (Ini + Vendas):", $"R$ {esperado:N2}",  Color.White);
            AddRow("Caixa informado:",               $"R$ {informado:N2}", Color.White);
            AddRow("Diferen\u00e7a:",                $"R$ {diferenca:N2}", Color.FromArgb(231, 76, 60));

            var pnlBtns = new Panel { Dock = DockStyle.Bottom, Height = 58, BackColor = Color.FromArgb(22, 30, 55) };

            var btnVoltar = new Button
            {
                Text      = "\u21E6  Voltar e corrigir",
                Left = 16, Top = 12, Width = 180, Height = 34,
                BackColor = Color.FromArgb(52, 68, 105), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnVoltar.FlatAppearance.BorderSize = 0;
            btnVoltar.Click += (_, __) => { dlg.DialogResult = DialogResult.Cancel; };

            var btnConfirmar = new Button
            {
                Text      = "Confirmar com autoriza\u00e7\u00e3o  \u2192",
                Left = 214, Top = 12, Width = 220, Height = 34,
                BackColor = Color.FromArgb(167, 50, 30), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnConfirmar.FlatAppearance.BorderSize = 0;
            btnConfirmar.Click += (_, __) =>
            {
                if (MostrarDialogAutorizacao()) dlg.DialogResult = DialogResult.OK;
            };

            pnlBtns.Controls.Add(btnVoltar);
            pnlBtns.Controls.Add(btnConfirmar);
            dlg.Controls.Add(pnlBtns);
            dlg.Controls.Add(pnlTop);

            return dlg.ShowDialog(this) == DialogResult.OK;
        }

        private bool MostrarDialogAutorizacao()
        {
            using var dlg = new Form();
            dlg.Text            = "Autoriza\u00e7\u00e3o para Fechamento";
            dlg.StartPosition   = FormStartPosition.CenterParent;
            dlg.Size            = new Size(420, 290);
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox     = dlg.MinimizeBox = false;
            dlg.BackColor       = Color.FromArgb(28, 37, 65);

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = Color.FromArgb(52, 68, 105) };
            var lblTit = new Label
            {
                Text      = "\U0001F512  Informe usu\u00e1rio e senha para fechar",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 10F, FontStyle.Bold),
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlTop.Controls.Add(lblTit);

            Color clrLbl = Color.FromArgb(150, 175, 220);
            var lblLogin = new Label { Text = "Usu\u00e1rio:", Left = 30, Top = 72, AutoSize = true,
                ForeColor = clrLbl, Font = new Font("Segoe UI", 9.5F) };
            var txtLogin = new TextBox { Left = 30, Top = 92, Width = 340, Height = 26,
                BackColor = Color.FromArgb(14, 21, 46), ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F), BorderStyle = BorderStyle.FixedSingle };
            var lblSenha = new Label { Text = "Senha:", Left = 30, Top = 126, AutoSize = true,
                ForeColor = clrLbl, Font = new Font("Segoe UI", 9.5F) };
            var txtSenha = new TextBox { Left = 30, Top = 146, Width = 340, Height = 26,
                BackColor = Color.FromArgb(14, 21, 46), ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F), BorderStyle = BorderStyle.FixedSingle, PasswordChar = '\u25cf' };
            var lblErro = new Label { Left = 30, Top = 182, Width = 340, Height = 20,
                ForeColor = Color.FromArgb(231, 76, 60), Font = new Font("Segoe UI", 8.5F), Text = "" };

            var pnlBtns = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = Color.FromArgb(22, 30, 55) };
            var btnVoltar = new Button
            {
                Text = "\u21E6 Voltar", Left = 16, Top = 10, Width = 120, Height = 30,
                BackColor = Color.FromArgb(52, 68, 105), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btnVoltar.FlatAppearance.BorderSize = 0;
            btnVoltar.Click += (_, __) => { dlg.DialogResult = DialogResult.Cancel; };

            var btnOk = new Button
            {
                Text = "Confirmar", Left = 284, Top = 10, Width = 110, Height = 30,
                BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += (_, __) =>
            {
                var bll     = new BLL.UsuarioBLL();
                var usuario = bll.Autenticar(txtLogin.Text.Trim(), txtSenha.Text);
                if (usuario == null)
                {
                    lblErro.Text = "Usu\u00e1rio ou senha incorretos.";
                    return;
                }
                dlg.DialogResult = DialogResult.OK;
            };

            pnlBtns.Controls.Add(btnVoltar);
            pnlBtns.Controls.Add(btnOk);
            dlg.Controls.Add(lblErro);
            dlg.Controls.Add(txtSenha);
            dlg.Controls.Add(lblSenha);
            dlg.Controls.Add(txtLogin);
            dlg.Controls.Add(lblLogin);
            dlg.Controls.Add(pnlBtns);
            dlg.Controls.Add(pnlTop);
            dlg.AcceptButton = btnOk;

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
    }
}
