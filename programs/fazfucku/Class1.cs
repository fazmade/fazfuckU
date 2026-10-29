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
        public void run()
        {
            while(counter != -1)
            {
                counter = runline(lines[counter].Split('¦'));
            }
        }
        private int runline(string[] line)
        {
            switch(line[0])
            {
                case "blah":
                    Console.WriteLine(line[1]);
                    break;
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
                case "m":
                    {
                        float result = 0f;
                        switch (line[2])
                        {
                            case "a"://abs
                                result = (float)getvar(line[3]);
                                if (result < 0f)
                                    result = -result;
                                break;
                            case "s"://sign
                                result = (float)getvar(line[3]);
                                if (result == 0f)
                                    result = 0f;
                                else if (result > 0f)
                                    result = 1f;
                                else if (result < 0f)
                                    result = -1f;
                                break;
                            case "+":
                                result = (float)getvar(line[3]) + (float)getvar(line[4]);
                                break;
                            case "-":
                                result = (float)getvar(line[3]) - (float)getvar(line[4]);
                                break;
                            case "*":
                                result = (float)getvar(line[3]) * (float)getvar(line[4]);
                                break;
                            case "/":
                                result = (float)getvar(line[3]) / (float)getvar(line[4]);
                                break;
                            case "%":
                                result = (float)getvar(line[3]) % (float)getvar(line[4]);
                                break;
                        }
                        setvar(line[1], result);
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
            }
            return counter + 1;
        }
        private void setvar(string identifier, object value)
        {
            if (identifier == "true" || identifier == "false" || identifier.StartsWith("\""))
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
            if (char.IsNumber(identifier[0]))
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
    }
}
