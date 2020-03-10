using Pokemon_TGC_Compendium.Entities;
using Pokemon_TGC_Compendium.ProducerConsumer;
using System;
using System.Text;

namespace Pokemon_TGC_Compendium
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            int chooseSN;
            string nodeButtonTCG = string.Empty;
            //string url = "https://www.pokemon.com/us/pokemon-tcg/pokemon-cards/";

            StringBuilder sbString = new StringBuilder();
            sbString.AppendLine("Caso de estudo - Compendio das informações simplificadas do Pokemon Trading Card Game, ");
            sbString.AppendLine("1 - Acesse o site https://www.pokemon.com/us/pokemon-tcg/pokemon-cards/");
            sbString.AppendLine("2 - Realize uma pesquisa sem preencher nenhum campo (Search)");
            sbString.AppendLine("3 - Informa abaixo a quantidade de páginas:");
            Console.Write(sbString);

            chooseSN = int.Parse(Console.ReadLine());

            TCGWebScraping tcgDois = new TCGWebScraping();
            tcgDois.RunFlow(chooseSN);

        }
    }
}
