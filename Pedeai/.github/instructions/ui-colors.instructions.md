---
description: "Use when creating, editing or styling any WinForms screen, form, panel, button, label, grid or any UI element in the RanGoFood project. Defines the official color palette, component styles and design rules."
applyTo: "**/*.cs"
---

# RanGoFood — Paleta de Cores e Estilo de UI

## Tema Geral

O sistema usa um **tema quente âmbar/marrom** (warm amber/dark) com fundo claro creme e sidebar escuro quase preto.

---

## Paleta Principal (`Color.FromArgb`)

### Estrutura da Janela

| Elemento                   | Cor (RGB)          | Descrição                        |
|----------------------------|--------------------|----------------------------------|
| Sidebar background          | `(18, 20, 25)`     | Quase preto com tom escuro       |
| Top bar background          | `Color.White`      | Branco puro                      |
| Content area / fundo geral  | `(248, 245, 240)`  | Creme claro                      |
| Footer background           | `(18, 20, 25)`     | Mesmo da sidebar                 |

### Sidebar

| Elemento                        | Cor (RGB)           |
|---------------------------------|---------------------|
| Barra de destaque topo (accent) | `(176, 110, 42)`    |
| Texto do botão nav              | `(195, 185, 165)`   |
| Hover dos botões nav            | `(35, 38, 48)`      |
| Active/click dos botões nav     | `(176, 110, 42)`    |
| Separadores / dividers          | `(40, 42, 52)`      |
| Texto das seções (ex: OPERACIONAL) | `(90, 82, 68)`   |
| Logo texto (RanGoFood)          | `(210, 185, 140)`   |
| Logo ícone                      | `(200, 165, 110)`   |

### Top Bar

| Elemento                        | Cor (RGB)           |
|---------------------------------|---------------------|
| Título da tela (lblTitulo)      | `(50, 40, 20)`      |
| Logo texto (lblLogoTopBar)      | `(50, 40, 20)`      |
| Botão "Atualizar"               | `(176, 110, 42)` — texto branco |
| Botão "Trocar Usuário" bg       | `(235, 228, 215)`   |
| Botão "Trocar Usuário" border   | `(176, 110, 42)`    |
| Botão "Trocar Usuário" texto    | `(120, 75, 25)`     |
| Botão minimizar/maximizar bg    | `(220, 215, 205)`   |
| Botão minimizar/maximizar texto | `(60, 55, 45)`      |
| Botão fechar (✕) bg             | `(192, 57, 43)` — padrão vermelho |

### Footer

| Elemento                    | Cor (RGB)           |
|-----------------------------|---------------------|
| Texto empresa               | `(120, 105, 80)`    |
| Texto usuário               | `(160, 140, 110)`   |

---

## Cards de Dashboard e Financeiro

Os cards devem usar **tons quentes** variados. Cada tipo de card tem uma cor designada:

| Card                  | Cor (RGB)           |
|-----------------------|---------------------|
| Pedidos Hoje          | `(176, 110, 42)`    |
| Faturamento Hoje      | `(115, 140, 50)`    |
| Total Clientes        | `(155, 95, 40)`     |
| Pedidos Pendentes     | `(190, 100, 30)`    |
| Total de Pedidos (Fin)| `(176, 110, 42)`    |
| Vendas                | `(115, 140, 50)`    |
| Compras/Entradas      | `(180, 70, 55)`     |
| Gastos Material       | `(160, 100, 38)`    |
| Taxa de Entrega       | `(130, 100, 48)`    |
| Lucro Estimado (+)    | `(115, 140, 50)`    |
| Lucro Estimado (-)    | `(180, 70, 55)`     |
| Dinheiro              | `(155, 130, 48)`    |
| Cartão                | `(73, 110, 160)`    |
| Pix                   | `(80, 130, 110)`    |

- Texto do título do card: `(235, 215, 185)`
- Texto do valor do card: `Color.White`

---

## Gráficos (GDI+)

| Elemento                           | Cor (RGB)          |
|------------------------------------|--------------------|
| Fundo do painel de gráfico         | `(248, 245, 240)`  |
| Fundo da barra de filtro de período| `(235, 228, 213)`  |
| Título do gráfico (texto)          | `(50, 40, 20)`     |
| Labels dos eixos                   | `(130, 115, 90)`   |
| Linhas de grade                    | `(215, 205, 190)`  |
| Barra "Vendas por Canal"           | `(176, 110, 42)`   |
| Barra "Top 3 Produtos"             | `(160, 100, 38)`   |
| Barra "Receita por Dia"            | `(115, 140, 50)`   |
| Accent bar "Vendas por Canal"      | `(176, 110, 42)`   |
| Accent bar "Top 3 Produtos"        | `(155, 95, 38)`    |
| Accent bar "Receita por Dia"       | `(115, 140, 50)`   |

### Botões de período no gráfico (Diário / Semanal / Mensal / Anual)

| Estado    | BackColor           | ForeColor           |
|-----------|---------------------|---------------------|
| Ativo     | `(176, 110, 42)`    | `Color.White`       |
| Inativo   | `(200, 192, 170)`   | `(70, 60, 45)`      |

---

## DataGridView (Grids)

| Elemento                        | Cor (RGB)           |
|---------------------------------|---------------------|
| Background geral                | `Color.White`       |
| Linhas da grade (GridColor)     | `(220, 210, 195)`   |
| Céls padrão bg                  | `Color.White`       |
| Céls padrão fg                  | `(50, 40, 30)`      |
| Linhas alternadas               | `(250, 247, 242)`   |
| Seleção bg                      | `(176, 110, 42)`    |
| Seleção fg                      | `Color.White`       |
| Cabeçalho bg                    | `(36, 48, 82)`      |
| Cabeçalho fg                    | `Color.White`       |

---

## Botões de Ação (tela de Pedidos)

| Botão             | Cor (RGB)           |
|-------------------|---------------------|
| Confirmar (✓)     | `(39, 174, 96)`     |
| Em Preparo (⏳)   | `(243, 156, 18)`    |
| Pronto (✅)        | `(22, 160, 133)`    |
| Saiu p/ Entrega (→) | `(52, 152, 219)` |
| Entregue (✓)      | `(22, 160, 133)`    |
| Cancelar (✕)      | `(192, 57, 43)`     |

Todos os botões de ação: `ForeColor = Color.White`, `FlatStyle = FlatStyle.Flat`, `BorderSize = 0`.

---

## Filtros / Formulários Gerais

| Elemento               | Cor (RGB)               |
|------------------------|-------------------------|
| Labels de filtro       | `(50, 40, 25)`          |
| Botão "Filtrar"        | `(176, 110, 42)` — branco |
| Botão "Ver Todos" bg   | `(200, 192, 170)`       |
| Botão "Ver Todos" fg   | `(60, 50, 35)`          |
| Texto de resumo rodapé | `(140, 90, 25)`         |
| Painel gastos bg       | `(235, 228, 213)`       |
| Rodapé resumo bg       | `(235, 228, 213)`       |

---

## frmPedidoManual (Pedido Manual)

| Elemento                  | Cor (RGB)           |
|---------------------------|---------------------|
| Top bar bg                | `(176, 110, 42)`    |
| Top bar texto             | `Color.White`       |
| Rodapé bg                 | `(245, 237, 216)`   |
| Sidebar bg                | `(17, 24, 50)`      |
| Lado direito (rightArea)  | `Color.White`       |
| Painel add item bg        | `(245, 237, 216)`   |
| Botão "Salvar Pedido"     | `(224, 113, 42)` — branco, bold |
| Botão "Cancelar" bg       | `(245, 237, 216)`   |
| Botão "Cancelar" border   | `(200, 165, 100)`   |
| Botão "Cancelar" fg       | `(120, 80, 30)`     |
| Botão "Buscar" produto    | `(224, 113, 42)` — branco |
| Botão "+ Adicionar"       | `(87, 120, 38)` — branco, bold |
| Botão "Buscar" cliente    | `(224, 113, 42)` — branco |
| Botão "Aplicar" cupom     | `(224, 113, 42)` — branco |
| Total label               | `(87, 120, 38)`, font 16pt Bold |
| Grid de itens bg          | `(250, 245, 238)`   |
| Grid seleção              | `(224, 113, 42)`    |
| Cabeçalho do grid         | `(240, 230, 202)`   |
| Separadores (dividers)    | `(34, 46, 82)`      |
| Labels de campo           | `(110, 130, 175)`   |
| Labels de seção           | `(80, 105, 160)`    |
| Inputs (TextBox/Numeric)  | bg `(14, 21, 46)`, fg `(50, 50, 50)` |

> **Nota:** O botão "½ + ½ Pizza" (`btnMeioAMeio`) deve ser mantido com `Visible = false`.

---

## Constantes de Referência (Form1.cs)

```csharp
private static readonly Color CorSidebar    = Color.FromArgb(18, 20, 25);
private static readonly Color CorTopBar     = Color.White;
private static readonly Color CorBotaoAtivo = Color.FromArgb(176, 110, 42);
private static readonly Color CorCard       = Color.FromArgb(165, 105, 42);
private static readonly Color CorFundo      = Color.FromArgb(248, 245, 240);
```

---

## Regras Gerais

1. **Nunca usar azul escuro** (`(28, 37, 65)`, `(36, 48, 82)`, etc.) como cor de fundo de painéis ou formulários.
2. **Nunca usar fundo preto/navy** `(15, 22, 45)` fora do sidebar e do formulário de pedido manual.
3. **Bordas de botões**: sempre `FlatStyle.Flat` + `BorderSize = 0` (exceto quando borda intencional outline).
4. **Font padrão**: `Segoe UI`, tamanhos 8–10pt para labels, 9–10pt para botões, 13–20pt para valores de cards.
5. **FlatStyle** obrigatório em todos os botões: `FlatStyle.Flat`.
6. Grids sempre com cabeçalho `(36, 48, 82)` / branco para contraste.
