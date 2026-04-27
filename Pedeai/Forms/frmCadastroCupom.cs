using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmCadastroCupom : Form
    {
        private CupomBLL _bll;
        private int _codigoEditando = 0;
        private System.Data.DataTable _allCupons;
        private System.Windows.Forms.TextBox _txtBuscaCupom;

        public frmCadastroCupom()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _bll = new CupomBLL();
            Load += (_, __) => { AdicionarPainelBusca(); CarregarGrid(); };
        }

        private void AdicionarPainelBusca()
        {
            var pnlSearch = new System.Windows.Forms.Panel
            { Dock = System.Windows.Forms.DockStyle.Top, Height = 38,
              BackColor = System.Drawing.Color.FromArgb(235, 226, 208) };
            _txtBuscaCupom = new System.Windows.Forms.TextBox
            { Left = 8, Top = 8, Width = 280,
              Font = new System.Drawing.Font("Segoe UI", 9.5F),
              PlaceholderText = "Buscar pelo código ou descrição do cupom..." };
            var btnBuscar = new System.Windows.Forms.Button
            { Left = 296, Top = 7, Width = 90, Height = 26, Text = "Buscar",
              BackColor = System.Drawing.Color.FromArgb(224, 113, 42),
              ForeColor = System.Drawing.Color.White,
              FlatStyle = System.Windows.Forms.FlatStyle.Flat,
              Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold),
              Cursor = Cursors.Hand };
            btnBuscar.FlatAppearance.BorderSize = 0;
            btnBuscar.Click += (_, __) => FiltrarGrid(_txtBuscaCupom.Text);
            _txtBuscaCupom.KeyDown += (s, ev) => { if (ev.KeyCode == System.Windows.Forms.Keys.Enter) FiltrarGrid(_txtBuscaCupom.Text); };
            pnlSearch.Controls.Add(_txtBuscaCupom);
            pnlSearch.Controls.Add(btnBuscar);
            Controls.Add(pnlSearch);
            Controls.SetChildIndex(pnlSearch, 1);
        }

        private void CarregarGrid()
        {
            try
            {
                _allCupons = _bll.Listar();
                FiltrarGrid(_txtBuscaCupom?.Text ?? "");
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void FiltrarGrid(string filtro)
        {
            if (_allCupons == null) return;
            var dv = new System.Data.DataView(_allCupons);
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var f = filtro.Replace("'", "''");
                dv.RowFilter = $"Cupom LIKE '%{f}%' OR Descricao LIKE '%{f}%'";
            }
            grid.DataSource = dv;
            if (grid.Columns.Contains("Codigo")) grid.Columns["Codigo"].Visible = false;
        }

        private void ModoNovo()
        {
            _codigoEditando = 0;
            txtDescricao.Clear();
            cmbTipo.SelectedIndex = 0; cmbSituacao.SelectedIndex = 0;
            numValor.Value = 0; numMinimo.Value = 0; numLimite.Value = 0;
            dtpValido.Value = DateTime.Today.AddMonths(1);
            pnlForm.Visible = true; txtDescricao.Focus();
        }

        private void CarregarParaEditar()
        {
            if (grid.SelectedRows.Count == 0) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var obj = _bll.PesquisaCodigo(cod);
            if (obj == null) return;
            _codigoEditando = cod;
            txtDescricao.Text = !string.IsNullOrWhiteSpace(obj.cupomDescricao) ? obj.cupomDescricao : (obj.cupomCodigo ?? "");
            cmbTipo.SelectedItem = obj.cupomTipo ?? "PERCENTUAL";
            numValor.Value = obj.cupomValor;
            numMinimo.Value = obj.cupomPedido_Minimo;
            numLimite.Value = obj.cupomLimite_Usos;
            dtpValido.Value = obj.cupomValido_Ate > DateTime.MinValue ? obj.cupomValido_Ate : DateTime.Today.AddMonths(1);
            cmbSituacao.SelectedItem = (obj.Situacao == "I") ? "Inativo" : "Ativo";
            pnlForm.Visible = true; txtDescricao.Focus();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDescricao.Text)) { MessageBox.Show("Informe o nome do cupom."); return; }
            var obj = new Cupom
            {
                Codigo             = _codigoEditando,
                cupomCodigo        = txtDescricao.Text.Trim().ToUpper(),
                cupomDescricao     = txtDescricao.Text.Trim(),
                cupomTipo          = cmbTipo.SelectedItem?.ToString() ?? "PERCENTUAL",
                cupomValor         = numValor.Value,
                cupomPedido_Minimo = numMinimo.Value,
                cupomLimite_Usos   = (int)numLimite.Value,
                cupomValido_Ate    = dtpValido.Value,
                Situacao           = cmbSituacao.SelectedItem?.ToString() == "Inativo" ? "I" : "A",
            };
            var erro = _bll.Salvar(obj);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            pnlForm.Visible = false; _codigoEditando = 0; CarregarGrid();
            // Sync to Supabase in background
            int codSalvo = obj.Codigo > 0 ? obj.Codigo : BuscarCodigoCupom(obj.cupomCodigo);
            if (codSalvo > 0)
                System.Threading.Tasks.Task.Run(async () =>
                    await DB.SupabaseService.SincronizarCupomAsync(codSalvo));
        }

        private int BuscarCodigoCupom(string codigoCupom)
        {
            try
            {
                if (_allCupons == null) return 0;
                foreach (System.Data.DataRow r in _allCupons.Rows)
                    if (r["Cupom"]?.ToString() == codigoCupom)
                        return Convert.ToInt32(r["Codigo"]);
            }
            catch { }
            return 0;
        }

        private void BtnDesativar_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            if (_codigoEditando == 0) { MessageBox.Show("Abra o cupom para edição primeiro."); return; }
            if (MessageBox.Show("Desativar cupom?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            var obj = _bll.PesquisaCodigo(_codigoEditando);
            if (obj != null) { obj.Situacao = "I"; _bll.Salvar(obj); }
            pnlForm.Visible = false; _codigoEditando = 0; CarregarGrid();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
