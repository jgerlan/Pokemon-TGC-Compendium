using HtmlAgilityPack;
using Newtonsoft.Json;
using Pokemon_TGC_Compendium.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Pokemon_TGC_Compendium.Business;

namespace Pokemon_TGC_Compendium.ProducerConsumer
{
    class TGCWebScrapingProduceConsumer
    {
        private string mainUrl = "https://www.pokemon.com/us/pokemon-tcg/pokemon-cards/";
        private BlockingCollection<string> queueProduceToMount { get; set; }
        private BlockingCollection<PokemonCard> queueMountToConsumer { get; set; }

        public void RunFlow(int nPages, bool chooseSN)
        {
            queueProduceToMount = new BlockingCollection<string>(12);
            queueMountToConsumer = new BlockingCollection<PokemonCard>(12);

            //string mainUrl = "https://www.pokemon.com/us/pokemon-tcg/pokemon-cards/";

            HtmlWeb page = new HtmlWeb();
            HtmlNode.ElementsFlags.Remove("form");
            List<HtmlDocument> pages = new List<HtmlDocument>();
            List<string> listCardLinks = new List<string>();
            List<string> listLinkPages = ListPages(nPages);

            Task Produce = Task.Run(() =>
            {
                try
                {
                    string tempUrl = mainUrl.Substring(0, 23);

                    foreach (string link in listLinkPages)
                    {
                        var pagTemp = page.Load(link).DocumentNode.SelectNodes("//div[@class=\"content-block content-block-full\"]/ul[@class=\"cards-grid clear\"]/li/a[@href]");
                        Parallel.ForEach(pagTemp, (tagLink) =>
                        //foreach (HtmlNode tagLink in page.Load(link).DocumentNode.SelectNodes("//div[@class=\"content-block content-block-full\"]/ul[@class=\"cards-grid clear\"]/li/a[@href]"))
                        {
                            string linkCard = tagLink.GetAttributeValue("href", string.Empty);
                            //listCardLinks.Add(tempUrl + linkCard);
                            queueProduceToMount.Add(tempUrl + linkCard);
                            Console.WriteLine("Add item {0} on queueProduceToMount.", tempUrl + linkCard);
                        });
                    }
                    queueProduceToMount.CompleteAdding();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0} Erro ao carregar página(s).", ex);
                    throw;
                }
            });

            Task Mount = Task.Run(() =>
            {
                HtmlWeb getCardInfoHtml = new HtmlWeb();
                //List<PokemonCard> listPokemonCard = new List<PokemonCard>();

                //foreach (string cardLink in queueProduceToMount.GetConsumingEnumerable())
                Parallel.ForEach(queueProduceToMount.GetConsumingEnumerable(), (cardLink) =>
                {
                    //Console.WriteLine($"Processing {cardLink} on thread {Thread.CurrentThread.ManagedThreadId}");
                    Console.WriteLine("Took out item {0} of queueProduceToMount.", cardLink);

                    PokemonCard newCardPokemonInfo = new PokemonCard();
                    HtmlDocument pageCardInfo = getCardInfoHtml.Load(cardLink);
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

                    //listPokemonCard.Add(newCardPokemonInfo);
                    queueMountToConsumer.Add(newCardPokemonInfo);
                    Console.WriteLine("Add item {0} on queueMountToConsumer", newCardPokemonInfo.numbering);
                });
                queueMountToConsumer.CompleteAdding();

            });

            Task Consume = Task.Run(() =>
            {
                string pokecardInfo = string.Empty;
                string data = DateTime.Now.ToString("yyyyMMddHHmmss");
                PokemonCardBUS pokemonBUS = new PokemonCardBUS();

                if (chooseSN)
                {
                    pokemonBUS.pokemonCardDAO.CreatePokemonCardInfoFile("[", "PokemonCardInfoCompedium" + "_" + data);
                    //await pokemonBUS.pokemonCardDAO.CreatePokemonCardInfoFileAsync("[", "PokemonCardInfoCompedium" + "_" + data, TimeSpan.FromSeconds(2), tryCount:10);

                    //foreach (var pokecardInfoObj in queueMountToConsumer.GetConsumingEnumerable())
                    Parallel.ForEach(queueMountToConsumer.GetConsumingEnumerable(), async (pokecardInfoObj) =>
                    {
                        pokecardInfo = JsonConvert.SerializeObject(pokecardInfoObj, Formatting.Indented);
                        string virg = ",\n";
                        if (queueMountToConsumer.IsCompleted)
                        {
                            virg = "\n";
                        }
                        //pokemonBUS.pokemonCardDAO.CreatePokemonCardInfoFile(pokecardInfo + virg, "PokemonCardInfoCompedium" + "_" + data);
                        await pokemonBUS.pokemonCardDAO.CreatePokemonCardInfoFileAsync(pokecardInfo + virg, "PokemonCardInfoCompedium" + "_" + data);

                    });
                    pokemonBUS.pokemonCardDAO.CreatePokemonCardInfoFile("]", "PokemonCardInfoCompedium" + "_" + data);
                }
                else
                {
                    int numberFile = 0;
                    //foreach (var cardInfo in queueMountToConsumer.GetConsumingEnumerable())
                    Parallel.ForEach(queueMountToConsumer.GetConsumingEnumerable(), (pokecardInfoObj) =>
                    {
                        numberFile++;
                        pokecardInfo = JsonConvert.SerializeObject(pokecardInfoObj, Formatting.Indented);
                        //PokemonCardBUS pokemonBUS = new PokemonCardBUS();
                        string nameFileCardInfo = pokecardInfoObj.name + "_" + numberFile;
                        pokemonBUS.pokemonCardDAO.CreatePokemonCardInfoFile(pokecardInfo, nameFileCardInfo);
                    });
                }

            });

            Task.WaitAll(Produce, Mount, Consume);
        }

        public List<string> ListPages(int nPages)
        {
            //string mainUrl = "https://www.pokemon.com/us/pokemon-tcg/pokemon-cards/";
            string queryString = string.Format("?cardName=&cardText=&evolvesFrom=&simpleSubmit=&format=unlimited&hitPointsMin=0&hitPointsMax=340&retreatCostMin=0&retreatCostMax=5&totalAttackCostMin=0&totalAttackCostMax=5&particularArtist");

            List<string> listLinkPages = new List<string>();
            listLinkPages.Add(mainUrl + queryString);
            if (nPages > 1)
            {
                for (int i = 2; i <= nPages; i++)
                {
                    listLinkPages.Add(mainUrl + i + queryString);
                }
            }

            return listLinkPages;
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
