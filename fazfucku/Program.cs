using System;
using System.Collections.Generic;
using System.IO;

namespace fazfucku
{
    internal class Program
    {
        static string filelocation;
        static void Main(string[] args)
        {
            bool gobromytoelo = true;
            while (gobromytoelo)
            {
                Console.Write("inpoot:");
                string[] input = Console.ReadLine().Split(' ');
                switch (input[0])
                {
                    case "diye":
                        gobromytoelo = false;
                        break;
                    case "location":
                        if (input.Length > 1)
                            filelocation = input[1];
                        else
                            Console.WriteLine(filelocation);
                        break;
                    case "runsingle":
                        if (filelocation == string.Empty)
                            Console.WriteLine("no file location set!");
                        else if (!File.Exists(filelocation))
                            Console.WriteLine(filelocation + " doesn't exist!");
                        else
                        {
                            fazassrunner myassisblicky = new fazassrunner();
                            myassisblicky.lines = File.ReadAllLines(filelocation);
                            myassisblicky.run();
                        }
                        break;
                    default:
                        Console.WriteLine("WAT");
                        break;
                }
            }
        }
    }
}
