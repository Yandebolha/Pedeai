-- ============================================================
-- RanGoFood  —  Script de reset completo do banco de dados
-- Apaga todos os dados transacionais e reinicia o sistema
-- como se fosse uma instalação nova.
--
-- ⚠️  ATENÇÃO: NÃO restaura a tabela `loja` (dados da plataforma
--     original). Apenas desativa FKs, trunca as tabelas e
--     repõe os registros iniciais obrigatórios.
--
-- Como executar (MySQL CLI):
--   mysql -u root -p pedeai < reset_db.sql
-- ============================================================

SET FOREIGN_KEY_CHECKS = 0;

-- ── 1. Tabelas transacionais — apagar tudo ────────────────────
TRUNCATE TABLE itens_pedido_web;
TRUNCATE TABLE pedido_web;
TRUNCATE TABLE item_entrada_mercadoria;
TRUNCATE TABLE parcela_entrada_mercadoria;
TRUNCATE TABLE entrada_mercadoria;
TRUNCATE TABLE gasto_material;
TRUNCATE TABLE necessidade_empresa;
TRUNCATE TABLE turno;
TRUNCATE TABLE estoque_item;

-- ── 2. Cadastros operaionais — apagar clientes, fornecedores, cupons ─
TRUNCATE TABLE cliente;
TRUNCATE TABLE cupom;
TRUNCATE TABLE fornecedor;

-- ── 3. Catálogo de produtos — apagar mercadorias e categorias ────────
TRUNCATE TABLE mercadoria;
TRUNCATE TABLE grupo_mercadoria;

-- ── 4. Remover tabelas legadas / não utilizadas (se existirem) ────────
--    Adicione aqui qualquer tabela que existia na plataforma
--    original mas que NÃO é referenciada pelo sistema.
DROP TABLE IF EXISTS produto;
DROP TABLE IF EXISTS categoria;
DROP TABLE IF EXISTS pedido;
DROP TABLE IF EXISTS item_pedido;
DROP TABLE IF EXISTS pagamento;
DROP TABLE IF EXISTS notificacao;
DROP TABLE IF EXISTS configuracao;
DROP TABLE IF EXISTS taxa_entrega;
DROP TABLE IF EXISTS banner;
DROP TABLE IF EXISTS avaliacao;
DROP TABLE IF EXISTS token_dispositivo;
DROP TABLE IF EXISTS endereco_cliente;
DROP TABLE IF EXISTS horario_funcionamento;

SET FOREIGN_KEY_CHECKS = 1;

-- ── 5. Repor registros iniciais obrigatórios ──────────────────────────

-- Empresa padrão (mantém se já existir)
INSERT IGNORE INTO empresa
    (Codigo, empNome, empNome_Fantasia, empCNPJ, empTelefone, empEmail, empEndereco, Info)
VALUES (1, 'Minha Empresa', '', '', '', '', '', '');

-- Configuração de impressão padrão (mantém se já existir)
INSERT IGNORE INTO config_impressao (Codigo) VALUES (1);

-- Usuário admin (senha: $up0rte  →  hash SHA-256)
-- Se o admin já existir somente redefinimos a senha.
INSERT INTO usuario
    (auxCodigo, usuNome, usuLogin, usuSenha, usuNivel, Situacao, Info)
VALUES
    (1, 'Administrador', 'admin',
     '3c79a53d3e8f4ca7d9069e91f9a2e8c69d4a47b3a15e236b5d7e1f8c0a9b2e4d',
     9, 'A', '')
ON DUPLICATE KEY UPDATE
    usuSenha = '3c79a53d3e8f4ca7d9069e91f9a2e8c69d4a47b3a15e236b5d7e1f8c0a9b2e4d',
    usuNivel = 9,
    Situacao = 'A';
-- Nota: o hash acima é gerado pelo DbMigrator na inicialização do sistema.
-- Ao abrir o aplicativo após este reset, o hash é regenerado automaticamente
-- com o algoritmo correto de SHA-256. Basta logar como admin / $up0rte.

-- ============================================================
-- Reset concluído. O sistema está pronto para uso como novo.
-- ============================================================
