using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmRelatorioFinanceiro
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // ── Allocate controls ───────────────────────────────────────────
            pnlTop       = new System.Windows.Forms.Panel();
            lblTitPage   = new System.Windows.Forms.Label();
            pnlFiltro    = new System.Windows.Forms.Panel();
            lblTipoFil   = new System.Windows.Forms.Label();
            cmbTipo      = new System.Windows.Forms.ComboBox();
            lblDeFil     = new System.Windows.Forms.Label();
            dtpDe        = new System.Windows.Forms.DateTimePicker();
            lblAteFil    = new System.Windows.Forms.Label();
            dtpAte       = new System.Windows.Forms.DateTimePicker();
            btnFiltrar   = new System.Windows.Forms.Button();
            btnImprimir  = new System.Windows.Forms.Button();
            btnExportCsv = new System.Windows.Forms.Button();
            grid         = new System.Windows.Forms.DataGridView();
            pnlBottom    = new System.Windows.Forms.Panel();
            lblTotal     = new System.Windows.Forms.Label();

            // ── pnlTop ──────────────────────────────────────────────────────
            pnlTop.Dock      = System.Windows.Forms.DockStyle.Top;
            pnlTop.Height    = 44;
            pnlTop.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            pnlTop.Controls.Add(lblTitPage);

            // ── lblTitPage ──────────────────────────────────────────────────
            lblTitPage.Text      = "Vendas por Per\u00edodo";
            lblTitPage.Dock      = System.Windows.Forms.DockStyle.Fill;
            lblTitPage.ForeColor = System.Drawing.Color.White;
            lblTitPage.Font      = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            lblTitPage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lblTitPage.Padding   = new System.Windows.Forms.Padding(12, 0, 0, 0);

            // ── pnlFiltro ───────────────────────────────────────────────────
            pnlFiltro.Dock      = System.Windows.Forms.DockStyle.Top;
            pnlFiltro.Height    = 46;
            pnlFiltro.BackColor = System.Drawing.Color.FromArgb(235, 228, 214);

            // ── lblTipoFil ──────────────────────────────────────────────────
            lblTipoFil.Text      = "Tipo:";
            lblTipoFil.Left      = 8;
            lblTipoFil.Top       = 14;
            lblTipoFil.AutoSize  = true;
            lblTipoFil.ForeColor = System.Drawing.Color.FromArgb(50, 40, 25);
            lblTipoFil.Font      = new System.Drawing.Font("Segoe UI", 8.5F);

            // ── cmbTipo ─────────────────────────────────────────────────────
            cmbTipo.Left          = 44;
            cmbTipo.Top           = 10;
            cmbTipo.Width         = 168;
            cmbTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbTipo.BackColor     = System.Drawing.Color.FromArgb(248, 245, 240);
            cmbTipo.ForeColor     = System.Drawing.Color.FromArgb(50, 40, 25);
            cmbTipo.FlatStyle     = System.Windows.Forms.FlatStyle.Flat;
            cmbTipo.Items.AddRange(new object[] { "Ambos", "Sa\u00edda (Vendas)", "Entrada (Compras/Gastos)" });
            cmbTipo.SelectedIndex = 0;

            // ── lblDeFil ────────────────────────────────────────────────────
            lblDeFil.Text      = "De:";
            lblDeFil.Left      = 222;
            lblDeFil.Top       = 14;
            lblDeFil.AutoSize  = true;
            lblDeFil.ForeColor = System.Drawing.Color.FromArgb(50, 40, 25);
            lblDeFil.Font      = new System.Drawing.Font("Segoe UI", 8.5F);

            // ── dtpDe ───────────────────────────────────────────────────────
            dtpDe.Left   = 248;
            dtpDe.Top    = 10;
            dtpDe.Width  = 120;
            dtpDe.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            dtpDe.Value  = System.DateTime.Today.AddMonths(-1);

            // ── lblAteFil ───────────────────────────────────────────────────
            lblAteFil.Text      = "At\u00e9:";
            lblAteFil.Left      = 378;
            lblAteFil.Top       = 14;
            lblAteFil.AutoSize  = true;
            lblAteFil.ForeColor = System.Drawing.Color.FromArgb(50, 40, 25);
            lblAteFil.Font      = new System.Drawing.Font("Segoe UI", 8.5F);

            // ── dtpAte ──────────────────────────────────────────────────────
            dtpAte.Left   = 408;
            dtpAte.Top    = 10;
            dtpAte.Width  = 120;
            dtpAte.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            dtpAte.Value  = System.DateTime.Today;

            // ── btnFiltrar ──────────────────────────────────────────────────
            btnFiltrar.Text      = "Filtrar";
            btnFiltrar.Left      = 538;
            btnFiltrar.Top       = 7;
            btnFiltrar.Width     = 80;
            btnFiltrar.Height    = 28;
            btnFiltrar.BackColor = System.Drawing.Color.FromArgb(224, 113, 42);
            btnFiltrar.ForeColor = System.Drawing.Color.White;
            btnFiltrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnFiltrar.Cursor    = System.Windows.Forms.Cursors.Hand;
            btnFiltrar.FlatAppearance.BorderSize = 0;
            btnFiltrar.Click += new System.EventHandler(this.BtnFiltrar_Click);

            // ── btnImprimir ─────────────────────────────────────────────────
            btnImprimir.Text      = "\uD83D\uDDA8 Imprimir";
            btnImprimir.Left      = 626;
            btnImprimir.Top       = 7;
            btnImprimir.Width     = 100;
            btnImprimir.Height    = 28;
            btnImprimir.BackColor = System.Drawing.Color.FromArgb(52, 100, 160);
            btnImprimir.ForeColor = System.Drawing.Color.White;
            btnImprimir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnImprimir.Cursor    = System.Windows.Forms.Cursors.Hand;
            btnImprimir.FlatAppearance.BorderSize = 0;
            btnImprimir.Click += new System.EventHandler(this.BtnImprimir_Click);

            // ── btnExportCsv ────────────────────────────────────────────────
            btnExportCsv.Text      = "Exportar CSV";
            btnExportCsv.Left      = 734;
            btnExportCsv.Top       = 7;
            btnExportCsv.Width     = 114;
            btnExportCsv.Height    = 28;
            btnExportCsv.BackColor = System.Drawing.Color.FromArgb(87, 120, 38);
            btnExportCsv.ForeColor = System.Drawing.Color.White;
            btnExportCsv.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnExportCsv.Cursor    = System.Windows.Forms.Cursors.Hand;
            btnExportCsv.FlatAppearance.BorderSize = 0;
            btnExportCsv.Click += new System.EventHandler(this.BtnExportCsv_Click);

            // Add filter controls
            pnlFiltro.Controls.Add(lblTipoFil);
            pnlFiltro.Controls.Add(cmbTipo);
            pnlFiltro.Controls.Add(lblDeFil);
            pnlFiltro.Controls.Add(dtpDe);
            pnlFiltro.Controls.Add(lblAteFil);
            pnlFiltro.Controls.Add(dtpAte);
            pnlFiltro.Controls.Add(btnFiltrar);
            pnlFiltro.Controls.Add(btnImprimir);
            pnlFiltro.Controls.Add(btnExportCsv);

            // ── grid ────────────────────────────────────────────────────────
            grid.Dock                              = System.Windows.Forms.DockStyle.Fill;
            grid.ReadOnly                          = true;
            grid.AllowUserToAddRows                = false;
            grid.RowHeadersVisible                 = false;
            grid.SelectionMode                     = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode               = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor                   = System.Drawing.Color.FromArgb(248, 245, 240);
            grid.GridColor                         = System.Drawing.Color.FromArgb(210, 200, 180);
            grid.EnableHeadersVisualStyles         = false;
            grid.BorderStyle                       = System.Windows.Forms.BorderStyle.None;
            grid.Font                              = new System.Drawing.Font("Segoe UI", 9F);
            grid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font      = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            grid.DefaultCellStyle.BackColor              = System.Drawing.Color.FromArgb(250, 246, 238);
            grid.DefaultCellStyle.ForeColor              = System.Drawing.Color.FromArgb(50, 40, 25);
            grid.DefaultCellStyle.SelectionBackColor     = System.Drawing.Color.FromArgb(224, 113, 42);
            grid.DefaultCellStyle.SelectionForeColor     = System.Drawing.Color.White;
            grid.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(240, 234, 218);
            grid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Grid_CellDoubleClick);
            grid.DataError       += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);

            // ── pnlBottom ───────────────────────────────────────────────────
            pnlBottom.Dock      = System.Windows.Forms.DockStyle.Bottom;
            pnlBottom.Height    = 32;
            pnlBottom.BackColor = System.Drawing.Color.FromArgb(235, 228, 214);
            pnlBottom.Controls.Add(lblTotal);

            // ── lblTotal ────────────────────────────────────────────────────
            lblTotal.Dock      = System.Windows.Forms.DockStyle.Fill;
            lblTotal.ForeColor = System.Drawing.Color.FromArgb(130, 80, 20);
            lblTotal.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lblTotal.Padding   = new System.Windows.Forms.Padding(8, 0, 0, 0);
            lblTotal.Text      = "";

            // ── Form ────────────────────────────────────────────────────────
            Controls.Add(grid);
            Controls.Add(pnlBottom);
            Controls.Add(pnlFiltro);
            Controls.Add(pnlTop);

            Text             = "Vendas por Per\u00edodo";
            StartPosition    = System.Windows.Forms.FormStartPosition.CenterParent;
            Size             = new System.Drawing.Size(900, 620);
            MinimumSize      = new System.Drawing.Size(750, 480);
            BackColor        = System.Drawing.Color.FromArgb(248, 245, 240);
            Font             = new System.Drawing.Font("Segoe UI", 9F);
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
        }

        // ── Field declarations (no inline initializers) ────────────────────
        private System.Windows.Forms.Panel            pnlTop;
        private System.Windows.Forms.Label            lblTitPage;
        private System.Windows.Forms.Panel            pnlFiltro;
        private System.Windows.Forms.Label            lblTipoFil;
        internal System.Windows.Forms.ComboBox        cmbTipo;
        private System.Windows.Forms.Label            lblDeFil;
        internal System.Windows.Forms.DateTimePicker  dtpDe;
        private System.Windows.Forms.Label            lblAteFil;
        internal System.Windows.Forms.DateTimePicker  dtpAte;
        internal System.Windows.Forms.Button          btnFiltrar;
        internal System.Windows.Forms.Button          btnImprimir;
        internal System.Windows.Forms.Button          btnExportCsv;
        internal System.Windows.Forms.DataGridView    grid;
        private System.Windows.Forms.Panel            pnlBottom;
        internal System.Windows.Forms.Label           lblTotal;
    }
}
