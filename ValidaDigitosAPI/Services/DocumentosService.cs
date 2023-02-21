using System.Text.RegularExpressions;
using ValidaDigitosAPI.DTOs;
using ValidaDigitosAPI.Enums;

namespace ValidaDigitosAPI.Services
{
    public class DocumentosService : IDocumentosService
    {

        #region Validar
        public bool Validar(DocumentoRequest request)
        {
            if (string.IsNullOrEmpty(request.Digitos))
                throw new ArgumentException("O número do documento não foi informado");

            switch (request.Tipo)
            {
                case eTipo.CPF:
                case eTipo.CNPJ:
                    return ValidarDocumento(request);

                default:
                    throw new ArgumentException("Tipo de documento inválido");
            }
        }

        private bool ValidarDocumento(DocumentoRequest request)
        {
            const int tamanhoCpf = 11;
            const int tamanhoCnpj = 14;

            var digitosNormalizados = NormalizarDigitos(request.Digitos);
            ValidarTamanhoDigitos(digitosNormalizados.Length, request.Tipo == eTipo.CPF ? tamanhoCpf : tamanhoCnpj);

            var numeros = ConverterDigitosEmNumeros(digitosNormalizados);
            var (digitoVerificador1, digitoVerificador2) = CalcularDigitosVerificadores(numeros, request.Tipo);

            return VerificarValidade(digitosNormalizados, digitoVerificador1, digitoVerificador2);
        }

        private string NormalizarDigitos(string digitos)
        {
            return Regex.Replace(digitos, @"[^\w\d]+", "");
        }

        private void ValidarTamanhoDigitos(int tamanhoAtual, int tamanhoEsperado)
        {
            if (tamanhoAtual != tamanhoEsperado)
                throw new ArgumentException("Número de dígitos inválido");
        }

        private int[] ConverterDigitosEmNumeros(string digitos)
        {
            return digitos.Select(d => int.Parse(d.ToString())).ToArray();
        }

        private (int, int) CalcularDigitosVerificadores(int[] numeros, eTipo tipo)
        {
            int digitoTamanho = tipo == eTipo.CPF ? 9 : 12;
            var digito1Multiplicador = tipo == eTipo.CPF ? 1 : 6;
            var digito2Multiplicador = tipo == eTipo.CPF ? 0 : 5;

            int somaDigito1 = numeros.Take(digitoTamanho).Select((n, i) => (n * (digito1Multiplicador + i > 9 ? digito1Multiplicador + i - 8 : digito1Multiplicador + i))).Sum();
            int digito1 = ObtemDigitoVerificador(somaDigito1);

            int somaDigito2 = numeros.Take(digitoTamanho++).Select((n, i) => (n * (digito2Multiplicador + i > 9 ? digito2Multiplicador + i - 8 : digito2Multiplicador + i))).Sum() + (digito1 * 9);
            int digito2 = ObtemDigitoVerificador(somaDigito2);

            return (digito1, digito2);
        }

        private int ObtemDigitoVerificador(int soma)
        {
            var dvModulo11 = 11;
            var digito = soma % dvModulo11;
            return digito == 10 ? 0 : digito;
        }

        private bool VerificarValidade(string digitos, int digitoVerificador1, int digitoVerificador2)
        {
            var diferencaNumerosASCII = 48;
            return digitoVerificador1 == Convert.ToInt32(digitos[digitos.Length - 2]) - diferencaNumerosASCII &&
                   digitoVerificador2 == Convert.ToInt32(digitos[digitos.Length - 1]) - diferencaNumerosASCII;
        }
        #endregion

        #region Gerar
        public string Gerar(GeracaoRequest request)
        {
            switch (request.Tipo)
            {
                case eTipo.CPF:
                case eTipo.CNPJ:
                    return GerarDocumento(request);

                default:
                    throw new ArgumentException("Tipo de documento inválido");
            }
        }

        private string GerarDocumento(GeracaoRequest request)
        {
            var quantidadeDigitosAleatorios = ObterQuantidadeDigitosAleatorios(request.Tipo);
            int[] digitos = GerarDigitosAleatorios(quantidadeDigitosAleatorios);
            digitos = request.Tipo == eTipo.CPF && request.Estado != null ? ObtemDigitoRegiaoFiscal(digitos, request.Estado) : digitos;
            return FormatarRetorno(digitos, request);
        }

        private int ObterQuantidadeDigitosAleatorios(eTipo tipo)
        {
            const int tamanhoCpf = 11;
            const int tamanhoCnpj = 14;
            int quantidadeDigitosAleatorios = tipo == eTipo.CPF ? tamanhoCpf - 2 : tamanhoCnpj - 2;
            return quantidadeDigitosAleatorios;
        }

        private int[] GerarDigitosAleatorios(int quantidadeDigitos)
        {
            var random = new Random();
            int[] digitos = new int[quantidadeDigitos];
            for (int i = 0; i < quantidadeDigitos; i++)
            {
                digitos[i] = random.Next(0, 10);
            }
            return digitos;
        }

        private int[] ObtemDigitoRegiaoFiscal(int[] digitos, eEstado? estado)
        {
            var arraySize = digitos.Length;

            digitos[arraySize - 1] = ObtemRegiaoFiscal(estado);

            return digitos;
        }

        private int ObtemRegiaoFiscal(eEstado? estado)
        {
            switch (estado)
            {
                case eEstado.RioGrandeDoSul:
                    return 0;

                case eEstado.DistritoFederal:
                case eEstado.Goiás:
                case eEstado.MatoGrossoDoSul:
                case eEstado.MatoGrosso:
                case eEstado.Tocantins:
                    return 1;

                case eEstado.Acre:
                case eEstado.Amazonas:
                case eEstado.Amapá:
                case eEstado.Pará:
                case eEstado.Rondônia:
                case eEstado.Roraima:
                    return 2;

                case eEstado.Ceará:
                case eEstado.Maranhão:
                case eEstado.Piauí:
                    return 3;

                case eEstado.Alagoas:
                case eEstado.Paraíba:
                case eEstado.Pernambuco:
                case eEstado.RioGrandeDoNorte:
                    return 4;

                case eEstado.Bahia:
                case eEstado.Sergipe:
                    return 5;

                case eEstado.MinasGerais:
                    return 6;

                case eEstado.EspíritoSanto:
                case eEstado.RioDeJaneiro:
                    return 7;

                case eEstado.SãoPaulo:
                    return 8;

                case eEstado.Paraná:
                case eEstado.SantaCatarina:
                    return 9;

                default:
                    throw new ArgumentException("Estado inválido");
            }
        }


        private string FormatarRetorno(int[] digitos, GeracaoRequest request)
        {
            var (digitoVerificador1, digitoVerificador2) = CalcularDigitosVerificadores(digitos, request.Tipo);
            string documento = string.Join("", digitos) + digitoVerificador1.ToString() + digitoVerificador2.ToString();
            return request.Formatado ? FormataDocumento(documento, request.Tipo) : documento;
        }

        private string FormataDocumento(string documento, eTipo tipo)
        {
            switch (tipo)
            {
                case eTipo.CPF:
                    return FormataCPF(documento);
                case eTipo.CNPJ:
                    return FormataCNPJ(documento);
                default:
                    throw new ArgumentException("Tipo de documento inválido");
            }
        }

        private string FormataCPF(string documento)
        {
            if (string.IsNullOrEmpty(documento) || documento.Length != 11)
                return documento;

            return documento.Insert(3, ".").Insert(7, ".").Insert(11, "-");
        }

        private string FormataCNPJ(string documento)
        {
            if (string.IsNullOrEmpty(documento) || documento.Length != 14)
                return documento;

            return documento.Insert(2, ".").Insert(6, ".").Insert(10, "/").Insert(15, "-");
        }
        #endregion
    }
}
