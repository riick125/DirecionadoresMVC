using DirecionadoresMVC.Aplicacao.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DirecionadoresMVC.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlacemarksController : ControllerBase
    {
        private readonly IPlacemarkServiceApp _placemarkService;

        public PlacemarksController(IPlacemarkServiceApp placemarkService)
        {
            _placemarkService = placemarkService;
        }

        [HttpPost("export")]
        [EndpointName("api/placemarks/export")]
        public IActionResult FiltrarExportarKML([FromQuery] string cliente = null, [FromQuery] string situacao = null, [FromQuery] string bairro = null, [FromQuery] string referencia = null, [FromQuery] string ruaCruzamento = null)
        {
            var result = _placemarkService.FiltrarExportarKml(cliente, situacao, bairro, referencia, ruaCruzamento);

            if (result.Validacao != null && !result.Validacao.Sucesso)
            {
                return BadRequest(result.Validacao.MensagemErro);
            }

            return Ok(result);
        }

        [HttpGet]
        public IActionResult ListarJson([FromQuery] string cliente = null, [FromQuery] string situacao = null, [FromQuery] string bairro = null, [FromQuery] string referencia = null, [FromQuery] string ruaCruzamento = null)
        {
            var result = _placemarkService.ListarJson(cliente, situacao, bairro, referencia, ruaCruzamento);

            if (result.Validacao != null && !result.Validacao.Sucesso)
            {
                return BadRequest(result.Validacao.MensagemErro);
            }

            return Ok(result);
        }

        [HttpGet("filters")]
        public IActionResult ObterElementosDisponiveisParaFiltragem()
        {
            var result = _placemarkService.ObterElementosDisponiveisParaFiltragem();

            if (result.Validacao != null && !result.Validacao.Sucesso)
            {
                return BadRequest(result.Validacao.MensagemErro);
            }

            return Ok(result);
        }
    }
}