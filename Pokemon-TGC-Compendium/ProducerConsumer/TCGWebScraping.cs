using HtmlAgilityPack;
using Newtonsoft.Json;
using Pokemon_TGC_Compendium.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;
using Pokemon_TGC_Compendium.Business;

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

            List<string> listLinkPages = new List<string>();
            listLinkPages.Add(mainUrl + queryString);
            if (nPages>1)
            {
                for (int i = 2; i <= nPages; i++)
                {
                    listLinkPages.Add(mainUrl + i + queryString);
                }
            }

            try
            {
                string tempUrl = mainUrl.Substring(0, 23);

                foreach (string link in listLinkPages)
                {
                    foreach (HtmlNode tagLink in page.Load(link).DocumentNode.SelectNodes("//div[@class=\"content-block content-block-full\"]/ul[@class=\"cards-grid clear\"]/li/a[@href]"))
                    {
                        string linkCard = tagLink.GetAttributeValue("href", string.Empty);
                        listCardLinks.Add(tempUrl + linkCard);
                    }
                }
                /*
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
                    }*/
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
            //Parallel.ForEach(listCardLinks, (cardLink) =>
            {
                //Console.WriteLine($"Processing {cardLink} on thread {Thread.CurrentThread.ManagedThreadId}");

                PokemonCard newCardPokemonInfo = new PokemonCard();
                HtmlDocument pageCardInfo = getCardInfoHtml.Load(cardLink);//.DocumentNode.SelectNodes("//div[@class=\"content-block content-block-full\"]/ul[@class=\"cards-grid clear\"]/li/a[@href]");
                newCardPokemonInfo.numbering = pageCardInfo.DocumentNode.SelectSingleNode("//div[@class=\"stats-footer\"]/span").InnerText;
                newCardPokemonInfo.name = pageCardInfo.DocumentNode.SelectSingleNode("//div[@class=\"card-description\"]/div[@class=\"color-block color-block-gray\"]/h1").InnerText;
                newCardPokemonInfo.expansion = pageCardInfo.DocumentNode.SelectSingleNode("//div[@class=\"stats-footer\"]/h3/a").InnerText;
                newCardPokemonInfo.urlImage = pageCardInfo.DocumentNode.SelectSingleNode("//div[@class=\"column-6 push-1\"]/div[@class=\"content-block content-block-full card-image\"]/img").GetAttributeValue("src", string.Empty);

                System.Net.WebRequest request = System.Net.WebRequest.Create(newCardPokemonInfo.urlImage);
                System.Net.WebResponse response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();

                Image bm = new Bitmap(responseStream);
                ImageFormat format = ImageFormat.Png;

                using (MemoryStream ms = new MemoryStream())
                {
                    bm.Save(ms, format);
                    newCardPokemonInfo.codedImage = Convert.ToBase64String(ms.ToArray());
                }

                listPokemonCard.Add(newCardPokemonInfo);
            }//);


            return listPokemonCard;
        }

        public void ConsumerCardInfo(List<PokemonCard> listPokemonCard, bool chooseSingleFile)
        {
            string pokecardInfo = string.Empty;
            if (chooseSingleFile)
            {
                pokecardInfo = JsonConvert.SerializeObject(listPokemonCard, Formatting.Indented);
                PokemonCardBUS pokemonBUS = new PokemonCardBUS();
                pokemonBUS.pokemonCardDAO.CreatePokemonCardInfoFile(pokecardInfo, "PokemonCardInfoCompedium" + "_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
            }
            else
            {
                int numberFile = 0;
                foreach (var cardInfo in listPokemonCard)
                {
                    numberFile++;
                    pokecardInfo = JsonConvert.SerializeObject(cardInfo, Formatting.Indented);
                    PokemonCardBUS pokemonBUS = new PokemonCardBUS();
                    string nameFileCardInfo = cardInfo.name +"_"+numberFile;
                    pokemonBUS.pokemonCardDAO.CreatePokemonCardInfoFile(pokecardInfo, nameFileCardInfo);
                }
            }
        }

        public void RunFlow(int nPages, bool chooseSN)
        {
            /*PokemonCard teste = new PokemonCard()
            {
                numbering = "1/73 Common",
                name = "Bulbassauro",
                expansion = "Shining Legends",
                urlImage = "https://assets.pokemon.com/assets/cms2/img/cards/web/SM35/SM35_EN_1.png"
            };
            List<PokemonCard> listTeste = new List<PokemonCard>();
            listTeste.Add(teste);
            List<string> testeLinkImage = new List<string>()
            {
                "https://www.pokemon.com/us/pokemon-tcg/pokemon-cards/sm-series/sm35/1/",
                "https://www.pokemon.com/us/pokemon-tcg/pokemon-cards/ex-series/ex4/46/",
                "https://www.pokemon.com/us/pokemon-tcg/pokemon-cards/sm-series/sma/SV6/"
            };*/

            List<string> returnProduce;
            List<PokemonCard> returnMountCardInfo;


            returnProduce = Produce(nPages);
            returnMountCardInfo = MountCardInfo(returnProduce);
            ConsumerCardInfo(returnMountCardInfo, chooseSN);

        }

        public int NumberOfPages()
        {
            string mainUrl = "https://www.pokemon.com/us/pokemon-tcg/pokemon-cards/";
            string queryString = string.Format("?cardName=&cardText=&evolvesFrom=&simpleSubmit=&format=unlimited&hitPointsMin=0&hitPointsMax=340&retreatCostMin=0&retreatCostMax=5&totalAttackCostMin=0&totalAttackCostMax=5&particularArtist");
            HtmlWeb page = new HtmlWeb();
            string rangePages = page.Load(mainUrl + queryString).DocumentNode.SelectSingleNode("//div[@id=\"cards-load-more\"]/div/span").InnerText;
            int numberPages;

            try
            {
                numberPages = int.Parse(rangePages.Substring(5));
                return numberPages;
            }
            catch (Exception)
            {
                throw;
            }
   
        }
    }
}
