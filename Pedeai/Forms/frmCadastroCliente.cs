using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.DB;

namespace Pedeai.Forms
{
    public class frmCadastroCliente : Form
    {
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
            pnlForm = new Panel { Dock = DockStyle.Bottom, Height = 220, Padding = new Padding(10), Visible = false };
            pnlForm.BackColor = Color.FromArgb(245, 245, 250);
            pnlForm.BorderStyle = BorderStyle.FixedSingle;

            int y = 8;
            txtNome = Campo(pnlForm, "Nome / Razão Social:", 10, y, 250); Campo2(pnlForm, "CPF/CNPJ:", 275, y); txtCpf = new TextBox { Left = 350, Top = y, Width = 130 }; pnlForm.Controls.Add(txtCpf);
            Campo2(pnlForm, "Situação:", 495, y); cmbSituacao = new ComboBox { Left = 555, Top = y, Width = 55, DropDownStyle = ComboBoxStyle.DropDownList }; cmbSituacao.Items.AddRange(new[] { "NORMAL", "INATIVO" }); cmbSituacao.SelectedIndex = 0; pnlForm.Controls.Add(cmbSituacao);

            y += 32;
            txtTelefone = Campo(pnlForm, "Telefone:", 10, y, 120); txtCelular = Campo(pnlForm, "Celular:", 145, y, 120); txtEmail = Campo(pnlForm, "Email:", 280, y, 230);

            y += 32;
            txtCep = Campo(pnlForm, "CEP:", 10, y, 80); txtEndereco = Campo(pnlForm, "Endereço:", 105, y, 200); txtNumero = Campo(pnlForm, "Nº:", 320, y, 60); txtComplemento = Campo(pnlForm, "Compl.:", 395, y, 120);

            y += 32;
            txtBairro = Campo(pnlForm, "Bairro:", 10, y, 160); txtCidade = Campo(pnlForm, "Cidade:", 185, y, 180); txtEstado = Campo(pnlForm, "UF:", 380, y, 40);

            y += 38;
            var btnS = Botao("Salvar", Color.FromArgb(33, 150, 243)); btnS.Left = 10; btnS.Top = y; btnS.Click += BtnSalvar_Click; pnlForm.Controls.Add(btnS);
            var btnC = Botao("Cancelar", Color.FromArgb(158, 158, 158)); btnC.Left = 120; btnC.Top = y; btnC.Click += (_, __) => { pnlForm.Visible = false; _codigoEditando = 0; }; pnlForm.Controls.Add(btnC);

            Controls.Add(grid);
            Controls.Add(topBar);
            Controls.Add(pnlForm);
        }

        private TextBox Campo(Panel p, string label, int x, int y, int w)
        {
            p.Controls.Add(new Label { Text = label, Left = x, Top = y + 3, AutoSize = true });
            var t = new TextBox { Left = x + label.Length * 6, Top = y, Width = w };
            p.Controls.Add(t); return t;
        }

        private void Campo2(Panel p, string label, int x, int y)
        {
            p.Controls.Add(new Label { Text = label, Left = x, Top = y + 3, AutoSize = true });
        }

        private Button Botao(string texto, Color cor)
        {
            return new Button { Text = texto, BackColor = cor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 100, Height = 28, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
        }

        private void CarregarGrid()
        {
            try { grid.DataSource = DbHelper.ListarClientes(txtBusca?.Text?.Trim() ?? ""); }
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
            var row = DbHelper.GetCliente(cod);
            if (row == null) return;
            _codigoEditando = cod;
            txtNome.Text = row["clieNome_RazaoSocial"]?.ToString() ?? "";
            txtTelefone.Text = row["clieTelefone"]?.ToString() ?? "";
            txtCelular.Text = row["clieCelular"]?.ToString() ?? "";
            txtEmail.Text = row["clieEmail"]?.ToString() ?? "";
            txtCpf.Text = row["clieCPF_CNPJ_"]?.ToString() ?? "";
            txtCep.Text = row["clieCEP"]?.ToString() ?? "";
            txtEndereco.Text = row["clieEndereco"]?.ToString() ?? "";
            txtNumero.Text = row["clieNumero"]?.ToString() ?? "";
            txtComplemento.Text = row["clieComplemento"]?.ToString() ?? "";
            txtBairro.Text = row["clieBairro"]?.ToString() ?? "";
            txtCidade.Text = row["clieCidade"]?.ToString() ?? "";
            txtEstado.Text = row["clieEstado"]?.ToString() ?? "";
            var sit = row["Situacao"]?.ToString() ?? "NORMAL";
            cmbSituacao.SelectedItem = sit;
            pnlForm.Visible = true; txtNome.Focus();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text)) { MessageBox.Show("Informe o nome."); return; }
            var erro = DbHelper.SalvarCliente(_codigoEditando, txtNome.Text.Trim(),
                txtTelefone.Text, txtCelular.Text, txtEmail.Text, txtCpf.Text,
                txtCep.Text, txtEndereco.Text, txtNumero.Text,
                txtComplemento.Text, txtBairro.Text, txtCidade.Text, txtEstado.Text,
                cmbSituacao.SelectedItem?.ToString() ?? "NORMAL");
            if (erro != "") { MessageBox.Show("Erro: " + erro); return; }
            pnlForm.Visible = false; _codigoEditando = 0; CarregarGrid();
        }
    }
}
