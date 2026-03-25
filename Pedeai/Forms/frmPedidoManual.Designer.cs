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
            numTaxa              = new NumericUpDown();
            txtObs               = new TextBox();
            cmbProduto           = new ComboBox();
            numQtde              = new NumericUpDown();
            numUnitario          = new NumericUpDown();
            txtObsItem           = new TextBox();
            gridItens            = new DataGridView();
            lblTotal             = new Label();

            // ── Top bar ───────────────────────────────────────────────────────
            var topBar = new Panel { Dock = DockStyle.Top, Height = 46,
                BackColor = Color.FromArgb(40, 40, 80) };
            topBar.Controls.Add(new Label
            {
                Text = "Pedido Manual", ForeColor = Color.White,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                AutoSize = true, Top = 9, Left = 12
            });

            // ── Painel de dados do cliente ─────────────────────────────────────
            var pnlDados = new Panel
            {
                Dock = DockStyle.Top, Height = 152,
                BackColor = Color.FromArgb(245, 245, 250),
                Padding = new Padding(10, 0, 10, 0)
            };

            // Linha 1 — Cliente | Selecionar | Telefone
            var lblCliente = new Label { Text = "Cliente:", Left = 10, Top = 11, AutoSize = true };
            txtNome.Left = 68; txtNome.Top = 7; txtNome.Width = 230;

            btnSelecionarCliente.Text      = "🔍 Selecionar";
            btnSelecionarCliente.Left      = 305; btnSelecionarCliente.Top  = 5;
            btnSelecionarCliente.Width     = 108; btnSelecionarCliente.Height = 24;
            btnSelecionarCliente.BackColor = Color.FromArgb(63, 81, 181);
            btnSelecionarCliente.ForeColor = Color.White;
            btnSelecionarCliente.FlatStyle = FlatStyle.Flat;
            btnSelecionarCliente.Font      = new Font("Segoe UI", 8.5F);
            btnSelecionarCliente.FlatAppearance.BorderSize = 0;
            btnSelecionarCliente.Cursor    = Cursors.Hand;
            btnSelecionarCliente.Click    += BtnSelecionarCliente_Click;

            var lblTelLabel = new Label { Text = "Telefone:", Left = 426, Top = 11, AutoSize = true };
            txtTelefone.Left = 490; txtTelefone.Top = 7; txtTelefone.Width = 170;
            txtTelefone.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            // Linha 2 — Tipo de Entrega | Endereço
            var lblEntrega = new Label { Text = "Entrega:", Left = 10, Top = 44, AutoSize = true };
            cmbEntrega.Left = 68; cmbEntrega.Top = 40; cmbEntrega.Width = 120;
            cmbEntrega.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEntrega.Items.AddRange(new object[] { "Retirada", "Entrega" });
            cmbEntrega.SelectedIndex = 0;
            cmbEntrega.SelectedIndexChanged += (_, __) => AtualizarVisibilidade();

            lblEndereco.Text = "Endereço:"; lblEndereco.Left = 200; lblEndereco.Top = 44; lblEndereco.AutoSize = true;
            txtEndereco.Left = 262; txtEndereco.Top = 40; txtEndereco.Width = 450;
            txtEndereco.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Linha 3 — Pagamento | Troco | Taxa
            var lblPag = new Label { Text = "Pagamento:", Left = 10, Top = 77, AutoSize = true };
            cmbPagamento.Left = 82; cmbPagamento.Top = 73; cmbPagamento.Width = 115;
            cmbPagamento.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPagamento.Items.AddRange(new object[] { "Dinheiro", "Cartão", "Pix" });
            cmbPagamento.SelectedIndex = 0;
            cmbPagamento.SelectedIndexChanged += (_, __) => AtualizarVisibilidade();

            lblTroco.Text = "Troco para R$:"; lblTroco.Left = 210; lblTroco.Top = 77; lblTroco.AutoSize = true;
            numTroco.Left = 305; numTroco.Top = 73; numTroco.Width = 95;
            numTroco.DecimalPlaces = 2; numTroco.Maximum = 9999;

            var lblTaxa = new Label { Text = "Taxa entrega R$:", Left = 412, Top = 77, AutoSize = true };
            numTaxa.Left = 522; numTaxa.Top = 73; numTaxa.Width = 95;
            numTaxa.DecimalPlaces = 2; numTaxa.Maximum = 999;
            numTaxa.ValueChanged += (_, __) => AtualizarTotal();

            // Linha 4 — Observações do pedido
            var lblObs = new Label { Text = "Obs.:", Left = 10, Top = 110, AutoSize = true };
            txtObs.Left = 52; txtObs.Top = 107; txtObs.Width = 600;
            txtObs.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            pnlDados.Controls.AddRange(new Control[]
            {
                lblCliente,  txtNome, btnSelecionarCliente,
                lblTelLabel, txtTelefone,
                lblEntrega,  cmbEntrega, lblEndereco, txtEndereco,
                lblPag,      cmbPagamento, lblTroco, numTroco, lblTaxa, numTaxa,
                lblObs,      txtObs
            });

            // ── Barra de adição de itens ──────────────────────────────────────
            var pnlItem = new Panel
            {
                Dock = DockStyle.Top, Height = 46,
                BackColor = Color.FromArgb(228, 232, 250)
            };

            var lblProd = new Label { Text = "Produto:", Left = 10, Top = 14, AutoSize = true };
            cmbProduto.Left = 70; cmbProduto.Top = 10; cmbProduto.Width = 270;
            cmbProduto.DropDownStyle = ComboBoxStyle.DropDown;
            cmbProduto.AutoCompleteMode   = AutoCompleteMode.SuggestAppend;
            cmbProduto.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmbProduto.SelectedIndexChanged += CmbProduto_Changed;

            var lblQtde = new Label { Text = "Qtde:", Left = 352, Top = 14, AutoSize = true };
            numQtde.Left = 385; numQtde.Top = 10; numQtde.Width = 58;
            numQtde.Minimum = 1; numQtde.Maximum = 999; numQtde.Value = 1;

            var lblUnit = new Label { Text = "Unit. R$:", Left = 454, Top = 14, AutoSize = true };
            numUnitario.Left = 507; numUnitario.Top = 10; numUnitario.Width = 90;
            numUnitario.DecimalPlaces = 2; numUnitario.Maximum = 9999;

            var btnAdd = new Button
            {
                Text = "➕ Adicionar", Top = 8, Width = 130, Height = 28,
                BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdicionarItem_Click;

            pnlItem.SizeChanged += (_, __) =>
            {
                btnAdd.Left = pnlItem.Width - btnAdd.Width - 8;
            };
            btnAdd.Left = 860;

            pnlItem.Controls.AddRange(new Control[]
                { lblProd, cmbProduto, lblQtde, numQtde, lblUnit, numUnitario, btnAdd });

            // ── Grid de itens ─────────────────────────────────────────────────
            gridItens.Dock = DockStyle.Fill;
            gridItens.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridItens.ReadOnly = true; gridItens.AllowUserToAddRows = false;
            gridItens.SelectionMode    = DataGridViewSelectionMode.FullRowSelect;
            gridItens.RowHeadersVisible = false;
            gridItens.BackgroundColor  = Color.White;
            gridItens.Font             = new Font("Segoe UI", 9F);
            gridItens.BorderStyle      = BorderStyle.None;
            gridItens.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 80);
            gridItens.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridItens.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nome",     HeaderText = "Produto",      FillWeight = 40 });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Qtde",     HeaderText = "Qtde",         FillWeight = 8  });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Unitario", HeaderText = "Unit. R$",     FillWeight = 14 });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Subtotal", HeaderText = "Subtotal R$",  FillWeight = 14 });
            var colDel = new DataGridViewButtonColumn
            {
                Name = "Remover", HeaderText = "", Text = "🗑 Remover",
                UseColumnTextForButtonValue = true,
                FillWeight = 12, MinimumWidth = 80,
                FlatStyle = FlatStyle.Flat
            };
            gridItens.Columns.Add(colDel);
            gridItens.CellClick += GridItens_CellClick;

            // ── Rodapé ────────────────────────────────────────────────────────
            var pnlRodape = new Panel
            {
                Dock = DockStyle.Bottom, Height = 52,
                BackColor = Color.FromArgb(240, 242, 248)
            };

            // btnRem removido — exclusão agora é feita pela coluna do grid

            var btnSal = new Button
            {
                Text = "✔ Salvar Pedido", Top = 11, Width = 150, Height = 30,
                BackColor = Color.FromArgb(33, 150, 243), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Bottom | AnchorStyles.None
            };
            btnSal.FlatAppearance.BorderSize = 0;
            btnSal.Click += BtnSalvar_Click;

            var btnCanc = new Button
            {
                Text = "✖ Cancelar", Top = 11, Width = 110, Height = 30,
                BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Bottom | AnchorStyles.None
            };
            btnCanc.FlatAppearance.BorderSize = 0;
            btnCanc.Click += (_, __) => Close();

            lblTotal.Text      = "Total: R$ 0,00";
            lblTotal.Top       = 16;
            lblTotal.Font      = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTotal.ForeColor = Color.FromArgb(25, 111, 61);
            lblTotal.AutoSize  = true;
            lblTotal.Anchor    = AnchorStyles.Bottom | AnchorStyles.None;

            // Centraliza Salvar + Cancelar; Total fica ancorado na direita
            void RepositionarRodape()
            {
                int margem = 10;
                // Total na direita
                lblTotal.Left = pnlRodape.Width - lblTotal.PreferredWidth - margem;
                // Salvar + Cancelar centralizados
                int pairW    = btnSal.Width + 8 + btnCanc.Width;
                int pairX    = (pnlRodape.Width - pairW) / 2;
                btnSal.Left  = pairX;
                btnCanc.Left = pairX + btnSal.Width + 8;
            }
            pnlRodape.SizeChanged   += (_, __) => RepositionarRodape();
            pnlRodape.HandleCreated += (_, __) => RepositionarRodape();

            pnlRodape.Controls.AddRange(new Control[] { lblTotal, btnSal, btnCanc });

            // ── Montagem do formulário ────────────────────────────────────────
            Controls.Add(gridItens);
            Controls.Add(pnlItem);
            Controls.Add(pnlDados);
            Controls.Add(topBar);
            Controls.Add(pnlRodape);

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(990, 660);
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
        private NumericUpDown numTaxa;
        private TextBox       txtObs;
        private ComboBox      cmbProduto;
        private NumericUpDown numQtde;
        private NumericUpDown numUnitario;
        private TextBox       txtObsItem;
        private DataGridView  gridItens;
        private Label         lblTotal;
    }
}
