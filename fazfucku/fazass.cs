using System;
using System.Collections.Generic;
using System.IO;

namespace fazfucku
{
    internal class fazassrunner
    {
        public string[] lines;
        private Dictionary<string, object> globaldict = new Dictionary<string, object>();
        private int counter = 0;
        //static Dictionary<string, int> pointers = new Dictionary<string, int>();
        //static bool isinhook = false;
        private List<object> funcstack = new List<object>();
        private bool blankvar;
        private bool debug_runline;
        public void run()
        {
            while(counter != -1)
            {
                if (debug_runline)
                    debugline(lines[counter]);
                counter = runline(lines[counter].Split('¦'));
            }
        }
        private int runline(string[] line)
        {
            switch(line[0])
            {
                case "print":
                    Console.WriteLine(getvar(line[1]));
                    break;
                case "end":
                    return -1;
                case "goto":
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
                case "comp":
                    {
                        bool result = false;
                        switch (line[3])
                        {
                            case "=":
                                result = getvar(line[2]) == getvar(line[4]);
                                break;
                            case "!":
                                result = getvar(line[2]) != getvar(line[4]);
                                break;
                            case "<":
                                result = (float)getvar(line[2]) < (float)getvar(line[4]);
                                break;
                            case ">":
                                result = (float)getvar(line[2]) > (float)getvar(line[4]);
                                break;
                            case "<=":
                                result = (float)getvar(line[2]) <= (float)getvar(line[4]);
                                break;
                            case "=>":
                                result = (float)getvar(line[2]) >= (float)getvar(line[4]);
                                break;
                        }
                        setvar(line[1], result);
                    }
                    break;
                case "m1": //math 1
                    {
                        float result = 0f;
                        float num = (float)getvar(line[3]);
                        switch (line[2])
                        {
                            case "a"://abs
                                if (num < 0f)
                                    result = -num;
                                else
                                    result = num;
                                break;
                            case "s"://sign
                                if (num == 0f)
                                    result = 0f;
                                else if (num > 0f)
                                    result = 1f;
                                else if (num < 0f)
                                    result = -1f;
                                break;
                            case "sq": //sqrt
                                result = (float)Math.Sqrt(num);
                                break;
                        }
                        setvar(line[1], result);
                    }
                    break;
                case "m2": //math 2
                    {
                        float result = 0f;
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
                            case "*":
                                result = num1 * num2;
                                break;
                            case "/":
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
                            case "si": //sin
                                result = (float)Math.Sin(num1 * Math.PI / num2);
                                break;
                            case "co": //cos
                                result = (float)Math.Cos(num1 * Math.PI / num2);
                                break;
                            case "ta": //tan
                                result = (float)Math.Tan(num1 * Math.PI / num2);
                                break;
                            case "lg": //log
                                result = (float)Math.Log(num1, num2);
                                break;
                        }
                        setvar(line[1], result);
                    }
                    break;
                case "logc": //logic
                    {
                        bool result = false;
                        bool bitch = (bool)getvar(line[3]);
                        bool boobies = (bool)getvar(line[4]);
                        switch (line[2])
                        {
                            case "&":
                                result = bitch && boobies;
                                break;
                            case "|":
                                result = bitch || boobies;
                                break;
                            case "^":
                                result = bitch ^ boobies;
                                break;
                        }
                        setvar(line[1], result);
                    }
                    break;
                case "not":
                    setvar(line[1], !(bool)getvar(line[2]));
                    break;
                case "set":
                    setvar(line[1],getvar(line[2]));
                    break;
                case "undef":
                    if (globaldict.ContainsKey(line[1]))
                        globaldict.Remove(line[1]);
                    break;
                case "fstk_a": //funcstack_add
                    funcstack.Insert(0, getvar(line[1]));
                    break;
                case "fstk_d": //funcstack_discard
                    funcstack.RemoveRange(0, int.Parse(line[1]));
                    break;
                case "fstk_pg": //funcstack_popgoto
                    {
                        int jumbojosh = (int)funcstack[0];
                        funcstack.RemoveAt(0);
                        return jumbojosh;
                    }
                case "conv": //convert
                    {
                        object jumbojosh = getvar(line[2]);
                        Type typojosh = jumbojosh.GetType();
                        switch (line[3])
                        {
                            default:
                                doerror(line[3] + " is an invalid type!");
                                break;
                            case "Tbool":
                                if (typojosh == typeof(float))
                                    setvar(line[1], (float)jumbojosh >= 0f);
                                else if (typojosh == typeof(string))
                                    setvar(line[1], (string)jumbojosh != string.Empty);
                                else
                                    setvar(line[1], (bool)jumbojosh);
                                break;
                            case "Tnum":
                                if (typojosh == typeof(bool))
                                    setvar(line[1], (bool)jumbojosh ? 1f : 0f);
                                else if (typojosh == typeof(string))
                                    setvar(line[1], getvar("N" + jumbojosh));
                                else
                                    setvar(line[1], (float)jumbojosh);
                                break;
                            case "Tstr":
                                if (typojosh == typeof(bool))
                                    setvar(line[1], (bool)jumbojosh ? "true" : "false");
                                else if (typojosh == typeof(float))
                                    setvar(line[1], jumbojosh.ToString().Replace(',', '.'));
                                else
                                    setvar(line[1], jumbojosh.ToString());
                                break;
                        }
                    }
                    break;

                //COMSOEL?!
                case "cons_getline":
                    setvar(line[1], Console.ReadLine());
                    break;
                case "cons_checkesc":
                    if (Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Escape)
                        return -1;
                    break;
                case "blah":
                    Console.WriteLine(line[1]);
                    break;
            }
            return counter + 1;
        }
        private void setvar(string identifier, object value)
        {
            if (identifier == "true" || identifier == "false" || identifier.StartsWith("\"") || identifier.StartsWith("N"))
            {
                doerror(identifier + " is a constant value!");
                return;
            }
            if (identifier == "B")
            {
                blankvar = (bool)value;
                return;
            }
            if (identifier.StartsWith("‡"))
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
            if (identifier == "true")
                return true;
            if (identifier == "false")
                return false;
            if (identifier.StartsWith("\""))
                return identifier.Substring(1);
            if (identifier.StartsWith("N"))
                return float.Parse(identifier.Replace(',', '.'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
            if (identifier == "B")
                return blankvar;
            if (identifier.StartsWith("‡"))
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
            Console.WriteLine(linenumber + ": " + err);
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
