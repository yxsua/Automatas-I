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
        private StreamWriter objeto;

        public Lecturas() {
            a = 10;
            fuente = new StreamReader("prueba.cpp");
            objeto = new StreamWriter("prueba.txt");
            objeto.WriteLine("*su nombre*");
            objeto.WriteLine("\nagarrados de la manooo, chinguen todos, a su madreeeEEeeEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
        }

        public void Dispose() {
            Console.WriteLine("\nDestructor");
            fuente.Close(); 
            objeto.Close();
        }

        public void Display() {
            char c;
            while(!fuente.EndOfStream) {
                c = (char) fuente.Read();
                Console.Write(c);
                objeto.Write(c);
            }
            fuente.Close();
            objeto.Close();
        }

        public void mensaje() {
            Console.WriteLine("Hola ITQ!" + a);
        }
    }
}