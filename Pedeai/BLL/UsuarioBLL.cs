using System;
using System.Collections.Generic;
using Pedeai.DAL;
using Pedeai.Modelo;

namespace Pedeai.BLL
{
    public class UsuarioBLL
    {
        private readonly UsuarioDAL _dal = new UsuarioDAL();

        public Usuario Autenticar(string login, string senha)
            => _dal.Autenticar(login, senha);

        public string BuscarNomePorLogin(string login)
            => _dal.BuscarNomePorLoginOuNome(login);

        public List<Usuario> Listar()
            => _dal.Listar();

        public Usuario PesquisaCodigo(int codigo)
            => _dal.PesquisaCodigo(codigo);

        public string Salvar(Usuario obj, bool alterarSenha)
        {
            if (string.IsNullOrWhiteSpace(obj.usuNome))   return "Informe o nome.";
            if (string.IsNullOrWhiteSpace(obj.usuLogin))  return "Informe o login.";
            if (obj.Codigo == 0)
            {
                if (string.IsNullOrWhiteSpace(obj.usuSenha)) return "Informe a senha.";
                return _dal.Inserir(obj);
            }
            return _dal.Alterar(obj, alterarSenha);
        }
    }
}
