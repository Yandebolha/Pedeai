using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmEntradaMercadoria : Form
    {
        private readonly EntradaMercadoriaBLL _bll  = new EntradaMercadoriaBLL();
        private readonly FornecedorBLL        _fornBLL = new FornecedorBLL();
        private readonly MercadoriaBLL        _mercBLL = new MercadoriaBLL();

        // In-memory list of items for the active new-entry form
        private readonly List<ItemEntradaMercadoria>        _itens   = new List<ItemEntradaMercadoria>();
        private readonly List<ParcelaEntradaMercadoria>     _parcelas= new List<ParcelaEntradaMercadoria>();

        // Cached product/supplier lookup tables for fast search
        private System.Data.DataTable _dtProdutos;
        private System.Data.DataTable _dtFornecedores;

        // Currently resolved supplier/product
        private int    _fornecedorCodigo  = 0;
        private int    _produtoAtualCodigo = 0;
        private int    _codigoEditando    = 0;  // 0 = nova entrada, >0 = editando

        public frmEntradaMercadoria()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            Load += OnLoad;
        }

        // ── Inicialização ────────────────────────────────────────────────────
        private void OnLoad(object sender, EventArgs e)
        {
            dtpDe.Value  = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpAte.Value = DateTime.Today;

            CarregarGrid();
            CarregarLookups();
            ConfigurarGridItens();
            ConfigurarGridParcelas();
            ConectarEventos();
        }

        private void CarregarLookups()
        {
            try { _dtProdutos    = _mercBLL.Listar(); } catch { }
            try { _dtFornecedores = _fornBLL.Listar(); } catch { }

            // AutoComplete for product name
            if (_dtProdutos != null)
            {
                var ac = new AutoCompleteStringCollection();
                foreach (System.Data.DataRow r in _dtProdutos.Rows)
                    ac.Add(r["Nome"]?.ToString() ?? "");
                txtProdNome.AutoCompleteMode   = AutoCompleteMode.SuggestAppend;
                txtProdNome.AutoCompleteSource = AutoCompleteSource.CustomSource;
                txtProdNome.AutoCompleteCustomSource = ac;
            }

            // AutoComplete for supplier name
            if (_dtFornecedores != null)
            {
                var acF = new AutoCompleteStringCollection();
                foreach (System.Data.DataRow r in _dtFornecedores.Rows)
                    acF.Add(r["RazaoSocial"]?.ToString() ?? "");
                txtFornNome.AutoCompleteMode   = AutoCompleteMode.SuggestAppend;
                txtFornNome.AutoCompleteSource = AutoCompleteSource.CustomSource;
                txtFornNome.AutoCompleteCustomSource = acF;
                txtFornNome.ReadOnly = false;
            }
        }

        private void ConfigurarGridItens()
        {
            gridItens.Columns.Clear();
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Codigo",     HeaderText = "C\u00f3d.",        Width = 50,  ReadOnly = true });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nome",       HeaderText = "Produto",      FillWeight = 100, ReadOnly = true });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Qtde",       HeaderText = "Qtde",         Width = 65,  ReadOnly = true });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "UnidEnt",    HeaderText = "Ent.",         Width = 45,  ReadOnly = true });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Fracao",     HeaderText = "Fra\u00e7\u00e3o",       Width = 55,  ReadOnly = true });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "UnidSai",    HeaderText = "Sa\u00edda",        Width = 45,  ReadOnly = true });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Custo",      HeaderText = "Custo R$",     Width = 90,  ReadOnly = true });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Subtotal",   HeaderText = "Subtotal",     Width = 100, ReadOnly = true });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "AtuCusto",   HeaderText = "At.Custo",     Width = 70,  ReadOnly = true });
            gridItens.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ConfigurarGridParcelas()
        {
            gridParcelas.Columns.Clear();
            gridParcelas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Num",        HeaderText = "#",           Width = 35, ReadOnly = true });
            gridParcelas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Vencimento", HeaderText = "Vencimento",  Width = 110, ReadOnly = false });
            gridParcelas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Valor",      HeaderText = "Valor R$",    FillWeight = 100, ReadOnly = false });
            gridParcelas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Obs",        HeaderText = "Observa\u00e7\u00e3o",  FillWeight = 100, ReadOnly = false });
            gridParcelas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ConectarEventos()
        {
            btnNovaEntrada.Click      += (_, __) => AbrirNovaEntrada();
            btnCancelarSel.Click      += BtnCancelarSel_Click;
            btnFiltrar.Click          += (_, __) => CarregarGrid();
            btnLimparFiltro.Click     += (_, __) => { dtpDe.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); dtpAte.Value = DateTime.Today; CarregarGrid(); };
            gridEntradas.CellDoubleClick += GridEntradas_CellDoubleClick;

            // Fornecedor: resolver por código
            txtFornCod.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Enter || ev.KeyCode == Keys.Tab)
                    ResolverFornecedorPorCodigo();
            };
            txtFornCod.Leave += (_, __) => ResolverFornecedorPorCodigo();

            // Fornecedor: resolver por nome
            txtFornNome.Leave += (_, __) => ResolverFornecedorPorNome();

            // Produto: resolver por código
            txtProdCod.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Enter || ev.KeyCode == Keys.Tab)
                    ResolverProdutoPorCodigo();
            };
            txtProdCod.Leave += (_, __) => ResolverProdutoPorCodigo();

            // Produto: resolver por nome (on Enter)
            txtProdNome.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Enter) ResolverProdutoPorNome();
            };
            txtProdNome.Leave += (_, __) => ResolverProdutoPorNome();

            btnAdicionarItem.Click    += BtnAdicionarItem_Click;
            chkFracionado.CheckedChanged += (_, __) => AtualizarVisibilidadeFracao();
            numQtde.ValueChanged      += (_, __) => { AtualizarLblUnidades(); RecalcularCusto(); };
            numFracao.ValueChanged    += (_, __) => { AtualizarLblUnidades(); RecalcularCusto(); };
            txtUnidSaida.TextChanged  += (_, __) => AtualizarLblUnidades();
            numValorTotal.ValueChanged += (_, __) => RecalcularCusto();
            btnBuscarForn.Click       += (_, __) => AbrirBuscaFornecedor();
            btnBuscarProd.Click       += (_, __) => AbrirBuscaProduto();
            btnRemoverItem.Click      += BtnRemoverItem_Click;
            btnConfirmarEntrada.Click += BtnConfirmarEntrada_Click;
            btnFecharForm.Click       += (_, __) => FecharNovaEntrada();
            btnGerarParcelas.Click    += BtnGerarParcelas_Click;

            // Ao editar valor de parcela diretamente no grid
            gridParcelas.CellEndEdit += (s, ev) =>
            {
                if (ev.RowIndex < 0 || ev.RowIndex >= _parcelas.Count) return;
                var cell = gridParcelas.Rows[ev.RowIndex].Cells[ev.ColumnIndex];
                if (gridParcelas.Columns[ev.ColumnIndex].Name == "Valor")
                {
                    if (decimal.TryParse(cell.Value?.ToString(), out decimal v))
                        _parcelas[ev.RowIndex].parValor = v;
                }
                else if (gridParcelas.Columns[ev.ColumnIndex].Name == "Obs")
                    _parcelas[ev.RowIndex].parObservacao = cell.Value?.ToString() ?? "";
                else if (gridParcelas.Columns[ev.ColumnIndex].Name == "Vencimento")
                {
                    var val = cell.Value?.ToString() ?? "";
                    if (DateTime.TryParseExact(val, new[]{ "dd/MM/yyyy", "d/M/yyyy", "dd/MM/yy" },
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out DateTime dt))
                    {
                        _parcelas[ev.RowIndex].parVencimento = dt;
                        gridParcelas.Rows[ev.RowIndex].Cells["Vencimento"].Value = dt.ToString("dd/MM/yyyy");
                    }
                }
            };
        }

        // ── Grid de entradas ─────────────────────────────────────────────────
        private void CarregarGrid()
        {
            try
            {
                var dt = _bll.Listar(dtpDe.Value.Date, dtpAte.Value.Date);
                gridEntradas.DataSource = dt;
                ConfigurarColunasEntradas();
            }
            catch (Exception ex) { MessageBox.Show("Erro ao carregar: " + ex.Message); }
        }

        private void ConfigurarColunasEntradas()
        {
            if (gridEntradas.Columns.Count == 0) return;
            foreach (DataGridViewColumn col in gridEntradas.Columns) col.Visible = false;
            var visivel = new[] { "entData", "Fornecedor", "Documento", "Itens", "Total", "Lancamento" };
            var cabecalhos = new System.Collections.Generic.Dictionary<string, string>
            {
                ["entData"]   = "Data",
                ["Fornecedor"]= "Fornecedor",
                ["Documento"] = "N\u00ba Doc",
                ["Itens"]     = "Itens",
                ["Total"]     = "Total R$",
                ["Lancamento"]= "Lan\u00e7amento",
            };
            foreach (var name in visivel)
                if (gridEntradas.Columns.Contains(name))
                {
                    gridEntradas.Columns[name].Visible = true;
                    if (cabecalhos.ContainsKey(name))
                        gridEntradas.Columns[name].HeaderText = cabecalhos[name];
                }
        }

        private void GridEntradas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = gridEntradas.Rows[e.RowIndex];
            if (row.DataBoundItem == null) return;
            var dtRow    = ((System.Data.DataRowView)row.DataBoundItem).Row;
            var cod      = Convert.ToInt32(dtRow["Codigo"]);
            var itens    = _bll.ListarItens(cod);
            var parcelas = _bll.ListarParcelas(cod);

            using var dlg = new frmDetalheEntrada(cod, dtRow, itens, parcelas, _bll);
            dlg.ShowDialog(this);
            if (dlg.QuereEditar)
                AbrirEdicaoEntrada(cod, dtRow, itens, parcelas);
        }

        private void AbrirEdicaoEntrada(int cod, System.Data.DataRow dtRow,
            List<ItemEntradaMercadoria> itens, List<ParcelaEntradaMercadoria> parcelas)
        {
            _codigoEditando    = cod;
            _fornecedorCodigo  = dtRow["Fornecedor"] != DBNull.Value ? 0 : 0; // será resolvido abaixo
            _produtoAtualCodigo = 0;

            // Cabeçalho
            txtFornNome.Text  = dtRow["Fornecedor"]?.ToString() ?? "";
            txtFornCod.Text   = "";
            ResolverFornecedorPorNome();
            if (DateTime.TryParse(dtRow["entData"]?.ToString(), out DateTime dt))
                dtpData.Value = dt;
            txtNumDoc.Text = dtRow["Documento"]?.ToString() ?? "";

            // Itens
            _itens.Clear();
            foreach (var it in itens)
                _itens.Add(new ItemEntradaMercadoria
                {
                    Codigo_Mercadoria  = it.Codigo_Mercadoria,
                    itmNome_Mercadoria = it.itmNome_Mercadoria,
                    itmQtde            = it.itmQtde,
                    itmFracao          = it.itmFracao,
                    itmUnid_Entrada    = it.itmUnid_Entrada,
                    itmUnid_Saida      = it.itmUnid_Saida,
                    itmPreco_Custo     = it.itmPreco_Custo,
                    itmSubtotal        = it.itmSubtotal,
                    itmAtualizar_Custo = it.itmAtualizar_Custo,
                    Situacao           = "A",
                });

            // Parcelas
            _parcelas.Clear();
            foreach (var p in parcelas)
                _parcelas.Add(new ParcelaEntradaMercadoria
                {
                    parNumero    = p.parNumero,
                    parVencimento= p.parVencimento,
                    parValor     = p.parValor,
                    parObservacao= p.parObservacao,
                });

            // Limpar campos de produto
            txtProdCod.Clear(); txtProdNome.Clear();
            numQtde.Value = 1; numValorTotal.Value = 0; numCustoItem.Value = 0;
            chkFracionado.Checked = false; chkAtualizarCusto.Checked = false;
            AtualizarVisibilidadeFracao();

            AtualizarGridItens();
            AtualizarGridParcelas();
            pnlNovaEntrada.Visible = true;
            btnConfirmarEntrada.Text = "\u2714 Salvar Edição";
            txtFornCod.Focus();
        }

        // ── Nova Entrada ─────────────────────────────────────────────────────
        private void AtualizarLblUnidades()
        {
            if (!chkFracionado.Checked) return;
            decimal total = numQtde.Value * numFracao.Value;
            string un = string.IsNullOrWhiteSpace(txtUnidSaida.Text) ? "UN" : txtUnidSaida.Text.Trim().ToUpper();
            lblUnidadesEntrada.Text = $"= {total:N2} {un}";
        }

        private void RecalcularCusto()
        {
            if (numValorTotal.Value <= 0) { numCustoItem.Value = 0; return; }
            decimal totalUnid = chkFracionado.Checked
                ? numQtde.Value * numFracao.Value
                : numQtde.Value;
            if (totalUnid <= 0) { numCustoItem.Value = 0; return; }
            var custo = numValorTotal.Value / totalUnid;
            numCustoItem.Value = custo > numCustoItem.Maximum ? numCustoItem.Maximum : custo;
        }

        private void AtualizarVisibilidadeFracao()
        {
            bool frac = chkFracionado.Checked;
            lblFracEntrada.Visible     = frac;
            numFracEntrada.Visible     = frac;
            txtUnidEntrada.Visible     = frac;
            lblIgual.Visible           = frac;
            numFracao.Visible          = frac;
            txtUnidSaida.Visible       = frac;
            lblUnidadesEntrada.Visible = frac;
            pnlAddItem.Height          = frac ? 80 : 44;
            pnlNovaEntrada.PerformLayout();
            if (frac) AtualizarLblUnidades();
            RecalcularCusto();
        }

        private void AbrirNovaEntrada()
        {
            _itens.Clear();
            _parcelas.Clear();
            _codigoEditando     = 0;
            _fornecedorCodigo   = 0;
            _produtoAtualCodigo = 0;
            btnConfirmarEntrada.Text = "\u2714 Confirmar";

            txtFornCod.Clear();
            txtFornNome.Clear();
            dtpData.Value = DateTime.Today;
            txtNumDoc.Clear();
            txtProdCod.Clear();
            txtProdNome.Clear();
            numQtde.Value = 1;
            numValorTotal.Value = 0;
            numCustoItem.Value = 0;
            chkAtualizarCusto.Checked = false;
            chkFracionado.Checked = false;
            numFracEntrada.Value = 1;
            AtualizarVisibilidadeFracao();

            AtualizarGridItens();
            pnlNovaEntrada.Visible = true;
            txtFornCod.Focus();
            AtualizarGridParcelas();
        }

        private void FecharNovaEntrada()
        {
            pnlNovaEntrada.Visible = false;
            _itens.Clear();
            _parcelas.Clear();
            _codigoEditando = 0;
            btnConfirmarEntrada.Text = "\u2714 Confirmar";
        }

        // ── Resolução de fornecedor ──────────────────────────────────────────
        private void ResolverFornecedorPorCodigo()
        {
            if (!int.TryParse(txtFornCod.Text.Trim(), out int cod) || cod <= 0) return;
            var forn = _fornBLL.PesquisaCodigo(cod);
            if (forn == null) { txtFornNome.Text = "(não encontrado)"; _fornecedorCodigo = 0; return; }
            _fornecedorCodigo = forn.Codigo;
            txtFornNome.Text  = forn.fornNome_RazaoSocial;
        }

        private void ResolverFornecedorPorNome()
        {
            var nome = txtFornNome.Text.Trim();
            if (string.IsNullOrEmpty(nome)) { _fornecedorCodigo = 0; return; }
            if (_dtFornecedores == null) return;
            foreach (System.Data.DataRow r in _dtFornecedores.Rows)
            {
                if (r["RazaoSocial"]?.ToString()?.Equals(nome, StringComparison.OrdinalIgnoreCase) == true)
                {
                    _fornecedorCodigo = Convert.ToInt32(r["Codigo"]);
                    txtFornCod.Text   = _fornecedorCodigo.ToString();
                    return;
                }
            }
            // Not matched from autocomplete — treat as free-text supplier name (no FK)
        }

        // ── Resolução de produto ─────────────────────────────────────────────
        private void ResolverProdutoPorCodigo()
        {
            if (!int.TryParse(txtProdCod.Text.Trim(), out int cod) || cod <= 0) return;
            var merc = _mercBLL.PesquisaCodigo(cod);
            if (merc == null) { txtProdNome.Text = "(não encontrado)"; _produtoAtualCodigo = 0; return; }
            _produtoAtualCodigo       = merc.Codigo;
            txtProdNome.Text          = merc.mercMercadoria;
            numCustoItem.Value        = merc.mercPreco_Custo;
        }

        private void ResolverProdutoPorNome()
        {
            var nome = txtProdNome.Text.Trim();
            if (string.IsNullOrEmpty(nome) || _dtProdutos == null) return;
            foreach (System.Data.DataRow r in _dtProdutos.Rows)
            {
                if (r["Nome"]?.ToString()?.Equals(nome, StringComparison.OrdinalIgnoreCase) == true)
                {
                    _produtoAtualCodigo  = Convert.ToInt32(r["Codigo"]);
                    txtProdCod.Text      = _produtoAtualCodigo.ToString();
                    var merc = _mercBLL.PesquisaCodigo(_produtoAtualCodigo);
                    if (merc != null && numCustoItem.Value == 0)
                        numCustoItem.Value = merc.mercPreco_Custo;
                    return;
                }
            }
        }

        // ── Adicionar / Remover item ─────────────────────────────────────────
        private void BtnAdicionarItem_Click(object sender, EventArgs e)
        {
            var nome = txtProdNome.Text.Trim();
            if (string.IsNullOrWhiteSpace(nome)) { MessageBox.Show("Informe o produto."); txtProdNome.Focus(); return; }
            if (numQtde.Value <= 0) { MessageBox.Show("Informe uma quantidade válida."); numQtde.Focus(); return; }

            var qtde     = numQtde.Value;
            var fracao   = (chkFracionado.Checked && numFracao.Value > 0) ? numFracao.Value : 1m;
            var unidEnt  = chkFracionado.Checked ? txtUnidEntrada.Text.Trim() : "";
            var unidSai  = chkFracionado.Checked ? txtUnidSaida.Text.Trim() : "";
            var custo    = numCustoItem.Value;
            decimal totalUnid = chkFracionado.Checked ? qtde * fracao : qtde;
            var subtotal = totalUnid * custo;

            var item = new ItemEntradaMercadoria
            {
                Codigo_Mercadoria  = _produtoAtualCodigo,
                itmNome_Mercadoria = nome,
                itmQtde            = qtde,
                itmFracao          = fracao,
                itmUnid_Entrada    = unidEnt,
                itmUnid_Saida      = unidSai,
                itmPreco_Custo     = custo,
                itmSubtotal        = subtotal,
                itmAtualizar_Custo = chkAtualizarCusto.Checked,
                Situacao           = "A",
            };
            _itens.Add(item);
            AtualizarGridItens();

            // Limpar campos de produto para próximo item
            txtProdCod.Clear();
            txtProdNome.Clear();
            numQtde.Value = 1;
            numFracEntrada.Value = 1;
            numFracao.Value = 1;
            txtUnidEntrada.Clear();
            txtUnidSaida.Clear();
            numValorTotal.Value = 0;
            numCustoItem.Value = 0;
            chkFracionado.Checked = false;
            AtualizarVisibilidadeFracao();
            _produtoAtualCodigo = 0;
            txtProdCod.Focus();
        }

        private void BtnRemoverItem_Click(object sender, EventArgs e)
        {
            if (gridItens.SelectedRows.Count == 0) return;
            var idx = gridItens.SelectedRows[0].Index;
            if (idx >= 0 && idx < _itens.Count)
            {
                _itens.RemoveAt(idx);
                AtualizarGridItens();
            }
        }

        private void AtualizarGridItens()
        {
            gridItens.Rows.Clear();
            decimal total = 0;
            foreach (var item in _itens)
            {
                gridItens.Rows.Add(
                    item.Codigo_Mercadoria > 0 ? item.Codigo_Mercadoria.ToString() : "-",
                    item.itmNome_Mercadoria,
                    item.itmQtde.ToString("N2"),
                    string.IsNullOrEmpty(item.itmUnid_Entrada) ? "UN" : item.itmUnid_Entrada.ToUpper(),
                    item.itmFracao == 1m ? "1" : item.itmFracao.ToString("N4").TrimEnd('0').TrimEnd('.'),
                    string.IsNullOrEmpty(item.itmUnid_Saida) ? "UN" : item.itmUnid_Saida.ToUpper(),
                    item.itmPreco_Custo.ToString("N2"),
                    item.itmSubtotal.ToString("N2"),
                    item.itmAtualizar_Custo ? "Sim" : "Não"
                );
                total += item.itmSubtotal;
            }
            lblTotal.Text = $"Total: R$ {total:N2}";
            // Re-gerar parcelas automaticamente se já tiver sido configurado
            if (_parcelas.Count > 0) RegenerarValoresParcelas(total);
        }

        private void BtnGerarParcelas_Click(object sender, EventArgs e)
        {
            int n = (int)numParcelas.Value;
            if (n <= 0) { _parcelas.Clear(); AtualizarGridParcelas(); return; }

            decimal total = 0;
            foreach (var it in _itens) total += it.itmSubtotal;
            if (total <= 0) { MessageBox.Show("Adicione itens antes de gerar parcelas."); return; }

            decimal valorParcela = Math.Round(total / n, 2);
            decimal ajuste = total - valorParcela * n;

            _parcelas.Clear();
            DateTime vcto = dtpPrimVencimento.Value.Date;
            for (int i = 1; i <= n; i++)
            {
                _parcelas.Add(new ParcelaEntradaMercadoria
                {
                    parNumero    = i,
                    parVencimento = vcto,
                    parValor     = i == n ? valorParcela + ajuste : valorParcela,
                    parObservacao = ""
                });
                vcto = vcto.AddMonths(1);
            }
            AtualizarGridParcelas();
        }

        private void RegenerarValoresParcelas(decimal novoTotal)
        {
            if (_parcelas.Count == 0) return;
            decimal vp = Math.Round(novoTotal / _parcelas.Count, 2);
            decimal ajuste = novoTotal - vp * _parcelas.Count;
            for (int i = 0; i < _parcelas.Count; i++)
                _parcelas[i].parValor = i == _parcelas.Count - 1 ? vp + ajuste : vp;
            AtualizarGridParcelas();
        }

        private void AtualizarGridParcelas()
        {
            gridParcelas.Rows.Clear();
            foreach (var p in _parcelas)
                gridParcelas.Rows.Add(p.parNumero.ToString(), p.parVencimento.ToString("dd/MM/yyyy"),
                    p.parValor.ToString("N2"), p.parObservacao);
        }

        // ── Confirmar entrada ────────────────────────────────────────────────
        private void BtnConfirmarEntrada_Click(object sender, EventArgs e)
        {
            var entrada = new EntradaMercadoria
            {
                Codigo_Fornecedor  = _fornecedorCodigo,
                entNome_Fornecedor = txtFornNome.Text.Trim(),
                entData            = dtpData.Value.Date,
                entNumeroDoc       = txtNumDoc.Text.Trim(),
                entObservacoes     = "",
                entValorTotal      = 0,
            };
            // Calculate total
            decimal total = 0;
            foreach (var item in _itens) total += item.itmSubtotal;
            entrada.entValorTotal = total;

            string erro;
            string msg;
            if (_codigoEditando > 0)
            {
                entrada.Codigo = _codigoEditando;
                erro = _bll.Atualizar(entrada, new List<ItemEntradaMercadoria>(_itens),
                    _parcelas.Count > 0 ? new List<ParcelaEntradaMercadoria>(_parcelas) : null);
                msg  = "Entrada atualizada com sucesso! Estoque recalculado.";
            }
            else
            {
                erro = _bll.Inserir(entrada, new List<ItemEntradaMercadoria>(_itens),
                    _parcelas.Count > 0 ? new List<ParcelaEntradaMercadoria>(_parcelas) : null);
                msg  = "Entrada registrada com sucesso! Estoque atualizado.";
            }
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }

            MessageBox.Show(msg, "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

            FecharNovaEntrada();
            CarregarGrid();
            // Refresh product lookup (stock changed)
            try { _dtProdutos = _mercBLL.Listar(); } catch { }
        }

        // ── Cancelar entrada selecionada ────────────────────────────────────
        private void BtnCancelarSel_Click(object sender, EventArgs e)
        {
            if (gridEntradas.SelectedRows.Count == 0) { MessageBox.Show("Selecione uma entrada para cancelar."); return; }
            var dtRow = ((System.Data.DataRowView)gridEntradas.SelectedRows[0].DataBoundItem).Row;
            var cod   = Convert.ToInt32(dtRow["Codigo"]);

            if (MessageBox.Show("Cancelar essa entrada irá reverter o estoque adicionado.\nDeseja continuar?",
                    "Cancelar Entrada", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            var erro = _bll.Cancelar(cod);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            MessageBox.Show("Entrada cancelada e estoque revertido.", "Sucesso");
            CarregarGrid();
        }

        // ── Busca de fornecedor ──────────────────────────────────────────────
        private void AbrirBuscaFornecedor()
        {
            if (_dtFornecedores == null) return;
            using var dlg = new Form();
            dlg.Text = "Selecionar Fornecedor";
            dlg.StartPosition = FormStartPosition.CenterParent;
            dlg.Size = new Size(520, 400);
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox = dlg.MinimizeBox = false;
            dlg.BackColor = Color.FromArgb(245, 237, 216);

            var txtF = new TextBox { Dock = DockStyle.Top, Height = 28, BackColor = Color.White,
                ForeColor = Color.FromArgb(50,50,50), Font = new Font("Segoe UI", 9.5F) };
            var gridF = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true,
                AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White, Font = new Font("Segoe UI", 9F) };
            gridF.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224,113,42);
            gridF.DefaultCellStyle.SelectionForeColor = Color.White;

            var dtF = _dtFornecedores.Copy();
            gridF.DataSource = dtF;
            if (gridF.Columns.Contains("Situacao")) gridF.Columns["Situacao"].Visible = false;

            txtF.TextChanged += (_, __) =>
            {
                var f = txtF.Text.Trim().Replace("'", "''");
                dtF.DefaultView.RowFilter = string.IsNullOrEmpty(f) ? "" : $"RazaoSocial LIKE '%{f}%'";
            };
            gridF.DoubleClick += (_, __) =>
            {
                if (gridF.SelectedRows.Count == 0) return;
                var row = gridF.SelectedRows[0];
                _fornecedorCodigo = Convert.ToInt32(row.Cells["Codigo"].Value);
                txtFornCod.Text  = _fornecedorCodigo.ToString();
                txtFornNome.Text = row.Cells["RazaoSocial"].Value?.ToString() ?? "";
                dlg.DialogResult = DialogResult.OK;
            };
            dlg.Controls.Add(gridF);
            dlg.Controls.Add(txtF);
            dlg.ShowDialog(this);
        }

        // ── Busca de produto ─────────────────────────────────────────────────
        private void AbrirBuscaProduto()
        {
            if (_dtProdutos == null) return;
            using var dlg = new Form();
            dlg.Text = "Selecionar Produto";
            dlg.StartPosition = FormStartPosition.CenterParent;
            dlg.Size = new Size(520, 400);
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox = dlg.MinimizeBox = false;
            dlg.BackColor = Color.FromArgb(245, 237, 216);

            var txtP = new TextBox { Dock = DockStyle.Top, Height = 28, BackColor = Color.White,
                ForeColor = Color.FromArgb(50,50,50), Font = new Font("Segoe UI", 9.5F) };
            var gridP = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true,
                AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White, Font = new Font("Segoe UI", 9F) };
            gridP.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224,113,42);
            gridP.DefaultCellStyle.SelectionForeColor = Color.White;

            var dtP = _dtProdutos.Copy();
            gridP.DataSource = dtP;
            if (gridP.Columns.Contains("Situacao"))    gridP.Columns["Situacao"].Visible = false;
            if (gridP.Columns.Contains("EhProduto"))   gridP.Columns["EhProduto"].Visible = false;

            txtP.TextChanged += (_, __) =>
            {
                var f = txtP.Text.Trim().Replace("'", "''");
                dtP.DefaultView.RowFilter = string.IsNullOrEmpty(f) ? "" : $"Nome LIKE '%{f}%'";
            };
            gridP.DoubleClick += (_, __) =>
            {
                if (gridP.SelectedRows.Count == 0) return;
                var row = gridP.SelectedRows[0];
                _produtoAtualCodigo = Convert.ToInt32(row.Cells["Codigo"].Value);
                txtProdCod.Text   = _produtoAtualCodigo.ToString();
                txtProdNome.Text  = row.Cells["Nome"].Value?.ToString() ?? "";
                var merc = _mercBLL.PesquisaCodigo(_produtoAtualCodigo);
                if (merc != null && numCustoItem.Value == 0)
                    numCustoItem.Value = merc.mercPreco_Custo;
                dlg.DialogResult = DialogResult.OK;
            };
            dlg.Controls.Add(gridP);
            dlg.Controls.Add(txtP);
            dlg.ShowDialog(this);
        }

        private void chkFracionado_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }

    // ── Detalhe leitura de entrada já registrada ────────────────────────────
    public partial class frmDetalheEntrada : Form
    {
        private readonly EntradaMercadoriaBLL _bll;
        private readonly int _codigoEntrada;
        private DataGridView _gridParcelas;
        public bool QuereEditar { get; private set; } = false;

        public frmDetalheEntrada(int codigo, System.Data.DataRow row, List<ItemEntradaMercadoria> itens,
            List<ParcelaEntradaMercadoria> parcelas, EntradaMercadoriaBLL bll)
        {
            _bll            = bll;
            _codigoEntrada  = codigo;

            var corFundo  = Color.FromArgb(245, 237, 216);
            var corHeader = Color.FromArgb(176, 110, 42);

            Text          = $"Entrada #{codigo} \u2014 Detalhes";
            BackColor     = corFundo;
            ForeColor     = Color.FromArgb(50, 40, 20);
            Font          = new Font("Segoe UI", 9F);
            ClientSize    = new Size(860, 540);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize   = new Size(720, 440);

            // Top bar
            var pnlTop  = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = corHeader };
            var lblInfo = new Label
            {
                Text = $"  Fornecedor: {row["Fornecedor"]}   |   Data: {Convert.ToDateTime(row["entData"]):dd/MM/yyyy}   |   Documento: {row["Documento"]}",
                Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.White
            };
            pnlTop.Controls.Add(lblInfo);

            // Items section label
            var lblItens = new Label { Text = "  Itens da Entrada", Dock = DockStyle.Top, Height = 24,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(120, 80, 30),
                TextAlign = ContentAlignment.MiddleLeft, BackColor = Color.FromArgb(235, 226, 208) };

            var gridItens = new DataGridView
            {
                Dock = DockStyle.Top, Height = 155,
                ReadOnly = true, AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false, BackgroundColor = Color.White,
                DefaultCellStyle    = { BackColor = Color.White, ForeColor = Color.FromArgb(50,40,20) },
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(250,247,242) },
                ColumnHeadersDefaultCellStyle   = { BackColor = corHeader, ForeColor = Color.White, Font = new Font("Segoe UI",9F,FontStyle.Bold) },
                GridColor = Color.FromArgb(200,185,160), BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 30, RowTemplate = { Height = 24 },
            };
            gridItens.DataError += (_, ev) => ev.ThrowException = false;

            decimal totalItens = 0;
            var dtItens = new System.Data.DataTable();
            dtItens.Columns.AddRange(new[] {
                new System.Data.DataColumn("C\u00f3d."),
                new System.Data.DataColumn("Produto"),
                new System.Data.DataColumn("Qtde"),
                new System.Data.DataColumn("Ent."),
                new System.Data.DataColumn("Fra\u00e7\u00e3o"),
                new System.Data.DataColumn("Sa\u00edda"),
                new System.Data.DataColumn("Custo R$"),
                new System.Data.DataColumn("Subtotal"),
                new System.Data.DataColumn("At.Custo"),
            });
            foreach (var it in itens)
            {
                dtItens.Rows.Add(
                    it.Codigo_Mercadoria > 0 ? it.Codigo_Mercadoria.ToString() : "-",
                    it.itmNome_Mercadoria,
                    it.itmQtde.ToString("N2"),
                    string.IsNullOrEmpty(it.itmUnid_Entrada) ? "UN" : it.itmUnid_Entrada.ToUpper(),
                    it.itmFracao == 1m ? "1" : it.itmFracao.ToString("N4").TrimEnd('0').TrimEnd('.'),
                    string.IsNullOrEmpty(it.itmUnid_Saida)   ? "UN" : it.itmUnid_Saida.ToUpper(),
                    it.itmPreco_Custo.ToString("N2"),
                    it.itmSubtotal.ToString("N2"),
                    it.itmAtualizar_Custo ? "Sim" : "N\u00e3o");
                totalItens += it.itmSubtotal;
            }
            gridItens.DataSource = dtItens;

            var pnlTotalItens = new Panel { Dock = DockStyle.Top, Height = 24, BackColor = Color.FromArgb(235,226,208) };
            var lblTot = new Label { Text = $"  Total: R$ {totalItens:N2}",
                Font = new Font("Segoe UI",9.5F,FontStyle.Bold), ForeColor = Color.FromArgb(87,120,38),
                Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            pnlTotalItens.Controls.Add(lblTot);

            // Parcelas section label
            var lblParc = new Label { Text = "  Parcelas", Dock = DockStyle.Top, Height = 24,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(120, 80, 30),
                TextAlign = ContentAlignment.MiddleLeft, BackColor = Color.FromArgb(235, 226, 208) };

            _gridParcelas = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true, AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false, BackgroundColor = Color.White,
                DefaultCellStyle    = { BackColor = Color.White, ForeColor = Color.FromArgb(50,40,20) },
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(250,247,242) },
                ColumnHeadersDefaultCellStyle   = { BackColor = corHeader, ForeColor = Color.White, Font = new Font("Segoe UI",9F,FontStyle.Bold) },
                GridColor = Color.FromArgb(200,185,160), BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 30, RowTemplate = { Height = 24 },
            };
            _gridParcelas.DataError += (_, ev) => ev.ThrowException = false;
            CarregarParcelasGrid(parcelas);

            // Footer
            var pnlFoot = new Panel { Dock = DockStyle.Bottom, Height = 44,
                BackColor = Color.FromArgb(235, 226, 208) };
            var btnMarcarPago = new Button
            {
                Text = "\u2714 Marcar Parcela como Paga", Left = 12, Top = 8, Width = 200, Height = 28,
                BackColor = Color.FromArgb(87, 120, 38), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI",9F,FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnMarcarPago.FlatAppearance.BorderSize = 0;
            btnMarcarPago.Click += BtnMarcarPago_Click;
            var btnFechar = new Button
            {
                Text = "Fechar", Left = 224, Top = 8, Width = 90, Height = 28,
                BackColor = Color.FromArgb(224, 113, 42), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btnFechar.FlatAppearance.BorderSize = 0;
            btnFechar.Click += (_, __) => Close();
            var btnEditar = new Button
            {
                Text = "\u270E Editar Entrada", Left = 328, Top = 8, Width = 130, Height = 28,
                BackColor = Color.FromArgb(52, 100, 160), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnEditar.FlatAppearance.BorderSize = 0;
            btnEditar.Click += (_, __) => { QuereEditar = true; Close(); };
            pnlFoot.Controls.AddRange(new Control[] { btnMarcarPago, btnFechar, btnEditar });

            // Layout: add in reverse Dock order (Fill last)
            Controls.Add(_gridParcelas);
            Controls.Add(lblParc);
            Controls.Add(pnlTotalItens);
            Controls.Add(gridItens);
            Controls.Add(lblItens);
            Controls.Add(pnlFoot);
            Controls.Add(pnlTop);
        }

        private void CarregarParcelasGrid(List<ParcelaEntradaMercadoria> parcelas)
        {
            var dt = new System.Data.DataTable();
            dt.Columns.AddRange(new[]
            {
                new System.Data.DataColumn("Codigo"),
                new System.Data.DataColumn("#"),
                new System.Data.DataColumn("Vencimento"),
                new System.Data.DataColumn("Valor R$"),
                new System.Data.DataColumn("Situa\u00e7\u00e3o"),
                new System.Data.DataColumn("Dt. Pagamento"),
                new System.Data.DataColumn("Observa\u00e7\u00e3o"),
            });
            foreach (var p in parcelas)
            {
                dt.Rows.Add(
                    p.Codigo.ToString(),
                    p.parNumero.ToString(),
                    p.parVencimento.ToString("dd/MM/yyyy"),
                    p.parValor.ToString("N2"),
                    p.Situacao == "P" ? "Paga" : "Em aberto",
                    p.parData_Pagamento.HasValue ? p.parData_Pagamento.Value.ToString("dd/MM/yyyy") : "-",
                    p.parObservacao ?? ""
                );
            }
            _gridParcelas.DataSource = dt;
            if (_gridParcelas.Columns.Contains("Codigo"))
                _gridParcelas.Columns["Codigo"].Visible = false;
            ColorirLinhasParcelas();
        }

        private void ColorirLinhasParcelas()
        {
            foreach (DataGridViewRow row in _gridParcelas.Rows)
            {
                if (row.DataBoundItem == null) continue;
                var dr = ((System.Data.DataRowView)row.DataBoundItem).Row;
                if (dr["Situa\u00e7\u00e3o"]?.ToString() == "Paga")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(220, 240, 210);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(45, 105, 45);
                }
            }
        }

        private void BtnMarcarPago_Click(object sender, EventArgs e)
        {
            if (_gridParcelas.SelectedRows.Count == 0)
            { MessageBox.Show("Selecione uma parcela para marcar como paga."); return; }
            var dr = ((System.Data.DataRowView)_gridParcelas.SelectedRows[0].DataBoundItem).Row;
            if (dr["Situa\u00e7\u00e3o"]?.ToString() == "Paga")
            { MessageBox.Show("Esta parcela j\u00e1 est\u00e1 paga."); return; }
            int cod = Convert.ToInt32(dr["Codigo"]);
            if (MessageBox.Show("Confirmar pagamento desta parcela?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            var erro = _bll.MarcarParcelaPaga(cod);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            var parcelas = _bll.ListarParcelas(_codigoEntrada);
            CarregarParcelasGrid(parcelas);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
