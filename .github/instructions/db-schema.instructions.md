---
applyTo: "**/*.cs,**/DAL/**,**/BLL/**,**/DB/**,**/*.sql"
---

# RanGoFood — Esquema do Banco de Dados

Engine: MySQL / MariaDB · Charset: utf8mb4  
Conexão configurada em `App.config` → chave `ConnectionString`.  
Migrações automáticas em `Pedeai/DB/DbMigrator.cs` (idempotente, roda a cada inicialização).

---

## Convenções globais

| Coluna | Tipo | Significado |
|---|---|---|
| `Codigo` | INT AUTO_INCREMENT PK | Chave primária de toda tabela |
| `auxCodigo` | INT DEFAULT 1 | Particionamento legado (sempre 1 nesta instalação) |
| `Situacao` | CHAR(1) | `'A'` = Ativo · `'I'` = Inativo · `'C'` = Cancelado · `'P'` = Pago |
| `Status_Transmissao` | CHAR(1) | `'N'` = Não enviado ao hub · `'S'` = Enviado |
| `Info` | VARCHAR(255) | Módulos de acesso do usuário (separados por vírgula) |

---

## Tabelas

### `usuario`
Autenticação e controle de acesso ao sistema.

| Coluna | Tipo | Descrição |
|---|---|---|
| `Codigo` | INT PK | ID do usuário |
| `auxCodigo` | INT | Partição legada |
| `usuNome` | VARCHAR(100) | Nome exibido na interface |
| `usuLogin` | VARCHAR(50) UNIQUE | Login para acesso |
| `usuSenha` | VARCHAR(64) | Hash SHA-256 da senha |
| `usuNivel` | TINYINT | `1`=Operador · `2`=Gerente · `9`=Admin |
| `Situacao` | CHAR(1) | `'A'`=Ativo · `'I'`=Inativo |
| `usuData_Cadastro` | DATETIME | Data de criação |
| `Info` | VARCHAR(255) | Lista de módulos liberados (ex.: `"Dashboard,Pedidos,Estoque"`) |

---

### `empresa`
Dados da empresa cadastrada no sistema (único registro, Codigo=1).

| Coluna | Tipo | Descrição |
|---|---|---|
| `Codigo` | INT PK (=1) | Identificador fixo |
| `empNome` | VARCHAR(100) | Razão social |
| `empNome_Fantasia` | VARCHAR(100) | Nome fantasia / marca |
| `empCNPJ` | VARCHAR(18) | CNPJ formatado |
| `empTelefone` | VARCHAR(20) | Telefone de contato |
| `empEmail` | VARCHAR(100) | Email |
| `empEndereco` | VARCHAR(255) | Endereço completo |
| `Info` | VARCHAR(255) | Campo extra livre |

---

### `loja`
Tabela de configurações da plataforma de pedidos online (legado, somente leitura pelo sistema).  
Não é criada pelo DbMigrator — já deve existir no banco da plataforma original.

---

### `grupo_mercadoria`
Categorias do catálogo de produtos.

| Coluna | Tipo | Descrição |
|---|---|---|
| `Codigo` | INT PK | ID da categoria |
| `auxCodigo` | INT | Partição legada |
| `grmeDescricao_` | VARCHAR(100) | Nome da categoria (exibido na listagem e pedidos) |
| `grmeOrdem` | INT | Ordem de exibição no cardápio |
| `Situacao` | CHAR(1) | `'A'`=Ativa · `'I'`=Inativa |
| `grmeData_Cadastro` | DATETIME | Data de criação (adicionado por migração) |

---

### `mercadoria`
Catálogo de produtos vendáveis.

| Coluna | Tipo | Descrição |
|---|---|---|
| `Codigo` | INT PK | ID do produto |
| `auxCodigo` | INT | Partição legada |
| `Codigo_Grupo` | INT FK→grupo_mercadoria | Categoria do produto |
| `mercMercadoria` | VARCHAR(150) | Nome do produto |
| `mercApresentacao` | TEXT | Descrição/apresentação para o cardápio online |
| `mercPreco_Venda` | DECIMAL(10,2) | Preço de venda |
| `mercPreco_Custo` | DECIMAL(10,2) | Custo unitário (atualizado pela entrada de mercadoria) |
| `mercPreco_Promocional` | DECIMAL(10,2) | Preço promocional (0 = sem promoção) |
| `mercEstoque_Atual` | DECIMAL(12,4) | Quantidade em estoque |
| `mercControla_Estoque` | TINYINT(1) | `1` = deduz estoque ao confirmar pedido |
| `mercImagem_Url` | VARCHAR(300) | URL da imagem do produto |
| `mercDestaque` | TINYINT(1) | `1` = aparece em destaque no cardápio |
| `mercOrdem` | INT | Ordem dentro da categoria |
| `mercHabilitar_Ifood` | TINYINT(1) | `1` = habilitado no iFood (integração) |
| `mercHabilitar_Site` | TINYINT(1) | `1` = visível no site de pedidos online |
| `Situacao` | CHAR(1) | `'A'`=Ativo · `'I'`=Inativo |
| `Status_Transmissao` | CHAR(1) | Controle de sincronização com hub |
| `Info` | VARCHAR(255) | Campo extra livre |
| `mercData_Cadastro` | DATETIME | Data de cadastro |

---

### `pedido_web`
Pedidos recebidos (web, app, manual).

| Coluna | Tipo | Descrição |
|---|---|---|
| `Codigo` | INT PK | ID interno do pedido |
| `auxCodigo` | INT | Partição legada |
| `pediNumero` | VARCHAR(20) | Número visível do pedido (ex.: `"0042"`) |
| `Codigo_Cliente` | INT FK→cliente | Cliente vinculado (NULL para pedidos anônimos) |
| `pediNome_Cliente` | VARCHAR(150) | Nome do cliente no momento do pedido |
| `pediTelefone_Cliente` | VARCHAR(20) | Telefone registrado no pedido |
| `pediSituacao` | TINYINT | `0`=Pendente `1`=Confirmado `2`=Em Preparo `3`=Pronto `4`=Saiu p/ Entrega `5`=Entregue `6`=Cancelado |
| `pediTipo_Entrega` | TINYINT | `0`=Retirada · `1`=Entrega |
| `pediForma_Pagamento` | TINYINT | `0`=Dinheiro · `1`=Cartão · `2`=Pix |
| `pediOrigem` | TINYINT | `0`=Web · `1`=App · `2`/`3`=Manual |
| `pediSubtotal` | DECIMAL(10,2) | Subtotal dos itens (sem taxa de entrega) |
| `pediTaxa_Entrega` | DECIMAL(10,2) | Taxa de entrega |
| `pediDesconto` | DECIMAL(10,2) | Desconto de cupom aplicado |
| `pediValor_Total` | DECIMAL(10,2) | Valor final a cobrar (subtotal + entrega − desconto) |
| `pediTroco_Para` | DECIMAL(10,2) | Valor informado para troco (dinheiro) |
| `pediValor_Pago` | DECIMAL(10,2) | Valor efetivamente recebido (NULL = pago integral) |
| `pediPago_Dinheiro` | DECIMAL(10,2) | Parte paga em dinheiro (pagamento misto) |
| `pediPago_Cartao` | DECIMAL(10,2) | Parte paga no cartão (pagamento misto) |
| `pediPago_Pix` | DECIMAL(10,2) | Parte paga via Pix (pagamento misto) |
| `pediCodigo_Transacao` | VARCHAR(100) | Código da transação (Pix/cartão) |
| `pediEndereco_Entrega` | VARCHAR(300) | Endereço de entrega completo |
| `pediObservacoes` | TEXT | Observações do pedido |
| `pediCancelado_Por` | VARCHAR(100) | Nome do usuário que autorizou o cancelamento |
| `pediData_Lancamento` | DATETIME | Data/hora do lançamento do pedido |
| `pediData_Atualizacao` | DATETIME | Última atualização de status |
| `Situacao` | CHAR(1) | `'A'`=Ativo (ativo no BD, não confundir com pediSituacao) |
| `Status_Transmissao` | CHAR(1) | Controle de sincronização com hub |
| `Info` | VARCHAR(255) | Campo extra livre |

---

### `itens_pedido_web`
Itens de cada pedido.

| Coluna | Tipo | Descrição |
|---|---|---|
| `Codigo` | INT PK | ID do item |
| `auxCodigo` | INT | Partição legada |
| `Codigo_Pedido` | INT FK→pedido_web | Pedido pai |
| `Codigo_Mercadoria` | INT FK→mercadoria | Produto do catálogo (0 = produto deletado) |
| `itpwNome_Mercadoria` | VARCHAR(200) | Nome do produto no momento do pedido (imutável) |
| `itpwQtde` | DECIMAL(10,4) | Quantidade pedida (0.5 = meia porção) |
| `itpwPreco_Unitario` | DECIMAL(10,2) | Preço unitário no momento do pedido |
| `itpwSubtotal` | DECIMAL(10,2) | Qtde × preço unitário |
| `itpwObservacoes` | VARCHAR(300) | Observações do item (ex.: "sem cebola") |
| `Situacao` | CHAR(1) | `'A'`=Ativo |
| `Status_Transmissao` | CHAR(1) | Controle de sincronização |
| `Info` | VARCHAR(255) | Campo extra livre |

---

### `cliente`
Cadastro de clientes.

| Coluna | Tipo | Descrição |
|---|---|---|
| `Codigo` | INT PK | ID do cliente |
| `auxCodigo` | INT | Partição legada |
| `clieNome_RazaoSocial` | VARCHAR(150) | Nome completo ou razão social |
| `clieTelefone` | VARCHAR(20) | Telefone fixo |
| `clieCelular` | VARCHAR(20) | Celular / WhatsApp |
| `clieEmail` | VARCHAR(100) | Email |
| `clieCPF_CNPJ_` | VARCHAR(18) | CPF ou CNPJ |
| `clieCEP` | VARCHAR(10) | CEP |
| `clieEndereco` | VARCHAR(200) | Logradouro |
| `clieNumero` | VARCHAR(20) | Número |
| `clieComplemento` | VARCHAR(100) | Complemento |
| `clieBairro` | VARCHAR(100) | Bairro |
| `clieCidade` | VARCHAR(100) | Cidade |
| `clieEstado` | VARCHAR(2) | UF |
| `clieTotalPedidos` | INT | Contador de pedidos (atualizado automaticamente ao confirmar pedido) |
| `clieTotalGasto` | DECIMAL(12,2) | Soma total gasta (atualizado automaticamente) |
| `clieData_Cadastro` | DATETIME | Data de cadastro |
| `Situacao` | CHAR(1) | `'A'`=Ativo · `'I'`=Inativo |
| `Status_Transmissao` | CHAR(1) | Controle de sincronização |
| `Info` | VARCHAR(255) | Campo extra livre |

---

### `fornecedor`
Cadastro de fornecedores.

| Coluna | Tipo | Descrição |
|---|---|---|
| `Codigo` | INT PK | ID do fornecedor |
| `auxCodigo` | INT | Partição legada |
| `fornNome_RazaoSocial` | VARCHAR(150) | Razão social |
| `fornApelido_Fantasia` | VARCHAR(100) | Nome fantasia |
| `fornCPF_CNPJ_` | VARCHAR(18) | CPF ou CNPJ |
| `fornRG_InscricaoEstadual` | VARCHAR(30) | RG ou Inscrição Estadual |
| `fornTelefone` | VARCHAR(20) | Telefone |
| `fornEmail` | VARCHAR(100) | Email |
| `fornContato` | VARCHAR(100) | Nome da pessoa de contato |
| `fornCEP` | VARCHAR(10) | CEP (preenchido automaticamente via API ViaCEP) |
| `fornEndereco` | VARCHAR(200) | Logradouro |
| `fornNumero` | VARCHAR(20) | Número |
| `fornBairro` | VARCHAR(100) | Bairro |
| `fornCidade` | VARCHAR(100) | Cidade |
| `fornEstado` | VARCHAR(2) | UF |
| `fornObservacoes` | VARCHAR(500) | Observações gerais |
| `fornData_Cadastro` | DATETIME | Data de cadastro |
| `Situacao` | CHAR(1) | `'A'`=Ativo · `'I'`=Inativo |
| `Status_Transmissao` | CHAR(1) | Controle de sincronização |
| `Info` | VARCHAR(255) | Campo extra livre |

---

### `cupom`
Cupons de desconto aplicáveis no checkout.

| Coluna | Tipo | Descrição |
|---|---|---|
| `Codigo` | INT PK | ID do cupom |
| `auxCodigo` | INT | Partição legada |
| `cupomCodigo` | VARCHAR(50) UNIQUE | Código digitado pelo cliente (ex.: `"PROMO10"`) |
| `cupomDescricao` | VARCHAR(200) | Descrição interna |
| `cupomTipo` | VARCHAR(20) | `'PERCENTUAL'` = % sobre o total · `'FIXO'` = valor fixo |
| `cupomValor` | DECIMAL(10,2) | Percentual (%) ou valor fixo (R$) de desconto |
| `cupomPedido_Minimo` | DECIMAL(10,2) | Valor mínimo do pedido para ativar o cupom |
| `cupomLimite_Usos` | INT | Quantidade máxima de usos (0 = ilimitado) |
| `cupomUsos_Realizados` | INT | Quantidade de usos já realizados |
| `cupomValido_Ate` | DATE | Data de expiração |
| `cupomData_Cadastro` | DATETIME | Data de cadastro |
| `Situacao` | CHAR(1) | `'A'`=Ativo · `'I'`=Inativo |
| `Status_Transmissao` | CHAR(1) | Controle de sincronização |
| `Info` | VARCHAR(255) | Campo extra livre |

---

### `gasto_material`
Gastos com materiais e insumos (lançados manualmente no Financeiro).

| Coluna | Tipo | Descrição |
|---|---|---|
| `Codigo` | INT PK | ID do gasto |
| `auxCodigo` | INT | Partição legada |
| `gmatData` | DATE | Data do gasto |
| `gmatDescricao` | VARCHAR(255) | Descrição do gasto |
| `gmatValor` | DECIMAL(10,2) | Valor gasto em R$ |
| `gmatObservacoes` | VARCHAR(500) | Observações adicionais |
| `Situacao` | CHAR(1) | `'A'`=Ativo · `'I'`=Excluído |
| `Info` | VARCHAR(255) | Campo extra livre |

---

### `necessidade_empresa`
Despesas fixas recorrentes (aluguel, energia, internet etc.).

| Coluna | Tipo | Descrição |
|---|---|---|
| `Codigo` | INT PK | ID da necessidade |
| `auxCodigo` | INT | Partição legada |
| `nempData` | DATE | Competência do lançamento |
| `nempCategoria` | VARCHAR(50) | Categoria (ex.: `"Aluguel"`, `"Energia"`) |
| `nempDescricao` | VARCHAR(200) | Descrição detalhada |
| `nempValor` | DECIMAL(10,2) | Valor em R$ |
| `Situacao` | CHAR(1) | `'A'`=Ativo · `'I'`=Excluído |
| `Info` | TEXT | Campo extra livre |

---

### `entrada_mercadoria`
Cabeçalho de entrada de mercadoria (nota fiscal de compra).

| Coluna | Tipo | Descrição |
|---|---|---|
| `Codigo` | INT PK | ID da entrada |
| `auxCodigo` | INT | Partição legada |
| `Codigo_Fornecedor` | INT FK→fornecedor | Fornecedor (pode ser NULL para entradas avulsas) |
| `entNome_Fornecedor` | VARCHAR(150) | Nome do fornecedor (gravado no ato da entrada) |
| `entData` | DATE | Data da entrada/nota |
| `entNumeroDoc` | VARCHAR(50) | Número do documento fiscal |
| `entObservacoes` | VARCHAR(500) | Observações |
| `entValorTotal` | DECIMAL(12,2) | Valor total da entrada |
| `Situacao` | CHAR(1) | `'A'`=Ativa · `'C'`=Cancelada |
| `Info` | VARCHAR(255) | Campo extra livre |
| `entData_Lancamento` | DATETIME | Data/hora do lançamento no sistema |

---

### `item_entrada_mercadoria`
Itens de uma entrada de mercadoria.

| Coluna | Tipo | Descrição |
|---|---|---|
| `Codigo` | INT PK | ID do item |
| `auxCodigo` | INT | Partição legada |
| `Codigo_Entrada` | INT FK→entrada_mercadoria | Entrada pai |
| `Codigo_Mercadoria` | INT FK→mercadoria | Produto do catálogo (0 = não vinculado) |
| `itmNome_Mercadoria` | VARCHAR(150) | Nome do produto na nota |
| `itmQtde` | DECIMAL(12,4) | Quantidade recebida |
| `itmPreco_Custo` | DECIMAL(12,4) | Preço de custo unitário |
| `itmSubtotal` | DECIMAL(12,2) | Qtde × custo |
| `itmAtualizar_Custo` | TINYINT(1) | `1` = atualiza `mercPreco_Custo` na mercadoria ao salvar |
| `Situacao` | CHAR(1) | `'A'`=Ativo |

---

### `parcela_entrada_mercadoria`
Parcelas de pagamento de uma entrada de mercadoria.

| Coluna | Tipo | Descrição |
|---|---|---|
| `Codigo` | INT PK | ID da parcela |
| `auxCodigo` | INT | Partição legada |
| `Codigo_Entrada` | INT FK→entrada_mercadoria | Entrada pai |
| `parNumero` | TINYINT | Número sequencial da parcela (1, 2, 3…) |
| `parVencimento` | DATE | Data de vencimento |
| `parValor` | DECIMAL(12,2) | Valor da parcela em R$ |
| `parObservacao` | VARCHAR(250) | Observação (ex.: `"Boleto 123"`) |
| `Situacao` | CHAR(1) | `'A'`=Aberta (a pagar) · `'P'`=Paga |
| `parData_Pagamento` | DATE | Data em que o pagamento foi registrado |

---

### `turno`
Registro de abertura e fechamento de caixa.

| Coluna | Tipo | Descrição |
|---|---|---|
| `Codigo` | INT PK | ID do turno |
| `auxCodigo` | INT | Partição legada |
| `turAbertura` | DATETIME | Data/hora de abertura do caixa |
| `turFechamento` | DATETIME | Data/hora de fechamento (NULL quando ainda aberto) |
| `turUsuario` | VARCHAR(100) | Nome do operador que abriu o caixa |
| `turCaixa_Inicial` | DECIMAL(10,2) | Valor informado no fundo de caixa |
| `turCaixa_Final` | DECIMAL(10,2) | Valor contado no fechamento |
| `turObservacao` | VARCHAR(500) | Observações do turno |
| `turSituacao` | CHAR(1) | `'A'`=Aberto · `'F'`=Fechado |

---

### `estoque_item`
Itens de estoque independentes do catálogo de produtos. Permite controlar matérias-primas e insumos que não são vendidos diretamente.

| Coluna | Tipo | Descrição |
|---|---|---|
| `Codigo` | INT PK | ID do item de estoque |
| `estoNome` | VARCHAR(150) | Nome do item (ex.: `"Farinha de trigo"`) |
| `estoUnidade` | VARCHAR(20) | Unidade de medida (ex.: `"kg"`, `"un"`, `"l"`) |
| `estoQtde_Atual` | DECIMAL(12,4) | Quantidade atual em estoque |
| `estoPreco_Custo` | DECIMAL(12,4) | Custo unitário médio |
| `estoEstoque_Min` | DECIMAL(12,4) | Quantidade mínima desejada (alerta visual) |
| `estoEh_Produto` | TINYINT(1) | `1` = sincronizar com `mercadoria` (este item também é vendido) |
| `estoFracao_Entrada` | DECIMAL(12,4) | Fator de conversão na entrada: 1 unidade recebida = N unidades em estoque (padrão `1`) |
| `estoFracao_Entrada_Unidade` | VARCHAR(20) | Unidade de medida da fração de entrada (ex.: `"cx"`, `"kg"`) |
| `estoFracao_Saida` | DECIMAL(12,4) | Fator de conversão na saída: 1 unidade do catálogo vendida = N unidades debitadas do estoque (padrão `1`) |
| `estoFracao_Saida_Unidade` | VARCHAR(20) | Unidade de medida da fração de saída (ex.: `"un"`, `"g"`) |
| `Codigo_Grupo` | INT FK→grupo_mercadoria | Categoria do catálogo a vincular quando `estoEh_Produto=1` |
| `Codigo_Mercadoria` | INT FK→mercadoria | Produto do catálogo vinculado (preenchido quando `estoEh_Produto=1`) |
| `Situacao` | CHAR(1) | `'A'`=Ativo · `'I'`=Inativo |
| `estoData_Cadastro` | DATETIME | Data de cadastro |

---

### `config_impressao`
Configurações da impressora térmica (único registro, Codigo=1).

| Coluna | Tipo | Descrição |
|---|---|---|
| `Codigo` | INT PK (=1) | Registro único |
| `cabNomeEmpresa` | VARCHAR(100) | Nome impresso no cabeçalho do cupom |
| `cabEndereco` | VARCHAR(200) | Endereço no cabeçalho |
| `cabTelefone` | VARCHAR(50) | Telefone no cabeçalho |
| `cabCNPJ` | VARCHAR(30) | CNPJ no cabeçalho |
| `separador` | VARCHAR(5) | Caractere separador de seções (ex.: `"-"`, `"="`) |
| `rodapeAvisoFiscal` | VARCHAR(100) | Texto de aviso fiscal no rodapé |
| `rodapeTextoLivre` | TEXT | Texto livre no rodapé (suporta `\n`) |
| `lblNumeroPedido` | VARCHAR(50) | Rótulo do número do pedido |
| `lblColunaItem` | VARCHAR(50) | Cabeçalho da coluna de itens |
| `lblColunaTotal` | VARCHAR(30) | Cabeçalho da coluna de total |
| `lblSubtotal` | VARCHAR(50) | Rótulo do subtotal |
| `lblTaxaEntrega` | VARCHAR(50) | Rótulo da taxa de entrega |
| `lblTotalPagar` | VARCHAR(50) | Rótulo do total a pagar |
| `lblAtendente` | VARCHAR(50) | Rótulo do atendente |
| `larguraCaracteres` | INT | Largura da impressora em caracteres (padrão: 42) |
| `impressoraNome` | VARCHAR(200) | Nome da impressora instalada no Windows |

---

## Relacionamentos resumidos

```
empresa (1)
usuario (N)
grupo_mercadoria (N) ←── mercadoria (N) ←── itens_pedido_web (N) ──→ pedido_web (N) ──→ cliente (1)
                                      ↑
                          estoque_item (N, opcional Codigo_Mercadoria)
fornecedor (1) ──→ entrada_mercadoria (N) ──→ item_entrada_mercadoria (N)
                                        └──→ parcela_entrada_mercadoria (N)
turno (N)
cupom (N) ──[aplicado em]──→ pedido_web.pediDesconto
gasto_material (N)
necessidade_empresa (N)
config_impressao (1)
loja (1, legado somente-leitura)
```

## Script de reset

Para apagar todos os dados transacionais e reiniciar o sistema como novo:  
`Pedeai/DB/reset_db.sql`
