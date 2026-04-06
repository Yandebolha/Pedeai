-- ============================================================
-- RanGoFood  —  Script de reset completo do banco de dados
-- Apaga todos os dados EXCETO a tabela `usuario`.
-- Os usuários e senhas são mantidos integralmente.
--
-- Como executar (MySQL CLI):
--   mysql -u root -p pedeai < reset_db.sql
-- ============================================================

SET FOREIGN_KEY_CHECKS = 0;

-- ── 1. Tabelas transacionais ──────────────────────────────────
TRUNCATE TABLE itens_pedido_web;
TRUNCATE TABLE pedido_web;
TRUNCATE TABLE item_entrada_mercadoria;
TRUNCATE TABLE parcela_entrada_mercadoria;
TRUNCATE TABLE entrada_mercadoria;
TRUNCATE TABLE gasto_material;
TRUNCATE TABLE necessidade_empresa;
TRUNCATE TABLE turno;
TRUNCATE TABLE estoque_item;

-- ── 2. Cadastros ──────────────────────────────────────────────
TRUNCATE TABLE cliente;
TRUNCATE TABLE cupom;
TRUNCATE TABLE fornecedor;
TRUNCATE TABLE mercadoria;
TRUNCATE TABLE grupo_mercadoria;

-- ── 3. Configurações (serão recriadas abaixo) ─────────────────
TRUNCATE TABLE empresa;
TRUNCATE TABLE config_impressao;

-- ── 4. Tabelas legadas (remove se existirem) ──────────────────
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

-- ── 5. Repor registros mínimos para o sistema funcionar ───────
--      (usuario NÃO é tocado — todos os usuários são mantidos)

INSERT IGNORE INTO empresa
    (Codigo, empNome, empNome_Fantasia, empCNPJ, empTelefone, empEmail, empEndereco, Info)
VALUES (1, 'Minha Empresa', '', '', '', '', '', '');

INSERT IGNORE INTO config_impressao (Codigo) VALUES (1);

-- ============================================================
-- Reset concluído. Usuários mantidos. Sistema pronto para uso.
-- ============================================================

