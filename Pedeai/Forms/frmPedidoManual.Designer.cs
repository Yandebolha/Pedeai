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
            txtNome            = new TextBox();
            btnSelecionarCliente = new Button();
            txtTelefone  = new TextBox();
            cmbEntrega   = new ComboBox();
            txtEndereco  = new TextBox();
            lblEndereco  = new Label();
            cmbPagamento = new ComboBox();
            numTroco     = new NumericUpDown();
            lblTroco     = new Label();
            numTaxa      = new NumericUpDown();
            txtObs       = new TextBox();
            cmbProduto   = new ComboBox();
            numQtde      = new NumericUpDown();
            numUnitario  = new NumericUpDown();
            txtObsItem   = new TextBox();
            gridItens    = new DataGridView();
            lblTotal     = new Label();

            // ── Top bar ───────────────────────────────────────────────────
            var topBar = new Panel { Dock = DockStyle.Top, Height = 44,
                BackColor = Color.FromArgb(40, 40, 80) };
            topBar.Controls.Add(new Label
            {
                Text = "Pedido Manual", ForeColor = Color.White,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                AutoSize = true, Top = 8, Left = 8
            });

            // ── Painel dados ──────────────────────────────────────────────
            var pnlDados = new Panel
            {
                Dock = DockStyle.Top, Height = 175,
                BackColor = Color.FromArgb(245, 245, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Linha 1: Cliente | Telefone
            pnlDados.Controls.Add(new Label { Text = "Cliente:",  Left = 0, Top = 10, AutoSize = true });
            txtNome.Left = 60; txtNome.Top = 6; txtNome.Width = 215;

            btnSelecionarCliente.Text = "🔍 Selecionar";
            btnSelecionarCliente.Left = 280; btnSelecionarCliente.Top = 4; btnSelecionarCliente.Width = 110; btnSelecionarCliente.Height = 24;
            btnSelecionarCliente.BackColor = Color.FromArgb(63, 81, 181); btnSelecionarCliente.ForeColor = Color.White;
            btnSelecionarCliente.FlatStyle = FlatStyle.Flat; btnSelecionarCliente.Font = new Font("Segoe UI", 8F);
            btnSelecionarCliente.FlatAppearance.BorderSize = 0;
            btnSelecionarCliente.Cursor = Cursors.Hand;
            btnSelecionarCliente.Click += BtnSelecionarCliente_Click;

            pnlDados.Controls.Add(new Label { Text = "Telefone:", Left = 400, Top = 10, AutoSize = true });
            txtTelefone.Left = 466; txtTelefone.Top = 6; txtTelefone.Width = 150;

            // Linha 2: Entrega | Endereço
            pnlDados.Controls.Add(new Label { Text = "Entrega:", Left = 0, Top = 42, AutoSize = true });
            cmbEntrega.Left = 60; cmbEntrega.Top = 38; cmbEntrega.Width = 120;
            cmbEntrega.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEntrega.Items.AddRange(new object[] { "Retirada", "Entrega" });
            cmbEntrega.SelectedIndex = 0;
            cmbEntrega.SelectedIndexChanged += (_, __) => AtualizarVisibilidade();

            lblEndereco.Text = "Endereço:"; lblEndereco.Left = 192; lblEndereco.Top = 41; lblEndereco.AutoSize = true;
            txtEndereco.Left = 258; txtEndereco.Top = 38; txtEndereco.Width = 330;

            // Linha 3: Pagamento | Troco | Taxa
            pnlDados.Controls.Add(new Label { Text = "Pagamento:", Left = 0, Top = 76, AutoSize = true });
            cmbPagamento.Left = 72; cmbPagamento.Top = 72; cmbPagamento.Width = 110;
            cmbPagamento.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPagamento.Items.AddRange(new object[] { "Dinheiro", "Cartão", "Pix" });
            cmbPagamento.SelectedIndex = 0;
            cmbPagamento.SelectedIndexChanged += (_, __) => AtualizarVisibilidade();

            lblTroco.Text = "Troco para R$:"; lblTroco.Left = 194; lblTroco.Top = 75; lblTroco.AutoSize = true;
            numTroco.Left = 285; numTroco.Top = 72; numTroco.Width = 90;
            numTroco.DecimalPlaces = 2; numTroco.Maximum = 9999;

            pnlDados.Controls.Add(new Label { Text = "Taxa entrega R$:", Left = 386, Top = 75, AutoSize = true });
            numTaxa.Left = 490; numTaxa.Top = 72; numTaxa.Width = 90;
            numTaxa.DecimalPlaces = 2; numTaxa.Maximum = 999;
            numTaxa.ValueChanged += (_, __) => AtualizarTotal();

            // Linha 4: Observações
            pnlDados.Controls.Add(new Label { Text = "Obs.:", Left = 0, Top = 110, AutoSize = true });
            txtObs.Left = 45; txtObs.Top = 106; txtObs.Width = 540;

            pnlDados.Controls.AddRange(new Control[]
            {
                txtNome, btnSelecionarCliente, txtTelefone,
                cmbEntrega, lblEndereco, txtEndereco,
                cmbPagamento, lblTroco, numTroco, numTaxa,
                txtObs
            });

            // ── Barra de itens ────────────────────────────────────────────
            var pnlItem = new Panel
            {
                Dock = DockStyle.Top, Height = 42,
                BackColor = Color.FromArgb(235, 238, 250)
            };

            pnlItem.Controls.Add(new Label { Text = "Produto:", Left = 0, Top = 12, AutoSize = true });
            cmbProduto.Left = 58; cmbProduto.Top = 8; cmbProduto.Width = 260;
            cmbProduto.DropDownStyle = ComboBoxStyle.DropDown;
            cmbProduto.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbProduto.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmbProduto.SelectedIndexChanged += CmbProduto_Changed;

            pnlItem.Controls.Add(new Label { Text = "Qtde:",    Left = 330, Top = 12, AutoSize = true });
            numQtde.Left = 365; numQtde.Top = 8; numQtde.Width = 55;
            numQtde.Minimum = 1; numQtde.Maximum = 999; numQtde.Value = 1;

            pnlItem.Controls.Add(new Label { Text = "Unit.R$:", Left = 428, Top = 12, AutoSize = true });
            numUnitario.Left = 482; numUnitario.Top = 8; numUnitario.Width = 80;
            numUnitario.DecimalPlaces = 2; numUnitario.Maximum = 9999;

            pnlItem.Controls.Add(new Label { Text = "Obs:",     Left = 572, Top = 12, AutoSize = true });
            txtObsItem.Left = 598; txtObsItem.Top = 8; txtObsItem.Width = 100;

            var btnAdd = new Button
            {
                Text = "➕ Adicionar", Left = 706, Top = 6, Width = 100, Height = 26,
                BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0; btnAdd.Click += BtnAdicionarItem_Click;

            pnlItem.Controls.AddRange(new Control[]
                { cmbProduto, numQtde, numUnitario, txtObsItem, btnAdd });

            // ── Grid de itens ─────────────────────────────────────────────
            gridItens.Dock = DockStyle.Fill;
            gridItens.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridItens.ReadOnly = true; gridItens.AllowUserToAddRows = false;
            gridItens.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridItens.RowHeadersVisible = false; gridItens.BackgroundColor = Color.White;
            gridItens.Font = new Font("Segoe UI", 9F); gridItens.BorderStyle = BorderStyle.None;
            gridItens.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 80);
            gridItens.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nome",     HeaderText = "Produto",     FillWeight = 40 });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Qtde",     HeaderText = "Qtde",        FillWeight = 10 });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Unitario", HeaderText = "Unit. R$",    FillWeight = 15 });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Subtotal", HeaderText = "Subtotal R$", FillWeight = 15 });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Obs",      HeaderText = "Obs",         FillWeight = 20 });

            // ── Rodapé ────────────────────────────────────────────────────
            var pnlRodape = new Panel
            {
                Dock = DockStyle.Bottom, Height = 48,
                BackColor = Color.FromArgb(245, 245, 250)
            };

            var btnRem = new Button
            {
                Text = "🗑 Remover item", Left = 0, Top = 8, Width = 130, Height = 28,
                BackColor = Color.FromArgb(192, 57, 43), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btnRem.FlatAppearance.BorderSize = 0; btnRem.Click += BtnRemoverItem_Click;

            lblTotal.Text      = "Total: R$ 0,00";
            lblTotal.Left      = 480; lblTotal.Top = 12;
            lblTotal.Font      = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTotal.ForeColor = Color.FromArgb(30, 100, 30);
            lblTotal.AutoSize  = true;

            var btnSal = new Button
            {
                Text = "✔ Salvar Pedido", Left = 640, Top = 8, Width = 135, Height = 28,
                BackColor = Color.FromArgb(33, 150, 243), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnSal.FlatAppearance.BorderSize = 0; btnSal.Click += BtnSalvar_Click;

            var btnCanc = new Button
            {
                Text = "Cancelar", Left = 785, Top = 8, Width = 100, Height = 28,
                BackColor = Color.FromArgb(120, 120, 120), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnCanc.FlatAppearance.BorderSize = 0; btnCanc.Click += (_, __) => Close();

            pnlRodape.Controls.AddRange(new Control[] { btnRem, lblTotal, btnSal, btnCanc });

            Controls.Add(gridItens);
            Controls.Add(pnlItem);
            Controls.Add(pnlDados);
            Controls.Add(topBar);
            Controls.Add(pnlRodape);

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(844, 641);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(800, 620);
            StartPosition = FormStartPosition.CenterParent;
            Text = "Novo Pedido Manual";

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
