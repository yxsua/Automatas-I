using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Proyecto, realizado por 
// Angeles Arteaga Ulises Yasua
// & Suarez Cruz Brayan Eduardo

namespace Archivos
{
    class Lecturas : IDisposable
    {
        private int a;
        private StreamReader fuente;
        private StreamWriter objeto;

        public Lecturas() {
            Console.WriteLine("Constructor inicializado...");
            a = 10;
            fuente = new StreamReader("prueba.cpp");
            objeto = new StreamWriter("prueba.txt");
        }

        public void Dispose() {
            Console.WriteLine("\nDestructor");
            fuente.Close(); 
            objeto.Close();
        }

        public void CopyFile() {
            char c;
            while(!fuente.EndOfStream) {
                c = (char) fuente.Read();
                objeto.Write(c);
            }
        }

        public void DisplayFile() {
            char c;
            while(!fuente.EndOfStream) {
                c = (char) fuente.Read();
                Console.Write(c);
            }
        }

        public int ContarLetras() {
            int letras = 0;
            while(!fuente.EndOfStream) {
                if (char.IsLetter((char) fuente.Read())) {
                    letras++;
                }
            }
            return letras;
        }
        
        public void BorraLetras() {
            while(!fuente.EndOfStream) {
                char c = (char) fuente.Read();
                if(char.IsLetter((char) c)) {
                    objeto.Write("-");
                } else {
                    objeto.Write(c);
                }
            }
        }


        public void BorrarTodo() {
            while(!fuente.EndOfStream) {
                char c = (char) fuente.Read();
                if(char.IsWhiteSpace((char) c)) {
                    objeto.Write(c);
                } else {
                    objeto.Write("-");
                }
            }
        }
        
        public void mensaje() {
            Console.WriteLine("Hola ITQ!" + a);
        }

    public bool esVocal(char caracter) {
        String vocales = "AEIOUÁÉÍÓÚaeiouáéíóú";
        if(vocales.Contains(caracter)) {
            return true;
        }
        return false;
    }
        
     public void BorrarVocales(char caracter) {
        while(!fuente.EndOfStream) {
          char c = (char) fuente.Read();
            if(esVocal(c)) {
                objeto.Write(caracter);
            } else {
                objeto.Write(c);                
            }
        }
    }
    }
}