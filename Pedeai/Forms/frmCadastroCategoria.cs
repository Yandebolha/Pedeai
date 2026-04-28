using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmCadastroCategoria : Form
    {
        private GrupoMercadoriaBLL _bll;
        private int _codigoEditando = 0;
        private System.Data.DataTable _allCategorias;
        private System.Windows.Forms.TextBox _txtBuscaCategoria;
        private string _imagemPath = "";   // local file path selected by user
        private Image  _previewImg = null; // stored separately to control paint fully

        public frmCadastroCategoria()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _bll = new GrupoMercadoriaBLL();
            // Disable default image painting; we draw manually in Paint
            picImagem.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            picImagem.Image    = null;
            picImagem.Paint   += PicImagem_Paint;
            Load += (_, __) => { AdicionarPainelBusca(); Carregar(); };
        }

        // ── circular clip on picImagem ────────────────────────────────
        private void PicImagem_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Fill background first
            g.Clear(picImagem.BackColor);

            if (_previewImg == null) return;

            // Clip to ellipse and draw image
            using var path = new GraphicsPath();
            path.AddEllipse(0, 0, picImagem.Width - 1, picImagem.Height - 1);
            g.SetClip(path);
            g.DrawImage(_previewImg, 0, 0, picImagem.Width, picImagem.Height);
        }

        private void BtnImagem_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title  = "Selecionar imagem da categoria",
                Filter = "Imagens|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            _imagemPath = dlg.FileName;
            try
            {
                // Dispose previous preview to avoid file locks
                _previewImg?.Dispose();
                // Load a copy so we don't lock the file
                using var tmp = Image.FromFile(_imagemPath);
                _previewImg = new Bitmap(tmp);
                picImagem.Refresh();
                lblImagem.Text = Path.GetFileName(_imagemPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar imagem: " + ex.Message);
                _imagemPath = "";
            }
        }

        // ── search panel ──────────────────────────────────────────────
        private void AdicionarPainelBusca()
        {
            var pnlSearch = new System.Windows.Forms.Panel
            { Dock = System.Windows.Forms.DockStyle.Top, Height = 38,
              BackColor = System.Drawing.Color.FromArgb(235, 226, 208) };
            _txtBuscaCategoria = new System.Windows.Forms.TextBox
            { Left = 8, Top = 8, Width = 260,
              Font = new System.Drawing.Font("Segoe UI", 9.5F),
              PlaceholderText = "Buscar pelo nome da categoria..." };
            var btnBuscar = new System.Windows.Forms.Button
            { Left = 276, Top = 7, Width = 90, Height = 26, Text = "Buscar",
              BackColor = System.Drawing.Color.FromArgb(224, 113, 42),
              ForeColor = System.Drawing.Color.White,
              FlatStyle = System.Windows.Forms.FlatStyle.Flat,
              Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold),
              Cursor = Cursors.Hand };
            btnBuscar.FlatAppearance.BorderSize = 0;
            btnBuscar.Click += (_, __) => FiltrarGrid(_txtBuscaCategoria.Text);
            _txtBuscaCategoria.KeyDown += (s, ev) => { if (ev.KeyCode == System.Windows.Forms.Keys.Enter) FiltrarGrid(_txtBuscaCategoria.Text); };
            pnlSearch.Controls.Add(_txtBuscaCategoria);
            pnlSearch.Controls.Add(btnBuscar);
            Controls.Add(pnlSearch);
            Controls.SetChildIndex(pnlSearch, 1);
        }

        private void Carregar()
        {
            try
            {
                _allCategorias = _bll.Listar();
                FiltrarGrid(_txtBuscaCategoria?.Text ?? "");
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void FiltrarGrid(string filtro)
        {
            if (_allCategorias == null) return;
            var dv = new System.Data.DataView(_allCategorias);
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var f = filtro.Replace("'", "''");
                dv.RowFilter = $"Nome LIKE '%{f}%'";
            }
            grid.DataSource = dv;
            foreach (DataGridViewColumn col in grid.Columns)
                col.Visible = col.Name == "Codigo" || col.Name == "Nome";
            if (grid.Columns.Contains("Codigo")) grid.Columns["Codigo"].Visible = false;
        }

        private void ModoNovo()
        {
            _codigoEditando = 0;
            txtNome.Clear();
            cmbSituacao.SelectedIndex = 0;
            chkHabSite.Checked = false;
            _previewImg?.Dispose(); _previewImg = null;
            _imagemPath = "";
            lblImagem.Text = "Nenhuma imagem selecionada";
            picImagem.Refresh();
            pnlForm.Visible = true;
            txtNome.Focus();
        }

        private void CarregarParaEditar()
        {
            if (grid.SelectedRows.Count == 0) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var obj = _bll.PesquisaCodigo(cod);
            if (obj == null) return;
            _codigoEditando    = cod;
            txtNome.Text       = obj.grmeDescricao_ ?? "";
            cmbSituacao.SelectedItem = obj.Situacao;
            chkHabSite.Checked = obj.grmeHabilitar_Site;
            _imagemPath        = "";
            _previewImg?.Dispose(); _previewImg = null;

            // Show existing URL as label
            lblImagem.Text = !string.IsNullOrWhiteSpace(obj.grmeImagem_Url)
                ? obj.grmeImagem_Url
                : "Nenhuma imagem selecionada";
            picImagem.Refresh();

            pnlForm.Visible = true;
            txtNome.Focus();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            var obj = new GrupoMercadoria
            {
                Codigo             = _codigoEditando,
                grmeDescricao_     = txtNome.Text.Trim(),
                grmeOrdem          = 0,
                Situacao           = _codigoEditando == 0 ? "A" : (cmbSituacao.SelectedItem?.ToString() ?? "A"),
                grmeHabilitar_Site = chkHabSite.Checked,
                grmeImagem_Url     = !string.IsNullOrWhiteSpace(_imagemPath)
                                     ? (_imagemPath.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? _imagemPath : "")
                                     : (lblImagem.Text.Trim() == "Nenhuma imagem selecionada" ? "" : lblImagem.Text.Trim()),
            };

            // Clear placeholder text so we don't store it
            if (obj.grmeImagem_Url == "Nenhuma imagem selecionada") obj.grmeImagem_Url = "";

            var erro = _bll.Salvar(obj);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }

            int savedCod = obj.Codigo;   // Incluir/Alterar sets Codigo on obj
            string savedPath = _imagemPath;
            pnlForm.Visible = false; _codigoEditando = 0; _imagemPath = ""; Carregar();

            // Sync to Supabase in background — will upload image if local path set
            Task.Run(async () =>
            {
                if (!string.IsNullOrWhiteSpace(savedPath))
                    await DB.SupabaseService.UploadCategoriaImagemAsync(savedCod, savedPath);
                await DB.SupabaseService.SincronizarGrupoAsync(savedCod);
            });
        }

        private void BtnDesativar_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            if (MessageBox.Show("Desativar categoria?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var obj = _bll.PesquisaCodigo(cod);
            if (obj == null) return;
            obj.Situacao = "I";
            _bll.Salvar(obj);
            pnlForm.Visible = false; Carregar();
            int codD = cod;
            Task.Run(async () => await DB.SupabaseService.SincronizarGrupoAsync(codD));
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
