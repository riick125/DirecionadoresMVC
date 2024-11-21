using DirecionadoresMVC.Aplicacao.DTOs;
using DirecionadoresMVC.Aplicacao.Interfaces;
using DirecionadoresMVC.Aplicacao.Validations;
using System.Xml;

namespace DirecionadoresMVC.Aplicacao.Services
{
    public class PlacemarkServiceApp : IPlacemarkServiceApp
    {
        private XmlDocument _documento;

        private List<string> _clientes, _situacoes, _bairros;

        private PlacemarkValidation _validation;

        public PlacemarkServiceApp()
        {
            _validation = new PlacemarkValidation();
        }

        public ExportacaoPlacemarkResultDto FiltrarExportarKml(string cliente, string situacao, string bairro, string referencia, string ruaCruzamento)
        {
            var filtroDto = CriarFiltro(cliente, situacao, bairro, referencia, ruaCruzamento);

            var exportacaoDto = BuscarListaPlacemarks(filtroDto);

            if (exportacaoDto.Validacao != null && exportacaoDto.Validacao.Sucesso)
            {
                if (exportacaoDto.Documento != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        exportacaoDto.Documento.Save(memoryStream);

                        exportacaoDto.Arquivo = memoryStream.ToArray();

                        exportacaoDto.ListaPlacemarksDto = null;
                    }
                }
            }

            return exportacaoDto;
        }

        public ExportacaoPlacemarkResultDto ListarJson(string cliente, string situacao, string bairro, string referencia, string ruaCruzamento)
        {
            var filtroDto = CriarFiltro(cliente, situacao, bairro, referencia, ruaCruzamento);

            return BuscarListaPlacemarks(filtroDto);
        }

        public ElementosFiltragemResultDto ObterElementosDisponiveisParaFiltragem()
        {
            var resultadoDocumentoDto = CarregarDocumento();

            var resultadoElementosDto = new ElementosFiltragemResultDto() { Validacao = resultadoDocumentoDto.Validacao };

            if (resultadoElementosDto.Validacao != null && resultadoElementosDto.Validacao.Sucesso)
            {
                resultadoElementosDto.Clientes.AddRange(_clientes);
                resultadoElementosDto.Situacoes.AddRange(_situacoes);
                resultadoElementosDto.Bairros.AddRange(_bairros);
            }

            return resultadoElementosDto;
        }

        private FiltroPlacemarkDto CriarFiltro(string cliente, string situacao, string bairro, string referencia, string ruaCruzamento)
        {
            return new FiltroPlacemarkDto()
            {
                Cliente = cliente,
                Situacao = situacao,
                Bairro = bairro,
                Referencia = referencia,
                RuaCruzamento = ruaCruzamento,
            };
        }

        private ExportacaoPlacemarkResultDto CarregarDocumento()
        {
            var resultadoDto = new ExportacaoPlacemarkResultDto() { Validacao = new ValidationDto("")};

            if (_documento == null)
            {
                _documento = new XmlDocument();

                _clientes = new List<string>();
                _situacoes = new List<string>();
                _bairros = new List<string>();

                var caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "Data", "DIRECIONADORES1.kml");

                if (!File.Exists(caminhoArquivo))
                {
                    resultadoDto.Validacao = new ValidationDto("Arquivo não encontrado");
                }
                else
                {
                    _documento.Load(caminhoArquivo);

                    var placemarks = _documento.GetElementsByTagName("Placemark");

                    if (placemarks == null)
                    {
                        resultadoDto.Validacao = new ValidationDto("Arquivo não encontrado");
                    }
                    else
                    {
                        resultadoDto.PlacemarksNodeList = placemarks;

                        foreach (XmlNode placemark in placemarks)
                        {
                            var valorCliente = ObterValorNodePorCampo(placemark, "CLIENTE");

                            if (!string.IsNullOrEmpty(valorCliente))
                            {
                                _clientes.Add(valorCliente);
                            }

                            var valorSituacao = ObterValorNodePorCampo(placemark, "SITUAÇÃO");

                            if (!string.IsNullOrEmpty(valorSituacao))
                            {
                                _situacoes.Add(valorSituacao);
                            }

                            var valorBairro = ObterValorNodePorCampo(placemark, "BAIRRO");

                            if (!string.IsNullOrEmpty(valorBairro))
                            {
                                _bairros.Add(valorBairro);
                            }
                        }

                        _clientes = _clientes.Distinct().ToList();
                        _situacoes = _situacoes.Distinct().ToList();
                        _bairros = _bairros.Distinct().ToList();
                    }
                }
            }

            return resultadoDto;
        }

        private ExportacaoPlacemarkResultDto BuscarListaPlacemarks(FiltroPlacemarkDto filtroDto)
        {
            var resultadoDto = CarregarDocumento();

            if (resultadoDto.Validacao != null && !resultadoDto.Validacao.Sucesso)
            {
                return resultadoDto;
            }

            var validacaoFiltros = ValidarFiltros(filtroDto);

            if (!validacaoFiltros.Sucesso)
            {
                resultadoDto.Validacao = validacaoFiltros;
            }
            else
            {
                var kmlElement = _documento.DocumentElement;
                if (kmlElement == null || kmlElement.Name != "kml")
                {
                    resultadoDto.Validacao = new ValidationDto("Arquivo KML inválido.");
                }
                else
                {
                    var novoDoc = new XmlDocument();
                    var novoKml = novoDoc.CreateElement("kml");
                    novoKml.SetAttribute("xmlns", "http://www.opengis.net/kml/2.2");
                    novoDoc.AppendChild(novoKml);

                    var novoDocument = novoDoc.CreateElement("Document");
                    novoKml.AppendChild(novoDocument);

                    var estilos = _documento.GetElementsByTagName("Style");
                    foreach (XmlNode estilo in estilos)
                    {
                        var importado = novoDoc.ImportNode(estilo, true);
                        novoDocument.AppendChild(importado);
                    }

                    foreach (XmlNode placemark in resultadoDto.PlacemarksNodeList)
                    {
                        if (FiltrarPlacemark(placemark, filtroDto))
                        {
                            var dto = new PlacemarkDto()
                            {
                                Cliente = ObterValorNodePorCampo(placemark, "CLIENTE"),

                                Situacao = ObterValorNodePorCampo(placemark, "SITUAÇÃO"),

                                Bairro = ObterValorNodePorCampo(placemark, "BAIRRO"),

                                Referencia = ObterValorNodePorCampo(placemark, "REFERENCIA"),

                                RuaCruzamento = ObterValorNodePorCampo(placemark, "RUA/CRUZAMENTO")
                            };

                            resultadoDto.ListaPlacemarksDto.Add(dto);

                            var importado = novoDoc.ImportNode(placemark, true);
                            novoDocument.AppendChild(importado);
                        }
                    }

                    resultadoDto.Documento = novoDoc;

                    resultadoDto.Validacao = new ValidationDto("");
                }
            }

            return resultadoDto;
        }

        private string ObterValorNodePorCampo(XmlNode placemark, string nomeCampo)
        {
            var extendedData = placemark["ExtendedData"];

            if (extendedData == null)
            {
                return string.Empty;
            }

            var childNodes = extendedData.ChildNodes;

            if (childNodes == null)
            {
                return string.Empty;
            }

            return ObterValorNode(childNodes, nomeCampo);
        }

        private bool FiltrarPlacemark(XmlNode placemark, FiltroPlacemarkDto filtroDto)
        {
            var extendedData = placemark["ExtendedData"];

            if (extendedData == null)
            {
                return false;
            }

            var childNodes = extendedData.ChildNodes;

            if (childNodes == null)
            {
                return false;
            }

            var cliente = ObterValorNode(childNodes, "CLIENTE");
            var situacao = ObterValorNode(childNodes, "SITUAÇÃO");
            var bairro = ObterValorNode(childNodes, "BAIRRO");
            var referencia = ObterValorNode(childNodes, "REFERENCIA");
            var ruaCruzamento = ObterValorNode(childNodes, "RUA/CRUZAMENTO");

            if (!string.IsNullOrEmpty(filtroDto.Cliente) && !cliente.Equals(filtroDto.Cliente, StringComparison.InvariantCultureIgnoreCase)) return false;
            if (!string.IsNullOrEmpty(filtroDto.Situacao) && !situacao.Equals(filtroDto.Situacao, StringComparison.InvariantCultureIgnoreCase)) return false;
            if (!string.IsNullOrEmpty(filtroDto.Bairro) && !bairro.Equals(filtroDto.Bairro, StringComparison.InvariantCultureIgnoreCase)) return false;
            if (!string.IsNullOrEmpty(filtroDto.Referencia) && !referencia.Contains(filtroDto.Referencia, StringComparison.InvariantCultureIgnoreCase)) return false;
            if (!string.IsNullOrEmpty(filtroDto.RuaCruzamento) && !ruaCruzamento.Contains(filtroDto.RuaCruzamento, StringComparison.InvariantCultureIgnoreCase)) return false;

            return true;
        }

        private ValidationDto ValidarFiltros(FiltroPlacemarkDto filtroDto)
        {
            var validacaoCliente = _validation.ValidarCliente(filtroDto, _clientes);

            if (!validacaoCliente.Sucesso)
            {
                return validacaoCliente;
            }

            var validacaoBairro = _validation.ValidarBairro(filtroDto, _bairros);

            if (!validacaoBairro.Sucesso)
            {
                return validacaoBairro;
            }

            var validacaoSituacao = _validation.ValidarSituacao(filtroDto, _situacoes);

            if (!validacaoSituacao.Sucesso)
            {
                return validacaoSituacao;
            }

            var validacaoReferencia = _validation.ValidarReferencia(filtroDto);

            if (!validacaoReferencia.Sucesso)
            {
                return validacaoReferencia;
            }

            var validacaoRuaCruzamento = _validation.ValidarRuaCruzamento(filtroDto);

            if (!validacaoRuaCruzamento.Sucesso)
            {
                return validacaoRuaCruzamento;
            }

            return new ValidationDto(null);
        }

        private string ObterValorNode(XmlNodeList list, string name)
        {
            var resultado = string.Empty;

            if (list == null)
            {
                return resultado;
            }

            var encontrou = false;
            var indice = 0;

            while (indice < list.Count && !encontrou)
            {
                var node = list[indice];

                if (node != null)
                {
                    var attribute = node.Attributes[0];

                    if (attribute != null && attribute.InnerText.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        resultado = node.InnerText;

                        encontrou = true;
                    }
                }

                indice++;
            }

            return resultado;
        }
    }
}