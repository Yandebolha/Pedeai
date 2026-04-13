---
name: pedeai-licenca
description: 'Trabalhar com o sistema de licenciamento Pedeai: PedeaiLicencaServer (VPS), LicencaService (cliente), renovação automática, formato de chave HMAC-SHA256. Use para: adicionar endpoints de licença, depurar renovação automática, alterar duração, entender o algoritmo de geração/validação de chaves.'
---

# Skill: Licenciamento Pedeai

## Quando Usar
- Modificar ou depurar o `PedeaiLicencaServer`
- Entender como a renovação automática funciona no cliente (`Pedeai/DB/LicencaApiClient.cs`)
- Alterar duração padrão da licença, segredo HMAC ou lógica de grace period
- Adicionar novos endpoints ao servidor de licença

## Arquivos-Chave

| Arquivo | Papel |
|---------|-------|
| `PedeaiLicencaServer/Controllers/LicencaController.cs` | Endpoints REST do servidor |
| `PedeaiLicencaServer/Services/LicencaService.cs` | Geração/validação da chave (HMAC) |
| `PedeaiLicencaServer/Filters/ApiKeyAuthAttribute.cs` | Auth via `X-Api-Key` header |
| `Pedeai/DB/LicencaService.cs` | Validação local da chave (mesmo algoritmo) |
| `Pedeai/DB/LicencaApiClient.cs` | HTTP client que chama o servidor VPS |
| `Pedeai/Program.cs` | `TentarAutoRenovarLicenca()` — renova quando ≤7 dias |

## Algoritmo da Chave

```
formato : YYYYMMDD-XXXXX-XXXXX-XXXXX
segredo : "PEDEAI-LIC-V1-2026"
payload : codigoEmpresa + "|" + "YYYYMMDD"
hash    : HMAC-SHA256(payload, segredo) → primeiros 15 hex chars → split 5+5+5
```

**IMPORTANTE:** O mesmo segredo e algoritmo existe nos dois lados (servidor e cliente). Se alterar um, alterar o outro.

## Endpoints do Servidor

```
GET  /api/licenca/ping         → público, sem auth
POST /api/licenca/renovar      → X-Api-Key, body: { codigoEmpresa, diasDuracao }
POST /api/licenca/validar      → X-Api-Key, body: { codigoEmpresa, chave }
```

## Grace Period (App.config)

| Chave | Descrição |
|-------|-----------|
| `LicencaVpsUrl` | URL base do servidor (ex.: `http://vps:5000`) |
| `LicencaApiKey` | Valor do header `X-Api-Key` |

## Procedimento: Alterar Duração Padrão
1. Abrir `LicencaController.cs`
2. Localizar `diasDuracao ?? 30` → alterar `30` para o novo padrão
3. O limite máximo é `366` dias (hardcoded na validação do controller)

## Procedimento: Rotacionar o Segredo HMAC
1. Alterar `Segredo` em `PedeaiLicencaServer/Services/LicencaService.cs`
2. Alterar o mesmo campo em `Pedeai/DB/LicencaService.cs`
3. Re-gerar todas as licenças ativas (chaves antigas se tornam inválidas)
