using ValidaDigitosAPI.Enums;

namespace ValidaDigitosAPI.DTOs
{
    public class GeracaoRequest
    {
        public eTipo Tipo { get; set; }
        public bool Formatado { get; set; } = false;
        public eEstado? Estado { get; set; }
    }
}
