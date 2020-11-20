using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Numerics;
using System.Linq;

namespace RSACipher
{
    public class RSA : ICipher
    {
        BigInteger phi;
        int e_number;
        BigInteger d;
        public bool GetKeys(int p_Number, int q_Number, out RSAkey PrivateKey, out RSAkey PublicKey)
        {
            if (IsPrimeNumber(p_Number) && IsPrimeNumber(q_Number))
            {
                BigInteger N_number = p_Number;
                N_number = N_number * q_Number;
                if (N_number > int.MaxValue)
                {
                    PrivateKey = null;
                    PublicKey = null;
                    return false;
                }
                if (N_number < 255)
                {
                    PrivateKey = null;
                    PublicKey = null;
                    return false;
                }

                phi = (p_Number - 1);
                phi = phi * (q_Number - 1);
                if (phi > 2)
                {
                    Get_e();
                    if (e_number != -1)
                    {
                        Get_d();
                        if (d == e_number)
                        {
                            d += N_number;
                        }
                        PrivateKey = new RSAkey();
                        PrivateKey.modulus = N_number;
                        PrivateKey.power = d;
                        PublicKey = new RSAkey();
                        PublicKey.modulus = N_number;
                        PublicKey.power = e_number;
                        return true;
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
                if (number % i == 0)
                {
                    isPrime = false;
                    break;
                }
            }
            return isPrime;
        }
        void Get_e()
        {
            int toReturn = -1; ;
            for (int i = 2; i < phi; i++)
            {
                if (IsPrimeNumber(i) && (phi % i != 0))
                {
                    toReturn = i;
                    break;
                }
            }
            e_number = toReturn;
        }
        public void Get_d()
        {
            BigInteger Prev1 = phi, Prev2 = e_number, Prev3 = 0, New1 = phi, New2 = 1, New3 = 0, aux = 0;
            while (Prev2 != 1)
            {
                aux = (Prev1 / Prev2);
                Prev3 = Prev1 - (aux * Prev2);
                New3 = New1 - (New2 * aux);
                while (New3 < 0)
                {
                    New3 += phi;
                }
                Prev1 = Prev2;
                Prev2 = Prev3;
                New1 = New2;
                New2 = New3;
            }
            d = New3;
        }
        public bool Cipher(string route, out byte[] cipheredMsg, RSAkey PublicKey)
        {
            StringBuilder sb = new StringBuilder();
            using (FileStream fs = File.OpenRead(route))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    int counter = 0;
                    while (counter < fs.Length)
                    {
                        byte[] ByteArray = reader.ReadBytes(3);

                        for (int i = 0; i < ByteArray.Length; i++)
                        {
                            BigInteger toBinary = CihperByte(ByteArray[i], PublicKey);
                            string Binary = Convert.ToString((int)toBinary, 16);
                            sb.Append(Binary.PadLeft(8, '0'));
                        }
                        counter += 3;

                    }
                }

            }
            cipheredMsg = Encoding.ASCII.GetBytes(sb.ToString());
            return true;
        }

        public BigInteger CihperByte(int toCipher, RSAkey key)
        {
            BigInteger res = BigInteger.ModPow(toCipher, key.power, key.modulus);
            return res;

        }
        public bool Decipher(string route, out byte[] Message, RSAkey PrivateKey)
        {
            List<byte> toReturn = new List<byte>();
            using (FileStream fs = File.OpenRead(route))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    int counter = 0;
                    while (counter < fs.Length)
                    {
                        byte[] ByteArray = reader.ReadBytes(24);
                        int iterations = 1;
                        for (int i = 0; i < ByteArray.Length;)
                        {
                            List<byte> toDecimal = new List<byte>();
                            
                            for (int k = i; k < 8*iterations; k++)
                            {
                                toDecimal.Add(ByteArray[k]);
                                i++; 
                            }
                            iterations++;
                            string Decimal = Encoding.ASCII.GetString(toDecimal.ToArray());
                            BigInteger toBinary = CihperByte(Convert.ToInt32(Decimal, 16), PrivateKey);
                            toReturn.Add(Convert.ToByte((int)toBinary));
                        }
                        counter += 24;
                    }
                }
            }
            Message = toReturn.ToArray();
            return true;
        }

    }
}