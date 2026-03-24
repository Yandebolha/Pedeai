using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public class frmCadastroProduto : Form
    {
        private readonly MercadoriaBLL      _bll    = new MercadoriaBLL();
        private readonly GrupoMercadoriaBLL _grpBLL = new GrupoMercadoriaBLL();

        private DataGridView grid    = new DataGridView();
        private Panel        pnlForm = new Panel();

        private ComboBox      cmbCategoria       = new ComboBox();
        private TextBox       txtNome            = new TextBox();
        private ComboBox      cmbSituacao        = new ComboBox();
        private TextBox       txtDescricao       = new TextBox();
        private Label         lblImagem          = new Label();
        private string        _caminhoImagem     = "";
        private NumericUpDown numPreco           = new NumericUpDown();
        private NumericUpDown numCusto           = new NumericUpDown();
        private NumericUpDown numPromo           = new NumericUpDown();
        private NumericUpDown numEstoque         = new NumericUpDown();
        private CheckBox      chkControlaEstoque = new CheckBox();
        private CheckBox      chkDestaque        = new CheckBox();
        private CheckBox      chkIfood           = new CheckBox();
        private int           _codigoEditando    = 0;

        public frmCadastroProduto()
        {
            Text          = "Cadastro de Produtos";
            Size          = new Size(920, 620);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize   = new Size(820, 540);
            Font          = new Font("Segoe UI", 9);
            BuildUI();
            CarregarGrid();
        }

        private void BuildUI()
        {
            var topBar = new Panel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(8, 6, 8, 0) };
            topBar.BackColor = Color.FromArgb(40, 40, 80);
            var btnNovo = Botao("+ Novo Produto", Color.FromArgb(0, 150, 136));
            btnNovo.Click += (s, e) => ModoNovo();
            var btnCat = Botao("Categorias", Color.FromArgb(103, 58, 183));
            btnCat.Left = 115;
            btnCat.Click += (s, e) => { new frmCadastroCategoria().ShowDialog(this); CarrecarComboCategorias(); };
            topBar.Controls.AddRange(new Control[] { btnNovo, btnCat });

            grid.Dock = DockStyle.Fill;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly = true; grid.AllowUserToAddRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false; grid.BackgroundColor = Color.White;
            grid.Font = new Font("Segoe UI", 9); grid.BorderStyle = BorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 80);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.DoubleClick += (s, e) => CarregarParaEditar();

            pnlForm = new Panel { Dock = DockStyle.Bottom, Height = 225, Padding = new Padding(10), Visible = false };
            pnlForm.BackColor = Color.FromArgb(245, 245, 250);
            pnlForm.BorderStyle = BorderStyle.FixedSingle;

            int y = 8;

            // Linha 1: Categoria | Nome | Situacao
            Lbl(pnlForm, "Categoria:", 10, y);
            cmbCategoria = new ComboBox { Left = 80, Top = y, Width = 170, DropDownStyle = ComboBoxStyle.DropDown,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend, AutoCompleteSource = AutoCompleteSource.ListItems };
            pnlForm.Controls.Add(cmbCategoria);

            Lbl(pnlForm, "Nome do Produto:", 262, y);
            txtNome = new TextBox { Left = 375, Top = y, Width = 290 };
            pnlForm.Controls.Add(txtNome);

            Lbl(pnlForm, "Situacao:", 682, y);
            cmbSituacao = new ComboBox { Left = 742, Top = y, Width = 90, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSituacao.Items.AddRange(new object[] { "Ativo", "Inativo" });
            cmbSituacao.SelectedIndex = 0;
            pnlForm.Controls.Add(cmbSituacao);

            y += 34;

            // Linha 2: Descricao | Imagem
            Lbl(pnlForm, "Descricao:", 10, y);
            txtDescricao = new TextBox { Left = 80, Top = y, Width = 330 };
            pnlForm.Controls.Add(txtDescricao);

            var btnImg = new Button { Text = "Imagem", Left = 425, Top = y - 1, Width = 85, Height = 24,
                BackColor = Color.FromArgb(80, 90, 140), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnImg.FlatAppearance.BorderSize = 0;
            btnImg.Click += BtnImagem_Click;
            pnlForm.Controls.Add(btnImg);

            lblImagem = new Label { Text = "nenhuma imagem selecionada", Left = 520, Top = y + 3,
                Width = 315, ForeColor = Color.Gray, AutoSize = false };
            pnlForm.Controls.Add(lblImagem);

            y += 34;

            // Linha 3: Preco | Custo | Promo | Estoque | checkboxes
            Lbl(pnlForm, "Preco R$:", 10, y);
            numPreco = Num2(73, y, 85); pnlForm.Controls.Add(numPreco);

            Lbl(pnlForm, "Custo R$:", 166, y);
            numCusto = Num2(228, y, 85); pnlForm.Controls.Add(numCusto);

            Lbl(pnlForm, "Promo R$:", 320, y);
            numPromo = Num2(385, y, 85); pnlForm.Controls.Add(numPromo);

            Lbl(pnlForm, "Estoque:", 479, y);
            numEstoque = Num2(535, y, 75); pnlForm.Controls.Add(numEstoque);

            chkControlaEstoque = new CheckBox { Text = "Controla estoque", Left = 620, Top = y + 2, AutoSize = true };
            pnlForm.Controls.Add(chkControlaEstoque);

            chkDestaque = new CheckBox { Text = "Destaque", Left = 760, Top = y + 2, AutoSize = true };
            pnlForm.Controls.Add(chkDestaque);

            y += 36;

            // Linha 4: iFood
            chkIfood = new CheckBox { Text = "Disponivel no iFood (habilita o produto no cardapio do iFood)",
                Left = 10, Top = y, AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 80, 0) };
            pnlForm.Controls.Add(chkIfood);

            y += 38;

            // Botoes
            var btnS = Botao("Salvar",    Color.FromArgb(33, 150, 243)); btnS.Left = 10;  btnS.Top = y; btnS.Click += BtnSalvar_Click; pnlForm.Controls.Add(btnS);
            var btnC = Botao("Cancelar",  Color.FromArgb(158, 158, 158)); btnC.Left = 120; btnC.Top = y; btnC.Click += (s, e) => { pnlForm.Visible = false; _codigoEditando = 0; }; pnlForm.Controls.Add(btnC);
            var btnD = Botao("Desativar", Color.FromArgb(244, 67, 54));  btnD.Left = 230; btnD.Top = y; btnD.Click += BtnDesativar_Click; pnlForm.Controls.Add(btnD);

            Controls.Add(grid);
            Controls.Add(topBar);
            Controls.Add(pnlForm);
        }

        private void Lbl(Panel p, string t, int x, int y) =>
            p.Controls.Add(new Label { Text = t, Left = x, Top = y + 3, AutoSize = true });

        private NumericUpDown Num2(int x, int y, int w) =>
            new NumericUpDown { Left = x, Top = y, Width = w, DecimalPlaces = 2, Maximum = 999999, Minimum = 0 };

        private Button Botao(string texto, Color cor) =>
            new Button { Text = texto, BackColor = cor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                         Width = 100, Height = 28, Font = new Font("Segoe UI", 9, FontStyle.Bold) };

        private void BtnImagem_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog
            {
                Title  = "Selecionar imagem do produto",
                Filter = "Imagens|*.jpg;*.jpeg;*.png;*.gif;*.webp|Todos|*.*"
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                _caminhoImagem = dlg.FileName;
                lblImagem.Text = Path.GetFileName(_caminhoImagem);
                lblImagem.ForeColor = Color.FromArgb(30, 120, 30);
            }
        }

        private void CarrecarComboCategorias()
        {
            try
            {
                cmbCategoria.Items.Clear();
                var dt = _grpBLL.Listar(true);
                foreach (System.Data.DataRow r in dt.Rows)
                    cmbCategoria.Items.Add(new CatItem(Convert.ToInt32(r["Codigo"]), r["Nome"]?.ToString() ?? ""));
            }
            catch { }
        }

        private void CarregarGrid()
        {
            try { grid.DataSource = _bll.Listar(); }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void ModoNovo()
        {
            CarrecarComboCategorias();
            _codigoEditando = 0;
            cmbCategoria.Text = "";
            txtNome.Clear(); txtDescricao.Clear();
            _caminhoImagem = ""; lblImagem.Text = "nenhuma imagem selecionada"; lblImagem.ForeColor = Color.Gray;
            numPreco.Value = 0; numCusto.Value = 0; numPromo.Value = 0; numEstoque.Value = 0;
            chkControlaEstoque.Checked = false; chkDestaque.Checked = false; chkIfood.Checked = false;
            cmbSituacao.SelectedIndex = 0;
            pnlForm.Visible = true; txtNome.Focus();
        }

        private void CarregarParaEditar()
        {
            if (grid.SelectedRows.Count == 0) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var obj = _bll.PesquisaCodigo(cod);
            if (obj == null) return;
            CarrecarComboCategorias();
            _codigoEditando   = cod;
            txtNome.Text      = obj.mercMercadoria ?? "";
            txtDescricao.Text = obj.mercApresentacao ?? "";
            _caminhoImagem    = obj.mercImagem_Url ?? "";
            lblImagem.Text    = string.IsNullOrEmpty(_caminhoImagem) ? "nenhuma imagem selecionada" : Path.GetFileName(_caminhoImagem);
            lblImagem.ForeColor = string.IsNullOrEmpty(_caminhoImagem) ? Color.Gray : Color.FromArgb(30, 120, 30);
            numPreco.Value    = obj.mercPreco_Venda;
            numCusto.Value    = obj.mercPreco_Custo;
            numPromo.Value    = obj.mercPreco_Promocional;
            numEstoque.Value  = obj.mercEstoque_Atual;
            chkControlaEstoque.Checked = obj.mercControla_Estoque;
            chkDestaque.Checked        = obj.mercDestaque;
            chkIfood.Checked           = obj.mercHabilitar_Ifood;
            cmbSituacao.SelectedItem   = obj.Situacao == "I" ? "Inativo" : "Ativo";
            cmbCategoria.Text = "";
            foreach (CatItem item in cmbCategoria.Items)
                if (item.Codigo == obj.Codigo_Grupo) { cmbCategoria.SelectedItem = item; break; }
            pnlForm.Visible = true; txtNome.Focus();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text)) { MessageBox.Show("Informe o nome do produto."); return; }

            int codigoGrupo = 0;
            if (cmbCategoria.SelectedItem is CatItem selItem)
                codigoGrupo = selItem.Codigo;
            else
                foreach (CatItem item in cmbCategoria.Items)
                    if (item.Nome.Equals(cmbCategoria.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                    { codigoGrupo = item.Codigo; break; }

            if (codigoGrupo == 0) { MessageBox.Show("Selecione ou digite uma categoria valida."); return; }

            var obj = new Mercadoria
            {
                Codigo                = _codigoEditando,
                Codigo_Grupo          = codigoGrupo,
                mercMercadoria        = txtNome.Text.Trim(),
                mercApresentacao      = txtDescricao.Text.Trim(),
                mercPreco_Venda       = numPreco.Value,
                mercPreco_Custo       = numCusto.Value,
                mercPreco_Promocional = numPromo.Value,
                mercEstoque_Atual     = numEstoque.Value,
                mercControla_Estoque  = chkControlaEstoque.Checked,
                mercImagem_Url        = _caminhoImagem,
                mercDestaque          = chkDestaque.Checked,
                mercOrdem             = 0,
                mercHabilitar_Ifood   = chkIfood.Checked,
                Situacao              = cmbSituacao.SelectedItem?.ToString() == "Inativo" ? "I" : "A",
            };
            var erro = _bll.Salvar(obj);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            pnlForm.Visible = false; _codigoEditando = 0; CarregarGrid();
        }

        private void BtnDesativar_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            if (MessageBox.Show("Desativar produto?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            _bll.AlternarSituacao(Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value));
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