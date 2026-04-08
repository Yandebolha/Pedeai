using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmAvisos
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.cmbFiltro = new System.Windows.Forms.ComboBox();
            this.btnAtualizar = new System.Windows.Forms.Button();
            this.lblTit = new System.Windows.Forms.Label();
            this.pnlResumo = new System.Windows.Forms.Panel();
            this.lblResumo = new System.Windows.Forms.Label();
            this.grid = new System.Windows.Forms.DataGridView();
            this.pnlFoot = new System.Windows.Forms.Panel();
            this.btnPago = new System.Windows.Forms.Button();
            this.pnlTop.SuspendLayout();
            this.pnlResumo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.pnlFoot.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.pnlTop.Controls.Add(this.cmbFiltro);
            this.pnlTop.Controls.Add(this.btnAtualizar);
            this.pnlTop.Controls.Add(this.lblTit);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(950, 52);
            this.pnlTop.TabIndex = 3;
            // 
            // cmbFiltro
            // 
            this.cmbFiltro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFiltro.Items.AddRange(new object[] {
            "Todas em Aberto",
            "Vencidas",
            "Vencem Hoje",
            "Próximos 7 dias",
            "Próximos 30 dias",
            "Pagas"});
            this.cmbFiltro.Location = new System.Drawing.Point(224, 17);
            this.cmbFiltro.Name = "cmbFiltro";
            this.cmbFiltro.Size = new System.Drawing.Size(160, 23);
            this.cmbFiltro.TabIndex = 0;
            this.cmbFiltro.SelectedIndex = 0;
            this.cmbFiltro.SelectedIndexChanged += new System.EventHandler(this.CmbFiltro_SelectedIndexChanged);
            // 
            // btnAtualizar
            // 
            this.btnAtualizar.BackColor = System.Drawing.Color.FromArgb(224, 113, 42);
            this.btnAtualizar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAtualizar.FlatAppearance.BorderSize = 0;
            this.btnAtualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAtualizar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnAtualizar.ForeColor = System.Drawing.Color.White;
            this.btnAtualizar.Location = new System.Drawing.Point(414, 15);
            this.btnAtualizar.Name = "btnAtualizar";
            this.btnAtualizar.Size = new System.Drawing.Size(95, 28);
            this.btnAtualizar.TabIndex = 1;
            this.btnAtualizar.Text = "⟳ Atualizar";
            this.btnAtualizar.UseVisualStyleBackColor = false;
            this.btnAtualizar.Click += new System.EventHandler(this.BtnAtualizar_Click);
            // 
            this.lblTit.AutoSize = true;
            this.lblTit.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTit.ForeColor = System.Drawing.Color.White;
            this.lblTit.Location = new System.Drawing.Point(0, 15);
            this.lblTit.Name = "lblTit";
            this.lblTit.Size = new System.Drawing.Size(205, 21);
            this.lblTit.TabIndex = 2;
            this.lblTit.Text = "🔔  Avisos de Pagamento";
            // 
            // pnlResumo
            // 
            this.pnlResumo.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.pnlResumo.Controls.Add(this.lblResumo);
            this.pnlResumo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlResumo.Location = new System.Drawing.Point(0, 52);
            this.pnlResumo.Name = "pnlResumo";
            this.pnlResumo.Size = new System.Drawing.Size(950, 36);
            this.pnlResumo.TabIndex = 1;
            // 
            // lblResumo
            // 
            this.lblResumo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblResumo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblResumo.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.lblResumo.Location = new System.Drawing.Point(0, 0);
            this.lblResumo.Name = "lblResumo";
            this.lblResumo.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.lblResumo.Size = new System.Drawing.Size(950, 36);
            this.lblResumo.TabIndex = 0;
            this.lblResumo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // grid
            // 
            this.grid.AllowUserToAddRows = false;
            this.grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grid.BackgroundColor = System.Drawing.Color.FromArgb(250, 245, 238);
            this.grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(240, 230, 202);
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grid.DefaultCellStyle = dataGridViewCellStyle2;
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.grid.GridColor = System.Drawing.Color.FromArgb(200, 185, 160);
            this.grid.Location = new System.Drawing.Point(0, 88);
            this.grid.Name = "grid";
            this.grid.ReadOnly = true;
            this.grid.RowHeadersVisible = false;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.Size = new System.Drawing.Size(950, 466);
            this.grid.TabIndex = 0;
            this.grid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            // 
            // pnlFoot
            // 
            this.pnlFoot.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.pnlFoot.Controls.Add(this.btnPago);
            this.pnlFoot.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFoot.Location = new System.Drawing.Point(0, 554);
            this.pnlFoot.Name = "pnlFoot";
            this.pnlFoot.Size = new System.Drawing.Size(950, 46);
            this.pnlFoot.TabIndex = 2;
            // 
            // btnPago
            // 
            this.btnPago.BackColor = System.Drawing.Color.FromArgb(87, 120, 38);
            this.btnPago.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPago.FlatAppearance.BorderSize = 0;
            this.btnPago.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPago.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnPago.ForeColor = System.Drawing.Color.White;
            this.btnPago.Location = new System.Drawing.Point(12, 9);
            this.btnPago.Name = "btnPago";
            this.btnPago.Size = new System.Drawing.Size(162, 28);
            this.btnPago.TabIndex = 0;
            this.btnPago.Text = "✔ Marcar como Pago";
            this.btnPago.UseVisualStyleBackColor = false;
            this.btnPago.Click += new System.EventHandler(this.BtnPago_Click);
            // 
            this.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.ClientSize = new System.Drawing.Size(950, 600);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.pnlResumo);
            this.Controls.Add(this.pnlFoot);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.MinimumSize = new System.Drawing.Size(820, 480);
            this.Name = "frmAvisos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Avisos — Parcelas a Pagar";
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlResumo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.pnlFoot.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private Panel        pnlTop;
        private Label        lblTit;
        private ComboBox     cmbFiltro;
        private Button       btnAtualizar;
        private Panel        pnlResumo;
        private Label        lblResumo;
        private DataGridView grid;
        private Panel        pnlFoot;
        private Button       btnPago;
    }
}
