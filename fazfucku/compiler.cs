using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static fazfucku.marker;

namespace fazfucku
{
    internal class fazfuckucompiler
    {
        public string[] inputfile;
        List<lineinfo> lines;
        public bool includemarkers;
        public int prematureoutput = -1;
        public bool nofunctions;
        public string compile()
        {
            try
            {
                lines = new List<lineinfo>();
                Console.WriteLine("compiler: initial lexer start");
                initiallexer();
                if (prematureoutput == 1)
                    return outputfile(false);

                Console.WriteLine("compiler: line detection start");
                linedetector();
                if (prematureoutput == 2)
                    return outputfile(true);

                Console.WriteLine("compiler: main conversion start");
                mainconversion();
                if (prematureoutput == 3)
                    return outputfile(true);

                if (nofunctions)
                    Console.WriteLine("compiler: skipping function stuff lol");
                else
                {
                    Console.WriteLine("compiler: function stuff start");
                    functionstuff();
                    if (prematureoutput == 4)
                        return outputfile(true);
                }

                Console.WriteLine("compiler: convert control flow start");
                gototime();
                if (prematureoutput == 5)
                    return outputfile(true);

                Console.WriteLine("compiler: remove old control start");
                removeoldcontrol();
                if (prematureoutput == 6)
                    return outputfile(true);

                Console.WriteLine("compiler: get unique marker locations start");
                getuniquemarkerlocations();
                if (prematureoutput == 7)
                    return outputfile(true);

                Console.WriteLine("compiler: convert from dynamic start");
                convertfromdynamic();
                if (prematureoutput == 8)
                    return outputfile(true);

                Console.WriteLine("compiler: replace longer function names");
                makeshort();
                if (prematureoutput == 9)
                    return outputfile(true);

                Console.WriteLine("compiler: output file");
                return outputfile(includemarkers);
            }
            catch (Exception e)
            {
                Console.WriteLine("compiler: " + e.ToString());
                return string.Empty;
            }
        }
        private void doerror(string err)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("compiler: " + err);
            Console.ResetColor();
            Program.funnywindowssound(Program.soundeffect.error);
        }
        string[] blockfunctions = new string[]
        {
            "if",
            "elif",
            "else",
            "while",
            "otherwise",
            "section",
            "connection",
            "for",
        };
        //initial lexer
        #region initial lexer
        lineinfo workingline;
        string workingtoken;
        int currentfileline;
        lextype curlextype;
        void poptoken()
        {
            if (workingtoken != string.Empty)
            {
                workingline.pieces.Add(workingtoken);
                workingtoken = string.Empty;
            }
        }
        void popline()
        {
            if (workingtoken != string.Empty)
                poptoken();
            if (workingline.pieces.Count != 0)
            {
                lines.Add(workingline);
                workingline = new lineinfo(-1);
            }
        }
        enum lextype { defauld, stringe, logicalic }
        void initiallexer()
        {
            workingline = new lineinfo(0);
            workingtoken = string.Empty;
            curlextype = lextype.defauld;
            char prevchar = '\0';
            char curchar = '\0';
            string dafile = string.Join("\n", inputfile);
            for (int i = 0; i < dafile.Length; i++)
            {
                curchar = dafile[i];
                switch (curlextype)
                {
                    case lextype.defauld:
                        if (curchar == '\n')
                            popline();
                        else if (char.IsWhiteSpace(curchar))
                        {
                            if (workingline.pieces.Count == 0 && (workingtoken == "func" || workingtoken == "inlinefunc" || workingtoken == "hook"))
                            {
                                workingtoken += " ";
                                poptoken();
                            }
                        }
                        else if (curchar == '\"' || curchar == '«')
                        {
                            workingtoken = "\"";
                            curlextype = lextype.stringe;
                        }
                        else if (curchar == '{' || curchar == '}')
                        {
                            popline();
                            workingtoken = curchar.ToString();
                            popline();
                        }
                        else if (curchar == ';')
                        {
                            while (char.IsWhiteSpace(curchar) && curchar != '\n' && i < dafile.Length)
                            {
                                i++;
                                curchar = dafile[i];
                            }
                            if (!(curchar == '\n' || char.IsUpper(curchar)))
                            {
                                popline();
                                i--; //process this char again
                            }
                        }
                        else if (char.IsUpper(curchar))
                        {
                            while (curchar != '\n' && i < dafile.Length)
                            {
                                i++;
                                curchar = dafile[i];
                            }
                            popline();
                        }
                        else if (curchar == '⟨')
                        {
                            poptoken();
                            workingtoken = "⟨";
                            poptoken();
                            curlextype = lextype.logicalic;
                        }
                        else if (curchar == '|')
                        {
                            poptoken();
                        }
                        else if (curchar == '(' || curchar == ')' || curchar == ',' || curchar == '=' || curchar == '[' || curchar == ']' || curchar == '¶')
                        {
                            poptoken();
                            workingtoken = curchar.ToString();
                            poptoken();
                        }
                        else
                            workingtoken += curchar;
                        break;
                    case lextype.stringe:
                        if (prevchar == '\\')
                        {
                            if (curchar == 'n')
                                workingtoken += '\n';
                            else if (curchar == 'p')
                                workingtoken += '|';
                            else if (curchar == 'b')
                                workingtoken += '\\';
                            else
                                workingtoken += curchar;
                        }
                        else if (curchar == '\\') { } //NOTHING HAPPENS
                        else if (curchar == '\"' || curchar == '»')
                        {
                            poptoken();
                            curlextype = lextype.defauld;
                        }
                        else
                            workingtoken += curchar;
                        break;
                    case lextype.logicalic:
                        if (curchar == '⟩')
                        {
                            poptoken();
                            workingtoken = "⟩";
                            poptoken();
                            curlextype = lextype.defauld;
                        }
                        else
                            workingtoken += curchar;
                        break;
                }
                if (curchar == '\n')
                {
                    currentfileline++;
                    if (workingline.originallineid == -1)
                        workingline.originallineid = currentfileline;
                }
                prevchar = curchar;
            }
            popline();
        }
        #endregion
        Dictionary<string, bool> customfunctions = new Dictionary<string, bool>(); //the bool is if it's inline
        //line detection
        #region line detection
        void linedetector()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                linetype ligmaballs = getlinetype(lines[i].pieces);
                lines[i].type = ligmaballs;
                if (ligmaballs == linetype.broken)
                    doerror("cannot get the type of line for " + lines[i].originallineid + " (" + i + ")!");
                else if (ligmaballs == linetype.blockfunc)
                {
                    if (lines[i + 1].pieces[0] != "{")
                    {
                        lines.Insert(i + 1, new lineinfo(lines[i].originallineid, linetype.standalone, new List<string> { "{" }));
                        lines.Insert(i + 3, new lineinfo(lines[i].originallineid, linetype.standalone, new List<string> { "}" }));
                    }
                }
                else if (ligmaballs == linetype.definationfunc)
                {
                    customfunctions.Add(lines[i].pieces[1], lines[i].pieces[0] == "inlinefunc");
                    adduniquemarker(i, lines[i].pieces[1]);
                }
                //Console.WriteLine(ligmaballs.ToString());
            }
        }
        linetype getlinetype(List<string> daline)
        {
            if (daline.Count == 0)
                return linetype.theresNOTHING;
            if (daline.Count == 1)
            {
                if (blockfunctions.Contains(daline[0]))
                    return linetype.blockfunc;
                return linetype.standalone;
            }
            if (daline[0] == "(")
                return linetype.mathdirect;
            if (daline[0] == "⟨")
                return linetype.booldirect;
            if (daline[0] == "func " || daline[0] == "inlinefunc ")
                return linetype.definationfunc;
            if (daline[0] == "¶")
                return linetype.markerlabel;
            if (daline[1] == ",")
                return getlinetype(daline.GetRange(2, daline.Count - 2));
            if (daline[1] == "=")
            {
                if (daline.Count < 3)
                    return linetype.broken;
                if (daline.Count == 3)
                    return linetype.set;
                if (daline[2] == "(")
                    return linetype.math;
                if (daline[2] == "⟨")
                    return linetype.boel;
                if (daline[3] == "(" || daline[3] == "[")
                    return linetype.functionset;
                if (daline[3] == "⟨")
                    return linetype.boolfunctionnargumentset;
                return linetype.broken;
            }
            if (daline[1] == "(" || daline[1] == "[")
                return linetype.function;
            if (daline[1] == "⟨")
            {
                if (blockfunctions.Contains(daline[0]))
                    return linetype.blockfunc;
                if (daline.Count > 4)
                    return linetype.boolfunctionwarg;
                return linetype.boolfunction;
            }
            return linetype.broken;
        }
        #endregion
        Dictionary<string, int> uniquemarkers = new Dictionary<string, int>();
        void adduniquemarker(int line, string nae)
        {
            lines[line].markers.Add(new marker(markertype.unique, uniquemarkers.Count));
            uniquemarkers.Add(nae, -1);
        }
        //main fazass conversion
        #region main fazass conversion
        void mainconversion()
        {
            Predicate<string> commaer = piec => piec == ","; //checks if it's a comma i think
            for (int i = 0; i < lines.Count; i++)
            {
                switch (lines[i].type)
                {
                    case linetype.theresNOTHING:
                        removeline(i);
                        i--;
                        break;
                    case linetype.function:
                        if (customfunctions.ContainsKey(lines[i].pieces[0]))
                        {
                            //adds goto
                            lines.Insert(i+1, new lineinfo(lines[i].originallineid, linetype.fazfuck, new List<string> { "goto", "@u" + lines[i].pieces[0] }));
                            //the jank of varialing
                            lines[i].pieces.RemoveAll(commaer);
                            List<string> parameters = getpartsbetween(lines[i].pieces, "(", ")");
                            List<string> outvariables = getpartsbetween(lines[i].pieces, "[", "]");
                            if (outvariables.Count != 0)
                            {
                                //add pop line
                                List<string> popperline = new List<string>();
                                popperline.AddRange(outvariables);
                                popperline.Add("fstk_pop"); //is the first part when reversed
                                popperline.Reverse();
                                lines.Insert(i+2, new lineinfo(lines[i].originallineid, linetype.fazfuck, popperline)); //add to afterwards
                            }
                            //convert to funcstack add line
                            lines[i].pieces = new List<string> { "fstk_add" };
                            lines[i].pieces.AddRange(outvariables);
                            lines[i].pieces.Add("$@+2");
                            lines[i].pieces.AddRange(parameters);
                        }
                        else
                        {
                            lines[i].pieces.RemoveAll(commaer);
                            lines[i].pieces.Remove("(");
                            lines[i].pieces.Remove(")");
                            lines[i].pieces.Remove("[");
                            lines[i].pieces.Remove("]");
                        }
                        //lines[i].type = linetype.fazfuck;
                        break;
                    case linetype.functionset:
                        {
                            lines[i].pieces.RemoveAll(commaer);
                            int variablecount = lines[i].pieces.IndexOf("=");
                            List<string> setvar = lines[i].pieces.GetRange(0, variablecount);
                            lines[i].pieces.RemoveRange(0, variablecount + 1);
                            if (customfunctions.ContainsKey(lines[i].pieces[0]))
                            {
                                //adds goto
                                lines.Insert(i + 1, new lineinfo(lines[i].originallineid, linetype.fazfuck, new List<string> { "goto", "@u" + lines[i].pieces[0] }));
                                //the jank of varialing
                                lines[i].pieces.RemoveAll(commaer);
                                List<string> parameters = getpartsbetween(lines[i].pieces, "(", ")");
                                List<string> outvariables = getpartsbetween(lines[i].pieces, "[", "]");
                                //add pop line (not in out because a set always should have an out)
                                List<string> popperline = new List<string>();
                                popperline.AddRange(setvar);
                                popperline.AddRange(outvariables);
                                popperline.Add("fstk_pop");
                                popperline.Reverse();
                                lines.Insert(i + 2, new lineinfo(lines[i].originallineid, linetype.fazfuck, popperline));
                                //convert to funcstack add line
                                lines[i].pieces = new List<string> { "fstk_add" };
                                for (int j = 0; j < setvar.Count; j++)
                                {
                                    lines[i].pieces.Add("NUT");
                                }
                                lines[i].pieces.AddRange(outvariables);
                                lines[i].pieces.Add("$@+2");
                                lines[i].pieces.AddRange(parameters);
                            }
                            else
                            {
                                lines[i].pieces.Remove("(");
                                lines[i].pieces.Remove(")");
                                lines[i].pieces.Remove("[");
                                lines[i].pieces.Remove("]");
                                lines[i].pieces.InsertRange(1, setvar); //add back setvar
                            }
                            //lines[i].type = linetype.fazfuck;
                        }
                        break;
                    case linetype.math:
                        {
                            string setvar = lines[i].pieces[0]; //save variable
                            lines[i].pieces.RemoveRange(0, 2); //removes the variable and =
                            lines[i].pieces.Remove("(");
                            lines[i].pieces.Remove(")");
                            parsemath(string.Join("", lines[i].pieces), out string num1, out string oper, out string num2);
                            lines[i].pieces = new List<string> { "math", setvar, oper, num1, num2 };
                            //lines[i].type = linetype.fazfuck;
                        }
                        break;
                    case linetype.mathdirect:
                        {
                            lines[i].pieces.Remove("(");
                            lines[i].pieces.Remove(")");
                            parsemath(string.Join("", lines[i].pieces), out string num1, out string oper, out string num2);
                            lines[i].pieces = new List<string> { "math", num1, oper, num1, num2 };
                            //lines[i].type = linetype.fazfuck;
                        }
                        break;
                    case linetype.set:
                        {
                            lines[i].pieces = new List<string> { "set", lines[i].pieces[0], lines[i].pieces[2] };
                            //lines[i].type = linetype.fazfuck;
                        }
                        break;
                    case linetype.booldirect:
                        lines[i].pieces = parsecond(lines[i].pieces[1], "");
                        //lines[i].type = linetype.fazfuck;
                        break;
                    case linetype.boel:
                        lines[i].pieces = parsecond(lines[i].pieces[3], lines[i].pieces[0]);
                        //lines[i].type = linetype.fazfuck;
                        break;
                    case linetype.boolfunction:
                        lines.Insert(i, new lineinfo(lines[i].originallineid, linetype.fazfuck, parsecond(lines[i].pieces[2], "B")));
                        i++;
                        lines[i].pieces.RemoveRange(1, lines[i].pieces.Count - 1);
                        break;
                    case linetype.blockfunc:
                        switch (lines[i].pieces[0])
                        {
                            case "if":
                                lines.Insert(i + 2, new lineinfo(lines[i].originallineid, linetype.fazfuck, parsecond(lines[i].pieces[2], "B")));
                                lines.Insert(i + 3, new lineinfo(lines[i].originallineid, linetype.fazfuck, new List<string> { "next_unless", "0" }));
                                lines[i].pieces = new List<string> { "section" };
                                break;
                            case "elif":
                                lines.Insert(i + 2, new lineinfo(lines[i].originallineid, linetype.fazfuck, parsecond(lines[i].pieces[2], "B")));
                                lines.Insert(i + 3, new lineinfo(lines[i].originallineid, linetype.fazfuck, new List<string> { "next_unless", "0" }));
                                lines[i].pieces = new List<string> { "connection" };
                                lines.Insert(i - 1, new lineinfo(lines[i].originallineid, linetype.standalone, new List<string> { "end" }));
                                i++;
                                break;
                            case "else":
                                lines[i].pieces = new List<string> { "connection" };
                                lines.Insert(i - 1, new lineinfo(lines[i].originallineid, linetype.standalone, new List<string> { "end" }));
                                i++;
                                break;
                            case "while":
                                List<string> conditioncheck = parsecond(lines[i].pieces[2], "B");
                                lines.Insert(i + 2, new lineinfo(lines[i].originallineid, linetype.fazfuck, conditioncheck));
                                lines.Insert(i + 3, new lineinfo(lines[i].originallineid, linetype.fazfuck, new List<string> { "end_unless", "0" }));
                                lines.Insert(skipindentation(i + 1, 0), new lineinfo(lines[i].originallineid, linetype.standalone, new List<string> { "restart" }));
                                lines[i].pieces = new List<string> { "section" };
                                /*if (lines[skap+1].pieces[0] == "otherwise")
                                {
                                    lines.Insert(i, new lineinfo(origid, linetype.function, conditioncheck));
                                    lines.Insert(i + 3, new lineinfo(origid, linetype.function, new List<string> { "next_unless", "-1" })); //the next flag would be on the otherwise connection; add offset of minus one since we're not in that indent yet
                                }*/
                                break;
                            case "otherwise":
                                doerror("otherwise not supported yet ):");
                                lines[i].pieces = new List<string> { "connection" };
                                break;
                        }
                        break;
                    case linetype.definationfunc:
                        lines[i].pieces.RemoveAll(commaer);
                        break;
                    case linetype.markerlabel:
                        adduniquemarker(i, lines[i].pieces[1]);
                        removeline(i);
                        i--;
                        break;
                }
            }
        }
        void parsemath(string toparse, out string num1, out string oper, out string num2)
        {
            List<string> peepees = new List<string>();
            string curp = "";
            for (int i = 0; i < toparse.Length; i++)
            {
                char curchar = toparse[i];
                if (curchar == '+' || curchar == '-' || curchar == '×' || curchar == '÷' || curchar == '%' || curchar == '^' || curchar == '_')
                {
                    peepees.Add(curp);
                    peepees.Add(curchar.ToString());
                    curp = "";
                }
                else if (!char.IsWhiteSpace(curchar))
                    curp += curchar;
            }
            peepees.Add(curp);
            if (peepees[0] == "-")
            {
                if (peepees[1] == "+")
                {
                    num1 = "-+" + peepees[2];
                    peepees.RemoveRange(0, 3);
                }
                else
                {
                    num1 = "-" + peepees[1];
                    peepees.RemoveRange(0, 2);
                }
            }
            else if (peepees[0] == "+")
            {
                num1 = "+" + peepees[1];
                peepees.RemoveRange(0, 2);
            }
            else
            {
                num1 = peepees[0];
                peepees.RemoveAt(0);
            }
            oper = peepees[0];
            if (peepees[1] == "-")
            {
                if (peepees[2] == "+")
                    num2 = "-+" + peepees[3];
                else
                    num2 = "-" + peepees[2];
            }
            else if (peepees[1] == "+")
                num2 = "+" + peepees[2];
            else
                num2 = peepees[1];
        }
        public Dictionary<string, string> specialcomparisons = new Dictionary<string, string>();
        List<string> parsecond(string toparse, string outputvar)
        {
            if (toparse.Contains(" "))
            {
                string[] splitty = toparse.Split(' ');
                bool has = false;
                foreach (string jumbojosh in specialcomparisons.Keys)
                {
                    if (splitty.Contains(jumbojosh))
                    {
                        has = true;
                        break;
                    }
                }
                if (has)
                {
                    List<string> daline = new List<string> { "comp", outputvar, "", "", "" };
                    int piecey = 2;
                    foreach (string jumbojosh in splitty)
                    {
                        if (specialcomparisons.ContainsKey(jumbojosh))
                        {
                            daline[3] = specialcomparisons[jumbojosh];
                            piecey = 4;
                        }
                        else
                            daline[piecey] += jumbojosh;
                    }
                    if (daline[2].StartsWith("\"") || daline[2].StartsWith("«"))
                        daline[2] = "\"" + daline[2].Substring(1, daline[2].Length - 2);
                    if (daline[4].StartsWith("\"") || daline[4].StartsWith("«"))
                        daline[4] = "\"" + daline[4].Substring(1, daline[4].Length - 2);
                    return daline;
                }
            }
            List<string> isalisteventhebestchoiceforthis = new List<string>();
            string workne = "";
            for (int i = 0; i < toparse.Length; i++)
            {
                char curchar = toparse[i];
                if (curchar == '=' || curchar == '≠' || curchar == '<' || curchar == '>' || curchar == '≤' || curchar == '≥' || curchar == '&' || curchar == '|')
                {
                    isalisteventhebestchoiceforthis.Add(workne);
                    isalisteventhebestchoiceforthis.Add(curchar.ToString());
                    workne = "";
                }
                else if (!char.IsWhiteSpace(curchar))
                    workne += curchar;
            }
            isalisteventhebestchoiceforthis.Add(workne);

            if (outputvar == string.Empty)
                outputvar = isalisteventhebestchoiceforthis[0];
            if (isalisteventhebestchoiceforthis.Count == 1)
                return new List<string> { "set", outputvar, isalisteventhebestchoiceforthis[0] };
            else
            {
                if (isalisteventhebestchoiceforthis[0].StartsWith("\"") || isalisteventhebestchoiceforthis[0].StartsWith("«"))
                    isalisteventhebestchoiceforthis[0] = "\"" + isalisteventhebestchoiceforthis[0].Substring(1, isalisteventhebestchoiceforthis[0].Length - 2);
                if (isalisteventhebestchoiceforthis[2].StartsWith("\"") || isalisteventhebestchoiceforthis[2].StartsWith("«"))
                    isalisteventhebestchoiceforthis[2] = "\"" + isalisteventhebestchoiceforthis[2].Substring(1, isalisteventhebestchoiceforthis[2].Length - 2);
                return new List<string> { "comp", outputvar, isalisteventhebestchoiceforthis[0], isalisteventhebestchoiceforthis[1], isalisteventhebestchoiceforthis[2] };
            }
        }
        #endregion
        List<string> getpartsbetween(List<string> parts, string startpiece, string endpiece)
        {
            if (!parts.Contains(startpiece) || !parts.Contains(endpiece))
                return new List<string>();
            List<string> aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa = new List<string>();
            int startindex = parts.IndexOf(startpiece) + 1;
            aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa = parts.GetRange(startindex, parts.IndexOf(endpiece) - startindex);
            return aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa;
        }
        //function stuff
        #region function stuff
        void functionstuff()
        {
            List<string> variablestack = new List<string>();
            int discardamount = 0;
            int indentation = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].type == linetype.definationfunc)
                {
                    variablestack.AddRange(getpartsbetween(lines[i].pieces, "[", "]"));
                    variablestack.Add("THIS IS THE PLACEHOLDER FOR HWEERE THE RETURN ID GOES");
                    List<string> paramaters = getpartsbetween(lines[i].pieces, "(", ")");
                    discardamount = paramaters.Count;
                    variablestack.AddRange(paramaters);
                    variablestack.Reverse();
                }
                else if (lines[i].pieces[0] == "return")
                {
                    lines[i].pieces = new List<string> { "fstk_discard", "L" + discardamount.ToString() };
                    lines.Insert(i + 1, new lineinfo(lines[i].originallineid, linetype.fazfuck, new List<string> { "fstk_popgoto" }));
                    i++;
                }
                else if (lines[i].pieces[0] == "{")
                    indentation++;
                else if (lines[i].pieces[0] == "}")
                {
                    indentation--;
                    if (indentation == 0) //we have exit! function!
                    {
                        variablestack.Clear();
                        discardamount = 0;
                    }
                }
                else
                {
                    for (int j = 0; j < lines[i].pieces.Count; j++)
                    {
                        int pindex = variablestack.IndexOf(lines[i].pieces[j]);
                        if (pindex != -1)
                            lines[i].pieces[j] = "S" + pindex;
                    }
                }
            }
        }
        #endregion
        //control flow conversion
        #region this part makes me want to kill myself (:
        public Dictionary<string, string[]> controlgotoes = new Dictionary<string, string[]>();
        void gototime()
        {
            int indentation = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].pieces[0] == "section")
                {
                    lines[i].markers.Add(new marker(markertype.start, indentation));
                    bool gomytoeloe = true;
                    int checkers = i;
                    while (gomytoeloe)
                    {
                        checkers = skipindentation(checkers + 1, 0) + 1;
                        if (lines[checkers].pieces[0] == "connection")
                            lines[checkers].markers.Add(new marker(markertype.next, indentation));
                        else
                            gomytoeloe = false;
                    }
                    lines[checkers].markers.Add(new marker(markertype.end, indentation));
                }
                else if (lines[i].pieces[0] == "{")
                    indentation++;
                else if (lines[i].pieces[0] == "}")
                    indentation--;
                else if (controlgotoes.ContainsKey(lines[i].pieces[0]))
                {
                    string[] gotodef = controlgotoes[lines[i].pieces[0]];
                    if (gotodef[1] == "@u")
                    {
                        lines[i].pieces = new List<string> { gotodef[0], "@u" + lines[i].pieces[1] };
                    }
                    else
                    {
                        int offset = -1;
                        if (lines[i].pieces.Count > 1)
                            offset -= int.Parse(lines[i].pieces[1]);
                        lines[i].pieces = new List<string> { gotodef[0], gotodef[1] + (indentation + offset) };
                    }
                }
            }
        }
        int skipindentation(int start, int offset)
        {
            int curindent = 0;
            for (int i = start; i < lines.Count; i++)
            {
                List<string> peepees = lines[i].pieces;
                if (peepees[0] == "{")
                    curindent++;
                else if (peepees[0] == "}")
                    curindent--;
                if (curindent == offset)
                    return i;
            }
            doerror("cannot skip indentation with an offset of " + offset + " starting at" + lines[start].originallineid + " (" + start + ")!");
            return start;
        }
        #endregion
        //remove old control
        #region remove old control
        void removeoldcontrol()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].type == linetype.definationfunc || lines[i].type == linetype.blockfunc || lines[i].pieces[0] == "{" || lines[i].pieces[0] == "}")
                {
                    removeline(i);
                    i--;
                }
            }
        }
        #endregion
        void removeline(int index)
        {
            if (lines[index].markers.Count > 0)
            {
                List<marker> markys = lines[index].markers;
                lines.RemoveAt(index);
                lines[index].markers.AddRange(markys);
            }
            else
                lines.RemoveAt(index);
        }
        //get unique marker locations
        #region get unique marker locations
        void getuniquemarkerlocations()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].markers.Count > 0)
                {
                    foreach (marker da_mark in lines[i].markers)
                    {
                        if (da_mark.tipe == markertype.unique)
                        {
                            string dictkey = uniquemarkers.ElementAt(da_mark.markerid).Key; //get the key at the index like a normal list
                            uniquemarkers[dictkey] = i;
                        }
                    }
                }
            }
        }
        #endregion
        //convert from dynamic
        #region convert from dynamic
        void convertfromdynamic()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = 0; j < lines[i].pieces.Count; j++)
                {
                    string token = lines[i].pieces[j];
                    if (token == "true")
                        lines[i].pieces[j] = "T";
                    else if (token == "false")
                        lines[i].pieces[j] = "F";
                    else if (token.StartsWith("@") || token.StartsWith("$@"))
                    {
                        bool numappendix = false;
                        if (token.StartsWith("$"))
                        {
                            numappendix = true;
                            token = token.Substring(1);
                        }
                        if (token.StartsWith("@+"))
                        {
                            int offset = int.Parse(token.Substring(2));
                            token = (i + offset).ToString();
                        }
                        else if (token.StartsWith("@-"))
                        {
                            int offset = int.Parse(token.Substring(2));
                            token = (i - offset).ToString();
                        }
                        else if (token.StartsWith("@start"))
                        {
                            int searchid = int.Parse(token.Substring(6));
                            token = searchmarker(i, markertype.start, searchid, -1).ToString();
                        }
                        else if (token.StartsWith("@next"))
                        {
                            int searchid = int.Parse(token.Substring(5));
                            token = searchmarker(i, markertype.next, searchid, 1).ToString();
                        }
                        else if (token.StartsWith("@end"))
                        {
                            int searchid = int.Parse(token.Substring(4));
                            token = searchmarker(i, markertype.end, searchid, 1).ToString();
                        }
                        else if (token.StartsWith("@u"))
                        {
                            int uniquelocation = uniquemarkers[token.Substring(2)];
                            token = uniquelocation.ToString();
                        }
                        if (numappendix)
                            token = "I" + token;
                        lines[i].pieces[j] = token;
                    }
                    else if (token.StartsWith("-") || token.StartsWith("+") || char.IsDigit(token[0]))
                    {
                        if (float.TryParse(token.Replace(',', '.'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _))
                            lines[i].pieces[j] = "#" + token.Replace(',', '.');
                    }
                    else if (token.StartsWith("L"))
                    {
                        lines[i].pieces[j] = token.Substring(1);
                    }
                    else if (token.StartsWith("\""))
                    {
                        lines[i].pieces[j] = token.Replace("\n", "\\n").Replace("|", "\\p").Replace("\\", "\\b");
                    }
                }
            }
        }
        int searchmarker(int startline, markertype tipee, int idee, int direction)
        {
            int curline = startline;
            while(curline != 0 && curline != lines.Count-1)
            {
                curline += direction;
                if (lines[curline].hasmarker(tipee, idee))
                    return curline;
                else if (tipee == markertype.next && lines[curline].hasmarker(markertype.end, idee))
                    return curline;
            }
            Console.WriteLine("compiler: sreahcing markier error!!! startline:" + startline + " markertype:" + tipee + " id:" + idee + " direcc:" + direction);
            return startline;
        }
        #endregion
        //replace longer function names
        #region he shortening
        public Dictionary<string, string> shorthand = new Dictionary<string, string>();
        public Dictionary<string, string[]> mathfunc = new Dictionary<string, string[]>();
        void makeshort()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                string functio = lines[i].pieces[0];
                if (functio == "math")
                    lines[i].pieces[0] = "m" + (lines[i].pieces.Count - 3);
                else if (mathfunc.ContainsKey(functio))
                {
                    lines[i].pieces[0] = mathfunc[functio][0];
                    lines[i].pieces.Insert(2, mathfunc[functio][1]);
                }
                else if (shorthand.ContainsKey(functio))
                    lines[i].pieces[0] = shorthand[functio];
            }
        }
        #endregion
        //output
        string outputfile(bool addmarkers)
        {
            StringBuilder thebuilder = new StringBuilder();
            foreach (lineinfo lijn in lines)
            {
                if (addmarkers && lijn.markers.Count > 0)
                    thebuilder.AppendLine(string.Join("|", lijn.pieces) + "<" + string.Join("<", lijn.markers) + "|" + lijn.originallineid);
                else
                    thebuilder.AppendLine(string.Join("|", lijn.pieces) + "|" + lijn.originallineid);
            }
            return thebuilder.ToString().TrimEnd('\n').TrimEnd('\r');
        }
    }
    enum linetype { math, mathdirect, boel, booldirect, function, functionset, standalone, boolfunction, set, definationfunc, broken, theresNOTHING, boolfunctionnargumentset, fazfuck, boolfunctionwarg, blockfunc, markerlabel }
    class lineinfo
    {
        public List<string> pieces = new List<string>();
        public List<marker> markers = new List<marker>();
        public int originallineid = -1;
        public linetype type = linetype.broken;
        public bool hasmarker(markertype tipee, int idee)
        {
            if (markers.Count == 0)
                return false;
            foreach (marker damarker in markers)
            {
                if (damarker.markerid == idee && damarker.tipe == tipee)
                    return true;
            }
            return false;
        }
        public lineinfo(int originalline)
        {
            originallineid = originalline;
        }
        public lineinfo(int originalline, linetype lintip, List<string> pcs)
        {
            originallineid = originalline;
            pieces = pcs;
            type = lintip;
        }
    }
    class marker
    {
        public enum markertype { start, next, end, unique }
        public markertype tipe;
        public int markerid;
        public marker(markertype tipee, int idee)
        {
            tipe = tipee;
            markerid = idee;
        }
        public override string ToString()
        {
            return tipe.ToString() + markerid.ToString();
        }
    }
}
