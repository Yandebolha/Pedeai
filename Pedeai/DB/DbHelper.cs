using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using MySqlConnector;

namespace Pedeai.DB
{
/// <summary>
/// Auxiliar de conexão MySQL — padrão ConstruFarma (MySqlConnection + MySqlCommand)
/// </summary>
public static class DbHelper
{
    public static string ConnectionString =>
        ConfigurationManager.AppSettings["ConnectionString"]
        ?? "Server=localhost;Database=pedeai;User=root;Password=;Port=3306;CharSet=utf8mb4;SslMode=None;";

    public static MySqlConnection AbrirConexao()
    {
        var conn = new MySqlConnection(ConnectionString);
        conn.Open();
        return conn;
    }

    /// <summary>Testa se a conexão com o banco está disponível. Retorna false em caso de falha.</summary>
    public static bool TestarConexao()
    {
        try
        {
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            return true;
        }
        catch { return false; }
    }

    // ── Sequência (padrão ConstruFarma) ─────────────────────────────────────

    private static int ProximoCodigo(string tabela)
    {
        using var conn = AbrirConexao();
        using var cmd = new MySqlCommand(
            "SELECT COALESCE(MAX(Codigo),0)+1 FROM `" + tabela + "`", conn);
        var result = cmd.ExecuteScalar();
        return result == null || result == DBNull.Value ? 1 : Convert.ToInt32(result);
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

    // ── Categorias (grupo_mercadoria) ────────────────────────────────────────

    public static DataTable ListarCategorias(bool apenasAtivas = false)
    {
        var dt = new DataTable();
        using var conn = AbrirConexao();
        var sql = @"SELECT Codigo, grmeDescricao_ AS Nome, grmeOrdem AS Ordem, Situacao
                    FROM grupo_mercadoria" +
                    (apenasAtivas ? " WHERE Situacao='A'" : "") +
                    " ORDER BY grmeOrdem, grmeDescricao_";
        using var cmd = new MySqlCommand(sql, conn);
        using var da = new MySqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    public static DataRow GetCategoria(int codigo)
    {
        var dt = new DataTable();
        using var conn = AbrirConexao();
        using var cmd = new MySqlCommand(
            "SELECT * FROM grupo_mercadoria WHERE Codigo=@cod LIMIT 1", conn);
        cmd.Parameters.AddWithValue("@cod", codigo);
        using var da = new MySqlDataAdapter(cmd);
        da.Fill(dt);
        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }

    public static string SalvarCategoria(int codigo, string nome, int ordem, string situacao)
    {
        try
        {
            using var conn = AbrirConexao();
            if (codigo == 0)
            {
                var novo = ProximoCodigo("grupo_mercadoria");
                var sql = @"INSERT INTO grupo_mercadoria
                            (auxCodigo,Codigo,grmeDescricao_,grmeOrdem,Situacao,Status_Transmissao,Info,grmeData_Cadastro)
                            VALUES(0,@cod,@nome,@ordem,@sit,'N','',NOW())";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cod", novo);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@ordem", ordem);
                cmd.Parameters.AddWithValue("@sit", situacao);
                cmd.ExecuteNonQuery();
            }
            else
            {
                var sql = "UPDATE grupo_mercadoria SET grmeDescricao_=@nome,grmeOrdem=@ordem,Situacao=@sit WHERE Codigo=@cod";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@ordem", ordem);
                cmd.Parameters.AddWithValue("@sit", situacao);
                cmd.Parameters.AddWithValue("@cod", codigo);
                cmd.ExecuteNonQuery();
            }
            return "";
        }
        catch (Exception ex) { return ex.Message; }
    }

    // ── Produtos (mercadoria) ────────────────────────────────────────────────

    public static DataRow GetProduto(int codigo)
    {
        var dt = new DataTable();
        using var conn = AbrirConexao();
        using var cmd = new MySqlCommand(
            "SELECT * FROM mercadoria WHERE Codigo=@cod LIMIT 1", conn);
        cmd.Parameters.AddWithValue("@cod", codigo);
        using var da = new MySqlDataAdapter(cmd);
        da.Fill(dt);
        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }

    public static string SalvarProduto(int codigo, int codigoGrupo, string nome,
        string descricao, decimal preco, decimal precoPromo,
        decimal estoque, bool controlaEstoque, string imagemUrl,
        bool destaque, int ordem, bool habilIfood, string situacao)
    {
        try
        {
            using var conn = AbrirConexao();
            if (codigo == 0)
            {
                var novo = ProximoCodigo("mercadoria");
                var sql = @"INSERT INTO mercadoria
                            (auxCodigo,Codigo,Codigo_Grupo,mercMercadoria,mercApresentacao,
                             mercPreco_Venda,mercPreco_Promocional,mercEstoque_Atual,
                             mercControla_Estoque,mercImagem_Url,mercDestaque,mercOrdem,
                             mercHabilitar_Ifood,Situacao,Status_Transmissao,Info,mercData_Cadastro)
                            VALUES(0,@cod,@grp,@nome,@desc,@preco,@promo,@est,@ctrl,@img,
                                   @dest,@ordem,@ifood,@sit,'N','',NOW())";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cod", novo);
                cmd.Parameters.AddWithValue("@grp", codigoGrupo);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@desc", descricao ?? "");
                cmd.Parameters.AddWithValue("@preco", preco);
                cmd.Parameters.AddWithValue("@promo", precoPromo);
                cmd.Parameters.AddWithValue("@est", estoque);
                cmd.Parameters.AddWithValue("@ctrl", controlaEstoque ? 1 : 0);
                cmd.Parameters.AddWithValue("@img", imagemUrl ?? "");
                cmd.Parameters.AddWithValue("@dest", destaque ? 1 : 0);
                cmd.Parameters.AddWithValue("@ordem", ordem);
                cmd.Parameters.AddWithValue("@ifood", habilIfood ? 1 : 0);
                cmd.Parameters.AddWithValue("@sit", situacao);
                cmd.ExecuteNonQuery();
            }
            else
            {
                var sql = @"UPDATE mercadoria SET
                            Codigo_Grupo=@grp,mercMercadoria=@nome,mercApresentacao=@desc,
                            mercPreco_Venda=@preco,mercPreco_Promocional=@promo,mercEstoque_Atual=@est,
                            mercControla_Estoque=@ctrl,mercImagem_Url=@img,mercDestaque=@dest,
                            mercOrdem=@ordem,mercHabilitar_Ifood=@ifood,Situacao=@sit
                            WHERE Codigo=@cod";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@grp", codigoGrupo);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@desc", descricao ?? "");
                cmd.Parameters.AddWithValue("@preco", preco);
                cmd.Parameters.AddWithValue("@promo", precoPromo);
                cmd.Parameters.AddWithValue("@est", estoque);
                cmd.Parameters.AddWithValue("@ctrl", controlaEstoque ? 1 : 0);
                cmd.Parameters.AddWithValue("@img", imagemUrl ?? "");
                cmd.Parameters.AddWithValue("@dest", destaque ? 1 : 0);
                cmd.Parameters.AddWithValue("@ordem", ordem);
                cmd.Parameters.AddWithValue("@ifood", habilIfood ? 1 : 0);
                cmd.Parameters.AddWithValue("@sit", situacao);
                cmd.Parameters.AddWithValue("@cod", codigo);
                cmd.ExecuteNonQuery();
            }
            return "";
        }
        catch (Exception ex) { return ex.Message; }
    }

    // ── Clientes ─────────────────────────────────────────────────────────────

    public static DataRow GetCliente(int codigo)
    {
        var dt = new DataTable();
        using var conn = AbrirConexao();
        using var cmd = new MySqlCommand(
            "SELECT * FROM cliente WHERE Codigo=@cod LIMIT 1", conn);
        cmd.Parameters.AddWithValue("@cod", codigo);
        using var da = new MySqlDataAdapter(cmd);
        da.Fill(dt);
        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }

    public static string SalvarCliente(int codigo, string nome, string telefone, string celular,
        string email, string cpf, string cep, string endereco, string numero,
        string complemento, string bairro, string cidade, string estado, string situacao)
    {
        try
        {
            using var conn = AbrirConexao();
            if (codigo == 0)
            {
                var novo = ProximoCodigo("cliente");
                var sql = @"INSERT INTO cliente
                            (auxCodigo,Codigo,clieNome_RazaoSocial,clieTelefone,clieCelular,
                             clieEmail,clieCPF_CNPJ_,clieCEP,clieEndereco,clieNumero,
                             clieComplemento,clieBairro,clieCidade,clieEstado,
                             clieTotalPedidos,clieTotalGasto,clieData_Cadastro,Situacao,Status_Transmissao,Info)
                            VALUES(0,@cod,@nome,@tel,@cel,@email,@cpf,@cep,@end,@num,
                                   @comp,@bairro,@cidade,@estado,0,0,NOW(),@sit,'N','')";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cod", novo);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@tel", telefone ?? "");
                cmd.Parameters.AddWithValue("@cel", celular ?? "");
                cmd.Parameters.AddWithValue("@email", email ?? "");
                cmd.Parameters.AddWithValue("@cpf", cpf ?? "");
                cmd.Parameters.AddWithValue("@cep", cep ?? "");
                cmd.Parameters.AddWithValue("@end", endereco ?? "");
                cmd.Parameters.AddWithValue("@num", numero ?? "");
                cmd.Parameters.AddWithValue("@comp", complemento ?? "");
                cmd.Parameters.AddWithValue("@bairro", bairro ?? "");
                cmd.Parameters.AddWithValue("@cidade", cidade ?? "");
                cmd.Parameters.AddWithValue("@estado", estado ?? "");
                cmd.Parameters.AddWithValue("@sit", situacao);
                cmd.ExecuteNonQuery();
            }
            else
            {
                var sql = @"UPDATE cliente SET
                            clieNome_RazaoSocial=@nome,clieTelefone=@tel,clieCelular=@cel,
                            clieEmail=@email,clieCPF_CNPJ_=@cpf,clieCEP=@cep,
                            clieEndereco=@end,clieNumero=@num,clieComplemento=@comp,
                            clieBairro=@bairro,clieCidade=@cidade,clieEstado=@estado,Situacao=@sit
                            WHERE Codigo=@cod";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@tel", telefone ?? "");
                cmd.Parameters.AddWithValue("@cel", celular ?? "");
                cmd.Parameters.AddWithValue("@email", email ?? "");
                cmd.Parameters.AddWithValue("@cpf", cpf ?? "");
                cmd.Parameters.AddWithValue("@cep", cep ?? "");
                cmd.Parameters.AddWithValue("@end", endereco ?? "");
                cmd.Parameters.AddWithValue("@num", numero ?? "");
                cmd.Parameters.AddWithValue("@comp", complemento ?? "");
                cmd.Parameters.AddWithValue("@bairro", bairro ?? "");
                cmd.Parameters.AddWithValue("@cidade", cidade ?? "");
                cmd.Parameters.AddWithValue("@estado", estado ?? "");
                cmd.Parameters.AddWithValue("@sit", situacao);
                cmd.Parameters.AddWithValue("@cod", codigo);
                cmd.ExecuteNonQuery();
            }
            return "";
        }
        catch (Exception ex) { return ex.Message; }
    }

    // ── Fornecedores ─────────────────────────────────────────────────────────

    public static DataRow GetFornecedor(int codigo)
    {
        var dt = new DataTable();
        using var conn = AbrirConexao();
        using var cmd = new MySqlCommand(
            "SELECT * FROM fornecedor WHERE Codigo=@cod LIMIT 1", conn);
        cmd.Parameters.AddWithValue("@cod", codigo);
        using var da = new MySqlDataAdapter(cmd);
        da.Fill(dt);
        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }

    public static string SalvarFornecedor(int codigo, string razaoSocial, string fantasia,
        string cnpj, string ie, string telefone, string email, string contato,
        string cep, string endereco, string numero, string bairro, string cidade,
        string estado, string obs, string situacao)
    {
        try
        {
            using var conn = AbrirConexao();
            if (codigo == 0)
            {
                var novo = ProximoCodigo("fornecedor");
                var sql = @"INSERT INTO fornecedor
                            (auxCodigo,Codigo,fornNome_RazaoSocial,fornApelido_Fantasia,fornCPF_CNPJ_,
                             fornRG_InscricaoEstadual,fornTelefone,fornEmail,fornContato,fornCEP,
                             fornEndereco,fornNumero,fornBairro,fornCidade,fornEstado,
                             fornObservacoes,fornData_Cadastro,Situacao,Status_Transmissao,Info)
                            VALUES(0,@cod,@razao,@fantasia,@cnpj,@ie,@tel,@email,@cont,@cep,
                                   @end,@num,@bairro,@cidade,@estado,@obs,NOW(),@sit,'N','')";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cod", novo);
                cmd.Parameters.AddWithValue("@razao", razaoSocial);
                cmd.Parameters.AddWithValue("@fantasia", fantasia ?? "");
                cmd.Parameters.AddWithValue("@cnpj", cnpj ?? "");
                cmd.Parameters.AddWithValue("@ie", ie ?? "");
                cmd.Parameters.AddWithValue("@tel", telefone ?? "");
                cmd.Parameters.AddWithValue("@email", email ?? "");
                cmd.Parameters.AddWithValue("@cont", contato ?? "");
                cmd.Parameters.AddWithValue("@cep", cep ?? "");
                cmd.Parameters.AddWithValue("@end", endereco ?? "");
                cmd.Parameters.AddWithValue("@num", numero ?? "");
                cmd.Parameters.AddWithValue("@bairro", bairro ?? "");
                cmd.Parameters.AddWithValue("@cidade", cidade ?? "");
                cmd.Parameters.AddWithValue("@estado", estado ?? "");
                cmd.Parameters.AddWithValue("@obs", obs ?? "");
                cmd.Parameters.AddWithValue("@sit", situacao);
                cmd.ExecuteNonQuery();
            }
            else
            {
                var sql = @"UPDATE fornecedor SET
                            fornNome_RazaoSocial=@razao,fornApelido_Fantasia=@fantasia,fornCPF_CNPJ_=@cnpj,
                            fornRG_InscricaoEstadual=@ie,fornTelefone=@tel,fornEmail=@email,fornContato=@cont,
                            fornCEP=@cep,fornEndereco=@end,fornNumero=@num,fornBairro=@bairro,
                            fornCidade=@cidade,fornEstado=@estado,fornObservacoes=@obs,Situacao=@sit
                            WHERE Codigo=@cod";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@razao", razaoSocial);
                cmd.Parameters.AddWithValue("@fantasia", fantasia ?? "");
                cmd.Parameters.AddWithValue("@cnpj", cnpj ?? "");
                cmd.Parameters.AddWithValue("@ie", ie ?? "");
                cmd.Parameters.AddWithValue("@tel", telefone ?? "");
                cmd.Parameters.AddWithValue("@email", email ?? "");
                cmd.Parameters.AddWithValue("@cont", contato ?? "");
                cmd.Parameters.AddWithValue("@cep", cep ?? "");
                cmd.Parameters.AddWithValue("@end", endereco ?? "");
                cmd.Parameters.AddWithValue("@num", numero ?? "");
                cmd.Parameters.AddWithValue("@bairro", bairro ?? "");
                cmd.Parameters.AddWithValue("@cidade", cidade ?? "");
                cmd.Parameters.AddWithValue("@estado", estado ?? "");
                cmd.Parameters.AddWithValue("@obs", obs ?? "");
                cmd.Parameters.AddWithValue("@sit", situacao);
                cmd.Parameters.AddWithValue("@cod", codigo);
                cmd.ExecuteNonQuery();
            }
            return "";
        }
        catch (Exception ex) { return ex.Message; }
    }

    // ── Cupons ───────────────────────────────────────────────────────────────

    public static DataRow GetCupom(int codigo)
    {
        var dt = new DataTable();
        using var conn = AbrirConexao();
        using var cmd = new MySqlCommand("SELECT * FROM cupom WHERE Codigo=@cod LIMIT 1", conn);
        cmd.Parameters.AddWithValue("@cod", codigo);
        using var da = new MySqlDataAdapter(cmd);
        da.Fill(dt);
        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }

    public static string SalvarCupom(int codigo, string cupomCod, string descricao,
        string tipo, decimal valor, decimal pedidoMinimo,
        int limiteUsos, DateTime validoAte, string situacao)
    {
        try
        {
            using var conn = AbrirConexao();
            if (codigo == 0)
            {
                var novo = ProximoCodigo("cupom");
                var sql = @"INSERT INTO cupom
                            (auxCodigo,Codigo,cupomCodigo,cupomDescricao,cupomTipo,cupomValor,
                             cupomPedido_Minimo,cupomLimite_Usos,cupomUsos_Realizados,
                             cupomValido_Ate,cupomData_Cadastro,Situacao,Status_Transmissao,Info)
                            VALUES(0,@cod,@cupom,@desc,@tipo,@valor,@minimo,@limite,0,@valido,NOW(),@sit,'N','')";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cod", novo);
                cmd.Parameters.AddWithValue("@cupom", cupomCod);
                cmd.Parameters.AddWithValue("@desc", descricao ?? "");
                cmd.Parameters.AddWithValue("@tipo", tipo);
                cmd.Parameters.AddWithValue("@valor", valor);
                cmd.Parameters.AddWithValue("@minimo", pedidoMinimo);
                cmd.Parameters.AddWithValue("@limite", limiteUsos);
                cmd.Parameters.AddWithValue("@valido", validoAte.Date);
                cmd.Parameters.AddWithValue("@sit", situacao);
                cmd.ExecuteNonQuery();
            }
            else
            {
                var sql = @"UPDATE cupom SET
                            cupomCodigo=@cupom,cupomDescricao=@desc,cupomTipo=@tipo,cupomValor=@valor,
                            cupomPedido_Minimo=@minimo,cupomLimite_Usos=@limite,cupomValido_Ate=@valido,Situacao=@sit
                            WHERE Codigo=@cod";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cupom", cupomCod);
                cmd.Parameters.AddWithValue("@desc", descricao ?? "");
                cmd.Parameters.AddWithValue("@tipo", tipo);
                cmd.Parameters.AddWithValue("@valor", valor);
                cmd.Parameters.AddWithValue("@minimo", pedidoMinimo);
                cmd.Parameters.AddWithValue("@limite", limiteUsos);
                cmd.Parameters.AddWithValue("@valido", validoAte.Date);
                cmd.Parameters.AddWithValue("@sit", situacao);
                cmd.Parameters.AddWithValue("@cod", codigo);
                cmd.ExecuteNonQuery();
            }
            return "";
        }
        catch (Exception ex) { return ex.Message; }
    }
}
}
