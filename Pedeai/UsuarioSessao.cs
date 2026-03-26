using System;
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

        /// <summary>Retorna true se o usuario tem acesso ao modulo informado.
        /// O campo Info controla os modulos para todos os niveis.
        /// Se Info estiver vazio e o nivel for 9 (Admin), libera tudo por padrao.</summary>
        public static bool TemModulo(string modulo)
        {
            if (!Logado) return false;
            // Admin sem permissoes configuradas: acesso total por padrao
            if (UsuarioAtual.usuNivel >= 9 && string.IsNullOrWhiteSpace(UsuarioAtual.Info)) return true;
            if (string.IsNullOrWhiteSpace(UsuarioAtual.Info)) return false;
            var partes = UsuarioAtual.Info.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in partes)
                if (p.Trim().Equals(modulo, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        /// <summary>Nome de exibicao do usuario logado (ou "Sistema" se nao autenticado).</summary>
        public static string NomeAtual
            => Logado ? UsuarioAtual.usuNome : "Sistema";
    }
}
