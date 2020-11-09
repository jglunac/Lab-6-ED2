using System;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            int Prev1 = 3120, Prev2 = 17, Prev3 = 0, New1 = 3120, New2 = 1, New3 = 0, aux = 0;
            while (Prev2 != 1)
            {
                aux = (Prev1 / Prev2);
                Prev3 = Prev1 - (aux * Prev2);
                New3 = New1 - (New2 * aux);
                if (New3 < 0)
                {
                    New3 += 3120;
                }
                Prev1 = Prev2;
                Prev2 = Prev3;
                New1 = New2;
                New2 = New3;
            }
            Console.WriteLine(New2);
        }
    }
}
