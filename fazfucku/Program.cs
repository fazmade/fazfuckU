using System;
using System.Collections.Generic;
using System.IO;
using System.Media;

namespace fazfucku
{
    internal class Program
    {
        static string filelocation;
        static List<string> compileinfoes = new List<string> { "base" };
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
                            bool listcompileinfostuff = false;
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
                                        case "listcompileinfostuff":
                                            listcompileinfostuff = true;
                                            break;
                                    }
                                }
                            }
                            foreach (string compileinfo in compileinfoes)
                            {
                                string[] infoed = File.ReadAllLines(getcompileinfopath(compileinfo));
                                int section = 0;
                                foreach (string line in infoed)
                                {
                                    if (line == "flags")
                                        section = 0;
                                    //else if (line == "hooks")
                                    //    section = 1;
                                    else if (line == "function shorthands")
                                        section = 2;
                                    else if (line == "math functions")
                                        section = 3;
                                    else if (line == "special comparisons")
                                        section = 4;
                                    else if (line == "control gotoes")
                                        section = 5;
                                    else if (line != string.Empty)
                                    {
                                        switch (section)
                                        {
                                            case 2:
                                                string[] iuweasfiosdugiusdhgdhsj = line.Split(' ');
                                                compilah.shorthand.Add(iuweasfiosdugiusdhgdhsj[0], iuweasfiosdugiusdhgdhsj[1]);
                                                break;
                                            case 3:
                                                string[] jsdfsadhfkhsadfkhalhfaksdj = line.Split(' ');
                                                compilah.mathfunc.Add(jsdfsadhfkhsadfkhalhfaksdj[0], new string[] { jsdfsadhfkhsadfkhalhfaksdj[1], jsdfsadhfkhsadfkhalhfaksdj[2] });
                                                break;
                                            case 4:
                                                string[] djfsdajfkldsajfkldjksjdf = line.Split(' ');
                                                compilah.specialcomparisons.Add(djfsdajfkldsajfkldjksjdf[0], djfsdajfkldsajfkldjksjdf[1]);
                                                break;
                                            case 5:
                                                string[] shfjkhsdkjghksdjhgkjd = line.Split(' ');
                                                compilah.controlgotoes.Add(shfjkhsdkjghksdjhgkjd[0], new string[] { shfjkhsdkjghksdjhgkjd[1], shfjkhsdkjghksdjhgkjd[2] });
                                                break;
                                        }
                                    }
                                }
                            }
                            if (listcompileinfostuff)
                            {
                                Console.WriteLine("function shorthands");
                                foreach (KeyValuePair<string, string> jumbojosh in compilah.shorthand)
                                {
                                    Console.WriteLine(jumbojosh.Key + " > " + jumbojosh.Value);
                                }
                                Console.WriteLine("math functions");
                                foreach (KeyValuePair<string, string[]> jumbojosh in compilah.mathfunc)
                                {
                                    Console.WriteLine(jumbojosh.Key + " > " + jumbojosh.Value[0] + " " + jumbojosh.Value[1]);
                                }
                                Console.WriteLine("special comparisons");
                                foreach (KeyValuePair<string, string> jumbojosh in compilah.specialcomparisons)
                                {
                                    Console.WriteLine(jumbojosh.Key + " > " + jumbojosh.Value);
                                }
                                Console.WriteLine("control gotoes");
                                foreach (KeyValuePair<string, string[]> jumbojosh in compilah.controlgotoes)
                                {
                                    Console.WriteLine(jumbojosh.Key + " > " + jumbojosh.Value[0] + " " + jumbojosh.Value[1]);
                                }
                                Console.WriteLine();
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
                                Console.WriteLine();
                                if (compileinfoes.Count == 0)
                                    Console.WriteLine("none compile infoes selected. why?");
                                else
                                {
                                    Console.WriteLine("currently using:");
                                    foreach (string compileinfo in compileinfoes)
                                    {
                                        Console.WriteLine(compileinfo + " (" + getcompileinfopath(compileinfo) + ")");
                                    }
                                }
                                Console.WriteLine();
                                Console.WriteLine("available:");
                                foreach (string fiel in Directory.GetFiles(Directory.GetCurrentDirectory()))
                                {
                                    string name = Path.GetFileName(fiel);
                                    if (name.StartsWith("ci_") && name.EndsWith(".txt"))
                                        Console.WriteLine(getcompileinfoname(fiel) + " (" + fiel + ")");
                                }
                                Console.WriteLine();
                            }
                            else
                            {
                                if (compileinfoes.Contains(input[1]))
                                {
                                    compileinfoes.Remove(input[1]);
                                    Console.WriteLine("removed " + input[1] + " from the current compile infoes");
                                }
                                else
                                {
                                    string location = getcompileinfopath(input[1]);
                                    if (File.Exists(location))
                                    {
                                        compileinfoes.Add(input[1]);
                                        Console.WriteLine("added " + input[1] + " to the current compile infoes");
                                    }
                                    else
                                    {
                                        Console.WriteLine(input[1] + " at " + location + " is not an existant compile info");
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
                    case "sound":
                        soundic = !soundic;
                        if (soundic)
                            Console.WriteLine("sound on");
                        else
                            Console.WriteLine("sound off");
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
            string getcompileinfopath(string name)
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "ci_" + name + ".txt");
            }
            string getcompileinfoname(string path)
            {
                return Path.GetFileNameWithoutExtension(path).Substring(3);
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
