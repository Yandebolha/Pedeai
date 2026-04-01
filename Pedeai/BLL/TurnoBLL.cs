using System;
using System.Data;
using Pedeai.DAL;
using Pedeai.Modelo;

namespace Pedeai.BLL
{
    public class TurnoBLL
    {
        private readonly TurnoDAL _dal = new TurnoDAL();

        /// <summary>Abre um novo turno. Retorna mensagem de erro ou string vazia.</summary>
        public string Abrir(decimal caixaInicial, string observacao)
        {
            try
            {
                var ativo = _dal.GetAtivo();
                if (ativo != null)
                    return $"Já existe um turno aberto (#{ativo.Codigo}) desde {ativo.turAbertura:dd/MM/yyyy HH:mm}.";

                var t = new Turno
                {
                    turAbertura    = DateTime.Now,
                    turUsuario     = UsuarioSessao.NomeAtual,
                    turCaixa_Inicial = caixaInicial,
                    turObservacao  = observacao ?? ""
                };
                _dal.Abrir(t);
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        /// <summary>Fecha o turno ativo. Retorna mensagem de erro ou string vazia.</summary>
        public string Fechar(decimal caixaFinal, string observacao)
        {
            try
            {
                var ativo = _dal.GetAtivo();
                if (ativo == null)
                    return "Nenhum turno aberto encontrado.";

                _dal.Fechar(ativo.Codigo, caixaFinal, observacao);
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public Turno GetAtivo() => _dal.GetAtivo();

        /// <summary>Retorna o total movimentado (vendas) durante o período do turno.</summary>
        public decimal GetTotalMovimentado(Turno t)
        {
            try
            {
                var fim = t.turFechamento ?? DateTime.Now;
                var dt = _dal.GetPedidosTurno(t.turAbertura, fim);
                decimal total = 0;
                foreach (System.Data.DataRow r in dt.Rows)
                    if (r["pediValor_Total"] != System.DBNull.Value)
                        total += Convert.ToDecimal(r["pediValor_Total"]);
                return total;
            }
            catch { return 0; }
        }

        public DataTable Listar(DateTime de, DateTime ate) => _dal.Listar(de, ate);

        public DataTable GetPedidosTurno(DateTime abertura, DateTime fechamento)
            => _dal.GetPedidosTurno(abertura, fechamento);
    }
}
