using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmCadastroCategoria
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.grid        = new System.Windows.Forms.DataGridView();
            this.topBar      = new System.Windows.Forms.Panel();
            this.btnNovo     = new System.Windows.Forms.Button();
            this.btnEditar   = new System.Windows.Forms.Button();
            this.pnlForm     = new System.Windows.Forms.Panel();
            this.pnlBtns     = new System.Windows.Forms.Panel();
            this.btnS        = new System.Windows.Forms.Button();
            this.btnC        = new System.Windows.Forms.Button();
            this.btnD        = new System.Windows.Forms.Button();
            this.lblNome     = new System.Windows.Forms.Label();
            this.txtNome     = new System.Windows.Forms.TextBox();
            this.cmbSituacao = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // grid
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grid.ReadOnly = true;
            this.grid.AllowUserToAddRows = false;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.RowHeadersVisible = false;
            this.grid.BackgroundColor = System.Drawing.Color.FromArgb(248, 242, 235);
            this.grid.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(248, 242, 235);
            this.grid.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(42, 20, 8);
            this.grid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(200, 70, 20);
            this.grid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.grid.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(243, 237, 229);
            this.grid.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grid.GridColor = System.Drawing.Color.FromArgb(215, 198, 178);
            this.grid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(45, 22, 10);
            this.grid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.grid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grid.ColumnHeadersHeight = 34;
            this.grid.RowTemplate.Height = 28;
            this.grid.DoubleClick += new System.EventHandler(this.Grid_DoubleClick);
            this.grid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            // topBar
            this.topBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.topBar.Height = 44;
            this.topBar.BackColor = System.Drawing.Color.FromArgb(45, 22, 10);
            this.topBar.Controls.Add(this.btnNovo);
            this.topBar.Controls.Add(this.btnEditar);
            // btnNovo
            this.btnNovo.Text = "+ Novo";
            this.btnNovo.Left = 8;
            this.btnNovo.Top = 8;
            this.btnNovo.Width = 90;
            this.btnNovo.Height = 28;
            this.btnNovo.BackColor = System.Drawing.Color.FromArgb(120, 80, 25);
            this.btnNovo.ForeColor = System.Drawing.Color.White;
            this.btnNovo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNovo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnNovo.FlatAppearance.BorderSize = 0;
            this.btnNovo.Click += new System.EventHandler(this.BtnNovo_Click);
            // btnEditar
            this.btnEditar.Text = "\u270F Editar";
            this.btnEditar.Left = 108;
            this.btnEditar.Top = 8;
            this.btnEditar.Width = 90;
            this.btnEditar.Height = 28;
            this.btnEditar.BackColor = System.Drawing.Color.FromArgb(160, 100, 30);
            this.btnEditar.ForeColor = System.Drawing.Color.White;
            this.btnEditar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnEditar.FlatAppearance.BorderSize = 0;
            this.btnEditar.Click += new System.EventHandler(this.BtnEditar_Click);
            // pnlForm
            this.pnlForm.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlForm.Height = 90;
            this.pnlForm.BackColor = System.Drawing.Color.FromArgb(55, 30, 12);
            this.pnlForm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlForm.Visible = false;
            this.pnlForm.Controls.Add(this.lblNome);
            this.pnlForm.Controls.Add(this.txtNome);
            this.pnlForm.Controls.Add(this.cmbSituacao);
            this.pnlForm.Controls.Add(this.pnlBtns);
            // pnlBtns
            this.pnlBtns.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBtns.Height = 44;
            this.pnlBtns.BackColor = System.Drawing.Color.FromArgb(55, 30, 12);
            this.pnlBtns.Controls.Add(this.btnS);
            this.pnlBtns.Controls.Add(this.btnC);
            this.pnlBtns.Controls.Add(this.btnD);
            this.pnlBtns.SizeChanged += new System.EventHandler(this.PnlBtns_SizeChanged);
            // btnS
            this.btnS.Text = "Salvar";
            this.btnS.Left = 10;
            this.btnS.Top = 10;
            this.btnS.Width = 110;
            this.btnS.Height = 28;
            this.btnS.BackColor = System.Drawing.Color.FromArgb(200, 70, 20);
            this.btnS.ForeColor = System.Drawing.Color.White;
            this.btnS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnS.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnS.FlatAppearance.BorderSize = 0;
            this.btnS.Click += new System.EventHandler(this.BtnSalvar_Click);
            // btnC
            this.btnC.Text = "Cancelar";
            this.btnC.Left = 130;
            this.btnC.Top = 10;
            this.btnC.Width = 110;
            this.btnC.Height = 28;
            this.btnC.BackColor = System.Drawing.Color.FromArgb(150, 125, 100);
            this.btnC.ForeColor = System.Drawing.Color.White;
            this.btnC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnC.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnC.FlatAppearance.BorderSize = 0;
            this.btnC.Click += new System.EventHandler(this.BtnCancelar_Click);
            // btnD
            this.btnD.Text = "Desativar";
            this.btnD.Left = 250;
            this.btnD.Top = 10;
            this.btnD.Width = 110;
            this.btnD.Height = 28;
            this.btnD.BackColor = System.Drawing.Color.FromArgb(178, 38, 20);
            this.btnD.ForeColor = System.Drawing.Color.White;
            this.btnD.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnD.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnD.FlatAppearance.BorderSize = 0;
            this.btnD.Click += new System.EventHandler(this.BtnDesativar_Click);
            // lblNome
            this.lblNome.Text = "Nome:";
            this.lblNome.ForeColor = System.Drawing.Color.FromArgb(195, 158, 120);
            this.lblNome.AutoSize = true;
            this.lblNome.Left = 20;
            this.lblNome.Top = 18;
            // txtNome
            this.txtNome.Left = 70;
            this.txtNome.Top = 14;
            this.txtNome.Width = 320;
            this.txtNome.BackColor = System.Drawing.Color.FromArgb(248, 242, 235);
            this.txtNome.ForeColor = System.Drawing.Color.FromArgb(42, 20, 8);
            this.txtNome.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            // cmbSituacao
            this.cmbSituacao.Left = 400;
            this.cmbSituacao.Top = 14;
            this.cmbSituacao.Width = 80;
            this.cmbSituacao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSituacao.Items.AddRange(new object[] { "A", "I" });
            this.cmbSituacao.SelectedIndex = 0;
            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 360);
            this.MinimumSize = new System.Drawing.Size(450, 320);
            this.BackColor = System.Drawing.Color.FromArgb(252, 248, 244);
            this.ForeColor = System.Drawing.Color.FromArgb(42, 20, 8);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Categorias de Produtos";
            this.Controls.Add(this.grid);
            this.Controls.Add(this.topBar);
            this.Controls.Add(this.pnlForm);
            this.ResumeLayout(false);
        }

        internal System.Windows.Forms.DataGridView grid;
        internal System.Windows.Forms.Panel        pnlForm;
        internal System.Windows.Forms.TextBox      txtNome;
        internal System.Windows.Forms.ComboBox     cmbSituacao;
        internal System.Windows.Forms.Button       btnNovo;
        internal System.Windows.Forms.Button       btnEditar;
        internal System.Windows.Forms.Button       btnS;
        internal System.Windows.Forms.Button       btnC;
        internal System.Windows.Forms.Button       btnD;
        private  System.Windows.Forms.Panel        topBar;
        private  System.Windows.Forms.Panel        pnlBtns;
        private  System.Windows.Forms.Label        lblNome;

        private void Grid_DoubleClick(object s, System.EventArgs e) { CarregarParaEditar(); }
        private void Grid_DataError(object s, System.Windows.Forms.DataGridViewDataErrorEventArgs e) { e.ThrowException = false; }
        private void BtnNovo_Click(object s, System.EventArgs e)   { ModoNovo(); }
        private void BtnEditar_Click(object s, System.EventArgs e)  { CarregarParaEditar(); }
        private void BtnCancelar_Click(object s, System.EventArgs e){ pnlForm.Visible = false; _codigoEditando = 0; }
        private void PnlBtns_SizeChanged(object s, System.EventArgs e)
        {
            int x = (pnlBtns.Width - 110 * 3 - 20) / 2;
            if (x < 10) x = 10;
            btnS.Left = x; btnC.Left = x + 120; btnD.Left = x + 240;
        }
    }
}
