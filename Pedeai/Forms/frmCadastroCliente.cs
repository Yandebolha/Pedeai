using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public class frmCadastroCliente : Form
    {
        private readonly ClienteBLL _bll = new ClienteBLL();
        private DataGridView grid = new DataGridView();
        private Panel pnlForm = new Panel();
        private TextBox txtBusca = new TextBox();
        private TextBox txtNome, txtTelefone, txtCelular, txtEmail, txtCpf;
        private TextBox txtCep, txtEndereco, txtNumero, txtComplemento, txtBairro, txtCidade, txtEstado;
        private ComboBox cmbSituacao = new ComboBox();
        private int _codigoEditando = 0;

        public frmCadastroCliente()
        {
            Text = "Cadastro de Clientes";
            Size = new Size(950, 620);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(860, 540);
            Font = new Font("Segoe UI", 9);
            BuildUI();
            CarregarGrid();
        }

        private void BuildUI()
        {
            var topBar = new Panel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(8, 6, 8, 0) };
            topBar.BackColor = Color.FromArgb(40, 40, 80);
            var lblB = new Label { Text = "Buscar:", ForeColor = Color.White, Left = 8, Top = 12, AutoSize = true };
            txtBusca = new TextBox { Left = 65, Top = 8, Width = 220 };
            txtBusca.KeyDown += (_, k) => { if (k.KeyCode == Keys.Enter) CarregarGrid(); };
            var btnB = Botao("Buscar", Color.FromArgb(63, 81, 181)); btnB.Left = 295; btnB.Top = 8; btnB.Click += (_, __) => CarregarGrid();
            var btnN = Botao("+ Novo Cliente", Color.FromArgb(0, 150, 136)); btnN.Left = 405; btnN.Top = 8; btnN.Click += (_, __) => ModoNovo();
            topBar.Controls.AddRange(new Control[] { lblB, txtBusca, btnB, btnN });

            grid.Dock = DockStyle.Fill;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly = true; grid.AllowUserToAddRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false; grid.BackgroundColor = Color.White;
            grid.Font = new Font("Segoe UI", 9); grid.BorderStyle = BorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 80);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.DoubleClick += (_, __) => CarregarParaEditar();

            // Formulário
            pnlForm = new Panel { Dock = DockStyle.Bottom, Height = 195, Padding = new Padding(10), Visible = false };
            pnlForm.BackColor = Color.FromArgb(245, 245, 250);
            pnlForm.BorderStyle = BorderStyle.FixedSingle;

            // ── Linha 1: Nome | CPF | Situação ────────────────────────────
            Lbl(pnlForm, "Nome / Razão Social:", 10, 11);
            txtNome = Txt(pnlForm, 150, 8, 260);

            Lbl(pnlForm, "CPF / CNPJ:", 422, 11);
            txtCpf = Txt(pnlForm, 500, 8, 140);

            Lbl(pnlForm, "Situação:", 654, 11);
            cmbSituacao = new ComboBox { Left = 714, Top = 8, Width = 100, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSituacao.Items.AddRange(new[] { "NORMAL", "INATIVO" });
            cmbSituacao.SelectedIndex = 0;
            pnlForm.Controls.Add(cmbSituacao);

            // ── Linha 2: Telefone | Celular | E-mail ──────────────────────
            Lbl(pnlForm, "Telefone:", 10, 43);
            txtTelefone = Txt(pnlForm, 72, 40, 130);

            Lbl(pnlForm, "Celular:", 214, 43);
            txtCelular = Txt(pnlForm, 265, 40, 130);

            Lbl(pnlForm, "E-mail:", 407, 43);
            txtEmail = Txt(pnlForm, 450, 40, 264);

            // ── Linha 3: CEP | Endereço | Nº | Compl. ───────────────────
            Lbl(pnlForm, "CEP:", 10, 75);
            txtCep = Txt(pnlForm, 42, 72, 80);

            Lbl(pnlForm, "Endereço:", 134, 75);
            txtEndereco = Txt(pnlForm, 200, 72, 230);

            Lbl(pnlForm, "Nº:", 442, 75);
            txtNumero = Txt(pnlForm, 462, 72, 60);

            Lbl(pnlForm, "Compl.:", 534, 75);
            txtComplemento = Txt(pnlForm, 580, 72, 134);

            // ── Linha 4: Bairro | Cidade | UF ───────────────────────────
            Lbl(pnlForm, "Bairro:", 10, 107);
            txtBairro = Txt(pnlForm, 55, 104, 190);

            Lbl(pnlForm, "Cidade:", 257, 107);
            txtCidade = Txt(pnlForm, 305, 104, 190);

            Lbl(pnlForm, "UF:", 507, 107);
            txtEstado = Txt(pnlForm, 527, 104, 50);

            // ── Botões ───────────────────────────────────────────────────
            var btnS = Botao("Salvar", Color.FromArgb(33, 150, 243));
            btnS.Left = 10; btnS.Top = 140; btnS.Click += BtnSalvar_Click;
            pnlForm.Controls.Add(btnS);

            var btnC = Botao("Cancelar", Color.FromArgb(158, 158, 158));
            btnC.Left = 120; btnC.Top = 140;
            btnC.Click += (_, __) => { pnlForm.Visible = false; _codigoEditando = 0; };
            pnlForm.Controls.Add(btnC);

            Controls.Add(grid);
            Controls.Add(topBar);
            Controls.Add(pnlForm);
        }

        private static Label Lbl(Panel p, string text, int x, int y)
        {
            var l = new Label { Text = text, Left = x, Top = y, AutoSize = true };
            p.Controls.Add(l); return l;
        }

        private static TextBox Txt(Panel p, int x, int y, int w)
        {
            var t = new TextBox { Left = x, Top = y, Width = w };
            p.Controls.Add(t); return t;
        }

        private Button Botao(string texto, Color cor)
        {
            return new Button { Text = texto, BackColor = cor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 100, Height = 28, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
        }

        private void CarregarGrid()
        {
            try { grid.DataSource = _bll.Listar(txtBusca?.Text?.Trim() ?? ""); }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void ModoNovo()
        {
            _codigoEditando = 0;
            foreach (var c in pnlForm.Controls)
                if (c is TextBox tb) tb.Clear();
            cmbSituacao.SelectedIndex = 0;
            pnlForm.Visible = true; txtNome.Focus();
        }

        private void CarregarParaEditar()
        {
            if (grid.SelectedRows.Count == 0) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var obj = _bll.PesquisaCodigo(cod);
            if (obj == null) return;
            _codigoEditando = cod;
            txtNome.Text        = obj.clieNome_RazaoSocial ?? "";
            txtTelefone.Text    = obj.clieTelefone ?? "";
            txtCelular.Text     = obj.clieCelular ?? "";
            txtEmail.Text       = obj.clieEmail ?? "";
            txtCpf.Text         = obj.clieCPF_CNPJ_ ?? "";
            txtCep.Text         = obj.clieCEP ?? "";
            txtEndereco.Text    = obj.clieEndereco ?? "";
            txtNumero.Text      = obj.clieNumero ?? "";
            txtComplemento.Text = obj.clieComplemento ?? "";
            txtBairro.Text      = obj.clieBairro ?? "";
            txtCidade.Text      = obj.clieCidade ?? "";
            txtEstado.Text      = obj.clieEstado ?? "";
            cmbSituacao.SelectedItem = obj.Situacao ?? "NORMAL";
            pnlForm.Visible = true; txtNome.Focus();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text)) { MessageBox.Show("Informe o nome."); return; }
            var obj = new Cliente
            {
                Codigo               = _codigoEditando,
                clieNome_RazaoSocial = txtNome.Text.Trim(),
                clieTelefone         = txtTelefone.Text,
                clieCelular          = txtCelular.Text,
                clieEmail            = txtEmail.Text,
                clieCPF_CNPJ_        = txtCpf.Text,
                clieCEP              = txtCep.Text,
                clieEndereco         = txtEndereco.Text,
                clieNumero           = txtNumero.Text,
                clieComplemento      = txtComplemento.Text,
                clieBairro           = txtBairro.Text,
                clieCidade           = txtCidade.Text,
                clieEstado           = txtEstado.Text,
                Situacao             = cmbSituacao.SelectedItem?.ToString() ?? "NORMAL",
            };
            var erro = _bll.Salvar(obj);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            pnlForm.Visible = false; _codigoEditando = 0; CarregarGrid();
        }
    }
}
