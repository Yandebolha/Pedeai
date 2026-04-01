using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmSelecionarCliente
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.grid          = new System.Windows.Forms.DataGridView();
            this.topBar        = new System.Windows.Forms.Panel();
            this.txtBusca      = new System.Windows.Forms.TextBox();
            this.btnB          = new System.Windows.Forms.Button();
            this.btnN          = new System.Windows.Forms.Button();
            this.btnSel        = new System.Windows.Forms.Button();
            this.pnlForm       = new System.Windows.Forms.Panel();
            this.txtNomeCad    = new System.Windows.Forms.TextBox();
            this.txtCpfCad     = new System.Windows.Forms.TextBox();
            this.cmbSituacao   = new System.Windows.Forms.ComboBox();
            this.txtTelefoneCad= new System.Windows.Forms.MaskedTextBox();
            this.txtCelularCad = new System.Windows.Forms.MaskedTextBox();
            this.txtEmailCad   = new System.Windows.Forms.TextBox();
            this.btnS          = new System.Windows.Forms.Button();
            this.btnC          = new System.Windows.Forms.Button();
            this.lblB          = new System.Windows.Forms.Label();
            this.lblNome       = new System.Windows.Forms.Label();
            this.lblCpf        = new System.Windows.Forms.Label();
            this.lblSit        = new System.Windows.Forms.Label();
            this.lblTel        = new System.Windows.Forms.Label();
            this.lblCel        = new System.Windows.Forms.Label();
            this.lblEmail      = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // grid
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grid.ReadOnly = true; this.grid.AllowUserToAddRows = false;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.RowHeadersVisible = false;
            this.grid.BackgroundColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.grid.GridColor = System.Drawing.Color.FromArgb(50, 60, 100);
            this.grid.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.grid.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.grid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(224, 113, 42);
            this.grid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.grid.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.grid.Font = new System.Drawing.Font("Segoe UI", 9F); this.grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(240, 230, 202);
            this.grid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.grid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grid.ColumnHeadersHeight = 34; this.grid.RowTemplate.Height = 28;
            this.grid.DoubleClick += new System.EventHandler(this.Grid_DoubleClick);
            this.grid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            // topBar
            this.topBar.Dock = System.Windows.Forms.DockStyle.Top; this.topBar.Height = 44;
            this.topBar.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.topBar.Controls.Add(this.lblB);
            this.topBar.Controls.Add(this.txtBusca);
            this.topBar.Controls.Add(this.btnB);
            this.topBar.Controls.Add(this.btnN);
            this.topBar.Controls.Add(this.btnSel);
            // lblB
            this.lblB.Text = "Buscar:"; this.lblB.ForeColor = System.Drawing.Color.White; this.lblB.Left = 8; this.lblB.Top = 12; this.lblB.AutoSize = true;
            // txtBusca
            this.txtBusca.Left = 65; this.txtBusca.Top = 8; this.txtBusca.Width = 220;
            this.txtBusca.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtBusca_KeyDown);
            // btnB
            this.btnB.Text = "Buscar"; this.btnB.Left = 295; this.btnB.Top = 8; this.btnB.Width = 80; this.btnB.Height = 28;
            this.btnB.BackColor = System.Drawing.Color.FromArgb(63, 81, 181); this.btnB.ForeColor = System.Drawing.Color.White;
            this.btnB.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnB.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnB.FlatAppearance.BorderSize = 0; this.btnB.Click += new System.EventHandler(this.BtnBuscar_Click);
            // btnN
            this.btnN.Text = "+ Novo"; this.btnN.Left = 385; this.btnN.Top = 8; this.btnN.Width = 80; this.btnN.Height = 28;
            this.btnN.BackColor = System.Drawing.Color.FromArgb(0, 150, 136); this.btnN.ForeColor = System.Drawing.Color.White;
            this.btnN.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnN.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnN.FlatAppearance.BorderSize = 0; this.btnN.Click += new System.EventHandler(this.BtnNovo_Click);
            // btnSel
            this.btnSel.Text = "\u2714 Selecionar"; this.btnSel.Left = 475; this.btnSel.Top = 8; this.btnSel.Width = 110; this.btnSel.Height = 28;
            this.btnSel.BackColor = System.Drawing.Color.FromArgb(33, 150, 243); this.btnSel.ForeColor = System.Drawing.Color.White;
            this.btnSel.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnSel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSel.FlatAppearance.BorderSize = 0; this.btnSel.Click += new System.EventHandler(this.BtnSelecionar_Click);
            // pnlForm
            this.pnlForm.Dock = System.Windows.Forms.DockStyle.Bottom; this.pnlForm.Height = 110;
            this.pnlForm.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.pnlForm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlForm.Visible = false;
            this.pnlForm.Controls.Add(this.lblNome);    this.pnlForm.Controls.Add(this.txtNomeCad);
            this.pnlForm.Controls.Add(this.lblCpf);     this.pnlForm.Controls.Add(this.txtCpfCad);
            this.pnlForm.Controls.Add(this.lblSit);     this.pnlForm.Controls.Add(this.cmbSituacao);
            this.pnlForm.Controls.Add(this.lblTel);     this.pnlForm.Controls.Add(this.txtTelefoneCad);
            this.pnlForm.Controls.Add(this.lblCel);     this.pnlForm.Controls.Add(this.txtCelularCad);
            this.pnlForm.Controls.Add(this.lblEmail);   this.pnlForm.Controls.Add(this.txtEmailCad);
            this.pnlForm.Controls.Add(this.btnS);
            this.pnlForm.Controls.Add(this.btnC);
            // row 1
            this.lblNome.Text="Nome / Razao Social:"; this.lblNome.AutoSize=true; this.lblNome.Left=10; this.lblNome.Top=11;
            this.txtNomeCad.Left=150; this.txtNomeCad.Top=8; this.txtNomeCad.Width=240;
            this.lblCpf.Text="CPF / CNPJ:"; this.lblCpf.AutoSize=true; this.lblCpf.Left=402; this.lblCpf.Top=11;
            this.txtCpfCad.Left=476; this.txtCpfCad.Top=8; this.txtCpfCad.Width=140;
            this.lblSit.Text="Situacao:"; this.lblSit.AutoSize=true; this.lblSit.Left=628; this.lblSit.Top=11;
            this.cmbSituacao.Left=682; this.cmbSituacao.Top=8; this.cmbSituacao.Width=90;
            this.cmbSituacao.DropDownStyle=System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSituacao.Items.AddRange(new object[]{"Ativo","Inativo"}); this.cmbSituacao.SelectedIndex=0;
            // row 2
            this.lblTel.Text="Telefone:"; this.lblTel.AutoSize=true; this.lblTel.Left=10; this.lblTel.Top=43;
            this.txtTelefoneCad.Mask="(00) 0000-0000"; this.txtTelefoneCad.Left=72; this.txtTelefoneCad.Top=40; this.txtTelefoneCad.Width=125;
            this.lblCel.Text="Celular:"; this.lblCel.AutoSize=true; this.lblCel.Left=208; this.lblCel.Top=43;
            this.txtCelularCad.Mask="(00) 00000-0000"; this.txtCelularCad.Left=255; this.txtCelularCad.Top=40; this.txtCelularCad.Width=135;
            this.lblEmail.Text="E-mail:"; this.lblEmail.AutoSize=true; this.lblEmail.Left=402; this.lblEmail.Top=43;
            this.txtEmailCad.Left=443; this.txtEmailCad.Top=40; this.txtEmailCad.Width=230;
            // buttons
            this.btnS.Text="Salvar"; this.btnS.Left=10; this.btnS.Top=72; this.btnS.Width=100; this.btnS.Height=28;
            this.btnS.BackColor=System.Drawing.Color.FromArgb(33,150,243); this.btnS.ForeColor=System.Drawing.Color.White;
            this.btnS.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnS.Font=new System.Drawing.Font("Segoe UI",9F,System.Drawing.FontStyle.Bold);
            this.btnS.FlatAppearance.BorderSize=0; this.btnS.Click+=new System.EventHandler(this.BtnSalvarCad_Click);
            this.btnC.Text="Cancelar"; this.btnC.Left=120; this.btnC.Top=72; this.btnC.Width=100; this.btnC.Height=28;
            this.btnC.BackColor=System.Drawing.Color.FromArgb(158,158,158); this.btnC.ForeColor=System.Drawing.Color.White;
            this.btnC.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnC.Font=new System.Drawing.Font("Segoe UI",9F,System.Drawing.FontStyle.Bold);
            this.btnC.FlatAppearance.BorderSize=0; this.btnC.Click+=new System.EventHandler(this.BtnCancelarCad_Click);
            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 500);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.MinimumSize = new System.Drawing.Size(700, 400);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Selecionar Cliente";
            this.Controls.Add(this.grid);
            this.Controls.Add(this.topBar);
            this.Controls.Add(this.pnlForm);
            this.ResumeLayout(false);
        }

        private  System.Windows.Forms.DataGridView  grid;
        private  System.Windows.Forms.Panel         pnlForm;
        internal System.Windows.Forms.TextBox        txtBusca;
        private  System.Windows.Forms.TextBox        txtNomeCad;
        private  System.Windows.Forms.MaskedTextBox  txtTelefoneCad;
        private  System.Windows.Forms.MaskedTextBox  txtCelularCad;
        private  System.Windows.Forms.TextBox        txtEmailCad;
        private  System.Windows.Forms.TextBox        txtCpfCad;
        private  System.Windows.Forms.ComboBox       cmbSituacao;
        internal System.Windows.Forms.Button         btnB;
        internal System.Windows.Forms.Button         btnN;
        internal System.Windows.Forms.Button         btnSel;
        internal System.Windows.Forms.Button         btnS;
        internal System.Windows.Forms.Button         btnC;
        private  System.Windows.Forms.Panel          topBar;
        private  System.Windows.Forms.Label          lblB;
        private  System.Windows.Forms.Label          lblNome;
        private  System.Windows.Forms.Label          lblCpf;
        private  System.Windows.Forms.Label          lblSit;
        private  System.Windows.Forms.Label          lblTel;
        private  System.Windows.Forms.Label          lblCel;
        private  System.Windows.Forms.Label          lblEmail;

        private void Grid_DataError(object s, System.Windows.Forms.DataGridViewDataErrorEventArgs e) { e.ThrowException = false; }
    }
}
