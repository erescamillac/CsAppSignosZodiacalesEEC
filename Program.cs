using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsAppSignosZodiacalesEEC
{

    /*
    * @author: Erick Escamilla Charco 
    * TODO: research on ncurses equivalent on C# for .NET Framework
    * research on Internationalization and CultureInfo:
    * System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");
    * 
    */

    // @author: Erick Escamilla Charco
    public class InputReader {
        // Default constructor
        public InputReader() {
        }

        public DateTime readDateTimeFromKeyboard(string message) {
            // DateTime is NOT nullable, use default value instead.
            // For more info, go to: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/default
            DateTime date = default;
            string dateAsString = "";
            bool error = false;

            do {
                try
                {
                    Console.WriteLine(message);
                    dateAsString = Console.ReadLine();
                    date = DateTime.Parse(dateAsString);
                    error = false;
                }
                catch (FormatException fE)
                {
                    Console.Error.WriteLine("La cadena {0} NO cumple con el FORMATO ESPECIFICADO, o es UNA FECHA INVÁLIDA.", dateAsString);
                    error = true;
                }
            } while (error);

            return date;
        }

    } //--FIN: InputReader
    
    public class App {

        // tuples array | arreglo de tuplas
        // Alternativas: uso de Diccionario, Listas o Arreglos de obj (obj.FechaFin, obj.SignoZodiacal)
        private static (DateTime FechaFin, string SignoZod)[] tablaSignosZod = { (DateTime.Parse("18/feb/2020"), "Acuario"),
                                                                                (DateTime.Parse("20/mar/2020"), "Piscis"),
                                                                                (DateTime.Parse("19/apr/2020"), "Aries"),
                                                                                (DateTime.Parse("20/may/2020"), "Tauro"),
                                                                                (DateTime.Parse("20/jun/2020"), "Géminis"),
                                                                                (DateTime.Parse("22/jul/2020"), "Cáncer"),
                                                                                (DateTime.Parse("22/aug/2020"), "Leo"),
                                                                                (DateTime.Parse("22/sep/2020"), "Virgo"),
                                                                                (DateTime.Parse("22/oct/2020"), "Libra"),
                                                                                (DateTime.Parse("21/nov/2020"), "Escorpio"),
                                                                                (DateTime.Parse("21/dec/2020"), "Sagitario"),
                                                                                (DateTime.Parse("19/jan/2020"), "Capricornio") };

        // empty constructor
        public App() {
        }

        public void execute() {
            InputReader inputReader = new InputReader();
            DateTime birthday;
            char continuar = 'n';
            string user = "", signoZodiacal = "";

            do {
                Console.Clear();
                Console.WriteLine("######--Astrología 101 by Erick Escamilla C.--######");
                Console.WriteLine("Introduzca su nombre: ");
                user = Console.ReadLine();

                birthday = inputReader.readDateTimeFromKeyboard("Ingrese su fecha de nacimiento (dd/mm/aaaa, ej. 25/12/2004, '25 de diciembre de 2004'): ");

                Console.WriteLine("--Intentando determinar signo zodiacal...");
                signoZodiacal = GetZodiacSign(birthday);
                Console.WriteLine("++Fin proceso de busqueda lineal de signo zodiacal...");

                Console.WriteLine("{0}, tu signo zodiacal es {1}.", user, signoZodiacal);

                Console.WriteLine("\t¿Desea determinar el Signo Zodiacal de otro usuario? [y/n]: ");
                continuar = Console.ReadKey().KeyChar;
            } while (Char.ToLower(continuar).Equals('y'));
        }//--FIN: execute()

        /* TODO: Mejorar con implementación por Búsqueda binaria, usar index sobre el Array de Tuplas "Tabla de signos"
         * considere la reordenación de la Tabla en orden cronológico DESC. Comenzar búsqueda contra el índice intermedio
         * [5] o [6] (fechas proximas a Junio / mediados de año)
         */
        private string GetZodiacSign(DateTime birthday) {
            // Emplear búsqueda lineal, primer match: rompe ciclo, break;
            string signoZodiacal = "";
            byte index = 0, prevIndex;
            bool found = false;
            int anioComp; // anio de Comparación, sobreescribe el anio ingresado por el usuario para comparar contra la Tabla de Signos Zodiacales
            DateTime fInicio, fFinal, birthdayComparacion;
            (DateTime fFinal, string signo) signoPrevio;

            // preformatear birthday para comparaciones contra la Tabla, 
            // Tabla de signos emplea 2020 como año de Referencia
            // IMPORTANTE!!! - Preformateo dede considerar que si el mes de birthday es Diciembre ENTONCES especificar 
            // birthdayComparacion con año 2019
            anioComp = birthday.Month == 12 && birthday.Day >=22 ? 2019 : 2020;
            birthdayComparacion = new DateTime(anioComp, birthday.Month, birthday.Day);

            foreach ((DateTime fFinal, string signo)entradaZodiacal in tablaSignosZod) {
                // 1.- Determinar fecha de inicio, para ello determinar index anterior.
                prevIndex = index == 0 ? (byte)11 : (byte)(index - 1);
                /*
                 * if(index == 0){
                 *      prevIndex = (byte)11;
                 * }else{
                 *      prevIndex = (byte)(index -1);
                 * }
                 */

                // 2.- Leer entrada anterior y calcular fecha de Inicio del signo actual, a partir de la fecha_fin del Signo Anterior
                signoPrevio = tablaSignosZod[prevIndex];
                fInicio = signoPrevio.fFinal.AddDays(1); // a la fecha final del Signo anterior se le suma 1 DÍA para calcular el dia de Inicio del signo actual
                fFinal = entradaZodiacal.fFinal;
                // 3. Validar MATCH
                // 3.1 IMPORTANTE !!! Detectar TRANSICIÓN CONFLICTIVA Dic -> Ene
                // fInicio (Diciembre) y fFinal (Enero)
                if (fInicio.Month == 12 && entradaZodiacal.fFinal.Month == 1) {
                    Console.WriteLine("Transición CONFLICTIVA Diciembre - Enero DETECTADA para Capricornio");
                    // mod fInicio (año 2019)
                    fInicio = new DateTime(fInicio.Year - 1, fInicio.Month, fInicio.Day);
                }

                // Después de la modificación a fFinal para la Transición CONFLICTIVA Dic - Ene, se puede proceder a 
                // comparación convencional.
                Console.WriteLine("Comparando: birthdayComp {0}, fInicio {1}, fFinal {2}", birthdayComparacion, fInicio, fFinal);
                if (birthdayComparacion >= fInicio && birthdayComparacion <= fFinal)
                {
                    Console.WriteLine("{0} >= {1} && {0} <= {2}", birthdayComparacion, fInicio, fFinal);
                    Console.WriteLine("Se hizo MATCH en {0}.", entradaZodiacal.signo);
                    found = true;
                    signoZodiacal = entradaZodiacal.signo;
                    break;
                }

                // al final de cada iteración
                index++;
            } //--FIN: foreach

            if (!found) {
                Console.WriteLine("[ERROR]: Signo Zodiacal NO Encontrado!");
                signoZodiacal = "Signo Zodiacal NO Encontrado en Tabla.";
            }

            return signoZodiacal;
        }
      
    } //--FIN: clase App

    class Program
    {
        static void Main(string[] args)
        {
            App app = new App();
            app.execute();
        }
    }
}
