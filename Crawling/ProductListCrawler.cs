using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace EcoBot.Crawling
{
    public class ProductListCrawler : BaseCrawler
    {
        //public void memo()
        //{
        //    // 상속 받은 BaseCrawler 클래스에서 InheritanceExample 함수 사용 예시
        //    string sample = InheritanceExample("랄라");

        //    // 의미 없는 for문 테스트
        //    for (int i = 0; i < sample.Length; i++)
        //    {
        //        Console.WriteLine(sample);
        //    }
        //}
        public void GetUrl(string url)
        {
           
            List<ProductList> productList = new List<ProductList>();
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                for (int i = 1; ; i++)
                {
                    try
                    {     
                        driver.Url = url;
                        driver.Navigate();
                        Thread.Sleep(300);
                        string productNodeXpath = "//*[@id='w201808105b6c62847f539']/div/div";

                        HtmlDocument document = new HtmlDocument();
                        document.LoadHtml(driver.PageSource);

                        var products = document.DocumentNode.SelectNodes(productNodeXpath);

                        if (products == null)
                        {
                            break;
                        }

                        ProductList product = new ProductList();
                        for (int j = 0; j < products.Count; j++)
                        {
                            product = new ProductList()
                            {
                                Thumnail = products[j].SelectNodes("//img")[j].GetAttributeValue("src", ""),
                                ProductUrl = "https://re-ground.co.kr/" + products[j].SelectNodes("//div/a")[j].GetAttributeValue("href", ""),

                            };

                            productList.Add(product);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

            }
        }

    }
    
   
}

