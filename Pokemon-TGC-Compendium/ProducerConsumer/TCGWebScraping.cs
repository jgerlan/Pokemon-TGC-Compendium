﻿using HtmlAgilityPack;
using Pokemon_TGC_Compendium.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Pokemon_TGC_Compendium.ProducerConsumer
{
    class TCGWebScraping
    {
        static List<string> Produce(int nPages)
        {
            string mainUrl = "https://www.pokemon.com/us/pokemon-tcg/pokemon-cards/";
            string queryString = string.Format("?cardName=&cardText=&evolvesFrom=&simpleSubmit=&format=unlimited&hitPointsMin=0&hitPointsMax=340&retreatCostMin=0&retreatCostMax=5&totalAttackCostMin=0&totalAttackCostMax=5&particularArtist");
            HtmlWeb page = new HtmlWeb();
            HtmlNode.ElementsFlags.Remove("form");
            List<HtmlDocument> pages = new List<HtmlDocument>();
            List<string> listCardLinks = new List<string>();

            try
            {
                string tempUrl = mainUrl.Substring(0, 23);
                foreach (HtmlNode tagLink in page.Load(mainUrl + queryString).DocumentNode.SelectNodes("//div[@class=\"content-block content-block-full\"]/ul[@class=\"cards-grid clear\"]/li/a[@href]"))
                {
                    string linkCard = tagLink.GetAttributeValue("href", string.Empty);
                    listCardLinks.Add(tempUrl + linkCard);
                }
                if (nPages >= 2)
                    for (int i = 2; i <= nPages; i++)
                    {
                        foreach (HtmlNode tagLink in page.Load(mainUrl + i + queryString).DocumentNode.SelectNodes("//div[@class=\"content-block content-block-full\"]/ul[@class=\"cards-grid clear\"]/li/a[@href]"))
                        {
                            string linkCard = tagLink.GetAttributeValue("href", string.Empty);
                            listCardLinks.Add(tempUrl + linkCard);
                        }
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Erro ao carregar página(s).", ex);
                throw;
            }
            return listCardLinks;
        }

        public List<PokemonCard> MountCardInfo(List<string> listCardLinks)
        {
            HtmlWeb getCardInfoHtml = new HtmlWeb();
            List<PokemonCard> listPokemonCard = new List<PokemonCard>();


            foreach (string cardLink in listCardLinks)
            {
                PokemonCard newCardPokemonInfo = new PokemonCard();
                HtmlDocument pageCardInfo = getCardInfoHtml.Load(cardLink);//.DocumentNode.SelectNodes("//div[@class=\"content-block content-block-full\"]/ul[@class=\"cards-grid clear\"]/li/a[@href]");
                newCardPokemonInfo.numbering = pageCardInfo.DocumentNode.SelectSingleNode("//div[@class=\"stats-footer\"]/span").InnerText;
                newCardPokemonInfo.name = pageCardInfo.DocumentNode.SelectSingleNode("//div[@class=\"card-description\"]/div[@class=\"color-block color-block-gray\"]/h1").InnerText;
                newCardPokemonInfo.expansion = pageCardInfo.DocumentNode.SelectSingleNode("//div[@class=\"stats-footer\"]/h3/a").InnerText;
                newCardPokemonInfo.urlImage = pageCardInfo.DocumentNode.SelectSingleNode("//div[@class=\"column-6 push-1\"]/div[@class=\"content-block content-block-full card-image\"]/img").GetAttributeValue("src", string.Empty);
                listPokemonCard.Add(newCardPokemonInfo);
            }


            return listPokemonCard;
        }

        public void ConsumerCardInfo(List<PokemonCard> listPokemonCard)
        {

        }

        public void RunFlow(int nPages)
        {
            PokemonCard teste = new PokemonCard()
            {
                numbering = "1/73 Common",
                name = "Bulbassauro",
                expansion = "Shining Legends",
                urlImage = "https://assets.pokemon.com/assets/cms2/img/cards/web/SM35/SM35_EN_1.png"
            };
            List<PokemonCard> listTeste = new List<PokemonCard>();
            listTeste.Add(teste);
            //List<string> listCardLink = Produce(nPages);
            //MountCardInfo(teste);
            ConsumerCardInfo(listTeste);

        }
    }
}
