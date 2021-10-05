using HtmlAgilityPack;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using WebScraping.Models;

namespace WebScraping.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string url = "https://www.vivareal.com.br/venda/sp/ribeirao-preto/?utm_source=google&utm_medium=cpc&utm_campaign=Institucional-Regioes-VivaReal&gclid=CjwKCAiA1eKBBhBZEiwAX3gql4MqwyZKbFyjAcktrLVckdOseyt035DOUhmeD9cY7KmfangiueyGSBoCKDAQAvD_BwE&utm_referrer=https%3A%2F%2Fwww.google.com%2F";
            var response = FindUrl(url).Result;
            var list = ParseHtml(response);
            WriteToTxt(list);
            return View();
        }

        private static async Task<string> FindUrl(string url)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetStringAsync(url);

            return await response;
        }

        private List<Result> ParseHtml(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            // Criação das variáveis com as tags que contém as informações desejadas
            var addresses = htmlDoc.DocumentNode.Descendants("span")
                .Where(node => node.GetAttributeValue("class", "") == "property-card__address")
                .Select(x => x.InnerHtml)
                .ToList();

            var titles = htmlDoc.DocumentNode.Descendants("span")
               .Where(node => node.GetAttributeValue("class", "")
               .Contains("property-card__title"))
               .Select(x => x.InnerHtml)
                              .ToList();

            var prices = htmlDoc.DocumentNode.Descendants("p")
             .Where(node => node.GetAttributeValue("style", "")
             .Contains("display: block;"))
             .Select(x => x.InnerHtml)
             .ToList();

            // Atribuição dos valores obtidos na classe Result
            var results = new List<Result>();

            for (int i = 0; i < titles.Count; i++)
            {
                string address;
                string price;

                if (i >= addresses.Count)
                    address = "Não possui";
                else
                    address = addresses[i];

                if (i >= prices.Count)
                    price = "Não possui";
                else
                    price = prices[i];

                // Valida se existe valores para as propriedades
                var result = new Result(address, titles[i], price);

                results.Add(result);
            }

            return results;
        }

        private void WriteToTxt(List<Result> results)
        {
            var sb = new StringBuilder();
            foreach (var result in results)
            {
                sb.AppendLine($"Descrição: {result.Title} \n Endereço: {result.Address}. \n Valor: {result.Price} \n");
            }

            System.IO.File.WriteAllText("results.txt", sb.ToString());
        }
    }
}
