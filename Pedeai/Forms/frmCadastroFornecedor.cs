using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public class frmCadastroFornecedor : Form
    {
        private readonly FornecedorBLL _bll = new FornecedorBLL();
        private DataGridView grid = new DataGridView();
        private Panel pnlForm = new Panel();
        private TextBox txtRazao, txtFantasia, txtCnpj, txtIe, txtTelefone, txtEmail, txtContato;
        private TextBox txtCep, txtEndereco, txtNumero, txtBairro, txtCidade, txtEstado, txtObs;
        private ComboBox cmbSituacao = new ComboBox();
        private int _codigoEditando = 0;

        public frmCadastroFornecedor()
        {
            Text = "Cadastro de Fornecedores";
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
            var btnN = Botao("+ Novo Fornecedor", Color.FromArgb(0, 150, 136)); btnN.Click += (_, __) => ModoNovo();
            var btnR = Botao("Atualizar", Color.FromArgb(63, 81, 181)); btnR.Left = 170; btnR.Click += (_, __) => CarregarGrid();
            topBar.Controls.AddRange(new Control[] { btnN, btnR });

            grid.Dock = DockStyle.Fill;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly = true; grid.AllowUserToAddRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false; grid.BackgroundColor = Color.White;
            grid.Font = new Font("Segoe UI", 9); grid.BorderStyle = BorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 80);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.DoubleClick += (_, __) => CarregarParaEditar();

            pnlForm = new Panel { Dock = DockStyle.Bottom, Height = 240, Padding = new Padding(10), Visible = false };
            pnlForm.BackColor = Color.FromArgb(245, 245, 250);
            pnlForm.BorderStyle = BorderStyle.FixedSingle;

            int y = 8;
            txtRazao = CampoF(pnlForm, "Razão Social:", 10, y, 230);
            txtFantasia = CampoF(pnlForm, "Nome Fantasia:", 255, y, 180);
            txtCnpj = CampoF(pnlForm, "CNPJ/CPF:", 450, y, 130);
            LblF(pnlForm, "Situação:", 595, y); cmbSituacao = new ComboBox { Left = 655, Top = y, Width = 65, DropDownStyle = ComboBoxStyle.DropDownList }; cmbSituacao.Items.AddRange(new[] { "A", "I" }); cmbSituacao.SelectedIndex = 0; pnlForm.Controls.Add(cmbSituacao);

            y += 32;
            txtIe = CampoF(pnlForm, "Insc.Estadual:", 10, y, 130);
            txtTelefone = CampoF(pnlForm, "Telefone:", 155, y, 120);
            txtEmail = CampoF(pnlForm, "Email:", 290, y, 200);
            txtContato = CampoF(pnlForm, "Contato:", 505, y, 150);

            y += 32;
            txtCep = CampoF(pnlForm, "CEP:", 10, y, 80);
            txtEndereco = CampoF(pnlForm, "Endereço:", 105, y, 220);
            txtNumero = CampoF(pnlForm, "Nº:", 340, y, 60);
            txtBairro = CampoF(pnlForm, "Bairro:", 415, y, 150);

            y += 32;
            txtCidade = CampoF(pnlForm, "Cidade:", 10, y, 200);
            txtEstado = CampoF(pnlForm, "UF:", 225, y, 40);

            y += 32;
            txtObs = CampoF(pnlForm, "Observações:", 10, y, 450);

            y += 38;
            var btnS = Botao("Salvar", Color.FromArgb(33, 150, 243)); btnS.Left = 10; btnS.Top = y; btnS.Click += BtnSalvar_Click; pnlForm.Controls.Add(btnS);
            var btnC = Botao("Cancelar", Color.FromArgb(158, 158, 158)); btnC.Left = 120; btnC.Top = y; btnC.Click += (_, __) => { pnlForm.Visible = false; _codigoEditando = 0; }; pnlForm.Controls.Add(btnC);

            Controls.Add(grid);
            Controls.Add(topBar);
            Controls.Add(pnlForm);
        }

        private TextBox CampoF(Panel p, string label, int x, int y, int w)
        {
            p.Controls.Add(new Label { Text = label, Left = x, Top = y + 3, AutoSize = true });
            var t = new TextBox { Left = x + label.Length * 6, Top = y, Width = w };
            p.Controls.Add(t); return t;
        }

        private void LblF(Panel p, string label, int x, int y)
        {
            p.Controls.Add(new Label { Text = label, Left = x, Top = y + 3, AutoSize = true });
        }

        private Button Botao(string texto, Color cor)
        {
            return new Button { Text = texto, BackColor = cor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 100, Height = 28, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
        }

        private void CarregarGrid()
        {
            try { grid.DataSource = _bll.Listar(); }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void ModoNovo()
        {
            _codigoEditando = 0;
            foreach (var c in pnlForm.Controls) if (c is TextBox tb) tb.Clear();
            cmbSituacao.SelectedIndex = 0;
            pnlForm.Visible = true; txtRazao.Focus();
        }

        private void CarregarParaEditar()
        {
            if (grid.SelectedRows.Count == 0) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var obj = _bll.PesquisaCodigo(cod);
            if (obj == null) return;
            _codigoEditando = cod;
            txtRazao.Text    = obj.fornNome_RazaoSocial ?? "";
            txtFantasia.Text = obj.fornApelido_Fantasia ?? "";
            txtCnpj.Text     = obj.fornCPF_CNPJ_ ?? "";
            txtIe.Text       = obj.fornRG_InscricaoEstadual ?? "";
            txtTelefone.Text = obj.fornTelefone ?? "";
            txtEmail.Text    = obj.fornEmail ?? "";
            txtContato.Text  = obj.fornContato ?? "";
            txtCep.Text      = obj.fornCEP ?? "";
            txtEndereco.Text = obj.fornEndereco ?? "";
            txtNumero.Text   = obj.fornNumero ?? "";
            txtBairro.Text   = obj.fornBairro ?? "";
            txtCidade.Text   = obj.fornCidade ?? "";
            txtEstado.Text   = obj.fornEstado ?? "";
            txtObs.Text      = obj.fornObservacoes ?? "";
            cmbSituacao.SelectedItem = obj.Situacao ?? "A";
            pnlForm.Visible = true; txtRazao.Focus();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRazao.Text)) { MessageBox.Show("Informe a Razão Social."); return; }
            var obj = new Fornecedor
            {
                Codigo                    = _codigoEditando,
                fornNome_RazaoSocial      = txtRazao.Text.Trim(),
                fornApelido_Fantasia      = txtFantasia.Text,
                fornCPF_CNPJ_             = txtCnpj.Text,
                fornRG_InscricaoEstadual  = txtIe.Text,
                fornTelefone              = txtTelefone.Text,
                fornEmail                 = txtEmail.Text,
                fornContato               = txtContato.Text,
                fornCEP                   = txtCep.Text,
                fornEndereco              = txtEndereco.Text,
                fornNumero                = txtNumero.Text,
                fornBairro                = txtBairro.Text,
                fornCidade                = txtCidade.Text,
                fornEstado                = txtEstado.Text,
                fornObservacoes           = txtObs.Text,
                Situacao                  = cmbSituacao.SelectedItem?.ToString() ?? "A",
            };
            var erro = _bll.Salvar(obj);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            pnlForm.Visible = false; _codigoEditando = 0; CarregarGrid();
        }
    }
}
