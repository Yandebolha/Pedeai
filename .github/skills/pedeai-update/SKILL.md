---
name: pedeai-update
description: 'Trabalhar com o sistema de atualização automática Pedeai: PedeaiUpdateServer (VPS), PedeaiUpdateAdmin (publicar pacotes), PedeaiUpdateService (Windows Service cliente). Use para: publicar nova versão, entender fluxo de atualização, depurar falha de apply, gerenciar níveis de cliente, criar ZIP de pacote correto.'
---

# Skill: Sistema de Atualização Pedeai

## Quando Usar
- Publicar uma nova versão do Pedeai
- Depurar falha no processo de atualização automática
- Alterar nível de atualização de um cliente
- Entender o fluxo completo servidor ↔ serviço
- Adicionar lógica de pré/pós-update

## Projetos Envolvidos

| Projeto | Papel |
|---------|-------|
| `PedeaiUpdateServer` | API VPS (ASP.NET Core 5, porta 5001, SQLite) |
| `PedeaiUpdateAdmin` | WinForms para o administrador publicar/gerenciar |
| `PedeaiUpdateService` | Windows Service na máquina do cliente |

## Arquivos-Chave

| Arquivo | Papel |
|---------|-------|
| `PedeaiUpdateServer/Controllers/UpdateController.cs` | Endpoints para clientes |
| `PedeaiUpdateServer/Controllers/AdminController.cs` | Endpoints admin |
| `PedeaiUpdateServer/Data/UpdateDb.cs` | Repositório SQLite |
| `PedeaiUpdateAdmin/Services/AdminApiClient.cs` | HTTP client admin |
| `PedeaiUpdateAdmin/Forms/frmUpdateAdmin.cs` | UI principal (4 abas) |
| `PedeaiUpdateService/Updater.cs` | Lógica de verificar/baixar/aplicar/confirmar |
| `PedeaiUpdateService/UpdateWorker.cs` | BackgroundService com polling |

---

## Procedimento: Publicar Nova Versão

1. Compilar o projeto `Pedeai` em Release
2. Criar ZIP com estrutura:
   ```
   pacote-vX.X.zip
   ├── files/
   │   ├── Pedeai.exe
   │   ├── *.dll
   │   └── ...
   └── update.sql   ← (opcional) migrations/scripts SQL
   ```
3. Abrir `PedeaiUpdateAdmin`
4. Aba **Publicar**: selecionar ZIP, preencher versão, nível e descrição
5. Clicar em Publicar — upload multipart para `/api/admin/pacotes/publicar`

---

## Fluxo de Atualização (PedeaiUpdateService)

```
1. Ler CodigoEmpresa do MySQL local
2. Se ClienteId == 0 → POST /api/update/registrar (salva ID em appsettings.json)
3. A cada IntervalMinutos:
   a. GET /api/update/verificar?versaoAtual=X.X
   b. Se temUpdate:
      - GET /api/update/download/{pacoteId} → salva ZIP temp
      - Para processo AppProcessName
      - Extrai files/ → copia sobre AppDir
      - Executa update.sql (se existir) no MySQL local
      - Inicia AppExe
      - POST /api/update/confirmar/{pacoteId} (sucesso/falha)
      - Atualiza VersaoAtual no appsettings.json
```

---

## Níveis de Atualização

| Nível | Nome | Recebe |
|-------|------|--------|
| `1` | Beta | Todas as versões imediatamente |
| `2` | Standard | Versões normais |
| `3` | Legacy | Apenas versões críticas / atrasadas |

Alterar via `PedeaiUpdateAdmin` → aba **Clientes** → botão Alterar Nível.  
Ou via API: `PUT /api/admin/clientes/{id}/nivel` com `X-Admin-Token`.

---

## Auth Headers

| Header | Usado por | Configurado em |
|--------|-----------|---------------|
| `X-Api-Key` | PedeaiUpdateService | `appsettings.json > UpdateApiKey` + servidor `ClienteApiKey` |
| `X-Client-Id` | PedeaiUpdateService | `appsettings.json > ClienteId` (auto-preenchido) |
| `X-Admin-Token` | PedeaiUpdateAdmin | Config da UI + servidor `AdminToken` |

---

## Config Keys (PedeaiUpdateService/appsettings.json)

| Chave | Descrição |
|-------|-----------|
| `UpdateVpsUrl` | URL base do servidor (ex.: `http://vps:5001`) |
| `UpdateApiKey` | Valor do header X-Api-Key |
| `ClienteId` | ID auto-registrado (0 = não registrado ainda) |
| `ConnectionString` | MySQL local (para ler CodigoEmpresa) |
| `AppDir` | Pasta de instalação do Pedeai.exe |
| `AppProcessName` | Nome do processo (para kill) |
| `AppExe` | Caminho completo do executável |
| `IntervalMinutos` | Frequência de verificação (ex.: 60) |
| `VersaoAtual` | Versão atual instalada (ex.: "1.0.0") |

---

## Instalação do Serviço Windows

Executar `instalar-servico.bat` como Administrador na máquina cliente.

Para desinstalar:
```bat
sc stop PedeaiUpdateService
sc delete PedeaiUpdateService
```

---

## Banco SQLite do Servidor

Tabelas em `Data/update.db`:
- `Clientes` — `Id`, `EmpresaId`, `Nivel`, `Bloqueado`, `UltimaVersao`
- `Pacotes` — `Id`, `Versao`, `Nivel`, `Descricao`, `Ativo`, `TemSql`, `DataPublicacao`
- `AplicacoesUpdate` — histórico de atualizações aplicadas por cliente
