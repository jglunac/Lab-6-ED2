using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Numerics;

namespace RSACipher
{
    public class RSA
    {
        int phi;
        int e_number;
        int d;
        string incompleteByte;
        List<byte> FinalBytes;
        public bool GetKeys(int p_Number, int q_Number, out RSAkey PrivateKey, out RSAkey PublicKey)
        {
            //p y q pueden ser iguales?
            //validar si p y q son primos
            if (IsPrimeNumber(p_Number) && IsPrimeNumber(q_Number))
            {
                int N_number = p_Number * q_Number;
                //el mínimo para phi es 3
                phi = (p_Number - 1) * (q_Number - 1);
                if (phi > 2)
                {
                    Get_e();
                    if (e_number != -1)
                    {
                        Get_d();
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
                }
            }
            e_number = toReturn;
        }

        public void Get_d()
        {
            int Prev1 = phi, Prev2 = e_number, Prev3 = 0, New1 = phi, New2 = 1, New3 = 0, aux = 0;
            while (Prev2 != 1)
            {
                aux = (Prev1 / Prev2);
                Prev3 = Prev1 - (aux * Prev2);
                New3 = New1 - (New2 * aux);
                if (New3 < 0)
                {
                    New3 += phi;
                }
                Prev1 = Prev2;
                Prev2 = Prev3;
                New1 = New2;
                New2 = New3;
            }
            d = Prev2;
        }
        public bool Cipher(string route, out byte[] cipheredMsg, RSAkey PublicKey)
        {
            int N_bits = Convert.ToString(PublicKey.modulus, 2).Length;
            e_number = PublicKey.power;
            bool exit = false;
            FinalBytes = new List<byte>();
            using (FileStream fs = File.OpenRead(route))
            {
                
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    int counter = 0;
                    while (counter < fs.Length)
                    {
                        byte[] ByteArray = reader.ReadBytes(1000);
                        string actualNumber = "", actualByte = "";
                        int i = 0;
                        while (i<ByteArray.Length)
                        {
                            int missingbits = N_bits - actualNumber.Length;
                            if (missingbits > actualByte.Length)
                            {
                                actualNumber += actualByte;
                                actualByte = Convert.ToString(ByteArray[i], 2);
                                i++;
                            }
                            else
                            {
                                actualNumber += actualByte.Substring(0, missingbits);
                                actualByte = actualByte.Remove(0, missingbits);
                            } 
                            
                            if (actualNumber.Length == N_bits)
                            {
                                int ToCipher = Convert.ToInt32(actualNumber, 2);
                                if (ToCipher >= PublicKey.modulus)
                                {
                                    ToCipher = Convert.ToInt32(actualNumber.Substring(0, N_bits - 1), 2);
                                    actualByte = actualNumber[actualNumber.Length - 1] + actualByte;
                                }
                                
                                Calculate_C(ToCipher, PublicKey.modulus);
                                actualNumber = "";

                            }
                        }
                        counter += 1000;
                    }

                }
            }
            cipheredMsg = new byte[3];
            return true;
        }
        void Calculate_C(int m_number, int n_number)
        {
            BigInteger c_number = BigInteger.ModPow(m_number, e_number, n_number);
            
            string binary_C = Convert.ToString(Convert.ToInt32(c_number), 2);
            while (binary_C.Length >= 8)
            {
                int ToAdd = Convert.ToInt32(binary_C.Substring(0, 8), 2);
                binary_C = binary_C.Remove(0, 8);
                FinalBytes.Add((byte)ToAdd);
            }
            if (binary_C.Length > 0) incompleteByte = binary_C;
        }

        public bool Decipher(string route, out byte[] Message, RSAkey Key)
        {
            Message = new byte[3];
            return true;
        }
    }
}

