---
name: pedeai-business-rules
description: 'Regras de negócio do sistema Pedeai: fluxo de pedidos, estados, validações, fidelização, estoque, turno de caixa, cupons, usuários e esquema de cores. Use para: implementar validações corretamente, entender transições de estado de pedidos, aplicar cores do sistema, depurar lógica de fidelidade ou cupom, entender quais campos são obrigatórios.'
---

# Regras de Negócio — Pedeai

## Paleta de Cores do Sistema

Use estas cores em TODOS os formulários (tanto o projeto principal quanto os módulos de infraestrutura):

| Token | Valor RGB | Uso |
|-------|-----------|-----|
| `CorFundo` | `(248, 245, 240)` | BackColor do Form e painéis principais |
| `CorSidebar` | `(18, 20, 25)` | Sidebar lateral |
| `CorTopBar` | `(176, 110, 42)` | Barra de título / topBar |
| `CorBotaoPrimario` | `(176, 110, 42)` | Botões de ação principal |
| `CorBotaoSalvar` | `(87, 120, 38)` | Botão Salvar / Confirmar |
| `CorBotaoCancelar` | `(224, 113, 42)` | Botão Cancelar / Editar |
| `CorBotaoPerigo` | `(192, 57, 43)` | Botão Excluir / Cancelar pedido |
| `CorBotaoSecundario` | `(200, 192, 170)` | Botões secundários inactivos |
| `CorGrid` | `(250, 245, 238)` | BackgroundColor do DataGridView |
| `CorGridHeader` | `(240, 230, 202)` | ColumnHeadersDefaultCellStyle.BackColor |
| `CorGridSeleção` | `(224, 113, 42)` | SelectionBackColor do DataGridView |
| `CorPainelForm` | `(245, 237, 216)` | Painéis de formulário / rodapé |
| `CorTextoPrimario` | `(50, 50, 50)` | ForeColor de labels e texto |
| `CorTextoSecundario` | `(130, 115, 90)` | Labels secundários / dicas |
| `CorFrontSidebar` | `(195, 185, 165)` | Texto nos botões da sidebar |
| `CorCard1` | `(176, 110, 42)` | Card dashboard Pedidos Hoje |
| `CorCard2` | `(115, 140, 50)` | Card dashboard Faturamento |
| `CorCard3` | `(155, 95, 40)` | Card dashboard Clientes |
| `CorCard4` | `(190, 100, 30)` | Card dashboard Pendentes |

### Regra de aplicação em formulários de módulos de infraestrutura

```csharp
// Aplique no InitializeComponent ou no Load do Form:
this.BackColor                       = Color.FromArgb(248, 245, 240);
this.Font                            = new Font("Segoe UI", 9F);

// TopBar / cabeçalho
topBar.BackColor                     = Color.FromArgb(176, 110, 42);
lblTitulo.ForeColor                  = Color.White;

// Botão primário
btn.BackColor                        = Color.FromArgb(176, 110, 42);
btn.ForeColor                        = Color.White;
btn.FlatStyle                        = FlatStyle.Flat;
btn.FlatAppearance.BorderSize        = 0;

// Botão salvar
btnSalvar.BackColor                  = Color.FromArgb(87, 120, 38);

// Botão perigo
btnExcluir.BackColor                 = Color.FromArgb(192, 57, 43);

// DataGridView
grid.BackgroundColor                 = Color.FromArgb(250, 245, 238);
grid.DefaultCellStyle.BackColor      = Color.White;
grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 113, 42);
grid.DefaultCellStyle.SelectionForeColor = Color.White;
grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 237, 216);
grid.ColumnHeadersDefaultCellStyle.BackColor   = Color.FromArgb(240, 230, 202);
grid.ColumnHeadersDefaultCellStyle.ForeColor   = Color.FromArgb(60, 60, 60);
grid.BorderStyle                     = BorderStyle.None;
grid.GridColor                       = Color.FromArgb(200, 185, 160);
```

---

## Módulo: Pedidos

### Estados do Pedido (`pediSituacao`)

| Valor | Label | Observação |
|-------|-------|------------|
| `0` | Pendente | Estado inicial de todos os pedidos |
| `1` | Confirmado | Imprime cupom + notifica WhatsApp |
| `2` | Em Preparo | Notifica WhatsApp |
| `3` | Pronto | Só para Retirada — finaliza com pagamento |
| `4` | Saiu p/ Entrega | Só para Entrega — notifica WhatsApp |
| `5` | Entregue | Terminal — finaliza com pagamento |
| `6` | Cancelado | Terminal — exige autorização de gerente |

### Transições Permitidas (máquina de estados)

```
0 → 1, 2, 3, 4, 5, 6
1 → 2, 3, 4, 5, 6
2 → 3, 4, 5, 6
3 → 4, 5, 6
4 → 5, 6
5 → (terminal, nenhuma)
6 → (terminal, nenhuma)
```

- Estados `5` e `6` são **terminais** — nenhuma alteração permitida após atingi-los
- **Cancelamento** de qualquer pedido (situação > 0) exige `frmAutorizacao` com usuário nível Gerente (2) ou Admin (9)
- **Finalização com pagamento** (situações 3-Retirada e 5-Entrega): dialog de pagamento obrigatório com discriminação por forma (Dinheiro/Cartão/Pix)
- Se o valor pago for menor que o total, exige autorização de gerente — o autorizador é gravado no pedido

### Validações de Inserção (frmPedidoManual)
- Nome do cliente é **obrigatório**
- Ao menos **1 item** deve ser adicionado
- Taxa de entrega aplicada automaticamente ao selecionar Bairro (se cadastrado)
- Cupom de fidelidade aplicado **automaticamente** ao selecionar cliente (se disponível)

### Tipo de Entrega (`pediTipo_Entrega`)
- `0` = Retirada — finaliza em estado 3 (Pronto)
- `1` = Entrega — finaliza em estado 5 (Entregue) — exige endereço

### Forma de Pagamento (`pediForma_Pagamento`)
- `0` = Dinheiro
- `1` = Cartão
- `2` = Pix

### Origem (`pediOrigem`)
- `0` = Web · `1` = App · `2` / `3` = Manual

---

## Módulo: Clientes

- **Nome/Razão Social** é obrigatório
- `clieTotalPedidos` e `clieTotalGasto` são atualizados automaticamente via `IncrementarTotais()` ao finalizar pedido
- `clieGasto_Mensal` reset automático quando o mês (`clieGasto_Mes_Ref` = `"YYYY-MM"`) muda
- Dados de endereço preenchidos automaticamente via API ViaCEP pelo CEP

---

## Módulo: Fidelização

### Configuração (`config_fidelizacao`, único registro)

- `fidMeta_Tipo`: `'VALOR'` (gasto mensal em R$) ou `'PEDIDOS'` (quantidade no mês)
- `fidMeta_Gasto` deve ser > 0
- Prêmio pode ser:
  - `'CUPOM'` — gera cupom com código `FID{YYYYMMDD_HHmmss}{3 letras do nome}`
  - `'PRODUTO'` — produto gratuito adicionado ao pedido com preço R$ 0,00

### Regras de disparo
- O cliente recebe **no máximo 1 prêmio por mês** por regra
- Disparo automático ao finalizar pagamento de pedido
- Ao disparar, envia notificação via WhatsApp com cupom/produto

### Validade do cupom gerado
- Default: 30 dias a partir da data de emissão (configurável via `fidCupom_Validade`)
- Cupom fidelidade tem `cupomLimite_Usos = 1` (uso único)

---

## Módulo: Cupons

- **Data de validade** não pode ser no passado
- Tipo `'PERCENTUAL'` — máximo 100%
- `cupomLimite_Usos = 0` significa **ilimitado**
- Cupom só é aplicado se o total do pedido ≥ `cupomPedido_Minimo`
- Cupom bloqueado se `cupomUsos_Realizados >= cupomLimite_Usos` (quando limite > 0)

---

## Módulo: Mercadorias

- **Nome** é obrigatório
- **Preço de venda** deve ser > 0
- **Categoria (Codigo_Grupo)** é obrigatória — toda mercadoria pertence a um grupo
- `mercHabilitar_Ifood` e `mercOrdem` são campos de plataforma externa — não expor no UI
- `mercControla_Estoque = 1` → deduz estoque ao confirmar pedido
- `mercApresentacao` = descrição para cardápio online

---

## Módulo: Estoque

- Item marcado com `estoEh_Produto = 1` → sincroniza automaticamente com tabela `mercadoria`
- Na sincronização:
  - `estoPreco_Custo` → `mercPreco_Custo`
  - Se `mercPreco_Venda == 0` → assume custo como venda
  - Ativa `mercControla_Estoque = 1`
- `estoFracao_Entrada`: 1 unidade recebida = N unidades de estoque
- `estoFracao_Saida`: 1 unidade vendida = N debitadas do estoque

---

## Módulo: Entrada de Mercadoria

- **Fornecedor** (ou nome) obrigatório
- **Pelo menos 1 item** com qtde > 0 e custo > 0
- Parcelas monitoradas com alertas por vencimento (vencidas / hoje / 7 dias / 30 dias)
- `itmAtualizar_Custo = 1` → atualiza `mercPreco_Custo` na mercadoria ao salvar

---

## Módulo: Turno de Caixa

- **Um único turno ativo** por vez — bloqueia nova abertura se já existir turno aberto
- Registra automaticamente o nome do operador (da sessão atual)
- Fechamento exige que haja turno aberto

---

## Módulo: Usuários

- **Nome**, **Login** e **Senha** (novos) são obrigatórios
- Senha armazenada como hash SHA-256
- Níveis: `1` = Operador · `2` = Gerente · `9` = Admin
- `Info` = lista de módulos liberados separados por vírgula (ex.: `"Dashboard,Pedidos,Estoque"`)
- Gerente ou Admin pode autorizar cancelamentos e descontos

---

## Módulo: Bairros / Taxa de Entrega

- **Nome do bairro** é obrigatório
- Busca de taxa por combinação exata `baiCidade + baiNome` (case-insensitive)
- Ao selecionar bairro no pedido manual → taxa preenchida automaticamente em `numTaxa`

---

## Licença (integração com PedeaiLicencaServer)

- Chave no formato `YYYYMMDD-XXXXX-XXXXX-XXXXX`
- Validação local sem internet (apenas parse da data + HMAC)
- Renovação automática quando ≤ 7 dias para vencer
- Período de graça (`empData_Graca`): sistema funciona por N dias mesmo sem renovar
- `empCodigo_Empresa` gerado uma única vez na primeira inicialização

---

## Módulos de Acesso (`UsuarioSessao.Info`)

Valores válidos para `Info` (separados por vírgula):

```
Dashboard, Pedidos, ConsultarPedido, Fidelizacao, WhatsApp,
Financeiro, Turno, Produtos, Categorias, Clientes, Fornecedores,
Cupons, Estoque, EntradaMercadoria, Bairros, Avisos, Empresa
```

Módulo ausente → botão não aparece na sidebar para aquele usuário.
