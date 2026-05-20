# Guia de Deploy — pedeai-site

## Como funciona o fluxo atual

```
Você sobe arquivos no GitHub (src/, public/, etc.)
           ↓
GitHub Actions roda automaticamente (build.yml)
           ↓
npm run build gera o dist/ atualizado
           ↓
Actions commita o dist/ novo no repositório
           ↓
Coolify detecta o novo commit e redeploya
           ↓
Site atualizado em demo.rangofood.com.br
```

---

## O que você precisa fazer para publicar alterações

### 1. Edite os arquivos localmente no VS Code

Faça suas alterações nos arquivos dentro da pasta `src/`.

### 2. Suba os arquivos alterados no GitHub

1. Acesse **github.com/rangofood/demo**
2. Entre na pasta correspondente (ex: `src/pages/`, `src/components/`)
3. Clique no arquivo → lápis ✏️ para editar **OU** clique em **Add file → Upload files**
4. Faça commit na branch `main`

### 3. Aguarde o GitHub Actions compilar

1. Acesse a aba **Actions** do repositório
2. Aguarde o workflow **"Build e Atualizar dist"** ficar verde ✅ (leva ~1 minuto)

### 4. Aguarde o Coolify republicar

O Coolify detecta o novo commit automaticamente e redeploya.  
Você pode acompanhar em **Coolify → Demo → production → Deployments**.

---

## Quais arquivos disparam o build automaticamente

| Pasta / Arquivo | Dispara build? |
|---|---|
| `src/**` | ✅ Sim |
| `public/**` | ✅ Sim |
| `index.html` | ✅ Sim |
| `package.json` | ✅ Sim |
| `vite.config.ts` | ✅ Sim |
| `tsconfig.json` | ✅ Sim |
| `components.json` | ✅ Sim |
| `migrations/` | ❌ Não (não precisa) |
| `README.md` | ❌ Não (não precisa) |
| `.github/workflows/` | ❌ Não |

---

## Configurações que NÃO devem ser alteradas

### GitHub Secrets (Settings → Secrets and variables → Actions)
| Secret | Descrição |
|---|---|
| `VITE_SUPABASE_URL` | URL do Supabase |
| `VITE_SUPABASE_ANON_KEY` | Chave anon do Supabase |
| `VITE_EMPRESA_CODIGO` | Código da empresa |

> ⚠️ Se algum desses secrets for deletado, o build vai gerar um site quebrado.

### Coolify — Environment Variables (Production)
| Variável | Descrição |
|---|---|
| `VITE_SUPABASE_URL` | URL do Supabase |
| `VITE_SUPABASE_ANON_KEY` | Chave anon do Supabase |
| `VITE_EMPRESA_CODIGO` | Código da empresa |

> ⚠️ Devem ter os mesmos valores dos secrets do GitHub.

### Coolify — Workflow Permissions
Em **Settings → Actions → General → Workflow permissions** do repositório:  
Deve estar marcado como **"Read and write permissions"**.

---

## Adicionando um novo cliente (nova empresa)

Para cada novo cliente você precisa:

1. Criar um novo projeto no Coolify com as variáveis da empresa correspondente (`VITE_EMPRESA_CODIGO` diferente)
2. Criar um novo repositório no GitHub (fork ou cópia do `demo`)
3. Configurar os 3 secrets no novo repositório
4. Garantir que **Workflow permissions** está como **Read and write**

---

## Arquivo do workflow (build.yml)

Crie o arquivo em `.github/workflows/build.yml` no repositório com o conteúdo abaixo:

```yaml
name: Build e Atualizar dist

on:
  workflow_dispatch:
  push:
    branches: [main]
    paths:
      - 'src/**'
      - 'public/**'
      - 'index.html'
      - 'package.json'
      - 'package-lock.json'
      - 'vite.config.ts'
      - 'tsconfig.json'
      - 'components.json'

jobs:
  build:
    runs-on: ubuntu-latest
    permissions:
      contents: write

    steps:
      - name: Checkout do repositório
        uses: actions/checkout@v4
        with:
          token: ${{ secrets.GITHUB_TOKEN }}

      - name: Configurar Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '20'
          cache: 'npm'

      - name: Instalar dependências
        run: npm ci

      - name: Criar arquivo .env
        run: |
          echo "VITE_SUPABASE_URL=${{ secrets.VITE_SUPABASE_URL }}" >> .env
          echo "VITE_SUPABASE_ANON_KEY=${{ secrets.VITE_SUPABASE_ANON_KEY }}" >> .env
          echo "VITE_EMPRESA_CODIGO=${{ secrets.VITE_EMPRESA_CODIGO }}" >> .env

      - name: Compilar projeto
        run: npm run build

      - name: Commitar dist atualizado
        run: |
          git config --global user.name "github-actions[bot]"
          git config --global user.email "github-actions[bot]@users.noreply.github.com"
          git add dist/ -f
          git diff --staged --quiet || git commit -m "chore: build automático [skip ci]"
          git push
```

> ℹ️ Para criar no GitHub: **Add file → Create new file** → digite `.github/workflows/build.yml` no nome → cole o conteúdo acima → commit.

---

## Solução de problemas

### Build falhou no GitHub Actions
- Verifique os 3 secrets em **Settings → Secrets and variables → Actions**
- Verifique se **Workflow permissions** está como **Read and write**

### Site não atualizou após build verde
- Acesse Coolify → projeto → **Redeploy**
- Verifique se o Coolify está monitorando a branch `main`

### Alteração não disparou o build
- Confirme que o arquivo alterado está em uma das pastas da tabela acima
- Se precisar forçar: vá em **Actions → "Build e Atualizar dist" → Run workflow**
