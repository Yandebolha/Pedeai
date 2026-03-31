using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmMovimentacoesDia
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlTop  = new Panel();
            lblTit  = new Label();
            pnlRes  = new Panel();
            lblRes  = new Label();
            grid    = new DataGridView();
            pnlFoot = new Panel();
            btnFech = new Button();

            // ── pnlTop ───────────────────────────────────────────────────
            pnlTop.Dock      = DockStyle.Top;
            pnlTop.Height    = 48;
            pnlTop.BackColor = System.Drawing.Color.FromArgb(36, 48, 82);
            pnlTop.SizeChanged += PnlTop_SizeChanged;

            // ── lblTit ───────────────────────────────────────────────────
            lblTit.Text      = "\U0001F4CB  Movimenta\u00e7\u00f5es do dia";
            lblTit.AutoSize  = true;
            lblTit.Top       = 14;
            lblTit.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblTit.ForeColor = System.Drawing.Color.White;
            pnlTop.Controls.Add(lblTit);

            // ── pnlRes ───────────────────────────────────────────────────
            pnlRes.Dock      = DockStyle.Top;
            pnlRes.Height    = 38;
            pnlRes.BackColor = System.Drawing.Color.FromArgb(28, 37, 65);

            // ── lblRes ───────────────────────────────────────────────────
            lblRes.Dock       = DockStyle.Fill;
            lblRes.TextAlign  = System.Drawing.ContentAlignment.MiddleLeft;
            lblRes.Padding    = new Padding(12, 0, 0, 0);
            lblRes.Font       = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblRes.ForeColor  = System.Drawing.Color.FromArgb(200, 225, 255);
            pnlRes.Controls.Add(lblRes);

            // ── grid ─────────────────────────────────────────────────────
            grid.Dock                            = DockStyle.Fill;
            grid.AutoSizeColumnsMode             = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly                        = true;
            grid.AllowUserToAddRows              = false;
            grid.SelectionMode                   = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible               = false;
            grid.BackgroundColor                 = System.Drawing.Color.FromArgb(20, 28, 55);
            grid.DefaultCellStyle.BackColor      = System.Drawing.Color.FromArgb(20, 28, 55);
            grid.DefaultCellStyle.ForeColor      = System.Drawing.Color.White;
            grid.GridColor                       = System.Drawing.Color.FromArgb(40, 55, 90);
            grid.BorderStyle                     = BorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(36, 48, 82);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            grid.Font                            = new Font("Segoe UI", 9F);
            grid.DataError   += Grid_DataError;
            grid.RowPrePaint += Grid_RowColor;

            // ── pnlFoot ──────────────────────────────────────────────────
            pnlFoot.Dock      = DockStyle.Bottom;
            pnlFoot.Height    = 40;
            pnlFoot.BackColor = System.Drawing.Color.FromArgb(28, 37, 65);
            pnlFoot.SizeChanged += PnlFoot_SizeChanged;

            // ── btnFech ──────────────────────────────────────────────────
            btnFech.Text      = "Fechar";
            btnFech.Left      = 0;
            btnFech.Top       = 8;
            btnFech.Width     = 100;
            btnFech.Height    = 26;
            btnFech.BackColor = System.Drawing.Color.FromArgb(80, 95, 130);
            btnFech.ForeColor = System.Drawing.Color.White;
            btnFech.FlatStyle = FlatStyle.Flat;
            btnFech.Cursor    = Cursors.Hand;
            btnFech.FlatAppearance.BorderSize = 0;
            btnFech.Click += BtnFech_Click;
            pnlFoot.Controls.Add(btnFech);

            // ── Form ─────────────────────────────────────────────────────
            Controls.Add(grid);
            Controls.Add(pnlRes);
            Controls.Add(pnlFoot);
            Controls.Add(pnlTop);

            Text          = "Movimenta\u00e7\u00f5es";
            BackColor     = System.Drawing.Color.FromArgb(15, 22, 45);
            ForeColor     = System.Drawing.Color.White;
            Font          = new Font("Segoe UI", 9F);
            ClientSize    = new System.Drawing.Size(900, 520);
            MinimumSize   = new System.Drawing.Size(700, 400);
            StartPosition = FormStartPosition.CenterParent;
        }

        private Panel        pnlTop;
        private Label        lblTit;
        private Panel        pnlRes;
        private Label        lblRes;
        private DataGridView grid;
        private Panel        pnlFoot;
        private Button       btnFech;
    }
}
