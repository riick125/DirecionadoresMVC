using System.Text.Json.Serialization;
using System.Xml;

namespace DirecionadoresMVC.Aplicacao.DTOs
{
    public class ExportacaoPlacemarkResultDto : ResultDto
    {
        public byte[]? Arquivo { get; set; }

        public List<PlacemarkDto> ListaPlacemarksDto { get; set; }

        [JsonIgnore]
        public XmlDocument? Documento { get; set; }

        [JsonIgnore]
        public XmlNodeList? PlacemarksNodeList { get; set; }

        public ExportacaoPlacemarkResultDto()
        {
            ListaPlacemarksDto = new List<PlacemarkDto>();
        }
    }
}