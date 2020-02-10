using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;

namespace Pokemon_TGC_Compendium.Entities
{
    class TCGScrapingPage : ScrapingPage
    {
        public int qtdPages { get; set; }
        public string uriForm { get; set; }

        public TCGScrapingPage()
        {
        }

        public TCGScrapingPage(string url, string primarySelectNode, int qtdPages)
            : base(url, primarySelectNode)
        {
            this.qtdPages = qtdPages;
        }

        public override void GetPages()
        {
            Console.WriteLine("Starting...\n");
            this.uriForm = "https://www.pokemon.com/us/pokemon-tcg/pokemon-cards/";

            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load("https://www.pokemon.com/us/pokemon-tcg/pokemon-cards/?cardName=&cardText=&evolvesFrom=&simpleSubmit=&format=unlimited&hitPointsMin=0&hitPointsMax=340&retreatCostMin=0&retreatCostMax=5&totalAttackCostMin=0&totalAttackCostMax=5&particularArtist=");
            var nodes = doc.DocumentNode.SelectNodes("//div[@class=\"column-12 push-1 card-results-anchor\"]/div[@class=\"content-block content-block-full\"]/ul[@class=\"cards-grid clear\"]/li/a");//.ToList();
                        
            foreach (HtmlAgilityPack.HtmlNode node in nodes)
            {
                //Console.WriteLine(node.InnerText);
                HtmlNode divNode = node.SelectSingleNode("//div[@class=\"content-block content-block-full animating\"]");
            }
            /*
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string HtmlResult = wc.UploadString(uriForm, "?cardName=&cardText=&evolvesFrom=&simpleSubmit=&format=unlimited&hitPointsMin=0&hitPointsMax=340&retreatCostMin=0&retreatCostMax=5&totalAttackCostMin=0&totalAttackCostMax=5&particularArtist=");
            }*/
        }
    }
}
