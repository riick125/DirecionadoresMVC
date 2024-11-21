using DirecionadoresMVC.Aplicacao.DTOs;

namespace DirecionadoresMVC.Aplicacao.Validations
{
    public class PlacemarkValidation
    {
        public ValidationDto ValidarCliente(FiltroPlacemarkDto filtroDto, List<string> clientes)
        {
            var mensagemErro = string.Empty;

            if (!string.IsNullOrEmpty(filtroDto.Cliente) && !clientes.Any(x => x.Equals(filtroDto.Cliente, StringComparison.InvariantCultureIgnoreCase)))
            {
                mensagemErro = "O campo de pré-seleção CLIENTE deve conter apenas itens previamente lidos e disponibilizados.";
            }

            return new ValidationDto(mensagemErro);
        }

        public ValidationDto ValidarSituacao(FiltroPlacemarkDto filtroDto, List<string> situacoes)
        {
            var mensagemErro = string.Empty;

            if (!string.IsNullOrEmpty(filtroDto.Situacao) && !situacoes.Any(x => x.Equals(filtroDto.Situacao, StringComparison.InvariantCultureIgnoreCase)))
            {
                mensagemErro = "O campo de pré-seleção SITUAÇÃO deve conter apenas itens previamente lidos e disponibilizados.";
            }

            return new ValidationDto(mensagemErro);
        }

        public ValidationDto ValidarBairro(FiltroPlacemarkDto filtroDto, List<string> bairros)
        {
            var mensagemErro = string.Empty;

            if (!string.IsNullOrEmpty(filtroDto.Bairro) && !bairros.Any(x => x.Equals(filtroDto.Bairro, StringComparison.InvariantCultureIgnoreCase)))
            {
                mensagemErro = "O campo de pré-seleção BAIRRO deve conter apenas itens previamente lidos e disponibilizados.";
            }

            return new ValidationDto(mensagemErro);
        }

        public ValidationDto ValidarReferencia(FiltroPlacemarkDto filtroDto)
        {
            var mensagemErro = string.Empty;

            if (!string.IsNullOrEmpty(filtroDto.Referencia) && filtroDto.Referencia.Length < 3)
            {
                mensagemErro = "O filtro de texto REFERENCIA deve ter pelo menos 3 caracteres.";
            }

            return new ValidationDto(mensagemErro);
        }

        public ValidationDto ValidarRuaCruzamento(FiltroPlacemarkDto filtroDto)
        {
            var mensagemErro = string.Empty;

            if (!string.IsNullOrEmpty(filtroDto.RuaCruzamento) && filtroDto.RuaCruzamento.Length < 3)
            {
                mensagemErro = "O filtro de texto RUA/CRUZAMENTO deve ter pelo menos 3 caracteres.";
            }

            return new ValidationDto(mensagemErro);
        }
    }
}