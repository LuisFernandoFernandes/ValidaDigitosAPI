using ValidaDigitosAPI.DTOs;
using ValidaDigitosAPI.Models;

namespace ValidaDigitosAPI.Services
{
    public interface IDocumentosService
    {
        Documentos Validar(DocumentoRequest request);
    }
}
