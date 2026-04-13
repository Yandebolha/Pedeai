---
name: pedeai-backup
description: 'Trabalhar com o módulo de backup PedeaiBackup: agendamento, estratégia incremental/completa, integração MEGAcmd, configuração de tabelas, retenção. Use para: adicionar tabelas ao backup, depurar falhas de upload MEGA, alterar horários de agendamento, entender o estado incremental.'
---

# Skill: Backup Pedeai

## Quando Usar
- Adicionar novas tabelas ao backup (completo ou incremental)
- Depurar falhas de upload para o MEGA
- Alterar lógica de agendamento ou retenção
- Configurar o `PedeaiBackup` em uma nova máquina

## Arquivos-Chave

| Arquivo | Papel |
|---------|-------|
| `PedeaiBackup/Services/BackupService.cs` | Lógica principal de dump + compressão |
| `PedeaiBackup/Services/MegaService.cs` | Wrapper MEGAcmd CLI |
| `PedeaiBackup/Services/AgendadorService.cs` | Timer com os dias/horários configurados |
| `PedeaiBackup/Services/ConfigManager.cs` | Lê/grava JSON em `%APPDATA%\PedeaiBackup\` |
| `PedeaiBackup/Models/BackupConfig.cs` | Todas as configurações serializáveis |
| `PedeaiBackup/Models/BackupEstado.cs` | Estado incremental (última vez, MaxCodigo por tabela) |
| `PedeaiBackup/Forms/frmBackupConfig.cs` | UI de configuração (TabControl: DB+MEGA / Agendamento / Log) |

## Estratégia de Backup

### Tabelas Completas (sempre dump total)
`empresa`, `usuario`, `grupo_mercadoria`, `mercadoria`, `configuracao_impressao`,
`fornecedor`, `cupom`, `fidelizacao`, `promocao`, `cardapio_dia`, `loja`

### Tabelas Incrementais (filtra `Codigo > MaxCodigo`)
`pedido`, `pedido_detalhe`, `gasto_material`, `entrada_mercadoria`,
`parcela_entrada_mercadoria`, `estoque_item`, `turno`, `necessidade_empresa`, `fidelizacao_historico`

## Procedimento: Adicionar Nova Tabela ao Backup

### Tabela de cadastro (dump completo):
1. Abrir `BackupService.cs`
2. Localizar o array de tabelas completas
3. Adicionar o nome da tabela

### Tabela de movimentação (incremental):
1. Abrir `BackupService.cs`
2. Localizar o array de tabelas incrementais
3. Adicionar o nome da tabela
4. Garantir que a tabela tem coluna `Codigo INT AUTO_INCREMENT`

## Configuração MEGAcmd

Requer MEGAcmd instalado. Configurar:
- `MegaCmdPath`: caminho do executável `mega-cmd.exe`
- `MegaEmail` / `MegaSenha`: credenciais da conta MEGA
- `MegaPasta`: pasta destino (criada automaticamente se não existir)

## Config Keys (`BackupConfig`)

| Propriedade | Tipo | Descrição |
|-------------|------|-----------|
| `DbHost` | string | Servidor MySQL |
| `DbPorta` | int | Porta (padrão 3306) |
| `DbNome` | string | Nome do banco |
| `DbUsuario` | string | Usuário MySQL |
| `DbSenha` | string | Senha MySQL |
| `MySqlDumpPath` | string | Caminho do `mysqldump.exe` |
| `MegaEmail` | string | Email MEGA |
| `MegaSenha` | string | Senha MEGA |
| `MegaPasta` | string | Pasta destino no MEGA |
| `MegaCmdPath` | string | Caminho do MEGAcmd |
| `DiasAtivos` | int[] | Dias da semana (0=Dom, 6=Sáb) |
| `Horarios` | string[] | Horários no formato "HH:mm" |
| `DiasRetencaoLocal` | int | Dias a manter backups locais |

## Localização dos Arquivos
- Config: `%APPDATA%\PedeaiBackup\config.json`
- Estado incremental: `%APPDATA%\PedeaiBackup\estado.json`
- Backups locais: `%APPDATA%\PedeaiBackup\backups\`
