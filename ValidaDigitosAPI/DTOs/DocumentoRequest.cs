using ValidaDigitosAPI.Enums;

namespace ValidaDigitosAPI.DTOs
{
    public class DocumentoRequest
    {
        public eTipo Tipo { get; set; }
        public string Digitos { get; set; }
    }
}
