using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmEntradaMercadoria
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // ── Cores base ───────────────────────────────────────────────
            var corFundo   = Color.FromArgb(15, 22, 45);
            var corCard    = Color.FromArgb(28, 37, 65);
            var corTopBar  = Color.FromArgb(36, 48, 82);
            var corFiltro  = Color.FromArgb(22, 30, 55);
            var corTexto   = Color.White;
            var corAzul    = Color.FromArgb(52, 152, 219);
            var corVerde   = Color.FromArgb(39, 174, 96);
            var corVermelho= Color.FromArgb(192, 57, 43);
            var corCinza   = Color.FromArgb(80, 95, 130);
            var corGrid    = Color.FromArgb(20, 28, 55);

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1050, 680);
            this.MinimumSize = new System.Drawing.Size(900, 580);
            this.BackColor = corFundo;
            this.ForeColor = corTexto;
            this.Font = new Font("Segoe UI", 9F);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Entrada de Mercadorias";

            // ── Top bar ──────────────────────────────────────────────────
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = corTopBar };

            btnNovaEntrada = new Button
            {
                Text = "+ Nova Entrada", Left = 8, Top = 10, Width = 130, Height = 28,
                BackColor = corVerde, ForeColor = corTexto, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnNovaEntrada.FlatAppearance.BorderSize = 0;

            btnCancelarSel = new Button
            {
                Text = "\u274C Cancelar Entrada", Left = 148, Top = 10, Width = 140, Height = 28,
                BackColor = corVermelho, ForeColor = corTexto, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnCancelarSel.FlatAppearance.BorderSize = 0;

            var lblTitulo = new Label
            {
                Text = "\U0001F4E6  Entrada de Mercadorias", AutoSize = true,
                Top = 14, Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = corTexto
            };
            pnlTop.Controls.AddRange(new Control[] { btnNovaEntrada, btnCancelarSel, lblTitulo });
            pnlTop.SizeChanged += (_, __) => lblTitulo.Left = (pnlTop.Width - lblTitulo.Width) / 2;

            // ── Barra de filtros ─────────────────────────────────────────
            var pnlFiltro = new Panel { Dock = DockStyle.Top, Height = 42, BackColor = corFiltro };
            var lblDe  = new Label { Text = "De:",  Left = 8,   Top = 12, AutoSize = true };
            dtpDe  = new DateTimePicker { Left = 32,  Top = 8,  Width = 120, Format = DateTimePickerFormat.Short };
            var lblAte = new Label { Text = "Até:", Left = 160, Top = 12, AutoSize = true };
            dtpAte = new DateTimePicker { Left = 185, Top = 8,  Width = 120, Format = DateTimePickerFormat.Short };

            btnFiltrar = new Button
            {
                Text = "Filtrar", Left = 314, Top = 9, Width = 80, Height = 26,
                BackColor = corAzul, ForeColor = corTexto, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btnFiltrar.FlatAppearance.BorderSize = 0;

            btnLimparFiltro = new Button
            {
                Text = "Limpar", Left = 401, Top = 9, Width = 80, Height = 26,
                BackColor = corCinza, ForeColor = corTexto, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btnLimparFiltro.FlatAppearance.BorderSize = 0;

            pnlFiltro.Controls.AddRange(new Control[] { lblDe, dtpDe, lblAte, dtpAte, btnFiltrar, btnLimparFiltro });

            // ── Grid de entradas ─────────────────────────────────────────
            gridEntradas = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true, AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false, BackgroundColor = corGrid,
                DefaultCellStyle  = { BackColor = corGrid, ForeColor = corTexto },
                GridColor = Color.FromArgb(40, 55, 90),
                Font = new Font("Segoe UI", 9F), BorderStyle = BorderStyle.None,
                ColumnHeadersDefaultCellStyle = { BackColor = corTopBar, ForeColor = corTexto },
            };
            gridEntradas.DataError += (_, e) => e.ThrowException = false;

            // ── Painel Nova Entrada (Bottom) ──────────────────────────────
            pnlNovaEntrada = new Panel
            {
                Dock = DockStyle.Bottom, Height = 470,
                BackColor = corCard, Visible = false
            };

            // Título do painel
            var pnlNovaTop = new Panel { Dock = DockStyle.Top, Height = 38, BackColor = corTopBar };
            lblNovaEntradaTitulo = new Label
            {
                Text = "  Nova Entrada de Mercadorias", Dock = DockStyle.Left,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = corTexto,
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlNovaTop.Controls.Add(lblNovaEntradaTitulo);

            // ── Campos cabeçalho ──────────────────────────────────────────
            var pnlCabecalho = new Panel { Dock = DockStyle.Top, Height = 36 };
            pnlCabecalho.BackColor = corCard;

            var lblForn    = new Label { Text = "Fornecedor:",    Left = 8,   Top = 10, AutoSize = true, ForeColor = corTexto };
            txtFornCod     = new TextBox { Left = 87, Top = 7, Width = 55, PlaceholderText = "Cód." };
            txtFornNome    = new TextBox { Left = 148, Top = 7, Width = 200, ReadOnly = true, BackColor = Color.FromArgb(12, 18, 40), ForeColor = corTexto };

            var lblData    = new Label { Text = "Data:",          Left = 360, Top = 10, AutoSize = true, ForeColor = corTexto };
            dtpData        = new DateTimePicker { Left = 396, Top = 7, Width = 120, Format = DateTimePickerFormat.Short };

            var lblNumDoc  = new Label { Text = "Nº Documento:",  Left = 526, Top = 10, AutoSize = true, ForeColor = corTexto };
            txtNumDoc      = new TextBox { Left = 625, Top = 7, Width = 120 };

            pnlCabecalho.Controls.AddRange(new Control[] { lblForn, txtFornCod, txtFornNome, lblData, dtpData, lblNumDoc, txtNumDoc });

            var pnlObs = new Panel { Dock = DockStyle.Top, Height = 32 };
            pnlObs.BackColor = corCard;
            var lblObs = new Label { Text = "Observações:",      Left = 8,   Top = 8, AutoSize = true, ForeColor = corTexto };
            txtObservacoes = new TextBox { Left = 90, Top = 5, Width = 660, Height = 22 };
            pnlObs.Controls.AddRange(new Control[] { lblObs, txtObservacoes });

            // ── Linha de adição de item ───────────────────────────────────
            var pnlAddItem = new Panel { Dock = DockStyle.Top, Height = 36 };
            pnlAddItem.BackColor = Color.FromArgb(22, 30, 55);

            var lblProd    = new Label { Text = "Produto:",       Left = 8,   Top = 10, AutoSize = true, ForeColor = corTexto };
            txtProdCod     = new TextBox { Left = 65,  Top = 7, Width = 55,  PlaceholderText = "Cód." };
            txtProdNome    = new TextBox { Left = 126, Top = 7, Width = 230, PlaceholderText = "Nome do produto..." };

            var lblQtde    = new Label { Text = "Qtde:",          Left = 364, Top = 10, AutoSize = true, ForeColor = corTexto };
            numQtde        = new NumericUpDown { Left = 395, Top = 7, Width = 75, DecimalPlaces = 3, Minimum = 0.001m, Maximum = 99999, Value = 1 };

            var lblCustoIt = new Label { Text = "Custo R$:",      Left = 478, Top = 10, AutoSize = true, ForeColor = corTexto };
            numCustoItem   = new NumericUpDown { Left = 538, Top = 7, Width = 90, DecimalPlaces = 4, Minimum = 0, Maximum = 99999 };

            chkAtualizarCusto = new CheckBox
            {
                Text = "Atualizar custo", Left = 636, Top = 9, AutoSize = true,
                ForeColor = corTexto, Checked = true
            };

            btnAdicionarItem = new Button
            {
                Text = "+ Adicionar", Left = 750, Top = 7, Width = 95, Height = 26,
                BackColor = corVerde, ForeColor = corTexto, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnAdicionarItem.FlatAppearance.BorderSize = 0;

            pnlAddItem.Controls.AddRange(new Control[] {
                lblProd, txtProdCod, txtProdNome,
                lblQtde, numQtde, lblCustoIt, numCustoItem,
                chkAtualizarCusto, btnAdicionarItem
            });

            // ── Grid de itens ─────────────────────────────────────────────
            gridItens = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true, AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false, BackgroundColor = corGrid,
                DefaultCellStyle  = { BackColor = corGrid, ForeColor = corTexto },
                GridColor = Color.FromArgb(40, 55, 90),
                Font = new Font("Segoe UI", 9F), BorderStyle = BorderStyle.None,
                ColumnHeadersDefaultCellStyle = { BackColor = corTopBar, ForeColor = corTexto },
            };
            gridItens.DataError += (_, e) => e.ThrowException = false;

            // ── Painel de Parcelas ──────────────────────────────────────
            var pnlParcelasOuter = new Panel { Dock = DockStyle.Bottom, Height = 100, BackColor = corCard };

            var pnlParcelasTop = new Panel { Dock = DockStyle.Top, Height = 34, BackColor = Color.FromArgb(22, 30, 55) };
            var lblParcTitulo = new Label
            {
                Text = "  🗓  Parcelas de Pagamento (opcional)",
                Dock = DockStyle.Left, Width = 280,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(200, 220, 255),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblNumParc = new Label { Text = "Qtde parcelas:", Left = 290, Top = 9, AutoSize = true, ForeColor = corTexto };
            numParcelas = new NumericUpDown { Left = 388, Top = 6, Width = 55, Minimum = 0, Maximum = 36, Value = 0 };

            var lblPrimVcto = new Label { Text = "1º vencimento:", Left = 450, Top = 9, AutoSize = true, ForeColor = corTexto };
            dtpPrimVencimento = new DateTimePicker { Left = 540, Top = 6, Width = 115, Format = DateTimePickerFormat.Short };

            btnGerarParcelas = new Button
            {
                Text = "↻ Gerar", Left = 665, Top = 6, Width = 75, Height = 24,
                BackColor = Color.FromArgb(52, 152, 219), ForeColor = corTexto, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnGerarParcelas.FlatAppearance.BorderSize = 0;
            pnlParcelasTop.Controls.Add(lblParcTitulo);
            pnlParcelasTop.Controls.AddRange(new Control[] { lblNumParc, numParcelas, lblPrimVcto, dtpPrimVencimento, btnGerarParcelas });

            gridParcelas = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false, BackgroundColor = corGrid,
                DefaultCellStyle  = { BackColor = corGrid, ForeColor = corTexto },
                GridColor = Color.FromArgb(40, 55, 90),
                Font = new Font("Segoe UI", 9F), BorderStyle = BorderStyle.None,
                ColumnHeadersDefaultCellStyle = { BackColor = corTopBar, ForeColor = corTexto },
            };
            gridParcelas.DataError += (_, e) => e.ThrowException = false;

            pnlParcelasOuter.Controls.Add(gridParcelas);
            pnlParcelasOuter.Controls.Add(pnlParcelasTop);

            // ── Rodapé do painel ─────────────────────────────────────────
            var pnlRodape = new Panel { Dock = DockStyle.Bottom, Height = 46, BackColor = corCard };

            lblTotal = new Label
            {
                Text = "Total: R$ 0,00", Left = 8, Top = 12,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(39, 174, 96),
                AutoSize = true
            };

            btnRemoverItem = new Button
            {
                Text = "\u2212 Remover Item", Left = 300, Top = 10, Width = 120, Height = 28,
                BackColor = corVermelho, ForeColor = corTexto, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnRemoverItem.FlatAppearance.BorderSize = 0;

            btnConfirmarEntrada = new Button
            {
                Text = "\u2714 Confirmar Entrada", Left = 0, Top = 10, Width = 160, Height = 28,
                BackColor = corVerde, ForeColor = corTexto, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnConfirmarEntrada.FlatAppearance.BorderSize = 0;

            btnFecharForm = new Button
            {
                Text = "Cancelar", Left = 0, Top = 10, Width = 100, Height = 28,
                BackColor = corCinza, ForeColor = corTexto, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnFecharForm.FlatAppearance.BorderSize = 0;

            void AlinhaBotoesRodape()
            {
                btnFecharForm.Left       = pnlRodape.Width - btnFecharForm.Width - 10;
                btnConfirmarEntrada.Left = btnFecharForm.Left - btnConfirmarEntrada.Width - 8;
                btnRemoverItem.Left      = btnConfirmarEntrada.Left - btnRemoverItem.Width - 20;
            }
            pnlRodape.SizeChanged += (_, __) => AlinhaBotoesRodape();
            pnlRodape.Controls.AddRange(new Control[] { lblTotal, btnRemoverItem, btnConfirmarEntrada, btnFecharForm });

            // ── Montar pnlNovaEntrada ─────────────────────────────────────
            pnlNovaEntrada.Controls.Add(gridItens);            pnlNovaEntrada.Controls.Add(pnlParcelasOuter);            pnlNovaEntrada.Controls.Add(pnlRodape);
            pnlNovaEntrada.Controls.Add(pnlAddItem);
            pnlNovaEntrada.Controls.Add(pnlObs);
            pnlNovaEntrada.Controls.Add(pnlCabecalho);
            pnlNovaEntrada.Controls.Add(pnlNovaTop);

            // ── Montar form ───────────────────────────────────────────────
            Controls.Add(gridEntradas);
            Controls.Add(pnlFiltro);
            Controls.Add(pnlTop);
            Controls.Add(pnlNovaEntrada);

            this.ResumeLayout(false);
        }

        // ── Controls ─────────────────────────────────────────────────────
        internal DataGridView    gridEntradas;
        internal DataGridView    gridItens;
        internal DataGridView    gridParcelas;
        internal Panel           pnlNovaEntrada;
        internal Label           lblNovaEntradaTitulo;
        internal Label           lblTotal;
        internal NumericUpDown   numParcelas;
        internal DateTimePicker  dtpPrimVencimento;
        internal Button          btnGerarParcelas;

        // filtros
        internal DateTimePicker  dtpDe;
        internal DateTimePicker  dtpAte;
        internal Button          btnFiltrar;
        internal Button          btnLimparFiltro;

        // header buttons
        internal Button          btnNovaEntrada;
        internal Button          btnCancelarSel;

        // cabeçalho nova entrada
        internal TextBox         txtFornCod;
        internal TextBox         txtFornNome;
        internal DateTimePicker  dtpData;
        internal TextBox         txtNumDoc;
        internal TextBox         txtObservacoes;

        // item
        internal TextBox         txtProdCod;
        internal TextBox         txtProdNome;
        internal NumericUpDown   numQtde;
        internal NumericUpDown   numCustoItem;
        internal CheckBox        chkAtualizarCusto;
        internal Button          btnAdicionarItem;
        internal Button          btnRemoverItem;

        // confirmar
        internal Button          btnConfirmarEntrada;
        internal Button          btnFecharForm;
    }
}
