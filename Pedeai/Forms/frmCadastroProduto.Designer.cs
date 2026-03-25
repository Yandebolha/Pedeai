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

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(904, 581);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(820, 540);
            BackColor = Color.FromArgb(15, 22, 45);
            ForeColor = Color.White;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Cadastro de Produtos";
        }

        private DataGridView  grid;
        private Panel         pnlForm;
        private ComboBox      cmbCategoria;
        private TextBox       txtNome;
        private ComboBox      cmbSituacao;
        private TextBox       txtDescricao;
        private Label         lblImagem;
        private NumericUpDown numPreco;
        private NumericUpDown numCusto;
        private NumericUpDown numPromo;
        private NumericUpDown numEstoque;
        private CheckBox      chkControlaEstoque;
        private CheckBox      chkDestaque;
        private CheckBox      chkIfood;
    }
}
