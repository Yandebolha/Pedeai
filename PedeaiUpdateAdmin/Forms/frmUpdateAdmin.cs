using PedeaiUpdateAdmin.Services;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PedeaiUpdateAdmin.Forms
{
    public partial class frmUpdateAdmin : Form
    {
        private AdminApiClient _api;
        private List<dynamic>  _clientes;
        private List<dynamic>  _pacotes;

        public frmUpdateAdmin()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode ==
                System.ComponentModel.LicenseUsageMode.Designtime) return;
            Load += FrmUpdateAdmin_Load;
        }

        private void FrmUpdateAdmin_Load(object sender, EventArgs e)
        {
            var (url, key, srk) = AdminApiClient.CarregarConfig();
            txtVpsUrl.Text         = url;
            txtAdminToken.Text     = key;
            txtServiceRoleKey.Text = srk;

            if (!string.IsNullOrWhiteSpace(url) && !string.IsNullOrWhiteSpace(key))
            {
                _api = new AdminApiClient(url, key, srk);
                CarregarClientes();
                CarregarPacotes();
            }
            else
            {
                tabControl.SelectedTab = tabConfig;
                SetStatus("Configure a URL do Supabase e a Chave antes de continuar.");
            }
        }

        // ── Publicar ──────────────────────────────────────────────────────────────

        private void BtnBrowseZip_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog();
            dlg.Title  = "Selecionar pacote ZIP de atualização";
            dlg.Filter = "Arquivos ZIP|*.zip";
            if (dlg.ShowDialog() == DialogResult.OK)
                txtArquivo.Text = dlg.FileName;
        }

        private async void BtnPublicar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtArquivo.Text))
            { MessageBox.Show("Selecione o arquivo ZIP.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (string.IsNullOrWhiteSpace(txtVersao.Text))
            { MessageBox.Show("Informe a versão.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (_api == null) { SetStatus("Configure a conexão primeiro."); return; }

            int nivel = cboNivel.SelectedIndex + 1;
            btnPublicar.Enabled = false;
            pbUpload.Visible    = true;
            pbUpload.Style      = ProgressBarStyle.Marquee;
            SetStatus("Enviando pacote...");

            try
            {
                long id = await _api.PublicarAsync(
                    txtArquivo.Text, txtVersao.Text.Trim(), nivel, txtDescricao.Text.Trim());
                SetStatus($"Publicado com sucesso! ID do pacote: {id}");
                lblStatusPublicacao.Text = $"✔ Publicado — ID {id} | Versão {txtVersao.Text} | Nível {nivel}";
                lblStatusPublicacao.ForeColor = System.Drawing.Color.Green;
                CarregarPacotes();
            }
            catch (Exception ex)
            {
                SetStatus($"Erro ao publicar: {ex.Message}");
                lblStatusPublicacao.Text = $"❌ Erro: {ex.Message}";
                lblStatusPublicacao.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                btnPublicar.Enabled = true;
                pbUpload.Visible    = false;
            }
        }

        // ── Clientes ──────────────────────────────────────────────────────────────

        private async void CarregarClientes()
        {
            if (_api == null) return;
            try
            {
                _clientes = await _api.ListarClientesAsync();
                FiltrarClientes();
            }
            catch (Exception ex) { SetStatus($"Erro ao carregar clientes: {ex.Message}"); }
        }

        private void FiltrarClientes()
        {
            if (_clientes == null) return;
            int filtro = cboFiltroNivel.SelectedIndex; // 0=Todos, 1-3 = nível

            dgvClientes.Rows.Clear();
            dgvClientes.Columns.Clear();
            dgvClientes.Columns.Add("Id",          "ID");
            dgvClientes.Columns.Add("Empresa",     "Empresa");
            dgvClientes.Columns.Add("Codigo",      "Código Emp.");
            dgvClientes.Columns.Add("Nivel",       "Nível");
            dgvClientes.Columns.Add("Versao",      "Versão Atual");
            dgvClientes.Columns.Add("Bloqueado",   "Bloqueado");
            dgvClientes.Columns.Add("UltConsulta", "Última Consulta");

            foreach (dynamic c in _clientes)
            {
                int nivel = c.Nivel != null ? (int)c.Nivel : 2;
                if (filtro > 0 && nivel != filtro) continue;

                string nome   = (string)c.NomeEmpresa   ?? "";
                string codigo = (string)c.CodigoEmpresa ?? "";
                if (string.IsNullOrWhiteSpace(nome)) nome = codigo; // fallback

                dgvClientes.Rows.Add(
                    c.Id != null ? (long)c.Id : 0L,
                    nome,
                    codigo,
                    NivelLabel(nivel),
                    (string)c.VersaoAtual    ?? "—",
                    c.Bloqueado != null && (bool)c.Bloqueado ? "SIM" : "não",
                    (string)c.UltimaConsulta ?? "—");
            }
        }

        private void CboFiltroNivel_Changed(object sender, EventArgs e) => FiltrarClientes();

        private void BtnRefreshClientes_Click(object sender, EventArgs e) => CarregarClientes();

        private async void BtnAlterarNivel_Click(object sender, EventArgs e)
        {
            if (!ObterClienteSelecionado(out long id)) return;
            using var dlg = new frmAlterarNivel();
            if (dlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                await _api.AlterarNivelAsync(id, dlg.NivelSelecionado);
                SetStatus($"Nível do cliente {id} alterado para {dlg.NivelSelecionado}.");
                CarregarClientes();
            }
            catch (Exception ex) { SetStatus($"Erro: {ex.Message}"); }
        }

        private async void BtnBloquear_Click(object sender, EventArgs e)
        {
            if (!ObterClienteSelecionado(out long id)) return;
            if (MessageBox.Show($"Bloquear cliente ID {id}?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try { await _api.BloquearClienteAsync(id, true); CarregarClientes(); }
            catch (Exception ex) { SetStatus($"Erro: {ex.Message}"); }
        }

        private async void BtnDesbloquear_Click(object sender, EventArgs e)
        {
            if (!ObterClienteSelecionado(out long id)) return;
            try { await _api.BloquearClienteAsync(id, false); CarregarClientes(); }
            catch (Exception ex) { SetStatus($"Erro: {ex.Message}"); }
        }

        private bool ObterClienteSelecionado(out long id)
        {
            id = 0;
            if (dgvClientes.SelectedRows.Count == 0)
            { MessageBox.Show("Selecione um cliente.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            id = Convert.ToInt64(dgvClientes.SelectedRows[0].Cells["Id"].Value);
            return true;
        }

        // ── Pacotes ───────────────────────────────────────────────────────────────

        private async void CarregarPacotes()
        {
            if (_api == null) return;
            try
            {
                _pacotes = await _api.ListarPacotesAsync();
                dgvPacotes.Rows.Clear(); dgvPacotes.Columns.Clear();
                dgvPacotes.Columns.Add("Id",      "ID");
                dgvPacotes.Columns.Add("Versao",  "Versão");
                dgvPacotes.Columns.Add("Nivel",   "Nível");
                dgvPacotes.Columns.Add("Desc",    "Descrição");
                dgvPacotes.Columns.Add("Tam",     "Tamanho");
                dgvPacotes.Columns.Add("TemSQL",  "SQL");
                dgvPacotes.Columns.Add("Ativo",   "Ativo");
                dgvPacotes.Columns.Add("Data",    "Publicado");

                foreach (dynamic p in _pacotes)
                {
                    int nivel = p.Nivel != null ? (int)p.Nivel : 2;
                    dgvPacotes.Rows.Add(
                        p.Id != null ? (long)p.Id : 0L,
                        (string)p.Versao         ?? "",
                        NivelLabel(nivel),
                        (string)p.Descricao      ?? "",
                        FormatarBytes(p.TamanhoBytes != null ? (long)p.TamanhoBytes : 0L),
                        p.TemSQL  != null && (bool)p.TemSQL  ? "Sim" : "—",
                        p.Ativo   != null && (bool)p.Ativo   ? "Sim" : "Não",
                        (string)p.DataPublicacao ?? "");
                }
            }
            catch (Exception ex) { SetStatus($"Erro ao carregar pacotes: {ex.Message}"); }
        }

        private void BtnRefreshPacotes_Click(object sender, EventArgs e) => CarregarPacotes();

        private async void BtnExcluirPacote_Click(object sender, EventArgs e)
        {
            if (dgvPacotes.SelectedRows.Count == 0)
            { MessageBox.Show("Selecione um pacote.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            long id = Convert.ToInt64(dgvPacotes.SelectedRows[0].Cells["Id"].Value);
            if (MessageBox.Show($"Desativar pacote ID {id}?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try { await _api.ExcluirPacoteAsync(id); CarregarPacotes(); }
            catch (Exception ex) { SetStatus($"Erro: {ex.Message}"); }
        }

        // ── Configuração ──────────────────────────────────────────────────────────

        private void BtnSalvarConfig_Click(object sender, EventArgs e)
        {
            AdminApiClient.SalvarConfig(txtVpsUrl.Text.Trim(), txtAdminToken.Text.Trim(), txtServiceRoleKey.Text.Trim());
            _api = new AdminApiClient(txtVpsUrl.Text.Trim(), txtAdminToken.Text.Trim(), txtServiceRoleKey.Text.Trim());
            SetStatus("Configuração salva.");
        }

        private async void BtnTestarConexao_Click(object sender, EventArgs e)
        {
            btnTestarConexao.Enabled = false;
            SetStatus("Testando conexão...");
            try
            {
                var client = new AdminApiClient(txtVpsUrl.Text.Trim(), txtAdminToken.Text.Trim(), txtServiceRoleKey.Text.Trim());
                var lista   = await client.ListarClientesAsync();
                SetStatus($"Conexão OK! {lista.Count} clientes registrados.");
            }
            catch (Exception ex) { SetStatus($"Falha na conexão: {ex.Message}"); }
            finally { btnTestarConexao.Enabled = true; }
        }

        // ── Utilitários ───────────────────────────────────────────────────────────

        private void SetStatus(string msg)
        {
            if (InvokeRequired) { Invoke(new Action<string>(SetStatus), msg); return; }
            lblStatusBar.Text = msg;
        }

        private static string NivelLabel(int n)
        {
            if (n == 1) return "1 — Beta";
            if (n == 3) return "3 — Legacy";
            return "2 — Standard";
        }

        private static string FormatarBytes(long b)
        {
            if (b < 1024) return $"{b} B";
            if (b < 1024 * 1024) return $"{b / 1024.0:F1} KB";
            return $"{b / (1024.0 * 1024):F1} MB";
        }
    }

    // ── Diálogo simples para escolher nível ───────────────────────────────────────
    internal class frmAlterarNivel : Form
    {
        internal int NivelSelecionado { get; private set; } = 2;
        private System.Windows.Forms.ComboBox _cbo;

        internal frmAlterarNivel()
        {
            Text = "Alterar Nível";
            ClientSize = new System.Drawing.Size(280, 130);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;

            var lbl = new System.Windows.Forms.Label();
            lbl.Text = "Selecione o novo nível:";
            lbl.SetBounds(8, 12, 250, 20);

            _cbo = new System.Windows.Forms.ComboBox();
            _cbo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            _cbo.Items.Add("1 — Beta");
            _cbo.Items.Add("2 — Standard");
            _cbo.Items.Add("3 — Legacy");
            _cbo.SelectedIndex = 1;
            _cbo.SetBounds(8, 36, 250, 23);

            var btnOk = new System.Windows.Forms.Button();
            btnOk.Text = "OK";
            btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            btnOk.SetBounds(8, 74, 80, 28);
            btnOk.Click += new System.EventHandler(BtnOk_Click);

            Controls.Add(lbl);
            Controls.Add(_cbo);
            Controls.Add(btnOk);
            AcceptButton = btnOk;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            NivelSelecionado = _cbo.SelectedIndex + 1;
        }
    }
}
