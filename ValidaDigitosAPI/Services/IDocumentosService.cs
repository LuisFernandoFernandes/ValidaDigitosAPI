using ValidaDigitosAPI.DTOs;

namespace ValidaDigitosAPI.Services
{
    public interface IDocumentosService
    {
        bool Validar(DocumentoRequest request);
        string Gerar(GeracaoRequest request);
    }
}
