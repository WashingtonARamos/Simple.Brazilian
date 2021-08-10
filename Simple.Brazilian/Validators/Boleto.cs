﻿using System;
using System.Globalization;

namespace Simple.Brazilian.Validators
{
    /// <summary>
    /// Classe para funções de validação de boletos bancários
    /// </summary>
    public static class Boleto
    {
        private static readonly DateTime DATA_BASE = new DateTime(1997, 10, 07);

        /// <summary>
        /// Executa o calculo do digito verificador
        /// do campo da linha digitavel 1, 2, ou 3 do boleto
        /// </summary>
        /// <returns>int DigitoVerificador</returns>
        public static int CalculateDV_Itau(string campoLinhaDigitavel)
            => ModValidation.Calculate2121Mod10IndividualDigits(campoLinhaDigitavel);

        /// <summary>
        /// Executa o calculo do DAC do codigo de barras
        /// a partir do formato:
        /// AAABXXXXXXXXXXXXXXCCCCCCCCCCCCDDDDEEEEEFGGG;
        /// A = Codigo do Banco; B = Codigo da Moeda; X = Campo 5;
        /// C = Nosso Numero; D = Agencia; E = Conta; F = DAC; G = Zeros;
        /// ou a linha digitavel do código de barras:
        /// AAABCCCCCZCCCCCCCDDDZDEEEEEFGGGZXXXXXXXXXXXXXX;
        /// Z = Digito Verificador;
        /// </summary>
        /// <param name="valor"></param>
        /// <returns>int DACcodigoBarras</returns>
        public static int CalculateCodebarDV(string valor)
        {
            // Caso foi inserido com os DACs, remove
            if (valor.Length == 46)
            {
                // Removendo os tres digitos verificadores da linha digitavel
                // do codigo de barras
                valor = valor.Remove(9, 1);
                valor = valor.Remove(19, 1);
                valor = valor.Remove(29, 1);

                string[] aux = new string[3];

                // Codigo do Banco + Num. Moeda
                aux[0] = valor.Substring(0, 4);
                // Nosso Numero + Agencia + Conta + DAC + Zeros
                aux[1] = valor.Substring(4, 25);
                // Fator Vencimento + Valor do Titulo
                // (Campo 5 da linha digitavel do codigo de barras)
                aux[2] = valor.Substring(29, 14);
                // Organizando a array para a string, para que seja calculado em seguida
                valor = aux[0] + aux[2] + aux[1];
            }

            // Tomando-se os 43 algarismos que compõem o Código de Barras (sem considerar a 5ª posição), 
            // multiplicando-os, iniciando-se da direita para a esquerda, pela sequência numérica de 2 a 9
            // (2, 3, 4, 5, 6, 7, 8, 9, 2, 3, 4... e assim por diante)

            int mult = 2;
            int sum = 0;

            for (int i = 42; i >= 0; i--)
            {
                char c = valor[i];
                int res = Convert.ToInt32(c.ToString()) * mult;
                sum += res;
                mult++;
                if (mult > 9) mult = 2;
            }

            int bDAC = 11 - (sum % 11);

            // Se o resultado desta for igual a 0, 1, 10 ou 11, considere DAC = 1.
            if (bDAC == 0) return 1;
            //if (bDAC == 1) return 1;
            if (bDAC == 10) return 1;
            if (bDAC == 11) return 1;

            return bDAC;
        }
        /// <summary>
        /// Executa o cálculo do Fator de Vencimento no formato "dd/mm/yyyy"
        /// </summary>
        /// <param name="stringVencimento">Texto da data de vencimento do boleto em "dd/mm/yyyy"</param>
        /// <returns>Fator de vencimento</returns>
        public static int CalculateDueDateFactor(string stringVencimento)
        {
            var dataVencimento = DateTime.ParseExact(stringVencimento, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return CalculateDueDateFactor(dataVencimento);
        }
        /// <summary>
        /// Executa o cálculo do Fator de Vencimento no formato "dd/mm/yyyy"
        /// </summary>
        /// <param name="dataVencimento">Data de vencimento do boleto</param>
        /// <returns>Fator de vencimento</returns>
        public static int CalculateDueDateFactor(DateTime dataVencimento)
        {
            // O fator de vencimento é o total de dias entre
            // a data base e a data de vencimento
            return (dataVencimento - DATA_BASE).Days;
        }
    }
}
