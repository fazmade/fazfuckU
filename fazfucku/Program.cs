using System;
using System.IO;
using System.Media;

namespace fazfucku
{
    internal class Program
    {
        static string filelocation;
        static string compileinfofile = string.Empty;
        static bool soundic = true;
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
                        Console.WriteLine(filelocation);
                        break;
                    case "runsingle":
                        if (filelocation == string.Empty)
                        {
                            funnywindowssound(soundeffect.error);
                            Console.WriteLine("no file location set!");
                        }
                        else if (!File.Exists(filelocation))
                        {
                            funnywindowssound(soundeffect.error);
                            Console.WriteLine(filelocation + " doesn't exist!");
                        }
                        else
                        {
                            fazassrunner myassisblicky = new fazassrunner();
                            myassisblicky.loadprogram(File.ReadAllLines(filelocation));
                            if (input.Length > 1)
                            {
                                for (int i = 1; i < input.Length; i++)
                                {
                                    switch (input[i])
                                    {
                                        case "runwithdebug":
                                        case "ruwd":
                                            myassisblicky.debug_runline = true;
                                            break;
                                    }
                                }
                            }
                            myassisblicky.run();
                        }
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
                                funnywindowssound(soundeffect.win); //it's the best part of the vs build process bro!
                                break;
                            }
                            else
                                Console.WriteLine("compiled file was empty!!");
                        }
                        funnywindowssound(soundeffect.bigerrorbro);
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
                                    {
                                        Console.WriteLine("invalid compile info! (" + location + ")");
                                        funnywindowssound(soundeffect.error);
                                    }
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
                        {
                            Console.WriteLine("WAT");
                            funnywindowssound(soundeffect.error);
                        }
                        break;
                }
            }
        }
        public enum soundeffect
        {
            win,
            error,
            bigerrorbro
        }
        public static void funnywindowssound(soundeffect sound)
        {
            if (!soundic)
                return;
            string soundname = "dshfhisakjhfiuwehrujmdsjkfghjopddbljkqwhfvshjstkashdfkjdfghjkyweh";
            Random thats_rad_dude = new Random();
            switch (sound)
            {
                case soundeffect.win:
                    soundname = "win.wav";
                    break;
                case soundeffect.error:
                    soundname = new string[] { "baby_hurt1.wav", "baby_hurt2.wav", "baby_hurt3.wav", "dad_hurt1.wav", "dad_hurt2.wav", "dad_hurt3.wav" }[thats_rad_dude.Next(6)];
                    break;
                case soundeffect.bigerrorbro:
                    soundname = new string[] { "baby_die.wav", "dad_die.wav" }[thats_rad_dude.Next(2)];
                    break;
            }
            SoundPlayer simpleSound = new SoundPlayer(Path.Combine(Directory.GetCurrentDirectory(), "sounds", soundname));
            simpleSound.Play();
            simpleSound.Dispose();
        }
    }
}
