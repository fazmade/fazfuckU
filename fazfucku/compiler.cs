using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fazfucku
{
    internal class compiler
    {
        public string inputfile
        public void compile()
        {

        }
        private void initiallexer()
        {
            char prevchar = char.MinValue;
            char curchar = char.MinValue;
            for (int i = 0; i < inputfile.Length; i++)
            {
                curchar = inputfile[i];
            }
        }
    }
    class lineinfo
    {
        public List<string> pieces;
        public List<marker> markers;
        public uint originallineid;
    }
    class marker
    {
        public enum markertype { iffl, next, loop, end }
        public markertype tipe;
        public uint markerid;
    }
}
