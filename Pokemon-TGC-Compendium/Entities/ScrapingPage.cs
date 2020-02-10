using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace Pokemon_TGC_Compendium.Entities
{
    class ScrapingPage
    {
        public string url { get; set; }
        public string primarySelectNode { get; set; }

        public ScrapingPage()
        {
        }

        public ScrapingPage(string url, string primarySelectNode)
        {
            this.url = url;
            this.primarySelectNode = primarySelectNode;
        }

        public void RunGetPages()
        {
            Thread work = new Thread(() => GetPages());
            work.Start();
        }

        public virtual void GetPages()
        {
            Console.WriteLine("Starting...\n");

            string source = GetSource(url);

            if (source == "") return;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(source);

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(primarySelectNode);

            foreach (HtmlNode node in nodes)
            {
                Console.WriteLine(node.InnerText);
            }
        }

        string GetSource(string url)
        {
            try
            {
                WebClient myWebClient = new WebClient();
                myWebClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                byte[] myDataBuffer = myWebClient.DownloadData(url);
                string source = Encoding.ASCII.GetString(myDataBuffer);

                return source;
            }
            catch (Exception ex)
            {
                return "";
                //throw;
            }


        }

    }
}
