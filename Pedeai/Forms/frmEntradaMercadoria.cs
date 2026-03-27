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

        public frmEntradaMercadoria()
        {
            InitializeComponent();
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
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Codigo",     HeaderText = "Cód.",      Width = 55,  ReadOnly = true });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nome",        HeaderText = "Produto",   FillWeight = 100, ReadOnly = true });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Qtde",        HeaderText = "Qtde",      Width = 80,  ReadOnly = true });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Custo",       HeaderText = "Custo R$",  Width = 100, ReadOnly = true });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Subtotal",    HeaderText = "Subtotal",  Width = 110, ReadOnly = true });
            gridItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "AtuCusto",    HeaderText = "At.Custo",  Width = 75,  ReadOnly = true });
            gridItens.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ConfigurarGridParcelas()
        {
            gridParcelas.Columns.Clear();
            gridParcelas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Num",        HeaderText = "#",           Width = 35, ReadOnly = true });
            gridParcelas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Vencimento", HeaderText = "Vencimento",  Width = 110, ReadOnly = true });
            gridParcelas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Valor",      HeaderText = "Valor R$",    FillWeight = 100, ReadOnly = false });
            gridParcelas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Obs",        HeaderText = "Observação",  FillWeight = 100, ReadOnly = false });
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
            var visivel = new[] { "Codigo", "entData", "Fornecedor", "Documento", "Itens", "Total", "Lancamento" };
            var cabecalhos = new System.Collections.Generic.Dictionary<string, string>
            {
                ["Codigo"]    = "Cód.",
                ["entData"]   = "Data",
                ["Fornecedor"]= "Fornecedor",
                ["Documento"] = "Nº Doc",
                ["Itens"]     = "Itens",
                ["Total"]     = "Total R$",
                ["Lancamento"]= "Lançamento",
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
            var dtRow = ((System.Data.DataRowView)row.DataBoundItem).Row;
            var cod  = Convert.ToInt32(dtRow["Codigo"]);
            var itens = _bll.ListarItens(cod);

            using var dlg = new frmDetalheEntrada(cod, dtRow, itens);
            dlg.ShowDialog(this);
        }

        // ── Nova Entrada ─────────────────────────────────────────────────────
        private void AbrirNovaEntrada()
        {
            _itens.Clear();
            _fornecedorCodigo   = 0;
            _produtoAtualCodigo = 0;

            txtFornCod.Clear();
            txtFornNome.Clear();
            dtpData.Value = DateTime.Today;
            txtNumDoc.Clear();
            txtObservacoes.Clear();
            txtProdCod.Clear();
            txtProdNome.Clear();
            numQtde.Value = 1;
            numCustoItem.Value = 0;
            chkAtualizarCusto.Checked = true;

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

            var qtde    = numQtde.Value;
            var custo   = numCustoItem.Value;
            var subtotal = qtde * custo;

            var item = new ItemEntradaMercadoria
            {
                Codigo_Mercadoria  = _produtoAtualCodigo,
                itmNome_Mercadoria = nome,
                itmQtde            = qtde,
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
            numCustoItem.Value = 0;
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
                    item.itmQtde.ToString("N3"),
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
                entObservacoes     = txtObservacoes.Text.Trim(),
                entValorTotal      = 0,
            };
            // Calculate total
            decimal total = 0;
            foreach (var item in _itens) total += item.itmSubtotal;
            entrada.entValorTotal = total;

            var erro = _bll.Inserir(entrada, new List<ItemEntradaMercadoria>(_itens),
                _parcelas.Count > 0 ? new List<ParcelaEntradaMercadoria>(_parcelas) : null);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }

            MessageBox.Show("Entrada registrada com sucesso! Estoque atualizado.", "Sucesso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

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
    }

    // ── Detalhe leitura de entrada já registrada ────────────────────────────
    public partial class frmDetalheEntrada : Form
    {
        public frmDetalheEntrada(int codigo, System.Data.DataRow row, List<ItemEntradaMercadoria> itens)
        {
            var corFundo = Color.FromArgb(15, 22, 45);
            var corCard  = Color.FromArgb(28, 37, 65);
            var corTopBar= Color.FromArgb(36, 48, 82);
            var corGrid  = Color.FromArgb(20, 28, 55);

            Text = $"Entrada #{codigo} — Detalhes";
            BackColor    = corFundo;
            ForeColor    = Color.White;
            Font         = new Font("Segoe UI", 9F);
            ClientSize   = new Size(700, 460);
            StartPosition= FormStartPosition.CenterParent;

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = corTopBar };
            var info   = new Label
            {
                Text = $"  Fornecedor: {row["Fornecedor"]}   |   Data: {Convert.ToDateTime(row["entData"]):dd/MM/yyyy}   |   Documento: {row["Documento"]}",
                Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.White
            };
            pnlTop.Controls.Add(info);

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true, AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false, BackgroundColor = corGrid,
                DefaultCellStyle = { BackColor = corGrid, ForeColor = Color.White },
                GridColor = Color.FromArgb(40, 55, 90), BorderStyle = BorderStyle.None,
                ColumnHeadersDefaultCellStyle = { BackColor = corTopBar, ForeColor = Color.White },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            };
            grid.DataError += (_, e) => e.ThrowException = false;

            var pnlFoot = new Panel { Dock = DockStyle.Bottom, Height = 42, BackColor = corCard };
            decimal total = 0; foreach (var it in itens) total += it.itmSubtotal;
            var lblTot = new Label
            {
                Text = $"  Total: R$ {total:N2}", Dock = DockStyle.Left, Width = 250,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(39, 174, 96),
                TextAlign = ContentAlignment.MiddleLeft
            };
            var btnFech = new Button
            {
                Text = "Fechar", Dock = DockStyle.Right, Width = 100,
                BackColor = Color.FromArgb(80, 95, 130), ForeColor = Color.White, FlatStyle = FlatStyle.Flat
            };
            btnFech.FlatAppearance.BorderSize = 0;
            btnFech.Click += (_, __) => Close();
            pnlFoot.Controls.AddRange(new Control[] { lblTot, btnFech });

            Controls.Add(grid);
            Controls.Add(pnlFoot);
            Controls.Add(pnlTop);

            // Bind
            var dt = new System.Data.DataTable();
            dt.Columns.AddRange(new[] {
                new System.Data.DataColumn("Codigo"),
                new System.Data.DataColumn("Produto"),
                new System.Data.DataColumn("Qtde"),
                new System.Data.DataColumn("Custo R$"),
                new System.Data.DataColumn("Subtotal"),
                new System.Data.DataColumn("Atualiza Custo"),
            });
            foreach (var it in itens)
                dt.Rows.Add(it.Codigo_Mercadoria > 0 ? it.Codigo_Mercadoria.ToString() : "-",
                    it.itmNome_Mercadoria, it.itmQtde.ToString("N3"),
                    it.itmPreco_Custo.ToString("N4"), it.itmSubtotal.ToString("N2"),
                    it.itmAtualizar_Custo ? "Sim" : "Não");
            grid.DataSource = dt;
        }
    }
}
