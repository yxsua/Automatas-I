using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Archivos
{
    class Program
    {
        static void Main(string[] args) 
        {
            Lecturas L = new Lecturas();
            L.Display();
            L.Dispose();
        }
    }
}