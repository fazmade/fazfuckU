using System;
using System.Collections.Generic;
using System.IO;
//using System.Linq;
using System.Media;
using System.Text;

namespace fazfucku
{
    internal class Program
    {
        static string filelocation;
        static List<string> compileinfoes = new List<string>();
        static bool soundic = true;
        static float busydelay = 0f;
        static void Main(string[] args)
        {
            foreach (string lijn in File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "options.txt")))
            {
                string[] lijnbutseperated = lijn.Split(' ');
                switch (lijnbutseperated[0])
                {
                    case "sound":
                        soundic = lijnbutseperated[1] == "on";
                        break;
                    case "compileinfoes":
                        for (int i = 1; i < lijnbutseperated.Length; i++)
                            compileinfoes.Add(lijnbutseperated[i]);
                        break;
                    case "busydelay":
                        busydelay = fuckingparsefloatass(lijnbutseperated[1]);
                        break;
                }
            }
            Console.Title = "da official fafuckU compiler and fazass console runner!";
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
                            Console.WriteLine("no file location set!");
                        else if (!filelocation.EndsWith(".fzas"))
                            Console.WriteLine(filelocation + " isn't a fazass file!");
                        else if (!File.Exists(filelocation))
                            Console.WriteLine(filelocation + " doesn't exist!");
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
                                        case "typecheck":
                                        case "tch":
                                            myassisblicky.debug_typecheck = true;
                                            break;
                                        default:
                                            Console.WriteLine(input[i] + " is not a valid run setting!");
                                            break;
                                    }
                                }
                            }
                            try
                            {
                                myassisblicky.run();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                                funnywindowssound(soundeffect.bigerrorbro);
                            }
                            break;
                        }
                        funnywindowssound(soundeffect.error);
                        break;
                    case "compile":
                        if (filelocation == string.Empty)
                            Console.WriteLine("no file location set!");
                        else if (!filelocation.EndsWith(".fzfk"))
                            Console.WriteLine(filelocation + " isn't a fazfuck file!");
                        else if (!File.Exists(filelocation))
                            Console.WriteLine(filelocation + " doesn't exist!");
                        else
                        {
                            fazfuckucompiler compilah = new fazfuckucompiler();
                            compilah.inputfile = File.ReadAllLines(filelocation);
                            compilah.busydelay = busydelay;
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
                                            compilah.prematureoutput = input[i];
                                            break;
                                        case "listcompileinfostuff":
                                            listcompileinfostuff = true;
                                            break;
                                        case "nofunctions":
                                        case "nofunc":
                                            compilah.nofunctions = true;
                                            break;
                                        case "nolinenum":
                                        case "noln":
                                            compilah.includelinenumbers = false;
                                            break;
                                        default:
                                            Console.WriteLine(input[i] + " is not a valid compile setting!");
                                            break;
                                    }
                                }
                            }
                            foreach (string compileinfo in compileinfoes)
                            {
                                try
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
                                                case 0:
                                                    string[] flags = line.Split(' ');
                                                    foreach (string flag in flags)
                                                    {
                                                        switch (flag)
                                                        {
                                                            case "fstk_popremovebeforeset":
                                                                compilah.info_fstk_popremoveafterset = false;
                                                                break;
                                                            case "fstk_popremoveafterset":
                                                                compilah.info_fstk_popremoveafterset = true;
                                                                break;
                                                            case "fstk_staticsize":
                                                                compilah.info_arbitrarysize = false;
                                                                break;
                                                        }
                                                    }
                                                    break;
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
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
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
                            funnywindowssound(soundeffect.bigerrorbro);
                            break;
                        }
                        funnywindowssound(soundeffect.error);
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
                    case "saveoptions":
                        StringBuilder optionsbuild = new StringBuilder();
                        if (soundic)
                            optionsbuild.AppendLine("sound on");
                        else
                            optionsbuild.AppendLine("sound off");
                        optionsbuild.AppendLine("compileinfoes " + string.Join(" ", compileinfoes));
                        optionsbuild.Append("busydelay " + busydelay.ToString().Replace(',','.'));
                        File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "options.txt"), optionsbuild.ToString());
                        Console.WriteLine("saved options");
                        break;
                    case "quickswap":
                    case "qs":
                        if (filelocation.EndsWith(".fzfk"))
                        {
                            filelocation = Path.ChangeExtension(filelocation, "fzas");
                            Console.WriteLine("switched to fzas");
                        }
                        else if (filelocation.EndsWith(".fzas"))
                        {
                            filelocation = Path.ChangeExtension(filelocation, "fzfk");
                            Console.WriteLine("switched to fzfk");
                        }
                        else
                        {
                            Console.WriteLine("cannot quickswap from this file type!");
                        }
                        break;
                    case "quickconvert":
                    case "qc":
                        if (filelocation.EndsWith(".fzfk"))
                        {
                            string dafile = File.ReadAllText(filelocation);
                            dafile = dafile.Replace("<(", "⟨");
                            dafile = dafile.Replace(")>", "⟩");
                            dafile = dafile.Replace("!=", "≠");
                            dafile = dafile.Replace("<=", "≤");
                            dafile = dafile.Replace("=>", "≥");
                            dafile = dafile.Replace("<<", "«");
                            dafile = dafile.Replace(">>", "»");
                        }
                        else
                        {
                            Console.WriteLine("cannot quickconvert this file type!");
                        }
                        break;
                    case "seembusy":
                        if (input.Length == 2)
                        {
                            try
                            {
                                busydelay = fuckingparsefloatass(input[1]);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("couldn't change extra delay cuz you're stupid");
                            }
                        }
                        Console.WriteLine("extra delay: +" + busydelay + "x");
                        break;
                    case "help":
                        if (input.Length == 1)
                        {
                            Console.WriteLine("hi");
                            Console.WriteLine("list of commands:\n. diye\n. location\n. runsingle ([flags])\n. compile ([flags])\n. compileinfo (compileinfo)\n. sound\n. saveoptions\n. quickswap\n. quickconvert\n. seembusy delay\n. help (command)\n. clear");
                        }
                        else
                        {
                            switch (input[1])
                            {
                                case "diye":
                                    Console.WriteLine("closes the program.");
                                    break;
                                case "location":
                                    Console.WriteLine("tells you the currently selected file.");
                                    break;
                                case "runsingle":
                                    Console.WriteLine("runs the current fzas file.");
                                    Console.WriteLine("([flags]):\n. runwithdebug/ruwd: runner outputs line it shall run.\n. typecheck/tch: outputs the internal type of variables in certain situations.");
                                    break;
                                case "compile":
                                    Console.WriteLine("compiles the current fzfk file.");
                                    Console.WriteLine("([flags]):\n. includemarkers/inclm: includes the internal markers in the output (but you can't run it because of that).");
                                    Console.WriteLine(". prematureoutput/pmo initials: put in the initials of a compiler stage, and the compiler will output the file at that point instead.");
                                    Console.WriteLine(". listcompileinfostuff: outputs all of the things added by the compile infoes before compiling.");
                                    Console.WriteLine(". nofunctions/nofunc: skips the stage that deals with functions and local.");
                                    Console.WriteLine(". nolinenum/noln: doesn't append the original line numbers to the outputted file, so it's less cluttered if you need to inspect it (but you can't run it because of that).");
                                    break;
                                case "compileinfo":
                                    Console.WriteLine("outputs which compile infoes are selected and which are available.");
                                    Console.WriteLine("(compileinfo): adds or removes a compile info instead.");
                                    break;
                                case "sound":
                                    Console.WriteLine("toggle wether sound can be played.");
                                    break;
                                case "saveoptions":
                                    Console.WriteLine("saves certain options to a file to automatically set at launch.");
                                    Console.WriteLine("(sound, selected compile infoes, seem busy)");
                                    break;
                                case "quickswap":
                                    Console.WriteLine("alias: qs");
                                    Console.WriteLine("quickly swap between file types.");
                                    Console.WriteLine("available: fzfk <-> fzas");
                                    break;
                                case "quickconvert":
                                    Console.WriteLine("alias: qc");
                                    Console.WriteLine("convert quicker to type representations of syntax to the actual characters.");
                                    Console.WriteLine("available: fzfk");
                                    break;
                                case "seembusy":
                                    Console.WriteLine("adds an extra delay to each compiler stage to make it seem like it's doing more.");
                                    Console.WriteLine("(delay): the amount of added delay. multiplies the actual time it took and then waits for that long (so 1=2x, 9=10x). set to 0 to disable.");
                                    break;
                                case "help":
                                    Console.WriteLine("WHAT!? HELP ME!");
                                    break;
                                case "clear":
                                    Console.WriteLine("clears da consale");
                                    break;
                            }
                        }
                        break;
                    case "clear":
                        Console.Clear();
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
        public static float fuckingparsefloatass(string bitchassnumber)
        {
            return float.Parse(bitchassnumber.Replace(',', '.'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
