using Omnimarket.Api.Models.Entidades;
using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Utils
{
    public static class EntregaHelper
    {
        public static bool TipoEntregaValido(int tipoEntregaId)
        {
            return Enum.IsDefined(typeof(TipoEntrega), tipoEntregaId);
        }

        public static bool TipoEntregaEhRetirada(int tipoEntregaId)
        {
            return tipoEntregaId == (int)TipoEntrega.Retirada;
        }

        public static string ObterNomeTipoEntrega(int tipoEntregaId)
        {
            if (!TipoEntregaValido(tipoEntregaId))
                return "Tipo de entrega desconhecido";

            return EnumExtensions.GetDisplayName((TipoEntrega)tipoEntregaId);
        }

        public static string? NormalizarCep(string? cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
                return null;

            var somenteDigitos = new string(cep.Where(char.IsDigit).ToArray());

            if (somenteDigitos.Length != 8)
                throw new InvalidOperationException("CEP deve conter 8 digitos.");

            return somenteDigitos;
        }

        public static bool AtendeEndereco(LojaEntregaOpcao opcao, string? cep, string? cidade, string? uf)
        {
            if (!string.IsNullOrWhiteSpace(cep))
                _ = NormalizarCep(cep);

            return true;
        }

        public static string MontarResumoCobertura(LojaEntregaOpcao opcao)
        {
            if (TipoEntregaEhRetirada(opcao.TipoEntregaId))
                return "Retirada presencial na loja.";

            if (!string.IsNullOrWhiteSpace(opcao.Observacao))
                return opcao.Observacao.Trim();

            return "Entrega conforme opcao cadastrada pela loja.";
        }

        public static string MontarCondicoesAceitas(LojaEntregaOpcao opcao)
        {
            return $"{ObterNomeTipoEntrega(opcao.TipoEntregaId)} | frete {opcao.ValorFrete:0.00} | prazo {opcao.PrazoEntregaDias} dia(s) | {MontarResumoCobertura(opcao)}";
        }
    }
}
