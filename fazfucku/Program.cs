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
            bool runwithdebug = false;
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
                            myassisblicky.debug_runline = runwithdebug;
                            myassisblicky.run();
                        }
                        break;
                    case "togglerundebug":
                        runwithdebug = !runwithdebug;
                        if (runwithdebug)
                            Console.WriteLine("running with debug");
                        else
                            Console.WriteLine("not running with debug");
                        break;
                    case "compile":
                        if (filelocation == string.Empty)
                            Console.WriteLine("no file location set!");
                        else if (!File.Exists(filelocation))
                            Console.WriteLine(filelocation + " doesn't exist!");
                        else
                        {
                            fazfuckucompiler compilah = new fazfuckucompiler();
                            compilah.inputfile = File.ReadAllText(filelocation);
                            string thecompiled = compilah.compile();
                            if (thecompiled != string.Empty)
                            {
                                string compiledplace = Path.ChangeExtension(filelocation, "fzas");
                                if (!File.Exists(compiledplace))
                                    File.Create(compiledplace).Dispose();
                                File.WriteAllText(compiledplace, thecompiled);
                                Console.WriteLine("compild!");
                            }
                            else
                                Console.WriteLine("compiled file was empty!!");
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
