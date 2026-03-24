using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public class frmCadastroCupom : Form
    {
        private readonly CupomBLL _bll = new CupomBLL();
        private DataGridView grid = new DataGridView();
        private Panel pnlForm = new Panel();
        private TextBox txtCodigo, txtDescricao;
        private ComboBox cmbTipo, cmbSituacao;
        private NumericUpDown numValor, numMinimo, numLimite;
        private DateTimePicker dtpValido;
        private int _codigoEditando = 0;

        public frmCadastroCupom()
        {
            Text = "Cadastro de Cupons";
            Size = new Size(850, 520);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(750, 450);
            Font = new Font("Segoe UI", 9);
            BuildUI();
            CarregarGrid();
        }

        private void BuildUI()
        {
            var topBar = new Panel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(8, 6, 8, 0) };
            topBar.BackColor = Color.FromArgb(40, 40, 80);
            var btnN = Botao("+ Novo Cupom", Color.FromArgb(0, 150, 136)); btnN.Click += (_, __) => ModoNovo();
            var btnR = Botao("Atualizar", Color.FromArgb(63, 81, 181)); btnR.Left = 115; btnR.Click += (_, __) => CarregarGrid();
            topBar.Controls.AddRange(new Control[] { btnN, btnR });

            grid.Dock = DockStyle.Fill;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly = true; grid.AllowUserToAddRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false; grid.BackgroundColor = Color.White;
            grid.Font = new Font("Segoe UI", 9); grid.BorderStyle = BorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 80);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.DoubleClick += (_, __) => CarregarParaEditar();

            pnlForm = new Panel { Dock = DockStyle.Bottom, Height = 150, Padding = new Padding(10), Visible = false };
            pnlForm.BackColor = Color.FromArgb(245, 245, 250);
            pnlForm.BorderStyle = BorderStyle.FixedSingle;

            int y = 8;
            LblC(pnlForm, "Código:", 10, y); txtCodigo = new TextBox { Left = 65, Top = y, Width = 100 }; pnlForm.Controls.Add(txtCodigo);
            LblC(pnlForm, "Descrição:", 175, y); txtDescricao = new TextBox { Left = 245, Top = y, Width = 200 }; pnlForm.Controls.Add(txtDescricao);
            LblC(pnlForm, "Tipo:", 455, y); cmbTipo = new ComboBox { Left = 490, Top = y, Width = 100, DropDownStyle = ComboBoxStyle.DropDownList }; cmbTipo.Items.AddRange(new[] { "PERCENTUAL", "VALOR" }); cmbTipo.SelectedIndex = 0; pnlForm.Controls.Add(cmbTipo);
            LblC(pnlForm, "Sit.:", 605, y); cmbSituacao = new ComboBox { Left = 637, Top = y, Width = 55, DropDownStyle = ComboBoxStyle.DropDownList }; cmbSituacao.Items.AddRange(new[] { "A", "I" }); cmbSituacao.SelectedIndex = 0; pnlForm.Controls.Add(cmbSituacao);

            y += 34;
            LblC(pnlForm, "Valor:", 10, y); numValor = Num3(60, y, 80); pnlForm.Controls.Add(numValor);
            LblC(pnlForm, "Ped.Mínimo:", 150, y); numMinimo = Num3(235, y, 80); pnlForm.Controls.Add(numMinimo);
            LblC(pnlForm, "Limite Usos:", 325, y); numLimite = new NumericUpDown { Left = 410, Top = y, Width = 70, Minimum = 0, Maximum = 99999 }; pnlForm.Controls.Add(numLimite);
            LblC(pnlForm, "Válido até:", 490, y); dtpValido = new DateTimePicker { Left = 565, Top = y, Width = 120, Format = DateTimePickerFormat.Short }; pnlForm.Controls.Add(dtpValido);

            y += 38;
            var btnS = Botao("Salvar", Color.FromArgb(33, 150, 243)); btnS.Left = 10; btnS.Top = y; btnS.Click += BtnSalvar_Click; pnlForm.Controls.Add(btnS);
            var btnC = Botao("Cancelar", Color.FromArgb(158, 158, 158)); btnC.Left = 120; btnC.Top = y; btnC.Click += (_, __) => { pnlForm.Visible = false; _codigoEditando = 0; }; pnlForm.Controls.Add(btnC);
            var btnD = Botao("Desativar", Color.FromArgb(244, 67, 54)); btnD.Left = 230; btnD.Top = y; btnD.Click += BtnDesativar_Click; pnlForm.Controls.Add(btnD);

            Controls.Add(grid);
            Controls.Add(topBar);
            Controls.Add(pnlForm);
        }

        private void LblC(Panel p, string t, int x, int y)
        {
            p.Controls.Add(new Label { Text = t, Left = x, Top = y + 3, AutoSize = true });
        }

        private NumericUpDown Num3(int x, int y, int w)
        {
            return new NumericUpDown { Left = x, Top = y, Width = w, DecimalPlaces = 2, Maximum = 9999, Minimum = 0 };
        }

        private Button Botao(string texto, Color cor)
        {
            return new Button { Text = texto, BackColor = cor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 100, Height = 28, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
        }

        private void CarregarGrid()
        {
            try { grid.DataSource = _bll.Listar(); }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void ModoNovo()
        {
            _codigoEditando = 0;
            txtCodigo.Clear(); txtDescricao.Clear();
            cmbTipo.SelectedIndex = 0; cmbSituacao.SelectedIndex = 0;
            numValor.Value = 0; numMinimo.Value = 0; numLimite.Value = 0;
            dtpValido.Value = DateTime.Today.AddMonths(1);
            pnlForm.Visible = true; txtCodigo.Focus();
        }

        private void CarregarParaEditar()
        {
            if (grid.SelectedRows.Count == 0) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var obj = _bll.PesquisaCodigo(cod);
            if (obj == null) return;
            _codigoEditando = cod;
            txtCodigo.Text = obj.cupomCodigo ?? "";
            txtDescricao.Text = obj.cupomDescricao ?? "";
            cmbTipo.SelectedItem = obj.cupomTipo ?? "PERCENTUAL";
            numValor.Value = obj.cupomValor;
            numMinimo.Value = obj.cupomPedido_Minimo;
            numLimite.Value = obj.cupomLimite_Usos;
            dtpValido.Value = obj.cupomValido_Ate > DateTime.MinValue ? obj.cupomValido_Ate : DateTime.Today.AddMonths(1);
            cmbSituacao.SelectedItem = obj.Situacao ?? "A";
            pnlForm.Visible = true; txtCodigo.Focus();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text)) { MessageBox.Show("Informe o código do cupom."); return; }
            var obj = new Cupom
            {
                Codigo             = _codigoEditando,
                cupomCodigo        = txtCodigo.Text.Trim().ToUpper(),
                cupomDescricao     = txtDescricao.Text,
                cupomTipo          = cmbTipo.SelectedItem?.ToString() ?? "PERCENTUAL",
                cupomValor         = numValor.Value,
                cupomPedido_Minimo = numMinimo.Value,
                cupomLimite_Usos   = (int)numLimite.Value,
                cupomValido_Ate    = dtpValido.Value,
                Situacao           = cmbSituacao.SelectedItem?.ToString() ?? "A",
            };
            var erro = _bll.Salvar(obj);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            pnlForm.Visible = false; _codigoEditando = 0; CarregarGrid();
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
    }
}
