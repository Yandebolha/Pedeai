using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmFidelizacao : Form
    {
        private readonly FidelizacaoBLL _bll;
        private int _codigoConfigEditando = 0;
        private int _produtoCodigo         = 0;   // produto selecionado do cardápio
        private bool _carregandoConfigs = false;
        private DataGridView _gridClientes;

        public frmFidelizacao()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _bll = new FidelizacaoBLL();
            Load += Form_Load;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            CarregarListaConfigs();
            CarregarHistorico();
            CriarAbaClientes();
        }

        // ── Aba Clientes ──────────────────────────────────────────────────────

        private void CriarAbaClientes()
        {
            var tabClientes = new TabPage
            {
                Text = "Clientes",
                BackColor = Color.FromArgb(248, 245, 240),
                Padding = new Padding(8)
            };

            var pnlBusca = new Panel
            {
                Dock = DockStyle.Top, Height = 42,
                BackColor = Color.FromArgb(235, 226, 208)
            };
            var txtBusca = new TextBox
            {
                Left = 8, Top = 8, Width = 280,
                Font = new Font("Segoe UI", 9.5F),
                PlaceholderText = "Buscar por nome ou celular..."
            };
            var btnBuscar = new Button
            {
                Left = 296, Top = 7, Width = 90, Height = 26,
                Text = "Buscar",
                BackColor = Color.FromArgb(176, 110, 42), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBuscar.FlatAppearance.BorderSize = 0;

            _gridClientes = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true, AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false, MultiSelect = false,
                BackgroundColor = Color.FromArgb(250, 246, 238),
                GridColor = Color.FromArgb(210, 200, 180),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9F),
            };
            _gridClientes.ColumnHeadersDefaultCellStyle.BackColor  = Color.FromArgb(176, 110, 42);
            _gridClientes.ColumnHeadersDefaultCellStyle.ForeColor  = Color.White;
            _gridClientes.ColumnHeadersDefaultCellStyle.Font       = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            _gridClientes.DefaultCellStyle.BackColor               = Color.FromArgb(250, 246, 238);
            _gridClientes.DefaultCellStyle.ForeColor               = Color.FromArgb(50, 40, 25);
            _gridClientes.DefaultCellStyle.SelectionBackColor      = Color.FromArgb(224, 113, 42);
            _gridClientes.DefaultCellStyle.SelectionForeColor      = Color.White;
            _gridClientes.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 234, 218);
            _gridClientes.EnableHeadersVisualStyles = false;
            _gridClientes.DataError += (s, e) => ((DataGridViewDataErrorEventArgs)e).ThrowException = false;

            pnlBusca.Controls.Add(btnBuscar);
            pnlBusca.Controls.Add(txtBusca);
            tabClientes.Controls.Add(_gridClientes);
            tabClientes.Controls.Add(pnlBusca);
            tabControl.Controls.Add(tabClientes);

            btnBuscar.Click += (_, __) => CarregarClientes(txtBusca.Text);
            txtBusca.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) CarregarClientes(txtBusca.Text); };
            tabControl.SelectedIndexChanged += (_, __) =>
            {
                if (tabControl.SelectedTab == tabClientes)
                    CarregarClientes(txtBusca.Text);
            };
        }

        private void CarregarClientes(string busca = "")
        {
            try
            {
                var dt = new ClienteBLL().Listar(busca?.Trim() ?? "");
                _gridClientes.DataSource = dt;
                ConfigurarGridClientes();
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void ConfigurarGridClientes()
        {
            if (_gridClientes == null || _gridClientes.Columns.Count == 0) return;
            foreach (DataGridViewColumn col in _gridClientes.Columns)
                col.Visible = false;

            void Col(string name, string header, float fill,
                DataGridViewContentAlignment align = DataGridViewContentAlignment.MiddleLeft)
            {
                if (!_gridClientes.Columns.Contains(name)) return;
                _gridClientes.Columns[name].Visible    = true;
                _gridClientes.Columns[name].HeaderText = header;
                _gridClientes.Columns[name].FillWeight = fill;
                _gridClientes.Columns[name].DefaultCellStyle.Alignment = align;
            }

            Col("Nome",          "Nome / Raz\u00e3o Social", 220);
            Col("Celular",       "Celular",       90,  DataGridViewContentAlignment.MiddleCenter);
            Col("TotalPedidos",  "Pedidos",        55,  DataGridViewContentAlignment.MiddleCenter);
            Col("PedidosMensal", "Pedidos M\u00eas", 65, DataGridViewContentAlignment.MiddleCenter);
            Col("TotalGasto",    "Gasto Ano R$",   90,  DataGridViewContentAlignment.MiddleRight);
            Col("GastoMensal",   "Gasto M\u00eas R$", 90, DataGridViewContentAlignment.MiddleRight);

            foreach (DataGridViewRow row in _gridClientes.Rows)
            {
                if (row.IsNewRow) continue;
                if (_gridClientes.Columns.Contains("TotalGasto") && row.Cells["TotalGasto"].Value is decimal d)
                    row.Cells["TotalGasto"].Value = d.ToString("N2");
                if (_gridClientes.Columns.Contains("GastoMensal") && row.Cells["GastoMensal"].Value is decimal dm)
                    row.Cells["GastoMensal"].Value = dm.ToString("N2");
            }
        }

        // ── Configuração ──────────────────────────────────────────────────────

        private void CarregarListaConfigs()
        {
            _carregandoConfigs = true;
            try
            {
                gridConfigs.DataSource = _bll.Listar();
                if (gridConfigs.Columns.Count > 0)
                {
                    if (gridConfigs.Columns["Codigo"] != null) gridConfigs.Columns["Codigo"].Visible = false;
                    if (gridConfigs.Columns["Nome"]   != null) { gridConfigs.Columns["Nome"].HeaderText = "Regra"; gridConfigs.Columns["Nome"].FillWeight = 45; }
                    if (gridConfigs.Columns["Ativo"]  != null) { gridConfigs.Columns["Ativo"].HeaderText = "Ativa"; gridConfigs.Columns["Ativo"].FillWeight = 10; }
                    if (gridConfigs.Columns["Meta"]   != null) { gridConfigs.Columns["Meta"].HeaderText = "Meta R$"; gridConfigs.Columns["Meta"].FillWeight = 18;
                                                                  gridConfigs.Columns["Meta"].DefaultCellStyle.Format = "N2"; }
                    if (gridConfigs.Columns["Tipo"]   != null) { gridConfigs.Columns["Tipo"].HeaderText = "Prêmio"; gridConfigs.Columns["Tipo"].FillWeight = 18; }
                }
                if (gridConfigs.Rows.Count > 0)
                    gridConfigs.Rows[0].Selected = true;
            }
            finally { _carregandoConfigs = false; }
        }

        private void GridConfigs_SelectionChanged(object sender, EventArgs e)
        {
            if (_carregandoConfigs) return;
            if (gridConfigs.SelectedRows.Count == 0) return;
            var row = gridConfigs.SelectedRows[0];
            if (row.Cells["Codigo"]?.Value == null || row.Cells["Codigo"].Value == DBNull.Value) return;
            int cod = Convert.ToInt32(row.Cells["Codigo"].Value);
            CarregarConfigDetalhe(cod);
        }

        private void CarregarConfigDetalhe(int codigo)
        {
            var cfg = _bll.Carregar(codigo);
            _codigoConfigEditando = cfg.Codigo;
            txtNomeRegra.Text       = cfg.fidNome;
            chkAtivo.Checked        = cfg.fidAtivo;
            numMeta.Value           = cfg.fidMeta_Gasto > 0 ? cfg.fidMeta_Gasto : 500m;
            cmbMetaTipo.SelectedIndex = cfg.fidMeta_Tipo == "PEDIDOS" ? 1 : 0;
            AtualizarLblMeta();
            rdCupom.Checked         = cfg.fidPremio_Tipo != "PRODUTO";
            rdProduto.Checked       = cfg.fidPremio_Tipo == "PRODUTO";
            cmbCupomTipo.SelectedIndex = cfg.fidCupom_Tipo == "FIXO" ? 1 : 0;
            numCupomValor.Value     = cfg.fidCupom_Valor > 0 ? Math.Min(cfg.fidCupom_Valor, numCupomValor.Maximum) : 10m;
            numCupomMin.Value       = cfg.fidCupom_Minimo >= 0 ? cfg.fidCupom_Minimo : 0m;
            numCupomValidade.Value  = cfg.fidCupom_Validade > 0 ? Math.Min(cfg.fidCupom_Validade, 365) : 30;
            txtProdNome.Text        = cfg.fidProduto_Nome ?? "";
            _produtoCodigo          = cfg.fidProduto_Codigo;
            numProdQtde.Value       = cfg.fidProduto_Qtde > 0 ? cfg.fidProduto_Qtde : 1;
            txtMsg.Text             = string.IsNullOrWhiteSpace(cfg.fidMensagem)
                                        ? "Parabéns {Nome}! Você atingiu R$ {Meta} em compras e ganhou um cupom {CupomCodigo} válido até {Validade}."
                                        : cfg.fidMensagem;
            AtualizarPainelPremio();
            btnSalvar.Text = "\u270E Salvar Edição";
        }

        private void BtnNovaRegra_Click(object sender, EventArgs e)
        {
            _codigoConfigEditando = 0;
            txtNomeRegra.Text       = "Nova Regra";
            chkAtivo.Checked        = true;
            numMeta.Value           = 500m;
            cmbMetaTipo.SelectedIndex = 0;
            AtualizarLblMeta();
            rdCupom.Checked         = true;
            rdProduto.Checked       = false;
            cmbCupomTipo.SelectedIndex = 0;
            numCupomValor.Value     = 10m;
            numCupomMin.Value       = 0m;
            numCupomValidade.Value  = 30;
            txtProdNome.Text        = "";
            _produtoCodigo          = 0;
            numProdQtde.Value       = 1;
            txtMsg.Text             = "Parabéns {Nome}! Você atingiu R$ {Meta} em compras e ganhou um cupom {CupomCodigo} válido até {Validade}.";
            gridConfigs.ClearSelection();
            AtualizarPainelPremio();
            btnSalvar.Text = "Salvar Nova Regra";
            txtNomeRegra.Focus();
            txtNomeRegra.SelectAll();
        }

        private void BtnExcluirRegra_Click(object sender, EventArgs e)
        {
            if (gridConfigs.SelectedRows.Count == 0) { MessageBox.Show("Selecione uma regra para excluir.", "Excluir Regra", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            int cod = Convert.ToInt32(gridConfigs.SelectedRows[0].Cells["Codigo"].Value);
            string nome = gridConfigs.SelectedRows[0].Cells["Nome"]?.Value?.ToString() ?? "";
            if (MessageBox.Show($"Excluir a regra \"{nome}\"?", "Confirmar Exclusão",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            var erro = _bll.Excluir(cod);
            if (!string.IsNullOrEmpty(erro))
                MessageBox.Show("Erro ao excluir: " + erro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                CarregarListaConfigs();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            var cfg = new ConfigFidelizacao
            {
                Codigo            = _codigoConfigEditando,
                fidNome           = txtNomeRegra.Text.Trim(),
                fidAtivo          = chkAtivo.Checked,
                fidMeta_Gasto     = numMeta.Value,
                fidMeta_Tipo      = cmbMetaTipo.SelectedIndex == 1 ? "PEDIDOS" : "VALOR",
                fidPremio_Tipo    = rdProduto.Checked ? "PRODUTO" : "CUPOM",
                fidCupom_Tipo     = cmbCupomTipo.SelectedIndex == 1 ? "FIXO" : "PERCENTUAL",
                fidCupom_Valor    = numCupomValor.Value,
                fidCupom_Minimo   = numCupomMin.Value,
                fidCupom_Validade = (int)numCupomValidade.Value,
                fidProduto_Nome   = txtProdNome.Text.Trim(),
                fidProduto_Codigo = _produtoCodigo,
                fidProduto_Qtde   = (int)numProdQtde.Value,
                fidMensagem       = txtMsg.Text.Trim(),
            };

            string erro = _codigoConfigEditando == 0 ? _bll.Incluir(cfg) : _bll.Alterar(cfg);
            if (!string.IsNullOrEmpty(erro))
            {
                MessageBox.Show("Erro ao salvar: " + erro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MessageBox.Show("Configuração salva com sucesso!", "Fidelização", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _codigoConfigEditando = cfg.Codigo;
            CarregarListaConfigs();
            // Re-select saved row
            foreach (DataGridViewRow r in gridConfigs.Rows)
            {
                if (r.Cells["Codigo"]?.Value != null && Convert.ToInt32(r.Cells["Codigo"].Value) == cfg.Codigo)
                { gridConfigs.ClearSelection(); r.Selected = true; break; }
            }
        }

        private void BtnBuscarProduto_Click(object sender, EventArgs e)
        {
            using var dlg = new Form();
            dlg.Text            = "Selecionar Produto";
            dlg.StartPosition   = FormStartPosition.CenterParent;
            dlg.Size            = new Size(600, 420);
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox     = dlg.MinimizeBox = false;
            dlg.BackColor       = Color.FromArgb(245, 237, 216);

            var txtFiltro = new TextBox
            {
                Dock = DockStyle.Top, Height = 28,
                Font = new Font("Segoe UI", 10F),
                PlaceholderText = "Filtrar por nome...",
                BorderStyle = BorderStyle.FixedSingle
            };

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
                BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 9F), MultiSelect = false
            };
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Codigo", HeaderText = "Cód", FillWeight = 10 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nome",   HeaderText = "Produto", FillWeight = 65 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Preco",  HeaderText = "Preço R$", FillWeight = 25 });

            var mercBLL = new MercadoriaBLL();
            var dt = mercBLL.Listar();

            void Preencher(string filtro)
            {
                grid.Rows.Clear();
                foreach (System.Data.DataRow r in dt.Rows)
                {
                    string nome = r["Nome"]?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(filtro) &&
                        nome.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) < 0) continue;
                    grid.Rows.Add(r["Codigo"], nome,
                        r["Preco"] == System.DBNull.Value ? "" : Convert.ToDecimal(r["Preco"]).ToString("N2"));
                }
            }
            Preencher("");
            txtFiltro.TextChanged += (_, __) => Preencher(txtFiltro.Text.Trim());

            (int cod, string nome) escolhido = (0, "");

            void Selecionar()
            {
                if (grid.CurrentRow == null) return;
                escolhido = (
                    Convert.ToInt32(grid.CurrentRow.Cells["Codigo"].Value),
                    grid.CurrentRow.Cells["Nome"].Value?.ToString() ?? "");
                dlg.DialogResult = DialogResult.OK;
            }

            grid.CellDoubleClick += (_, __) => Selecionar();

            var btnOk = new Button
            {
                Text = "Selecionar", Dock = DockStyle.Bottom, Height = 34,
                BackColor = Color.FromArgb(87, 120, 38), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += (_, __) => Selecionar();

            dlg.Controls.Add(grid);
            dlg.Controls.Add(btnOk);
            dlg.Controls.Add(txtFiltro);

            if (dlg.ShowDialog(this) == DialogResult.OK && escolhido.cod > 0)
            {
                _produtoCodigo   = escolhido.cod;
                txtProdNome.Text = escolhido.nome;
            }
        }

        private void ChkAtivo_CheckedChanged(object sender, EventArgs e)
        {
            // Visual feedback — no additional logic needed
        }

        private void RdPremio_CheckedChanged(object sender, EventArgs e)
        {
            AtualizarPainelPremio();
        }

        private void CmbMetaTipo_SelectedIndexChanged(object sender, EventArgs e)
            => AtualizarLblMeta();

        private void AtualizarLblMeta()
        {
            bool isPedidos = cmbMetaTipo.SelectedIndex == 1;
            lblMeta.Text          = isPedidos ? "Meta (Pedidos/mês):" : "Meta de Gasto (R$):";
            numMeta.DecimalPlaces = isPedidos ? 0 : 2;
            if (isPedidos && numMeta.Value != Math.Floor(numMeta.Value))
                numMeta.Value = Math.Floor(numMeta.Value);
        }

        private void AtualizarPainelPremio()
        {
            pnlCupom.Visible   = rdCupom.Checked;
            pnlProduto.Visible = rdProduto.Checked;
        }

        // ── Histórico ──────────────────────────────────────────────────────────

        private void CarregarHistorico()
        {
            try
            {
                gridHistorico.DataSource = _bll.ListarHistorico(dtpDe.Value, dtpAte.Value);
                AjustarColunas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar histórico: " + ex.Message);
            }
        }

        private void BtnFiltrar_Click(object sender, EventArgs e)
        {
            CarregarHistorico();
        }

        private void AjustarColunas()
        {
            if (gridHistorico.Columns.Count == 0) return;
            foreach (DataGridViewColumn c in gridHistorico.Columns)
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            if (gridHistorico.Columns.Contains("Codigo"))
                gridHistorico.Columns["Codigo"].Visible = false;
            if (gridHistorico.Columns.Contains("Data"))
            {
                gridHistorico.Columns["Data"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                gridHistorico.Columns["Data"].FillWeight = 60;
            }
            if (gridHistorico.Columns.Contains("Cliente"))
                gridHistorico.Columns["Cliente"].FillWeight = 120;
            if (gridHistorico.Columns.Contains("Telefone"))
                gridHistorico.Columns["Telefone"].FillWeight = 70;
            if (gridHistorico.Columns.Contains("Cupom"))
                gridHistorico.Columns["Cupom"].FillWeight = 80;
            if (gridHistorico.Columns.Contains("Descricao"))
                gridHistorico.Columns["Descricao"].FillWeight = 150;
        }

        private void Grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
