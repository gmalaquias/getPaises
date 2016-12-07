using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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

                if(letras != null)
                    foreach(var l in letras)
                    {
                        foreach (var p in l.FindElements(By.TagName("a")))
                        {
                            paises_link.Add(p.GetAttribute("href"));
                            Console.WriteLine(p.Text);
                        }
                    }

                foreach(var p in paises_link)
                {
                    driver.Navigate().GoToUrl(p);
                    var nome = driver.FindElementByXPath("//*[@id=\"divider\"]/h1").Text;
                    var capital = driver.FindElementByXPath("//*[@id=\"divider\"]/div[2]/ul[1]/li[3]").Text;
                    var url_img = driver.FindElementByXPath("//*[@id=\"divider\"]/div[1]/img").GetAttribute("src");
                    var nomeImg = Guid.NewGuid();

                    using (WebClient client = new WebClient())
                        client.DownloadFile(new Uri(url_img), @"c:\users\gabriel.malaquias\temp\"+nomeImg+".png");
                }

                Console.ReadKey();
            }
        }
    }
}
