using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

/*
    20%
        Requerimiento 1: Escribir en el LOG, fecha y hora en los constructores
        Requerimiento 2: Agregar el + o el - al exponente del numero
    80%
        OpTermino       -> + | -
        OpFcator        -> * | / | %
        OpLogico        -> && | || | !
        OpRelacional    -> (>|< =?) | (! | =) =
        Asignacion      -> =
        IncTermino      -> ++ | -- | += | -=
        IncFactor       -> *|/|% =
        Cadena          -> "c*"
        Inicio          -> {
        Fin             -> }
*/

namespace Lexico_1
{
    public class Lexico : Token, IDisposable
    {
        private StreamReader archivo;
        public StreamWriter log;
        public Lexico() // Constructor
        {
            log = new StreamWriter("prueba.log");
            log.AutoFlush = true;
            log.WriteLine("Analizador Lexico");
            log.WriteLine("Autor: Guillermo Fernandez");
            log.WriteLine("Fecha: " + DateTime.Now.ToString());
            if (!File.Exists("prueba.cpp"))
            {
                throw new Error("El archivo prueba.cpp no existe", log);
            }
            archivo = new StreamReader("prueba.cpp");
        }
        public Lexico(string nombre) // Constructor
        {
            log = new StreamWriter(Path.GetFileNameWithoutExtension(nombre) + ".log");
            log.AutoFlush = true;
            log.WriteLine("Analizador Lexico");
            log.WriteLine("Autor: Ulises Angeles");
            log.WriteLine("Fecha: " + DateTime.Now.ToString());
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

        public void nextToken()
        {
            char c;
            string buffer = "";

            // a) Buscar el primer caracter válido
            while (char.IsWhiteSpace(c = (char)archivo.Read()))
            {
            }
            buffer += c; // buffer = buffer + c;
            if (char.IsLetter(c))
            {
                setClasificacion(Tipos.Identificador);
                while (char.IsLetterOrDigit(c = (char)archivo.Peek()))
                {
                    buffer += c;
                    archivo.Read();
                }
            }
            else if (char.IsDigit(c))
            {
                setClasificacion(Tipos.Numero);
                while (char.IsDigit(c = (char)archivo.Peek()))
                {
                    buffer += c;
                    archivo.Read();
                }
                if (c == '.')
                {
                    // Parte fraccional
                    archivo.Read();
                    buffer += c;
                    if (char.IsDigit(c = (char)archivo.Peek()))
                    {
                        while (char.IsDigit(c = (char)archivo.Peek()))
                        {
                            buffer += c;
                            archivo.Read();
                        }
                    }
                    else
                    {
                        throw new Error("lexico: se espera un digito " + buffer, log);
                    }
                }
                if (char.ToLower(c) == 'e')
                {
                    archivo.Read();
                    buffer += c;
                    if (char.IsDigit(c = (char)archivo.Peek()) || c == '+' || c == '-')
                    {
                        if (c == '+' || c == '-') 
                        { 
                            buffer += c; archivo.Read(); 
                        }

                        while (char.IsDigit(c = (char)archivo.Peek()))
                        {
                            buffer += c;
                            archivo.Read();
                        }
                    }
                    else
                    {
                        throw new Error("lexico: se espera un digito " + buffer, log);
                    }
                }
            }
            else if (c == ';')
            {
                setClasificacion(Tipos.FinSentencia);
            }
            else if (c == '+' || c == '-')
            {
                setClasificacion(Tipos.OpTermino);
                char d = c;
                if (((c = (char)archivo.Peek()) == '+' || c == '-') && d == c)
                {
                    buffer += c;
                    archivo.Read();
                    setClasificacion(Tipos.IncTermino);
                }
                else if ((c = (char)archivo.Peek()) == '=')
                {
                    buffer += c;
                    archivo.Read();
                    setClasificacion(Tipos.IncTermino);
                }
            }
            else if (c == '*' || c == '/' || c == '%')
            {
                setClasificacion(Tipos.OpFactor);
                char d = c;
                if ((c = (char)archivo.Peek()) == '=')
                {
                    buffer += c;
                    archivo.Read();
                    setClasificacion(Tipos.IncFactor);
                }
            }
            else if (c == '{')
            {
                setClasificacion(Tipos.Inicio);
            }
            else if (c == '}')
            {
                setClasificacion(Tipos.Fin);
            }
            else if (c == '?')
            {
                setClasificacion(Tipos.OpTernario);
            }
            else if (c == '=')
            {
                setClasificacion(Tipos.Asignacion);
                if ((c = (char)archivo.Peek()) == '=' || c == '>' || c == '<')
                {
                    buffer += c;
                    archivo.Read();
                    setClasificacion(Tipos.OpRelacional);
                }
            }
            else if (c == '|' || c == '&' || c == '!')
            {
                char d = c;
                setClasificacion(Tipos.Caracter);
                if (c == '!')
                {
                    setClasificacion(Tipos.OpLogico);
                    if ((c = (char)archivo.Peek()) == '=')
                    {
                        buffer += c;
                        archivo.Read();
                        setClasificacion(Tipos.OpRelacional);
                    }
                }
                else if (((c = (char)archivo.Peek()) == '|' || c == '&') && d == c)
                {
                    buffer += c;
                    archivo.Read();
                    setClasificacion(Tipos.OpLogico);
                }
            }
            else if (c == '<' || c == '>')
            {
                setClasificacion(Tipos.OpRelacional);
                if ((c = (char)archivo.Peek()) == '=')
                {
                    buffer += c;
                    archivo.Read();
                }
            }
            else if (c == '\"') {
                setClasificacion(Tipos.Cadena);
                while (((c = (char)archivo.Peek()) != '\"') && (!finArchivo()))
                {
                    buffer += c;
                    archivo.Read();
                }
                if(finArchivo()) 
                {
                    throw new Error("lexico: se esperaba cierre de cadena " + buffer, log);
                }
                buffer += c;
                archivo.Read();
            }
            /* Lo que se usaría para comentarios
            else if (c == '/') {
                setClasificacion(Tipos.Caracter);
                if ((c = (char)archivo.Peek()) == '/' || c == '*')
                {
                    buffer += c;
                    archivo.Read();
                    setClasificacion(Tipos.Comentario); 
                }
            }
            else if (c == '*') {
                setClasificacion(Tipos.Caracter);
                if ((c = (char)archivo.Peek()) == '/')
                {
                    buffer += c;
                    archivo.Read(); 
                    setClasificacion(Tipos.Comentario); 
                }
            } 
            */
            else
            {
                setClasificacion(Tipos.Caracter);
            }
            setContenido(buffer);
            log.WriteLine(getContenido() + " = " + getClasificacion());
        }
        public bool finArchivo()
        {
            return archivo.EndOfStream;
        }
    }
}