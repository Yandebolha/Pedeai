using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmCadastroCliente
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.grid = new System.Windows.Forms.DataGridView();
            this.topBar = new System.Windows.Forms.Panel();
            this.lblB = new System.Windows.Forms.Label();
            this.txtBusca = new System.Windows.Forms.TextBox();
            this.btnB = new System.Windows.Forms.Button();
            this.btnN = new System.Windows.Forms.Button();
            this.pnlForm = new System.Windows.Forms.Panel();
            this.lblNome = new System.Windows.Forms.Label();
            this.txtNome = new System.Windows.Forms.TextBox();
            this.lblCpf = new System.Windows.Forms.Label();
            this.txtCpf = new System.Windows.Forms.TextBox();
            this.lblSit = new System.Windows.Forms.Label();
            this.cmbSituacao = new System.Windows.Forms.ComboBox();
            this.lblCel = new System.Windows.Forms.Label();
            this.txtCelular = new System.Windows.Forms.MaskedTextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblCep = new System.Windows.Forms.Label();
            this.txtCep = new System.Windows.Forms.MaskedTextBox();
            this.lblEnd = new System.Windows.Forms.Label();
            this.txtEndereco = new System.Windows.Forms.TextBox();
            this.lblNum = new System.Windows.Forms.Label();
            this.txtNumero = new System.Windows.Forms.TextBox();
            this.lblComp = new System.Windows.Forms.Label();
            this.txtComplemento = new System.Windows.Forms.TextBox();
            this.lblBai = new System.Windows.Forms.Label();
            this.txtBairro = new System.Windows.Forms.TextBox();
            this.lblCid = new System.Windows.Forms.Label();
            this.txtCidade = new System.Windows.Forms.TextBox();
            this.lblUF = new System.Windows.Forms.Label();
            this.txtEstado = new System.Windows.Forms.TextBox();
            this.pnlBtns = new System.Windows.Forms.Panel();
            this.btnS = new System.Windows.Forms.Button();
            this.btnC = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.topBar.SuspendLayout();
            this.pnlForm.SuspendLayout();
            this.pnlBtns.SuspendLayout();
            this.SuspendLayout();
            // 
            // grid
            // 
            this.grid.AllowUserToAddRows = false;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(237)))), ((int)(((byte)(216)))));
            this.grid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            this.grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grid.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(245)))), ((int)(((byte)(238)))));
            this.grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(230)))), ((int)(((byte)(202)))));
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.grid.ColumnHeadersHeight = 34;
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(113)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grid.DefaultCellStyle = dataGridViewCellStyle9;
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.grid.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(185)))), ((int)(((byte)(160)))));
            this.grid.Location = new System.Drawing.Point(0, 51);
            this.grid.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grid.Name = "grid";
            this.grid.ReadOnly = true;
            this.grid.RowHeadersVisible = false;
            this.grid.RowTemplate.Height = 28;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.Size = new System.Drawing.Size(1003, 325);
            this.grid.TabIndex = 0;
            this.grid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            this.grid.DoubleClick += new System.EventHandler(this.Grid_DoubleClick);
            // 
            // topBar
            // 
            this.topBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(110)))), ((int)(((byte)(42)))));
            this.topBar.Controls.Add(this.lblB);
            this.topBar.Controls.Add(this.txtBusca);
            this.topBar.Controls.Add(this.btnB);
            this.topBar.Controls.Add(this.btnN);
            this.topBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.topBar.Location = new System.Drawing.Point(0, 0);
            this.topBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.topBar.Name = "topBar";
            this.topBar.Size = new System.Drawing.Size(1003, 51);
            this.topBar.TabIndex = 1;
            // 
            // lblB
            // 
            this.lblB.AutoSize = true;
            this.lblB.ForeColor = System.Drawing.Color.White;
            this.lblB.Location = new System.Drawing.Point(9, 14);
            this.lblB.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblB.Name = "lblB";
            this.lblB.Size = new System.Drawing.Size(45, 15);
            this.lblB.TabIndex = 0;
            this.lblB.Text = "Buscar:";
            // 
            // txtBusca
            // 
            this.txtBusca.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(237)))), ((int)(((byte)(216)))));
            this.txtBusca.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtBusca.Location = new System.Drawing.Point(76, 9);
            this.txtBusca.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtBusca.Name = "txtBusca";
            this.txtBusca.Size = new System.Drawing.Size(256, 23);
            this.txtBusca.TabIndex = 1;
            this.txtBusca.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtBusca_KeyDown);
            // 
            // btnB
            // 
            this.btnB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(113)))), ((int)(((byte)(42)))));
            this.btnB.FlatAppearance.BorderSize = 0;
            this.btnB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnB.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnB.ForeColor = System.Drawing.Color.White;
            this.btnB.Location = new System.Drawing.Point(344, 9);
            this.btnB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnB.Name = "btnB";
            this.btnB.Size = new System.Drawing.Size(117, 32);
            this.btnB.TabIndex = 2;
            this.btnB.Text = "Buscar";
            this.btnB.UseVisualStyleBackColor = false;
            this.btnB.Click += new System.EventHandler(this.BtnBuscar_Click);
            // 
            // btnN
            // 
            this.btnN.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(120)))), ((int)(((byte)(38)))));
            this.btnN.FlatAppearance.BorderSize = 0;
            this.btnN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnN.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnN.ForeColor = System.Drawing.Color.White;
            this.btnN.Location = new System.Drawing.Point(472, 9);
            this.btnN.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnN.Name = "btnN";
            this.btnN.Size = new System.Drawing.Size(140, 32);
            this.btnN.TabIndex = 3;
            this.btnN.Text = "+ Novo Cliente";
            this.btnN.UseVisualStyleBackColor = false;
            this.btnN.Click += new System.EventHandler(this.BtnNovo_Click);
            // 
            // pnlForm
            // 
            this.pnlForm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(237)))), ((int)(((byte)(216)))));
            this.pnlForm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlForm.Controls.Add(this.lblNome);
            this.pnlForm.Controls.Add(this.txtNome);
            this.pnlForm.Controls.Add(this.lblCpf);
            this.pnlForm.Controls.Add(this.txtCpf);
            this.pnlForm.Controls.Add(this.lblSit);
            this.pnlForm.Controls.Add(this.cmbSituacao);
            this.pnlForm.Controls.Add(this.lblCel);
            this.pnlForm.Controls.Add(this.txtCelular);
            this.pnlForm.Controls.Add(this.lblEmail);
            this.pnlForm.Controls.Add(this.txtEmail);
            this.pnlForm.Controls.Add(this.lblCep);
            this.pnlForm.Controls.Add(this.txtCep);
            this.pnlForm.Controls.Add(this.lblEnd);
            this.pnlForm.Controls.Add(this.txtEndereco);
            this.pnlForm.Controls.Add(this.lblNum);
            this.pnlForm.Controls.Add(this.txtNumero);
            this.pnlForm.Controls.Add(this.lblComp);
            this.pnlForm.Controls.Add(this.txtComplemento);
            this.pnlForm.Controls.Add(this.lblBai);
            this.pnlForm.Controls.Add(this.txtBairro);
            this.pnlForm.Controls.Add(this.lblCid);
            this.pnlForm.Controls.Add(this.txtCidade);
            this.pnlForm.Controls.Add(this.lblUF);
            this.pnlForm.Controls.Add(this.txtEstado);
            this.pnlForm.Controls.Add(this.pnlBtns);
            this.pnlForm.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlForm.Location = new System.Drawing.Point(0, 376);
            this.pnlForm.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlForm.Name = "pnlForm";
            this.pnlForm.Size = new System.Drawing.Size(1003, 236);
            this.pnlForm.TabIndex = 2;
            this.pnlForm.Visible = false;
            // 
            // lblNome
            // 
            this.lblNome.AutoSize = true;
            this.lblNome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(80)))), ((int)(((byte)(120)))));
            this.lblNome.Location = new System.Drawing.Point(12, 13);
            this.lblNome.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNome.Name = "lblNome";
            this.lblNome.Size = new System.Drawing.Size(119, 15);
            this.lblNome.TabIndex = 0;
            this.lblNome.Text = "Nome / Razao Social:";
            // 
            // txtNome
            // 
            this.txtNome.BackColor = System.Drawing.Color.White;
            this.txtNome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtNome.Location = new System.Drawing.Point(139, 9);
            this.txtNome.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtNome.Name = "txtNome";
            this.txtNome.Size = new System.Drawing.Size(297, 23);
            this.txtNome.TabIndex = 1;
            // 
            // lblCpf
            // 
            this.lblCpf.AutoSize = true;
            this.lblCpf.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(80)))), ((int)(((byte)(120)))));
            this.lblCpf.Location = new System.Drawing.Point(447, 13);
            this.lblCpf.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCpf.Name = "lblCpf";
            this.lblCpf.Size = new System.Drawing.Size(69, 15);
            this.lblCpf.TabIndex = 2;
            this.lblCpf.Text = "CPF / CNPJ:";
            // 
            // txtCpf
            // 
            this.txtCpf.BackColor = System.Drawing.Color.White;
            this.txtCpf.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtCpf.Location = new System.Drawing.Point(524, 9);
            this.txtCpf.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtCpf.Name = "txtCpf";
            this.txtCpf.Size = new System.Drawing.Size(186, 23);
            this.txtCpf.TabIndex = 3;
            this.txtCpf.TextChanged += new System.EventHandler(this.TxtCpf_TextChanged);
            // 
            // lblSit
            // 
            this.lblSit.AutoSize = true;
            this.lblSit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(80)))), ((int)(((byte)(120)))));
            this.lblSit.Location = new System.Drawing.Point(735, 13);
            this.lblSit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSit.Name = "lblSit";
            this.lblSit.Size = new System.Drawing.Size(55, 15);
            this.lblSit.TabIndex = 4;
            this.lblSit.Text = "Situacao:";
            // 
            // cmbSituacao
            // 
            this.cmbSituacao.BackColor = System.Drawing.Color.White;
            this.cmbSituacao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSituacao.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.cmbSituacao.Items.AddRange(new object[] {
            "Ativo",
            "Inativo"});
            this.cmbSituacao.Location = new System.Drawing.Point(803, 9);
            this.cmbSituacao.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbSituacao.Name = "cmbSituacao";
            this.cmbSituacao.Size = new System.Drawing.Size(110, 23);
            this.cmbSituacao.TabIndex = 5;
            // 
            // lblCel
            // 
            this.lblCel.AutoSize = true;
            this.lblCel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(80)))), ((int)(((byte)(120)))));
            this.lblCel.Location = new System.Drawing.Point(12, 54);
            this.lblCel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCel.Name = "lblCel";
            this.lblCel.Size = new System.Drawing.Size(47, 15);
            this.lblCel.TabIndex = 6;
            this.lblCel.Text = "Celular:";
            // 
            // txtCelular
            // 
            this.txtCelular.BackColor = System.Drawing.Color.White;
            this.txtCelular.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtCelular.Location = new System.Drawing.Point(84, 51);
            this.txtCelular.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtCelular.Mask = "(00) 00000-0000";
            this.txtCelular.Name = "txtCelular";
            this.txtCelular.Size = new System.Drawing.Size(163, 23);
            this.txtCelular.TabIndex = 7;
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(80)))), ((int)(((byte)(120)))));
            this.lblEmail.Location = new System.Drawing.Point(260, 54);
            this.lblEmail.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(44, 15);
            this.lblEmail.TabIndex = 8;
            this.lblEmail.Text = "E-mail:";
            // 
            // txtEmail
            // 
            this.txtEmail.BackColor = System.Drawing.Color.White;
            this.txtEmail.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtEmail.Location = new System.Drawing.Point(309, 51);
            this.txtEmail.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(308, 23);
            this.txtEmail.TabIndex = 9;
            // 
            // lblCep
            // 
            this.lblCep.AutoSize = true;
            this.lblCep.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(80)))), ((int)(((byte)(120)))));
            this.lblCep.Location = new System.Drawing.Point(641, 55);
            this.lblCep.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCep.Name = "lblCep";
            this.lblCep.Size = new System.Drawing.Size(31, 15);
            this.lblCep.TabIndex = 10;
            this.lblCep.Text = "CEP:";
            this.lblCep.Click += new System.EventHandler(this.lblCep_Click);
            // 
            // txtCep
            // 
            this.txtCep.BackColor = System.Drawing.Color.White;
            this.txtCep.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtCep.Location = new System.Drawing.Point(680, 51);
            this.txtCep.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtCep.Mask = "00000-000";
            this.txtCep.Name = "txtCep";
            this.txtCep.Size = new System.Drawing.Size(110, 23);
            this.txtCep.TabIndex = 11;
            this.txtCep.MaskInputRejected += new System.Windows.Forms.MaskInputRejectedEventHandler(this.txtCep_MaskInputRejected);
            this.txtCep.Leave += new System.EventHandler(this.TxtCep_Leave);
            // 
            // lblEnd
            // 
            this.lblEnd.AutoSize = true;
            this.lblEnd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(80)))), ((int)(((byte)(120)))));
            this.lblEnd.Location = new System.Drawing.Point(11, 92);
            this.lblEnd.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEnd.Name = "lblEnd";
            this.lblEnd.Size = new System.Drawing.Size(59, 15);
            this.lblEnd.TabIndex = 12;
            this.lblEnd.Text = "Endereco:";
            this.lblEnd.Click += new System.EventHandler(this.lblEnd_Click);
            // 
            // txtEndereco
            // 
            this.txtEndereco.BackColor = System.Drawing.Color.White;
            this.txtEndereco.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtEndereco.Location = new System.Drawing.Point(88, 88);
            this.txtEndereco.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtEndereco.Name = "txtEndereco";
            this.txtEndereco.Size = new System.Drawing.Size(262, 23);
            this.txtEndereco.TabIndex = 13;
            // 
            // lblNum
            // 
            this.lblNum.AutoSize = true;
            this.lblNum.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(80)))), ((int)(((byte)(120)))));
            this.lblNum.Location = new System.Drawing.Point(370, 92);
            this.lblNum.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNum.Name = "lblNum";
            this.lblNum.Size = new System.Drawing.Size(24, 15);
            this.lblNum.TabIndex = 14;
            this.lblNum.Text = "Nº:";
            // 
            // txtNumero
            // 
            this.txtNumero.BackColor = System.Drawing.Color.White;
            this.txtNumero.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtNumero.Location = new System.Drawing.Point(393, 88);
            this.txtNumero.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtNumero.Name = "txtNumero";
            this.txtNumero.Size = new System.Drawing.Size(67, 23);
            this.txtNumero.TabIndex = 15;
            // 
            // lblComp
            // 
            this.lblComp.AutoSize = true;
            this.lblComp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(80)))), ((int)(((byte)(120)))));
            this.lblComp.Location = new System.Drawing.Point(469, 92);
            this.lblComp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblComp.Name = "lblComp";
            this.lblComp.Size = new System.Drawing.Size(49, 15);
            this.lblComp.TabIndex = 16;
            this.lblComp.Text = "Compl.:";
            // 
            // txtComplemento
            // 
            this.txtComplemento.BackColor = System.Drawing.Color.White;
            this.txtComplemento.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtComplemento.Location = new System.Drawing.Point(524, 88);
            this.txtComplemento.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtComplemento.Name = "txtComplemento";
            this.txtComplemento.Size = new System.Drawing.Size(151, 23);
            this.txtComplemento.TabIndex = 17;
            // 
            // lblBai
            // 
            this.lblBai.AutoSize = true;
            this.lblBai.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(80)))), ((int)(((byte)(120)))));
            this.lblBai.Location = new System.Drawing.Point(683, 92);
            this.lblBai.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBai.Name = "lblBai";
            this.lblBai.Size = new System.Drawing.Size(41, 15);
            this.lblBai.TabIndex = 18;
            this.lblBai.Text = "Bairro:";
            // 
            // txtBairro
            // 
            this.txtBairro.BackColor = System.Drawing.Color.White;
            this.txtBairro.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtBairro.Location = new System.Drawing.Point(739, 89);
            this.txtBairro.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtBairro.Name = "txtBairro";
            this.txtBairro.Size = new System.Drawing.Size(219, 23);
            this.txtBairro.TabIndex = 19;
            // 
            // lblCid
            // 
            this.lblCid.AutoSize = true;
            this.lblCid.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(80)))), ((int)(((byte)(120)))));
            this.lblCid.Location = new System.Drawing.Point(12, 132);
            this.lblCid.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCid.Name = "lblCid";
            this.lblCid.Size = new System.Drawing.Size(47, 15);
            this.lblCid.TabIndex = 20;
            this.lblCid.Text = "Cidade:";
            // 
            // txtCidade
            // 
            this.txtCidade.BackColor = System.Drawing.Color.White;
            this.txtCidade.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtCidade.Location = new System.Drawing.Point(68, 129);
            this.txtCidade.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtCidade.Name = "txtCidade";
            this.txtCidade.Size = new System.Drawing.Size(219, 23);
            this.txtCidade.TabIndex = 21;
            // 
            // lblUF
            // 
            this.lblUF.AutoSize = true;
            this.lblUF.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(80)))), ((int)(((byte)(120)))));
            this.lblUF.Location = new System.Drawing.Point(313, 135);
            this.lblUF.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUF.Name = "lblUF";
            this.lblUF.Size = new System.Drawing.Size(24, 15);
            this.lblUF.TabIndex = 22;
            this.lblUF.Text = "UF:";
            // 
            // txtEstado
            // 
            this.txtEstado.BackColor = System.Drawing.Color.White;
            this.txtEstado.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtEstado.Location = new System.Drawing.Point(336, 132);
            this.txtEstado.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtEstado.Name = "txtEstado";
            this.txtEstado.Size = new System.Drawing.Size(58, 23);
            this.txtEstado.TabIndex = 23;
            // 
            // pnlBtns
            // 
            this.pnlBtns.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(237)))), ((int)(((byte)(216)))));
            this.pnlBtns.Controls.Add(this.btnS);
            this.pnlBtns.Controls.Add(this.btnC);
            this.pnlBtns.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBtns.Location = new System.Drawing.Point(0, 179);
            this.pnlBtns.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlBtns.Name = "pnlBtns";
            this.pnlBtns.Size = new System.Drawing.Size(1001, 55);
            this.pnlBtns.TabIndex = 24;
            this.pnlBtns.SizeChanged += new System.EventHandler(this.PnlBtns_SizeChanged);
            // 
            // btnS
            // 
            this.btnS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(120)))), ((int)(((byte)(38)))));
            this.btnS.FlatAppearance.BorderSize = 0;
            this.btnS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnS.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnS.ForeColor = System.Drawing.Color.White;
            this.btnS.Location = new System.Drawing.Point(12, 12);
            this.btnS.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnS.Name = "btnS";
            this.btnS.Size = new System.Drawing.Size(128, 32);
            this.btnS.TabIndex = 0;
            this.btnS.Text = "Salvar";
            this.btnS.UseVisualStyleBackColor = false;
            this.btnS.Click += new System.EventHandler(this.BtnSalvar_Click);
            // 
            // btnC
            // 
            this.btnC.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(113)))), ((int)(((byte)(42)))));
            this.btnC.FlatAppearance.BorderSize = 0;
            this.btnC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnC.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnC.ForeColor = System.Drawing.Color.White;
            this.btnC.Location = new System.Drawing.Point(152, 12);
            this.btnC.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnC.Name = "btnC";
            this.btnC.Size = new System.Drawing.Size(128, 32);
            this.btnC.TabIndex = 1;
            this.btnC.Text = "Cancelar";
            this.btnC.UseVisualStyleBackColor = false;
            this.btnC.Click += new System.EventHandler(this.BtnCancelarCliente_Click);
            // 
            // frmCadastroCliente
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(237)))), ((int)(((byte)(216)))));
            this.ClientSize = new System.Drawing.Size(1003, 612);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.topBar);
            this.Controls.Add(this.pnlForm);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(907, 525);
            this.Name = "frmCadastroCliente";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cadastro de Clientes";
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.topBar.ResumeLayout(false);
            this.topBar.PerformLayout();
            this.pnlForm.ResumeLayout(false);
            this.pnlForm.PerformLayout();
            this.pnlBtns.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        internal System.Windows.Forms.DataGridView   grid;
        internal System.Windows.Forms.Panel          pnlForm;
        internal System.Windows.Forms.TextBox        txtBusca;
        internal System.Windows.Forms.TextBox        txtNome;
        internal System.Windows.Forms.TextBox        txtEmail;
        internal System.Windows.Forms.TextBox        txtCpf;
        internal System.Windows.Forms.TextBox        txtEndereco;
        internal System.Windows.Forms.TextBox        txtNumero;
        internal System.Windows.Forms.TextBox        txtComplemento;
        internal System.Windows.Forms.TextBox        txtBairro;
        internal System.Windows.Forms.TextBox        txtCidade;
        internal System.Windows.Forms.TextBox        txtEstado;
        internal System.Windows.Forms.MaskedTextBox  txtCelular;
        internal System.Windows.Forms.MaskedTextBox  txtCep;
        internal System.Windows.Forms.ComboBox       cmbSituacao;
        internal System.Windows.Forms.Button         btnB;
        internal System.Windows.Forms.Button         btnN;
        internal System.Windows.Forms.Button         btnS;
        internal System.Windows.Forms.Button         btnC;
        private  System.Windows.Forms.Panel          topBar;
        private  System.Windows.Forms.Panel          pnlBtns;
        private  System.Windows.Forms.Label          lblB;
        private  System.Windows.Forms.Label          lblNome;
        private  System.Windows.Forms.Label          lblCpf;
        private  System.Windows.Forms.Label          lblSit;
        private  System.Windows.Forms.Label          lblCel;
        private  System.Windows.Forms.Label          lblEmail;
        private  System.Windows.Forms.Label          lblCep;
        private  System.Windows.Forms.Label          lblEnd;
        private  System.Windows.Forms.Label          lblNum;
        private  System.Windows.Forms.Label          lblComp;
        private  System.Windows.Forms.Label          lblBai;
        private  System.Windows.Forms.Label          lblCid;
        private  System.Windows.Forms.Label          lblUF;

        private void Grid_DoubleClick(object s, System.EventArgs e) { CarregarParaEditar(); }
        private void Grid_DataError(object s, System.Windows.Forms.DataGridViewDataErrorEventArgs e) { e.ThrowException = false; }
        private void TxtBusca_KeyDown(object s, System.Windows.Forms.KeyEventArgs e) { if (e.KeyCode == System.Windows.Forms.Keys.Enter) CarregarGrid(); }
        private void BtnBuscar_Click(object s, System.EventArgs e) { CarregarGrid(); }
        private void BtnNovo_Click(object s, System.EventArgs e)   { ModoNovo(); }
        private void BtnCancelarCliente_Click(object s, System.EventArgs e) { pnlForm.Visible = false; _codigoEditando = 0; }
        private void PnlBtns_SizeChanged(object s, System.EventArgs e)
        {
            int x = (pnlBtns.Width - 110 * 2 - 10) / 2;
            if (x < 10) x = 10;
            btnS.Left = x; btnC.Left = x + 120;
        }
    }
}
