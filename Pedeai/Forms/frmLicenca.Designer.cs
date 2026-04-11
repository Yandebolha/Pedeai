using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmLicenca
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblInstrucao = new System.Windows.Forms.Label();
            this.lblCodLabel = new System.Windows.Forms.Label();
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.btnCopiar = new System.Windows.Forms.Button();
            this.lblChaveLabel = new System.Windows.Forms.Label();
            this.txtChave = new System.Windows.Forms.TextBox();
            this.btnAtivar = new System.Windows.Forms.Button();
            this.btnSair = new System.Windows.Forms.Button();
            this.btnGraca = new System.Windows.Forms.Button();
            this.lblGracaAviso = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(60)))), ((int)(((byte)(10)))));
            this.lblTitulo.Location = new System.Drawing.Point(0, 18);
            this.lblTitulo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(583, 35);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "🔒  Ativação do Sistema";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblInstrucao
            // 
            this.lblInstrucao.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblInstrucao.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(60)))), ((int)(((byte)(30)))));
            this.lblInstrucao.Location = new System.Drawing.Point(23, 60);
            this.lblInstrucao.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInstrucao.Name = "lblInstrucao";
            this.lblInstrucao.Size = new System.Drawing.Size(537, 46);
            this.lblInstrucao.TabIndex = 1;
            this.lblInstrucao.Text = "O sistema ainda não está ativado.\r\nForneça o Código da Empresa ao suporte para re" +
    "ceber sua chave de ativação.";
            this.lblInstrucao.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCodLabel
            // 
            this.lblCodLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCodLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(50)))), ((int)(((byte)(30)))));
            this.lblCodLabel.Location = new System.Drawing.Point(23, 125);
            this.lblCodLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCodLabel.Name = "lblCodLabel";
            this.lblCodLabel.Size = new System.Drawing.Size(163, 25);
            this.lblCodLabel.TabIndex = 2;
            this.lblCodLabel.Text = "Código da Empresa:";
            // 
            // txtCodigo
            // 
            this.txtCodigo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(230)))), ((int)(((byte)(210)))));
            this.txtCodigo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCodigo.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtCodigo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(60)))), ((int)(((byte)(10)))));
            this.txtCodigo.Location = new System.Drawing.Point(196, 122);
            this.txtCodigo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.ReadOnly = true;
            this.txtCodigo.Size = new System.Drawing.Size(151, 25);
            this.txtCodigo.TabIndex = 0;
            // 
            // btnCopiar
            // 
            this.btnCopiar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(100)))), ((int)(((byte)(160)))));
            this.btnCopiar.FlatAppearance.BorderSize = 0;
            this.btnCopiar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopiar.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnCopiar.ForeColor = System.Drawing.Color.White;
            this.btnCopiar.Location = new System.Drawing.Point(359, 122);
            this.btnCopiar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCopiar.Name = "btnCopiar";
            this.btnCopiar.Size = new System.Drawing.Size(105, 29);
            this.btnCopiar.TabIndex = 1;
            this.btnCopiar.Text = "Copiar";
            this.btnCopiar.UseVisualStyleBackColor = false;
            this.btnCopiar.Click += new System.EventHandler(this.BtnCopiar_Click);
            // 
            // lblChaveLabel
            // 
            this.lblChaveLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblChaveLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(50)))), ((int)(((byte)(30)))));
            this.lblChaveLabel.Location = new System.Drawing.Point(23, 173);
            this.lblChaveLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblChaveLabel.Name = "lblChaveLabel";
            this.lblChaveLabel.Size = new System.Drawing.Size(163, 25);
            this.lblChaveLabel.TabIndex = 3;
            this.lblChaveLabel.Text = "Chave de Ativação:";
            // 
            // txtChave
            // 
            this.txtChave.BackColor = System.Drawing.Color.White;
            this.txtChave.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtChave.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtChave.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtChave.Location = new System.Drawing.Point(196, 171);
            this.txtChave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtChave.Name = "txtChave";
            this.txtChave.PlaceholderText = "20260511-AAAAA-BBBBB-CCCCC";
            this.txtChave.Size = new System.Drawing.Size(352, 23);
            this.txtChave.TabIndex = 2;
            // 
            // btnAtivar
            // 
            this.btnAtivar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(140)))), ((int)(((byte)(38)))));
            this.btnAtivar.FlatAppearance.BorderSize = 0;
            this.btnAtivar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAtivar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnAtivar.ForeColor = System.Drawing.Color.White;
            this.btnAtivar.Location = new System.Drawing.Point(93, 226);
            this.btnAtivar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAtivar.Name = "btnAtivar";
            this.btnAtivar.Size = new System.Drawing.Size(152, 39);
            this.btnAtivar.TabIndex = 3;
            this.btnAtivar.Text = "✔  Ativar";
            this.btnAtivar.UseVisualStyleBackColor = false;
            this.btnAtivar.Click += new System.EventHandler(this.BtnAtivar_Click);
            // 
            // btnSair
            // 
            this.btnSair.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(60)))), ((int)(((byte)(40)))));
            this.btnSair.FlatAppearance.BorderSize = 0;
            this.btnSair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSair.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSair.ForeColor = System.Drawing.Color.White;
            this.btnSair.Location = new System.Drawing.Point(268, 226);
            this.btnSair.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(128, 39);
            this.btnSair.TabIndex = 4;
            this.btnSair.Text = "Sair";
            this.btnSair.UseVisualStyleBackColor = false;
            this.btnSair.Click += new System.EventHandler(this.BtnSair_Click);
            // 
            // btnGraca
            // 
            this.btnGraca.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(110)))), ((int)(((byte)(20)))));
            this.btnGraca.FlatAppearance.BorderSize = 0;
            this.btnGraca.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGraca.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnGraca.ForeColor = System.Drawing.Color.White;
            this.btnGraca.Location = new System.Drawing.Point(93, 309);
            this.btnGraca.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGraca.Name = "btnGraca";
            this.btnGraca.Size = new System.Drawing.Size(385, 39);
            this.btnGraca.TabIndex = 5;
            this.btnGraca.Text = "⏳  Usar Período de Graça";
            this.btnGraca.UseVisualStyleBackColor = false;
            this.btnGraca.Visible = false;
            this.btnGraca.Click += new System.EventHandler(this.BtnGraca_Click);
            // 
            // lblGracaAviso
            // 
            this.lblGracaAviso.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblGracaAviso.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(60)))), ((int)(((byte)(10)))));
            this.lblGracaAviso.Location = new System.Drawing.Point(0, 279);
            this.lblGracaAviso.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGracaAviso.Name = "lblGracaAviso";
            this.lblGracaAviso.Size = new System.Drawing.Size(583, 25);
            this.lblGracaAviso.TabIndex = 5;
            this.lblGracaAviso.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblGracaAviso.Visible = false;
            // 
            // frmLicenca
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(242)))), ((int)(((byte)(225)))));
            this.ClientSize = new System.Drawing.Size(583, 369);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblInstrucao);
            this.Controls.Add(this.lblCodLabel);
            this.Controls.Add(this.txtCodigo);
            this.Controls.Add(this.btnCopiar);
            this.Controls.Add(this.lblChaveLabel);
            this.Controls.Add(this.txtChave);
            this.Controls.Add(this.btnAtivar);
            this.Controls.Add(this.btnSair);
            this.Controls.Add(this.lblGracaAviso);
            this.Controls.Add(this.btnGraca);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLicenca";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ativação do Sistema";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label   lblTitulo;
        private System.Windows.Forms.Label   lblInstrucao;
        private System.Windows.Forms.Label   lblCodLabel;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.Button  btnCopiar;
        private System.Windows.Forms.Label   lblChaveLabel;
        private System.Windows.Forms.TextBox txtChave;
        private System.Windows.Forms.Button  btnAtivar;
        private System.Windows.Forms.Button  btnSair;
        internal System.Windows.Forms.Button  btnGraca;
        internal System.Windows.Forms.Label   lblGracaAviso;
    }
}
