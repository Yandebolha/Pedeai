using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmCadastroCupom
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
            this.btnN        = new System.Windows.Forms.Button();
            this.btnR        = new System.Windows.Forms.Button();
            this.pnlForm     = new System.Windows.Forms.Panel();
            this.pnlBtns     = new System.Windows.Forms.Panel();
            this.btnS        = new System.Windows.Forms.Button();
            this.btnC        = new System.Windows.Forms.Button();
            this.btnD        = new System.Windows.Forms.Button();
            this.txtDescricao= new System.Windows.Forms.TextBox();
            this.cmbTipo     = new System.Windows.Forms.ComboBox();
            this.cmbSituacao = new System.Windows.Forms.ComboBox();
            this.numValor    = new System.Windows.Forms.NumericUpDown();
            this.numMinimo   = new System.Windows.Forms.NumericUpDown();
            this.numLimite   = new System.Windows.Forms.NumericUpDown();
            this.dtpValido   = new System.Windows.Forms.DateTimePicker();
            this.lblDesc     = new System.Windows.Forms.Label();
            this.lblTipo     = new System.Windows.Forms.Label();
            this.lblSit      = new System.Windows.Forms.Label();
            this.lblVal      = new System.Windows.Forms.Label();
            this.lblMin      = new System.Windows.Forms.Label();
            this.lblLim      = new System.Windows.Forms.Label();
            this.lblVal2     = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // grid
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grid.ReadOnly = true;
            this.grid.AllowUserToAddRows = false;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.RowHeadersVisible = false;
            this.grid.BackgroundColor = System.Drawing.Color.FromArgb(250, 245, 238);
            this.grid.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            this.grid.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.grid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(224, 113, 42);
            this.grid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.grid.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.grid.GridColor = System.Drawing.Color.FromArgb(200, 185, 160);
            this.grid.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(240, 230, 202);
            this.grid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.grid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grid.ColumnHeadersHeight = 34;
            this.grid.RowTemplate.Height = 28;
            this.grid.DoubleClick += new System.EventHandler(this.Grid_DoubleClick);
            this.grid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            // topBar
            this.topBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.topBar.Height = 44;
            this.topBar.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.topBar.Controls.Add(this.btnN);
            this.topBar.Controls.Add(this.btnR);
            // btnN
            this.btnN.Text = "+ Novo Cupom";
            this.btnN.Left = 8; this.btnN.Top = 8; this.btnN.Width = 110; this.btnN.Height = 28;
            this.btnN.BackColor = System.Drawing.Color.FromArgb(87, 120, 38);
            this.btnN.ForeColor = System.Drawing.Color.White;
            this.btnN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnN.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnN.FlatAppearance.BorderSize = 0;
            this.btnN.Click += new System.EventHandler(this.BtnNovoCupom_Click);
            // btnR
            this.btnR.Text = "Atualizar";
            this.btnR.Left = 128; this.btnR.Top = 8; this.btnR.Width = 100; this.btnR.Height = 28;
            this.btnR.BackColor = System.Drawing.Color.FromArgb(224, 113, 42);
            this.btnR.ForeColor = System.Drawing.Color.White;
            this.btnR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnR.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnR.FlatAppearance.BorderSize = 0;
            this.btnR.Click += new System.EventHandler(this.BtnAtualizarCupom_Click);
            // pnlForm
            this.pnlForm.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlForm.Height = 150;
            this.pnlForm.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.pnlForm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlForm.Visible = false;
            this.pnlForm.Controls.Add(this.lblDesc);   this.pnlForm.Controls.Add(this.txtDescricao);
            this.pnlForm.Controls.Add(this.lblTipo);   this.pnlForm.Controls.Add(this.cmbTipo);
            this.pnlForm.Controls.Add(this.lblSit);    this.pnlForm.Controls.Add(this.cmbSituacao);
            this.pnlForm.Controls.Add(this.lblVal);    this.pnlForm.Controls.Add(this.numValor);
            this.pnlForm.Controls.Add(this.lblMin);    this.pnlForm.Controls.Add(this.numMinimo);
            this.pnlForm.Controls.Add(this.lblLim);    this.pnlForm.Controls.Add(this.numLimite);
            this.pnlForm.Controls.Add(this.lblVal2);   this.pnlForm.Controls.Add(this.dtpValido);
            this.pnlForm.Controls.Add(this.pnlBtns);
            // row 1 labels/fields
            this.lblDesc.Text = "Nome:"; this.lblDesc.AutoSize = true; this.lblDesc.Left = 10; this.lblDesc.Top = 11;
            this.txtDescricao.Left = 65; this.txtDescricao.Top = 8; this.txtDescricao.Width = 250;
            this.lblTipo.Text = "Tipo:"; this.lblTipo.AutoSize = true; this.lblTipo.Left = 455; this.lblTipo.Top = 11;
            this.cmbTipo.Left = 490; this.cmbTipo.Top = 8; this.cmbTipo.Width = 100;
            this.cmbTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipo.Items.AddRange(new object[] { "PERCENTUAL", "VALOR" });
            this.cmbTipo.SelectedIndex = 0;
            this.lblSit.Text = "Situa\u00e7\u00e3o:"; this.lblSit.AutoSize = true; this.lblSit.Left = 601; this.lblSit.Top = 11;
            this.cmbSituacao.Left = 660; this.cmbSituacao.Top = 8; this.cmbSituacao.Width = 90;
            this.cmbSituacao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSituacao.Items.AddRange(new object[] { "Ativo", "Inativo" });
            this.cmbSituacao.SelectedIndex = 0;
            // row 2
            this.lblVal.Text = "Valor:"; this.lblVal.AutoSize = true; this.lblVal.Left = 10; this.lblVal.Top = 45;
            this.numValor.Left = 55; this.numValor.Top = 42; this.numValor.Width = 80;
            this.numValor.DecimalPlaces = 2; this.numValor.Maximum = 9999;
            this.lblMin.Text = "Ped. M\u00ednimo:"; this.lblMin.AutoSize = true; this.lblMin.Left = 145; this.lblMin.Top = 45;
            this.numMinimo.Left = 230; this.numMinimo.Top = 42; this.numMinimo.Width = 80;
            this.numMinimo.DecimalPlaces = 2; this.numMinimo.Maximum = 9999;
            this.lblLim.Text = "Limite Usos:"; this.lblLim.AutoSize = true; this.lblLim.Left = 320; this.lblLim.Top = 45;
            this.numLimite.Left = 404; this.numLimite.Top = 42; this.numLimite.Width = 70;
            this.numLimite.Minimum = 0; this.numLimite.Maximum = 99999;
            this.lblVal2.Text = "V\u00e1lido at\u00e9:"; this.lblVal2.AutoSize = true; this.lblVal2.Left = 484; this.lblVal2.Top = 45;
            this.dtpValido.Left = 555; this.dtpValido.Top = 42; this.dtpValido.Width = 120;
            this.dtpValido.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            // pnlBtns
            this.pnlBtns.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBtns.Height = 48;
            this.pnlBtns.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.pnlBtns.Controls.Add(this.btnS);
            this.pnlBtns.Controls.Add(this.btnC);
            this.pnlBtns.Controls.Add(this.btnD);
            this.pnlBtns.SizeChanged += new System.EventHandler(this.PnlBtns_SizeChanged);
            // btnS
            this.btnS.Text = "Salvar"; this.btnS.Left = 10; this.btnS.Top = 10; this.btnS.Width = 110; this.btnS.Height = 28;
            this.btnS.BackColor = System.Drawing.Color.FromArgb(87, 120, 38); this.btnS.ForeColor = System.Drawing.Color.White;
            this.btnS.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnS.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnS.FlatAppearance.BorderSize = 0; this.btnS.Click += new System.EventHandler(this.BtnSalvar_Click);
            // btnC
            this.btnC.Text = "Cancelar"; this.btnC.Left = 130; this.btnC.Top = 10; this.btnC.Width = 110; this.btnC.Height = 28;
            this.btnC.BackColor = System.Drawing.Color.FromArgb(224, 113, 42); this.btnC.ForeColor = System.Drawing.Color.White;
            this.btnC.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnC.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnC.FlatAppearance.BorderSize = 0; this.btnC.Click += new System.EventHandler(this.BtnCancelarCupom_Click);
            // btnD
            this.btnD.Text = "Desativar"; this.btnD.Left = 250; this.btnD.Top = 10; this.btnD.Width = 110; this.btnD.Height = 28;
            this.btnD.BackColor = System.Drawing.Color.FromArgb(192, 57, 43); this.btnD.ForeColor = System.Drawing.Color.White;
            this.btnD.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnD.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnD.FlatAppearance.BorderSize = 0; this.btnD.Click += new System.EventHandler(this.BtnDesativar_Click);
            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 481);
            this.MinimumSize = new System.Drawing.Size(750, 450);
            this.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cadastro de Cupons";
            this.Controls.Add(this.grid);
            this.Controls.Add(this.topBar);
            this.Controls.Add(this.pnlForm);
            this.ResumeLayout(false);
        }

        internal System.Windows.Forms.DataGridView   grid;
        internal System.Windows.Forms.Panel          pnlForm;
        internal System.Windows.Forms.TextBox        txtDescricao;
        internal System.Windows.Forms.ComboBox       cmbTipo;
        internal System.Windows.Forms.ComboBox       cmbSituacao;
        internal System.Windows.Forms.NumericUpDown  numValor;
        internal System.Windows.Forms.NumericUpDown  numMinimo;
        internal System.Windows.Forms.NumericUpDown  numLimite;
        internal System.Windows.Forms.DateTimePicker dtpValido;
        internal System.Windows.Forms.Button         btnN;
        internal System.Windows.Forms.Button         btnR;
        internal System.Windows.Forms.Button         btnS;
        internal System.Windows.Forms.Button         btnC;
        internal System.Windows.Forms.Button         btnD;
        private  System.Windows.Forms.Panel          topBar;
        private  System.Windows.Forms.Panel          pnlBtns;
        private  System.Windows.Forms.Label          lblDesc;
        private  System.Windows.Forms.Label          lblTipo;
        private  System.Windows.Forms.Label          lblSit;
        private  System.Windows.Forms.Label          lblVal;
        private  System.Windows.Forms.Label          lblMin;
        private  System.Windows.Forms.Label          lblLim;
        private  System.Windows.Forms.Label          lblVal2;

        private void Grid_DoubleClick(object s, System.EventArgs e) { CarregarParaEditar(); }
        private void Grid_DataError(object s, System.Windows.Forms.DataGridViewDataErrorEventArgs e) { e.ThrowException = false; }
        private void BtnNovoCupom_Click(object s, System.EventArgs e)    { ModoNovo(); }
        private void BtnAtualizarCupom_Click(object s, System.EventArgs e){ CarregarGrid(); }
        private void BtnCancelarCupom_Click(object s, System.EventArgs e) { pnlForm.Visible = false; _codigoEditando = 0; }
        private void PnlBtns_SizeChanged(object s, System.EventArgs e)
        {
            int x = (pnlBtns.Width - 110 * 3 - 20) / 2;
            if (x < 10) x = 10;
            btnS.Left = x; btnC.Left = x + 120; btnD.Left = x + 240;
        }
    }
}
