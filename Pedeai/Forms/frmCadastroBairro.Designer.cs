using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmCadastroBairro
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
            this.lblBusca    = new System.Windows.Forms.Label();
            this.txtBusca    = new System.Windows.Forms.TextBox();
            this.lblCidFiltro = new System.Windows.Forms.Label();
            this.txtCidFiltro = new System.Windows.Forms.TextBox();
            this.btnPesquisar = new System.Windows.Forms.Button();
            this.pnlForm     = new System.Windows.Forms.Panel();
            this.pnlBtns     = new System.Windows.Forms.Panel();
            this.btnS        = new System.Windows.Forms.Button();
            this.btnC        = new System.Windows.Forms.Button();
            this.btnD        = new System.Windows.Forms.Button();
            this.lblCidade   = new System.Windows.Forms.Label();
            this.txtCidade   = new System.Windows.Forms.TextBox();
            this.lblBairro   = new System.Windows.Forms.Label();
            this.txtBairro   = new System.Windows.Forms.TextBox();
            this.lblTaxa     = new System.Windows.Forms.Label();
            this.numTaxa     = new System.Windows.Forms.NumericUpDown();
            this.lblSit      = new System.Windows.Forms.Label();
            this.cmbSituacao = new System.Windows.Forms.ComboBox();
            this.lblCEP      = new System.Windows.Forms.Label();
            this.txtCEP      = new System.Windows.Forms.TextBox();
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
            this.topBar.Controls.Add(this.lblBusca);
            this.topBar.Controls.Add(this.txtBusca);
            this.topBar.Controls.Add(this.lblCidFiltro);
            this.topBar.Controls.Add(this.txtCidFiltro);
            this.topBar.Controls.Add(this.btnPesquisar);

            // btnNovo
            this.btnNovo.Text = "+ Novo";
            this.btnNovo.Left = 8;
            this.btnNovo.Top = 8;
            this.btnNovo.Width = 90;
            this.btnNovo.Height = 28;
            this.btnNovo.BackColor = System.Drawing.Color.FromArgb(87, 120, 38);
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
            this.btnEditar.BackColor = System.Drawing.Color.FromArgb(230, 126, 34);
            this.btnEditar.ForeColor = System.Drawing.Color.White;
            this.btnEditar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnEditar.FlatAppearance.BorderSize = 0;
            this.btnEditar.Click += new System.EventHandler(this.BtnEditar_Click);

            // lblBusca
            this.lblBusca.Text      = "🔍 Pesquisar bairro:";
            this.lblBusca.Left      = 218;
            this.lblBusca.Top       = 14;
            this.lblBusca.AutoSize  = true;
            this.lblBusca.Font      = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblBusca.ForeColor = System.Drawing.Color.White;

            // txtBusca
            this.txtBusca.Left        = 338;
            this.txtBusca.Top         = 10;
            this.txtBusca.Width       = 160;
            this.txtBusca.Height      = 24;
            this.txtBusca.Font        = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtBusca.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBusca.BackColor   = System.Drawing.Color.White;
            this.txtBusca.PlaceholderText = "Digite o nome do bairro...";
            this.txtBusca.TextChanged += new System.EventHandler(this.TxtBusca_TextChanged);

            // lblCidFiltro
            this.lblCidFiltro.Text      = "Cidade:";
            this.lblCidFiltro.Left      = 508;
            this.lblCidFiltro.Top       = 14;
            this.lblCidFiltro.AutoSize  = true;
            this.lblCidFiltro.Font      = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblCidFiltro.ForeColor = System.Drawing.Color.White;

            // txtCidFiltro
            this.txtCidFiltro.Left        = 556;
            this.txtCidFiltro.Top         = 10;
            this.txtCidFiltro.Width       = 120;
            this.txtCidFiltro.Height      = 24;
            this.txtCidFiltro.Font        = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtCidFiltro.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCidFiltro.BackColor   = System.Drawing.Color.White;
            this.txtCidFiltro.PlaceholderText = "Filtrar por cidade...";

            // btnPesquisar
            this.btnPesquisar.Text = "\U0001F50D Pesquisar";
            this.btnPesquisar.Left = 684;
            this.btnPesquisar.Top  = 8;
            this.btnPesquisar.Width  = 104;
            this.btnPesquisar.Height = 28;
            this.btnPesquisar.BackColor = System.Drawing.Color.FromArgb(224, 113, 42);
            this.btnPesquisar.ForeColor = System.Drawing.Color.White;
            this.btnPesquisar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPesquisar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnPesquisar.FlatAppearance.BorderSize = 0;
            this.btnPesquisar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPesquisar.Click += new System.EventHandler(this.BtnPesquisar_Click);

            // pnlForm
            this.pnlForm.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlForm.Height = 120;
            this.pnlForm.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.pnlForm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlForm.Visible = false;
            this.pnlForm.Controls.Add(this.lblCidade);
            this.pnlForm.Controls.Add(this.txtCidade);
            this.pnlForm.Controls.Add(this.lblBairro);
            this.pnlForm.Controls.Add(this.txtBairro);
            this.pnlForm.Controls.Add(this.lblTaxa);
            this.pnlForm.Controls.Add(this.numTaxa);
            this.pnlForm.Controls.Add(this.lblSit);
            this.pnlForm.Controls.Add(this.cmbSituacao);
            this.pnlForm.Controls.Add(this.lblCEP);
            this.pnlForm.Controls.Add(this.txtCEP);
            this.pnlForm.Controls.Add(this.pnlBtns);

            // pnlBtns
            this.pnlBtns.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBtns.Height = 44;
            this.pnlBtns.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.pnlBtns.Controls.Add(this.btnS);
            this.pnlBtns.Controls.Add(this.btnC);
            this.pnlBtns.Controls.Add(this.btnD);

            // btnS (Salvar)
            this.btnS.Text = "\u2714 Salvar";
            this.btnS.Left = 8;
            this.btnS.Top = 8;
            this.btnS.Width = 90;
            this.btnS.Height = 28;
            this.btnS.BackColor = System.Drawing.Color.FromArgb(87, 120, 38);
            this.btnS.ForeColor = System.Drawing.Color.White;
            this.btnS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnS.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnS.FlatAppearance.BorderSize = 0;
            this.btnS.Click += new System.EventHandler(this.BtnSalvar_Click);

            // btnC (Cancelar)
            this.btnC.Text = "Cancelar";
            this.btnC.Left = 108;
            this.btnC.Top = 8;
            this.btnC.Width = 90;
            this.btnC.Height = 28;
            this.btnC.BackColor = System.Drawing.Color.FromArgb(180, 90, 30);
            this.btnC.ForeColor = System.Drawing.Color.White;
            this.btnC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnC.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnC.FlatAppearance.BorderSize = 0;
            this.btnC.Click += new System.EventHandler(this.BtnCancelar_Click);

            // btnD (Desativar)
            this.btnD.Text = "\u26D4 Desativar";
            this.btnD.Left = 208;
            this.btnD.Top = 8;
            this.btnD.Width = 100;
            this.btnD.Height = 28;
            this.btnD.BackColor = System.Drawing.Color.FromArgb(140, 60, 60);
            this.btnD.ForeColor = System.Drawing.Color.White;
            this.btnD.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnD.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnD.FlatAppearance.BorderSize = 0;
            this.btnD.Click += new System.EventHandler(this.BtnDesativar_Click);

            // lblCEP
            this.lblCEP.Text = "CEP";
            this.lblCEP.Left = 8;
            this.lblCEP.Top = 10;
            this.lblCEP.AutoSize = true;
            this.lblCEP.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCEP.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);

            // txtCEP
            this.txtCEP.Left = 8;
            this.txtCEP.Top = 28;
            this.txtCEP.Width = 110;
            this.txtCEP.MaxLength = 9;
            this.txtCEP.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtCEP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCEP.BackColor = System.Drawing.Color.White;
            this.txtCEP.PlaceholderText = "00000-000";
            this.txtCEP.Leave       += new System.EventHandler(this.TxtCEP_Leave);
            this.txtCEP.TextChanged += new System.EventHandler(this.TxtCEP_TextChanged);

            // lblCidade
            this.lblCidade.Text = "Cidade";
            this.lblCidade.Left = 130;
            this.lblCidade.Top = 10;
            this.lblCidade.AutoSize = true;
            this.lblCidade.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCidade.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);

            // txtCidade
            this.txtCidade.Left = 130;
            this.txtCidade.Top = 28;
            this.txtCidade.Width = 180;
            this.txtCidade.Height = 24;
            this.txtCidade.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtCidade.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCidade.BackColor = System.Drawing.Color.White;

            // lblBairro
            this.lblBairro.Text = "Bairro";
            this.lblBairro.Left = 322;
            this.lblBairro.Top = 10;
            this.lblBairro.AutoSize = true;
            this.lblBairro.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblBairro.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);

            // txtBairro
            this.txtBairro.Left = 322;
            this.txtBairro.Top = 28;
            this.txtBairro.Width = 200;
            this.txtBairro.Height = 24;
            this.txtBairro.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBairro.BackColor = System.Drawing.Color.White;
            this.txtBairro.Font = new System.Drawing.Font("Segoe UI", 9.5F);

            // lblTaxa
            this.lblTaxa.Text = "Taxa de Entrega (R$)";
            this.lblTaxa.Left = 534;
            this.lblTaxa.Top = 10;
            this.lblTaxa.AutoSize = true;
            this.lblTaxa.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTaxa.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);

            // numTaxa
            this.numTaxa.Left = 534;
            this.numTaxa.Top = 28;
            this.numTaxa.Width = 120;
            this.numTaxa.Height = 24;
            this.numTaxa.DecimalPlaces = 2;
            this.numTaxa.Maximum = 999M;
            this.numTaxa.BackColor = System.Drawing.Color.White;
            this.numTaxa.Font = new System.Drawing.Font("Segoe UI", 9.5F);

            // lblSit
            this.lblSit.Text = "Situação";
            this.lblSit.Left = 666;
            this.lblSit.Top = 10;
            this.lblSit.AutoSize = true;
            this.lblSit.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSit.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);

            // cmbSituacao
            this.cmbSituacao.Left = 666;
            this.cmbSituacao.Top = 28;
            this.cmbSituacao.Width = 90;
            this.cmbSituacao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSituacao.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cmbSituacao.Items.AddRange(new object[] { "A", "I" });
            this.cmbSituacao.SelectedIndex = 0;

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 520);
            this.KeyPreview = true;
            this.Controls.Add(this.grid);
            this.Controls.Add(this.pnlForm);
            this.Controls.Add(this.topBar);
            this.Text = "Cadastro de Bairros / Taxa de Entrega";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.BackColor = System.Drawing.Color.FromArgb(250, 245, 238);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ResumeLayout(false);
        }

        internal System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.Panel         topBar;
        private System.Windows.Forms.Button        btnNovo;
        private System.Windows.Forms.Button        btnEditar;
        private System.Windows.Forms.Label         lblBusca;
        private System.Windows.Forms.TextBox       txtBusca;
        private System.Windows.Forms.Label         lblCidFiltro;
        private System.Windows.Forms.TextBox       txtCidFiltro;
        private System.Windows.Forms.Button        btnPesquisar;
        private System.Windows.Forms.Panel         pnlForm;
        private System.Windows.Forms.Panel         pnlBtns;
        private System.Windows.Forms.Button        btnS;
        private System.Windows.Forms.Button        btnC;
        private System.Windows.Forms.Button        btnD;
        private System.Windows.Forms.Label         lblCidade;
        private System.Windows.Forms.TextBox       txtCidade;
        private System.Windows.Forms.Label         lblBairro;
        private System.Windows.Forms.TextBox       txtBairro;
        private System.Windows.Forms.Label         lblTaxa;
        private System.Windows.Forms.NumericUpDown numTaxa;
        private System.Windows.Forms.Label         lblSit;
        private System.Windows.Forms.ComboBox      cmbSituacao;
        private System.Windows.Forms.Label         lblCEP;
        private System.Windows.Forms.TextBox       txtCEP;
    }
}
