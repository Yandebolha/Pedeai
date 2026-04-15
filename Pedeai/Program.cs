using System;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
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

            // ── Atualização automática silenciosa ─────────────────────────
            // 1. Aplica qualquer atualização já baixada na inicialização anterior
            AplicarAtualizacaoPendente();
            // 2. Verifica + baixa nova atualização em background (aplica no próximo start)
            _ = Task.Run(() => VerificarEBaixarAtualizacao());

            // ── Auto-renovação de licença via VPS ───────────────────────────
            var empresa = new EmpresaDAL().Carregar();
            TentarAutoRenovarLicenca(empresa);

            // ── Registro no Supabase + sync de licença ────────────────────
            // Roda em thread separada para não bloquear a UI, mas aguarda
            // até 8s para que a ChaveLicenca já esteja salva antes do reload.
            try { Task.Run(() => RegistrarNoSupabase(empresa)).Wait(TimeSpan.FromSeconds(8)); } catch { }

            // Se o administrador bloqueou este cliente no Supabase, impede o acesso
            if (_clienteBloqueado)
            {
                MessageBox.Show(
                    "Este sistema foi bloqueado pelo administrador.\nEntre em contato com o suporte.",
                    "Acesso Bloqueado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Limite de máquinas simultâneas excedido
            if (_maquinasExcedidas)
            {
                MessageBox.Show(
                    "O número máximo de máquinas simultâneas desta licença já foi atingido.\n" +
                    "Encerre o sistema em outra máquina e tente novamente.",
                    "Limite de Acessos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Sync periódico a cada 10 min (nível + licença + bloqueio)
            var syncTimer = new System.Threading.Timer(_ =>
            {
                try
                {
                    var emp = new EmpresaDAL().Carregar();
                    _ = RegistrarNoSupabase(emp);

                    // Verifica bloqueio detectado na última sync
                    if (_clienteBloqueado)
                    {
                        var form = Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null;
                        form?.Invoke(new Action(() =>
                        {
                            MessageBox.Show(
                                "Este sistema foi bloqueado pelo administrador.\nO sistema será encerrado.",
                                "Acesso Bloqueado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                        }));
                    }
                    if (_maquinasExcedidas)
                    {
                        var form = Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null;
                        form?.Invoke(new Action(() =>
                        {
                            MessageBox.Show(
                                "Limite de máquinas simultâneas atingido. O sistema será encerrado.",
                                "Limite de Acessos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Application.Exit();
                        }));
                    }
                }
                catch { }
            }, null, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));
            GC.KeepAlive(syncTimer);

            // Recarrega após possível atualização da chave vinda do Supabase
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

        private static volatile bool _clienteBloqueado = false;
        private static volatile bool _maquinasExcedidas = false;

        
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
                    restBase + "Clientes?CodigoEmpresa=eq." + Uri.EscapeDataString(cod) + "&select=Id,Nivel,ChaveLicenca,Bloqueado,VersaoAtual,MaxMaquinas");
                string getBody = await getResp.Content.ReadAsStringAsync();

                if (!getResp.IsSuccessStatusCode) return;

                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var existentes = JsonSerializer.Deserialize<ClienteIdNivel[]>(getBody, opts);

                if (existentes != null && existentes.Length > 0)
                {
                    long id    = existentes[0].Id;
                    int  nivel = existentes[0].Nivel;
                    bool bloq  = existentes[0].Bloqueado;

                    // ── Bloqueio ────────────────────────────────────────────
                    if (bloq)
                    {
                        _clienteBloqueado = true;
                        return;
                    }
                    // ── Limite de máquinas simultâneas ───────────────────────
                    int maxMaq = existentes[0].MaxMaquinas;
                    if (maxMaq > 0)
                    {
                        // Conta quantas instalações da mesma chave consultaram nos últimos 15 min
                        string chaveCliente = existentes[0].ChaveLicenca ?? "";
                        if (!string.IsNullOrWhiteSpace(chaveCliente))
                        {
                            string limite = DateTime.UtcNow.AddMinutes(-15).ToString("o");
                            string urlAtivas = restBase + "Clientes?ChaveLicenca=eq."
                                + Uri.EscapeDataString(chaveCliente)
                                + "&UltimaConsulta=gte." + Uri.EscapeDataString(limite)
                                + "&select=Id";
                            var rAtivas = await http.GetAsync(urlAtivas);
                            if (rAtivas.IsSuccessStatusCode)
                            {
                                var bAtivas = await rAtivas.Content.ReadAsStringAsync();
                                var ativas  = JsonSerializer.Deserialize<ClienteIdNivel[]>(bAtivas, opts);
                                // Se já tem maxMaq ativas E esta instalação ainda não está entre elas
                                // (não bloqueamos se esta máquina já está contada)
                                bool estaAtiva = ativas != null && System.Array.Exists(
                                    ativas, a => a.Id == id);
                                if (!estaAtiva && ativas != null && ativas.Length >= maxMaq)
                                {
                                    _maquinasExcedidas = true;
                                    return;
                                }
                            }
                        }
                    }
                    // ── Sync Nível ──────────────────────────────────────────
                    if (nivel > 0 && nivel != empresa.empNivel_Atualizacao)
                        new EmpresaDAL().SalvarNivelAtualizacao(empresa.Codigo, nivel);

                    // ── Sync ChaveLicenca ───────────────────────────────────
                    string chaveSup = existentes[0].ChaveLicenca ?? "";
                    if (!string.IsNullOrWhiteSpace(chaveSup)
                        && chaveSup != empresa.empChave_Licenca
                        && LicencaService.ValidarChave(cod, chaveSup))
                    {
                        new EmpresaDAL().SalvarLicenca(empresa.Codigo, chaveSup);
                        // Reseta período de graça ao receber chave válida do servidor
                        if (empresa.empData_Graca.HasValue)
                            new EmpresaDAL().SalvarDataGraca(empresa.Codigo, null);
                    }

                    // ── Sync VersaoAtual ────────────────────────────────────
                    string versaoSup = existentes[0].VersaoAtual ?? "";
                    if (!string.IsNullOrWhiteSpace(versaoSup)
                        && versaoSup != empresa.empVersao_Atual)
                        new EmpresaDAL().SalvarVersaoAtual(empresa.Codigo, versaoSup);

                    // ── Sync MaxMaquinas ─────────────────────────────────────
                    int maxMaqSup = existentes[0].MaxMaquinas;
                    if (maxMaqSup != empresa.empMax_Maquinas)
                        new EmpresaDAL().SalvarMaxMaquinas(empresa.Codigo, maxMaqSup);

                    var patch = JsonSerializer.Serialize(new
                    {
                        NomeEmpresa    = nome ?? "",
                        VersaoAtual    = empresa.empVersao_Atual,
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
            public long   Id            { get; set; }
            public int    Nivel         { get; set; }
            public string ChaveLicenca  { get; set; }
            public bool   Bloqueado     { get; set; }
            public string VersaoAtual   { get; set; }
            public int    MaxMaquinas   { get; set; }
        }

        // ── Atualização automática ─────────────────────────────────────────────

        private const string PASTA_PENDENTE = @"C:\Pedeai\_update_pending";
        private const string PASTA_APP      = @"C:\Pedeai";

        /// <summary>
        /// Se existir uma pasta de atualização pendente, lança um script oculto que:
        /// 1. Aguarda este processo encerrar (3 s)
        /// 2. Copia os arquivos com xcopy
        /// 3. Apaga a pasta pendente
        /// 4. Reinicia o Pedeai.exe
        /// Encerra o processo atual imediatamente para liberar os arquivos bloqueados.
        /// </summary>
        private static void AplicarAtualizacaoPendente()
        {
            if (!Directory.Exists(PASTA_PENDENTE)) return;
            try
            {
                string exePath = Path.Combine(PASTA_APP, "RanGoFood.exe");
                string bat = $@"@echo off
timeout /t 3 /nobreak > nul
xcopy /E /Y /I ""{PASTA_PENDENTE}"" ""{PASTA_APP}""
rmdir /S /Q ""{PASTA_PENDENTE}""
if exist ""{exePath}"" start """" ""{exePath}""
del ""%~f0""
";
                string batPath = Path.Combine(Path.GetTempPath(), "pedeai_apply_update.bat");
                File.WriteAllText(batPath, bat, System.Text.Encoding.Default);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName        = batPath,
                    WindowStyle     = System.Diagnostics.ProcessWindowStyle.Hidden,
                    CreateNoWindow  = true,
                    UseShellExecute = true
                });
                Environment.Exit(0); // libera o bloqueio dos arquivos
            }
            catch { /* se falhar, ignora e continua normalmente */ }
        }

        /// <summary>
        /// Consulta o Supabase para verificar se há versão mais nova do que a instalada.
        /// Se houver, baixa o ZIP e extrai em C:\Pedeai\_update_pending\ para aplicar no próximo start.
        /// </summary>
        private static async Task VerificarEBaixarAtualizacao()
        {
            try
            {
                string url = ConfigurationManager.AppSettings["SupabaseUrl"] ?? "";
                string key = ConfigurationManager.AppSettings["SupabaseKey"] ?? "";
                if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(key)) return;

                var empresa = new EmpresaDAL().Carregar();
                int nivel         = empresa.empNivel_Atualizacao > 0 ? empresa.empNivel_Atualizacao : 2;
                string versaoAtual = empresa.empVersao_Atual ?? "";

                string restBase    = url.TrimEnd('/') + "/rest/v1/";
                string storageBase = url.TrimEnd('/') + "/storage/v1/";

                using var http = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
                http.DefaultRequestHeaders.Add("apikey", key);
                http.DefaultRequestHeaders.Add("Authorization", "Bearer " + key);

                // Busca pacote mais recente com Versao > versaoAtual e Nivel <= nivel do cliente
                string versaoEnc = Uri.EscapeDataString(versaoAtual);
                string pkgUrl = restBase + "Pacotes?Ativo=eq.true&Nivel=lte." + nivel
                              + "&Versao=gt." + versaoEnc
                              + "&order=Versao.desc&limit=1";

                var resp = await http.GetAsync(pkgUrl);
                if (!resp.IsSuccessStatusCode) return;

                string body = await resp.Content.ReadAsStringAsync();
                var opts    = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var pacotes = JsonSerializer.Deserialize<PacoteInfo[]>(body, opts);
                if (pacotes == null || pacotes.Length == 0) return;

                var pacote = pacotes[0];

                // Baixa o ZIP
                string caminho = pacote.CaminhoArquivo ?? "";
                if (string.IsNullOrWhiteSpace(caminho)) return;

                // Codifica segmentos do path preservando '/'
                string pathCodificado = string.Join("/",
                    caminho.Split('/').Select(Uri.EscapeDataString));
                string downloadUrl = storageBase + "object/pacotes/" + pathCodificado;

                using var dlResp = await http.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
                if (!dlResp.IsSuccessStatusCode) return;

                string zipTemp = Path.Combine(Path.GetTempPath(), $"pedeai_dl_{pacote.Id}.zip");
                using (var fs = new FileStream(zipTemp, FileMode.Create, FileAccess.Write))
                    await dlResp.Content.CopyToAsync(fs);

                // Extrai para pasta pendente (vazia antes)
                if (Directory.Exists(PASTA_PENDENTE))
                    Directory.Delete(PASTA_PENDENTE, recursive: true);
                ZipFile.ExtractToDirectory(zipTemp, PASTA_PENDENTE);
                File.Delete(zipTemp);

                // Salva nova versão no banco (para que na próxima consulta não baixe de novo)
                new EmpresaDAL().SalvarVersaoAtual(empresa.Codigo, pacote.Versao);
            }
            catch { /* background — falha silenciosa */ }
        }

        private class PacoteInfo
        {
            public long   Id             { get; set; }
            public string Versao         { get; set; }
            public string CaminhoArquivo { get; set; }
            public int    Nivel          { get; set; }
        }
    }
}
