using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexico_1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (Lexico L = new Lexico("prueba.cpp"))
                {
                    // Se analizará cada token de manera individual
                    // siempre y cuando no se acabe el archivo
                    while (!L.finArchivo())
                    {
                        L.nextToken();
                    }
                }
            }
            // Hay que manejar las excepciones necesarias en la clase principal
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
