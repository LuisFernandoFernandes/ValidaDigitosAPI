using System.Text.RegularExpressions;
using ValidaDigitosAPI.DTOs;
using ValidaDigitosAPI.Enums;
using ValidaDigitosAPI.Models;

namespace ValidaDigitosAPI.Services
{
    public class DocumentosService : IDocumentosService
    {
        public Documentos Validar(DocumentoRequest request)
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

        private Documentos ValidarDocumento(DocumentoRequest request)
        {
            const int tamanhoCpf = 11;
            const int tamanhoCnpj = 14;

            var digitosNormalizados = NormalizarDigitos(request.Digitos);
            ValidarTamanhoDigitos(digitosNormalizados.Length, request.Tipo == eTipo.CPF ? tamanhoCpf : tamanhoCnpj);

            var numeros = ConverterDigitosEmNumeros(digitosNormalizados);
            var (somaDigito1, somaDigito2) = CalcularSomaDigitos(numeros, request.Tipo);
            var (digitoVerificador1, digitoVerificador2) = CalcularDigitoVerificador(somaDigito1, somaDigito2);

            var valido = VerificarValidade(digitosNormalizados, digitoVerificador1, digitoVerificador2);

            return new Documentos { Tipo = request.Tipo, Digitos = request.Digitos, Valido = valido };
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

        private (int, int) CalcularSomaDigitos(int[] numeros, eTipo tipo)
        {
            int digito1Tamanho = tipo == eTipo.CPF ? 9 : 12;
            int digito2Tamanho = tipo == eTipo.CPF ? 10 : 13;
            var digito1Multiplicador = tipo == eTipo.CPF ? 1 : 6;
            var digito2Multiplicador = tipo == eTipo.CPF ? 0 : 5;

            int digito1 = numeros.Take(digito1Tamanho).Select((n, i) => (n * (digito1Multiplicador + i > 9 ? digito1Multiplicador + i - 8 : digito1Multiplicador + i))).Sum();
            int digito2 = numeros.Take(digito2Tamanho).Select((n, i) => (n * (digito2Multiplicador + i > 9 ? digito2Multiplicador + i - 8 : digito2Multiplicador + i))).Sum();

            return (digito1, digito2);
        }

        private (int, int) CalcularDigitoVerificador(int soma1, int soma2)
        {
            var dvModulo11 = 11;
            var digitoVerificador1 = soma1 % dvModulo11;
            var digitoVerificador2 = soma2 % dvModulo11;
            return (digitoVerificador1 == 10 ? 0 : digitoVerificador1, digitoVerificador2 == 10 ? 0 : digitoVerificador2);
        }

        private bool VerificarValidade(string digitos, int digitoVerificador1, int digitoVerificador2)
        {
            var diferencaNumerosASCII = 48;
            return digitoVerificador1 == Convert.ToInt32(digitos[digitos.Length - 2]) - diferencaNumerosASCII &&
                   digitoVerificador2 == Convert.ToInt32(digitos[digitos.Length - 1]) - diferencaNumerosASCII;
        }
    }
}
