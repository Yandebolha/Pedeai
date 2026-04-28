using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmCadastroProduto : Form
    {
        private MercadoriaBLL      _bll;
        private GrupoMercadoriaBLL _grpBLL;
        private string    _caminhoImagem = "";
        private int       _codigoEditando = 0;
        private System.Data.DataTable _dtProdutos;
        private System.Windows.Forms.ComboBox _cmbCatFiltro;

        // Vinculo groups for Adicionais/Complementos
        private readonly List<VinculoItem> _vinculosAdicionais   = new List<VinculoItem>();
        private readonly List<VinculoItem> _vinculosComplementos = new List<VinculoItem>();

        private struct VinculoItem
        {
            public int    CodigoGrupo;
            public string NomeGrupo;
            public override string ToString() => NomeGrupo;
        }

        public frmCadastroProduto()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _bll = new MercadoriaBLL(); _grpBLL = new GrupoMercadoriaBLL();
            Load += (_, __) => { AdicionarFiltroCat(); CarregarGrid(); };
        }


        private void BtnImagem_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog
            {
                Title  = "Selecionar imagem do produto",
                Filter = "Imagens|*.jpg;*.jpeg;*.png;*.gif;*.webp|Todos|*.*"
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                _caminhoImagem = dlg.FileName;
                lblImagem.Text = Path.GetFileName(_caminhoImagem);
                lblImagem.ForeColor = Color.FromArgb(30, 120, 30);
            }
        }

        private void AdicionarFiltroCat()
        {
            _cmbCatFiltro = new System.Windows.Forms.ComboBox
            { Left = _txtFiltro.Right + 8, Top = 7, Width = 180,
              DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList,
              Font = new System.Drawing.Font("Segoe UI", 9F) };
            _cmbCatFiltro.Items.Add("(Todas as categorias)");
            _cmbCatFiltro.SelectedIndex = 0;
            pnlSearch.Controls.Add(_cmbCatFiltro);
            btnPesq.Left = _cmbCatFiltro.Right + 8;
            _cmbCatFiltro.SelectedIndexChanged += (_, __) => FiltrarGrid(_txtFiltro?.Text ?? "");
        }

        private void CarrecarComboCategorias()
        {
            try
            {
                cmbCategoria.Items.Clear();
                var dt = _grpBLL.Listar(true);
                foreach (System.Data.DataRow r in dt.Rows)
                    cmbCategoria.Items.Add(new CatItem(Convert.ToInt32(r["Codigo"]), r["Nome"]?.ToString() ?? ""));
            }
            catch { }
        }

        private void CarregarGrid()
        {
            try
            {
                _dtProdutos = _bll.Listar();
                // Atualiza combo de categorias de filtro
                if (_cmbCatFiltro != null)
                {
                    string selCat = _cmbCatFiltro.SelectedIndex > 0 ? _cmbCatFiltro.SelectedItem?.ToString() : null;
                    _cmbCatFiltro.Items.Clear();
                    _cmbCatFiltro.Items.Add("(Todas as categorias)");
                    var categorias = new System.Collections.Generic.HashSet<string>();
                    foreach (System.Data.DataRow r in _dtProdutos.Rows)
                    {
                        string cat = r["Categoria"]?.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(cat)) categorias.Add(cat);
                    }
                    foreach (var c in categorias)
                        _cmbCatFiltro.Items.Add(c);
                    _cmbCatFiltro.SelectedIndex = selCat != null && _cmbCatFiltro.Items.Contains(selCat)
                        ? _cmbCatFiltro.Items.IndexOf(selCat) : 0;
                }
                FiltrarGrid(_txtFiltro?.Text ?? "");
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void FiltrarGrid(string filtro)
        {
            if (_dtProdutos == null) return;
            var dv = new System.Data.DataView(_dtProdutos);
            var conditions = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var f = filtro.Replace("'", "''");
                conditions.Add($"(Nome LIKE '%{f}%' OR Categoria LIKE '%{f}%')");
            }
            if (_cmbCatFiltro != null && _cmbCatFiltro.SelectedIndex > 0)
            {
                var cat = (_cmbCatFiltro.SelectedItem?.ToString() ?? "").Replace("'", "''");
                conditions.Add($"Categoria = '{cat}'");
            }
            if (conditions.Count > 0)
                dv.RowFilter = string.Join(" AND ", conditions);
            grid.DataSource = dv;
            ConfigurarColunasProdutos();
        }

        private void ConfigurarColunasProdutos()
        {
            if (grid.Columns.Count == 0) return;
            foreach (DataGridViewColumn col in grid.Columns)
                col.Visible = false;
            var show = new[] { "Nome", "Categoria", "Preco", "Estoque" };
            foreach (var name in show)
                if (grid.Columns.Contains(name)) grid.Columns[name].Visible = true;
            var caps = new System.Collections.Generic.Dictionary<string, string>
            {
                ["Preco"] = "Pre\u00e7o",
            };
            foreach (var kv in caps)
                if (grid.Columns.Contains(kv.Key)) grid.Columns[kv.Key].HeaderText = kv.Value;
            // Formato decimal com 2 casas
            if (grid.Columns.Contains("Preco"))   grid.Columns["Preco"].DefaultCellStyle.Format   = "N2";
            if (grid.Columns.Contains("Estoque")) grid.Columns["Estoque"].DefaultCellStyle.Format = "N2";
        }

        private void ModoNovo()
        {
            CarrecarComboCategorias();
            CarrecarComboGruposVinc();
            _codigoEditando = 0;
            cmbCategoria.Text = "";
            txtNome.Clear();
            _caminhoImagem = ""; lblImagem.Text = "nenhuma imagem selecionada"; lblImagem.ForeColor = Color.Gray;
            numPreco.Value = 0; numCusto.Value = 0; numPromo.Value = 0; numEstoque.Value = 0;
            chkControlaEstoque.Checked = false; chkDestaque.Checked = false; chkSite.Checked = false;
            chkAdicionais.Checked = false; chkComplementos.Checked = false;
            _vinculosAdicionais.Clear(); _vinculosComplementos.Clear();
            AtualizarListaVinculos();
            cmbSituacao.SelectedIndex = 0;
            pnlForm.Visible = true; txtNome.Focus();
        }

        private void CarregarParaEditar()
        {
            if (grid.SelectedRows.Count == 0) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var obj = _bll.PesquisaCodigo(cod);
            if (obj == null) return;
            CarrecarComboCategorias();
            CarrecarComboGruposVinc();
            _codigoEditando   = cod;
            txtNome.Text      = obj.mercMercadoria ?? "";
            _caminhoImagem    = obj.mercImagem_Url ?? "";
            lblImagem.Text    = string.IsNullOrEmpty(_caminhoImagem) ? "nenhuma imagem selecionada" : Path.GetFileName(_caminhoImagem);
            lblImagem.ForeColor = string.IsNullOrEmpty(_caminhoImagem) ? Color.Gray : Color.FromArgb(30, 120, 30);
            numPreco.Value    = obj.mercPreco_Venda;
            numCusto.Value    = obj.mercPreco_Custo;
            numPromo.Value    = obj.mercPreco_Promocional;
            numEstoque.Value  = obj.mercEstoque_Atual;
            chkControlaEstoque.Checked = obj.mercControla_Estoque;
            chkDestaque.Checked        = obj.mercDestaque;
            chkSite.Checked            = obj.mercHabilitar_Site;
            cmbSituacao.SelectedItem   = obj.Situacao == "I" ? "Inativo" : "Ativo";
            cmbCategoria.Text = "";
            foreach (CatItem item in cmbCategoria.Items)
                if (item.Codigo == obj.Codigo_Grupo) { cmbCategoria.SelectedItem = item; break; }
            // Load vinculos
            _vinculosAdicionais.Clear(); _vinculosComplementos.Clear();
            CarregarVinculosDoDb(cod);
            chkAdicionais.Checked   = _vinculosAdicionais.Count > 0;
            chkComplementos.Checked = _vinculosComplementos.Count > 0;
            AtualizarListaVinculos();
            pnlForm.Visible = true; txtNome.Focus();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text)) { MessageBox.Show("Informe o nome do produto."); return; }

            int codigoGrupo = 0;
            if (cmbCategoria.SelectedItem is CatItem selItem)
                codigoGrupo = selItem.Codigo;
            else
                foreach (CatItem item in cmbCategoria.Items)
                    if (item.Nome.Equals(cmbCategoria.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                    { codigoGrupo = item.Codigo; break; }
            if (codigoGrupo == 0) { MessageBox.Show("Selecione ou digite uma categoria valida."); return; }

            var obj = new Mercadoria
            {
                Codigo                = _codigoEditando,
                Codigo_Grupo          = codigoGrupo,
                mercMercadoria        = txtNome.Text.Trim(),
                mercApresentacao      = "",
                mercPreco_Venda       = numPreco.Value,
                mercPreco_Custo       = numCusto.Value,
                mercPreco_Promocional = numPromo.Value,
                mercEstoque_Atual     = numEstoque.Value,
                mercControla_Estoque  = chkControlaEstoque.Checked,
                mercImagem_Url        = _caminhoImagem?.StartsWith("http", StringComparison.OrdinalIgnoreCase) == true
                                         ? _caminhoImagem : "",
                mercDestaque          = chkDestaque.Checked,
                mercOrdem             = 0,
                mercHabilitar_Site    = chkSite.Checked,
                Situacao              = cmbSituacao.SelectedItem?.ToString() == "Inativo" ? "I" : "A",
            };
            var erro = _bll.Salvar(obj);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            pnlForm.Visible = false; _codigoEditando = 0; CarregarGrid();
            // Sync to Supabase in background — upload image if it's a local file path
            int codSalvo = obj.Codigo > 0 ? obj.Codigo : BuscarCodigo(obj.mercMercadoria);
            string imagemLocal = _caminhoImagem;
            _caminhoImagem = "";
            // Save vinculos to DB
            if (codSalvo > 0) SalvarVinculosNoDb(codSalvo);
            var vincsAd   = new List<VinculoItem>(_vinculosAdicionais);
            var vincsComp = new List<VinculoItem>(_vinculosComplementos);
            if (codSalvo > 0)
                System.Threading.Tasks.Task.Run(async () =>
                {
                    // Upload image only if it's a local file (not already a URL)
                    if (!string.IsNullOrWhiteSpace(imagemLocal)
                        && !imagemLocal.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                        && System.IO.File.Exists(imagemLocal))
                    {
                        await DB.SupabaseService.UploadProdutoImagemAsync(codSalvo, imagemLocal);
                    }
                    await DB.SupabaseService.SincronizarProdutoAsync(codSalvo);
                    var adTuples   = vincsAd.ConvertAll(v => (v.CodigoGrupo, v.NomeGrupo));
                    var compTuples = vincsComp.ConvertAll(v => (v.CodigoGrupo, v.NomeGrupo));
                    await DB.SupabaseService.SincronizarVinculosGrupoAsync(codSalvo, adTuples, compTuples);
                });
        }

        private int BuscarCodigo(string nome)
        {
            try
            {
                if (_dtProdutos == null) return 0;
                foreach (System.Data.DataRow r in _dtProdutos.Rows)
                    if (r["Nome"]?.ToString() == nome)
                        return Convert.ToInt32(r["Codigo"]);
            }
            catch { }
            return 0;
        }

        private void BtnDesativar_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            if (MessageBox.Show("Desativar produto?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            int cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            _bll.AlternarSituacao(cod);
            pnlForm.Visible = false; CarregarGrid();
            System.Threading.Tasks.Task.Run(async () =>
                await DB.SupabaseService.SincronizarProdutoAsync(cod));
        }

        // ── Vinculos helpers ──────────────────────────────────────────────────

        private void CarrecarComboGruposVinc()
        {
            cmbGrupoVinc.Items.Clear();
            try
            {
                var dt = _grpBLL.Listar(true);
                foreach (System.Data.DataRow r in dt.Rows)
                    cmbGrupoVinc.Items.Add(new CatItem(Convert.ToInt32(r["Codigo"]), r["Nome"]?.ToString() ?? ""));
            }
            catch { }
            if (cmbGrupoVinc.Items.Count > 0) cmbGrupoVinc.SelectedIndex = 0;
        }

        private void ChkVinculos_CheckedChanged(object sender, EventArgs e)
        {
            pnlVinculos.Visible = chkAdicionais.Checked || chkComplementos.Checked;
            AtualizarListaVinculos();
        }

        private void AtualizarListaVinculos()
        {
            lstGruposVinc.Items.Clear();
            if (chkAdicionais.Checked)
                foreach (var v in _vinculosAdicionais)
                    lstGruposVinc.Items.Add($"[Adicional] {v.NomeGrupo}|A|{v.CodigoGrupo}");
            if (chkComplementos.Checked)
                foreach (var v in _vinculosComplementos)
                    lstGruposVinc.Items.Add($"[Complemento] {v.NomeGrupo}|C|{v.CodigoGrupo}");
        }

        private void BtnAddGrupoVinc_Click(object sender, EventArgs e)
        {
            if (!(cmbGrupoVinc.SelectedItem is CatItem cat)) return;
            if (!chkAdicionais.Checked && !chkComplementos.Checked) return;

            // Determine type (if both checked, pop a small dialog; default to first checked)
            string tipo;
            if (chkAdicionais.Checked && chkComplementos.Checked)
            {
                var res = MessageBox.Show("Adicionar como Adicional?\nClique Não para Complemento.",
                    "Tipo de vínculo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                tipo = res == DialogResult.Yes ? "A" : "C";
            }
            else tipo = chkAdicionais.Checked ? "A" : "C";

            var lista = tipo == "A" ? _vinculosAdicionais : _vinculosComplementos;
            if (lista.Exists(x => x.CodigoGrupo == cat.Codigo)) return; // already added
            lista.Add(new VinculoItem { CodigoGrupo = cat.Codigo, NomeGrupo = cat.Nome });
            AtualizarListaVinculos();
        }

        private void BtnRemGrupoVinc_Click(object sender, EventArgs e)
        {
            if (lstGruposVinc.SelectedItem == null) return;
            string sel = lstGruposVinc.SelectedItem.ToString() ?? "";
            var parts = sel.Split('|');
            if (parts.Length < 3) return;
            string tipo = parts[1];
            if (int.TryParse(parts[2], out int cod))
            {
                if (tipo == "A") _vinculosAdicionais.RemoveAll(x => x.CodigoGrupo == cod);
                else             _vinculosComplementos.RemoveAll(x => x.CodigoGrupo == cod);
            }
            AtualizarListaVinculos();
        }

        private void CarregarVinculosDoDb(int codigoMercadoria)
        {
            try
            {
                using var conn = new MySqlConnector.MySqlConnection(
                    System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]);
                conn.Open();
                using var cmd = new MySqlConnector.MySqlCommand(
                    "SELECT mvg.Codigo_Grupo, mvg.tipo, COALESCE(gm.grmeNome,'') AS Nome " +
                    "FROM mercadoria_vinculo_grupo mvg " +
                    "LEFT JOIN grupo_mercadoria gm ON gm.Codigo=mvg.Codigo_Grupo " +
                    "WHERE mvg.Codigo_Mercadoria=@cod AND mvg.Situacao='A'", conn);
                cmd.Parameters.AddWithValue("@cod", codigoMercadoria);
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var vi = new VinculoItem { CodigoGrupo = dr.GetInt32(0), NomeGrupo = dr.GetString(2) };
                    if (dr.GetString(1) == "A") _vinculosAdicionais.Add(vi);
                    else                        _vinculosComplementos.Add(vi);
                }
            }
            catch { }
        }

        private void SalvarVinculosNoDb(int codigoMercadoria)
        {
            try
            {
                using var conn = new MySqlConnector.MySqlConnection(
                    System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]);
                conn.Open();
                // Mark all existing as inactive then re-insert active ones
                using (var del = new MySqlConnector.MySqlCommand(
                    "UPDATE mercadoria_vinculo_grupo SET Situacao='I' WHERE Codigo_Mercadoria=@cod", conn))
                {
                    del.Parameters.AddWithValue("@cod", codigoMercadoria);
                    del.ExecuteNonQuery();
                }
                void UpsertVinculo(int codGrupo, string tipo)
                {
                    using var ins = new MySqlConnector.MySqlCommand(@"
                        INSERT INTO mercadoria_vinculo_grupo (Codigo_Mercadoria, Codigo_Grupo, tipo, Situacao)
                        VALUES(@m,@g,@t,'A')
                        ON DUPLICATE KEY UPDATE Situacao='A'", conn);
                    ins.Parameters.AddWithValue("@m", codigoMercadoria);
                    ins.Parameters.AddWithValue("@g", codGrupo);
                    ins.Parameters.AddWithValue("@t", tipo);
                    ins.ExecuteNonQuery();
                }
                if (chkAdicionais.Checked)
                    foreach (var v in _vinculosAdicionais)   UpsertVinculo(v.CodigoGrupo, "A");
                if (chkComplementos.Checked)
                    foreach (var v in _vinculosComplementos) UpsertVinculo(v.CodigoGrupo, "C");
            }
            catch (Exception ex) { Logger.Log("frmCadastroProduto","SalvarVinculosNoDb","Erro",ex); }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private async void BtnSincSite_Click(object sender, EventArgs e)
        {
            btnSincSite.Enabled = false;
            btnSincSite.Text    = "Sincronizando...";
            try
            {
                await DB.SupabaseService.SincronizarTodosProdutosAsync();
                await DB.SupabaseService.SincronizarTodasImagensAsync();
                MessageBox.Show("Produtos sincronizados com o site com sucesso!", "Sincronização",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro na sincronização: " + ex.Message, "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSincSite.Enabled = true;
                btnSincSite.Text    = "\u2601 Sincronizar Site";
            }
        }

        private class CatItem
        {
            public int Codigo; public string Nome;
            public CatItem(int c, string n) { Codigo = c; Nome = n; }
            public override string ToString() => Nome;
        }
    }
}
