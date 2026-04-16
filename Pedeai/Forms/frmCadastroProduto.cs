using System;
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
            { Left = 380, Top = 7, Width = 180,
              DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList,
              Font = new System.Drawing.Font("Segoe UI", 9F) };
            _cmbCatFiltro.Items.Add("(Todas as categorias)");
            _cmbCatFiltro.SelectedIndex = 0;
            pnlSearch.Controls.Add(_cmbCatFiltro);
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
        }

        private void ModoNovo()
        {
            CarrecarComboCategorias();
            _codigoEditando = 0;
            cmbCategoria.Text = "";
            txtNome.Clear();
            _caminhoImagem = ""; lblImagem.Text = "nenhuma imagem selecionada"; lblImagem.ForeColor = Color.Gray;
            numPreco.Value = 0; numCusto.Value = 0; numPromo.Value = 0; numEstoque.Value = 0;
            chkControlaEstoque.Checked = false; chkDestaque.Checked = false; chkSite.Checked = false;
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
                mercImagem_Url        = _caminhoImagem,
                mercDestaque          = chkDestaque.Checked,
                mercOrdem             = 0,
                mercHabilitar_Site    = chkSite.Checked,
                Situacao              = cmbSituacao.SelectedItem?.ToString() == "Inativo" ? "I" : "A",
            };
            var erro = _bll.Salvar(obj);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            pnlForm.Visible = false; _codigoEditando = 0; CarregarGrid();
        }

        private void BtnDesativar_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            if (MessageBox.Show("Desativar produto?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            _bll.AlternarSituacao(Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value));
            pnlForm.Visible = false; CarregarGrid();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private class CatItem
        {
            public int Codigo; public string Nome;
            public CatItem(int c, string n) { Codigo = c; Nome = n; }
            public override string ToString() => Nome;
        }
    }
}
