using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmCadastroProduto
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
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

            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(904, 581);
            this.MinimumSize = new System.Drawing.Size(820, 540);
            this.BackColor = System.Drawing.Color.FromArgb(15, 22, 45);
            this.ForeColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cadastro de Produtos";

            // ── Top bar ───────────────────────────────────────────────────
            var topBar = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Color.FromArgb(36, 48, 82) };
            btnNovo = new Button
            {
                Text = "+ Novo Produto", Left = 8, Top = 8, Width = 115, Height = 28,
                BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnNovo.FlatAppearance.BorderSize = 0; btnNovo.Click += (_, __) => ModoNovo();

            btnEditar = new Button
            {
                Text = "\u270F Editar", Left = 133, Top = 8, Width = 95, Height = 28,
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
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(28, 37, 65);
            grid.GridColor = Color.FromArgb(40, 55, 90);
            grid.Font = new Font("Segoe UI", 9F); grid.BorderStyle = BorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(36, 48, 82);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 34;
            grid.RowTemplate.Height = 28;
            grid.CellDoubleClick += (_, __) => CarregarParaEditar();
            grid.DataError += (_, e) => e.ThrowException = false;

            // ── Painel formulário ──────────────────────────────────────────
            pnlForm.Dock = DockStyle.Bottom; pnlForm.Height = 225;
            pnlForm.BackColor = Color.FromArgb(28, 37, 65);
            pnlForm.BorderStyle = BorderStyle.FixedSingle;
            pnlForm.Visible = DesignMode;

            var lblCat  = new Label { Text = "Categoria:",      Left = 10,  Top = 11, AutoSize = true };
            cmbCategoria.Left = 80; cmbCategoria.Top = 8; cmbCategoria.Width = 160;
            cmbCategoria.DropDownStyle = ComboBoxStyle.DropDown;
            cmbCategoria.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbCategoria.AutoCompleteSource = AutoCompleteSource.ListItems;

            var lblNom  = new Label { Text = "Nome do Produto:", Left = 252, Top = 11, AutoSize = true };
            txtNome.Left = 365; txtNome.Top = 8; txtNome.Width = 270;

            var lblSit  = new Label { Text = "Situa\u00e7\u00e3o:",       Left = 647, Top = 11, AutoSize = true };
            cmbSituacao.Left = 702; cmbSituacao.Top = 8; cmbSituacao.Width = 90;
            cmbSituacao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSituacao.Items.AddRange(new object[] { "Ativo", "Inativo" });
            cmbSituacao.SelectedIndex = 0;

            btnImg = new Button
            {
                Text = "Imagem", Left = 10, Top = 41, Width = 85, Height = 24,
                BackColor = Color.FromArgb(52, 100, 170), ForeColor = Color.White, FlatStyle = FlatStyle.Flat
            };
            btnImg.FlatAppearance.BorderSize = 0; btnImg.Click += BtnImagem_Click;

            lblImagem.Text = "nenhuma imagem selecionada";
            lblImagem.Left = 105; lblImagem.Top = 45;
            lblImagem.Width = 690; lblImagem.ForeColor = Color.Gray;
            lblImagem.AutoSize = false;

            var lblPrc  = new Label { Text = "Pre\u00e7o R$:",  Left = 10,  Top = 79, AutoSize = true };
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

            var lblPub = new Label { Text = "Publica\u00e7\u00f5es:", Left = 10, Top = 119, AutoSize = true, ForeColor = Color.FromArgb(180, 190, 220) };
            chkSite.Text     = "No site";   chkSite.Left     = 90;  chkSite.Top     = 116; chkSite.AutoSize = true;
            chkDestaque.Text = "Destaque";  chkDestaque.Left = 180; chkDestaque.Top = 116; chkDestaque.AutoSize = true;

            var pnlBtns = new Panel { Dock = DockStyle.Bottom, Height = 48, BackColor = Color.FromArgb(28, 37, 65) };
            btnS = new Button { Text = "Salvar",    Top = 10, Width = 110, Height = 28,
                BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnS.FlatAppearance.BorderSize = 0; btnS.Click += BtnSalvar_Click;

            btnC = new Button { Text = "Cancelar",  Top = 10, Width = 110, Height = 28,
                BackColor = Color.FromArgb(80, 95, 130), ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnC.FlatAppearance.BorderSize = 0;
            btnC.Click += (_, __) => { pnlForm.Visible = false; _codigoEditando = 0; };

            btnD = new Button { Text = "Desativar", Top = 10, Width = 110, Height = 28,
                BackColor = Color.FromArgb(192, 57, 43), ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
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
            this.ResumeLayout(false);
        }

        internal DataGridView  grid;
        internal Panel         pnlForm;
        internal ComboBox      cmbCategoria;
        internal TextBox       txtNome;
        internal ComboBox      cmbSituacao;
        internal Label         lblImagem;
        internal NumericUpDown numPreco;
        internal NumericUpDown numCusto;
        internal NumericUpDown numPromo;
        internal NumericUpDown numEstoque;
        internal CheckBox      chkControlaEstoque;
        internal CheckBox      chkDestaque;
        internal CheckBox      chkSite;
        internal Button        btnNovo;
        internal Button        btnEditar;
        internal Button        btnCat;
        internal Button        btnPesq;
        internal Button        btnImg;
        internal Button        btnS;
        internal Button        btnC;
        internal Button        btnD;
        internal TextBox       _txtFiltro;
    }
}
