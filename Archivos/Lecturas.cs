using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Archivos
{
    class Lecturas
    {
        private int a;
        private StreamReader fuente;

        public Lecturas() {
            a = 10;
            fuente = new StreamReader("prueba.cpp");
        }

        public void Display() {
            char c;
            while(!fuente.EndOfStream) {
                c = (char) fuente.Read();
                Console.Write(c);
            }
        }

        public void mensaje() {
            Console.WriteLine("Hola ITQ!" + a);
        }
    }
}