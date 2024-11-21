namespace DirecionadoresMVC.Aplicacao.DTOs
{
    public class ValidationDto
    {
        public string MensagemErro { get; set; }

        public bool Sucesso { get; set; }

        public ValidationDto(string mensagemErro)
        {
            MensagemErro = mensagemErro;
            Sucesso = string.IsNullOrEmpty(mensagemErro);
        }
    }
}