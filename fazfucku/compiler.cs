using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using static fazfucku.marker;

namespace fazfucku
{
    enum linetype { math, mathdirect, boel, booldirect, function, functionset, standalone, controlfunction, set, definationhookfunc, definationsection, broken, theresNOTHING, functionboolnargumentset, fazfuck, controlfunctionarged }
    internal class fazfuckucompiler
    {
        public string[] inputfile;
        List<lineinfo> lines;
        public string compile()
        {
            try
            {
                lines = new List<lineinfo>();
                Console.WriteLine("compiler: initial lexer start");
                initiallexer();
                Console.WriteLine("compiler: line detection start");
                linedetector();
                Console.WriteLine("compiler: main fazass conversion start");
                mainfazassconvert();
                Console.WriteLine("compiler: convert control flow start");
                gototime();
                Console.WriteLine("compiler: remove brackets start");
                removebrackets();
                Console.WriteLine("compiler: convert from dynamic start");
                convertfromdynamic();
                Console.WriteLine("compiler: replace longer function names");
                makeshort();
                Console.WriteLine("compiler: output file");
                return outputfile(false);
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
                workingline = new lineinfo();
            }
        }
        enum lextype { defauld, stringe, logicalic }
        void initiallexer()
        {
            workingline = new lineinfo();
            workingtoken = string.Empty;
            curlextype = lextype.defauld;
            char prevchar = '\0';
            char curchar = '\0';
            inputfile[0] = string.Empty;
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
                            if (curchar == '\"' || curchar == '\\' || curchar == '»')
                                workingtoken += curchar;
                            else if (curchar == 'n')
                                workingtoken += '\n';
                            else
                            {
                                workingtoken += '\\';
                                workingtoken += curchar;
                            }
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
        void linedetector()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                linetype ligmaballs = getlinetype(lines[i].pieces);
                lines[i].type = ligmaballs;
                if (ligmaballs == linetype.broken)
                    doerror("cannot get the type of line for " + lines[i].originallineid + " (" + i + ")!");
                else if (ligmaballs == linetype.controlfunction || (ligmaballs == linetype.standalone && (lines[i].pieces[0] == "else" || lines[i].pieces[0] == "otherwise")))
                {
                    if (lines[i+1].pieces[0] != "{")
                    {
                        lineinfo newline = new lineinfo();
                        newline.pieces.Add("{");
                        newline.originallineid = lines[i].originallineid;
                        lines.Insert(i + 1, newline);
                        newline = new lineinfo();
                        newline.pieces.Add("}");
                        newline.originallineid = lines[i].originallineid;
                        lines.Insert(i + 3, newline);
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
                        lines[i].type = linetype.fazfuck;
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
                            lines[i].type = linetype.fazfuck;
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
                            lines[i].type = linetype.fazfuck;
                        }
                        break;
                    case linetype.mathdirect:
                        {
                            lines[i].pieces.Remove("(");
                            lines[i].pieces.Remove(")");
                            parsemath(string.Join("", lines[i].pieces), out string num1, out string oper, out string num2);
                            lines[i].pieces = new List<string> { "math", num1, oper, num1, num2 };
                            lines[i].type = linetype.fazfuck;
                        }
                        break;
                    case linetype.set:
                        {
                            lines[i].pieces = new List<string> { "set", lines[i].pieces[0], lines[i].pieces[2] };
                            lines[i].type = linetype.fazfuck;
                        }
                        break;
                    case linetype.booldirect:
                        lines[i].pieces = parsecond(lines[i].pieces[1], "");
                        lines[i].type = linetype.fazfuck;
                        break;
                    case linetype.boel:
                        lines[i].pieces = parsecond(lines[i].pieces[3], lines[i].pieces[0]);
                        lines[i].type = linetype.fazfuck;
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
                if (curchar == '+' || curchar == '-' || curchar == '×' || curchar == '÷' || curchar == '%' || curchar == '^' || curchar == '@')
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
                if (splitty.Contains("contains") || splitty.Contains("!contains") || splitty.Contains("startswith") || splitty.Contains("!startswith") || splitty.Contains("endswith") || splitty.Contains("!endswith"))
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
        void gototime()
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
                                lines[checkers].markers.Add(new marker(marker.markertype.next, loopid));
                                checkers++;
                                lines[checkers].pieces = new List<string> { "conddontgoto", "@next" + loopid };
                            }
                            else if (lines[checkers].pieces[0] == "else")
                            {
                                lines[checkers].pieces = new List<string> { "goto", "@end" + loopid };
                                checkers++;
                                lines[checkers].markers.Add(new marker(marker.markertype.next, loopid));
                            }
                            else
                                wouwie = false;
                        }
                        lines[checkers].markers.Add(new marker(marker.markertype.end, loopid));
                        loopid++;
                    }
                    //add while
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
        //convert from dynamic
        void convertfromdynamic()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = 0; j < lines[i].pieces.Count; j++)
                {
                    string token = lines[i].pieces[j];
                    if (token.StartsWith("@"))
                    {
                        if (token.StartsWith("@start"))
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
                else if (shorthand.ContainsKey(functio))
                    lines[i].pieces[0] = shorthand[functio];
            }
        }
        Dictionary<string, string> shorthand = new Dictionary<string, string>
        {
            { "print", "pr" },
            { "goto", "go" },
            { "conddontgoto", "cdg" },
            { "conddogoto", "cdog" },
            { "comp", "c" },
            { "set", "s" },
            { "undef", "ud" },
            { "fstk_add", "fa" },
            { "fstk_discard", "fd" },
            { "fstk_popgoto", "fg" },
            { "fstk_pop", "fp" },
            { "to_string", "tstr" },
            { "string_replace", "str_rep" },
            { "string_lower", "str_lw" },
            { "string_upper", "str_up" },
            { "string_substring", "str_sub" },
            { "string_split", "str_spl" }
        };
        //output
        string outputfile(bool addmarkers)
        {
            StringBuilder thebuilder = new StringBuilder();
            foreach (lineinfo lijn in lines)
            {
                if (addmarkers && lijn.markers.Count > 0)
                    thebuilder.AppendLine(string.Join("¦", lijn.pieces) + "<" + string.Join("<", lijn.markers));
                else
                    thebuilder.AppendLine(string.Join("¦", lijn.pieces));
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
