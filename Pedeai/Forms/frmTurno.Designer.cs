namespace Pedeai.Forms
{
    partial class frmTurno
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblStatus      = new System.Windows.Forms.Label();
            this.lblCaixaIni    = new System.Windows.Forms.Label();
            this.pnlAbrir       = new System.Windows.Forms.Panel();
            this.lblAbrir       = new System.Windows.Forms.Label();
            this.lblCaixaIniLabel = new System.Windows.Forms.Label();
            this.numCaixaInicial  = new System.Windows.Forms.NumericUpDown();
            this.lblObsAbrir    = new System.Windows.Forms.Label();
            this.txtObsAbrir    = new System.Windows.Forms.TextBox();
            this.btnAbrir       = new System.Windows.Forms.Button();
            this.pnlFechar      = new System.Windows.Forms.Panel();
            this.lblFechar      = new System.Windows.Forms.Label();
            this.lblCaixaFinLabel = new System.Windows.Forms.Label();
            this.numCaixaFinal  = new System.Windows.Forms.NumericUpDown();
            this.lblObsFechar   = new System.Windows.Forms.Label();
            this.txtObsFechar   = new System.Windows.Forms.TextBox();
            this.btnFechar      = new System.Windows.Forms.Button();
            this.pnlHistorico   = new System.Windows.Forms.Panel();
            this.lblHist        = new System.Windows.Forms.Label();
            this.pnlFiltro      = new System.Windows.Forms.Panel();
            this.lblDe          = new System.Windows.Forms.Label();
            this.dtpDe          = new System.Windows.Forms.DateTimePicker();
            this.lblAte         = new System.Windows.Forms.Label();
            this.dtpAte         = new System.Windows.Forms.DateTimePicker();
            this.btnFiltrar     = new System.Windows.Forms.Button();
            this.gridHistorico  = new System.Windows.Forms.DataGridView();
            this.btnRelatorio   = new System.Windows.Forms.Button();

            // pnlAbrir
            this.pnlAbrir.SuspendLayout();
            // numCaixaInicial
            ((System.ComponentModel.ISupportInitialize)(this.numCaixaInicial)).BeginInit();
            // pnlFechar
            this.pnlFechar.SuspendLayout();
            // numCaixaFinal
            ((System.ComponentModel.ISupportInitialize)(this.numCaixaFinal)).BeginInit();
            // pnlHistorico
            this.pnlHistorico.SuspendLayout();
            // pnlFiltro
            this.pnlFiltro.SuspendLayout();
            // gridHistorico
            ((System.ComponentModel.ISupportInitialize)(this.gridHistorico)).BeginInit();
            this.SuspendLayout();

            // ── lblStatus ──────────────────────────────────────────────────
            this.lblStatus.Dock        = System.Windows.Forms.DockStyle.Top;
            this.lblStatus.Height      = 36;
            this.lblStatus.Font        = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor   = System.Drawing.Color.White;
            this.lblStatus.BackColor   = System.Drawing.Color.FromArgb(192, 57, 43);
            this.lblStatus.Text        = "NENHUM TURNO ABERTO";
            this.lblStatus.TextAlign   = System.Drawing.ContentAlignment.MiddleCenter;

            // ── lblCaixaIni ────────────────────────────────────────────────
            this.lblCaixaIni.Dock      = System.Windows.Forms.DockStyle.Top;
            this.lblCaixaIni.Height    = 24;
            this.lblCaixaIni.ForeColor = System.Drawing.Color.FromArgb(220, 230, 255);
            this.lblCaixaIni.Font      = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCaixaIni.Text      = "";
            this.lblCaixaIni.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCaixaIni.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);

            // ── pnlAbrir ───────────────────────────────────────────────────
            this.pnlAbrir.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlAbrir.Height    = 140;
            this.pnlAbrir.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.pnlAbrir.Padding   = new System.Windows.Forms.Padding(12, 8, 12, 8);

            this.lblAbrir.Text      = "Abrir Turno";
            this.lblAbrir.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblAbrir.ForeColor = System.Drawing.Color.FromArgb(87, 120, 38);
            this.lblAbrir.Left = 12; this.lblAbrir.Top = 8; this.lblAbrir.AutoSize = true;

            this.lblCaixaIniLabel.Text      = "Caixa inicial (R$):";
            this.lblCaixaIniLabel.ForeColor = System.Drawing.Color.White;
            this.lblCaixaIniLabel.Left = 12; this.lblCaixaIniLabel.Top = 36; this.lblCaixaIniLabel.AutoSize = true;

            this.numCaixaInicial.Left            = 160; this.numCaixaInicial.Top = 32;
            this.numCaixaInicial.Width           = 130;
            this.numCaixaInicial.DecimalPlaces   = 2;
            this.numCaixaInicial.Maximum         = 99999M;
            this.numCaixaInicial.Value           = 0;

            this.lblObsAbrir.Text      = "Observa\u00e7\u00e3o:";
            this.lblObsAbrir.ForeColor = System.Drawing.Color.White;
            this.lblObsAbrir.Left = 12; this.lblObsAbrir.Top = 72; this.lblObsAbrir.AutoSize = true;

            this.txtObsAbrir.Left  = 160; this.txtObsAbrir.Top = 68;
            this.txtObsAbrir.Width = 260;

            this.btnAbrir.Text      = "\u25B6 Abrir Turno";
            this.btnAbrir.Left      = 12; this.btnAbrir.Top = 100;
            this.btnAbrir.Width     = 140; this.btnAbrir.Height = 30;
            this.btnAbrir.BackColor = System.Drawing.Color.FromArgb(87, 120, 38);
            this.btnAbrir.ForeColor = System.Drawing.Color.White;
            this.btnAbrir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbrir.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnAbrir.Click    += new System.EventHandler(this.BtnAbrir_Click);
            this.btnAbrir.FlatAppearance.BorderSize = 0;

            this.pnlAbrir.Controls.Add(this.lblAbrir);
            this.pnlAbrir.Controls.Add(this.lblCaixaIniLabel);
            this.pnlAbrir.Controls.Add(this.numCaixaInicial);
            this.pnlAbrir.Controls.Add(this.lblObsAbrir);
            this.pnlAbrir.Controls.Add(this.txtObsAbrir);
            this.pnlAbrir.Controls.Add(this.btnAbrir);

            // ── pnlFechar ──────────────────────────────────────────────────
            this.pnlFechar.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlFechar.Height    = 140;
            this.pnlFechar.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.pnlFechar.Padding   = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.pnlFechar.Visible   = false;

            this.lblFechar.Text      = "Fechar Turno";
            this.lblFechar.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblFechar.ForeColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.lblFechar.Left = 12; this.lblFechar.Top = 8; this.lblFechar.AutoSize = true;

            this.lblCaixaFinLabel.Text      = "Caixa final (R$):";
            this.lblCaixaFinLabel.ForeColor = System.Drawing.Color.White;
            this.lblCaixaFinLabel.Left = 12; this.lblCaixaFinLabel.Top = 36; this.lblCaixaFinLabel.AutoSize = true;

            this.numCaixaFinal.Left          = 160; this.numCaixaFinal.Top = 32;
            this.numCaixaFinal.Width         = 130;
            this.numCaixaFinal.DecimalPlaces = 2;
            this.numCaixaFinal.Maximum       = 99999M;
            this.numCaixaFinal.Value         = 0;

            this.lblObsFechar.Text      = "Observa\u00e7\u00e3o:";
            this.lblObsFechar.ForeColor = System.Drawing.Color.White;
            this.lblObsFechar.Left = 12; this.lblObsFechar.Top = 72; this.lblObsFechar.AutoSize = true;

            this.txtObsFechar.Left  = 160; this.txtObsFechar.Top = 68;
            this.txtObsFechar.Width = 260;

            this.btnFechar.Text      = "\u25A0 Fechar Turno";
            this.btnFechar.Left      = 12; this.btnFechar.Top = 100;
            this.btnFechar.Width     = 140; this.btnFechar.Height = 30;
            this.btnFechar.BackColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.btnFechar.ForeColor = System.Drawing.Color.White;
            this.btnFechar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFechar.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnFechar.Click    += new System.EventHandler(this.BtnFechar_Click);
            this.btnFechar.FlatAppearance.BorderSize = 0;

            this.pnlFechar.Controls.Add(this.lblFechar);
            this.pnlFechar.Controls.Add(this.lblCaixaFinLabel);
            this.pnlFechar.Controls.Add(this.numCaixaFinal);
            this.pnlFechar.Controls.Add(this.lblObsFechar);
            this.pnlFechar.Controls.Add(this.txtObsFechar);
            this.pnlFechar.Controls.Add(this.btnFechar);

            // ── pnlHistorico ───────────────────────────────────────────────
            this.pnlHistorico.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.pnlHistorico.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);

            this.lblHist.Text      = "Hist\u00f3rico de Turnos";
            this.lblHist.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblHist.ForeColor = System.Drawing.Color.FromArgb(243, 156, 18);
            this.lblHist.Dock      = System.Windows.Forms.DockStyle.Top;
            this.lblHist.Height    = 28;
            this.lblHist.Padding   = new System.Windows.Forms.Padding(4, 6, 0, 0);
            this.lblHist.BackColor = System.Drawing.Color.Transparent;

            // pnlFiltro (inside pnlHistorico)
            this.pnlFiltro.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltro.Height    = 40;
            this.pnlFiltro.BackColor = System.Drawing.Color.Transparent;

            this.lblDe.Text      = "De:";
            this.lblDe.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.lblDe.Left = 4; this.lblDe.Top = 12; this.lblDe.AutoSize = true;

            this.dtpDe.Left   = 30; this.dtpDe.Top = 8; this.dtpDe.Width = 110;
            this.dtpDe.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDe.Value  = System.DateTime.Today.AddDays(-30);

            this.lblAte.Text      = "At\u00e9:";
            this.lblAte.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.lblAte.Left = 152; this.lblAte.Top = 12; this.lblAte.AutoSize = true;

            this.dtpAte.Left   = 182; this.dtpAte.Top = 8; this.dtpAte.Width = 110;
            this.dtpAte.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpAte.Value  = System.DateTime.Today;

            this.btnFiltrar.Text      = "Filtrar";
            this.btnFiltrar.Left      = 304; this.btnFiltrar.Top = 6;
            this.btnFiltrar.Width     = 80; this.btnFiltrar.Height = 26;
            this.btnFiltrar.BackColor = System.Drawing.Color.FromArgb(224, 113, 42);
            this.btnFiltrar.ForeColor = System.Drawing.Color.White;
            this.btnFiltrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFiltrar.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnFiltrar.Click    += new System.EventHandler(this.BtnFiltrar_Click);
            this.btnFiltrar.FlatAppearance.BorderSize = 0;

            this.pnlFiltro.Controls.Add(this.lblDe);
            this.pnlFiltro.Controls.Add(this.dtpDe);
            this.pnlFiltro.Controls.Add(this.lblAte);
            this.pnlFiltro.Controls.Add(this.dtpAte);
            this.pnlFiltro.Controls.Add(this.btnFiltrar);

            // gridHistorico
            this.gridHistorico.Dock                  = System.Windows.Forms.DockStyle.Fill;
            this.gridHistorico.BackgroundColor        = System.Drawing.Color.FromArgb(250, 245, 238);
            this.gridHistorico.ForeColor              = System.Drawing.Color.FromArgb(50, 50, 50);
            this.gridHistorico.GridColor              = System.Drawing.Color.FromArgb(200, 185, 160);
            this.gridHistorico.CellBorderStyle        = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.gridHistorico.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridHistorico.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(240, 230, 202);
            this.gridHistorico.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.gridHistorico.DefaultCellStyle.BackColor  = System.Drawing.Color.White;
            this.gridHistorico.DefaultCellStyle.ForeColor  = System.Drawing.Color.FromArgb(50, 50, 50);
            this.gridHistorico.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(224, 113, 42);
            this.gridHistorico.AutoSizeColumnsMode    = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridHistorico.ReadOnly               = true;
            this.gridHistorico.SelectionMode          = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridHistorico.AllowUserToAddRows     = false;
            this.gridHistorico.RowHeadersVisible      = false;
            this.gridHistorico.EnableHeadersVisualStyles = false;

            // ── btnRelatorio ─────────────────────────────────────────────
            this.btnRelatorio.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.btnRelatorio.Height    = 32;
            this.btnRelatorio.Text      = "\U0001F4CA Ver Movimenta\u00e7\u00f5es do Turno";
            this.btnRelatorio.BackColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.btnRelatorio.ForeColor = System.Drawing.Color.White;
            this.btnRelatorio.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRelatorio.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnRelatorio.Click    += new System.EventHandler(this.BtnRelatorio_Click);
            this.btnRelatorio.FlatAppearance.BorderSize = 0;

            this.pnlHistorico.Controls.Add(this.gridHistorico);
            this.pnlHistorico.Controls.Add(this.btnRelatorio);
            this.pnlHistorico.Controls.Add(this.pnlFiltro);
            this.pnlHistorico.Controls.Add(this.lblHist);

            // ── Form ──────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = System.Drawing.Color.FromArgb(245, 237, 216);
            this.ClientSize          = new System.Drawing.Size(640, 560);
            this.Font                = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize         = new System.Drawing.Size(640, 500);
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text                = "Turno de Caixa";
            this.Load               += new System.EventHandler(this.frmTurno_Load);

            this.Controls.Add(this.pnlHistorico);
            this.Controls.Add(this.pnlFechar);
            this.Controls.Add(this.pnlAbrir);
            this.Controls.Add(this.lblCaixaIni);
            this.Controls.Add(this.lblStatus);

            this.pnlAbrir.ResumeLayout(false);
            this.pnlAbrir.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCaixaInicial)).EndInit();
            this.pnlFechar.ResumeLayout(false);
            this.pnlFechar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCaixaFinal)).EndInit();
            this.pnlHistorico.ResumeLayout(false);
            this.pnlFiltro.ResumeLayout(false);
            this.pnlFiltro.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridHistorico)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label            lblStatus;
        private System.Windows.Forms.Label            lblCaixaIni;
        private System.Windows.Forms.Panel            pnlAbrir;
        private System.Windows.Forms.Label            lblAbrir;
        private System.Windows.Forms.Label            lblCaixaIniLabel;
        private System.Windows.Forms.NumericUpDown    numCaixaInicial;
        private System.Windows.Forms.Label            lblObsAbrir;
        private System.Windows.Forms.TextBox          txtObsAbrir;
        private System.Windows.Forms.Button           btnAbrir;
        private System.Windows.Forms.Panel            pnlFechar;
        private System.Windows.Forms.Label            lblFechar;
        private System.Windows.Forms.Label            lblCaixaFinLabel;
        private System.Windows.Forms.NumericUpDown    numCaixaFinal;
        private System.Windows.Forms.Label            lblObsFechar;
        private System.Windows.Forms.TextBox          txtObsFechar;
        private System.Windows.Forms.Button           btnFechar;
        private System.Windows.Forms.Panel            pnlHistorico;
        private System.Windows.Forms.Label            lblHist;
        private System.Windows.Forms.Panel            pnlFiltro;
        private System.Windows.Forms.Label            lblDe;
        private System.Windows.Forms.DateTimePicker   dtpDe;
        private System.Windows.Forms.Label            lblAte;
        private System.Windows.Forms.DateTimePicker   dtpAte;
        private System.Windows.Forms.Button           btnFiltrar;
        private System.Windows.Forms.DataGridView     gridHistorico;
        private System.Windows.Forms.Button           btnRelatorio;
    }
}
