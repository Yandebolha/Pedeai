using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmCadastroProduto : Form
    {
        private readonly MercadoriaBLL      _bll    = new MercadoriaBLL();
        private readonly GrupoMercadoriaBLL _grpBLL = new GrupoMercadoriaBLL();
        private string    _caminhoImagem = "";
        private int       _codigoEditando = 0;
        private System.Data.DataTable _dtProdutos;
        private TextBox   _txtFiltro;
        internal Button   btnNovo;
        internal Button   btnEditar;
        internal Button   btnCat;
        internal Button   btnPesq;
        internal Button   btnImg;
        internal Button   btnS;
        internal Button   btnC;
        internal Button   btnD;

        public frmCadastroProduto()
        {
            InitializeComponent();
            BuildUI();
            if (!DesignMode) CarregarGrid();
        }

        private void BuildUI()
        {
            grid               = new DataGridView();
            pnlForm            = new Panel();
            cmbCategoria       = new ComboBox();
            txtNome            = new TextBox();
            cmbSituacao        = new ComboBox();
            lblImagem          = new Label();
            numPreco           = new NumericUpDown();
            numCusto           = new NumericUpDown();
            numPromo           = new NumericUpDown();
            numEstoque         = new NumericUpDown();
            chkControlaEstoque = new CheckBox();
            chkDestaque        = new CheckBox();
            chkSite            = new CheckBox();

            // ── Top bar ───────────────────────────────────────────────────
            var topBar = new Panel { Dock = DockStyle.Top, Height = 44,
                BackColor = Color.FromArgb(36, 48, 82) };
            btnNovo = new Button
            {
                Text = "+ Novo Produto", Left = 8, Top = 8, Width = 115, Height = 28,
                BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnNovo.FlatAppearance.BorderSize = 0; btnNovo.Click += (_, __) => ModoNovo();

            btnEditar = new Button
            {
                Text = "✏ Editar", Left = 133, Top = 8, Width = 95, Height = 28,
                BackColor = Color.FromArgb(230, 126, 34), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnEditar.FlatAppearance.BorderSize = 0; btnEditar.Click += (_, __) => CarregarParaEditar();

            btnCat = new Button
            {
                Text = "Categorias", Left = 238, Top = 8, Width = 100, Height = 28,
                BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnCat.FlatAppearance.BorderSize = 0;
            btnCat.Click += (_, __) => { new frmCadastroCategoria().ShowDialog(this); CarrecarComboCategorias(); };
            topBar.Controls.AddRange(new Control[] { btnNovo, btnEditar, btnCat });

            // ── Barra de pesquisa ─────────────────────────────────────────
            var pnlSearch = new Panel { Dock = DockStyle.Top, Height = 38, BackColor = Color.FromArgb(22, 30, 55) };
            _txtFiltro = new TextBox
            {
                Left = 8, Top = 7, Width = 260, Font = new Font("Segoe UI", 9.5F),
                PlaceholderText = "Pesquisar por nome ou categoria..."
            };
            btnPesq = new Button
            {
                Text = "Pesquisar", Left = 276, Top = 6, Width = 90, Height = 26,
                BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btnPesq.FlatAppearance.BorderSize = 0;
            btnPesq.Click += (_, __) => FiltrarGrid(_txtFiltro.Text);
            _txtFiltro.KeyDown += (s, ev) => { if (ev.KeyCode == Keys.Enter) FiltrarGrid(_txtFiltro.Text); };
            pnlSearch.Controls.AddRange(new Control[] { _txtFiltro, btnPesq });

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
            grid.CellDoubleClick += (_, __) => CarregarParaEditar();

            // ── Painel formulário ──────────────────────────────────────────
            pnlForm.Dock = DockStyle.Bottom; pnlForm.Height = 225;
            pnlForm.BackColor = Color.FromArgb(28, 37, 65);
            pnlForm.BorderStyle = BorderStyle.FixedSingle;
            pnlForm.Visible = false;

            // Linha 1: Categoria | Nome | Situação
            var lblCat  = new Label { Text = "Categoria:",      Left = 10,  Top = 11, AutoSize = true };
            cmbCategoria.Left = 80; cmbCategoria.Top = 8; cmbCategoria.Width = 160;
            cmbCategoria.DropDownStyle = ComboBoxStyle.DropDown;
            cmbCategoria.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbCategoria.AutoCompleteSource = AutoCompleteSource.ListItems;

            var lblNom  = new Label { Text = "Nome do Produto:", Left = 252, Top = 11, AutoSize = true };
            txtNome.Left = 365; txtNome.Top = 8; txtNome.Width = 270;

            var lblSit  = new Label { Text = "Situação:",       Left = 647, Top = 11, AutoSize = true };
            cmbSituacao.Left = 702; cmbSituacao.Top = 8; cmbSituacao.Width = 90;
            cmbSituacao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSituacao.Items.AddRange(new object[] { "Ativo", "Inativo" });
            cmbSituacao.SelectedIndex = 0;

            // Linha 2: Imagem
            btnImg = new Button
            {
                Text = "Imagem", Left = 10, Top = 41, Width = 85, Height = 24,
                BackColor = Color.FromArgb(52, 100, 170), ForeColor = Color.White, FlatStyle = FlatStyle.Flat
            };
            btnImg.FlatAppearance.BorderSize = 0; btnImg.Click += BtnImagem_Click;

            lblImagem.Text      = "nenhuma imagem selecionada";
            lblImagem.Left      = 105; lblImagem.Top = 45;
            lblImagem.Width     = 690; lblImagem.ForeColor = Color.Gray;
            lblImagem.AutoSize  = false;

            // Linha 3: Preço | Custo | Promo | Estoque | Checkboxes
            var lblPrc  = new Label { Text = "Preço R$:",  Left = 10,  Top = 79, AutoSize = true };
            numPreco.Left = 73; numPreco.Top = 76; numPreco.Width = 85;
            numPreco.DecimalPlaces = 2; numPreco.Maximum = 999999;

            var lblCst  = new Label { Text = "Custo R$:",  Left = 166, Top = 79, AutoSize = true };
            numCusto.Left = 228; numCusto.Top = 76; numCusto.Width = 85;
            numCusto.DecimalPlaces = 2; numCusto.Maximum = 999999;

            var lblPrm  = new Label { Text = "Promo R$:",  Left = 320, Top = 79, AutoSize = true };
            numPromo.Left = 382; numPromo.Top = 76; numPromo.Width = 85;
            numPromo.DecimalPlaces = 2; numPromo.Maximum = 999999;

            var lblEst  = new Label { Text = "Estoque:",   Left = 474, Top = 79, AutoSize = true };
            numEstoque.Left = 530; numEstoque.Top = 76; numEstoque.Width = 75;
            numEstoque.DecimalPlaces = 2; numEstoque.Minimum = -999999; numEstoque.Maximum = 999999;

            chkControlaEstoque.Text = "Controla estoque"; chkControlaEstoque.Left = 615; chkControlaEstoque.Top = 78; chkControlaEstoque.AutoSize = true;

            // Linha 4: Publicações
            var lblPub = new Label { Text = "Publicações:", Left = 10, Top = 119, AutoSize = true, ForeColor = Color.FromArgb(180, 190, 220) };
            chkSite.Text     = "No site";   chkSite.Left     = 90;  chkSite.Top     = 116; chkSite.AutoSize = true;
            chkDestaque.Text = "Destaque";  chkDestaque.Left = 180; chkDestaque.Top = 116; chkDestaque.AutoSize = true;

            // Botões
            var pnlBtns = new Panel { Dock = DockStyle.Bottom, Height = 48,
                BackColor = Color.FromArgb(28, 37, 65) };
            btnS = new Button { Text = "Salvar",    Top = 10, Width = 110, Height = 28,
                BackColor = Color.FromArgb(52, 152, 219),  ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnS.FlatAppearance.BorderSize = 0; btnS.Click += BtnSalvar_Click;

            btnC = new Button { Text = "Cancelar",  Top = 10, Width = 110, Height = 28,
                BackColor = Color.FromArgb(80, 95, 130), ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnC.FlatAppearance.BorderSize = 0;
            btnC.Click += (_, __) => { pnlForm.Visible = false; _codigoEditando = 0; };

            btnD = new Button { Text = "Desativar", Top = 10, Width = 110, Height = 28,
                BackColor = Color.FromArgb(192, 57, 43),   ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnD.FlatAppearance.BorderSize = 0; btnD.Click += BtnDesativar_Click;

            void CentrarBotoes()
            {
                int x = (pnlBtns.Width - 110 * 3 - 10 * 2) / 2;
                if (x < 10) x = 10;
                btnS.Left = x; btnC.Left = x + 120; btnD.Left = x + 240;
            }
            pnlBtns.SizeChanged += (_, __) => CentrarBotoes();
            pnlBtns.Controls.AddRange(new Control[] { btnS, btnC, btnD });

            pnlForm.Controls.AddRange(new Control[]
            {
                lblCat, cmbCategoria, lblNom, txtNome, lblSit, cmbSituacao,
                btnImg, lblImagem,
                lblPrc, numPreco, lblCst, numCusto, lblPrm, numPromo, lblEst, numEstoque,
                chkControlaEstoque, lblPub, chkSite, chkDestaque
            });
            pnlForm.Controls.Add(pnlBtns);

            Controls.Add(grid); Controls.Add(pnlSearch); Controls.Add(topBar); Controls.Add(pnlForm);
        }

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
            try
            {
                _dtProdutos = _bll.Listar();
                FiltrarGrid(_txtFiltro?.Text ?? "");
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void FiltrarGrid(string filtro)
        {
            if (_dtProdutos == null) return;
            var dv = new System.Data.DataView(_dtProdutos);
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var f = filtro.Replace("'", "''");
                dv.RowFilter = $"Nome LIKE '%{f}%' OR Categoria LIKE '%{f}%'";
            }
            grid.DataSource = dv;
            ConfigurarColunasProdutos();
        }

        private void ConfigurarColunasProdutos()
        {
            if (grid.Columns.Count == 0) return;
            foreach (DataGridViewColumn col in grid.Columns)
                col.Visible = false;
            var show = new[] { "Nome", "Categoria", "Preco", "Promocional", "Estoque" };
            foreach (var name in show)
                if (grid.Columns.Contains(name)) grid.Columns[name].Visible = true;
            var caps = new System.Collections.Generic.Dictionary<string, string>
            {
                ["Preco"]      = "Preço",
                ["Promocional"] = "Preço Promo.",
            };
            foreach (var kv in caps)
                if (grid.Columns.Contains(kv.Key)) grid.Columns[kv.Key].HeaderText = kv.Value;
        }

        private void ModoNovo()
        {
            CarrecarComboCategorias();
            _codigoEditando = 0;
            cmbCategoria.Text = "";
            txtNome.Clear();
            _caminhoImagem = ""; lblImagem.Text = "nenhuma imagem selecionada"; lblImagem.ForeColor = Color.Gray;
            numPreco.Value = 0; numCusto.Value = 0; numPromo.Value = 0; numEstoque.Value = 0;
            chkControlaEstoque.Checked = false; chkDestaque.Checked = false; chkSite.Checked = false;
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
            _caminhoImagem    = obj.mercImagem_Url ?? "";
            lblImagem.Text    = string.IsNullOrEmpty(_caminhoImagem) ? "nenhuma imagem selecionada" : Path.GetFileName(_caminhoImagem);
            lblImagem.ForeColor = string.IsNullOrEmpty(_caminhoImagem) ? Color.Gray : Color.FromArgb(30, 120, 30);
            numPreco.Value    = obj.mercPreco_Venda;
            numCusto.Value    = obj.mercPreco_Custo;
            numPromo.Value    = obj.mercPreco_Promocional;
            numEstoque.Value  = obj.mercEstoque_Atual;
            chkControlaEstoque.Checked = obj.mercControla_Estoque;
            chkDestaque.Checked        = obj.mercDestaque;
            chkSite.Checked            = obj.mercHabilitar_Site;
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
                mercApresentacao      = "",
                mercPreco_Venda       = numPreco.Value,
                mercPreco_Custo       = numCusto.Value,
                mercPreco_Promocional = numPromo.Value,
                mercEstoque_Atual     = numEstoque.Value,
                mercControla_Estoque  = chkControlaEstoque.Checked,
                mercImagem_Url        = _caminhoImagem,
                mercDestaque          = chkDestaque.Checked,
                mercOrdem             = 0,
                mercHabilitar_Site    = chkSite.Checked,
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