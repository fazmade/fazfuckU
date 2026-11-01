using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fazfucku
{
    internal class compiler
    {
        public string inputfile;
        public void compile()
        {
            initiallexer();
        }
        List<lineinfo> lines;
        lineinfo workingline;
        string workingtoken;
        int currentfileline;
        void poptoken()
        {
            workingline.pieces.Add(workingtoken);
            workingtoken = string.Empty;
        }
        void popline()
        {
            if (workingtoken != string.Empty)
                poptoken();
            lines.Add(workingline);
            workingline = new lineinfo();
        }
        void initiallexer()
        {
            workingline = new lineinfo();
            workingtoken = string.Empty;
            char prevchar = char.MinValue;
            char curchar = char.MinValue;
            for (int i = 0; i < inputfile.Length; i++)
            {
                curchar = inputfile[i];
                if (curchar == '\n')
                {
                    currentfileline++;
                    if (workingline.originallineid == -1)
                        workingline.originallineid = currentfileline;
                }
                prevchar = curchar;
            }
        }
    }
    class lineinfo
    {
        public List<string> pieces;
        public List<marker> markers;
        public int originallineid = -1;
    }
    class marker
    {
        public enum markertype { iffle, next, loop, end }
        public markertype tipe;
        public int markerid;
    }
}
