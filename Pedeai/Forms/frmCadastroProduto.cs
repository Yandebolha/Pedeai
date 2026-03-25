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
        private string _caminhoImagem = "";
        private int    _codigoEditando = 0;

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
            txtDescricao       = new TextBox();
            lblImagem          = new Label();
            numPreco           = new NumericUpDown();
            numCusto           = new NumericUpDown();
            numPromo           = new NumericUpDown();
            numEstoque         = new NumericUpDown();
            chkControlaEstoque = new CheckBox();
            chkDestaque        = new CheckBox();
            chkIfood           = new CheckBox();

            // ── Top bar ───────────────────────────────────────────────────
            var topBar = new Panel { Dock = DockStyle.Top, Height = 44,
                BackColor = Color.FromArgb(36, 48, 82) };
            var btnNovo = new Button
            {
                Text = "+ Novo Produto", Left = 8, Top = 8, Width = 115, Height = 28,
                BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnNovo.FlatAppearance.BorderSize = 0; btnNovo.Click += (_, __) => ModoNovo();

            var btnCat = new Button
            {
                Text = "Categorias", Left = 133, Top = 8, Width = 100, Height = 28,
                BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnCat.FlatAppearance.BorderSize = 0;
            btnCat.Click += (_, __) => { new frmCadastroCategoria().ShowDialog(this); CarrecarComboCategorias(); };
            topBar.Controls.AddRange(new Control[] { btnNovo, btnCat });

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

            // Linha 2: Descrição | Imagem
            var lblDesc = new Label { Text = "Descrição:",      Left = 10,  Top = 45, AutoSize = true };
            txtDescricao.Left = 80; txtDescricao.Top = 42; txtDescricao.Width = 330;

            var btnImg = new Button
            {
                Text = "Imagem", Left = 425, Top = 41, Width = 85, Height = 24,
                BackColor = Color.FromArgb(52, 100, 170), ForeColor = Color.White, FlatStyle = FlatStyle.Flat
            };
            btnImg.FlatAppearance.BorderSize = 0; btnImg.Click += BtnImagem_Click;

            lblImagem.Text      = "nenhuma imagem selecionada";
            lblImagem.Left      = 520; lblImagem.Top = 45;
            lblImagem.Width     = 275; lblImagem.ForeColor = Color.Gray;
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
            numEstoque.DecimalPlaces = 2; numEstoque.Maximum = 999999;

            chkControlaEstoque.Text = "Controla estoque"; chkControlaEstoque.Left = 615; chkControlaEstoque.Top = 78; chkControlaEstoque.AutoSize = true;
            chkDestaque.Text        = "Destaque";          chkDestaque.Left        = 755; chkDestaque.Top        = 78; chkDestaque.AutoSize        = true;

            // Linha 4: iFood
            chkIfood.Text      = "Disponível no iFood (habilita o produto no cardápio do iFood)";
            chkIfood.Left      = 10; chkIfood.Top = 114; chkIfood.AutoSize = true;
            chkIfood.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
            chkIfood.ForeColor = Color.FromArgb(200, 80, 0);

            // Botões
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

            var btnD = new Button { Text = "Desativar", Top = 10, Width = 110, Height = 28,
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
                lblDesc, txtDescricao, btnImg, lblImagem,
                lblPrc, numPreco, lblCst, numCusto, lblPrm, numPromo, lblEst, numEstoque,
                chkControlaEstoque, chkDestaque,
                chkIfood
            });
            pnlForm.Controls.Add(pnlBtns);

            Controls.Add(grid); Controls.Add(topBar); Controls.Add(pnlForm);
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