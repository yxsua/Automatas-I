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
            // L.CopyFile();
            // L.DisplayFile();
            // Console.WriteLine("Cantidad de letras: " + L.ContarLetras());
            // L.BorraLetras();
            L.BorrarTodo();
            L.Dispose();
            
        }
    }
}