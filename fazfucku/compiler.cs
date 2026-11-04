using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fazfucku
{
    enum linetype { math, mathdirect, boel, booldirect, function, functionset, functionout, standalone, controlfunction, set, definationfunction, definationhook, definationsection, definationfunctionstandalone, definationfunctionout, definationfunctionoutonly, definationhookstandalone, broken, theresNOTHING, functionboolnargumentset, functionoutset }
    internal class fazfuckucompiler
    {
        public string inputfile;
        List<lineinfo> lines;
        public string compile()
        {
            try
            {
                lines = new List<lineinfo>();
                Console.WriteLine("compiler: initial lexer start");
                initiallexer();
                Console.WriteLine("compiler: line convert thingy get better name start");
                lineconvertthingy();
                Console.WriteLine("compiler: output file");
                return outputfile();
            }
            catch (Exception e)
            {
                Console.WriteLine("compiler: " + e.ToString());
                return string.Empty;
            }
        }
        //initial lexer
        lineinfo workingline;
        string workingtoken;
        int currentfileline;
        enum lextype { defauld, stringe }
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
        void initiallexer()
        {
            workingline = new lineinfo();
            workingtoken = string.Empty;
            curlextype = lextype.defauld;
            char prevchar = '\0';
            char curchar = '\0';
            for (int i = 0; i < inputfile.Length; i++)
            {
                curchar = inputfile[i];
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
                        else if (curchar == '\"')
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
                                curchar = inputfile[i];
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
                                curchar = inputfile[i];
                            }
                            popline();
                        }
                        else if (curchar == '(' || curchar == ')' || curchar == ',' || curchar == '=' || curchar == '[' || curchar == ']' || curchar == '⟨' || curchar == '⟩')
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
                            if (curchar == '\"' || curchar == '\\')
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
                        else if (curchar == '\"')
                        {
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
        }
        //line convert thingy
        void lineconvertthingy()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                linetype ligmaballs = getlinetype(lines[i].pieces);

                if (ligmaballs == linetype.controlfunction || (ligmaballs == linetype.standalone && (lines[i].pieces[0] == "else" || lines[i].pieces[0] == "otherwise")))
                {
                    if (lines[i+1].pieces[0] != "{")
                    {
                        lineinfo newline = new lineinfo();
                        newline.pieces.Add("{");
                        newline.originallineid = lines[i].originallineid;
                        lines.Insert(i + 1, newline);
                        newline = new lineinfo();
                        newline.pieces.Add("}");
                        newline.originallineid = lines[i + 1].originallineid;
                        lines.Insert(i + 3, newline);
                    }
                }
                Console.WriteLine(ligmaballs.ToString());
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
            if (daline[0] == "func ")
            {
                if (daline.Count == 2)
                    return linetype.definationfunctionstandalone;
                if (daline[2] == "(")
                    return linetype.definationfunction;
                if (daline[2] == "[") //implement later
                    return linetype.broken;
            }
            if (daline[0] == "hook ")
            {
                if (daline.Count == 2)
                    return linetype.definationhookstandalone;
                return linetype.definationhook;
            }
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
                if (daline[3] == "(")
                    return linetype.functionset;
                if (daline[3] == "⟨")
                    return linetype.functionboolnargumentset;
                if (daline[3] == "[")
                    return linetype.functionoutset;
                return linetype.broken;
            }
            if (daline[1] == "(")
                return linetype.function;
            if (daline[1] == "⟨")
                return linetype.controlfunction;
            if (daline[1] == "[")
                return linetype.functionout;
            return linetype.broken;
        }
        //output
        string outputfile()
        {
            StringBuilder thebuilder = new StringBuilder();
            foreach (lineinfo lijn in lines)
            {
                thebuilder.AppendLine(string.Join("¦", lijn.pieces));
            }
            thebuilder.Remove(thebuilder.Length - 1, 1);
            return thebuilder.ToString();
        }
    }
    class lineinfo
    {
        public List<string> pieces = new List<string>();
        public List<marker> markers = new List<marker>();
        public int originallineid = -1;
        public linetype type = linetype.broken;
    }
    class marker
    {
        public enum markertype { start, next, end, unique }
        public markertype tipe;
        public int markerid;
    }
}
