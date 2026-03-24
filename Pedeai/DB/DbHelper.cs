using System;
using System.Data;
using System.Configuration;
using MySqlConnector;

namespace Pedeai.DB;

/// <summary>
/// Auxiliar de conexão MySQL — padrão ConstruFarma (MySqlConnection + MySqlCommand)
/// </summary>
public static class DbHelper
{
    public static string ConnectionString =>
        ConfigurationManager.AppSettings["ConnectionString"]
        ?? "Server=localhost;Database=pedeai;User=root;Password=;Port=3306;CharSet=utf8mb4;";

    public static MySqlConnection AbrirConexao()
    {
        var conn = new MySqlConnection(ConnectionString);
        conn.Open();
        return conn;
    }

    // ── Pedidos ─────────────────────────────────────────────────────────────

    public static DataTable ListarPedidos(string situacao = null, DateTime? data = null)
    {
        var dt = new DataTable();
        using var conn = AbrirConexao();

        var sql = @"SELECT p.Codigo,
                           p.pediNumero       AS Numero,
                           p.pediNome_Cliente AS Cliente,
                           p.pediTelefone_Cliente AS Telefone,
                           p.pediSituacao     AS Status,
                           p.pediForma_Pagamento AS Pagamento,
                           p.pediTipo_Entrega AS Entrega,
                           p.pediValor_Total  AS Total,
                           p.pediOrigem       AS Origem,
                           p.pediData_Lancamento AS DataHora
                    FROM pedido_web p
                    WHERE 1=1 ";

        if (!string.IsNullOrEmpty(situacao)) sql += " AND p.pediSituacao = @sit";
        if (data.HasValue) sql += " AND DATE(p.pediData_Lancamento) = @data";
        sql += " ORDER BY p.pediData_Lancamento DESC LIMIT 200";

        using var cmd = new MySqlCommand(sql, conn);
        if (!string.IsNullOrEmpty(situacao)) cmd.Parameters.AddWithValue("@sit", situacao);
        if (data.HasValue) cmd.Parameters.AddWithValue("@data", data.Value.Date);

        using var da = new MySqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    public static DataTable GetItensPedido(int codigoPedido)
    {
        var dt = new DataTable();
        using var conn = AbrirConexao();
        var sql = @"SELECT i.itpwNome_Mercadoria AS Produto,
                           i.itpwQtde AS Qtde,
                           i.itpwPreco_Unitario AS Unitario,
                           i.itpwSubtotal AS Subtotal,
                           i.itpwObservacoes AS Obs
                    FROM itens_pedido_web i
                    WHERE i.Codigo_Pedido = @cod
                    ORDER BY i.Codigo";
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@cod", codigoPedido);
        using var da = new MySqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    public static void AtualizarSituacaoPedido(int codigo, int novaSituacao)
    {
        using var conn = AbrirConexao();
        var sql = "UPDATE pedido_web SET pediSituacao = @sit, pediData_Atualizacao = NOW() WHERE Codigo = @cod";
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@sit", novaSituacao);
        cmd.Parameters.AddWithValue("@cod", codigo);
        cmd.ExecuteNonQuery();
    }

    // ── Mercadorias ──────────────────────────────────────────────────────────

    public static DataTable ListarMercadorias()
    {
        var dt = new DataTable();
        using var conn = AbrirConexao();
        var sql = @"SELECT m.Codigo, m.mercMercadoria AS Nome,
                           g.grmeDescricao_ AS Categoria,
                           m.mercPreco_Venda AS Preco,
                           m.mercPreco_Promocional AS Promocional,
                           m.mercEstoque_Atual AS Estoque,
                           m.Situacao
                    FROM mercadoria m
                    LEFT JOIN grupo_mercadoria g ON g.Codigo = m.Codigo_Grupo
                    ORDER BY g.grmeOrdem, m.mercOrdem, m.mercMercadoria";
        using var cmd = new MySqlCommand(sql, conn);
        using var da = new MySqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    public static void AlternarSituacaoMercadoria(int codigo)
    {
        using var conn = AbrirConexao();
        var sql = "UPDATE mercadoria SET Situacao = IF(Situacao='A','I','A') WHERE Codigo = @cod";
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@cod", codigo);
        cmd.ExecuteNonQuery();
    }

    // ── Clientes ─────────────────────────────────────────────────────────────

    public static DataTable ListarClientes(string busca = "")
    {
        var dt = new DataTable();
        using var conn = AbrirConexao();
        var sql = @"SELECT Codigo, clieNome_RazaoSocial AS Nome,
                           clieTelefone AS Telefone, clieCelular AS Celular,
                           clieEmail AS Email, clieCidade AS Cidade,
                           clieTotalPedidos AS TotalPedidos,
                           clieTotalGasto AS TotalGasto, Situacao
                    FROM cliente WHERE 1=1";
        if (!string.IsNullOrWhiteSpace(busca))
            sql += " AND (clieNome_RazaoSocial LIKE @b OR clieTelefone LIKE @b OR clieCelular LIKE @b)";
        sql += " ORDER BY clieNome_RazaoSocial LIMIT 200";

        using var cmd = new MySqlCommand(sql, conn);
        if (!string.IsNullOrWhiteSpace(busca)) cmd.Parameters.AddWithValue("@b", $"%{busca}%");
        using var da = new MySqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    // ── Fornecedores ─────────────────────────────────────────────────────────

    public static DataTable ListarFornecedores()
    {
        var dt = new DataTable();
        using var conn = AbrirConexao();
        var sql = @"SELECT Codigo, fornNome_RazaoSocial AS RazaoSocial,
                           fornApelido_Fantasia AS NomeFantasia,
                           fornCPF_CNPJ_ AS CNPJ, fornTelefone AS Telefone,
                           fornEmail AS Email, fornContato AS Contato, Situacao
                    FROM fornecedor ORDER BY fornNome_RazaoSocial";
        using var cmd = new MySqlCommand(sql, conn);
        using var da = new MySqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    // ── Cupons ───────────────────────────────────────────────────────────────

    public static DataTable ListarCupons()
    {
        var dt = new DataTable();
        using var conn = AbrirConexao();
        var sql = @"SELECT Codigo, cupomCodigo AS Cupom, cupomDescricao AS Descricao,
                           cupomTipo AS Tipo, cupomValor AS Valor,
                           cupomUsos_Realizados AS Usos,
                           cupomValido_Ate AS ValidoAte, Situacao
                    FROM cupom ORDER BY cupomData_Cadastro DESC";
        using var cmd = new MySqlCommand(sql, conn);
        using var da = new MySqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    // ── Loja ─────────────────────────────────────────────────────────────────

    public static DataRow GetLoja()
    {
        using var conn = AbrirConexao();
        var sql = "SELECT * FROM loja LIMIT 1";
        using var cmd = new MySqlCommand(sql, conn);
        var dt = new DataTable();
        new MySqlDataAdapter(cmd).Fill(dt);
        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }

    // ── Estatísticas dashboard ───────────────────────────────────────────────

    public static (int pedidosHoje, decimal faturamentoHoje, int clientesTotal, int pedidosPendentes)
        GetEstatisticas()
    {
        using var conn = AbrirConexao();
        var sql = @"SELECT
            (SELECT COUNT(*) FROM pedido_web WHERE DATE(pediData_Lancamento) = CURDATE() AND pediSituacao <> 6) AS pedidosHoje,
            (SELECT COALESCE(SUM(pediValor_Total),0) FROM pedido_web WHERE DATE(pediData_Lancamento) = CURDATE() AND pediSituacao <> 6) AS faturamento,
            (SELECT COUNT(*) FROM cliente) AS clientes,
            (SELECT COUNT(*) FROM pedido_web WHERE pediSituacao IN (0,1,2)) AS pendentes";
        using var cmd = new MySqlCommand(sql, conn);
        using var r = cmd.ExecuteReader();
        if (r.Read())
            return (r.GetInt32(0), r.GetDecimal(1), r.GetInt32(2), r.GetInt32(3));
        return (0, 0, 0, 0);
    }
}
