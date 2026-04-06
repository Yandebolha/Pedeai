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
        private PedidoBLL     _pedidoBLL;
        private MercadoriaBLL _mercBLL;
        private ClienteBLL    _clienteBLL;
        private CupomBLL      _cupomBLL;
        private readonly List<ItemPedidoWeb> _itens = new List<ItemPedidoWeb>();
        private readonly List<ProdItem>      _produtos = new List<ProdItem>();
        private ProdItem _produtoSelecionado = null;
        private Cupom    _cupomAplicado      = null;
        private decimal  _descontoCupom      = 0m;
        private int _codigoCliente = 0;

        public frmPedidoManual()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _pedidoBLL = new PedidoBLL(); _mercBLL = new MercadoriaBLL(); _clienteBLL = new ClienteBLL(); _cupomBLL = new CupomBLL();
            Load  += (_, __) => CarregarProdutos();
            Shown += (_, __) => PnlAddItem_SizeChanged(null, EventArgs.Empty);
        }

        // -- Selecao de cliente ----------------------------------------------
        private void BtnSelecionarCliente_Click(object sender, EventArgs e)
        {
            using var frm = new frmSelecionarCliente();
            if (frm.ShowDialog(this) == DialogResult.OK && frm.ClienteSelecionado != null)
            {
                var c = frm.ClienteSelecionado;
                _codigoCliente   = c.Codigo;
                txtNome.Text     = c.clieNome_RazaoSocial ?? "";
                txtTelefone.Text = !string.IsNullOrWhiteSpace(c.clieCelular)
                                   ? c.clieCelular : c.clieTelefone ?? "";
                if (!string.IsNullOrWhiteSpace(c.clieEndereco))
                {
                    var endParts = new System.Collections.Generic.List<string>();
                    var rua = (c.clieEndereco + ", " + c.clieNumero).Trim(',', ' ');
                    if (!string.IsNullOrWhiteSpace(rua)) endParts.Add(rua);
                    if (!string.IsNullOrWhiteSpace(c.clieComplemento)) endParts.Add(c.clieComplemento.Trim());
                    txtEndereco.Text = string.Join(" - ", endParts);
                }
            }
        }

        // -- Visibilidade dinamica -------------------------------------------
        private void AtualizarVisibilidade()
        {
            bool entrega  = cmbEntrega.SelectedIndex == 1;
            bool dinheiro = cmbPagamento.SelectedIndex == 0;
            lblEndereco.Visible = txtEndereco.Visible = entrega;
            lblTroco.Visible    = numTroco.Visible    = dinheiro;
            lblTaxa.Visible     = numTaxa.Visible     = entrega;
            if (!entrega) numTaxa.Value = 0;
            AtualizarTotal();
        }

        // -- Produtos --------------------------------------------------------
        private void CarregarProdutos()
        {
            try
            {
                var dt = _mercBLL.Listar();
                _produtos.Clear();
                var collection = new AutoCompleteStringCollection();
                foreach (System.Data.DataRow r in dt.Rows)
                {
                    var p = new ProdItem(
                        Convert.ToInt32(r["Codigo"]),
                        r["Nome"]?.ToString() ?? "",
                        r["Preco"] == System.DBNull.Value ? 0 : Convert.ToDecimal(r["Preco"]));
                    _produtos.Add(p);
                    collection.Add(p.Nome);
                }
                txtBuscaProduto.AutoCompleteCustomSource = collection;
                txtBuscaProduto.AutoCompleteMode         = AutoCompleteMode.SuggestAppend;
                txtBuscaProduto.AutoCompleteSource       = AutoCompleteSource.CustomSource;
            }
            catch { }
        }

        private void BtnBuscarProduto_Click(object sender, EventArgs e)
        {
            using var dlg = new Form();
            dlg.Text            = "Selecionar Produto";
            dlg.StartPosition   = FormStartPosition.CenterParent;
            dlg.Size            = new Size(600, 440);
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox     = dlg.MinimizeBox = false;
            dlg.BackColor       = Color.FromArgb(245, 237, 216);

            var txtFiltro = new TextBox { Dock = DockStyle.Top, Height = 28,
                BackColor = Color.White, ForeColor = Color.FromArgb(50, 50, 50),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10F),
                PlaceholderText = "Filtrar por nome..." };

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.FromArgb(245, 237, 216), GridColor = Color.FromArgb(200, 185, 160),
                DefaultCellStyle = { BackColor = Color.FromArgb(245, 237, 216), ForeColor = Color.FromArgb(50, 50, 50),
                    SelectionBackColor = Color.FromArgb(224, 113, 42), SelectionForeColor = Color.White },
                ColumnHeadersDefaultCellStyle = { BackColor = Color.FromArgb(176, 110, 42), ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold) },
                BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 9F),
                MultiSelect = false
            };
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nome",  HeaderText = "Produto",   FillWeight = 60 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Preco", HeaderText = "Pre\u00e7o R$",  FillWeight = 30 });

            void Preencher(string filtro)
            {
                grid.Rows.Clear();
                foreach (var p in _produtos)
                    if (string.IsNullOrWhiteSpace(filtro) ||
                        p.Nome.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0)
                        grid.Rows.Add(p.Nome, p.Preco.ToString("N2"));
            }
            Preencher("");

            txtFiltro.TextChanged += (_, __) => Preencher(txtFiltro.Text.Trim());

            ProdItem escolhido = null;
            grid.CellDoubleClick += (_, __) =>
            {
                if (grid.CurrentRow == null) return;
                var nome = grid.CurrentRow.Cells["Nome"].Value?.ToString() ?? "";
                escolhido = _produtos.Find(p => p.Nome == nome);
                dlg.DialogResult = DialogResult.OK;
            };

            var btnOk = new Button { Text = "Selecionar", Dock = DockStyle.Bottom, Height = 34,
                BackColor = Color.FromArgb(87, 120, 38), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += (_, __) =>
            {
                if (grid.CurrentRow == null) return;
                var nome = grid.CurrentRow.Cells["Nome"].Value?.ToString() ?? "";
                escolhido = _produtos.Find(p => p.Nome == nome);
                dlg.DialogResult = DialogResult.OK;
            };

            dlg.Controls.Add(grid);
            dlg.Controls.Add(btnOk);
            dlg.Controls.Add(txtFiltro);

            if (dlg.ShowDialog(this) == DialogResult.OK && escolhido != null)
            {
                _produtoSelecionado = escolhido;
                txtBuscaProduto.Text = escolhido.Nome;
                numUnitario.Value    = escolhido.Preco > numUnitario.Maximum ? numUnitario.Maximum : escolhido.Preco;
                AtualizarDesconto();
            }
        }

        private void TxtBusca_TextChanged(object sender, EventArgs e)
        {
            var nome  = txtBuscaProduto.Text.Trim();
            var found = _produtos.Find(p => p.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
            if (found != null)
            {
                _produtoSelecionado = found;
                numUnitario.Value   = found.Preco > numUnitario.Maximum ? numUnitario.Maximum : found.Preco;
            }
            else
            {
                _produtoSelecionado = null;
            }
            AtualizarDesconto();
        }

        private void NumUnitario_ValueChanged(object sender, EventArgs e)
        {
            AtualizarDesconto();
            AtualizarTotal();
        }

        private void AtualizarDesconto()
        {
            if (_produtoSelecionado != null
                && _produtoSelecionado.Preco > 0
                && numUnitario.Value > 0
                && numUnitario.Value < _produtoSelecionado.Preco)
            {
                decimal pct = (1m - numUnitario.Value / _produtoSelecionado.Preco) * 100m;
                lblDesconto.Text    = "Desconto: " + pct.ToString("0.0") + "%";
                lblDesconto.Visible = true;
            }
            else
            {
                lblDesconto.Visible = false;
            }
        }

        // -- Itens -----------------------------------------------------------
        private void BtnAdicionarItem_Click(object sender, EventArgs e)
        {
            if (numUnitario.Value <= 0) { MessageBox.Show("Informe o preco unitario."); return; }
            string nome    = txtBuscaProduto.Text.Trim();
            int    codMerc = _produtoSelecionado?.Codigo ?? 0;
            if (string.IsNullOrWhiteSpace(nome)) { MessageBox.Show("Informe o produto."); return; }

            decimal descPct  = Math.Max(0, Math.Min(100, numDescontoItem.Value));
            decimal unitFinal = numUnitario.Value * (1m - descPct / 100m);

            var item = new ItemPedidoWeb
            {
                Codigo_Mercadoria   = codMerc,
                itpwNome_Mercadoria = nome,
                itpwQtde            = (int)numQtde.Value,
                itpwPreco_Unitario  = numUnitario.Value,
                itpwSubtotal        = unitFinal * numQtde.Value,
            };
            _itens.Add(item);

            string descStr = descPct > 0 ? descPct.ToString("0.#") + "%" : "";
            gridItens.Rows.Add(item.itpwNome_Mercadoria, item.itpwQtde,
                item.itpwPreco_Unitario.ToString("N2"), descStr,
                item.itpwSubtotal.ToString("N2"));

            txtBuscaProduto.Text   = "";
            _produtoSelecionado    = null;
            lblDesconto.Visible    = false;
            numQtde.Value          = 1;
            numUnitario.Value      = 0;
            numDescontoItem.Value  = 0;
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
            bool    entrega = cmbEntrega.SelectedIndex == 1;
            decimal taxa    = entrega ? numTaxa.Value : 0;
            // Recalcula desconto do cupom sobre o subtotal atual
            if (_cupomAplicado != null)
            {
                _descontoCupom = _cupomAplicado.cupomTipo == "PERCENTUAL"
                    ? sub * _cupomAplicado.cupomValor / 100m
                    : _cupomAplicado.cupomValor;
                string tipoStr = _cupomAplicado.cupomTipo == "PERCENTUAL"
                    ? $"{_cupomAplicado.cupomValor:0.#}%"
                    : $"R$ {_cupomAplicado.cupomValor:N2}";
                lblCupomInfo.Text = $"\u2714 {_cupomAplicado.cupomDescricao} ({tipoStr}) \u2212 R$ {_descontoCupom:N2}";
            }
            decimal total = sub + taxa - _descontoCupom;
            if (total < 0) total = 0;
            lblTotal.Text   = "R$ " + total.ToString("N2");
            AtualizarTrocoInfo();
        }

        private void AtualizarTrocoInfo()
        {
            if (cmbPagamento.SelectedIndex == 0 && numTroco.Value > 0)
            {
                decimal sub   = 0; foreach (var i in _itens) sub += i.itpwSubtotal;
                bool    ehEnt = cmbEntrega.SelectedIndex == 1;
                decimal total = sub + (ehEnt ? numTaxa.Value : 0);
                decimal troco = numTroco.Value - total;
                if (troco >= 0)
                {
                    lblTrocoInfo.Text      = "Troco: R$ " + troco.ToString("N2");
                    lblTrocoInfo.ForeColor = Color.FromArgb(39, 174, 96);
                }
                else
                {
                    lblTrocoInfo.Text      = "Falta R$ " + (-troco).ToString("N2");
                    lblTrocoInfo.ForeColor = Color.FromArgb(192, 57, 43);
                }
                lblTrocoInfo.Visible = true;
            }
            else
            {
                lblTrocoInfo.Visible = false;
            }
        }

        // -- Salvar ----------------------------------------------------------
        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text)) { MessageBox.Show("Informe o nome do cliente."); return; }
            if (_itens.Count == 0) { MessageBox.Show("Adicione ao menos um item."); return; }

            decimal sub = 0; foreach (var i in _itens) sub += i.itpwSubtotal;
            bool    ehEntrega = cmbEntrega.SelectedIndex == 1;
            decimal taxa      = ehEntrega ? numTaxa.Value : 0m;
            decimal total     = Math.Max(0, sub + taxa - _descontoCupom);

            var pedido = new PedidoWeb
            {
                Codigo_Cliente       = _codigoCliente,
                pediNome_Cliente     = txtNome.Text.Trim(),
                pediTelefone_Cliente = txtTelefone.Text.Trim(),
                pediTipo_Entrega     = cmbEntrega.SelectedIndex,
                pediForma_Pagamento  = cmbPagamento.SelectedIndex,
                pediSubtotal         = sub,
                pediTaxa_Entrega     = taxa,
                pediDesconto         = _descontoCupom,
                pediValor_Total      = total,
                pediTroco_Para       = cmbPagamento.SelectedIndex == 0 && numTroco.Value > 0 ? numTroco.Value : (decimal?)null,
                pediEndereco_Entrega = ehEntrega ? txtEndereco.Text.Trim() : "",
                pediObservacoes      = txtObs.Text.Trim(),
                pediOrigem           = 2,
            };

            var erro = _pedidoBLL.InserirManual(pedido, _itens);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro ao salvar: " + erro); return; }

            if (_codigoCliente > 0)
                _clienteBLL.IncrementarTotais(_codigoCliente, total);

            MessageBox.Show("Pedido " + pedido.pediNumero + " criado com sucesso!", "Sucesso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }

        // -- Cupom -----------------------------------------------------------
        private void BtnAplicarCupom_Click(object sender, EventArgs e)
        {
            if (btnAplicarCupom.Text == "Remover") { RemoverCupom(); return; }

            string codigo = txtCupom.Text.Trim();
            if (string.IsNullOrWhiteSpace(codigo)) { RemoverCupom(); return; }

            decimal subtotal = 0; foreach (var i in _itens) subtotal += i.itpwSubtotal;
            var (cupom, erro) = _cupomBLL.ValidarEObter(codigo, subtotal);
            if (!string.IsNullOrEmpty(erro))
            {
                lblCupomInfo.Text      = erro;
                lblCupomInfo.ForeColor = Color.FromArgb(192, 57, 43);
                return;
            }

            _cupomAplicado = cupom;
            string tipoStr = cupom.cupomTipo == "PERCENTUAL"
                ? $"{cupom.cupomValor:0.#}%"
                : $"R$ {cupom.cupomValor:N2}";
            lblCupomInfo.ForeColor    = Color.FromArgb(39, 174, 96);
            btnAplicarCupom.Text      = "Remover";
            btnAplicarCupom.BackColor = Color.FromArgb(80, 40, 35);
            btnAplicarCupom.ForeColor = Color.FromArgb(200, 130, 120);
            AtualizarTotal();
        }

        private void RemoverCupom()
        {
            _cupomAplicado  = null;
            _descontoCupom  = 0m;
            txtCupom.Text   = "";
            lblCupomInfo.Text = "";
            btnAplicarCupom.Text      = "Aplicar";
            btnAplicarCupom.BackColor = Color.FromArgb(52, 73, 94);
            btnAplicarCupom.ForeColor = Color.FromArgb(170, 200, 240);
            AtualizarTotal();
        }

        // -- Meio a Meio -----------------------------------------------------
        private void BtnMeioAMeio_Click(object sender, EventArgs e)
        {
            if (_produtos.Count == 0) { MessageBox.Show("Nenhum produto carregado."); return; }

            using var dlg = new Form();
            dlg.Text            = "Pedido Fracionado";
            dlg.StartPosition   = FormStartPosition.CenterParent;
            dlg.Size            = new Size(700, 430);
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox     = dlg.MinimizeBox = false;
            dlg.BackColor       = Color.FromArgb(245, 237, 216);

            DataGridView MkGrid()
            {
                var g = new DataGridView
                {
                    Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    BackgroundColor = Color.FromArgb(250, 245, 238), GridColor = Color.FromArgb(200, 185, 160),
                    DefaultCellStyle = { BackColor = Color.White, ForeColor = Color.FromArgb(50, 50, 50),
                        SelectionBackColor = Color.FromArgb(224, 113, 42), SelectionForeColor = Color.White },
                    ColumnHeadersDefaultCellStyle = { BackColor = Color.FromArgb(176, 110, 42), ForeColor = Color.White,
                        Font = new Font("Segoe UI", 8.5F, FontStyle.Bold) },
                    BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 9F), MultiSelect = false,
                };
                g.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nome",  HeaderText = "Produto",   FillWeight = 70 });
                g.Columns.Add(new DataGridViewTextBoxColumn { Name = "Preco", HeaderText = "Pre\u00e7o R$", FillWeight = 30 });
                foreach (var p in _produtos) g.Rows.Add(p.Nome, p.Preco.ToString("N2"));
                return g;
            }

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 36, BackColor = Color.FromArgb(176, 110, 42) };
            var lblTit = new Label { Text = "Selecione os dois produtos:", Dock = DockStyle.Fill,
                ForeColor = Color.White, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 0, 0, 0) };
            pnlTop.Controls.Add(lblTit);

            var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, BackColor = Color.Transparent };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            Label MkLbl(string t) => new Label { Text = t, Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(70, 70, 70), Font = new Font("Segoe UI", 8.5F),
                TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(6, 0, 0, 0) };

            var grid1 = MkGrid();
            var grid2 = MkGrid();
            tbl.Controls.Add(MkLbl("1\u00ba Sabor"), 0, 0);
            tbl.Controls.Add(MkLbl("2\u00ba Sabor"), 1, 0);
            tbl.Controls.Add(grid1, 0, 1);
            tbl.Controls.Add(grid2, 1, 1);

            var btnOk = new Button { Text = "Adicionar Pedido Fracionado", Dock = DockStyle.Bottom, Height = 38,
                BackColor = Color.FromArgb(87, 120, 38), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnOk.FlatAppearance.BorderSize = 0;

            ProdItem escolha1 = null, escolha2 = null;
            btnOk.Click += (_, __) =>
            {
                if (grid1.CurrentRow == null || grid2.CurrentRow == null)
                { MessageBox.Show("Selecione um produto em cada coluna.", "Pedido Fracionado"); return; }
                var n1 = grid1.CurrentRow.Cells["Nome"].Value?.ToString() ?? "";
                var n2 = grid2.CurrentRow.Cells["Nome"].Value?.ToString() ?? "";
                escolha1 = _produtos.Find(p => p.Nome == n1);
                escolha2 = _produtos.Find(p => p.Nome == n2);
                if (escolha1 == null || escolha2 == null) { MessageBox.Show("Produto n\u00e3o encontrado."); return; }
                dlg.DialogResult = DialogResult.OK;
            };

            dlg.Controls.Add(tbl);
            dlg.Controls.Add(btnOk);
            dlg.Controls.Add(pnlTop);

            if (dlg.ShowDialog(this) == DialogResult.OK && escolha1 != null && escolha2 != null)
            {
                decimal precoFinal = Math.Max(escolha1.Preco, escolha2.Preco);
                decimal descPct    = numDescontoItem.Value;
                decimal unitFinal  = precoFinal * (1m - descPct / 100m);
                int     qty        = (int)numQtde.Value;
                string  nome       = $"\u00BD {escolha1.Nome} + \u00BD {escolha2.Nome}";

                var item = new ItemPedidoWeb
                {
                    Codigo_Mercadoria   = escolha1.Codigo,
                    itpwNome_Mercadoria = nome,
                    itpwQtde            = qty,
                    itpwPreco_Unitario  = precoFinal,
                    itpwSubtotal        = unitFinal * qty,
                };
                _itens.Add(item);
                string descStr = descPct > 0 ? descPct.ToString("0.#") + "%" : "";
                gridItens.Rows.Add(nome, qty, precoFinal.ToString("N2"), descStr, item.itpwSubtotal.ToString("N2"));
                numQtde.Value = 1;
                AtualizarTotal();
            }
        }

        private class ProdItem
        {
            public int Codigo; public string Nome; public decimal Preco;
            public ProdItem(int c, string n, decimal p) { Codigo = c; Nome = n; Preco = p; }
        }
    }
}
