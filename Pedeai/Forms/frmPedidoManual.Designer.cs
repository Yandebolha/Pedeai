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
            // palette
            var cBg     = Color.FromArgb(11, 16, 36);
            var cPanel  = Color.FromArgb(17, 24, 50);
            var cCard   = Color.FromArgb(22, 31, 60);
            var cInput  = Color.FromArgb(14, 21, 46);
            var cBorder = Color.FromArgb(34, 46, 82);
            var cLbl    = Color.FromArgb(110, 130, 175);
            var cSecHd  = Color.FromArgb(80, 105, 160);
            var cAccent = Color.FromArgb(52, 152, 219);
            var cGreen  = Color.FromArgb(39, 174, 96);
            var cWhite  = Color.White;
            var fntLbl  = new Font("Segoe UI", 8.5F);
            var fntInp  = new Font("Segoe UI", 9.5F);
            var fntSec  = new Font("Segoe UI", 7.5F, FontStyle.Bold);

            // helpers
            Label MkLbl(string t) => new Label { Text = t, ForeColor = cLbl, AutoSize = true, Font = fntLbl };
            Label MkSec(string t) => new Label { Text = t.ToUpper(), ForeColor = cSecHd, AutoSize = true, Font = fntSec };
            Panel MkDiv() => new Panel { BackColor = cBorder, Height = 1 };

            // form setup
            BackColor = cBg; ForeColor = cWhite;
            Text = "Novo Pedido Manual";
            ClientSize    = new Size(1050, 680);
            MinimumSize   = new Size(900, 560);
            StartPosition = FormStartPosition.CenterParent;
            Font          = new Font("Segoe UI", 9F);
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode       = AutoScaleMode.Font;

            // controls
            txtNome              = new TextBox  { BackColor = cInput, ForeColor = cWhite, Font = fntInp, BorderStyle = BorderStyle.FixedSingle };
            txtTelefone          = new TextBox  { BackColor = cInput, ForeColor = cWhite, Font = fntInp, BorderStyle = BorderStyle.FixedSingle };
            txtEndereco          = new TextBox  { BackColor = cInput, ForeColor = cWhite, Font = fntInp, BorderStyle = BorderStyle.FixedSingle };
            txtObs               = new TextBox  { BackColor = cInput, ForeColor = cWhite, Font = fntInp, BorderStyle = BorderStyle.FixedSingle };
            txtBuscaProduto      = new TextBox  { BackColor = cInput, ForeColor = cWhite, Font = fntInp, BorderStyle = BorderStyle.FixedSingle, PlaceholderText = "Pesquisar produto..." };
            cmbEntrega           = new ComboBox { BackColor = cInput, ForeColor = cWhite, Font = fntInp, FlatStyle = FlatStyle.Flat, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbPagamento         = new ComboBox { BackColor = cInput, ForeColor = cWhite, Font = fntInp, FlatStyle = FlatStyle.Flat, DropDownStyle = ComboBoxStyle.DropDownList };
            numTroco             = new NumericUpDown { BackColor = cInput, ForeColor = cWhite, Font = fntInp, DecimalPlaces = 2, Maximum = 9999 };
            numTaxa              = new NumericUpDown { BackColor = cInput, ForeColor = cWhite, Font = fntInp, DecimalPlaces = 2, Maximum = 999 };
            numQtde              = new NumericUpDown { BackColor = cInput, ForeColor = cWhite, Font = fntInp, Minimum = 1, Maximum = 999, Value = 1 };
            numUnitario          = new NumericUpDown { BackColor = cInput, ForeColor = cWhite, Font = fntInp, DecimalPlaces = 2, Maximum = 99999 };
            numDescontoItem      = new NumericUpDown { BackColor = cInput, ForeColor = cWhite, Font = fntInp, DecimalPlaces = 1, Maximum = 100 };
            gridItens            = new DataGridView();
            lblTotal             = new Label();
            lblEndereco          = new Label();
            lblTroco             = new Label();
            lblTrocoInfo         = new Label();
            lblTaxa              = new Label();
            lblDesconto          = new Label();
            btnSelecionarCliente = new Button();
            btnBuscarProduto     = new Button();

            cmbEntrega.Items.AddRange(new object[] { "Retirada", "Entrega" });
            cmbEntrega.SelectedIndex = 0;
            cmbEntrega.SelectedIndexChanged += (_, __) => AtualizarVisibilidade();
            cmbPagamento.Items.AddRange(new object[] { "Dinheiro", "Cart\u00e3o", "Pix" });
            cmbPagamento.SelectedIndex = 0;
            cmbPagamento.SelectedIndexChanged += (_, __) => AtualizarVisibilidade();
            numTroco.ValueChanged    += (_, __) => AtualizarTrocoInfo();
            numTaxa.ValueChanged     += (_, __) => AtualizarTotal();
            numUnitario.ValueChanged += NumUnitario_ValueChanged;
            txtBuscaProduto.TextChanged += TxtBusca_TextChanged;

            // TOP BAR
            var topBar = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = cPanel };
            topBar.Controls.Add(new Label
            {
                Text = "Pedido Manual", ForeColor = cWhite,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold), AutoSize = true, Top = 10, Left = 16
            });

            // FOOTER
            var pnlRodape = new Panel { Dock = DockStyle.Bottom, Height = 54, BackColor = cPanel };
            btnSal = new Button
            {
                Text = "Salvar Pedido", Height = 34, Width = 148, Top = 10,
                BackColor = cAccent, ForeColor = cWhite, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnSal.FlatAppearance.BorderSize = 0;
            btnSal.Click += BtnSalvar_Click;
            btnCanc = new Button
            {
                Text = "Cancelar", Height = 34, Width = 98, Top = 10,
                BackColor = Color.FromArgb(40, 50, 85), ForeColor = Color.FromArgb(120, 140, 180),
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F), Cursor = Cursors.Hand
            };
            btnCanc.FlatAppearance.BorderSize = 0;
            btnCanc.Click += (_, __) => Close();
            lblTotal.Text = "R$ 0,00";
            lblTotal.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTotal.ForeColor = cGreen; lblTotal.AutoSize = true; lblTotal.Top = 12;
            var lblTotalTag = new Label { Text = "Total", ForeColor = cLbl, Font = fntLbl, AutoSize = true, Top = 18 };
            void LayoutFooter() {
                int cx = (pnlRodape.Width - btnSal.Width - 8 - btnCanc.Width) / 2;
                btnSal.Left = cx; btnCanc.Left = cx + btnSal.Width + 8;
                lblTotal.Left = pnlRodape.Width - lblTotal.PreferredWidth - 20;
                lblTotalTag.Left = lblTotal.Left - lblTotalTag.PreferredWidth - 6;
            }
            pnlRodape.SizeChanged  += (_, __) => LayoutFooter();
            pnlRodape.HandleCreated += (_, __) => LayoutFooter();
            pnlRodape.Controls.AddRange(new Control[] { lblTotalTag, lblTotal, btnSal, btnCanc });

            // MAIN SPLIT
            var mainArea = new Panel { Dock = DockStyle.Fill, BackColor = cBg };

            // SIDEBAR
            var sidebar = new Panel { Dock = DockStyle.Left, Width = 310, BackColor = cPanel };
            var sideContent = new Panel { Dock = DockStyle.Fill, BackColor = cPanel };

            const int sx = 14, sw = 282, sh = 26, sg = 6, ss = 10;
            int sy = 12;

            // CLIENTE
            var s1 = MkSec("Cliente");
            s1.SetBounds(sx, sy, sw, 16); sideContent.Controls.Add(s1); sy += 20;
            var lNome = MkLbl("Nome / Raz\u00e3o Social");
            lNome.SetBounds(sx, sy, sw, 16); sideContent.Controls.Add(lNome); sy += 18;
            txtNome.SetBounds(sx, sy, sw - 68, sh); sideContent.Controls.Add(txtNome);
            btnSelecionarCliente.Text = "Buscar";
            btnSelecionarCliente.BackColor = cAccent; btnSelecionarCliente.ForeColor = cWhite;
            btnSelecionarCliente.FlatStyle = FlatStyle.Flat;
            btnSelecionarCliente.Font = new Font("Segoe UI", 8F);
            btnSelecionarCliente.FlatAppearance.BorderSize = 0;
            btnSelecionarCliente.Cursor = Cursors.Hand;
            btnSelecionarCliente.SetBounds(sx + sw - 64, sy, 64, sh);
            btnSelecionarCliente.Click += BtnSelecionarCliente_Click;
            sideContent.Controls.Add(btnSelecionarCliente);
            sy += sh + sg;
            var lTel = MkLbl("Telefone");
            lTel.SetBounds(sx, sy, sw, 16); sideContent.Controls.Add(lTel); sy += 18;
            txtTelefone.SetBounds(sx, sy, sw, sh); sideContent.Controls.Add(txtTelefone);
            sy += sh + ss;
            var d1 = MkDiv(); d1.SetBounds(0, sy, 310, 1); sideContent.Controls.Add(d1); sy += 1 + ss;

            // ENTREGA
            var s2 = MkSec("Entrega");
            s2.SetBounds(sx, sy, sw, 16); sideContent.Controls.Add(s2); sy += 20;
            cmbEntrega.SetBounds(sx, sy, sw, sh); sideContent.Controls.Add(cmbEntrega);
            sy += sh + sg;
            lblEndereco.Text = "Endere\u00e7o"; lblEndereco.ForeColor = cLbl; lblEndereco.AutoSize = true; lblEndereco.Font = fntLbl;
            lblEndereco.SetBounds(sx, sy, sw, 16); sideContent.Controls.Add(lblEndereco); sy += 18;
            txtEndereco.SetBounds(sx, sy, sw, sh); sideContent.Controls.Add(txtEndereco);
            sy += sh + sg;
            lblTaxa.Text = "Taxa de Entrega (R$)"; lblTaxa.ForeColor = cLbl; lblTaxa.AutoSize = true; lblTaxa.Font = fntLbl;
            lblTaxa.SetBounds(sx, sy, sw, 16); sideContent.Controls.Add(lblTaxa); sy += 18;
            numTaxa.SetBounds(sx, sy, 130, sh); sideContent.Controls.Add(numTaxa);
            sy += sh + ss;
            var d2 = MkDiv(); d2.SetBounds(0, sy, 310, 1); sideContent.Controls.Add(d2); sy += 1 + ss;

            // PAGAMENTO
            var s3 = MkSec("Pagamento");
            s3.SetBounds(sx, sy, sw, 16); sideContent.Controls.Add(s3); sy += 20;
            cmbPagamento.SetBounds(sx, sy, sw, sh); sideContent.Controls.Add(cmbPagamento);
            sy += sh + sg;
            lblTroco.Text = "Troco para (R$)"; lblTroco.ForeColor = cLbl; lblTroco.AutoSize = true; lblTroco.Font = fntLbl;
            lblTroco.SetBounds(sx, sy, sw, 16); sideContent.Controls.Add(lblTroco); sy += 18;
            numTroco.SetBounds(sx, sy, 130, sh); sideContent.Controls.Add(numTroco);
            lblTrocoInfo.Text = ""; lblTrocoInfo.ForeColor = cGreen; lblTrocoInfo.AutoSize = true;
            lblTrocoInfo.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblTrocoInfo.SetBounds(sx + 140, sy + 4, 140, 18); sideContent.Controls.Add(lblTrocoInfo);
            sy += sh + ss;
            var d3 = MkDiv(); d3.SetBounds(0, sy, 310, 1); sideContent.Controls.Add(d3); sy += 1 + ss;

            // OBSERVACOES
            var s4 = MkSec("Observa\u00e7\u00f5es");
            s4.SetBounds(sx, sy, sw, 16); sideContent.Controls.Add(s4); sy += 20;
            txtObs.SetBounds(sx, sy, sw, sh); sideContent.Controls.Add(txtObs);

            sidebar.Controls.Add(sideContent);
            var sideDiv = new Panel { Dock = DockStyle.Left, Width = 1, BackColor = cBorder };

            // RIGHT AREA
            var rightArea = new Panel { Dock = DockStyle.Fill, BackColor = cBg };

            // ADD ITEM PANEL
            var pnlAddItem = new Panel { Dock = DockStyle.Top, Height = 84, BackColor = cCard };
            var aiSec = MkSec("Adicionar Item");
            aiSec.SetBounds(14, 10, 200, 16); pnlAddItem.Controls.Add(aiSec);

            txtBuscaProduto.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnBuscarProduto.Text = "Buscar";
            btnBuscarProduto.BackColor = Color.FromArgb(38, 50, 90); btnBuscarProduto.ForeColor = cLbl;
            btnBuscarProduto.FlatStyle = FlatStyle.Flat; btnBuscarProduto.Font = fntLbl;
            btnBuscarProduto.Height = 26; btnBuscarProduto.Width = 64;
            btnBuscarProduto.FlatAppearance.BorderSize = 1; btnBuscarProduto.FlatAppearance.BorderColor = cBorder;
            btnBuscarProduto.Cursor = Cursors.Hand;
            btnBuscarProduto.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBuscarProduto.Click += BtnBuscarProduto_Click;

            var lQtde = MkLbl("Qtde");
            numQtde.Width = 68;
            var lVal  = MkLbl("Valor");
            numUnitario.Width = 90;
            var lDsc  = MkLbl("Desc.%");
            numDescontoItem.Width = 68;
            lblDesconto.ForeColor = Color.FromArgb(231, 76, 60); lblDesconto.Font = fntLbl;
            lblDesconto.AutoSize = true; lblDesconto.Visible = false;

            btnAdd = new Button
            {
                Text = "+  Adicionar", Height = 26, Width = 124,
                BackColor = cGreen, ForeColor = cWhite, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdicionarItem_Click;

            pnlAddItem.Controls.AddRange(new Control[]
            {
                txtBuscaProduto, btnBuscarProduto,
                lQtde, numQtde, lVal, numUnitario, lDsc, numDescontoItem, lblDesconto, btnAdd
            });

            pnlAddItem.SizeChanged += (_, __) =>
            {
                int pw = pnlAddItem.Width - 14;
                txtBuscaProduto.SetBounds(14, 28, pw - 82, 26);
                btnBuscarProduto.SetBounds(pw - 64, 28, 64, 26);
                int x = 14;
                lQtde.SetBounds(x, 60, 34, 18); x += 36;
                numQtde.SetBounds(x, 57, 68, 26); x += 76;
                lVal.SetBounds(x, 60, 36, 18); x += 38;
                numUnitario.SetBounds(x, 57, 90, 26); x += 98;
                lDsc.SetBounds(x, 60, 44, 18); x += 46;
                numDescontoItem.SetBounds(x, 57, 68, 26); x += 76;
                lblDesconto.SetBounds(x, 61, 120, 16);
                btnAdd.SetBounds(pw - 110, 57, 124, 26);
            };

            // GRID
            gridItens.Dock = DockStyle.Fill;
            gridItens.ReadOnly = true; gridItens.AllowUserToAddRows = false;
            gridItens.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridItens.RowHeadersVisible = false; gridItens.MultiSelect = false;
            gridItens.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridItens.BackgroundColor = cBg; gridItens.GridColor = cBorder;
            gridItens.BorderStyle = BorderStyle.None;
            gridItens.Font = new Font("Segoe UI", 9F);
            gridItens.RowTemplate.Height = 32;
            gridItens.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            gridItens.DefaultCellStyle.BackColor           = cBg;
            gridItens.DefaultCellStyle.ForeColor           = cWhite;
            gridItens.DefaultCellStyle.SelectionBackColor  = Color.FromArgb(30, 70, 120);
            gridItens.DefaultCellStyle.SelectionForeColor  = cWhite;
            gridItens.DefaultCellStyle.Padding             = new Padding(6, 0, 0, 0);
            gridItens.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(14, 20, 45);
            gridItens.ColumnHeadersDefaultCellStyle.BackColor   = cCard;
            gridItens.ColumnHeadersDefaultCellStyle.ForeColor   = cLbl;
            gridItens.ColumnHeadersDefaultCellStyle.Font        = new Font("Segoe UI", 8F, FontStyle.Bold);
            gridItens.ColumnHeadersHeight = 30;
            gridItens.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nome",     HeaderText = "Produto",  FillWeight = 45 });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Qtde",     HeaderText = "Qtde",     FillWeight = 8  });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Unitario", HeaderText = "Valor",    FillWeight = 12 });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Desconto", HeaderText = "Desc.",    FillWeight = 10 });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Subtotal", HeaderText = "Subtotal", FillWeight = 14 });
            var colDel = new DataGridViewButtonColumn
            {
                Name = "Remover", HeaderText = "", Text = "\u2715",
                UseColumnTextForButtonValue = true, FillWeight = 7, MinimumWidth = 42, FlatStyle = FlatStyle.Flat
            };
            gridItens.Columns.Add(colDel);
            gridItens.CellClick += GridItens_CellClick;

            rightArea.Controls.Add(gridItens);
            rightArea.Controls.Add(pnlAddItem);

            mainArea.Controls.Add(rightArea);
            mainArea.Controls.Add(sideDiv);
            mainArea.Controls.Add(sidebar);

            Controls.Add(mainArea);
            Controls.Add(topBar);
            Controls.Add(pnlRodape);

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
        internal Button       btnSal;
        internal Button       btnCanc;
        internal Button       btnAdd;
    }
}