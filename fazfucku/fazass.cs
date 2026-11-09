using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace fazfucku
{
    internal class fazassrunner
    {
        private string[][] lines;
        private int[] linenum;
        private Dictionary<string, object> globaldict = new Dictionary<string, object>();
        private int counter = 0;
        //static Dictionary<string, int> pointers = new Dictionary<string, int>();
        //static bool isinhook = false;
        private List<object> funcstack = new List<object>();
        private bool blankvar;
        public bool debug_runline = false;
        private float mathhalfrot = (float)Math.PI;
        public void loadprogram(string[] prog)
        {
            List<string[]> processinglines = new List<string[]>();
            List<int> processinglinenum = new List<int>();
            foreach (string line in prog)
            {
                string[] split = line.Split('|'); //allovit
                string[] values = new string[split.Length-1]; //actual function data
                Array.ConstrainedCopy(split, 0, values, 0, split.Length - 1); //copies them all except the original line number
                processinglinenum.Add(int.Parse(split[split.Length - 1])); //store original line number
                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i].StartsWith("\""))
                        values[i] = values[i].Replace("\\n", "\n").Replace("\\p", "|").Replace("\\b", "\\");
                }
                processinglines.Add(values);
            }
            lines = processinglines.ToArray();
            linenum = processinglinenum.ToArray();
        }
        public void run()
        {
            while(counter != -1)
            {
                if (debug_runline)
                    debugline(string.Join(" ", lines[counter]));
                counter = runline(lines[counter]);
            }
        }
        private int runline(string[] line)
        {
            try
            {
                switch (line[0])
                {
                    case "pr":
                        Console.WriteLine(getvar(line[1]));
                        break;
                    case "rel":
                        return -1;
                    case "go":
                        return int.Parse(line[1]);
                    case "cdg"://conddontgoto
                        if (!blankvar)
                            return int.Parse(line[1]);
                        break;
                    case "csg"://condsplitgoto
                        if (blankvar)
                            return int.Parse(line[2]);
                        else
                            return int.Parse(line[1]);
                    case "cdog"://conddogoto
                        if (blankvar)
                            return int.Parse(line[1]);
                        break;
                    case "condset":
                        if (blankvar)
                            setvar(line[1], getvar(line[3]));
                        else
                            setvar(line[1], getvar(line[2]));
                        break;
                    case "indexset":
                        {
                            int index = (int)Math.Floor((float)getvar(line[2]));
                            if (index < 0 || index > (line.Length - 4))
                                setvar(line[1], getvar(line[3]));
                            else
                                setvar(line[1], getvar(line[index + 4]));
                        }
                        break;
                    case "c":
                        {
                            bool result = false;
                            object obj1 = getvar(line[2]);
                            object obj2 = getvar(line[4]);
                            switch (line[3])
                            {
                                case "=":
                                    result = obj1.Equals(obj2);
                                    break;
                                case "≠":
                                    result = !obj1.Equals(obj2);
                                    break;
                                case "<":
                                    result = (float)obj1 < (float)obj2;
                                    break;
                                case ">":
                                    result = (float)obj1 > (float)obj2;
                                    break;
                                case "≤":
                                    result = (float)obj1 <= (float)obj2;
                                    break;
                                case "≥":
                                    result = (float)obj1 >= (float)obj2;
                                    break;
                                case "&":
                                    result = (bool)obj1 && (bool)obj2;
                                    break;
                                case "|":
                                    result = (bool)obj1 || (bool)obj2;
                                    break;
                                case "c":
                                    result = ((string)obj1).Contains((string)obj2);
                                    break;
                                case "!c":
                                    result = !((string)obj1).Contains((string)obj2);
                                    break;
                                case "s":
                                    result = ((string)obj1).StartsWith((string)obj2);
                                    break;
                                case "!s":
                                    result = !((string)obj1).StartsWith((string)obj2);
                                    break;
                                case "e":
                                    result = ((string)obj1).EndsWith((string)obj2);
                                    break;
                                case "!e":
                                    result = !((string)obj1).EndsWith((string)obj2);
                                    break;
                            }
                            setvar(line[1], result);
                        }
                        break;
                    case "mx": //math any
                        {
                            float result = float.NaN;
                            float num1 = (float)getvar(line[3]);
                            switch (line[2])
                            {
                                case "mx":
                                    result = num1;
                                    for (int i = 4; i < line.Length; i++)
                                    {
                                        float num = (float)getvar(line[i]);
                                        if (num > result)
                                            result = num;
                                    }
                                    break;
                                case "mn":
                                    result = num1;
                                    for (int i = 4; i < line.Length; i++)
                                    {
                                        float num = (float)getvar(line[i]);
                                        if (num < result)
                                            result = num;
                                    }
                                    break;
                                case "c+":
                                    result = num1;
                                    for (int i = 4; i < line.Length; i++)
                                        result += (float)getvar(line[i]);
                                    break;
                                case "c×":
                                    result = num1;
                                    for (int i = 4; i < line.Length; i++)
                                        result *= (float)getvar(line[i]);
                                    break;
                                case "sa":
                                    result = num1 * num1;
                                    for (int i = 4; i < line.Length; i++)
                                    {
                                        float num = (float)getvar(line[i]);
                                        result += num * num;
                                    }
                                    break;
                            }
                            setvar(line[1], result);
                        }
                        break;
                    case "m1": //math 1
                        {
                            float result = float.NaN;
                            float num1 = (float)getvar(line[3]);
                            switch (line[2])
                            {
                                case "s"://sign
                                    if (num1 == 0f)
                                        result = 0f;
                                    else if (num1 > 0f)
                                        result = 1f;
                                    else if (num1 < 0f)
                                        result = -1f;
                                    break;
                                case "sq": //sqrt
                                    result = (float)Math.Sqrt(num1);
                                    break;
                            }
                            setvar(line[1], result);
                        }
                        break;
                    case "m2": //math 2
                        {
                            float result = float.NaN;
                            float num1 = (float)getvar(line[3]);
                            float num2 = (float)getvar(line[4]);
                            switch (line[2])
                            {
                                case "+":
                                    result = num1 + num2;
                                    break;
                                case "-":
                                    result = num1 - num2;
                                    break;
                                case "×":
                                    result = num1 * num2;
                                    break;
                                case "÷":
                                    result = num1 / num2;
                                    break;
                                case "%":
                                    result = num1 % num2;
                                    break;
                                case "^":
                                    result = (float)Math.Pow(num1, num2);
                                    break;
                                case "mx": //max
                                    if (num1 > num2)
                                        result = num1;
                                    else
                                        result = num2;
                                    break;
                                case "mn": //min
                                    if (num1 < num2)
                                        result = num1;
                                    else
                                        result = num2;
                                    break;
                                case "rn": //round
                                    result = (float)Math.Round(num1 / num2) * num2;
                                    break;
                                case "fl": //floor
                                    result = (float)Math.Floor(num1 / num2) * num2;
                                    break;
                                case "cl": //cieling
                                    result = (float)Math.Ceiling(num1 / num2) * num2;
                                    break;
                                case "_": //log
                                    result = (float)Math.Log(num1, num2);
                                    break;
                            }
                            setvar(line[1], result);
                        }
                        break;
                    case "m3": //math 3
                        {
                            float result = float.NaN;
                            float num1 = (float)getvar(line[3]);
                            float num2 = (float)getvar(line[4]);
                            float num3 = (float)getvar(line[5]);
                            switch (line[2])
                            {
                                case "lf":
                                    result = num1 * num2 + num2;
                                    break;
                            }
                            setvar(line[1], result);
                        }
                        break;
                    case "mt": //math trig
                        {
                            float result = float.NaN;
                            float num = (float)getvar(line[3]) * (float)Math.PI / mathhalfrot;
                            switch (line[2])
                            {
                                case "s": //sin
                                    result = (float)Math.Sin(num);
                                    break;
                                case "c": //cos
                                    result = (float)Math.Cos(num);
                                    break;
                                case "t": //tan
                                    result = (float)Math.Tan(num);
                                    break;
                                case "as": //asin
                                    result = (float)Math.Asin(num);
                                    break;
                                case "ac": //acos
                                    result = (float)Math.Acos(num);
                                    break;
                                case "at": //atan
                                    result = (float)Math.Atan(num);
                                    break;
                            }
                            setvar(line[1], result);
                        }
                        break;
                    case "s":
                        setvar(line[1],getvar(line[2]));
                        break;
                    case "ud":
                        if (globaldict.ContainsKey(line[1]))
                            globaldict.Remove(line[1]);
                        break;
                    case "fa": //funcstack_add
                        for (int i = 1; i < line.Length; i++)
                        {
                            funcstack.Insert(0, getvar(line[i]));
                        }
                        break;
                    case "fd": //funcstack_discard
                        funcstack.RemoveRange(0, int.Parse(line[1]));
                        break;
                    case "fg": //funcstack_popgoto
                        {
                            int jumbojosh = (int)funcstack[0];
                            funcstack.RemoveAt(0);
                            return jumbojosh;
                        }
                    case "fp": //funcstack_pop
                        for (int i = 1; i < line.Length; i++)
                        {
                            setvar(line[i], funcstack[0]);
                            funcstack.RemoveAt(0);
                        }
                        break;
                    case "tstr": //to_string
                        {
                            object jumbojosh = getvar(line[2]);
                            Type typojosh = jumbojosh.GetType();
                            if (typojosh == typeof(bool))
                                setvar(line[1], (bool)jumbojosh ? "true" : "false");
                            else if (typojosh == typeof(float))
                                setvar(line[1], jumbojosh.ToString().Replace(',', '.'));
                            else
                                setvar(line[1], jumbojosh.ToString());
                        }
                        break;
                    case "prs": //parse
                        setvar(line[1], getvar((string)getvar(line[2]))); //aha yes.
                        break;
                    case "str_rep": //string_replace
                        setvar(line[1], ((string)getvar(line[2])).Replace((string)getvar(line[3]), (string)getvar(line[4])) );
                        break;
                    case "str_lw": //string_lower
                        setvar(line[1], ((string)getvar(line[2])).ToLowerInvariant());
                        break;
                    case "str_up": //string_upper
                        setvar(line[1], ((string)getvar(line[2])).ToLowerInvariant());
                        break;
                    case "str_sub": //string_substring
                        setvar(line[1], ((string)getvar(line[2])).Substring((int)getvar(line[3]), (int)getvar(line[4])));
                        break;
                    case "str_spl": //string_split
                        setvar(line[1], ((string)getvar(line[2])).Split(((string)getvar(line[3]))[0]));
                        break;
                    case "str_joe": //string_join
                        {
                            string result = (string)getvar(line[2]);
                            for (int i = 3; i < line.Length; i++)
                                result += (string)getvar(line[i]);
                            setvar(line[1], result);
                        }
                        break;
                    case "str_len": //string_length
                        setvar(line[1], ((string)getvar(line[2])).Length);
                        break;

                    //list functions
                    case "l_cr": //list_create
                        setvar(line[1], new List<object>());
                        break;
                    case "l_at": //list_addtop
                        ((List<object>)getvar(line[1])).Add(getvar(line[2]));
                        break;
                    case "l_aa": //list_addat
                        ((List<object>)getvar(line[1])).Insert((int)getvar(line[3]), getvar(line[2]));
                        break;
                    case "l_s": //list_set
                        ((List<object>)getvar(line[1]))[(int)getvar(line[3])] = getvar(line[2]);
                        break;
                    case "l_g": //list_get
                        setvar(line[1], ((List<object>)getvar(line[2]))[(int)getvar(line[3])]);
                        break;
                    case "l_am": //list_amount
                        setvar(line[1], ((List<object>)getvar(line[2])).Count - (float)getvar(line[3]));
                        break;
                    case "l_rf": //list_removefirst
                        ((List<object>)getvar(line[1])).Remove(getvar(line[2]));
                        break;
                    case "l_ra": //list_removeat
                        ((List<object>)getvar(line[1])).RemoveAt((int)getvar(line[2]));
                        break;
                    case "l_rl": //list_removeallof
                        object checkthing = getvar(line[2]);
                        Predicate<object> isthing = the => the.Equals(checkthing);
                        ((List<object>)getvar(line[1])).RemoveAll(isthing);
                        break;
                    case "l_cl": //list_clear
                        ((List<object>)getvar(line[1])).Clear();
                        break;
                    case "l_i": //list_indexof
                        setvar(line[1], ((List<object>)getvar(line[2])).IndexOf(getvar(line[3])));
                        break;
                    case "l_h": //list_has
                        setvar(line[1], ((List<object>)getvar(line[2])).Contains(getvar(line[3])));
                        break;

                    //COMSOEL?!
                    case "cons_gtline":
                        setvar(line[1], Console.ReadLine());
                        break;
                    case "cons_cheesc":
                        if (Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Escape)
                            return -1;
                        break;
                    case "cons_gtkey":
                        setvar(line[1], (int)Console.ReadKey(true).Key);
                        break;
                    case "blah":
                        Console.WriteLine("blah" + line[1]);
                        break;
                    case "debug_variables":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("---globaldict");
                        foreach (KeyValuePair<string, object> entry in globaldict)
                            Console.WriteLine(entry.Key + " : " + entry.Value.ToString());
                        Console.WriteLine("---funcstack");
                        Console.WriteLine(string.Join("|", funcstack));
                        Console.WriteLine("---other");
                        Console.WriteLine("blankvar: " + blankvar);
                        Console.WriteLine("mathhalfrot: " + mathhalfrot);
                        Console.ResetColor();
                        break;
                }
                return counter + 1;
            }
            catch (Exception e)
            {
                doerror(e.ToString());
                return -1;
            }
        }
        private void setvar(string identifier, object value)
        {
            if (identifier == "T" || identifier == "F" || identifier == "NUT")
            {
                doerror(identifier + " is a constant value!");
                return;
            }
            char firstchar = identifier[0];
            if (firstchar == '\"' || firstchar == '#')
            {
                doerror(identifier + " is a constant value!");
                return;
            }
            if (firstchar == '-' || firstchar == '+' || firstchar == '+')
            {
                doerror(identifier + " cannot set a variable with a modifier!");
                return;
            }
            if (identifier == "B")
            {
                blankvar = (bool)value;
                return;
            }
            if (identifier == "mathhalfrot")
            {
                mathhalfrot = (float)value;
                return;
            }
            if (identifier == "_")
                return;
            if (identifier.StartsWith("S"))
            {
                funcstack[int.Parse(identifier.Substring(1))] = value;
                return;
            }
            if (globaldict.ContainsKey(identifier))
            {
                globaldict[identifier] = value;
                return;
            }
            globaldict.Add(identifier, value);
        }
        private object getvar(string identifier)
        {
            if (identifier == "NUT")
                return null;
            if (identifier == "T")
                return true;
            if (identifier == "F")
                return false;
            if (identifier == "π")
                return (float)Math.PI;
            if (identifier == "mathfalfrot")
                return mathhalfrot;
            if (identifier == "_")
            {
                doerror("cannot get value from discard!");
                return null;
            }
            char firstchar = identifier[0];
            if (firstchar == '\"')
                return identifier.Substring(1);
            if (firstchar == '#')
                return float.Parse(identifier.Substring(1).Replace(',', '.'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
            if (firstchar == '!')
                return !(bool)getvar(identifier.Substring(1));
            if (firstchar == '-')
                return -(float)getvar(identifier.Substring(1));
            if (firstchar == '+')
                return Math.Abs((float)getvar(identifier.Substring(1)));
            if (identifier == "B")
                return blankvar;
            if (identifier.StartsWith("S"))
                return funcstack[int.Parse(identifier.Substring(1))];
            if (globaldict.ContainsKey(identifier))
                return globaldict[identifier];
            return null;
        }
        private void doerror(string err, int linenumber = -8008135)
        {
            if (linenumber == -8008135)
                linenumber = counter;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(linenum[linenumber] + " (" + linenumber + "): " + err);
            Console.ResetColor();
        }
        static void debugline(string text)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
