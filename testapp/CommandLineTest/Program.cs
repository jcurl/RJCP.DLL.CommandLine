using System;

namespace CommandLineTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Argument Length: {0}", args.Length);
            for (int i = 0; i < args.Length; i++) {
                Console.WriteLine(" Argument {0}: -=> {1} <=-", i, args[i]);
            }
        }
    }
}
