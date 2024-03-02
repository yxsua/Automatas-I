using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Lexico_1
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
                // Los identificadores empiezan con una letra seguido
                // por una cantidad indefinida de letras o digitos
                setClasificacion(Tipos.Identificador);

                // Si no es una letra o un dígito, entonces no es necesario
                // concatenar más
                while (char.IsLetterOrDigit(c = (char)archivo.Peek()))
                {
                    buffer += c;
                    archivo.Read();
                }
            }
            else if (char.IsDigit(c))
            {
                // Si empieza con un dígito, entonces es un número
                // Un número puede tener una cantidad n de dígitos,
                // tener parte fraccionaría y una parte exponencial
                // con o sin signo (sin signo es considerado positivo)
                setClasificacion(Tipos.Numero);

                // Si no hay más dígitos, no es necesario seguir concatenando
                while (char.IsDigit(c = (char)archivo.Peek()))
                {
                    buffer += c;
                    archivo.Read();
                }

                // Si después del dígito hay un punto, concatenamos la parte fraccional
                if (c == '.')
                {
                    // Parte fraccional
                    archivo.Read();
                    buffer += c;
                    if (char.IsDigit(c = (char)archivo.Peek()))
                    {
                        // Se concatenan números hasta que ya no haya más dígitos
                        while (char.IsDigit(c = (char)archivo.Peek()))
                        {
                            buffer += c;
                            archivo.Read();
                        }
                    }
                    else
                    {
                        // Si se coloca un punto pero ningún número después, 
                        // arrojamos una excepción
                        throw new Error("lexico: se espera un digito " + buffer, log);
                    }
                }
                // Si el número tiene una E, concatenamos la parte exponencial
                if (char.ToLower(c) == 'e')
                {
                    archivo.Read();
                    buffer += c;
                    // Revisamos que después de la E haya un dígito o un + o -
                    if (char.IsDigit(c = (char)archivo.Peek()) || c == '+' || c == '-')
                    {
                        // Si es un más o un menos, hay que añadirlo al buffer
                        if (c == '+' || c == '-')
                        {
                            buffer += c; archivo.Read();
                        }

                        // Concatenamos una cantidad n de dígitos
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
                            // Si no se coloca un número después del signo del exponencial,
                            // se arroja error
                            throw new Error("lexico: se espera un digito " + buffer, log);
                        }
                    }
                    // Si el usuario no ingresa un dígito después de la E
                    // arrojamos una excepción
                    else
                    {
                        throw new Error("lexico: se espera un digito " + buffer, log);
                    }
                }
            }
            else if (c == ';')
            {
                // Al terminar una sentencia, se usa el ;
                setClasificacion(Tipos.FinSentencia);
            }
            else if (c == '+' || c == '-')
            {
                // Si se usa el operador + o - sin nada adicional, se clasifica
                // como un operador de término
                setClasificacion(Tipos.OpTermino);
                char d = c;
                // Si se usa otro + o - además del + o - original,
                // entonces trabajamos con un incremento de término
                // Lo mimso aplica si se usa en = después
                if ((((c = (char)archivo.Peek()) == '+' || c == '-') && d == c) || c == '=')
                {
                    buffer += c;
                    archivo.Read();
                    setClasificacion(Tipos.IncTermino);
                }
            }
            else if (c == '*' || c == '/' || c == '%')
            {
                // Se aplica la misma lógica que el token anterior,
                // si está solo el operador entonces se clasifica
                // como operador de factor, si tiene un = después entonces
                // es un incremento de factor
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
                // Los bloques de código inician con un {
                setClasificacion(Tipos.Inicio);
            }
            else if (c == '}')
            {
                // Y terminan con un }
                setClasificacion(Tipos.Fin);
            }
            else if (c == '?')
            {
                // El operador ternario funciona como un if abreviado, se usa con el caracter ?
                setClasificacion(Tipos.OpTernario);
            }
            else if (c == '=')
            {
                // Si el signo = está solo, entonces es un operador de asignación,
                // Si lo sigue un símbolo de mayor, menor u otro igual,
                // entonces es un operador relacional
                setClasificacion(Tipos.Asignacion);
                if ((c = (char)archivo.Peek()) == '=')
                {
                    buffer += c;
                    archivo.Read();
                    setClasificacion(Tipos.OpRelacional);
                }
            }
            else if (c == '|' || c == '&' || c == '!')
            {
                char d = c;
                // Si el | o & están solos, entonces es un caracter sin más
                setClasificacion(Tipos.Caracter);

                // Si el ! está solo, es un operador lógico
                if (c == '!')
                {
                    setClasificacion(Tipos.OpLogico);
                    // Si al ! le sigue un =, entonces es un operador relacional
                    if ((c = (char)archivo.Peek()) == '=')
                    {
                        buffer += c;
                        archivo.Read();
                        setClasificacion(Tipos.OpRelacional);
                    }
                }
                // Si al | o & le sigue otro caracter igual, entonces es un operador lógico
                else if (((c = (char)archivo.Peek()) == '|' || c == '&') && d == c)
                {
                    buffer += c;
                    archivo.Read();
                    setClasificacion(Tipos.OpLogico);
                }
            }
            // Los caracteres de menor o mayor son operadores relacionales
            else if (c == '<' || c == '>')
            {
                setClasificacion(Tipos.OpRelacional);
                // Los operadores menor o igual y mayor o igual
                // también son operadores relacionales
                if ((c = (char)archivo.Peek()) == '=')
                {
                    buffer += c;
                    archivo.Read();
                }
            }
            else if (c == '\"')
            {
                setClasificacion(Tipos.Cadena);
                // Una cadena se inicia siempre con unas comillas
                while (((c = (char)archivo.Peek()) != '\"') && (!finArchivo()))
                {
                    // A la cadena se le concatenan 0 o más caracteres, hasta encontrar la siguiente comilla
                    buffer += c;
                    archivo.Read();
                }
                if (finArchivo())
                {
                    // Si la comilla nunca se cierra, arrojamos un error
                    throw new Error("lexico: se esperaba cierre de cadena " + buffer, log);
                }

                // El ciclo no añadirá la última comilla de la cadena,
                // hay que concatenarla fuera del ciclo
                buffer += c;
                archivo.Read();
            }
            else if (c == '$')
            {
                this.setClasificacion(Tipos.Caracter);
                if ((char.IsDigit(c = (char)archivo.Peek())))
                {
                    this.setClasificacion(Tipos.Moneda);
                    while (char.IsDigit(c = (char)archivo.Peek()))
                    {
                        buffer += c;
                        archivo.Read();
                    }
                    // Si después del dígito hay un punto, concatenamos la parte fraccional
                    if (c == '.')
                    {
                        // Parte fraccional
                        archivo.Read();
                        buffer += c;
                        if (char.IsDigit(c = (char)archivo.Peek()))
                        {
                            // Se concatenan números hasta que ya no haya más dígitos
                            while (char.IsDigit(c = (char)archivo.Peek()))
                            {
                                buffer += c;
                                archivo.Read();
                            }
                        }
                        else
                        {
                            // Si se coloca un punto pero ningún número después, 
                            // arrojamos una excepción
                            throw new Error("lexico: se espera un digito " + buffer, log);
                        }
                    }
                }
            }
            else
            {
                // Cualquier otro token se considerará un caracter
                setClasificacion(Tipos.Caracter);
            }
            // Añadimos el contenido el buffer al contenido del token
            setContenido(buffer);
            log.WriteLine(getContenido() + " = " + getClasificacion());
        }
        public bool finArchivo()
        {
            return archivo.EndOfStream;
        }
    }
}