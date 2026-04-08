using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmConsultarPedido
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // 1. Allocate all controls
            this.pnlTop            = new System.Windows.Forms.Panel();
            this.lblTitulo         = new System.Windows.Forms.Label();
            this.pnlBusca          = new System.Windows.Forms.Panel();
            this.lblLblNum         = new System.Windows.Forms.Label();
            this.txtNumPedido      = new System.Windows.Forms.TextBox();
            this.btnBuscar         = new System.Windows.Forms.Button();
            this.pnlInfo           = new System.Windows.Forms.Panel();
            this.lblInfoTit        = new System.Windows.Forms.Label();
            this.pnlInfoSep        = new System.Windows.Forms.Panel();
            this.lblCapCliente     = new System.Windows.Forms.Label();
            this.lblCliente        = new System.Windows.Forms.Label();
            this.lblCapTaxaEnt     = new System.Windows.Forms.Label();
            this.lblTaxaEnt        = new System.Windows.Forms.Label();
            this.lblCapCondPgto    = new System.Windows.Forms.Label();
            this.lblCondPgto       = new System.Windows.Forms.Label();
            this.lblCapValorPedido = new System.Windows.Forms.Label();
            this.lblValorPedido    = new System.Windows.Forms.Label();
            this.lblCapValorPago   = new System.Windows.Forms.Label();
            this.lblValorPago      = new System.Windows.Forms.Label();
            this.lblCapDesconto    = new System.Windows.Forms.Label();
            this.lblDesconto       = new System.Windows.Forms.Label();
            this.lblCapAutorizador = new System.Windows.Forms.Label();
            this.lblAutorizador    = new System.Windows.Forms.Label();
            this.lblItensTitle     = new System.Windows.Forms.Label();
            this.gridItens         = new System.Windows.Forms.DataGridView();
            this.pnlFoot           = new System.Windows.Forms.Panel();
            this.btnFechar         = new System.Windows.Forms.Button();

            // ── pnlTop ──────────────────────────────────────────────────
            this.pnlTop.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Height    = 48;
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.pnlTop.Controls.Add(this.lblTitulo);

            // ── lblTitulo ────────────────────────────────────────────────
            this.lblTitulo.Text      = "\U0001F50D  Consultar Pedido";
            this.lblTitulo.AutoSize  = true;
            this.lblTitulo.Font      = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Left      = 16;
            this.lblTitulo.Top       = 12;

            // ── pnlBusca ─────────────────────────────────────────────────
            this.pnlBusca.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlBusca.Height    = 52;
            this.pnlBusca.BackColor = System.Drawing.Color.FromArgb(235, 228, 213);
            this.pnlBusca.Controls.Add(this.lblLblNum);
            this.pnlBusca.Controls.Add(this.txtNumPedido);
            this.pnlBusca.Controls.Add(this.btnBuscar);

            // ── lblLblNum ────────────────────────────────────────────────
            this.lblLblNum.Text      = "N\u00ba do Pedido:";
            this.lblLblNum.AutoSize  = true;
            this.lblLblNum.Font      = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblLblNum.ForeColor = System.Drawing.Color.FromArgb(60, 50, 35);
            this.lblLblNum.Left      = 16;
            this.lblLblNum.Top       = 16;

            // ── txtNumPedido ─────────────────────────────────────────────
            this.txtNumPedido.Left        = 124;
            this.txtNumPedido.Top         = 12;
            this.txtNumPedido.Width       = 200;
            this.txtNumPedido.Font        = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNumPedido.BackColor   = System.Drawing.Color.White;
            this.txtNumPedido.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNumPedido.KeyDown    += new System.Windows.Forms.KeyEventHandler(this.TxtNumPedido_KeyDown);

            // ── btnBuscar ────────────────────────────────────────────────
            this.btnBuscar.Text      = "\U0001F50D  Buscar";
            this.btnBuscar.Left      = 338;
            this.btnBuscar.Top       = 11;
            this.btnBuscar.Width     = 110;
            this.btnBuscar.Height    = 28;
            this.btnBuscar.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.btnBuscar.ForeColor = System.Drawing.Color.White;
            this.btnBuscar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscar.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnBuscar.Font      = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnBuscar.FlatAppearance.BorderSize = 0;
            this.btnBuscar.Click    += new System.EventHandler(this.BtnBuscar_Click);

            // ── pnlInfo ──────────────────────────────────────────────────
            this.pnlInfo.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlInfo.Height    = 168;
            this.pnlInfo.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.pnlInfo.Visible   = false;
            this.pnlInfo.Controls.Add(this.lblInfoTit);
            this.pnlInfo.Controls.Add(this.pnlInfoSep);
            this.pnlInfo.Controls.Add(this.lblCapCliente);
            this.pnlInfo.Controls.Add(this.lblCliente);
            this.pnlInfo.Controls.Add(this.lblCapTaxaEnt);
            this.pnlInfo.Controls.Add(this.lblTaxaEnt);
            this.pnlInfo.Controls.Add(this.lblCapCondPgto);
            this.pnlInfo.Controls.Add(this.lblCondPgto);
            this.pnlInfo.Controls.Add(this.lblCapValorPedido);
            this.pnlInfo.Controls.Add(this.lblValorPedido);
            this.pnlInfo.Controls.Add(this.lblCapValorPago);
            this.pnlInfo.Controls.Add(this.lblValorPago);
            this.pnlInfo.Controls.Add(this.lblCapDesconto);
            this.pnlInfo.Controls.Add(this.lblDesconto);
            this.pnlInfo.Controls.Add(this.lblCapAutorizador);
            this.pnlInfo.Controls.Add(this.lblAutorizador);

            // ── lblInfoTit ───────────────────────────────────────────────
            this.lblInfoTit.Text      = "Informa\u00e7\u00f5es do Pedido";
            this.lblInfoTit.AutoSize  = true;
            this.lblInfoTit.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblInfoTit.ForeColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.lblInfoTit.Left      = 16;
            this.lblInfoTit.Top       = 8;

            // ── pnlInfoSep ───────────────────────────────────────────────
            this.pnlInfoSep.Left      = 16;
            this.pnlInfoSep.Top       = 30;
            this.pnlInfoSep.Width     = 720;
            this.pnlInfoSep.Height    = 1;
            this.pnlInfoSep.BackColor = System.Drawing.Color.FromArgb(200, 185, 160);

            // ── Row 1: Cliente ───────────────────────────────────────────
            this.lblCapCliente.Text      = "Cliente:";
            this.lblCapCliente.AutoSize  = true;
            this.lblCapCliente.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCapCliente.ForeColor = System.Drawing.Color.FromArgb(100, 80, 50);
            this.lblCapCliente.Left      = 16;
            this.lblCapCliente.Top       = 40;

            this.lblCliente.Text      = "\u2014";
            this.lblCliente.AutoSize  = false;
            this.lblCliente.Width     = 590;
            this.lblCliente.Font      = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCliente.ForeColor = System.Drawing.Color.FromArgb(40, 30, 20);
            this.lblCliente.Left      = 80;
            this.lblCliente.Top       = 40;

            // ── Row 2: Taxa de Entrega | Condição de Pagamento ───────────
            this.lblCapTaxaEnt.Text      = "Taxa de Entrega:";
            this.lblCapTaxaEnt.AutoSize  = true;
            this.lblCapTaxaEnt.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCapTaxaEnt.ForeColor = System.Drawing.Color.FromArgb(100, 80, 50);
            this.lblCapTaxaEnt.Left      = 16;
            this.lblCapTaxaEnt.Top       = 68;

            this.lblTaxaEnt.Text      = "\u2014";
            this.lblTaxaEnt.AutoSize  = false;
            this.lblTaxaEnt.Width     = 140;
            this.lblTaxaEnt.Font      = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTaxaEnt.ForeColor = System.Drawing.Color.FromArgb(40, 30, 20);
            this.lblTaxaEnt.Left      = 144;
            this.lblTaxaEnt.Top       = 68;

            this.lblCapCondPgto.Text      = "Cond. Pagamento:";
            this.lblCapCondPgto.AutoSize  = true;
            this.lblCapCondPgto.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCapCondPgto.ForeColor = System.Drawing.Color.FromArgb(100, 80, 50);
            this.lblCapCondPgto.Left      = 330;
            this.lblCapCondPgto.Top       = 68;

            this.lblCondPgto.Text      = "\u2014";
            this.lblCondPgto.AutoSize  = false;
            this.lblCondPgto.Width     = 250;
            this.lblCondPgto.Font      = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCondPgto.ForeColor = System.Drawing.Color.FromArgb(40, 30, 20);
            this.lblCondPgto.Left      = 462;
            this.lblCondPgto.Top       = 68;

            // ── Row 3: Valor do Pedido | Valor Pago ─────────────────────
            this.lblCapValorPedido.Text      = "Valor do Pedido:";
            this.lblCapValorPedido.AutoSize  = true;
            this.lblCapValorPedido.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCapValorPedido.ForeColor = System.Drawing.Color.FromArgb(100, 80, 50);
            this.lblCapValorPedido.Left      = 16;
            this.lblCapValorPedido.Top       = 96;

            this.lblValorPedido.Text      = "\u2014";
            this.lblValorPedido.AutoSize  = false;
            this.lblValorPedido.Width     = 140;
            this.lblValorPedido.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblValorPedido.ForeColor = System.Drawing.Color.FromArgb(87, 120, 38);
            this.lblValorPedido.Left      = 144;
            this.lblValorPedido.Top       = 96;

            this.lblCapValorPago.Text      = "Valor Pago:";
            this.lblCapValorPago.AutoSize  = true;
            this.lblCapValorPago.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCapValorPago.ForeColor = System.Drawing.Color.FromArgb(100, 80, 50);
            this.lblCapValorPago.Left      = 330;
            this.lblCapValorPago.Top       = 96;

            this.lblValorPago.Text      = "\u2014";
            this.lblValorPago.AutoSize  = false;
            this.lblValorPago.Width     = 150;
            this.lblValorPago.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblValorPago.ForeColor = System.Drawing.Color.FromArgb(87, 120, 38);
            this.lblValorPago.Left      = 420;
            this.lblValorPago.Top       = 96;

            // ── Row 4: Desconto | Autorizado por ────────────────────────
            this.lblCapDesconto.Text      = "Desconto:";
            this.lblCapDesconto.AutoSize  = true;
            this.lblCapDesconto.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCapDesconto.ForeColor = System.Drawing.Color.FromArgb(100, 80, 50);
            this.lblCapDesconto.Left      = 16;
            this.lblCapDesconto.Top       = 124;

            this.lblDesconto.Text      = "\u2014";
            this.lblDesconto.AutoSize  = false;
            this.lblDesconto.Width     = 220;
            this.lblDesconto.Font      = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDesconto.ForeColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.lblDesconto.Left      = 86;
            this.lblDesconto.Top       = 124;

            this.lblCapAutorizador.Text      = "Autorizado por:";
            this.lblCapAutorizador.AutoSize  = true;
            this.lblCapAutorizador.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCapAutorizador.ForeColor = System.Drawing.Color.FromArgb(100, 80, 50);
            this.lblCapAutorizador.Left      = 330;
            this.lblCapAutorizador.Top       = 124;

            this.lblAutorizador.Text      = "\u2014";
            this.lblAutorizador.AutoSize  = false;
            this.lblAutorizador.Width     = 250;
            this.lblAutorizador.Font      = new System.Drawing.Font("Segoe UI", 9F);
            this.lblAutorizador.ForeColor = System.Drawing.Color.FromArgb(40, 30, 20);
            this.lblAutorizador.Left      = 440;
            this.lblAutorizador.Top       = 124;

            // ── lblItensTitle ────────────────────────────────────────────
            this.lblItensTitle.Dock      = System.Windows.Forms.DockStyle.Top;
            this.lblItensTitle.Height    = 28;
            this.lblItensTitle.Text      = "Itens do Pedido";
            this.lblItensTitle.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblItensTitle.ForeColor = System.Drawing.Color.FromArgb(50, 40, 25);
            this.lblItensTitle.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.lblItensTitle.Padding   = new System.Windows.Forms.Padding(4, 6, 0, 0);

            // ── gridItens ────────────────────────────────────────────────
            this.gridItens.Dock                                      = System.Windows.Forms.DockStyle.Fill;
            this.gridItens.AutoSizeColumnsMode                       = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridItens.ReadOnly                                  = true;
            this.gridItens.AllowUserToAddRows                        = false;
            this.gridItens.SelectionMode                             = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridItens.RowHeadersVisible                         = false;
            this.gridItens.BackgroundColor                           = System.Drawing.Color.White;
            this.gridItens.GridColor                                 = System.Drawing.Color.FromArgb(220, 210, 195);
            this.gridItens.BorderStyle                               = System.Windows.Forms.BorderStyle.None;
            this.gridItens.Font                                      = new System.Drawing.Font("Segoe UI", 9F);
            this.gridItens.DefaultCellStyle.BackColor                = System.Drawing.Color.White;
            this.gridItens.DefaultCellStyle.ForeColor                = System.Drawing.Color.FromArgb(50, 40, 30);
            this.gridItens.DefaultCellStyle.SelectionBackColor       = System.Drawing.Color.FromArgb(176, 110, 42);
            this.gridItens.DefaultCellStyle.SelectionForeColor       = System.Drawing.Color.White;
            this.gridItens.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(250, 247, 242);
            this.gridItens.ColumnHeadersDefaultCellStyle.BackColor   = System.Drawing.Color.FromArgb(36, 48, 82);
            this.gridItens.ColumnHeadersDefaultCellStyle.ForeColor   = System.Drawing.Color.White;
            this.gridItens.ColumnHeadersDefaultCellStyle.Font        = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.gridItens.ColumnHeadersHeightSizeMode               = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridItens.ColumnHeadersHeight                       = 36;
            this.gridItens.RowTemplate.Height                        = 28;
            this.gridItens.MultiSelect                               = false;
            this.gridItens.DataError                                += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);

            // ── pnlFoot ──────────────────────────────────────────────────
            this.pnlFoot.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFoot.Height    = 44;
            this.pnlFoot.BackColor = System.Drawing.Color.FromArgb(235, 228, 213);
            this.pnlFoot.Controls.Add(this.btnFechar);

            // ── btnFechar ────────────────────────────────────────────────
            this.btnFechar.Text      = "Fechar";
            this.btnFechar.Left      = 330;
            this.btnFechar.Top       = 8;
            this.btnFechar.Width     = 100;
            this.btnFechar.Height    = 28;
            this.btnFechar.BackColor = System.Drawing.Color.FromArgb(224, 113, 42);
            this.btnFechar.ForeColor = System.Drawing.Color.White;
            this.btnFechar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFechar.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnFechar.FlatAppearance.BorderSize = 0;
            this.btnFechar.Click    += new System.EventHandler(this.BtnFechar_Click);

            // ── Add controls to Form (order controls DockStyle layout) ───
            this.Controls.Add(this.gridItens);    // Fill — added first
            this.Controls.Add(this.lblItensTitle);// Top — below pnlInfo
            this.Controls.Add(this.pnlInfo);      // Top — below pnlBusca
            this.Controls.Add(this.pnlBusca);     // Top — below pnlTop
            this.Controls.Add(this.pnlFoot);      // Bottom
            this.Controls.Add(this.pnlTop);       // Top — topmost (added last)

            // ── Form ─────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(760, 540);
            this.Text                = "Consultar Pedido";
            this.BackColor           = System.Drawing.Color.FromArgb(245, 237, 216);
            this.Font                = new System.Drawing.Font("Segoe UI", 9F);
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox         = false;
            this.MinimizeBox         = false;
        }

        // ── Field declarations ────────────────────────────────────────────
        private System.Windows.Forms.Panel          pnlTop;
        private System.Windows.Forms.Label          lblTitulo;
        private System.Windows.Forms.Panel          pnlBusca;
        private System.Windows.Forms.Label          lblLblNum;
        private System.Windows.Forms.TextBox        txtNumPedido;
        private System.Windows.Forms.Button         btnBuscar;
        private System.Windows.Forms.Panel          pnlInfo;
        private System.Windows.Forms.Label          lblInfoTit;
        private System.Windows.Forms.Panel          pnlInfoSep;
        private System.Windows.Forms.Label          lblCapCliente;
        internal System.Windows.Forms.Label         lblCliente;
        private System.Windows.Forms.Label          lblCapTaxaEnt;
        internal System.Windows.Forms.Label         lblTaxaEnt;
        private System.Windows.Forms.Label          lblCapCondPgto;
        internal System.Windows.Forms.Label         lblCondPgto;
        private System.Windows.Forms.Label          lblCapValorPedido;
        internal System.Windows.Forms.Label         lblValorPedido;
        private System.Windows.Forms.Label          lblCapValorPago;
        internal System.Windows.Forms.Label         lblValorPago;
        private System.Windows.Forms.Label          lblCapDesconto;
        internal System.Windows.Forms.Label         lblDesconto;
        private System.Windows.Forms.Label          lblCapAutorizador;
        internal System.Windows.Forms.Label         lblAutorizador;
        private System.Windows.Forms.Label          lblItensTitle;
        internal System.Windows.Forms.DataGridView  gridItens;
        private System.Windows.Forms.Panel          pnlFoot;
        private System.Windows.Forms.Button         btnFechar;
    }
}
