using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pedeai.DAL;
using Pedeai.DB;
using Pedeai.Forms;

namespace Pedeai
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Testa servidor MySQL e abre configuração se não conectar.
            // Loop: repete até conectar ou usuário cancelar.
            while (!DB.DbHelper.TestarConexao())
            {
                using var config = new Forms.frmConexao();
                config.MotivoErro = "Não foi possível conectar ao servidor MySQL.\nConfigure o endereço do servidor e tente novamente.";
                if (config.ShowDialog() != DialogResult.OK) return;
            }

            // Cria banco (se não existir) + tabelas + dados iniciais
            if (!DbMigrator.Executar()) return;

            // ── Auto-renovação de licença via VPS ───────────────────────────
            var empresa = new EmpresaDAL().Carregar();
            TentarAutoRenovarLicenca(empresa);

            // ── Registro no Supabase (background — aparece no gerenciador) ──
            _ = RegistrarNoSupabase(empresa);

            // Recarrega após possível atualização da chave
            empresa = new EmpresaDAL().Carregar();

            bool licencaValida = LicencaService.ValidarChave(empresa.empCodigo_Empresa, empresa.empChave_Licenca);

            if (!licencaValida)
            {
                // Se ainda não há data de início de graça, inicia agora (novo install ou expiração)
                if (empresa.empData_Graca == null)
                {
                    new EmpresaDAL().SalvarDataGraca(empresa.Codigo, DateTime.Today);
                    empresa.empData_Graca = DateTime.Today;
                }

                int diasGraca = (int)(DateTime.Today - empresa.empData_Graca.Value).TotalDays;
                // diasGraca 0-3 = período de graça ativo; >= 4 = bloqueado (botão desabilitado)
                using var lic = new frmLicenca(empresa, diasGraca);
                if (lic.ShowDialog() != DialogResult.OK) return;
            }
            else
            {
                // Aviso de vencimento próximo (últimos 5 dias) — após auto-renovação, só aparece
                // se a VPS estava offline e não conseguiu renovar.
                var exp = LicencaService.ObterExpiracao(empresa.empChave_Licenca);
                if (exp.HasValue && (exp.Value - DateTime.Today).TotalDays <= 5)
                {
                    int dias = (int)(exp.Value - DateTime.Today).TotalDays;
                    string msg = dias == 0
                        ? "⚠️ Sua licença VENCE HOJE! Renove para continuar usando o sistema."
                        : $"⚠️ Sua licença vence em {dias} dia(s) ({exp.Value:dd/MM/yyyy}).\r\nRenove para não ser bloqueado.";
                    MessageBox.Show(msg, "Licença — Aviso de Vencimento",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            // Exibe tela de login; cancela aplicacao se o usuario fechar sem logar
            using (var login = new frmLogin())
            {
                if (login.ShowDialog() != DialogResult.OK) return;
            }

            Application.Run(new Form1());
        }

        /// <summary>
        /// Tenta renovar automaticamente a licença via VPS se ela estiver vencida ou próxima do vencimento.
        /// Falha silenciosamente se a VPS estiver offline ou não configurada.
        /// </summary>
        private static void TentarAutoRenovarLicenca(Modelo.Empresa empresa)
        {
            string vpsUrl = ConfigurationManager.AppSettings["LicencaVpsUrl"] ?? "";
            string apiKey = ConfigurationManager.AppSettings["LicencaApiKey"] ?? "";

            if (string.IsNullOrWhiteSpace(vpsUrl) || string.IsNullOrWhiteSpace(apiKey))
                return; // VPS não configurada — usa fluxo manual

            // Renovar se: licença inválida OU vencendo em até 7 dias
            bool precisaRenovar = !LicencaService.ValidarChave(empresa.empCodigo_Empresa, empresa.empChave_Licenca);
            if (!precisaRenovar)
            {
                var exp = LicencaService.ObterExpiracao(empresa.empChave_Licenca);
                precisaRenovar = exp.HasValue && (exp.Value - DateTime.Today).TotalDays <= 7;
            }

            if (!precisaRenovar) return;

            string novaChave = LicencaApiClient.TentarRenovar(
                empresa.empCodigo_Empresa, apiKey, vpsUrl, diasValidade: 30);

            if (!string.IsNullOrWhiteSpace(novaChave)
                && LicencaService.ValidarChave(empresa.empCodigo_Empresa, novaChave))
            {
                new EmpresaDAL().SalvarLicenca(empresa.Codigo, novaChave);
                // Reseta período de graça se havia sido iniciado
                if (empresa.empData_Graca.HasValue)
                    new EmpresaDAL().SalvarDataGraca(empresa.Codigo, null);
            }
        }

        /// <summary>
        /// Registra ou atualiza esta instalação na tabela Clientes do Supabase.
        /// Executado em background — falha silenciosamente se offline.
        /// Após o registro, o cliente aparece no PedeaiUpdateAdmin e no LicencaGenerator.
        /// </summary>
        private static async Task RegistrarNoSupabase(Modelo.Empresa empresa)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["SupabaseUrl"] ?? "";
                string key = ConfigurationManager.AppSettings["SupabaseKey"] ?? "";
                if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(key)) return;
                if (string.IsNullOrWhiteSpace(empresa.empCodigo_Empresa)) return;

                string cod  = empresa.empCodigo_Empresa.Trim().ToUpperInvariant();
                string nome = string.IsNullOrWhiteSpace(empresa.empNome_Fantasia)
                    ? empresa.empNome : empresa.empNome_Fantasia;
                string restBase = url.TrimEnd('/') + "/rest/v1/";

                using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
                http.DefaultRequestHeaders.Add("apikey", key);
                http.DefaultRequestHeaders.Add("Authorization", "Bearer " + key);

                // 1. Verifica se já existe
                var getResp = await http.GetAsync(
                    restBase + "Clientes?CodigoEmpresa=eq." + Uri.EscapeDataString(cod) + "&select=Id,Nivel");
                string getBody = await getResp.Content.ReadAsStringAsync();

                if (!getResp.IsSuccessStatusCode) return;

                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var existentes = JsonSerializer.Deserialize<ClienteIdNivel[]>(getBody, opts);

                if (existentes != null && existentes.Length > 0)
                {
                    // Já existe — atualiza NomeEmpresa, VersaoAtual e UltimaConsulta
                    long id    = existentes[0].Id;
                    int  nivel = existentes[0].Nivel;

                    // Sincroniza nível local se diferente
                    if (nivel != empresa.empNivel_Atualizacao)
                        new EmpresaDAL().SalvarNivelAtualizacao(empresa.Codigo, nivel);

                    var patch = JsonSerializer.Serialize(new
                    {
                        NomeEmpresa    = nome ?? "",
                        UltimaConsulta = DateTime.UtcNow
                    });
                    var patchReq = new HttpRequestMessage(new HttpMethod("PATCH"),
                        restBase + "Clientes?Id=eq." + id);
                    patchReq.Content = new StringContent(patch, Encoding.UTF8, "application/json");
                    await http.SendAsync(patchReq);
                }
                else
                {
                    // Novo cliente — registra
                    int nivelLocal = empresa.empNivel_Atualizacao > 0 ? empresa.empNivel_Atualizacao : 2;
                    var payload = JsonSerializer.Serialize(new
                    {
                        CodigoEmpresa  = cod,
                        NomeEmpresa    = nome ?? "",
                        Nivel          = nivelLocal,
                        VersaoAtual    = "",
                        Bloqueado      = false,
                        DataRegistro   = DateTime.UtcNow,
                        UltimaConsulta = DateTime.UtcNow
                    });
                    var postReq = new HttpRequestMessage(HttpMethod.Post, restBase + "Clientes");
                    postReq.Headers.Add("Prefer", "return=minimal");
                    postReq.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                    await http.SendAsync(postReq);
                }
            }
            catch { /* background — falha silenciosa */ }
        }

        private class ClienteIdNivel
        {
            public long Id    { get; set; }
            public int  Nivel { get; set; }
        }
    }
}
