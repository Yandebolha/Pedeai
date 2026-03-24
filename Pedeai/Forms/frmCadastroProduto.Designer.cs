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
                BackColor = Color.FromArgb(40, 40, 80) };
            var btnNovo = new Button
            {
                Text = "+ Novo Produto", Left = 8, Top = 8, Width = 115, Height = 28,
                BackColor = Color.FromArgb(0, 150, 136), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnNovo.FlatAppearance.BorderSize = 0; btnNovo.Click += (_, __) => ModoNovo();

            var btnCat = new Button
            {
                Text = "Categorias", Left = 133, Top = 8, Width = 100, Height = 28,
                BackColor = Color.FromArgb(103, 58, 183), ForeColor = Color.White,
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
            grid.RowHeadersVisible = false; grid.BackgroundColor = Color.White;
            grid.Font = new Font("Segoe UI", 9F); grid.BorderStyle = BorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 80);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.DoubleClick += (_, __) => CarregarParaEditar();

            // ── Painel formulário ──────────────────────────────────────────
            pnlForm.Dock = DockStyle.Bottom; pnlForm.Height = 225;
            pnlForm.BackColor = Color.FromArgb(245, 245, 250);
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
                BackColor = Color.FromArgb(80, 90, 140), ForeColor = Color.White, FlatStyle = FlatStyle.Flat
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
            var btnS = new Button { Text = "Salvar",    Left = 10,  Top = 152, Width = 100, Height = 28,
                BackColor = Color.FromArgb(33, 150, 243),  ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnS.FlatAppearance.BorderSize = 0; btnS.Click += BtnSalvar_Click;

            var btnC = new Button { Text = "Cancelar",  Left = 120, Top = 152, Width = 100, Height = 28,
                BackColor = Color.FromArgb(158, 158, 158), ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnC.FlatAppearance.BorderSize = 0;
            btnC.Click += (_, __) => { pnlForm.Visible = false; _codigoEditando = 0; };

            var btnD = new Button { Text = "Desativar", Left = 230, Top = 152, Width = 100, Height = 28,
                BackColor = Color.FromArgb(244, 67, 54),   ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnD.FlatAppearance.BorderSize = 0; btnD.Click += BtnDesativar_Click;

            pnlForm.Controls.AddRange(new Control[]
            {
                lblCat, cmbCategoria, lblNom, txtNome, lblSit, cmbSituacao,
                lblDesc, txtDescricao, btnImg, lblImagem,
                lblPrc, numPreco, lblCst, numCusto, lblPrm, numPromo, lblEst, numEstoque,
                chkControlaEstoque, chkDestaque,
                chkIfood,
                btnS, btnC, btnD
            });

            Controls.Add(grid); Controls.Add(topBar); Controls.Add(pnlForm);

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(904, 581);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(820, 540);
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
