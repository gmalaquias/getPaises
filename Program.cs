using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace getPaises
{
    class Program
    {
        public static string LOCAL_DRIVER = "c:/";
        public static string URL_MAPS = "http://pt.knoema.com/atlas";

        static void Main(string[] args)
        {
            //instancia o phantom para rodar por tras as requisições
            using (var driver = new ChromeDriver(LOCAL_DRIVER))
            {
                //abre o site da NFSe
                driver.Navigate().GoToUrl(URL_MAPS);

                var letras = driver.FindElementsByClassName("letter");

                List<string> paises_link = new List<string>();

                if (letras != null)
                    foreach (var l in letras)
                    {
                        foreach (var p in l.FindElements(By.TagName("a")))
                        {
                            paises_link.Add(p.GetAttribute("href"));
                            Console.WriteLine(p.Text);
                        }
                    }


                List<dados> dados = new List<getPaises.dados>();

                foreach (var p in paises_link)
                {
                    driver.Navigate().GoToUrl(p);
                    var d = new getPaises.dados();

                    var nome = driver.FindElementByXPath("//*[@id=\"divider\"]/h1");
                    d.nome = nome.Text;

                    var url_img = driver.FindElementByXPath("//*[@id=\"divider\"]/div[1]/img").GetAttribute("src");
                    d.imagem = Guid.NewGuid().ToString() + ".png";

                    var presidente = driver.FindElementByXPath("//*[@id=\"divider\"]/div[2]/ul[1]/li[1]");
                    d.nPresidente = presidente.FindElement(By.TagName("span")).Text.Replace(":", "");
                    d.presidente = presidente.Text.Replace(presidente.FindElement(By.TagName("span")).Text, "");

                    var vice_presidente = driver.FindElementByXPath("//*[@id=\"divider\"]/div[2]/ul[1]/li[2]");
                    d.nVice_presidente = vice_presidente.FindElement(By.TagName("span")).Text.Replace(":", "");
                    d.vice_presidente = vice_presidente.Text.Replace(vice_presidente.FindElement(By.TagName("span")).Text, "");

                    try
                    {
                        var populacao_ano = driver.FindElementByXPath("//*[@id=\"divider\"]/div[2]/ul[2]/li[1]/small");
                        d.populacao_ano = populacao_ano.Text.Replace("(", "").Replace(")", "");

                        var populacao = driver.FindElementByXPath("//*[@id=\"divider\"]/div[2]/ul[2]/li[1]");
                        d.populacao = populacao.Text.Replace(populacao.FindElement(By.TagName("span")).Text, "").Replace("(" + d.populacao_ano + ")", "").Replace(".", "").Trim();
                    }
                    catch
                    {
                        d.populacao = "0";
                        d.populacao_ano = "Sem dados";
                    }

                    try
                    {
                        var area_ano = driver.FindElementByXPath("//*[@id=\"divider\"]/div[2]/ul[2]/li[2]/small");
                        d.area_ano = area_ano.Text.Replace("(", "").Replace(")", "");

                        var area = driver.FindElementByXPath("//*[@id=\"divider\"]/div[2]/ul[2]/li[2]");
                        d.area = area.Text.Replace(area.FindElement(By.TagName("span")).Text, "").Replace("(" + d.area_ano + ")", "").Replace(".", "").Trim();
                    }
                    catch
                    {
                        d.area_ano = "Sem dados";
                        d.area = "0";
                    }

                    try
                    {
                        var pib_ano = driver.FindElementByXPath("//*[@id=\"divider\"]/div[2]/ul[2]/li[3]/small");
                        d.PIB_percapita_ano = pib_ano.Text.Replace("(", "").Replace(")", "");

                        var pib = driver.FindElementByXPath("//*[@id=\"divider\"]/div[2]/ul[2]/li[3]");
                        d.PIB_percapita = pib.Text.Replace(pib.FindElement(By.TagName("span")).Text, "").Replace("(" + d.PIB_percapita_ano + ")", "").Replace(".", "").Trim();
                    }
                    catch
                    {
                        d.PIB_percapita_ano = "Sem dados";
                        d.PIB_percapita = "0";
                    }

                    try
                    {
                        var gini_ano = driver.FindElementByXPath("//*[@id=\"divider\"]/div[2]/ul[2]/li[5]/small");
                        d.gini_ano = gini_ano.Text.Replace("(", "").Replace(")", "");

                        var gini = driver.FindElementByXPath("//*[@id=\"divider\"]/div[2]/ul[2]/li[5]");
                        d.gini = gini.Text.Replace(gini.FindElement(By.TagName("span")).Text, "").Replace("(" + d.gini_ano + ")", "").Trim();
                    }
                    catch
                    {
                        d.gini_ano = "Sem dados";
                        d.gini = "0";
                    }

                    try
                    {
                        var facilidade_ano = driver.FindElementByXPath("//*[@id=\"divider\"]/div[2]/ul[2]/li[6]/small");
                        d.facilicade_negocios_ano = facilidade_ano.Text.Replace("(", "").Replace(")", "");

                        var facilidade = driver.FindElementByXPath("//*[@id=\"divider\"]/div[2]/ul[2]/li[6]");
                        d.facilicade_negocios = facilidade.Text.Replace(facilidade.FindElement(By.TagName("span")).Text, "").Replace("(" + d.facilicade_negocios_ano + ")", "").Trim();
                    }
                    catch
                    {
                        d.facilicade_negocios_ano = "Sem dados";
                        d.facilicade_negocios = "0";
                    }

                    using (WebClient client = new WebClient())
                        client.DownloadFile(new Uri(url_img), @"c:\users\gabriel.malaquias\desktop\paises\img\bandeiras\" + d.imagem);

                    var capital = driver.FindElementByXPath("//*[@id=\"divider\"]/div[2]/ul[1]/li[3]");
                    d.capital = capital.Text.Replace(capital.FindElement(By.TagName("span")).Text, "");

                    try
                    {
                        driver.Navigate().GoToUrl("https://www.google.com.br/webhp?sourceid=chrome-instant&ion=1&espv=2&ie=UTF-8#q=capital+" + d.nome.Replace(" ", "+"));
                        d.capital = driver.FindElementByXPath("//*[@id=\"uid_0\"]/div[1]/div[2]/div[2]/div/div[2]/div/div/div[1]").Text;
                    }
                    catch { }

                    d.saiba_mais = p;

                    try
                    {
                        Thread.Sleep(2000);
                        d.imgMapa = d.imagem;
                        var imgMapa = driver.FindElementByXPath("//*[@id=\"lu_map\"]").GetAttribute("src");
                        using (WebClient client = new WebClient())
                        {
                            client.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
                            client.DownloadFile(new Uri(imgMapa), @"c:\users\gabriel.malaquias\desktop\paises\img\mapa\" + d.imgMapa);
                        }
                    }
                    catch(Exception e)
                    {
                        d.imgMapa = "";
                    }

                    dados.Add(d);
                }

                System.IO.File.WriteAllText(@"c:\users\gabriel.malaquias\desktop\paises\dados.json", JsonConvert.SerializeObject(dados));

                Console.ReadKey();
            }
        }
    }

    public class dados
    {
        public string nome { get; set; }

        public string capital { get; set; }

        public string imagem { get; set; }

        public string saiba_mais { get; set; }

        public string nPresidente { get; set; }

        public string presidente { get; set; }

        public string nVice_presidente { get; set; }

        public string vice_presidente { get; set; }

        public string populacao { get; set; }

        public string populacao_ano { get; set; }

        public string area { get; set; }

        public string area_ano { get; set; }

        public string PIB_percapita { get; set; }

        public string PIB_percapita_ano { get; set; }

        public string gini { get; set; }

        public string gini_ano { get; set; }

        public string facilicade_negocios { get; set; }

        public string facilicade_negocios_ano { get; set; }

        public string imgMapa { get; set; }

    }

}


