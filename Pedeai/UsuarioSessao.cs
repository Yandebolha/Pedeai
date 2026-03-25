using Pedeai.Modelo;

namespace Pedeai
{
    /// <summary>
    /// Gerencia o usuario logado durante a sessao do sistema.
    /// </summary>
    public static class UsuarioSessao
    {
        public static Usuario UsuarioAtual { get; private set; }

        public static bool Logado => UsuarioAtual != null;

        /// <summary>Inicia a sessao com o usuario autenticado.</summary>
        public static void Iniciar(Usuario usuario) => UsuarioAtual = usuario;

        /// <summary>Encerra a sessao atual.</summary>
        public static void Encerrar() => UsuarioAtual = null;

        /// <summary>Retorna true se o usuario logado tem nivel >= nivelMinimo.</summary>
        public static bool TemNivel(int nivelMinimo)
            => Logado && UsuarioAtual.usuNivel >= nivelMinimo;

        /// <summary>Nome de exibicao do usuario logado (ou "Sistema" se nao autenticado).</summary>
        public static string NomeAtual
            => Logado ? UsuarioAtual.usuNome : "Sistema";
    }
}
