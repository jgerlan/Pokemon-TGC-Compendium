using System;
using System.Text;

namespace Pokemon_TGC_Compendium
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            char chooseSN = 'n';

            StringBuilder sbString = new StringBuilder();
            sbString.AppendLine("Caso de estudo - Compendio das informações simplificadas do Pokemon Trading Card Game, ");
            sbString.AppendLine("capturado via web scraping no site do respectivo game.");
            sbString.Append("Deseja iniciar extração dos dados? (S/N): ");
            Console.Write(sbString);

            chooseSN = char.Parse(Console.ReadLine());

            if(chooseSN=='n')
            {
                Console.WriteLine("Obrigado!\n");
            }
            else
            {
                new RunScrapingPage();
            }
        }
    }
}
