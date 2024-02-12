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
            
            using (Lecturas L = new Lecturas())
            {
                /* Console.WriteLine("Ingresa un caracter para reemplazar en el archivo: ");
                ConsoleKeyInfo keyInfo = Console.ReadKey(); */
                char letra = '$';
                L.BorrarVocales(letra);
            }         
            
        }
    }
}