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
            int chooseNumberPages = 0;
            bool chooseSN = true;
            string chooseSNCond = string.Empty;
            //string url = "https://www.pokemon.com/us/pokemon-tcg/pokemon-cards/";
            TCGWebScraping tcgDois = new TCGWebScraping();
            var numberTotalPages = 845;//tcgDois.NumberOfPages();

            StringBuilder sbString = new StringBuilder();
            sbString.AppendLine("Caso de estudo - Compendio das informações simplificadas do Pokemon Trading Card Game, ");
            sbString.AppendLine("1 - Acesse o site https://www.pokemon.com/us/pokemon-tcg/pokemon-cards/");
            sbString.AppendLine("2 - Realize uma pesquisa sem preencher nenhum campo (Search)");
            sbString.Append("3 - Informe a quantidade de páginas (São ao todo "+ numberTotalPages + " paginas):");
            Console.Write(sbString);
            try
            {
                chooseNumberPages = int.Parse(Console.ReadLine());
                if (chooseNumberPages >= 1)
                {
                    Console.Write("4 - Salvar informacoes em um único arquivo? (S/N)");
                    chooseSNCond = Console.ReadLine();
                    if (chooseSNCond == "s" || chooseSNCond == "S")
                    {
                        chooseSN = true;
                    }
                    else if (chooseSNCond == "n" || chooseSNCond == "N")
                    {
                        chooseSN = false;
                    }
                    else
                    {
                        Console.WriteLine("Você digitou uma opção inválida, por favor tente novamente com uma opção válida!");
                    }

                    tcgDois.RunFlow(chooseNumberPages, chooseSN);

                }
                else
                {
                    Console.WriteLine("Você digitou uma opção inválida, por favor tente novamente com uma opção válida!");
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Erro ao ler do prompt.", ex);
                throw;
            }
            
        }
    }
}
