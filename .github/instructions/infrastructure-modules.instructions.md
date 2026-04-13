---
applyTo: "PedeaiLicencaServer/**,PedeaiBackup/**,PedeaiUpdateServer/**,PedeaiUpdateAdmin/**,PedeaiUpdateService/**"
---

# Módulos de Infraestrutura — Pedeai

Cinco projetos separados do principal (`Pedeai`), todos em .NET 5.  
Rodando no VPS Hostinger via Docker (`docker-compose.vps.yml`).

---

## 1. PedeaiLicencaServer (ASP.NET Core 5, porta 5000)

### Responsabilidade
Servidor VPS que gera e valida chaves de licença para a aplicação principal Pedeai.

### Endpoints

| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| `GET`  | `/api/licenca/ping` | Pública | Health-check |
| `POST` | `/api/licenca/renovar` | `X-Api-Key` | Gera nova chave (30 dias por padrão, máx 366) |
| `POST` | `/api/licenca/validar` | `X-Api-Key` | Valida uma chave existente |

### Autenticação
Header `X-Api-Key` comparado com `appsettings.json > ApiKey`.  
Implementado em `Filters/ApiKeyAuthAttribute.cs` com comparação constant-time.

### Formato da Chave de Licença
`YYYYMMDD-XXXXX-XXXXX-XXXXX`  
- `YYYYMMDD` = data de expiração  
- `XXXXX-XXXXX-XXXXX` = HMAC-SHA256 do `codigoEmpresa + expiração` com segredo `"PEDEAI-LIC-V1-2026"`, truncado em 15 hex chars

Gerado em `Services/LicencaService.GerarChave()`.  
**Mesmo algoritmo** deve ser usado na validação side-client em `Pedeai/DB/LicencaService.cs`.

### Config Keys (`appsettings.json`)
```json
{
  "ApiKey": "ALTERAR_ANTES_DE_USAR",
  "Urls": "http://0.0.0.0:5000"
}
```

### Deploy
`Dockerfile` incluso. Porta mapeada: `5000:5000` no compose.

---

## 2. PedeaiBackup (WinForms .NET 5)

### Responsabilidade
Aplicação desktop que agenda backups do MySQL para a nuvem MEGA via MEGAcmd CLI.

### Estratégia de Backup
- **Completo:** tabelas de cadastro (empresa, usuário, mercadoria, etc.) — sempre dump total
- **Incremental:** tabelas de movimentação (pedido, turno, gasto, entrada_mercadoria, etc.) — filtra `Codigo > MaxCodigo` salvo no estado anterior

Tabelas full: `empresa`, `usuario`, `grupo_mercadoria`, `mercadoria`, `configuracao_impressao`, `fornecedor`, `cupom`, `fidelizacao`, `promocao`, `cardapio_dia`, `loja`  
Tabelas incrementais: `pedido`, `pedido_detalhe`, `gasto_material`, `entrada_mercadoria`, `parcela_entrada_mercadoria`, `estoque_item`, `turno`, `necessidade_empresa`, `fidelizacao_historico`

### Serviços Principais
| Classe | Responsabilidade |
|--------|-----------------|
| `BackupService` | Executa mysqldump, comprime GZip, limpa antigos |
| `MegaService` | Wrapper CLI do MEGAcmd (login, mkdir, put) |
| `AgendadorService` | Timer que dispara backup nos horários configurados |
| `ConfigManager` | Serializa/desserializa JSON em `%APPDATA%\PedeaiBackup\` |

### Config Keys (`BackupConfig.cs`)
`DbHost`, `DbPorta`, `DbNome`, `DbUsuario`, `DbSenha`, `MySqlDumpPath`  
`MegaEmail`, `MegaSenha`, `MegaPasta`, `MegaCmdPath`  
`DiasAtivos` (int[] 0-6), `Horarios` (string[] "HH:mm"), `DiasRetencaoLocal` (int)

### Requisito Externo
MEGAcmd instalado no sistema. Caminho configurável via `MegaCmdPath`.

---

## 3. PedeaiUpdateServer (ASP.NET Core 5, porta 5001)

### Responsabilidade
Servidor VPS que gerencia pacotes de atualização e cliente registrados. Persiste dados em SQLite (`Data/update.db`).

### Endpoints — Clientes (`X-Api-Key` + `X-Client-Id`)

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET`  | `/api/update/ping` | Health-check (público) |
| `POST` | `/api/update/registrar` | Registra cliente (primeira vez) |
| `GET`  | `/api/update/verificar` | Verifica se há versão nova (`?versaoAtual=X.X`) |
| `GET`  | `/api/update/download/{pacoteId}` | Baixa o ZIP do pacote |
| `POST` | `/api/update/confirmar/{pacoteId}` | Confirma instalação com sucesso/falha |

### Endpoints — Admin (`X-Admin-Token`)

| Método | Rota | Descrição |
|--------|------|-----------|
| `POST`   | `/api/admin/pacotes/publicar` | Upload multipart de `.zip` |
| `GET`    | `/api/admin/pacotes` | Lista pacotes publicados |
| `DELETE` | `/api/admin/pacotes/{id}` | Desativa pacote |
| `GET`    | `/api/admin/clientes` | Lista clientes registrados |
| `PUT`    | `/api/admin/clientes/{id}/nivel` | Altera nível do cliente |

### Níveis de Atualização
| Nível | Nome | Descrição |
|-------|------|-----------|
| `1` | Beta | Recebe atualizações imediatamente |
| `2` | Standard | Distribuição normal |
| `3` | Legacy | Recebe atualizações atrasadas / apenas críticas |

### Estrutura do ZIP de Pacote
```
meu-pacote.zip
├── files/          ← arquivos que serão copiados sobre o AppDir
│   ├── Pedeai.exe
│   └── ...
└── update.sql      ← (opcional) script SQL executado pós-cópia
```

### Config Keys (`appsettings.json`)
```json
{
  "ClienteApiKey": "ALTERAR",
  "AdminToken": "ALTERAR",
  "DataDir": "Data",
  "PackagesDir": "Data/packages"
}
```

---

## 4. PedeaiUpdateAdmin (WinForms .NET 5)

### Responsabilidade
Interface desktop para o administrador publicar pacotes e gerenciar clientes no PedeaiUpdateServer.

### Funcionalidades (4 abas)
- **Publicar:** selecionar ZIP, preencher versão/nível/descrição → `PublicarAsync()`
- **Clientes:** listar instalações registradas, alterar nível, bloquear/desbloquear
- **Pacotes:** listar pacotes publicados, desativar
- **Config:** endpoint do servidor + Admin Token

### `AdminApiClient` → endpoints consumidos
- `PublicarAsync(zipPath, versao, nivel, descricao)` — multipart upload, timeout 10 min
- `ListarPacotesAsync()`, `ExcluirPacoteAsync(id)`
- Header: `X-Admin-Token`

---

## 5. PedeaiUpdateService (Windows Service .NET 5)

### Responsabilidade
Serviço Windows que roda na máquina do cliente, verifica atualizações periodicamente e aplica automaticamente.

### Fluxo de Atualização
1. Lê `CodigoEmpresa` do MySQL local para identificar o cliente
2. Registra-se no servidor (1ª vez) e salva `ClienteId` no `appsettings.json`
3. Chama `/api/update/verificar?versaoAtual=X.X` periodicamente
4. Se há update: baixa ZIP → para o app → copia `files/` → executa `update.sql` → reinicia app → confirma sucesso

### Config Keys (`appsettings.json`)
| Chave | Descrição |
|-------|-----------|
| `UpdateVpsUrl` | URL base do PedeaiUpdateServer |
| `UpdateApiKey` | Chave X-Api-Key |
| `ClienteId` | Preenchido automaticamente no 1° registro |
| `ConnectionString` | MySQL local para ler CodigoEmpresa |
| `AppDir` | Diretório de instalação do Pedeai |
| `AppProcessName` | Nome do processo para kill/restart |
| `AppExe` | Caminho do executável principal |
| `IntervalMinutos` | Frequência de verificação |
| `VersaoAtual` | Versão instalada (atualizada após cada apply) |

### Instalação do Serviço
Executar `instalar-servico.bat` como admin:
```bat
sc create PedeaiUpdateService binPath= "C:\...\PedeaiUpdateService.exe"
sc start PedeaiUpdateService
```

---

## Variáveis de Ambiente (VPS — produção)

Configuradas via `.env` no docker-compose:

| Variável | Projeto | Descrição |
|----------|---------|-----------|
| `LICENCA_API_KEY` | LicencaServer | Chave X-Api-Key |
| `UPDATE_CLIENT_API_KEY` | UpdateServer | Chave para clientes |
| `UPDATE_ADMIN_TOKEN` | UpdateServer | Token admin |

Nunca commitar valores reais — usar `.env.example` como referência.
