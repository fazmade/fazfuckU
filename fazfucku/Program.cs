using System;
using System.IO;
using System.Media;

namespace fazfucku
{
    internal class Program
    {
        static string filelocation;
        static string compileinfofile = string.Empty;
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
                            compilah.inputfile = File.ReadAllLines(filelocation);
                            if (input.Length > 1)
                            {
                                for (int i = 1; i < input.Length; i++)
                                {
                                    switch (input[i])
                                    {
                                        case "includemarkers":
                                        case "inclm":
                                            compilah.includemarkers = true;
                                            break;
                                        case "prematureoutput":
                                        case "pmo":
                                            i++;
                                            compilah.prematureoutput = int.Parse(input[i]);
                                            break;
                                    }
                                }
                            }
                            if (compileinfofile != string.Empty)
                            {
                                string[] infoed = File.ReadAllLines(compileinfofile);
                                int section = 0;
                                foreach (string line in infoed)
                                {
                                    if (line == "hooks")
                                        section = 1;
                                    else if (line == "function shorthands")
                                        section = 2;
                                    else if (line != string.Empty)
                                    {
                                        switch (section)
                                        {
                                            case 2:
                                                string[] iuweasfiosdugiusdhgdhsj = line.Split(' ');
                                                compilah.shorthand.Add(iuweasfiosdugiusdhgdhsj[0], iuweasfiosdugiusdhgdhsj[1]);
                                                break;
                                        }
                                    }
                                }
                            }
                            string thecompiled = compilah.compile();
                            if (thecompiled != string.Empty)
                            {
                                string compiledplace = Path.ChangeExtension(filelocation, "fzas");
                                if (!File.Exists(compiledplace))
                                    File.Create(compiledplace).Dispose();
                                File.WriteAllText(compiledplace, thecompiled);
                                Console.WriteLine("compild!");
                                funnywindowssound("tada.wav"); //it's the best part of the vs build process bro!
                                break;
                            }
                            else
                                Console.WriteLine("compiled file was empty!!");
                        }
                        funnywindowssound("Windows Error.wav");
                        break;
                    case "compileinfo":
                        try
                        {
                            if (input.Length == 1)
                            {
                                if (compileinfofile == string.Empty)
                                    Console.WriteLine("no compile info selected");
                                else
                                    Console.WriteLine("currently using " + Path.GetFileNameWithoutExtension(compileinfofile).Substring(3) + " (" + compileinfofile + ")");
                                Console.WriteLine("available:");
                                foreach (string fiel in Directory.GetFiles(Directory.GetCurrentDirectory()))
                                {
                                    string name = Path.GetFileName(fiel);
                                    if (name.StartsWith("ci_") && name.EndsWith(".txt"))
                                        Console.WriteLine(name.Substring(3, name.Length - 7));
                                }
                            }
                            else
                            {
                                if (input[1] == string.Empty)
                                    compileinfofile = "";
                                else
                                {
                                    string location = Path.Combine(Directory.GetCurrentDirectory(), "ci_" + input[1] + ".txt");
                                    if (File.Exists(location))
                                    {
                                        compileinfofile = location;
                                        Console.WriteLine("now using " + input[1] + " (" + compileinfofile + ")");
                                    }
                                    else
                                        Console.WriteLine("invalid compile info! (" + location + ")");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                        break;
                    default:
                        if (File.Exists(input[0]))
                            filelocation = input[0];
                        else
                            Console.WriteLine("WAT");
                        break;
                }
            }
            void funnywindowssound(string sound)
            {
                SoundPlayer simpleSound = new SoundPlayer(@"c:\Windows\Media\" + sound);
                simpleSound.Play();
                simpleSound.Dispose();
            }
        }
    }
}
