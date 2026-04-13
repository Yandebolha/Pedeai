namespace Pedeai.Forms
{
    partial class frmWhatsApp
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

            // ── Form ─────────────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = System.Drawing.Color.FromArgb(245, 237, 216);
            this.ClientSize          = new System.Drawing.Size(700, 560);
            this.Font                = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize         = new System.Drawing.Size(640, 500);
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text                = "WhatsApp";

            this.ResumeLayout(false);
        }

        // ── Campos de UI (preenchidos em runtime por ConstruirUI) ─────────────
        private System.Windows.Forms.Timer       _timerStatus;
        private System.Windows.Forms.PictureBox  _picQR;
        private System.Windows.Forms.Label       _lblStatus;
        private System.Windows.Forms.Label       _lblInstrucao;
        private System.Windows.Forms.Button      _btnRefreshQR;
        private System.Windows.Forms.Button      _btnDesconectar;
        private System.Windows.Forms.TextBox     _txtTelefone;
        private System.Windows.Forms.Label       _lblPairingCode;
        private System.Windows.Forms.TextBox     _txtApiUrl;
        private System.Windows.Forms.TextBox     _txtApiKey;
        private System.Windows.Forms.TextBox     _txtInstance;
        private System.Windows.Forms.TextBox     _txtMsgPreparo;
        private System.Windows.Forms.TextBox     _txtMsgEntrega;
        private System.Windows.Forms.TextBox     _txtMsgCupom;
        private System.Windows.Forms.DataGridView _gridPromocoes;
        private System.Windows.Forms.DataGridView _gridPromItens;
    }
}
