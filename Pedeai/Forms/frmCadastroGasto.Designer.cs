using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmCadastroGasto
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.Text            = "Lancar Gasto de Material";
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = this.MinimizeBox = false;
            this.BackColor       = Color.FromArgb(28, 37, 65);
            this.ForeColor       = Color.White;
            this.Font            = new Font("Segoe UI", 10F);
            this.ClientSize      = new System.Drawing.Size(420, 240);

            Color clr = Color.FromArgb(15, 22, 45);
            int ex = 20, lw = 100, tw = 280, y = 16;

            Label MkL(string t) => new Label { Text = t, ForeColor = Color.FromArgb(180,190,220), AutoSize = true, Font = new Font("Segoe UI",10F) };

            var lblData = MkL("Data:");
            lblData.SetBounds(ex, y+3, lw, 22);
            dtpData = new DateTimePicker { Format = DateTimePickerFormat.Short };
            dtpData.SetBounds(ex+lw+8, y, tw, 26);
            y += 44;

            var lblDesc = MkL("Descricao:");
            lblDesc.SetBounds(ex, y+3, lw, 22);
            txtDescricao = new TextBox { BackColor = clr, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            txtDescricao.SetBounds(ex+lw+8, y, tw, 26);
            y += 44;

            var lblVal = MkL("Valor (R$):");
            lblVal.SetBounds(ex, y+3, lw, 22);
            numValor = new NumericUpDown { DecimalPlaces = 2, Maximum = 999999M, BackColor = clr, ForeColor = Color.White };
            numValor.SetBounds(ex+lw+8, y, 140, 26);
            y += 44;

            var lblObs = MkL("Obs:");
            lblObs.SetBounds(ex, y+3, lw, 22);
            txtObs = new TextBox { BackColor = clr, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            txtObs.SetBounds(ex+lw+8, y, tw, 26);
            y += 44;

            var btnSalvar = new Button
            {
                Text      = "\u2714 Salvar",
                Bounds    = new System.Drawing.Rectangle(ex+lw+8, y+4, 130, 30),
                BackColor = Color.FromArgb(39,174,96),
                ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btnSalvar.FlatAppearance.BorderSize = 0;
            btnSalvar.Click += BtnSalvar_Click;

            var btnCan = new Button
            {
                Text         = "Cancelar",
                Bounds       = new System.Drawing.Rectangle(ex+lw+148, y+4, 130, 30),
                BackColor    = Color.FromArgb(108,117,125),
                ForeColor    = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
                DialogResult = System.Windows.Forms.DialogResult.Cancel
            };
            btnCan.FlatAppearance.BorderSize = 0;

            this.AcceptButton = btnSalvar;
            this.CancelButton = btnCan;

            this.Controls.AddRange(new System.Windows.Forms.Control[]
                { lblData, dtpData, lblDesc, txtDescricao, lblVal, numValor, lblObs, txtObs, btnSalvar, btnCan });
        }

        internal DateTimePicker dtpData;
        internal System.Windows.Forms.TextBox txtDescricao;
        internal System.Windows.Forms.NumericUpDown numValor;
        internal System.Windows.Forms.TextBox txtObs;
    }
}
