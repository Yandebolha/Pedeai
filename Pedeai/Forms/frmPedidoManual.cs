using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmPedidoManual : Form
    {
        private readonly PedidoBLL     _pedidoBLL = new PedidoBLL();
        private readonly MercadoriaBLL _mercBLL   = new MercadoriaBLL();
        private readonly List<ItemPedidoWeb> _itens = new List<ItemPedidoWeb>();
        private static readonly Color CorHeader = Color.FromArgb(40, 40, 80);
        private int _codigoCliente = 0;

        public frmPedidoManual()
        {
            InitializeComponent();
            if (!DesignMode) CarregarProdutos();
        }

        // ── Seleção de cliente ────────────────────────────────────────────────
        private void BtnSelecionarCliente_Click(object sender, EventArgs e)
        {
            using var frm = new frmSelecionarCliente();
            if (frm.ShowDialog(this) == DialogResult.OK && frm.ClienteSelecionado != null)
            {
                var c = frm.ClienteSelecionado;
                _codigoCliente        = c.Codigo;
                txtNome.Text          = c.clieNome_RazaoSocial ?? "";
                txtTelefone.Text      = !string.IsNullOrWhiteSpace(c.clieCelular)
                                        ? c.clieCelular
                                        : c.clieTelefone ?? "";
                if (!string.IsNullOrWhiteSpace(c.clieEndereco))
                    txtEndereco.Text  = $"{c.clieEndereco}, {c.clieNumero}".Trim(',', ' ');
            }
        }

        // ── Visibilidade dinâmica ─────────────────────────────────────────────
        private void AtualizarVisibilidade()
        {
            bool entrega  = cmbEntrega.SelectedIndex == 1;
            bool dinheiro = cmbPagamento.SelectedIndex == 0;
            lblEndereco.Visible = txtEndereco.Visible = entrega;
            lblTroco.Visible    = numTroco.Visible    = dinheiro;
            numTaxa.Visible = entrega;
            // Zera taxa quando não é entrega para não impactar no total
            if (!entrega) numTaxa.Value = 0;
            AtualizarTotal();
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
            };
            _itens.Add(item);

            gridItens.Rows.Add(item.itpwNome_Mercadoria, item.itpwQtde,
                item.itpwPreco_Unitario.ToString("N2"),
                item.itpwSubtotal.ToString("N2"));

            // Reset
            cmbProduto.SelectedIndex = -1; cmbProduto.Text = "";
            numQtde.Value = 1; numUnitario.Value = 0;
            AtualizarTotal();
        }

        private void GridItens_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (gridItens.Columns[e.ColumnIndex].Name != "Remover") return;
            _itens.RemoveAt(e.RowIndex);
            gridItens.Rows.RemoveAt(e.RowIndex);
            AtualizarTotal();
        }

        private void AtualizarTotal()
        {
            decimal sub  = 0;
            foreach (var i in _itens) sub += i.itpwSubtotal;
            bool entrega = cmbEntrega.SelectedIndex == 1;
            decimal taxa = entrega ? numTaxa.Value : 0;
            decimal total = sub + taxa;
            lblTotal.Text = $"Total: R$ {total:N2}";
        }

        // ── Salvar ────────────────────────────────────────────────────────────
        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text)) { MessageBox.Show("Informe o nome do cliente."); return; }
            if (_itens.Count == 0) { MessageBox.Show("Adicione ao menos um item."); return; }

            decimal sub = 0; foreach (var i in _itens) sub += i.itpwSubtotal;
            bool ehEntrega   = cmbEntrega.SelectedIndex == 1;
            decimal taxa     = ehEntrega ? numTaxa.Value : 0m;
            decimal total    = sub + taxa;

            var pedido = new PedidoWeb
            {
                Codigo_Cliente       = _codigoCliente,
                pediNome_Cliente     = txtNome.Text.Trim(),
                pediTelefone_Cliente = txtTelefone.Text.Trim(),
                pediTipo_Entrega     = cmbEntrega.SelectedIndex,   // 0=Retirada 1=Entrega
                pediForma_Pagamento  = cmbPagamento.SelectedIndex, // 0=Dinheiro 1=Cartão 2=Pix
                pediSubtotal         = sub,
                pediTaxa_Entrega     = taxa,
                pediValor_Total      = total,
                pediTroco_Para       = cmbPagamento.SelectedIndex == 0 && numTroco.Value > 0 ? numTroco.Value : (decimal?)null,
                pediEndereco_Entrega = ehEntrega ? txtEndereco.Text.Trim() : "",
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
