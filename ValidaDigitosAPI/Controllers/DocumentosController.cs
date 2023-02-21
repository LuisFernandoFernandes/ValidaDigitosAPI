using Microsoft.AspNetCore.Mvc;
using ValidaDigitosAPI.DTOs;
using ValidaDigitosAPI.Services;

namespace ValidaDigitosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentosController : ControllerBase
    {
        private readonly IDocumentosService _service;

        public DocumentosController()
        {
            _service = new DocumentosService();
        }

        [HttpGet("validar")]
        public ActionResult<bool> Validar([FromQuery] DocumentoRequest request)
        {
            try
            {
                return Ok(_service.Validar(request));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return Problem("Algo deu errado, contate o administrador.");
            }
        }

        [HttpGet("gerar")]
        public ActionResult<string> Gerar([FromQuery] GeracaoRequest request)
        {
            try
            {
                return Ok(_service.Gerar(request));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return Problem("Algo deu errado, contate o administrador.");
            }
        }

    }
}
