# Copilot Instructions — Pedeai (Desktop C#)

## Regra principal: DbMigrator deve sempre estar sincronizado

Toda vez que uma nova propriedade for adicionada a um **Modelo** (`Pedeai/Modelo/*.cs`)
ou uma nova coluna for usada em um **DAL** (`Pedeai/DAL/*.cs`), a migration correspondente
**DEVE** ser adicionada a `Pedeai/DB/DbMigrator.cs` antes de finalizar a implementação.

### Como adicionar a migration

1. Abra `Pedeai/DB/DbMigrator.cs` e localize o bloco da tabela correspondente.
2. Adicione a chamada `AddColumnIfNotExists` no final do bloco da tabela:

```csharp
AddColumnIfNotExists(conn, db, "nome_tabela", "nome_coluna",
    "TIPO NOT NULL DEFAULT valor COMMENT 'descrição'");
```

3. Se for uma **nova tabela**, adicione um bloco `Exec(conn, @"CREATE TABLE IF NOT EXISTS ...")`.

### Anti-padrão a evitar

❌ **NÃO** crie métodos `EnsureMigrations()` dentro de DALs para compensar colunas faltando.
Esses métodos só são chamados quando o formulário específico é aberto, deixando instalações
novas quebradas antes de acessar esse formulário.

✅ **SEMPRE** use `DbMigrator.cs` como fonte única de verdade para o schema do banco.

---

## Regra: Atualizar memória do repositório

Após implementar qualquer funcionalidade relevante (nova tabela, novo campo importante,
novo fluxo), atualize o arquivo `/memories/repo/pedeai-site-estrutura.md` com as
informações pertinentes ao site (pedeai-site).

Para mudanças no banco MySQL/desktop, documente o que foi adicionado aqui mesmo
como comentário no bloco do `DbMigrator`.

---

## Checklist antes de finalizar uma implementação

- [ ] Propriedade adicionada ao Modelo (`Modelo/*.cs`)
- [ ] Coluna mapeada no DAL (SELECT, INSERT, UPDATE e mapeamento no reader)
- [ ] `AddColumnIfNotExists` (ou `CREATE TABLE IF NOT EXISTS`) adicionado ao `DbMigrator.cs`
- [ ] Se a coluna afeta sincronização Supabase → `SupabaseService.cs` atualizado
- [ ] Se afeta o site → `pedeai-site/src` atualizado e `pedeai-site-estrutura.md` atualizado

---

## Estrutura do projeto

| Pasta | Conteúdo |
|-------|----------|
| `Pedeai/Modelo/` | Classes de modelo (mapeiam tabelas do banco) |
| `Pedeai/DAL/` | Acesso a dados (queries MySQL) |
| `Pedeai/BLL/` | Regras de negócio |
| `Pedeai/Forms/` | Formulários WinForms |
| `Pedeai/DB/DbMigrator.cs` | **Schema único do banco** — toda migração vai aqui |
| `Pedeai/DB/SupabaseService.cs` | Sincronização com Supabase |
| `pedeai-site/src/` | Site React (cardápio online) |
