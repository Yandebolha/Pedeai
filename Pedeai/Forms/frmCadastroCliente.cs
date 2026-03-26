using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmCadastroCliente : Form
    {
        private readonly ClienteBLL _bll = new ClienteBLL();
        private int _codigoEditando = 0;
        private bool _formatingCpf = false;

        public frmCadastroCliente()
        {
            InitializeComponent();
            BuildUI();
            if (!DesignMode) CarregarGrid();
        }

        private void BuildUI()
        {
            grid        = new DataGridView();
            pnlForm     = new Panel();
            txtBusca    = new TextBox();
            txtNome     = new TextBox();
            txtEmail    = new TextBox();
            txtCpf      = new TextBox();
            txtEndereco = new TextBox();
            txtNumero   = new TextBox();
            txtComplemento = new TextBox();
            txtBairro   = new TextBox();
            txtCidade   = new TextBox();
            txtEstado   = new TextBox();
            txtTelefone = new MaskedTextBox();
            txtCelular  = new MaskedTextBox();
            txtCep      = new MaskedTextBox();
            cmbSituacao = new ComboBox();

            // ── Top bar ──────────────────────────────────────────────────
            var topBar = new Panel { Dock = DockStyle.Top, Height = 44,
                BackColor = Color.FromArgb(36, 48, 82) };
            var lblB = new Label { Text = "Buscar:", ForeColor = Color.White,
                Left = 8, Top = 12, AutoSize = true };
            txtBusca.Left = 65; txtBusca.Top = 8; txtBusca.Width = 220;
            txtBusca.BackColor = Color.FromArgb(28, 37, 65); txtBusca.ForeColor = Color.White;
            txtBusca.KeyDown += (_, k) => { if (k.KeyCode == Keys.Enter) CarregarGrid(); };
            var btnB = new Button
            {
                Text = "Buscar", Left = 295, Top = 8, Width = 100, Height = 28,
                BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnB.FlatAppearance.BorderSize = 0; btnB.Click += (_, __) => CarregarGrid();
            var btnN = new Button
            {
                Text = "+ Novo Cliente", Left = 405, Top = 8, Width = 120, Height = 28,
                BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnN.FlatAppearance.BorderSize = 0; btnN.Click += (_, __) => ModoNovo();
            topBar.Controls.AddRange(new Control[] { lblB, txtBusca, btnB, btnN });

            // ── Grid ──────────────────────────────────────────────────────
            grid.Dock = DockStyle.Fill;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly = true; grid.AllowUserToAddRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false; grid.BackgroundColor = Color.FromArgb(20, 28, 55);
            grid.DefaultCellStyle.BackColor = Color.FromArgb(20, 28, 55);
            grid.DefaultCellStyle.ForeColor = Color.White;
            grid.GridColor = Color.FromArgb(40, 55, 90);
            grid.Font = new Font("Segoe UI", 9F); grid.BorderStyle = BorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(36, 48, 82);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.DoubleClick += (_, __) => CarregarParaEditar();

            // ── Painel formulário ──────────────────────────────────────────
            pnlForm.Dock = DockStyle.Bottom; pnlForm.Height = 210;
            pnlForm.BackColor = Color.FromArgb(28, 37, 65);
            pnlForm.BorderStyle = BorderStyle.FixedSingle;
            pnlForm.Visible = false;

            var cLblCliente = Color.FromArgb(160, 175, 210);
            var cInputBg    = Color.FromArgb(20, 28, 55);

            var lblNome = new Label { Text = "Nome / Razão Social:", Left = 10, Top = 11, AutoSize = true, ForeColor = cLblCliente };
            txtNome.Left = 155; txtNome.Top = 8; txtNome.Width = 255;
            txtNome.BackColor = cInputBg; txtNome.ForeColor = Color.White;

            var lblCpf  = new Label { Text = "CPF / CNPJ:", Left = 422, Top = 11, AutoSize = true, ForeColor = cLblCliente };
            txtCpf.Left = 500; txtCpf.Top = 8; txtCpf.Width = 160;
            txtCpf.BackColor = cInputBg; txtCpf.ForeColor = Color.White;
            txtCpf.KeyPress += (s, e) => { if (!System.Char.IsDigit(e.KeyChar) && !System.Char.IsControl(e.KeyChar)) e.Handled = true; };
            txtCpf.TextChanged += TxtCpf_TextChanged;

            var lblSit  = new Label { Text = "Situação:", Left = 672, Top = 11, AutoSize = true, ForeColor = cLblCliente };
            cmbSituacao.Left = 730; cmbSituacao.Top = 8; cmbSituacao.Width = 95;
            cmbSituacao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSituacao.Items.AddRange(new object[] { "Ativo", "Inativo" });
            cmbSituacao.SelectedIndex = 0;

            var lblTel  = new Label { Text = "Telefone:", Left = 10, Top = 47, AutoSize = true, ForeColor = cLblCliente };
            txtTelefone.Mask = "(00) 0000-0000"; txtTelefone.Left = 72; txtTelefone.Top = 44; txtTelefone.Width = 130;
            txtTelefone.BackColor = cInputBg; txtTelefone.ForeColor = Color.White;

            var lblCel  = new Label { Text = "Celular:",  Left = 216, Top = 47, AutoSize = true, ForeColor = cLblCliente };
            txtCelular.Mask = "(00) 00000-0000"; txtCelular.Left = 268; txtCelular.Top = 44; txtCelular.Width = 140;
            txtCelular.BackColor = cInputBg; txtCelular.ForeColor = Color.White;

            var lblEmail = new Label { Text = "E-mail:", Left = 420, Top = 47, AutoSize = true, ForeColor = cLblCliente };
            txtEmail.Left = 462; txtEmail.Top = 44; txtEmail.Width = 265;
            txtEmail.BackColor = cInputBg; txtEmail.ForeColor = Color.White;

            var lblCep  = new Label { Text = "CEP:",     Left = 10,  Top = 83, AutoSize = true, ForeColor = cLblCliente };
            txtCep.Mask = "00000-000"; txtCep.Left = 44; txtCep.Top = 80; txtCep.Width = 95;
            txtCep.BackColor = cInputBg; txtCep.ForeColor = Color.White;
            txtCep.Leave += TxtCep_Leave;

            var lblEnd  = new Label { Text = "Endereço:", Left = 152, Top = 83, AutoSize = true, ForeColor = cLblCliente };
            txtEndereco.Left = 218; txtEndereco.Top = 80; txtEndereco.Width = 225;
            txtEndereco.BackColor = cInputBg; txtEndereco.ForeColor = Color.White;

            var lblNum  = new Label { Text = "Nº:",      Left = 456, Top = 83, AutoSize = true, ForeColor = cLblCliente };
            txtNumero.Left = 476; txtNumero.Top = 80; txtNumero.Width = 58;
            txtNumero.BackColor = cInputBg; txtNumero.ForeColor = Color.White;

            var lblComp = new Label { Text = "Compl.:",  Left = 547, Top = 83, AutoSize = true, ForeColor = cLblCliente };
            txtComplemento.Left = 594; txtComplemento.Top = 80; txtComplemento.Width = 130;
            txtComplemento.BackColor = cInputBg; txtComplemento.ForeColor = Color.White;

            var lblBai  = new Label { Text = "Bairro:",  Left = 10,  Top = 119, AutoSize = true, ForeColor = cLblCliente };
            txtBairro.Left = 58; txtBairro.Top = 116; txtBairro.Width = 188;
            txtBairro.BackColor = cInputBg; txtBairro.ForeColor = Color.White;

            var lblCid  = new Label { Text = "Cidade:",  Left = 260, Top = 119, AutoSize = true, ForeColor = cLblCliente };
            txtCidade.Left = 308; txtCidade.Top = 116; txtCidade.Width = 188;
            txtCidade.BackColor = cInputBg; txtCidade.ForeColor = Color.White;

            var lblUF   = new Label { Text = "UF:",      Left = 508, Top = 119, AutoSize = true, ForeColor = cLblCliente };
            txtEstado.Left = 528; txtEstado.Top = 116; txtEstado.Width = 50;
            txtEstado.BackColor = cInputBg; txtEstado.ForeColor = Color.White;

            var pnlBtns = new Panel { Dock = DockStyle.Bottom, Height = 48,
                BackColor = Color.FromArgb(28, 37, 65) };
            var btnS = new Button { Text = "Salvar",    Top = 10, Width = 110, Height = 28,
                BackColor = Color.FromArgb(52, 152, 219),  ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnS.FlatAppearance.BorderSize = 0; btnS.Click += BtnSalvar_Click;

            var btnC = new Button { Text = "Cancelar",  Top = 10, Width = 110, Height = 28,
                BackColor = Color.FromArgb(80, 95, 130), ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnC.FlatAppearance.BorderSize = 0;
            btnC.Click += (_, __) => { pnlForm.Visible = false; _codigoEditando = 0; };

            void CentrarBotoes()
            {
                int x = (pnlBtns.Width - 110 * 2 - 10) / 2;
                if (x < 10) x = 10;
                btnS.Left = x; btnC.Left = x + 120;
            }
            pnlBtns.SizeChanged += (_, __) => CentrarBotoes();
            pnlBtns.Controls.AddRange(new Control[] { btnS, btnC });

            pnlForm.Controls.AddRange(new Control[]
            {
                lblNome, txtNome, lblCpf, txtCpf, lblSit, cmbSituacao,
                lblTel, txtTelefone, lblCel, txtCelular, lblEmail, txtEmail,
                lblCep, txtCep, lblEnd, txtEndereco, lblNum, txtNumero, lblComp, txtComplemento,
                lblBai, txtBairro, lblCid, txtCidade, lblUF, txtEstado
            });
            pnlForm.Controls.Add(pnlBtns);

            Controls.Add(grid); Controls.Add(topBar); Controls.Add(pnlForm);
        }

        private void TxtCpf_TextChanged(object sender, EventArgs e)
        {
            if (_formatingCpf) return;
            _formatingCpf = true;
            var digits = new string(System.Array.FindAll(txtCpf.Text.ToCharArray(), char.IsDigit));
            if (digits.Length > 14) digits = digits.Substring(0, 14);
            string fmt = FormatCpfCnpj(digits);
            int caret = txtCpf.SelectionStart;
            int diff   = fmt.Length - txtCpf.Text.Length;
            txtCpf.Text = fmt;
            txtCpf.SelectionStart = Math.Min(Math.Max(caret + diff, 0), fmt.Length);
            _formatingCpf = false;
        }

        private static string FormatCpfCnpj(string d)
        {
            if (d.Length <=  3) return d;
            if (d.Length <=  6) return $"{d.Substring(0,3)}.{d.Substring(3)}";
            if (d.Length <=  9) return $"{d.Substring(0,3)}.{d.Substring(3,3)}.{d.Substring(6)}";
            if (d.Length <= 11) return $"{d.Substring(0,3)}.{d.Substring(3,3)}.{d.Substring(6,3)}-{d.Substring(9)}";
            if (d.Length <= 12) return $"{d.Substring(0,2)}.{d.Substring(2,3)}.{d.Substring(5,3)}/{d.Substring(8)}";
            return $"{d.Substring(0,2)}.{d.Substring(2,3)}.{d.Substring(5,3)}/{d.Substring(8,4)}-{d.Substring(12)}";
        }

        private async void TxtCep_Leave(object sender, EventArgs e) => await BuscarCep();

        private async Task BuscarCep()
        {
            var cep = new string(System.Array.FindAll(txtCep.Text.ToCharArray(), char.IsDigit));
            if (cep.Length != 8) return;
            try
            {
                using (var http = new HttpClient { Timeout = TimeSpan.FromSeconds(6) })
                {
                    var json = await http.GetStringAsync("https://viacep.com.br/ws/" + cep + "/json/");
                    var obj = JObject.Parse(json);
                    if (obj["erro"] == null)
                    {
                        txtEndereco.Text = obj["logradouro"]?.ToString() ?? "";
                        txtBairro.Text   = obj["bairro"]?.ToString()     ?? "";
                        txtCidade.Text   = obj["localidade"]?.ToString() ?? "";
                        txtEstado.Text   = obj["uf"]?.ToString()         ?? "";
                        txtNumero.Focus();
                    }
                }
            }
            catch { }
        }

        private void CarregarGrid()
        {
            try
            {
                grid.DataSource = _bll.Listar(txtBusca?.Text?.Trim() ?? "");
                FormatarGrid();
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void FormatarGrid()
        {
            if (grid.Columns.Count == 0) return;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            void Col(string name, string header, int width, DataGridViewContentAlignment align = DataGridViewContentAlignment.MiddleLeft)
            {
                if (!grid.Columns.Contains(name)) return;
                grid.Columns[name].HeaderText = header;
                grid.Columns[name].Width      = width;
                grid.Columns[name].DefaultCellStyle.Alignment = align;
            }

            Col("Codigo",       "Cód.",         50,  DataGridViewContentAlignment.MiddleCenter);
            Col("Nome",         "Nome / Razão Social", 200);
            Col("Telefone",     "Telefone",      110,  DataGridViewContentAlignment.MiddleCenter);
            Col("Celular",      "Celular",        125,  DataGridViewContentAlignment.MiddleCenter);
            Col("Email",        "E-mail",         180);
            Col("Cidade",       "Cidade",         120);
            Col("TotalPedidos", "Pedidos",         70,  DataGridViewContentAlignment.MiddleCenter);
            Col("TotalGasto",   "Total Gasto",     90,  DataGridViewContentAlignment.MiddleRight);
            Col("Situacao",     "Sit.",             45,  DataGridViewContentAlignment.MiddleCenter);

            // Colore a célula Situacao: verde = A, vermelho = I
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;
                var sit = row.Cells["Situacao"].Value?.ToString();
                row.Cells["Situacao"].Style.ForeColor     = sit == "A" ? System.Drawing.Color.FromArgb(39, 200, 100) : System.Drawing.Color.FromArgb(231, 76, 60);
                row.Cells["Situacao"].Style.Font          = new Font(grid.Font, FontStyle.Bold);

                // Formata TotalGasto com R$
                if (grid.Columns.Contains("TotalGasto") && row.Cells["TotalGasto"].Value is decimal d)
                    row.Cells["TotalGasto"].Value = d.ToString("N2");
            }
        }

        private void ModoNovo()
        {
            _codigoEditando = 0;
            foreach (Control c in pnlForm.Controls)
            {
                if (c is TextBox tb) tb.Clear();
                else if (c is MaskedTextBox mtb) mtb.Clear();
            }
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
            cmbSituacao.SelectedIndex = (obj.Situacao == "I") ? 1 : 0;
            pnlForm.Visible = true; txtNome.Focus();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            // Validações obrigatórias
            if (string.IsNullOrWhiteSpace(txtNome.Text))
                { MessageBox.Show("Informe o nome / razão social.", "Campo obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtNome.Focus(); return; }

            var celDigs = new string(System.Array.FindAll(txtCelular.Text.ToCharArray(), char.IsDigit));
            if (celDigs.Length < 10)
                { MessageBox.Show("Informe o celular completo.", "Campo obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtCelular.Focus(); return; }

            var cepDigs = new string(System.Array.FindAll(txtCep.Text.ToCharArray(), char.IsDigit));
            if (cepDigs.Length < 8)
                { MessageBox.Show("Informe o CEP completo (8 dígitos).", "Campo obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtCep.Focus(); return; }

            if (string.IsNullOrWhiteSpace(txtNumero.Text))
                { MessageBox.Show("Informe o número do endereço.", "Campo obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtNumero.Focus(); return; }

            if (string.IsNullOrWhiteSpace(txtComplemento.Text))
                { MessageBox.Show("Informe o complemento.", "Campo obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtComplemento.Focus(); return; }
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
                Situacao             = cmbSituacao.SelectedIndex == 1 ? "I" : "A",
            };
            var erro = _bll.Salvar(obj);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            pnlForm.Visible = false; _codigoEditando = 0; CarregarGrid();
        }
    }
}
