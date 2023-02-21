using ValidaDigitosAPI.Enums;

namespace ValidaDigitosAPI.Models
{
    public class Documentos
    {
        public eTipo Tipo { get; set; }
        public string Digitos { get; set; }
        public bool Valido { get; set; }
    }
}
