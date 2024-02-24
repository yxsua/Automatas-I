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
                    while (!L.finArchivo())
                    {
                        L.nextToken();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
