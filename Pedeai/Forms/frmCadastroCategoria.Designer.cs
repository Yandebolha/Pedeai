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
            this.chkHabSite  = new System.Windows.Forms.CheckBox();
            this.picImagem   = new System.Windows.Forms.PictureBox();
            this.btnImagem   = new System.Windows.Forms.Button();
            this.lblImagem   = new System.Windows.Forms.Label();
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
            this.grid.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grid.GridColor = System.Drawing.Color.FromArgb(200, 185, 160);
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
            this.topBar.Controls.Add(this.btnNovo);
            this.topBar.Controls.Add(this.btnEditar);
            // btnNovo
            this.btnNovo.Text = "+ Novo";
            this.btnNovo.Left = 8; this.btnNovo.Top = 8; this.btnNovo.Width = 90; this.btnNovo.Height = 28;
            this.btnNovo.BackColor = System.Drawing.Color.FromArgb(87, 120, 38);
            this.btnNovo.ForeColor = System.Drawing.Color.White;
            this.btnNovo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNovo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnNovo.FlatAppearance.BorderSize = 0;
            this.btnNovo.Click += new System.EventHandler(this.BtnNovo_Click);
            // btnEditar
            this.btnEditar.Text = "\u270F Editar";
            this.btnEditar.Left = 108; this.btnEditar.Top = 8; this.btnEditar.Width = 90; this.btnEditar.Height = 28;
            this.btnEditar.BackColor = System.Drawing.Color.FromArgb(230, 126, 34);
            this.btnEditar.ForeColor = System.Drawing.Color.White;
            this.btnEditar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnEditar.FlatAppearance.BorderSize = 0;
            this.btnEditar.Click += new System.EventHandler(this.BtnEditar_Click);
            // pnlForm — taller to fit image row
            this.pnlForm.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlForm.Height = 220;
            this.pnlForm.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.pnlForm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlForm.Visible = false;
            this.pnlForm.Controls.Add(this.lblNome);
            this.pnlForm.Controls.Add(this.txtNome);
            this.pnlForm.Controls.Add(this.cmbSituacao);
            this.pnlForm.Controls.Add(this.chkHabSite);
            this.pnlForm.Controls.Add(this.picImagem);
            this.pnlForm.Controls.Add(this.btnImagem);
            this.pnlForm.Controls.Add(this.lblImagem);
            this.pnlForm.Controls.Add(this.pnlBtns);
            // pnlBtns
            this.pnlBtns.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBtns.Height = 44;
            this.pnlBtns.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.pnlBtns.Controls.Add(this.btnS);
            this.pnlBtns.Controls.Add(this.btnC);
            this.pnlBtns.Controls.Add(this.btnD);
            this.pnlBtns.SizeChanged += new System.EventHandler(this.PnlBtns_SizeChanged);
            // btnS
            this.btnS.Text = "Salvar"; this.btnS.Left = 10; this.btnS.Top = 10; this.btnS.Width = 110; this.btnS.Height = 28;
            this.btnS.BackColor = System.Drawing.Color.FromArgb(87, 120, 38); this.btnS.ForeColor = System.Drawing.Color.White;
            this.btnS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnS.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnS.FlatAppearance.BorderSize = 0;
            this.btnS.Click += new System.EventHandler(this.BtnSalvar_Click);
            // btnC
            this.btnC.Text = "Cancelar"; this.btnC.Left = 130; this.btnC.Top = 10; this.btnC.Width = 110; this.btnC.Height = 28;
            this.btnC.BackColor = System.Drawing.Color.FromArgb(224, 113, 42); this.btnC.ForeColor = System.Drawing.Color.White;
            this.btnC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnC.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnC.FlatAppearance.BorderSize = 0;
            this.btnC.Click += new System.EventHandler(this.BtnCancelar_Click);
            // btnD
            this.btnD.Text = "Desativar"; this.btnD.Left = 250; this.btnD.Top = 10; this.btnD.Width = 110; this.btnD.Height = 28;
            this.btnD.BackColor = System.Drawing.Color.FromArgb(192, 57, 43); this.btnD.ForeColor = System.Drawing.Color.White;
            this.btnD.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnD.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnD.FlatAppearance.BorderSize = 0;
            this.btnD.Click += new System.EventHandler(this.BtnDesativar_Click);
            // lblNome
            this.lblNome.Text = "Nome:"; this.lblNome.AutoSize = true; this.lblNome.Left = 20; this.lblNome.Top = 18;
            this.lblNome.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            // txtNome
            this.txtNome.Left = 70; this.txtNome.Top = 14; this.txtNome.Width = 260;
            this.txtNome.BackColor = System.Drawing.Color.White;
            this.txtNome.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtNome.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            // cmbSituacao
            this.cmbSituacao.Left = 340; this.cmbSituacao.Top = 14; this.cmbSituacao.Width = 80;
            this.cmbSituacao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSituacao.Items.AddRange(new object[] { "A", "I" });
            this.cmbSituacao.SelectedIndex = 0;
            // chkHabSite
            this.chkHabSite.Text = "Habilitar no Site";
            this.chkHabSite.Left = 20; this.chkHabSite.Top = 50; this.chkHabSite.AutoSize = true;
            this.chkHabSite.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.chkHabSite.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            // picImagem â€” round preview 80x80
            this.picImagem.Left = 20; this.picImagem.Top = 75;
            this.picImagem.Width = 80; this.picImagem.Height = 80;
            this.picImagem.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            this.picImagem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picImagem.BackColor = System.Drawing.Color.FromArgb(220, 210, 190);
            // btnImagem
            this.btnImagem.Text = "\U0001F5BC Escolher Imagem";
            this.btnImagem.Left = 110; this.btnImagem.Top = 80; this.btnImagem.Width = 160; this.btnImagem.Height = 30;
            this.btnImagem.BackColor = System.Drawing.Color.FromArgb(224, 113, 42);
            this.btnImagem.ForeColor = System.Drawing.Color.White;
            this.btnImagem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImagem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnImagem.FlatAppearance.BorderSize = 0;
            this.btnImagem.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImagem.Click += new System.EventHandler(this.BtnImagem_Click);
            // lblImagem
            this.lblImagem.Left = 110; this.lblImagem.Top = 116;
            this.lblImagem.Width = 260; this.lblImagem.AutoSize = false;
            this.lblImagem.Text = "Nenhuma imagem selecionada";
            this.lblImagem.ForeColor = System.Drawing.Color.FromArgb(120, 100, 60);
            this.lblImagem.Font = new System.Drawing.Font("Segoe UI", 8F);
            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 400);
            this.MinimumSize = new System.Drawing.Size(480, 360);
            this.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
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
        internal System.Windows.Forms.CheckBox     chkHabSite;
        internal System.Windows.Forms.PictureBox   picImagem;
        internal System.Windows.Forms.Button       btnImagem;
        internal System.Windows.Forms.Label        lblImagem;
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

