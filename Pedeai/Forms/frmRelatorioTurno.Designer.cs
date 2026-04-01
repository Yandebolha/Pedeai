namespace Pedeai.Forms
{
    partial class frmRelatorioTurno
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblCabecalho   = new System.Windows.Forms.Label();
            this.lblCaixaInicial= new System.Windows.Forms.Label();
            this.lblFechamento  = new System.Windows.Forms.Label();
            this.lblCaixaFinal  = new System.Windows.Forms.Label();
            this.lblResumo      = new System.Windows.Forms.Label();
            this.gridPedidos    = new System.Windows.Forms.DataGridView();
            this.btnImprimir    = new System.Windows.Forms.Button();
            this.pnlInfo        = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.gridPedidos)).BeginInit();
            this.pnlInfo.SuspendLayout();
            this.SuspendLayout();

            // ── pnlInfo (cabeçalho) ────────────────────────────────────────
            this.pnlInfo.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlInfo.Height    = 90;
            this.pnlInfo.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);

            this.lblCabecalho.Dock      = System.Windows.Forms.DockStyle.Top;
            this.lblCabecalho.Height    = 32;
            this.lblCabecalho.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblCabecalho.ForeColor = System.Drawing.Color.White;
            this.lblCabecalho.BackColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.lblCabecalho.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCabecalho.Text      = "";

            this.lblCaixaInicial.Left = 12; this.lblCaixaInicial.Top = 38;
            this.lblCaixaInicial.AutoSize  = true;
            this.lblCaixaInicial.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.lblCaixaInicial.Font      = new System.Drawing.Font("Segoe UI", 9F);

            this.lblFechamento.Left = 220; this.lblFechamento.Top = 38;
            this.lblFechamento.AutoSize  = true;
            this.lblFechamento.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.lblFechamento.Font      = new System.Drawing.Font("Segoe UI", 9F);

            this.lblCaixaFinal.Left = 440; this.lblCaixaFinal.Top = 38;
            this.lblCaixaFinal.AutoSize  = true;
            this.lblCaixaFinal.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.lblCaixaFinal.Font      = new System.Drawing.Font("Segoe UI", 9F);

            this.pnlInfo.Controls.Add(this.lblCaixaInicial);
            this.pnlInfo.Controls.Add(this.lblFechamento);
            this.pnlInfo.Controls.Add(this.lblCaixaFinal);
            this.pnlInfo.Controls.Add(this.lblCabecalho);

            // ── gridPedidos ────────────────────────────────────────────────
            this.gridPedidos.Dock                  = System.Windows.Forms.DockStyle.Fill;
            this.gridPedidos.BackgroundColor        = System.Drawing.Color.FromArgb(235, 226, 208);
            this.gridPedidos.ForeColor              = System.Drawing.Color.White;
            this.gridPedidos.GridColor              = System.Drawing.Color.FromArgb(50, 60, 100);
            this.gridPedidos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridPedidos.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.gridPedidos.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(200, 210, 240);
            this.gridPedidos.DefaultCellStyle.BackColor  = System.Drawing.Color.FromArgb(30, 40, 70);
            this.gridPedidos.DefaultCellStyle.ForeColor  = System.Drawing.Color.White;
            this.gridPedidos.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(224, 113, 42);
            this.gridPedidos.AutoSizeColumnsMode    = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridPedidos.ReadOnly               = true;
            this.gridPedidos.SelectionMode          = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridPedidos.AllowUserToAddRows      = false;
            this.gridPedidos.RowHeadersVisible       = false;
            this.gridPedidos.EnableHeadersVisualStyles = false;
            this.gridPedidos.DataError              += (_, e2) => e2.ThrowException = false;

            // ── lblResumo ──────────────────────────────────────────────────
            this.lblResumo.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.lblResumo.Height    = 28;
            this.lblResumo.BackColor = System.Drawing.Color.FromArgb(235, 226, 208);
            this.lblResumo.ForeColor = System.Drawing.Color.FromArgb(243, 156, 18);
            this.lblResumo.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblResumo.Padding   = new System.Windows.Forms.Padding(6, 6, 0, 0);
            this.lblResumo.Text      = "";

            // ── btnImprimir ────────────────────────────────────────────────
            this.btnImprimir.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.btnImprimir.Height    = 32;
            this.btnImprimir.Text      = "\U0001F5A8 Imprimir Relat\u00f3rio";
            this.btnImprimir.BackColor = System.Drawing.Color.FromArgb(224, 113, 42);
            this.btnImprimir.ForeColor = System.Drawing.Color.White;
            this.btnImprimir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImprimir.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnImprimir.Click    += new System.EventHandler(this.BtnImprimir_Click);
            this.btnImprimir.FlatAppearance.BorderSize = 0;

            // ── Form ──────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = System.Drawing.Color.FromArgb(245, 237, 216);
            this.ClientSize          = new System.Drawing.Size(860, 580);
            this.Font                = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize         = new System.Drawing.Size(700, 480);
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text                = "Relat\u00f3rio de Turno \u2014 Movimenta\u00e7\u00f5es";
            this.Load               += new System.EventHandler(this.frmRelatorioTurno_Load);

            this.Controls.Add(this.gridPedidos);
            this.Controls.Add(this.lblResumo);
            this.Controls.Add(this.btnImprimir);
            this.Controls.Add(this.pnlInfo);

            ((System.ComponentModel.ISupportInitialize)(this.gridPedidos)).EndInit();
            this.pnlInfo.ResumeLayout(false);
            this.pnlInfo.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label       lblCabecalho;
        private System.Windows.Forms.Label       lblCaixaInicial;
        private System.Windows.Forms.Label       lblFechamento;
        private System.Windows.Forms.Label       lblCaixaFinal;
        private System.Windows.Forms.Label       lblResumo;
        private System.Windows.Forms.DataGridView gridPedidos;
        private System.Windows.Forms.Button      btnImprimir;
        private System.Windows.Forms.Panel       pnlInfo;
    }
}
