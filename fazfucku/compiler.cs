using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static fazfucku.marker;

namespace fazfucku
{
    enum linetype { math, mathdirect, boel, booldirect, function, functionset, standalone, controlfunction, set, definationhookfunc, definationsection, broken, theresNOTHING, functionboolnargumentset, fazfuck, controlfunctionarged }
    internal class fazfuckucompiler
    {
        public string[] inputfile;
        List<lineinfo> lines;
        public bool includemarkers;
        public int prematureoutput = -1;
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
                    return outputfile(false);
                Console.WriteLine("compiler: main fazass conversion start");
                mainfazassconvert();
                if (prematureoutput == 3)
                    return outputfile(false);
                Console.WriteLine("compiler: convert control flow start");
                gototime();
                if (prematureoutput == 4)
                    return outputfile(true);
                Console.WriteLine("compiler: remove brackets start");
                removebrackets();
                if (prematureoutput == 5)
                    return outputfile(true);
                Console.WriteLine("compiler: convert from dynamic start");
                convertfromdynamic();
                if (prematureoutput == 6)
                    return outputfile(true);
                Console.WriteLine("compiler: replace longer function names");
                makeshort();
                if (prematureoutput == 7)
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
        }
        //initial lexer
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
            workingline = new lineinfo(-1);
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
                            if (workingline.pieces.Count == 0 && (workingtoken == "func" || workingtoken == "section" || workingtoken == "hook"))
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
                            while (char.IsWhiteSpace(curchar) && curchar != '\n')
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
                            while (curchar != '\n')
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
                        else if (curchar == '(' || curchar == ')' || curchar == ',' || curchar == '=' || curchar == '[' || curchar == ']')
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
        //line detection
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
        void linedetector()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                linetype ligmaballs = getlinetype(lines[i].pieces);
                lines[i].type = ligmaballs;
                if (ligmaballs == linetype.broken)
                    doerror("cannot get the type of line for " + lines[i].originallineid + " (" + i + ")!");
                else if (blockfunctions.Contains(lines[i].pieces[0]))
                {
                    if (lines[i+1].pieces[0] != "{")
                    {
                        lines.Insert(i + 1, new lineinfo(lines[i].originallineid, linetype.standalone, new List<string> { "{" }));
                        lines.Insert(i + 3, new lineinfo(lines[i].originallineid, linetype.standalone, new List<string> { "}" }));
                    }
                }
                //Console.WriteLine(ligmaballs.ToString());
            }
        }
        linetype getlinetype(List<string> daline)
        {
            if (daline.Count == 0)
                return linetype.theresNOTHING;
            if (daline.Count == 1)
                return linetype.standalone;
            if (daline[0] == "(")
                return linetype.mathdirect;
            if (daline[0] == "⟨")
                return linetype.booldirect;
            if (daline[0] == "func " || daline[0] == "hook ")
                return linetype.definationhookfunc;
            if (daline[0] == "section ")
                return linetype.definationsection;
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
                    return linetype.functionboolnargumentset;
                return linetype.broken;
            }
            if (daline[1] == "(" || daline[1] == "[")
                return linetype.function;
            if (daline[1] == "⟨")
            {
                if (daline.Count > 4)
                    return linetype.controlfunctionarged;
                return linetype.controlfunction;
            }
            return linetype.broken;
        }
        //main fazass conversion
        void mainfazassconvert()
        {
            Predicate<string> commaer = piec => piec == ","; //checks if it's a comma i think
            for (int i = 0; i < lines.Count; i++)
            {
                switch (lines[i].type)
                {
                    case linetype.theresNOTHING:
                        lines.RemoveAt(i);
                        i--;
                        break;
                    case linetype.function:
                        lines[i].pieces.Remove("(");
                        lines[i].pieces.Remove(")");
                        lines[i].pieces.Remove("[");
                        lines[i].pieces.Remove("]");
                        lines[i].pieces.RemoveAll(commaer);
                        //lines[i].type = linetype.fazfuck;
                        break;
                    case linetype.functionset:
                        {
                            lines[i].pieces.RemoveAll(commaer);
                            int variablecount = lines[i].pieces.IndexOf("=");
                            List<string> setvar = lines[i].pieces.GetRange(0, variablecount);
                            lines[i].pieces.RemoveRange(0, variablecount + 1);
                            lines[i].pieces.Remove("(");
                            lines[i].pieces.Remove(")");
                            lines[i].pieces.Remove("[");
                            lines[i].pieces.Remove("]");
                            lines[i].pieces.InsertRange(1, setvar); //add back setvar
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
                    case linetype.controlfunction:
                        if (blockfunctions.Contains(lines[i].pieces[0]))
                        {
                            switch(lines[i].pieces[0])
                            {
                                case "if":
                                    lines.Insert(i+2, new lineinfo(lines[i].originallineid, linetype.function, parsecond(lines[i].pieces[1], "B")));
                                    lines.Insert(i+3, new lineinfo(lines[i].originallineid, linetype.function, new List<string> { "conddont_next", "0" }));
                                    lines.Insert(skipindentation(i + 1, 0), new lineinfo(lines[i].originallineid, linetype.standalone, new List<string> { "end" }));
                                    lines[i].pieces = new List<string> { "section" };
                                    break;
                                case "elif":
                                    lines.Insert(i + 2, new lineinfo(lines[i].originallineid, linetype.function, parsecond(lines[i].pieces[1], "B")));
                                    lines.Insert(i + 3, new lineinfo(lines[i].originallineid, linetype.function, new List<string> { "conddont_next", "0" }));
                                    lines.Insert(skipindentation(i + 1, 0), new lineinfo(lines[i].originallineid, linetype.standalone, new List<string> { "end" }));
                                    lines[i].pieces = new List<string> { "connection" };
                                    break;
                                case "else":
                                case "otherwise":
                                    lines[i].pieces = new List<string> { "connection" };
                                    break;
                                case "while":
                                    List<string> conditioncheck = parsecond(lines[i].pieces[1], "B");
                                    int origid = lines[i].originallineid;
                                    lines.Insert(i + 2, new lineinfo(origid, linetype.function, conditioncheck));
                                    lines.Insert(i + 3, new lineinfo(origid, linetype.function, new List<string> { "conddont_end", "0" }));
                                    int skap = skipindentation(i + 1, 0);
                                    lines.Insert(skap, new lineinfo(origid, linetype.standalone, new List<string> { "retry" }));
                                    if (lines[skap+1].pieces[0] == "otherwise")
                                    {
                                        lines.Insert(i, new lineinfo(origid, linetype.function, conditioncheck));
                                        lines.Insert(i + 3, new lineinfo(origid, linetype.function, new List<string> { "conddo_next", "-1" })); //the next flag would be on the otherwise connection; add offset of minus one since we're not in that indent yet
                                    }
                                    lines[i].pieces = new List<string> { "section" };
                                    break;
                            }
                        }
                        else
                        {
                            lines.Insert(i, new lineinfo(lines[i].originallineid, linetype.fazfuck, parsecond(lines[i].pieces[1], "B")));
                            i++;
                            lines[i].pieces.RemoveRange(1, lines[i].pieces.Count - 1);
                        }
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
        Dictionary<string, string> specialcomparisons = new Dictionary<string, string>
        {
            { "contains", "c" },
            { "!contains", "!c" },
            { "startswith", "s" },
            { "!startswith", "!s" },
            { "endswith", "e" },
            { "!endswith", "!e" }
        };
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
                return new List<string> { "comp", outputvar, isalisteventhebestchoiceforthis[0], isalisteventhebestchoiceforthis[1], isalisteventhebestchoiceforthis[2] };
        }
        //control flow conversion
        /*void gototime()
        {
            int loopid = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].type == linetype.controlfunction)
                {
                    if (lines[i].pieces[0] == "if")
                    {
                        lineinfo compline = new lineinfo();
                        compline.pieces = parsecond(lines[i].pieces[2], "B");
                        compline.originallineid = lines[i].originallineid;
                        lines.Insert(i, compline);
                        i++;
                        lines[i].pieces = new List<string> { "conddontgoto", "@next" + loopid };
                        bool wouwie = true;
                        int checkers = i;
                        while(wouwie)
                        {
                            checkers = skipindentation(checkers + 1, 0) + 1;
                            if (lines[checkers].pieces[0] == "elif")
                            {
                                compline = new lineinfo();
                                compline.pieces = new List<string> { "goto", "@end" + loopid };
                                compline.originallineid = lines[checkers].originallineid;
                                lines.Insert(checkers, compline);
                                checkers++;
                                compline = new lineinfo();
                                compline.pieces = parsecond(lines[checkers].pieces[2], "B");
                                compline.originallineid = lines[checkers].originallineid;
                                lines.Insert(checkers, compline);
                                lines[checkers].markers.Add(new marker(markertype.next, loopid));
                                checkers++;
                                lines[checkers].pieces = new List<string> { "conddontgoto", "@next" + loopid };
                            }
                            else if (lines[checkers].pieces[0] == "else")
                            {
                                lines[checkers].pieces = new List<string> { "goto", "@end" + loopid };
                                //checkers++;
                                lines[checkers+1].markers.Add(new marker(markertype.next, loopid));
                            }
                            else
                                wouwie = false;
                        }
                        lines[checkers].markers.Add(new marker(markertype.end, loopid));
                        loopid++;
                    }
                    //add while
                }
            }
        }*/
        public Dictionary<string, string[]> controlgotoes = new Dictionary<string, string[]>
        {
            { "retry", new string[] { "goto", "@start" } },
            { "next", new string[] { "goto", "@next" } },
            { "end", new string[] { "goto", "@end" } },
            { "conddont_retry", new string[] { "condontgoto", "@start" } },
            { "conddont_next", new string[] { "condontgoto", "@next" } },
            { "conddont_end", new string[] { "condontgoto", "@end" } },
            { "conddo_retry", new string[] { "condogoto", "@start" } },
            { "conddo_next", new string[] { "condogoto", "@next" } },
            { "conddo_end", new string[] { "condogoto", "@end" } }
        };
        void gototime()
        {
            int indentation = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].type == linetype.standalone)
                {
                    if (lines[i].pieces[0] == "section")
                    {
                        lines[i].markers.Add(new marker(markertype.start, indentation));
                        bool weredonehere = true;
                        int checkers = i;
                        while(weredonehere)
                        {
                            checkers = skipindentation(checkers + 1, 0) + 1;
                            if (lines[checkers].pieces[0] == "connection")
                            {
                                removeline(checkers);
                                lines[checkers].markers.Add(new marker(markertype.next, indentation));
                            }
                            else
                                weredonehere = false;
                        }
                        lines[checkers].markers.Add(new marker(markertype.end, indentation));
                    }
                    else if (lines[i].pieces[0] == "{")
                        indentation++;
                    else if (lines[i].pieces[0] == "}")
                        indentation--;
                }
                else if (lines[i].type == linetype.function || lines[i].type == linetype.standalone)
                {
                    if (controlgotoes.ContainsKey(lines[i].pieces[0]))
                    {
                        int offset = 0;
                        if (lines[i].type == linetype.function)
                            offset = int.Parse(lines[i].pieces[1]);
                        lines[i].pieces = new List<string> { controlgotoes[lines[i].pieces[0]][0], controlgotoes[lines[i].pieces[0]][1] + (indentation - offset) };
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
        //remove brackets
        void removebrackets()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].type == linetype.standalone)
                {
                    if (lines[i].pieces[0] == "{" || lines[i].pieces[0] == "}")
                    {
                        removeline(i);
                        i--;
                    }
                }
            }
        }
        //convert from dynamic
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
                    else if (token.StartsWith("@"))
                    {
                        if (token.StartsWith("@+"))
                        {
                            int offset = int.Parse(token.Substring(2));
                            lines[i].pieces[j] = (i + offset).ToString();
                        }
                        else if (token.StartsWith("@-"))
                        {
                            int offset = int.Parse(token.Substring(2));
                            lines[i].pieces[j] = (i - offset).ToString();
                        }
                        else if (token.StartsWith("@start"))
                        {
                            int searchid = int.Parse(token.Substring(6));
                            lines[i].pieces[j] = searchmarker(i, markertype.start, searchid, -1).ToString();
                        }
                        else if (token.StartsWith("@next"))
                        {
                            int searchid = int.Parse(token.Substring(5));
                            lines[i].pieces[j] = searchmarker(i, markertype.next, searchid, 1).ToString();
                        }
                        else if (token.StartsWith("@end"))
                        {
                            int searchid = int.Parse(token.Substring(4));
                            lines[i].pieces[j] = searchmarker(i, markertype.end, searchid, 1).ToString();
                        }
                    }
                    else if (token.StartsWith("-") || token.StartsWith("+") || char.IsDigit(token[0]))
                    {
                        if (float.TryParse(token.Replace(',', '.'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _))
                            lines[i].pieces[j] = "#" + token.Replace(',', '.');
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
            while(curline != 0 && curline != lines.Count)
            {
                curline += direction;
                if (lines[curline].hasmarker(tipee, idee))
                    return curline;
                else if (tipee == markertype.next && lines[curline].hasmarker(markertype.end, idee))
                    return curline;
            }
            return startline;
        }
        //replace longer function names
        public Dictionary<string, string> shorthand = new Dictionary<string, string>
        {
            { "print", "pr" },
            { "release", "rel" },
            { "goto", "go" },
            { "conddontgoto", "cdg" },
            { "condsplitgoto", "csg" },
            { "conddogoto", "cdog" },
            { "comp", "c" },
            { "set", "s" },
            { "undef", "ud" },
            { "fstk_add", "fa" },
            { "fstk_discard", "fd" },
            { "fstk_popgoto", "fg" },
            { "fstk_pop", "fp" },
            { "to_string", "tstr" },
            { "parse", "prs" },
            { "string_replace", "str_rep" },
            { "string_lower", "str_lw" },
            { "string_upper", "str_up" },
            { "string_substring", "str_sub" },
            { "string_split", "str_spl" },
            { "string_join", "str_joe" },
            { "string_length", "str_len" },
            { "list_create", "l_cr" },
            { "list_addtop", "l_at" },
            { "list_addat", "l_aa" },
            { "list_set", "l_s" },
            { "list_get", "l_g" },
            { "list_amount", "l_am" },
            { "list_removefirst", "l_rf" },
            { "list_removeat", "l_ra" },
            { "list_removeallof", "l_rl" },
            { "list_clear", "l_cl" },
            { "list_indexof", "l_i" },
            { "list_has", "l_h" }
        };
        public Dictionary<string, string[]> mathfunc = new Dictionary<string, string[]>
        {
            { "round", new string[] { "m2", "rn" } },
            { "floor", new string[] { "m2", "fl" } },
            { "flr", new string[] { "m2", "fl" } },
            { "ceiling", new string[] { "m2", "cl" } },
            { "ceil", new string[] { "m2", "cl" } },
            { "sin", new string[] { "mt", "s" } },
            { "cos", new string[] { "mt", "c" } },
            { "tan", new string[] { "mt", "t" } },
            { "asin", new string[] { "mt", "as" } },
            { "acos", new string[] { "mt", "ac" } },
            { "atan", new string[] { "mt", "at" } },
            { "sign", new string[] { "m1", "s" } },
            { "max", new string[] { "mx", "mx" } },
            { "min", new string[] { "mx", "mn" } },
            { "axb", new string[] { "m3", "lf" } },
            { "sqrt", new string[] { "m1", "sq" } },
            { "√", new string[] { "m1", "sq" } },
            { "cadd", new string[] { "mx", "c+" } },
            { "cmul", new string[] { "mx", "c×" } },
            { "sqradd", new string[] { "mx", "sa" } }
        };
        void makeshort()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                string functio = lines[i].pieces[0];
                if (functio == "math")
                {
                    if (lines[i].pieces[2] == "mx" || lines[i].pieces[2] == "mn")
                        lines[i].pieces[0] = "mx";
                    else
                        lines[i].pieces[0] = "m" + (lines[i].pieces.Count - 3);
                }
                else if (mathfunc.ContainsKey(functio))
                {
                    lines[i].pieces[0] = mathfunc[functio][0];
                    lines[i].pieces.Insert(2, mathfunc[functio][1]);
                }
                else if (shorthand.ContainsKey(functio))
                    lines[i].pieces[0] = shorthand[functio];
            }
        }
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
            return thebuilder.ToString();
        }
    }
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
