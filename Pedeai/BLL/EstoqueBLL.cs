using System;
using System.Data;
using Pedeai.DAL;
using Pedeai.Modelo;

namespace Pedeai.BLL
{
    public class EstoqueBLL
    {
        private readonly EstoqueItemDAL     _dal  = new EstoqueItemDAL();
        private readonly MercadoriaDAL      _mDAL = new MercadoriaDAL();
        private readonly GrupoMercadoriaDAL _gDAL = new GrupoMercadoriaDAL();

        public DataTable Listar(string filtro = "") => _dal.Listar(filtro);

        public EstoqueItem PesquisaCodigo(int codigo) => _dal.PesquisaCodigo(codigo);

        /// <summary>Salva item. Se estoEh_Produto=true, sincroniza com mercadoria.</summary>
        public string Salvar(EstoqueItem obj)
        {
            if (string.IsNullOrWhiteSpace(obj.estoNome))
                return "Informe o nome do item.";

            string erro = obj.Codigo == 0 ? _dal.Incluir(obj) : _dal.Alterar(obj);
            if (!string.IsNullOrEmpty(erro)) return erro;

            if (obj.estoEh_Produto)
                erro = SincronizarProduto(obj);

            return erro;
        }

        private string SincronizarProduto(EstoqueItem obj)
        {
            int codGrupo = 1;
            if (obj.Codigo_Grupo.HasValue && obj.Codigo_Grupo.Value > 0)
            {
                codGrupo = obj.Codigo_Grupo.Value;
            }
            else
            {
                DataTable cats = _gDAL.Listar();
                if (cats.Rows.Count > 0)
                    codGrupo = Convert.ToInt32(cats.Rows[0]["Codigo"]);
            }

            if (obj.Codigo_Mercadoria.HasValue && obj.Codigo_Mercadoria.Value > 0)
            {
                var merc = _mDAL.PesquisaCodigo(obj.Codigo_Mercadoria.Value);
                if (merc != null)
                {
                    merc.mercMercadoria      = obj.estoNome;
                    merc.mercPreco_Custo     = obj.estoPreco_Custo;
                    merc.mercPreco_Venda     = obj.estoPreco_Custo > 0 ? obj.estoPreco_Custo : merc.mercPreco_Venda;
                    merc.mercEstoque_Atual   = obj.estoQtde_Atual;
                    merc.mercControla_Estoque = true;
                    return _mDAL.Alterar(merc);
                }
            }

            var novo = new Mercadoria
            {
                mercMercadoria       = obj.estoNome,
                Codigo_Grupo         = codGrupo,
                mercPreco_Venda      = obj.estoPreco_Custo > 0 ? obj.estoPreco_Custo : 1,
                mercPreco_Custo      = obj.estoPreco_Custo,
                mercEstoque_Atual    = obj.estoQtde_Atual,
                mercControla_Estoque = true,
                Situacao             = "A",
                mercData_Cadastro    = DateTime.Now,
            };
            var err2 = _mDAL.Incluir(novo);
            if (!string.IsNullOrEmpty(err2)) return err2;

            obj.Codigo_Mercadoria = novo.Codigo;
            return _dal.Alterar(obj);
        }

        public string AjustarQuantidade(int codigo, decimal delta)
        {
            if (delta == 0) return "Informe uma quantidade maior que zero.";
            var erro = _dal.AjustarQuantidade(codigo, delta);
            if (!string.IsNullOrEmpty(erro)) return erro;

            var item = _dal.PesquisaCodigo(codigo);
            if (item?.estoEh_Produto == true && item.Codigo_Mercadoria.HasValue)
            {
                var merc = _mDAL.PesquisaCodigo(item.Codigo_Mercadoria.Value);
                if (merc != null)
                {
                    merc.mercEstoque_Atual += delta;
                    _mDAL.Alterar(merc);
                }
            }
            return "";
        }

        public void Desativar(int codigo) => _dal.Desativar(codigo);
    }
}

