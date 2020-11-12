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
        int N_bits;
        string incompleteByte = "";
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
            N_bits = Convert.ToString(PublicKey.modulus, 2).Length;
            if (PublicKey.modulus >0)
            {
                int ToReadBits = N_bits - 1;
                e_number = PublicKey.power;
                bool exit = false;
                FinalBytes = new List<byte>();
                using (FileStream fs = File.OpenRead(route))
                {
                    long aux = fs.Length;
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        int counter = 0;
                        string actualNumber = "", actualByte = "";
                        while (counter < fs.Length)
                        {
                            byte[] ByteArray = reader.ReadBytes(1000);
                            exit = false;
                            int i = 0;
                            while (!exit)
                            {
                                int missingbits = ToReadBits - actualNumber.Length;
                                if (missingbits > actualByte.Length)
                                {

                                    if (i < ByteArray.Length)
                                    {
                                        actualNumber += actualByte;
                                        actualByte = Convert.ToString(ByteArray[i], 2).PadLeft(8, '0');
                                    }
                                    else exit = true;
                                    i++;
                                }
                                else
                                {

                                    //if (i >= ByteArray.Length && missingbits>=actualByte.Length)
                                    //{
                                    //    exit = true;
                                    //    actualNumber += actualByte;
                                    //    actualNumber = actualNumber.PadRight(ToReadBits, '0');
                                    //}
                                    //else
                                    //{

                                    //}
                                    actualNumber += actualByte.Substring(0, missingbits);
                                    actualByte = actualByte.Remove(0, missingbits);

                                }

                                if (actualNumber.Length == ToReadBits)
                                {
                                    int ToCipher = Convert.ToInt32(actualNumber, 2);
                                    Write_C(ToCipher, Calculate_C(ToCipher, PublicKey.modulus), true);
                                    actualNumber = "";
                                }
                            }
                            counter += 1000;
                        }
                        if (actualNumber.Length > 0 || actualByte.Length>0)
                        {
                            actualNumber += actualByte;
                            int ToCipher = Convert.ToInt32(actualNumber.PadRight(ToReadBits, '0'), 2);
                            Write_C(ToCipher, Calculate_C(ToCipher, PublicKey.modulus), true);
                            actualNumber = "";
                        }
                        Write_C(0, Calculate_C(0, PublicKey.modulus), true);
                    }
                }
                cipheredMsg = new byte[FinalBytes.Count];
                cipheredMsg = FinalBytes.ToArray();
                return true;
            }
            else
            {
                cipheredMsg = null;
                return false;
            }
            
        }
        int Calculate_C(int m_number, int n_number)
        {
            return (int)BigInteger.ModPow(m_number, e_number, n_number);
            
        }
         void Write_C(int m_number, int towrite, bool Cipher)
        {
            int ToAdd = -1;
            string binary_C = "";
            int C_size;
            if (Cipher) C_size = N_bits;
            else C_size = N_bits - 1;
            if (m_number != 0)
            {
                binary_C = Convert.ToString(towrite, 2);
                binary_C = binary_C.PadLeft(C_size, '0');
            }
            if (incompleteByte.Length > 0)
            {
                int missingbits = 8 - incompleteByte.Length;
                if (binary_C.Length >= missingbits)
                {
                    ToAdd = Convert.ToInt32(incompleteByte + binary_C.Substring(0, missingbits), 2);
                    binary_C = binary_C.Remove(0, missingbits);
                    FinalBytes.Add((byte)ToAdd);
                    incompleteByte = "";
                }
                //else if (binary_C != "")
                //{
                //    incompleteByte += binary_C;
                //    binary_C = "";
                //}
                else
                {
                    incompleteByte = incompleteByte.PadRight(8, '0');
                    ToAdd = Convert.ToInt32(incompleteByte, 2);
                    FinalBytes.Add((byte)ToAdd);
                }
            }
            while (binary_C.Length >= 8)
            {
                ToAdd = Convert.ToInt32(binary_C.Substring(0, 8), 2);
                binary_C = binary_C.Remove(0, 8);
                FinalBytes.Add((byte)ToAdd);
            }
            if (binary_C.Length > 0)
            {
                if (N_bits >= 8 || !Cipher) incompleteByte = binary_C;
                else
                {
                    ToAdd = Convert.ToInt32(binary_C, 2);

                    FinalBytes.Add((byte)ToAdd);
                }
            }
        }

        public bool Decipher(string route, out byte[] Message, RSAkey PrivateKey)
        {
            N_bits = Convert.ToString(PrivateKey.modulus, 2).Length;
            if (PrivateKey.modulus > 0)
            {
                int ToReadBits;
                if (PrivateKey.modulus >= 8) ToReadBits = N_bits;
                else ToReadBits = 8;
                e_number = PrivateKey.power;
                bool exit = false;
                FinalBytes = new List<byte>();
                using (FileStream fs = File.OpenRead(route))
                {
                    long auxiliarghh = fs.Length;
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        int counter = 0;
                        string actualNumber = "", actualByte = "";
                        while (counter < fs.Length)
                        {
                            byte[] ByteArray = reader.ReadBytes(1000);
                            exit = false;
                            int i = 0;
                            while (!exit)
                            {
                                int missingbits = ToReadBits - actualNumber.Length;
                                if (missingbits > actualByte.Length)
                                {
                                    
                                    if (i < ByteArray.Length)
                                    {
                                        actualNumber += actualByte;
                                        actualByte = Convert.ToString(ByteArray[i], 2).PadLeft(8, '0');
                                    }
                                    else exit = true;
                                    i++;
                                }
                                else
                                {
                                    actualNumber += actualByte.Substring(0, missingbits);
                                    actualByte = actualByte.Remove(0, missingbits);
                                }

                                if (actualNumber.Length == ToReadBits)
                                {
                                    int ToDecipher = Convert.ToInt32(actualNumber, 2);
                                    int aux = Calculate_C(ToDecipher, PrivateKey.modulus);
                                    Write_C(ToDecipher, aux, false);
                                    actualNumber = "";
                                }
                            }
                            counter += 1000;
                        }
                        
                        
                    }
                }
                Message = new byte[FinalBytes.Count];
                Message = FinalBytes.ToArray();
                return true;
            }
            else
            {
                Message = null;
                return false;
            }
        }
    }
}

