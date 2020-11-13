using System;
using RSACipher;
using System.IO;
using System.Numerics;
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
            //BigInteger f = BigInteger.ModPow(18, 89, 119);
            //Console.WriteLine(f);
            //Console.ReadKey();

            //string Path1 = @"C:\Users\brazi\Desktop\ESTRUCTURA DE DATOS II\cuento.txt";

            //RSA rSA1 = new RSA();
            //rSA1.GetKeys(7, 17, out RSAkey Priv, out RSAkey Pub);
            //bool cipher = true;
            //if (cipher)
            //{
            //    string Path1 = @"C:\Users\brazi\Desktop\ESTRUCTURA DE DATOS II\easy test.txt";
            //    string Path2 = @"C:\Users\brazi\Desktop\clavecifrado.txt";
            //    RSA rSA = new RSA();



            //    byte[] arr;
            //    rSA.Cipher(Path1, out arr, Pub);

            //    using (FileStream fs = new FileStream(Path2, FileMode.Create))
            //    {
            //        for (int i = 0; i < arr.Length; i++)
            //        {
            //            fs.WriteByte(arr[i]);
            //        }
            //    }
            //}
            //cipher = false;
            //if (!cipher)
            //{
            //    string Path1 = @"C:\Users\brazi\Desktop\clavecifrado.txt";
            //    string Path2 = @"C:\Users\brazi\Desktop\clavedescifrado.txt";
            //    RSA rSA = new RSA();


            //    byte[] arr;
            //    rSA.Decipher(Path1, out arr, Priv);
            //    using (FileStream fs = new FileStream(Path2, FileMode.Create))
            //    {
            //        for (int i = 0; i < arr.Length; i++)
            //        {
            //            fs.WriteByte(arr[i]);
            //        }
            //    }
            //}

            using (FileStream fs = File.OpenRead(@"C:\Users\joseg\Desktop\Pruebas Cifrado\Descifrados\Cuentazo.txt"))
            {
                long L = fs.Length;
                using (FileStream fs2 = File.OpenRead(@"C:\Users\joseg\Desktop\Pruebas Cifrado\Originales\cuento.txt"))
                {
                    long L2 = fs2.Length;
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        using (BinaryReader reader1 = new BinaryReader(fs2))
                        {
                            int counter = 0;
                            byte array1;
                            byte array2;
                            while (counter < fs.Length)
                            {
                                array1 = reader.ReadByte();
                                array2 = reader1.ReadByte();
                                counter++;
                                if (array2 != array1)
                                {
                                    Console.WriteLine(false);
                                }
                            }
                        }
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
