﻿using System;
using System.Linq;

namespace Simple.Brazilian.Validators
{
    public static class Boleto
    {
        /// <summary>
        /// Executa o calculo do digito verificador
        /// do campo da linha digitavel 1, 2, ou 3 do boleto
        /// </summary>
        /// <param name="campoLinhaDigitavel"></param>
        /// <returns>int DigitoVerificador</returns>
        public static int CalculaDigitoVerificador(string campoLinhaDigitavel)
        {
            int mult = 2;
            int sum = 0;

            for (int i = (campoLinhaDigitavel.Length - 1); i >= 0; i--)
            {
                char c = campoLinhaDigitavel[i];

                int res = Convert.ToInt32(c.ToString()) * mult;
                sum += res > 9 ? (res - 9) : res;
                mult = mult == 2 ? 1 : 2;
            }

            int DigitoVerificador = 10 - (sum % 10);
            if (DigitoVerificador == 10) DigitoVerificador = 0;

            return DigitoVerificador;
        }
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
        public static int CalculoDACcodigoBarras(string valor)
        {
            // Caso foi insirido 
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
                // Organizando array para a string para que seja calculado
                valor = aux[0] + aux[2] + aux[1];
            }

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

            if (bDAC == 0) return _ = 1;
            if (bDAC == 1) return _ = 1;
            if (bDAC == 10) return _ = 1;
            if (bDAC == 11) return _ = 1;

            return bDAC;
        }
        /// <summary>
        /// Executa o cálculo do Fator de Vencimento no formato "dd/mm/yyyy"
        /// </summary>
        /// <param name="dataVencimentoRaw"></param>
        /// <returns>int fatorVencimento</returns>
        public static int FatorVencimento(string dataVencimentoRaw)
        {
            // Uma int array recebe os valores da data sem as barras '/'
            var dataUnmask = dataVencimentoRaw.Split('/')
                                          .Select(Int32.Parse)
                                          .ToArray();

            // Data de refêrencia imutável
            var dataInicial = new DateTime(1997, 10, 07);

            // Recebe as datas nas seguintes ordens:
            // Ano posição 2, Mês posição 1, Dia posição 0
            var dataVencimento = new DateTime(dataUnmask[2],
                                              dataUnmask[1], 
                                              dataUnmask[0]);

            // O fator de vencimento é o total de dias entre
            // a data inicial e a data de vencimento
            var fatorVencimento = (dataVencimento - dataInicial).Days;

            return fatorVencimento;
        }
    }
}
