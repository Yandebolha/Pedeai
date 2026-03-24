using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public class frmPedidoManual : Form
    {
        private readonly PedidoBLL      _pedidoBLL  = new PedidoBLL();
        private readonly MercadoriaBLL  _mercBLL    = new MercadoriaBLL();

        // ── Cliente
        private TextBox       txtNome       = new TextBox();
        private TextBox       txtTelefone   = new TextBox();

        // ── Entrega
        private ComboBox      cmbEntrega    = new ComboBox();
        private TextBox       txtEndereco   = new TextBox();
        private Label         lblEndereco   = new Label();

        // ── Pagamento
        private ComboBox      cmbPagamento  = new ComboBox();
        private NumericUpDown numTroco      = new NumericUpDown();
        private Label         lblTroco      = new Label();
        private NumericUpDown numTaxa       = new NumericUpDown();

        // ── Observações
        private TextBox       txtObs        = new TextBox();

        // ── Itens
        private ComboBox      cmbProduto    = new ComboBox();
        private NumericUpDown numQtde       = new NumericUpDown();
        private NumericUpDown numUnitario   = new NumericUpDown();
        private TextBox       txtObsItem    = new TextBox();
        private DataGridView  gridItens     = new DataGridView();
        private List<ItemPedidoWeb> _itens  = new List<ItemPedidoWeb>();

        // ── Total
        private Label         lblTotal      = new Label();

        private static readonly Color CorHeader = Color.FromArgb(40, 40, 80);

        public frmPedidoManual()
        {
            Text          = "Novo Pedido Manual";
            Size          = new Size(860, 680);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize   = new Size(800, 620);
            Font          = new Font("Segoe UI", 9);
            BuildUI();
            CarregarProdutos();
        }

        private void BuildUI()
        {
            // ── Top bar ──────────────────────────────────────────────────────
            var topBar = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = CorHeader, Padding = new Padding(8, 6, 8, 0) };
            topBar.Controls.Add(new Label { Text = "Pedido Manual", ForeColor = Color.White,
                Font = new Font("Segoe UI", 13, FontStyle.Bold), AutoSize = true, Top = 8, Left = 8 });

            // ── Painel superior (dados do pedido) ─────────────────────────────
            var pnlDados = new Panel { Dock = DockStyle.Top, Height = 175, BackColor = Color.FromArgb(245, 245, 250),
                Padding = new Padding(12, 8, 12, 8), BorderStyle = BorderStyle.FixedSingle };

            // Linha 1: Nome | Telefone
            L(pnlDados, "Cliente:", 0, 8);
            txtNome = new TextBox { Left = 60, Top = 6, Width = 300 }; pnlDados.Controls.Add(txtNome);
            L(pnlDados, "Telefone:", 374, 8);
            txtTelefone = new TextBox { Left = 440, Top = 6, Width = 150 }; pnlDados.Controls.Add(txtTelefone);

            // Linha 2: Entrega | Endereço
            L(pnlDados, "Entrega:", 0, 40);
            cmbEntrega = new ComboBox { Left = 60, Top = 38, Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbEntrega.Items.AddRange(new object[] { "Retirada", "Entrega" });
            cmbEntrega.SelectedIndex = 0;
            cmbEntrega.SelectedIndexChanged += (s, e) => AtualizarVisibilidade();
            pnlDados.Controls.Add(cmbEntrega);

            lblEndereco = new Label { Text = "Endereço:", Left = 192, Top = 41, AutoSize = true };
            pnlDados.Controls.Add(lblEndereco);
            txtEndereco = new TextBox { Left = 258, Top = 38, Width = 330 }; pnlDados.Controls.Add(txtEndereco);

            // Linha 3: Pagamento | Troco | Taxa entrega
            L(pnlDados, "Pagamento:", 0, 74);
            cmbPagamento = new ComboBox { Left = 72, Top = 72, Width = 110, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbPagamento.Items.AddRange(new object[] { "Dinheiro", "Cartão", "Pix" });
            cmbPagamento.SelectedIndex = 0;
            cmbPagamento.SelectedIndexChanged += (s, e) => AtualizarVisibilidade();
            pnlDados.Controls.Add(cmbPagamento);

            lblTroco = new Label { Text = "Troco para R$:", Left = 194, Top = 75, AutoSize = true };
            pnlDados.Controls.Add(lblTroco);
            numTroco = new NumericUpDown { Left = 285, Top = 72, Width = 90, DecimalPlaces = 2, Maximum = 9999 };
            pnlDados.Controls.Add(numTroco);

            L(pnlDados, "Taxa entrega R$:", 386, 75);
            numTaxa = new NumericUpDown { Left = 490, Top = 72, Width = 90, DecimalPlaces = 2, Maximum = 999 };
            pnlDados.Controls.Add(numTaxa);
            numTaxa.ValueChanged += (s, e) => AtualizarTotal();

            // Linha 4: Observações
            L(pnlDados, "Obs.:", 0, 108);
            txtObs = new TextBox { Left = 45, Top = 106, Width = 540 }; pnlDados.Controls.Add(txtObs);

            // ── Linha de adição de itens ─────────────────────────────────────
            var pnlItem = new Panel { Dock = DockStyle.Top, Height = 42, BackColor = Color.FromArgb(235, 238, 250),
                Padding = new Padding(12, 4, 12, 0) };

            L(pnlItem, "Produto:", 0, 10);
            cmbProduto = new ComboBox { Left = 58, Top = 8, Width = 260, DropDownStyle = ComboBoxStyle.DropDown,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend, AutoCompleteSource = AutoCompleteSource.ListItems };
            cmbProduto.SelectedIndexChanged += CmbProduto_Changed;
            pnlItem.Controls.Add(cmbProduto);

            L(pnlItem, "Qtde:", 330, 10);
            numQtde = new NumericUpDown { Left = 365, Top = 8, Width = 55, Minimum = 1, Maximum = 999, Value = 1 };
            pnlItem.Controls.Add(numQtde);

            L(pnlItem, "Unit.R$:", 428, 10);
            numUnitario = new NumericUpDown { Left = 482, Top = 8, Width = 80, DecimalPlaces = 2, Maximum = 9999 };
            pnlItem.Controls.Add(numUnitario);

            L(pnlItem, "Obs:", 572, 10);
            txtObsItem = new TextBox { Left = 598, Top = 8, Width = 100 }; pnlItem.Controls.Add(txtObsItem);

            var btnAdd = new Button { Text = "➕ Adicionar", Left = 706, Top = 6, Width = 100, Height = 26,
                BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdicionarItem_Click;
            pnlItem.Controls.Add(btnAdd);

            // ── Grid de itens ────────────────────────────────────────────────
            gridItens.Dock = DockStyle.Fill;
            gridItens.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridItens.ReadOnly = true; gridItens.AllowUserToAddRows = false;
            gridItens.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridItens.RowHeadersVisible = false;
            gridItens.BackgroundColor = Color.White;
            gridItens.Font = new Font("Segoe UI", 9); gridItens.BorderStyle = BorderStyle.None;
            gridItens.ColumnHeadersDefaultCellStyle.BackColor = CorHeader;
            gridItens.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nome",     HeaderText = "Produto",     FillWeight = 40 });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Qtde",     HeaderText = "Qtde",        FillWeight = 10 });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Unitario", HeaderText = "Unit. R$",    FillWeight = 15 });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Subtotal", HeaderText = "Subtotal R$", FillWeight = 15 });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Obs",      HeaderText = "Obs",         FillWeight = 20 });

            // ── Rodapé ───────────────────────────────────────────────────────
            var pnlRodape = new Panel { Dock = DockStyle.Bottom, Height = 48, BackColor = Color.FromArgb(245, 245, 250),
                Padding = new Padding(12, 8, 12, 0) };

            var btnRemover = new Button { Text = "🗑 Remover item", Left = 0, Top = 8, Width = 130, Height = 28,
                BackColor = Color.FromArgb(192, 57, 43), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnRemover.FlatAppearance.BorderSize = 0;
            btnRemover.Click += BtnRemoverItem_Click;
            pnlRodape.Controls.Add(btnRemover);

            lblTotal = new Label { Text = "Total: R$ 0,00", Left = 480, Top = 12,
                Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = Color.FromArgb(30, 100, 30), AutoSize = true };
            pnlRodape.Controls.Add(lblTotal);

            var btnSalvar = Botao("✔ Salvar Pedido", Color.FromArgb(33, 150, 243)); btnSalvar.Left = 640; btnSalvar.Top = 8;
            btnSalvar.Width = 135;
            btnSalvar.Click += BtnSalvar_Click;
            pnlRodape.Controls.Add(btnSalvar);

            var btnCanc = Botao("Cancelar", Color.FromArgb(120, 120, 120)); btnCanc.Left = 785; btnCanc.Top = 8;
            btnCanc.Click += (s, e) => Close();
            pnlRodape.Controls.Add(btnCanc);

            Controls.Add(gridItens);
            Controls.Add(pnlItem);
            Controls.Add(pnlDados);
            Controls.Add(topBar);
            Controls.Add(pnlRodape);

            AtualizarVisibilidade();
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private void L(Panel p, string t, int x, int y) =>
            p.Controls.Add(new Label { Text = t, Left = x, Top = y + 2, AutoSize = true });

        private Button Botao(string t, Color c) =>
            new Button { Text = t, BackColor = c, ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                         Width = 100, Height = 28, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };

        // ── Visibilidade dinâmica ─────────────────────────────────────────────
        private void AtualizarVisibilidade()
        {
            bool entrega   = cmbEntrega.SelectedIndex == 1;
            bool dinheiro  = cmbPagamento.SelectedIndex == 0;
            lblEndereco.Visible = txtEndereco.Visible = entrega;
            lblTroco.Visible    = numTroco.Visible    = dinheiro;
        }

        // ── Produtos ─────────────────────────────────────────────────────────
        private void CarregarProdutos()
        {
            try
            {
                var dt = _mercBLL.Listar();
                cmbProduto.Items.Clear();
                foreach (System.Data.DataRow r in dt.Rows)
                    cmbProduto.Items.Add(new ProdItem(
                        Convert.ToInt32(r["Codigo"]),
                        r["Nome"]?.ToString() ?? "",
                        r["Preco"] == DBNull.Value ? 0 : Convert.ToDecimal(r["Preco"])));
            }
            catch { }
        }

        private void CmbProduto_Changed(object sender, EventArgs e)
        {
            if (cmbProduto.SelectedItem is ProdItem p)
                numUnitario.Value = p.Preco;
        }

        // ── Itens ─────────────────────────────────────────────────────────────
        private void BtnAdicionarItem_Click(object sender, EventArgs e)
        {
            if (numUnitario.Value <= 0) { MessageBox.Show("Informe o preço unitário."); return; }
            string nome = "";
            int codMerc = 0;
            if (cmbProduto.SelectedItem is ProdItem p) { nome = p.Nome; codMerc = p.Codigo; }
            else nome = cmbProduto.Text.Trim();
            if (string.IsNullOrWhiteSpace(nome)) { MessageBox.Show("Selecione ou digite o produto."); return; }

            var item = new ItemPedidoWeb
            {
                Codigo_Mercadoria   = codMerc,
                itpwNome_Mercadoria = nome,
                itpwQtde            = (int)numQtde.Value,
                itpwPreco_Unitario  = numUnitario.Value,
                itpwSubtotal        = numUnitario.Value * numQtde.Value,
                itpwObservacoes     = txtObsItem.Text.Trim(),
            };
            _itens.Add(item);

            gridItens.Rows.Add(item.itpwNome_Mercadoria, item.itpwQtde,
                item.itpwPreco_Unitario.ToString("N2"),
                item.itpwSubtotal.ToString("N2"),
                item.itpwObservacoes);

            // Reset
            cmbProduto.SelectedIndex = -1; cmbProduto.Text = "";
            numQtde.Value = 1; numUnitario.Value = 0; txtObsItem.Clear();
            AtualizarTotal();
        }

        private void BtnRemoverItem_Click(object sender, EventArgs e)
        {
            if (gridItens.SelectedRows.Count == 0) return;
            int idx = gridItens.SelectedRows[0].Index;
            _itens.RemoveAt(idx);
            gridItens.Rows.RemoveAt(idx);
            AtualizarTotal();
        }

        private void AtualizarTotal()
        {
            decimal sub   = 0;
            foreach (var i in _itens) sub += i.itpwSubtotal;
            decimal total = sub + numTaxa.Value;
            lblTotal.Text = $"Total: R$ {total:N2}";
        }

        // ── Salvar ────────────────────────────────────────────────────────────
        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text)) { MessageBox.Show("Informe o nome do cliente."); return; }
            if (_itens.Count == 0) { MessageBox.Show("Adicione ao menos um item."); return; }

            decimal sub = 0; foreach (var i in _itens) sub += i.itpwSubtotal;

            var pedido = new PedidoWeb
            {
                pediNome_Cliente     = txtNome.Text.Trim(),
                pediTelefone_Cliente = txtTelefone.Text.Trim(),
                pediTipo_Entrega     = cmbEntrega.SelectedIndex,   // 0=Retirada 1=Entrega
                pediForma_Pagamento  = cmbPagamento.SelectedIndex, // 0=Dinheiro 1=Cartão 2=Pix
                pediSubtotal         = sub,
                pediTaxa_Entrega     = numTaxa.Value,
                pediValor_Total      = sub + numTaxa.Value,
                pediTroco_Para       = cmbPagamento.SelectedIndex == 0 && numTroco.Value > 0 ? numTroco.Value : (decimal?)null,
                pediEndereco_Entrega = txtEndereco.Text.Trim(),
                pediObservacoes      = txtObs.Text.Trim(),
                pediOrigem           = 2, // Manual
            };

            var erro = _pedidoBLL.InserirManual(pedido, _itens);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro ao salvar: " + erro); return; }

            MessageBox.Show($"Pedido {pedido.pediNumero} criado com sucesso!", "Sucesso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }

        private class ProdItem
        {
            public int Codigo; public string Nome; public decimal Preco;
            public ProdItem(int c, string n, decimal p) { Codigo = c; Nome = n; Preco = p; }
            public override string ToString() => Nome;
        }
    }
}
