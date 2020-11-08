using System;
using System.Collections.Generic;
using System.Text;

namespace RSACipher
{
    public class RSA
    {
        int phi;
        public bool GetKeys(int p_Number, int q_Number, out RSAkey PrivateKey, out RSAkey PublicKey)
        {
            //p y q pueden ser iguales?
            //validar si p y q son primos
            if (IsPrimeNumber(p_Number) && IsPrimeNumber(q_Number))
            {
                int N_number = p_Number + q_Number;
                //el mínimo para phi es 3
                phi = (p_Number - 1) * (q_Number - 1);
                if (phi>2)
                {
                    int e_number;
                }
                else
                {
                    PrivateKey = null;
                    PublicKey = null;
                    return false;
                }
            }
            else
            {
                PrivateKey = null;
                PublicKey = null;
                return false;
            }
        }
        public bool IsPrimeNumber(int number)
        {
            bool isPrime = true;
            for (int i = 2; i < number; i++)
            {
                if(number%i == 0)
                {
                    isPrime = false;
                    break;
                }
            }
            return isPrime;
        }
        int Get_e()
        {
            int toReturn = -1; ;
            //el mínimo para phi es 3
            for (int i = 2; i < phi; i++)
            {
                if (IsPrimeNumber(i))
                {

                }
            }
        }
        bool Cipher(string route, out byte[] cipheredMsg, T Key);
        bool Decipher(string route, out byte[] Message, T Key);
    }

