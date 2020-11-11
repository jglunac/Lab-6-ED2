using System;
using System.Numerics;
using System.IO;
using RSACipher;
namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //int Prev1 = 3120, Prev2 = 17, Prev3 = 0, New1 = 3120, New2 = 1, New3 = 0, aux = 0;
            //while (Prev2 != 1)
            //{
            //    aux = (Prev1 / Prev2);
            //    Prev3 = Prev1 - (aux * Prev2);
            //    New3 = New1 - (New2 * aux);
            //    if (New3 < 0)
            //    {
            //        New3 += 3120;
            //    }
            //    Prev1 = Prev2;
            //    Prev2 = Prev3;
            //    New1 = New2;
            //    New2 = New3;
            //}
            //Console.WriteLine(New2);
            //BigInteger f = BigInteger.ModPow(531, 17, 3233);
            //Console.WriteLine(f);
            //Console.ReadKey();
            string Path1 = @"C:\Users\brazi\Desktop\clave";
            string Path2 = @"C:\Users\brazi\Desktop\clavecifrado";
            RSA rSA = new RSA();
            RSAkey rSAkey = new RSAkey();
            rSAkey.power = 17;
            rSAkey.modulus = 3233;
            byte[] arr;
            rSA.Cipher(Path1, out arr, rSAkey);
            using (FileStream fs = new FileStream(Path2, FileMode.Create ))
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    fs.WriteByte(arr[i]);
                }
            }
        }
    }
}
