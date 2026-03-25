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
        private readonly MercadoriaBLL      _bll    = new MercadoriaBLL();
        private readonly GrupoMercadoriaBLL _grpBLL = new GrupoMercadoriaBLL();
        private string _caminhoImagem = "";
        private int    _codigoEditando = 0;

        public frmCadastroProduto()
        {
            InitializeComponent();
            BuildUI();
            if (!DesignMode) CarregarGrid();
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
            try { grid.DataSource = _bll.Listar(); }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void ModoNovo()
        {
            CarrecarComboCategorias();
            _codigoEditando = 0;
            cmbCategoria.Text = "";
            txtNome.Clear(); txtDescricao.Clear();
            _caminhoImagem = ""; lblImagem.Text = "nenhuma imagem selecionada"; lblImagem.ForeColor = Color.Gray;
            numPreco.Value = 0; numCusto.Value = 0; numPromo.Value = 0; numEstoque.Value = 0;
            chkControlaEstoque.Checked = false; chkDestaque.Checked = false; chkIfood.Checked = false;
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
            txtDescricao.Text = obj.mercApresentacao ?? "";
            _caminhoImagem    = obj.mercImagem_Url ?? "";
            lblImagem.Text    = string.IsNullOrEmpty(_caminhoImagem) ? "nenhuma imagem selecionada" : Path.GetFileName(_caminhoImagem);
            lblImagem.ForeColor = string.IsNullOrEmpty(_caminhoImagem) ? Color.Gray : Color.FromArgb(30, 120, 30);
            numPreco.Value    = obj.mercPreco_Venda;
            numCusto.Value    = obj.mercPreco_Custo;
            numPromo.Value    = obj.mercPreco_Promocional;
            numEstoque.Value  = obj.mercEstoque_Atual;
            chkControlaEstoque.Checked = obj.mercControla_Estoque;
            chkDestaque.Checked        = obj.mercDestaque;
            chkIfood.Checked           = obj.mercHabilitar_Ifood;
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
                mercApresentacao      = txtDescricao.Text.Trim(),
                mercPreco_Venda       = numPreco.Value,
                mercPreco_Custo       = numCusto.Value,
                mercPreco_Promocional = numPromo.Value,
                mercEstoque_Atual     = numEstoque.Value,
                mercControla_Estoque  = chkControlaEstoque.Checked,
                mercImagem_Url        = _caminhoImagem,
                mercDestaque          = chkDestaque.Checked,
                mercOrdem             = 0,
                mercHabilitar_Ifood   = chkIfood.Checked,
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

        private class CatItem
        {
            public int Codigo; public string Nome;
            public CatItem(int c, string n) { Codigo = c; Nome = n; }
            public override string ToString() => Nome;
        }
    }
}