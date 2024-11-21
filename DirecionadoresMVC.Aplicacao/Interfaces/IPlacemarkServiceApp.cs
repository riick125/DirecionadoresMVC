using DirecionadoresMVC.Aplicacao.DTOs;
using System.Xml;

namespace DirecionadoresMVC.Aplicacao.Interfaces
{
    public interface IPlacemarkServiceApp
    {
        ExportacaoPlacemarkResultDto FiltrarExportarKml(string cliente, string situacao, string bairro, string referencia, string ruaCruzamento);

        ExportacaoPlacemarkResultDto ListarJson(string cliente, string situacao, string bairro, string referencia, string ruaCruzamento);

        ElementosFiltragemResultDto ObterElementosDisponiveisParaFiltragem();
    }
}