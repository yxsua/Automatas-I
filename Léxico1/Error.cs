using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexico_1
{
    public class Error : Exception
    {
        public Error(string mensaje, StreamWriter log) : base("Error: "+mensaje)
        {
            log.WriteLine("Error: "+mensaje);
        }
    }
}