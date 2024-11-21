namespace DirecionadoresMVC.Aplicacao.DTOs
{
    public class ElementosFiltragemResultDto : ResultDto
    {
        public List<string> Clientes { get; set; }

        public List<string> Situacoes { get; set; }

        public List<string> Bairros { get; set; }

        public ElementosFiltragemResultDto()
        {
            Clientes = new List<string>();
            Situacoes = new List<string>();
            Bairros = new List<string>();
        }
    }
}