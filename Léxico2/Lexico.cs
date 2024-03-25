using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Lexico_2
{
    public class Lexico : Token, IDisposable
    {
        // Obtenemos el objeto (archivo) que vamos a leer
        // y hacemos un flujo de salida para un log con el mismo nombre
        private StreamReader archivo;
        public StreamWriter log;
        public Lexico() // Constructor
        {
            // El constructor con defecto trabajará siempre con un archivo "prueba"
            log = new StreamWriter("prueba.log");
            log.AutoFlush = true;
            log.WriteLine("Analizador Lexico");
            log.WriteLine("Autor: Ulises Angeles");
            log.WriteLine("Fecha: " + DateTime.Now.ToString());
            // Se arroja esta excepción si no existe el archivo
            if (!File.Exists("prueba.cpp"))
            {
                throw new Error("El archivo prueba.cpp no existe", log);
            }
            archivo = new StreamReader("prueba.cpp");
        }
        public Lexico(string nombre) // Constructor
        {
            // Este constructor trabajará con un archivo pasado como parámetro al crear el objeto
            log = new StreamWriter(Path.GetFileNameWithoutExtension(nombre) + ".log");
            log.AutoFlush = true;
            log.WriteLine("Analizador Lexico");
            log.WriteLine("Autor: Ulises Angeles");
            log.WriteLine("Fecha: " + DateTime.Now.ToString());
            // Hay que revisar que el archivo tenga una extensión de código c++
            if (Path.GetExtension(nombre) != ".cpp")
            {
                throw new Error("El archivo " + nombre + " no tiene extension CPP", log);
            }
            if (!File.Exists(nombre))
            {
                throw new Error("El archivo " + nombre + " no existe", log);
            }
            archivo = new StreamReader(nombre);
        }
        public void Dispose() // Destructor
        {
            archivo.Close();
            log.Close();
        }

        /*
        Identificador   -> L (L|D)*
        Numero          -> D+ (. D+)? (E(+|-)? D+)?
        FinSentencia    -> ;
        OpTermino       -> + | -
        OpFactor        -> * | / | %
        OpLogico        -> && | || | !
        OpRelacional    -> (>|< =?) | (! | =) =
        OpTernario      -> ?
        Asignacion      -> =
        IncTermino      -> ++ | -- | += | -=
        IncFactor       -> *|/|% =
        Cadena          -> "c*"
        Inicio          -> {
        Fin             -> }
        Caracter        -> c
        */

        private const int F = -1;
        private const int E = -2;

        int Automata(int estado, char t)
        {
            switch (estado)
            {
                case 0:
                    if (char.IsLetter(t))
                    {
                        estado = 1;
                    }
                    else if (char.IsDigit(t))
                    {
                        estado = 2;
                    }
                    else if (t == ';')
                    {
                        estado = 8;
                    }
                    else if (t == '+')
                    {
                        estado = 9;
                    }
                    else if (t == '-')
                    {
                        estado = 10;
                    }
                    else if (t == '*' || t == '/' || t == '%')
                    {
                        estado = 12;
                    }
                    else if (t == '&')
                    {
                        estado = 14;
                    }
                    else if (t == '|')
                    {
                        estado = 15;
                    }
                    else if (t == '<' || t == '>')
                    {
                        estado = 17;
                    }
                    else if (t == '=')
                    {
                        estado = 18;
                    }
                    else if (t == '!')
                    {
                        estado = 19;
                    }
                    else if (t == '?')
                    {
                        estado = 21;
                    }
                    else if (t == '\"')
                    {
                        estado = 22;
                    }
                    else if (t == '{')
                    {
                        estado = 24;
                    }
                    else if (t == '}')
                    {
                        estado = 25;
                    }
                    else if (!char.IsWhiteSpace(t))
                    {
                        estado = 26;
                    }

                    return estado;

                    break;
                case 1:
                    this.setClasificacion(Tipos.Identificador);
                    if (!char.IsLetterOrDigit(t))
                    {
                        estado = F;
                    }
                    break;
                case 2:
                    this.setClasificacion(Tipos.Numero);
                    if (t == '.')
                    {
                        estado = 3;
                    }
                    else if (char.ToLower(t) == 'e')
                    {
                        estado = 5;
                    }
                    else if (!char.IsDigit(t))
                    {
                        estado = F;
                    }
                    break;
                case 3:
                    if (char.IsDigit(t))
                    {
                        estado = 4;
                    }
                    else
                    {
                        estado = E;
                    }
                    break;
                case 4:
                    if (char.ToLower(t) == 'e')
                    {
                        estado = 5;
                    }
                    else if (!char.IsDigit(t))
                    {
                        estado = F;
                    }
                    break;
                case 5:
                    if (t == '+' || t == '-')
                    {
                        estado = 6;
                    }
                    else if (char.IsDigit(t))
                    {
                        estado = 7;
                    }
                    else
                    {
                        estado = E;
                    }
                    break;
                case 6:
                    if (char.IsDigit(t))
                    {
                        estado = 7;
                    }
                    else
                    {
                        estado = E;
                    }
                    break;
                case 7:
                    if (!char.IsDigit(t))
                    {
                        estado = F;
                    }
                    break;
                case 8:
                    this.setClasificacion(Tipos.FinSentencia);
                    estado = F;
                    break;
                case 9:
                    this.setClasificacion(Tipos.OpTermino);
                    if (t == '+' || t == '=')
                    {
                        estado = 11;
                    }
                    else
                    {
                        estado = F;
                    }
                    break;
                case 10:
                    this.setClasificacion(Tipos.OpTermino);
                    if (t == '-' || t == '=')
                    {
                        estado = 11;
                    }
                    else
                    {
                        estado = F;
                    }
                    break;
                case 11:
                    this.setClasificacion(Tipos.IncTermino);
                    estado = F;
                    break;
                case 12:
                    this.setClasificacion(Tipos.OpFactor);
                    if (t == '=')
                    {
                        estado = 13;
                    }
                    else
                    {
                        estado = F;
                    }
                    break;
                case 13:
                    this.setClasificacion(Tipos.IncFactor);
                    estado = F;
                    break;
                case 14:
                    this.setClasificacion(Tipos.Caracter);
                    if (t == '&')
                    {
                        estado = 16;
                    }
                    else
                    {
                        estado = F;
                    }
                    break;
                case 15:
                    this.setClasificacion(Tipos.Caracter);
                    if (t == '|')
                    {
                        estado = 16;
                    }
                    else
                    {
                        estado = F;
                    }
                    break;
                case 16:
                    this.setClasificacion(Tipos.OpLogico);
                    estado = F;
                    break;
                case 17:
                    this.setClasificacion(Tipos.OpRelacional);
                    if (t == '=')
                    {
                        estado = 20;
                    }
                    else
                    {
                        estado = F;
                    }
                    break;
                case 18:
                    this.setClasificacion(Tipos.Asignacion);
                    if (t == '=')
                    {
                        estado = 20;
                    }
                    else
                    {
                        estado = F;
                    }
                    break;
                case 19:
                    this.setClasificacion(Tipos.OpLogico);
                    if (t == '=')
                    {
                        estado = 20;
                    }
                    else
                    {
                        estado = F;
                    }
                    break;
                case 20:
                    this.setClasificacion(Tipos.OpRelacional);
                    estado = F;
                    break;
                case 21:
                    this.setClasificacion(Tipos.OpTernario);
                    estado = F;
                    break;
                case 22:
                    this.setClasificacion(Tipos.Cadena);
                    if (t == '\"')
                    {
                        estado = 23;
                    }
                    else if (finArchivo())
                    {
                        estado = E;
                    }
                    break;
                case 23:
                    estado = F;
                    break;
                case 24:
                    this.setClasificacion(Tipos.Inicio);
                    estado = F;
                    break;
                case 25:
                    this.setClasificacion(Tipos.Fin);
                    estado = F;
                    break;
                case 26:
                    this.setClasificacion(Tipos.Caracter);
                    estado = F;
                    break;
            }

            return estado;
        }

        public void nextToken()
        {
            char c;
            string buffer = "";
            int estado = 0;

            while (estado >= 0)
            {
                c = (char)archivo.Peek();

                estado = Automata(estado, c);

                if (estado >= 0)
                {
                    if (estado > 0)
                    {
                        buffer += c;
                    }
                    archivo.Read();
                }
            }

            if (estado == E)
            {
                if (this.getClasificacion() == Tipos.Numero)
                {
                    throw new Error("Lexico: se esperaba un dígito -> " + buffer, log);
                }
                else if (this.getClasificacion() == Tipos.Cadena)
                {
                    throw new Error("Léxico: se esperaba cierre de cadena -> " + buffer, log);
                }
            }

            this.setContenido(buffer);

            if (this.getClasificacion() == Tipos.Identificador)
            {
                switch (this.getContenido())
                {
                    case "char":
                    case "int":
                    case "float":
                        this.setClasificacion(Tipos.TipoDato);
                        break;
                    case "if":
                    case "else":
                    case "switch":
                        this.setClasificacion(Tipos.Condicion);
                        break;
                    case "for":
                    case "while":
                    case "do":
                        this.setClasificacion(Tipos.Ciclo);
                        break;
                }
            }

            log.WriteLine(this.getContenido() + " = " + this.getClasificacion());
        }
        public bool finArchivo()
        {
            return archivo.EndOfStream;
        }
    }
}