using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public class frmCadastroMarmita : Form
    {
        private readonly MarmitaBLL   _bll     = new MarmitaBLL();
        private readonly MercadoriaBLL _mercBll = new MercadoriaBLL();

        // Paleta de cores do sistema
        private static readonly Color ClrBg     = Color.FromArgb(245, 237, 216);
        private static readonly Color ClrHeader = Color.FromArgb(176, 110, 42);
        private static readonly Color ClrText   = Color.FromArgb(50, 40, 25);
        private static readonly Color ClrFoot   = Color.FromArgb(235, 226, 208);
        private static readonly Color ClrOrange = Color.FromArgb(224, 113, 42);
        private static readonly Color ClrBrown  = Color.FromArgb(120, 100, 68);
        private static readonly Color ClrRed    = Color.FromArgb(192, 57, 43);
        private static readonly Color ClrGreen  = Color.FromArgb(39, 130, 57);
        private static readonly Color ClrGrid1  = Color.FromArgb(250, 246, 238);
        private static readonly Color ClrGrid2  = Color.FromArgb(240, 234, 218);
        private static readonly Color ClrGHdr   = Color.FromArgb(176, 110, 42);

        // Marmita em edição
        private int _codigoEditando = 0;

        // Marmita image
        private string _marmitaImagemPath = "";
        private System.Drawing.Image _marmitaPreviewImg = null;

        // Produtos disponíveis (para adicionar aos itens)
        private readonly List<(int Codigo, string Nome, decimal Preco)> _produtos
            = new List<(int, string, decimal)>();

        // Controles
        private DataGridView gridMarmitas;
        private DataGridView gridItens;
        private Panel pnlForm;
        private TextBox txtDescricao;
        private NumericUpDown numValor;
        private Label lblFormTitulo;
        private Button btnNovaMAR, btnSalvar, btnExcluir, btnFechar;
        private Button btnAddItem, btnRemItem;
        private NumericUpDown numCusto;
        private NumericUpDown numMaxComp;
        private CheckBox chkHabilitarSite, chkDestaque;
        private PictureBox picMarmita;
        private Label lblMarmitaImagem;

        public frmCadastroMarmita()
        {
            InitUI();
            if (System.ComponentModel.LicenseManager.UsageMode
                    == System.ComponentModel.LicenseUsageMode.Designtime) return;
            Load += (_, __) => { CarregarProdutos(); CarregarGrid(); };
        }

        // ── Construção da UI ─────────────────────────────────────────────────

        private void InitUI()
        {
            Text            = "Cadastro de Marmitas";
            BackColor       = ClrBg;
            Font            = new Font("Segoe UI", 9F);
            StartPosition   = FormStartPosition.CenterScreen;
            Size            = new Size(900, 640);
            MinimumSize     = new Size(700, 520);

            // ── Cabeçalho ──────────────────────────────────────────────────
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = ClrHeader };
            var lblTit = new Label
            {
                Text      = "\U0001F96B  Marmitas",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 13F, FontStyle.Bold),
                Dock      = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter
            };
            pnlTop.Controls.Add(lblTit);

            // ── Toolbar ───────────────────────────────────────────────────
            var pnlTool = new Panel
                { Dock = DockStyle.Top, Height = 44, BackColor = ClrFoot, Padding = new Padding(8, 6, 8, 6) };
            btnNovaMAR = MkBtn("+ Nova Marmita", ClrOrange, 160);
            btnNovaMAR.Click += BtnNova_Click;
            btnExcluir = MkBtn("Excluir", ClrRed, 100);
            btnExcluir.Click += BtnExcluir_Click;
            var btnSincSite = MkBtn("\u2601 Sincronizar Site", Color.FromArgb(30, 120, 200), 140);
            btnSincSite.Click += BtnSincSite_Click;
            btnFechar = MkBtn("Fechar", ClrBrown, 90);
            btnFechar.Click += (_, __) => Close();
            btnExcluir.Left = 176; btnSincSite.Left = 284; btnFechar.Left = 432;
            pnlTool.Controls.Add(btnNovaMAR);
            pnlTool.Controls.Add(btnExcluir);
            pnlTool.Controls.Add(btnSincSite);
            pnlTool.Controls.Add(btnFechar);

            // ── Grid marmitas ─────────────────────────────────────────────
            var pnlGridTop = new Panel { Dock = DockStyle.Left, Width = 420, Padding = new Padding(8, 6, 4, 6) };
            pnlGridTop.BackColor = ClrBg;

            var lblGridTit = new Label
            {
                Text = "Marmitas cadastradas",
                ForeColor = Color.FromArgb(100, 80, 50),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Dock = DockStyle.Top, Height = 22
            };
            gridMarmitas = MkGrid();
            gridMarmitas.Dock = DockStyle.Fill;
            gridMarmitas.SelectionChanged += GridMarmitas_SelectionChanged;
            gridMarmitas.CellDoubleClick  += GridMarmitas_DoubleClick;
            pnlGridTop.Controls.Add(gridMarmitas);
            pnlGridTop.Controls.Add(lblGridTit);

            // ── Painel direito: form + itens ──────────────────────────────
            var pnlRight = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4, 6, 8, 6), BackColor = ClrBg };

            // Formulário de cadastro
            pnlForm = new Panel
            {
                Dock = DockStyle.Top, Height = 165, BackColor = ClrFoot,
                Padding = new Padding(10, 8, 10, 8), Visible = false
            };
            lblFormTitulo = new Label
            {
                Text = "Nova Marmita",
                ForeColor = Color.FromArgb(100, 80, 50),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Dock = DockStyle.Top, Height = 22
            };
            // Row 1: description
            var pnlRowDesc = new Panel { Dock = DockStyle.Top, Height = 36, BackColor = ClrFoot };
            txtDescricao = new TextBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White,
                ForeColor = ClrText, Font = new Font("Segoe UI", 10F)
            };
            var lblDesc = new Label
            {
                Text = "Descri\u00e7\u00e3o:", Width = 75, Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(100, 80, 50)
            };
            pnlRowDesc.Controls.Add(txtDescricao);
            pnlRowDesc.Controls.Add(lblDesc);
            // Row 2: cost / value / max complementos
            var pnlFields = new Panel { Dock = DockStyle.Top, Height = 36, BackColor = ClrFoot };
            // Valor R$ — fixed width on the right
            var lblVal = new Label
            {
                Text = "Valor R$:", Width = 60, Dock = DockStyle.Right,
                TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(100, 80, 50)
            };
            numValor = new NumericUpDown
            {
                Width = 90, Dock = DockStyle.Right,
                DecimalPlaces = 2, Minimum = 0, Maximum = 99999,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.White, ForeColor = ClrText, ThousandsSeparator = true,
                Margin = new Padding(0, 4, 4, 4)
            };
            var lblCusto = new Label
            {
                Text = "Custo R$:", Width = 60, Dock = DockStyle.Right,
                TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(100, 80, 50)
            };
            numCusto = new NumericUpDown
            {
                Width = 90, Dock = DockStyle.Right,
                DecimalPlaces = 2, Minimum = 0, Maximum = 99999,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White, ForeColor = ClrText, ThousandsSeparator = true,
                Margin = new Padding(0, 4, 4, 4)
            };
            var lblMaxComp = new Label
            {
                Text = "Máx. Compl.:", Width = 75, Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(100, 80, 50)
            };
            numMaxComp = new NumericUpDown
            {
                Width = 60, Dock = DockStyle.Left,
                DecimalPlaces = 0, Minimum = 1, Maximum = 50, Value = 1,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.White, ForeColor = ClrText,
                Margin = new Padding(0, 4, 8, 4)
            };
            // Right-to-left: numCusto, lblCusto, numValor, lblVal | left: lblMaxComp, numMaxComp
            pnlFields.Controls.Add(lblCusto);
            pnlFields.Controls.Add(numCusto);
            pnlFields.Controls.Add(lblVal);
            pnlFields.Controls.Add(numValor);
            pnlFields.Controls.Add(lblMaxComp);
            pnlFields.Controls.Add(numMaxComp);

            // Row 3: Site checkboxes
            var pnlOpcoes = new Panel { Dock = DockStyle.Top, Height = 32, BackColor = ClrFoot };
            chkHabilitarSite = new CheckBox
            {
                Text = "Habilitar no Site", Left = 0, Top = 6, AutoSize = true,
                ForeColor = ClrText, Font = new Font("Segoe UI", 9F)
            };
            chkDestaque = new CheckBox
            {
                Text = "Destaque", Left = 150, Top = 6, AutoSize = true,
                ForeColor = ClrText, Font = new Font("Segoe UI", 9F)
            };
            pnlOpcoes.Controls.Add(chkHabilitarSite);
            pnlOpcoes.Controls.Add(chkDestaque);

            // Row 4: Image picker
            var pnlImgRow = new Panel { Dock = DockStyle.Top, Height = 90, BackColor = ClrFoot };
            picMarmita = new PictureBox
            {
                Left = 0, Top = 5, Width = 72, Height = 72,
                SizeMode = PictureBoxSizeMode.Normal,
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(220, 210, 190)
            };
            picMarmita.Paint += PicMarmita_Paint;
            var btnMarmitaImagem = new Button
            {
                Text = "\U0001F5BC Imagem da Marmita",
                Left = 82, Top = 10, Width = 175, Height = 28,
                BackColor = ClrOrange, ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold)
            };
            btnMarmitaImagem.FlatAppearance.BorderSize = 0;
            btnMarmitaImagem.Click += BtnMarmitaImagem_Click;
            lblMarmitaImagem = new Label
            {
                Left = 82, Top = 44, Width = 250, AutoSize = false, Height = 18,
                Text = "Nenhuma imagem selecionada",
                ForeColor = Color.FromArgb(120, 100, 60),
                Font = new Font("Segoe UI", 8F)
            };
            pnlImgRow.Controls.Add(picMarmita);
            pnlImgRow.Controls.Add(btnMarmitaImagem);
            pnlImgRow.Controls.Add(lblMarmitaImagem);

            var pnlBtns = new Panel { Dock = DockStyle.Bottom, Height = 34 };
            btnSalvar = MkBtn("Salvar", ClrGreen, 100);
            btnSalvar.Click += BtnSalvar_Click;
            var btnCancelar = MkBtn("Cancelar", ClrBrown, 100);
            btnCancelar.Left = 108;
            btnCancelar.Click += (_, __) => { pnlForm.Visible = false; _codigoEditando = 0; };
            pnlBtns.Controls.Add(btnSalvar);
            pnlBtns.Controls.Add(btnCancelar);

            pnlForm.Height = 300;
            pnlForm.Controls.Add(pnlBtns);
            pnlForm.Controls.Add(pnlImgRow);
            pnlForm.Controls.Add(pnlOpcoes);
            pnlForm.Controls.Add(pnlFields);
            pnlForm.Controls.Add(pnlRowDesc);
            pnlForm.Controls.Add(lblFormTitulo);

            // Sub-grid: Itens da marmita selecionada
            var pnlItemsSec = new Panel { Dock = DockStyle.Fill, BackColor = ClrBg };
            var pnlItemsTool = new Panel { Dock = DockStyle.Top, Height = 36, BackColor = ClrFoot };
            var lblItemsTit = new Label
            {
                Text = "Ingredientes / Produtos da marmita selecionada",
                ForeColor = Color.FromArgb(100, 80, 50),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Dock = DockStyle.Left, Width = 310, TextAlign = ContentAlignment.MiddleLeft
            };
            btnAddItem = MkBtn("+ Adicionar Produto", ClrOrange, 160);
            btnAddItem.Dock = DockStyle.Right;
            btnAddItem.Click += BtnAddItem_Click;
            btnRemItem = MkBtn("Remover", ClrRed, 90);
            btnRemItem.Dock = DockStyle.Right;
            btnRemItem.Click += BtnRemItem_Click;
            pnlItemsTool.Controls.Add(btnRemItem);
            pnlItemsTool.Controls.Add(btnAddItem);
            pnlItemsTool.Controls.Add(lblItemsTit);

            gridItens = MkGrid();
            gridItens.Dock = DockStyle.Fill;

            pnlItemsSec.Controls.Add(gridItens);
            pnlItemsSec.Controls.Add(pnlItemsTool);

            pnlRight.Controls.Add(pnlItemsSec);
            pnlRight.Controls.Add(pnlForm);

            var pnlContent = new Panel { Dock = DockStyle.Fill };
            pnlContent.Controls.Add(pnlRight);
            pnlContent.Controls.Add(pnlGridTop);

            Controls.Add(pnlContent);
            Controls.Add(pnlTool);
            Controls.Add(pnlTop);
        }

        private Button MkBtn(string text, Color back, int width)
        {
            var b = new Button
            {
                Text = text, Width = width, Height = 28, Top = 0,
                BackColor = back, ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold)
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private DataGridView MkGrid()
        {
            var g = new DataGridView
            {
                ReadOnly = true, AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false, MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = ClrGrid1,
                DefaultCellStyle =
                {
                    BackColor = ClrGrid1, ForeColor = ClrText,
                    SelectionBackColor = ClrOrange, SelectionForeColor = Color.White
                },
                AlternatingRowsDefaultCellStyle = { BackColor = ClrGrid2 },
                ColumnHeadersDefaultCellStyle =
                {
                    BackColor = ClrGHdr, ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                },
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 32, RowTemplate = { Height = 26 },
                GridColor = Color.FromArgb(210, 195, 165),
                EnableHeadersVisualStyles = false,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9F)
            };
            g.DataError += (_, e) => e.ThrowException = false;
            return g;
        }

        // ── Dados ────────────────────────────────────────────────────────────

        private void CarregarProdutos()
        {
            _produtos.Clear();
            try
            {
                var dt = _mercBll.Listar();
                foreach (System.Data.DataRow r in dt.Rows)
                    if (r["Situacao"]?.ToString() == "A")
                        _produtos.Add((
                            Convert.ToInt32(r["Codigo"]),
                            r["Nome"]?.ToString() ?? "",
                            r["Preco"] == DBNull.Value ? 0m : Convert.ToDecimal(r["Preco"])));
            }
            catch { }
        }

        private void CarregarGrid()
        {
            try
            {
                gridMarmitas.DataSource = null;
                var dt = _bll.Listar();
                gridMarmitas.DataSource = dt;
                if (gridMarmitas.Columns.Contains("Codigo"))
                    gridMarmitas.Columns["Codigo"].Visible = false;
                if (gridMarmitas.Columns.Contains("Situacao"))
                    gridMarmitas.Columns["Situacao"].Visible = false;
                if (gridMarmitas.Columns.Contains("Descricao"))
                { gridMarmitas.Columns["Descricao"].HeaderText = "Descrição"; gridMarmitas.Columns["Descricao"].FillWeight = 55; }
                if (gridMarmitas.Columns.Contains("Valor R$"))
                { gridMarmitas.Columns["Valor R$"].HeaderText = "Valor R$"; gridMarmitas.Columns["Valor R$"].FillWeight = 18;
                  gridMarmitas.Columns["Valor R$"].DefaultCellStyle.Format = "N2"; }
                if (gridMarmitas.Columns.Contains("Custo R$"))
                { gridMarmitas.Columns["Custo R$"].HeaderText = "Custo R$"; gridMarmitas.Columns["Custo R$"].FillWeight = 18;
                  gridMarmitas.Columns["Custo R$"].DefaultCellStyle.Format = "N2"; }
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void CarregarItens(int codigoMarmita)
        {
            gridItens.DataSource = null;
            gridItens.Columns.Clear();
            if (codigoMarmita <= 0) return;
            var itens = _bll.ListarItens(codigoMarmita);
            var dt = new System.Data.DataTable();
            dt.Columns.Add("Codigo",   typeof(int));
            dt.Columns.Add("Produto",  typeof(string));
            dt.Columns.Add("Qtde",     typeof(decimal));
            foreach (var i in itens)
                dt.Rows.Add(i.Codigo, i.maritmNome, i.maritmQtde);
            gridItens.DataSource = dt;
            if (gridItens.Columns.Contains("Codigo")) gridItens.Columns["Codigo"].Visible = false;
            if (gridItens.Columns.Contains("Produto")) { gridItens.Columns["Produto"].HeaderText = "Produto"; gridItens.Columns["Produto"].FillWeight = 70; }
            if (gridItens.Columns.Contains("Qtde")) { gridItens.Columns["Qtde"].HeaderText = "Qtde"; gridItens.Columns["Qtde"].FillWeight = 20; gridItens.Columns["Qtde"].DefaultCellStyle.Format = "N2"; }
        }

        private int GetSelectedMarmitaCod()
        {
            if (gridMarmitas.SelectedRows.Count == 0) return 0;
            var val = gridMarmitas.SelectedRows[0].Cells["Codigo"]?.Value;
            return val == null || val == DBNull.Value ? 0 : Convert.ToInt32(val);
        }

        // ── Eventos ──────────────────────────────────────────────────────────

        private void GridMarmitas_SelectionChanged(object sender, EventArgs e)
            => CarregarItens(GetSelectedMarmitaCod());

        private void GridMarmitas_DoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            int cod = GetSelectedMarmitaCod();
            if (cod <= 0) return;
            var obj = _bll.PesquisaCodigo(cod);
            if (obj == null) return;
            _codigoEditando          = cod;
            txtDescricao.Text        = obj.marDescricao;
            numValor.Value           = obj.marValor > numValor.Maximum ? numValor.Maximum : obj.marValor;
            numCusto.Value           = obj.marCusto > numCusto.Maximum ? numCusto.Maximum : obj.marCusto;
            numMaxComp.Value         = obj.marMaxComplementos < 1 ? 1 : (obj.marMaxComplementos > 50 ? 50 : obj.marMaxComplementos);
            chkHabilitarSite.Checked = obj.marHabilitar_Site;
            chkDestaque.Checked      = obj.marDestaque;
            // Keep existing URL in _marmitaImagemPath so it is preserved on save
            _marmitaImagemPath = obj.marImagem_Url ?? "";
            _marmitaPreviewImg?.Dispose(); _marmitaPreviewImg = null;
            lblMarmitaImagem.Text = !string.IsNullOrWhiteSpace(_marmitaImagemPath)
                ? System.IO.Path.GetFileName(_marmitaImagemPath) : "Nenhuma imagem selecionada";
            picMarmita.Refresh();
            lblFormTitulo.Text       = "Editar Marmita";
            pnlForm.Visible          = true;
            txtDescricao.Focus();
        }

        private void BtnNova_Click(object sender, EventArgs e)
        {
            _codigoEditando = 0;
            txtDescricao.Text        = "";
            numValor.Value           = 0;
            numCusto.Value           = 0;
            numMaxComp.Value         = 1;
            chkHabilitarSite.Checked = false;
            chkDestaque.Checked      = false;
            _marmitaImagemPath       = "";
            _marmitaPreviewImg?.Dispose(); _marmitaPreviewImg = null;
            lblMarmitaImagem.Text    = "Nenhuma imagem selecionada";
            picMarmita.Refresh();
            lblFormTitulo.Text       = "Nova Marmita";
            pnlForm.Visible          = true;
            txtDescricao.Focus();
        }

        private void PicMarmita_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(picMarmita.BackColor);
            if (_marmitaPreviewImg == null) return;
            using var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, picMarmita.Width - 1, picMarmita.Height - 1);
            g.SetClip(path);
            g.DrawImage(_marmitaPreviewImg, 0, 0, picMarmita.Width, picMarmita.Height);
        }

        private void BtnMarmitaImagem_Click(object sender, EventArgs e)
        {
            using var dlgImg = new OpenFileDialog
            {
                Title  = "Selecionar imagem da marmita",
                Filter = "Imagens|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp"
            };
            if (dlgImg.ShowDialog() != DialogResult.OK) return;
            _marmitaImagemPath = dlgImg.FileName;
            try
            {
                _marmitaPreviewImg?.Dispose();
                using var tmp = System.Drawing.Image.FromFile(_marmitaImagemPath);
                _marmitaPreviewImg = new Bitmap(tmp);
                picMarmita.Refresh();
                lblMarmitaImagem.Text = System.IO.Path.GetFileName(_marmitaImagemPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar imagem: " + ex.Message);
                _marmitaImagemPath = "";
            }
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            // Only persist HTTP URLs to MySQL; local paths are handled by the Upload method
            string urlParaSalvar = _marmitaImagemPath?.StartsWith("http", StringComparison.OrdinalIgnoreCase) == true
                ? _marmitaImagemPath : "";

            var obj = new Marmita
            {
                Codigo            = _codigoEditando,
                marDescricao      = txtDescricao.Text.Trim(),
                marValor          = numValor.Value,
                marCusto          = numCusto.Value,
                marMaxComplementos = (int)numMaxComp.Value,
                marHabilitar_Site = chkHabilitarSite.Checked,
                marDestaque       = chkDestaque.Checked,
                marImagem_Url     = urlParaSalvar,
                Situacao          = 'A'
            };
            var erro = _bll.Salvar(obj);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show(erro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            pnlForm.Visible = false;
            int savedCod = _codigoEditando > 0 ? _codigoEditando : obj.Codigo;
            string imagemPath = _marmitaImagemPath;
            _codigoEditando = 0;
            _marmitaImagemPath = "";
            CarregarGrid();
            if (savedCod > 0)
                System.Threading.Tasks.Task.Run(async () =>
                {
                    if (!string.IsNullOrWhiteSpace(imagemPath))
                        await DB.SupabaseService.UploadMarmitaImagemAsync(savedCod, imagemPath);
                    await DB.SupabaseService.SincronizarMarmitaAsync(savedCod);
                    await DB.SupabaseService.SincronizarItensMarmitaAsync(savedCod);
                });
        }

        private void BtnExcluir_Click(object sender, EventArgs e)
        {
            int cod = GetSelectedMarmitaCod();
            if (cod <= 0) { MessageBox.Show("Selecione uma marmita."); return; }
            if (MessageBox.Show("Excluir esta marmita e todos os seus itens?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            var erro = _bll.Excluir(cod);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show(erro); return; }
            CarregarGrid();
            gridItens.DataSource = null;
        }

        private async void BtnSincSite_Click(object sender, EventArgs e)
        {
            int cod = GetSelectedMarmitaCod();
            if (cod <= 0) { MessageBox.Show("Selecione uma marmita para sincronizar."); return; }
            var btn = (Button)sender;
            btn.Enabled = false;
            btn.Text    = "Sincronizando...";
            try
            {
                await DB.SupabaseService.SincronizarMarmitaAsync(cod);
                await DB.SupabaseService.SincronizarItensMarmitaAsync(cod);
                MessageBox.Show("Marmita sincronizada com o site com sucesso!", "Sincronização",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro na sincronização: " + ex.Message, "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btn.Enabled = true;
                btn.Text    = "\u2601 Sincronizar Site";
            }
        }

        private void BtnAddItem_Click(object sender, EventArgs e)
        {
            int codMar = GetSelectedMarmitaCod();
            if (codMar <= 0) { MessageBox.Show("Selecione uma marmita primeiro."); return; }

            // Diálogo de seleção de produto
            using var dlg = new Form();
            dlg.Text = "Adicionar Produto à Marmita";
            dlg.StartPosition = FormStartPosition.CenterParent;
            dlg.Size = new Size(540, 440);
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox = dlg.MinimizeBox = false;
            dlg.BackColor = ClrBg;

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = ClrHeader };
            var lblTit = new Label { Text = "Selecionar Produto", ForeColor = Color.White, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            pnlTop.Controls.Add(lblTit);

            var txtF = new TextBox { Dock = DockStyle.Top, Height = 28, BackColor = Color.White, ForeColor = ClrText, Font = new Font("Segoe UI", 10F), PlaceholderText = "Filtrar por nome...", BorderStyle = BorderStyle.FixedSingle };

            var grid2 = MkGrid();
            grid2.Dock = DockStyle.Fill;
            grid2.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nome", HeaderText = "Produto", FillWeight = 70 });
            var Preencher = new Action<string>(f =>
            {
                grid2.Rows.Clear();
                foreach (var p in _produtos)
                    if (string.IsNullOrWhiteSpace(f) || p.Nome.IndexOf(f, StringComparison.OrdinalIgnoreCase) >= 0)
                        grid2.Rows.Add(p.Nome);
            });
            Preencher("");
            txtF.TextChanged += (_, __) => Preencher(txtF.Text);

            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 80, BackColor = ClrFoot, Padding = new Padding(8, 4, 8, 4) };
            var lblQ = new Label { Text = "Qtde:", Left = 8, Top = 10, AutoSize = true, ForeColor = Color.FromArgb(100, 80, 50) };
            var numQ = new NumericUpDown { Left = 60, Top = 6, Width = 80, Height = 26, Value = 1, Minimum = 0.001m, Maximum = 9999, DecimalPlaces = 3, BackColor = Color.White, ForeColor = ClrText };
            var btnOk = MkBtn("Adicionar", ClrGreen, 110);
            btnOk.Top = 42; btnOk.Left = 8;
            var btnCnc = MkBtn("Cancelar", ClrBrown, 100);
            btnCnc.Top = 42; btnCnc.Left = 125;
            btnCnc.Click += (_, __) => dlg.DialogResult = DialogResult.Cancel;
            pnlBottom.Controls.AddRange(new Control[] { lblQ, numQ, btnOk, btnCnc });

            (int Codigo, string Nome, decimal Preco) escolhido = default;
            btnOk.Click += (_, __) =>
            {
                if (grid2.CurrentRow == null) return;
                string nome = grid2.CurrentRow.Cells["Nome"].Value?.ToString() ?? "";
                escolhido = _produtos.Find(p => p.Nome == nome);
                if (string.IsNullOrEmpty(escolhido.Nome)) return;
                dlg.DialogResult = DialogResult.OK;
            };
            grid2.CellDoubleClick += (_, __) => btnOk.PerformClick();

            dlg.Controls.Add(grid2);
            dlg.Controls.Add(pnlBottom);
            dlg.Controls.Add(txtF);
            dlg.Controls.Add(pnlTop);

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            var item = new MarmitaItem
            {
                Codigo_Marmita    = codMar,
                maritmCodigo_Merc = escolhido.Codigo,
                maritmNome        = escolhido.Nome,
                maritmQtde        = numQ.Value
            };
            var erro = _bll.AdicionarItem(item);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show(erro); return; }
            CarregarItens(codMar);
            int codMarSync = codMar;
            System.Threading.Tasks.Task.Run(async () =>
                await DB.SupabaseService.SincronizarItensMarmitaAsync(codMarSync));
        }

        private void BtnRemItem_Click(object sender, EventArgs e)
        {
            if (gridItens.SelectedRows.Count == 0 || !gridItens.Columns.Contains("Codigo")) return;
            var val = gridItens.SelectedRows[0].Cells["Codigo"]?.Value;
            if (val == null || val == DBNull.Value) return;
            int cod = Convert.ToInt32(val);
            var erro = _bll.RemoverItem(cod);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show(erro); return; }
            int codMarSync = GetSelectedMarmitaCod();
            CarregarItens(codMarSync);
            System.Threading.Tasks.Task.Run(async () =>
                await DB.SupabaseService.SincronizarItensMarmitaAsync(codMarSync));
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { if (pnlForm.Visible) pnlForm.Visible = false; else Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ── Complemento methods removed — items are grouped automatically by product category ──
    }
}
