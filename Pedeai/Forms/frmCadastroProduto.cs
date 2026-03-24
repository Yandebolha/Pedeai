using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.DB;

namespace Pedeai.Forms
{
    public class frmCadastroProduto : Form
    {
        private DataGridView grid = new DataGridView();
        private Panel pnlForm = new Panel();
        private TextBox txtNome = new TextBox();
        private TextBox txtDescricao = new TextBox();
        private TextBox txtImagem = new TextBox();
        private ComboBox cmbCategoria = new ComboBox();
        private NumericUpDown numPreco = new NumericUpDown();
        private NumericUpDown numPromo = new NumericUpDown();
        private NumericUpDown numEstoque = new NumericUpDown();
        private NumericUpDown numOrdem = new NumericUpDown();
        private CheckBox chkControlaEstoque = new CheckBox();
        private CheckBox chkDestaque = new CheckBox();
        private CheckBox chkIfood = new CheckBox();
        private ComboBox cmbSituacao = new ComboBox();
        private int _codigoEditando = 0;

        public frmCadastroProduto()
        {
            Text = "Cadastro de Produtos";
            Size = new Size(900, 600);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 520);
            Font = new Font("Segoe UI", 9);
            BuildUI();
            CarregarGrid();
        }

        private void BuildUI()
        {
            var topBar = new Panel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(8, 6, 8, 0) };
            topBar.BackColor = Color.FromArgb(40, 40, 80);

            var btnNovo = Botao("+ Novo Produto", Color.FromArgb(0, 150, 136));
            btnNovo.Click += (_, __) => ModoNovo();
            var btnCat = Botao("Categorias", Color.FromArgb(103, 58, 183));
            btnCat.Left = 115;
            btnCat.Click += (_, __) => new frmCadastroCategoria().ShowDialog(this);
            topBar.Controls.AddRange(new Control[] { btnNovo, btnCat });

            // Grid
            grid.Dock = DockStyle.Fill;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false;
            grid.BackgroundColor = Color.White;
            grid.Font = new Font("Segoe UI", 9);
            grid.BorderStyle = BorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 80);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.DoubleClick += (_, __) => CarregarParaEditar();

            // Formulário
            pnlForm = new Panel { Dock = DockStyle.Bottom, Height = 210, Padding = new Padding(10), Visible = false };
            pnlForm.BackColor = Color.FromArgb(245, 245, 250);
            pnlForm.BorderStyle = BorderStyle.FixedSingle;

            int y = 8;
            // Linha 1
            Add(pnlForm, "Categoria:", 10, y); cmbCategoria = new ComboBox { Left = 75, Top = y, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList }; pnlForm.Controls.Add(cmbCategoria);
            Add(pnlForm, "Nome:", 235, y); txtNome = new TextBox { Left = 275, Top = y, Width = 250 }; pnlForm.Controls.Add(txtNome);
            Add(pnlForm, "Ordem:", 538, y); numOrdem = new NumericUpDown { Left = 590, Top = y, Width = 60 }; pnlForm.Controls.Add(numOrdem);
            Add(pnlForm, "Sit.:", 660, y); cmbSituacao = new ComboBox { Left = 690, Top = y, Width = 55, DropDownStyle = ComboBoxStyle.DropDownList }; cmbSituacao.Items.AddRange(new[] { "A", "I" }); cmbSituacao.SelectedIndex = 0; pnlForm.Controls.Add(cmbSituacao);

            y += 34;
            // Linha 2
            Add(pnlForm, "Descrição:", 10, y); txtDescricao = new TextBox { Left = 75, Top = y, Width = 325 }; pnlForm.Controls.Add(txtDescricao);
            Add(pnlForm, "Imagem URL:", 410, y); txtImagem = new TextBox { Left = 490, Top = y, Width = 265 }; pnlForm.Controls.Add(txtImagem);

            y += 34;
            // Linha 3
            Add(pnlForm, "Preço R$:", 10, y); numPreco = Decimal3(75, y); pnlForm.Controls.Add(numPreco);
            Add(pnlForm, "Promo R$:", 165, y); numPromo = Decimal3(240, y); pnlForm.Controls.Add(numPromo);
            Add(pnlForm, "Estoque:", 330, y); numEstoque = Decimal3(390, y); pnlForm.Controls.Add(numEstoque);
            chkControlaEstoque = new CheckBox { Text = "Ctrl.Estoque", Left = 480, Top = y + 2, Width = 105 }; pnlForm.Controls.Add(chkControlaEstoque);
            chkDestaque = new CheckBox { Text = "Destaque", Left = 590, Top = y + 2, Width = 85 }; pnlForm.Controls.Add(chkDestaque);
            chkIfood = new CheckBox { Text = "iFood", Left = 680, Top = y + 2, Width = 60 }; pnlForm.Controls.Add(chkIfood);

            y += 40;
            // Botões
            var btnSalvar = Botao("Salvar", Color.FromArgb(33, 150, 243)); btnSalvar.Left = 10; btnSalvar.Top = y; btnSalvar.Click += BtnSalvar_Click; pnlForm.Controls.Add(btnSalvar);
            var btnCanc = Botao("Cancelar", Color.FromArgb(158, 158, 158)); btnCanc.Left = 120; btnCanc.Top = y; btnCanc.Click += (_, __) => { pnlForm.Visible = false; _codigoEditando = 0; }; pnlForm.Controls.Add(btnCanc);
            var btnDes = Botao("Desativar", Color.FromArgb(244, 67, 54)); btnDes.Left = 230; btnDes.Top = y; btnDes.Click += BtnDesativar_Click; pnlForm.Controls.Add(btnDes);

            Controls.Add(grid);
            Controls.Add(topBar);
            Controls.Add(pnlForm);
        }

        private void Add(Panel p, string label, int x, int y)
        {
            p.Controls.Add(new Label { Text = label, Left = x, Top = y + 3, AutoSize = true });
        }

        private NumericUpDown Decimal3(int x, int y)
        {
            return new NumericUpDown { Left = x, Top = y, Width = 80, DecimalPlaces = 2, Maximum = 99999, Minimum = 0 };
        }

        private Button Botao(string texto, Color cor)
        {
            return new Button { Text = texto, BackColor = cor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 100, Height = 28, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
        }

        private void CarrecarComboCategorias()
        {
            try
            {
                cmbCategoria.Items.Clear();
                cmbCategoria.Items.Add(new CatItem(0, "-- Selecione --"));
                var dt = DbHelper.ListarCategorias(true);
                foreach (DataRow r in dt.Rows)
                    cmbCategoria.Items.Add(new CatItem(Convert.ToInt32(r["Codigo"]), r["Nome"]?.ToString() ?? ""));
                cmbCategoria.SelectedIndex = 0;
            }
            catch { }
        }

        private void CarregarGrid()
        {
            try { grid.DataSource = DbHelper.ListarMercadorias(); }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void ModoNovo()
        {
            CarrecarComboCategorias();
            _codigoEditando = 0;
            txtNome.Clear(); txtDescricao.Clear(); txtImagem.Clear();
            numPreco.Value = 0; numPromo.Value = 0; numEstoque.Value = 0; numOrdem.Value = 0;
            chkControlaEstoque.Checked = false; chkDestaque.Checked = false; chkIfood.Checked = false;
            cmbSituacao.SelectedIndex = 0;
            pnlForm.Visible = true; txtNome.Focus();
        }

        private void CarregarParaEditar()
        {
            if (grid.SelectedRows.Count == 0) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var row = DbHelper.GetProduto(cod);
            if (row == null) return;
            CarrecarComboCategorias();
            _codigoEditando = cod;
            txtNome.Text = row["mercMercadoria"]?.ToString() ?? "";
            txtDescricao.Text = row["mercApresentacao"]?.ToString() ?? "";
            txtImagem.Text = row["mercImagem_Url"]?.ToString() ?? "";
            numPreco.Value = row["mercPreco_Venda"] == DBNull.Value ? 0 : Convert.ToDecimal(row["mercPreco_Venda"]);
            numPromo.Value = row["mercPreco_Promocional"] == DBNull.Value ? 0 : Convert.ToDecimal(row["mercPreco_Promocional"]);
            numEstoque.Value = row["mercEstoque_Atual"] == DBNull.Value ? 0 : Convert.ToDecimal(row["mercEstoque_Atual"]);
            numOrdem.Value = row["mercOrdem"] == DBNull.Value ? 0 : Convert.ToDecimal(row["mercOrdem"]);
            chkControlaEstoque.Checked = row["mercControla_Estoque"]?.ToString() == "1" || row["mercControla_Estoque"]?.ToString() == "True";
            chkDestaque.Checked = row["mercDestaque"]?.ToString() == "1" || row["mercDestaque"]?.ToString() == "True";
            chkIfood.Checked = row["mercHabilitar_Ifood"]?.ToString() == "1" || row["mercHabilitar_Ifood"]?.ToString() == "True";
            var sit = row["Situacao"]?.ToString() ?? "A";
            cmbSituacao.SelectedItem = sit;
            // Seleciona categoria no combo
            var codGrp = row["Codigo_Grupo"] == DBNull.Value ? 0 : Convert.ToInt32(row["Codigo_Grupo"]);
            foreach (CatItem item in cmbCategoria.Items)
                if (item.Codigo == codGrp) { cmbCategoria.SelectedItem = item; break; }
            pnlForm.Visible = true; txtNome.Focus();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text)) { MessageBox.Show("Informe o nome do produto."); return; }
            var catSel = cmbCategoria.SelectedItem as CatItem;
            var erro = DbHelper.SalvarProduto(_codigoEditando, catSel?.Codigo ?? 0,
                txtNome.Text.Trim(), txtDescricao.Text.Trim(), numPreco.Value, numPromo.Value,
                numEstoque.Value, chkControlaEstoque.Checked, txtImagem.Text.Trim(),
                chkDestaque.Checked, (int)numOrdem.Value, chkIfood.Checked,
                cmbSituacao.SelectedItem?.ToString() ?? "A");
            if (erro != "") { MessageBox.Show("Erro: " + erro); return; }
            pnlForm.Visible = false; _codigoEditando = 0; CarregarGrid();
        }

        private void BtnDesativar_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            if (MessageBox.Show("Desativar produto?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            DbHelper.AlternarSituacaoMercadoria(Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value));
            pnlForm.Visible = false; CarregarGrid();
        }

        private class CatItem
        {
            public int Codigo; public string Nome;
            public CatItem(int c, string n) { Codigo = c; Nome = n; }
            public override string ToString() => Nome;
        }
    }
}
