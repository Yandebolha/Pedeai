using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmPedidoManual
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtNome              = new TextBox();
            btnSelecionarCliente = new Button();
            txtTelefone          = new TextBox();
            cmbEntrega           = new ComboBox();
            txtEndereco          = new TextBox();
            lblEndereco          = new Label();
            cmbPagamento         = new ComboBox();
            numTroco             = new NumericUpDown();
            lblTroco             = new Label();
            lblTrocoInfo         = new Label();
            numTaxa              = new NumericUpDown();
            txtObs               = new TextBox();
            txtBuscaProduto      = new TextBox();
            btnBuscarProduto     = new Button();
            lblDesconto          = new Label();
            numQtde              = new NumericUpDown();
            numUnitario          = new NumericUpDown();
            numDescontoItem      = new NumericUpDown();
            gridItens            = new DataGridView();
            lblTotal             = new Label();

            // Top bar
            var topBar = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = Color.FromArgb(28, 37, 65) };
            topBar.Controls.Add(new Label { Text = "Pedido Manual", ForeColor = Color.White,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold), AutoSize = true, Top = 9, Left = 12 });

            // Painel dados do cliente
            var pnlDados = new Panel { Dock = DockStyle.Top, Height = 130,
                BackColor = Color.FromArgb(15, 22, 45), Padding = new Padding(10, 0, 10, 0) };

            var lblCliente = new Label { Text = "Cliente:", Left = 10, Top = 11, AutoSize = true };
            txtNome.Left = 68; txtNome.Top = 7; txtNome.Width = 230;

            btnSelecionarCliente.Text      = "Buscar";
            btnSelecionarCliente.Left      = 305; btnSelecionarCliente.Top    = 5;
            btnSelecionarCliente.Width     = 58; btnSelecionarCliente.Height = 24;
            btnSelecionarCliente.BackColor = Color.FromArgb(63, 81, 181);
            btnSelecionarCliente.ForeColor = Color.White;
            btnSelecionarCliente.FlatStyle = FlatStyle.Flat;
            btnSelecionarCliente.Font      = new Font("Segoe UI", 8.5F);
            btnSelecionarCliente.FlatAppearance.BorderSize = 0;
            btnSelecionarCliente.Cursor    = Cursors.Hand;
            btnSelecionarCliente.Click    += BtnSelecionarCliente_Click;

            var lblTelLabel = new Label { Text = "Telefone:", Left = 420, Top = 11, AutoSize = true };
            txtTelefone.Left = 480; txtTelefone.Top = 7; txtTelefone.Width = 170;

            // Linha 2: Entrega + Endereco
            var lblEntrega = new Label { Text = "Entrega:", Left = 10, Top = 44, AutoSize = true };
            cmbEntrega.Left = 68; cmbEntrega.Top = 40; cmbEntrega.Width = 120;
            cmbEntrega.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEntrega.Items.AddRange(new object[] { "Retirada", "Entrega" });
            cmbEntrega.SelectedIndex = 0;
            cmbEntrega.SelectedIndexChanged += (_, __) => AtualizarVisibilidade();

            lblEndereco.Text = "End.:"; lblEndereco.Left = 200; lblEndereco.Top = 44; lblEndereco.AutoSize = true;
            txtEndereco.Left = 234; txtEndereco.Top = 40; txtEndereco.Width = 380;

            // Linha 3: Pagamento | Troco | TrocoInfo | Taxa
            var lblPag = new Label { Text = "Pagamento:", Left = 10, Top = 77, AutoSize = true };
            cmbPagamento.Left = 82; cmbPagamento.Top = 73; cmbPagamento.Width = 115;
            cmbPagamento.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPagamento.Items.AddRange(new object[] { "Dinheiro", "Cartao", "Pix" });
            cmbPagamento.SelectedIndex = 0;
            cmbPagamento.SelectedIndexChanged += (_, __) => AtualizarVisibilidade();

            lblTroco.Text = "Troco R$:"; lblTroco.Left = 210; lblTroco.Top = 77; lblTroco.AutoSize = true;
            numTroco.Left = 277; numTroco.Top = 73; numTroco.Width = 88;
            numTroco.DecimalPlaces = 2; numTroco.Maximum = 9999;
            numTroco.ValueChanged += (_, __) => AtualizarTrocoInfo();

            lblTrocoInfo.Text = ""; lblTrocoInfo.Left = 372; lblTrocoInfo.Top = 77;
            lblTrocoInfo.AutoSize = true; lblTrocoInfo.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblTrocoInfo.ForeColor = Color.FromArgb(39, 174, 96);

            lblTaxa = new Label { Text = "Taxa R$:", Left = 420, Top = 77, AutoSize = true };
            numTaxa.Left = 472; numTaxa.Top = 73; numTaxa.Width = 88;
            numTaxa.DecimalPlaces = 2; numTaxa.Maximum = 999;
            numTaxa.ValueChanged += (_, __) => AtualizarTotal();

            // Linha 4: Obs
            var lblObs = new Label { Text = "Obs.:", Left = 10, Top = 107, AutoSize = true };
            txtObs.Left = 52; txtObs.Top = 104; txtObs.Width = 350;

            pnlDados.Controls.AddRange(new Control[]
            {
                lblCliente,  txtNome, btnSelecionarCliente,
                lblTelLabel, txtTelefone,
                lblEntrega,  cmbEntrega, lblEndereco, txtEndereco,
                lblPag,      cmbPagamento, lblTroco, numTroco, lblTrocoInfo, lblTaxa, numTaxa,
                lblObs,      txtObs
            });
            // dark palette for dados panel
            var cDarkInput = Color.FromArgb(20, 28, 55);
            var cLblDados  = Color.FromArgb(160, 175, 210);
            foreach (Control c in pnlDados.Controls)
            {
                if (c is Label l && l != lblTrocoInfo) l.ForeColor = cLblDados;
                else if (c is TextBox tb) { tb.BackColor = cDarkInput; tb.ForeColor = Color.White; }
                else if (c is ComboBox cb) { cb.BackColor = cDarkInput; cb.ForeColor = Color.White; }
                else if (c is NumericUpDown nud) { nud.BackColor = cDarkInput; nud.ForeColor = Color.White; }
            }

            // Barra de adicao de itens
            var pnlItem = new Panel { Dock = DockStyle.Top, Height = 78, BackColor = Color.FromArgb(22, 30, 55) };

            var lblProd = new Label { Text = "Produto:", Left = 10, Top = 12, AutoSize = true };
            txtBuscaProduto.Left = 72; txtBuscaProduto.Top = 8; txtBuscaProduto.Width = 280;
            txtBuscaProduto.PlaceholderText = "Digite ou pesquise...";
            txtBuscaProduto.TextChanged += TxtBusca_TextChanged;

            btnBuscarProduto.Text      = "Buscar";
            btnBuscarProduto.Left      = 360; btnBuscarProduto.Top    = 6;
            btnBuscarProduto.Width     = 58;  btnBuscarProduto.Height = 26;
            btnBuscarProduto.BackColor = Color.FromArgb(63, 81, 181);
            btnBuscarProduto.ForeColor = Color.White;
            btnBuscarProduto.FlatStyle = FlatStyle.Flat;
            btnBuscarProduto.Font      = new Font("Segoe UI", 8.5F);
            btnBuscarProduto.FlatAppearance.BorderSize = 0;
            btnBuscarProduto.Cursor    = Cursors.Hand;
            btnBuscarProduto.Click    += BtnBuscarProduto_Click;

            // Row 2: Qtde | Unit. R$ | Desc. % | + Adicionar
            var lblQtde = new Label { Text = "Qtde:", Left = 10, Top = 48, AutoSize = true };
            numQtde.Left = 50; numQtde.Top = 44; numQtde.Width = 60;
            numQtde.Minimum = 1; numQtde.Maximum = 999; numQtde.Value = 1;

            var lblUnit = new Label { Text = "Valor:", Left = 124, Top = 48, AutoSize = true };
            numUnitario.Left = 184; numUnitario.Top = 44; numUnitario.Width = 100;
            numUnitario.DecimalPlaces = 2; numUnitario.Maximum = 9999;
            numUnitario.ValueChanged += NumUnitario_ValueChanged;

            var lblDescItem = new Label { Text = "Desc. %:", Left = 296, Top = 48, AutoSize = true };
            numDescontoItem.Left = 356; numDescontoItem.Top = 44; numDescontoItem.Width = 70;
            numDescontoItem.DecimalPlaces = 1; numDescontoItem.Minimum = 0; numDescontoItem.Maximum = 100;

            var btnAdd = new Button { Text = "+ Adicionar", Top = 42, Width = 130, Height = 28,
                BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdicionarItem_Click;
            pnlItem.SizeChanged += (_, __) => { btnAdd.Left = pnlItem.Width - btnAdd.Width - 10; };
            btnAdd.Left = 840;

            lblDesconto.Text = ""; lblDesconto.Left = 460; lblDesconto.Top = 48;
            lblDesconto.AutoSize = true; lblDesconto.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblDesconto.ForeColor = Color.FromArgb(192, 57, 43); lblDesconto.Visible = false;

            pnlItem.Controls.AddRange(new Control[]
                { lblProd, txtBuscaProduto, btnBuscarProduto,
                  lblQtde, numQtde, lblUnit, numUnitario,
                  lblDescItem, numDescontoItem, lblDesconto, btnAdd });
            // dark palette for item panel labels
            foreach (Control c in pnlItem.Controls)
                if (c is Label l && l != lblDesconto) l.ForeColor = Color.FromArgb(160, 175, 210);

            // Grid de itens
            gridItens.Dock = DockStyle.Fill;
            gridItens.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridItens.ReadOnly = true; gridItens.AllowUserToAddRows = false;
            gridItens.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridItens.RowHeadersVisible = false;
            gridItens.BackgroundColor = Color.FromArgb(15, 22, 45);
            gridItens.Font = new Font("Segoe UI", 9F);
            gridItens.BorderStyle = BorderStyle.None;
            gridItens.GridColor = Color.FromArgb(30, 40, 70);
            gridItens.DefaultCellStyle.BackColor = Color.FromArgb(15, 22, 45);
            gridItens.DefaultCellStyle.ForeColor = Color.White;
            gridItens.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            gridItens.DefaultCellStyle.SelectionForeColor = Color.White;
            gridItens.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(28, 37, 65);
            gridItens.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridItens.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nome",     HeaderText = "Produto",     FillWeight = 40 });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Qtde",     HeaderText = "Qtde",        FillWeight = 8  });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Unitario", HeaderText = "Valor",       FillWeight = 14 });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Desconto", HeaderText = "Desc.",       FillWeight = 10 });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Subtotal", HeaderText = "Subtotal R$", FillWeight = 14 });
            var colDel = new DataGridViewButtonColumn { Name = "Remover", HeaderText = "", Text = "Remover",
                UseColumnTextForButtonValue = true, FillWeight = 8, MinimumWidth = 60, FlatStyle = FlatStyle.Flat };
            gridItens.Columns.Add(colDel);
            gridItens.CellClick += GridItens_CellClick;

            // Rodape
            var pnlRodape = new Panel { Dock = DockStyle.Bottom, Height = 52, BackColor = Color.FromArgb(20, 27, 50) };

            var btnSal = new Button { Text = "Salvar Pedido", Top = 11, Width = 140, Height = 30,
                BackColor = Color.FromArgb(33, 150, 243), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnSal.FlatAppearance.BorderSize = 0;
            btnSal.Click += BtnSalvar_Click;

            var btnCanc = new Button { Text = "Cancelar", Top = 11, Width = 100, Height = 30,
                BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnCanc.FlatAppearance.BorderSize = 0;
            btnCanc.Click += (_, __) => Close();

            lblTotal.Text = "Total: R$ 0,00"; lblTotal.Top = 16;
            lblTotal.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTotal.ForeColor = Color.FromArgb(46, 213, 115); lblTotal.AutoSize = true;

            pnlRodape.SizeChanged += (_, __) =>
            {
                int pairW = btnSal.Width + 8 + btnCanc.Width;
                int pairX = (pnlRodape.Width - pairW) / 2;
                btnSal.Left  = pairX;
                btnCanc.Left = pairX + btnSal.Width + 8;
                lblTotal.Left = pnlRodape.Width - lblTotal.PreferredWidth - 10;
            };
            pnlRodape.HandleCreated += (_, __) =>
            {
                int pairW = btnSal.Width + 8 + btnCanc.Width;
                int pairX = (pnlRodape.Width - pairW) / 2;
                btnSal.Left  = pairX;
                btnCanc.Left = pairX + btnSal.Width + 8;
                lblTotal.Left = pnlRodape.Width - lblTotal.PreferredWidth - 10;
            };
            pnlRodape.Controls.AddRange(new Control[] { lblTotal, btnSal, btnCanc });

            // Montagem
            Controls.Add(gridItens);
            Controls.Add(pnlItem);
            Controls.Add(pnlDados);
            Controls.Add(topBar);
            Controls.Add(pnlRodape);

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(990, 680);
            Font                = new Font("Segoe UI", 9F);
            MinimumSize         = new Size(950, 580);
            StartPosition       = FormStartPosition.CenterParent;
            Text                = "Novo Pedido Manual";

            AtualizarVisibilidade();
        }

        private TextBox       txtNome;
        private Button        btnSelecionarCliente;
        private TextBox       txtTelefone;
        private ComboBox      cmbEntrega;
        private TextBox       txtEndereco;
        private Label         lblEndereco;
        private ComboBox      cmbPagamento;
        private NumericUpDown numTroco;
        private Label         lblTroco;
        private Label         lblTrocoInfo;
        private NumericUpDown numTaxa;
        private Label         lblTaxa;
        private TextBox       txtObs;
        private TextBox       txtBuscaProduto;
        private Button        btnBuscarProduto;
        private Label         lblDesconto;
        private NumericUpDown numQtde;
        private NumericUpDown numUnitario;
        private NumericUpDown numDescontoItem;
        private DataGridView  gridItens;
        private Label         lblTotal;
    }
}
