using HtmlAgilityPack;
using System;
using System.Threading;


namespace Pokemon_TGC_Compendium
{
    class RunScrapingPage
    {
        string url = "https://www.pokemon.com/us/pokemon-tcg/pokemon-cards/";

        public RunScrapingPage()
        {

        }
        private void RunGetPages()
        {
            Thread work = new Thread(() => GetPages());
            work.Start();
        }

        void GetPages()
        {
            Console.WriteLine("Starting...\n");

            string source = GetSource(url);
            
            if (source == "") return;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(source);

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//button[@id=simpleSubmit]");

            foreach (HtmlNode node in nodes)
            {
                Console.WriteLine(node.InnerText);
            }
        }

        string GetSource (string url)
        {
            try
            {
                return "";
            }
            catch (Exception ex)
            {
                return "";
                //throw;
            }

            
        }
    }
}