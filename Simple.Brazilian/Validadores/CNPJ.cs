﻿using System;

namespace Simple.Brazilian.Validadores
{
    public static class CNPJ
    {
        public static bool IsValid(string cnpj)
        {
            throw new NotImplementedException();
        }
        public static string Mask(string cnpj)
        {
            throw new NotImplementedException();
        }
        public static string Unmask(string cnpj) => Formatadores.Texto.RemoveMascara(cnpj);
        
    }
}
