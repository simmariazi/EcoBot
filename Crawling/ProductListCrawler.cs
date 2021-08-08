using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EcoBot.DB;
using EcoBot.Model;
using HtmlAgilityPack;
using MySql.Data.MySqlClient;
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


        //잡 테이블에 있는 데이터불러오기

        //잡에있는 데이터 중 리팩의 url만 불러오ㅑ
        public void RepacUrl()
        {
            //url = job의 셀러아이디 1 
            // 잡 가져오기
            List<Job> jobs = (new Repositories()).GetJobs(1);

            ProductListCrawler getRepac = new ProductListCrawler();

            for (int i = 0; i < jobs.Count; i++)
            {
                getRepac.GetRepacUrl(jobs[i].url);
            }
        }

        public void GetRepacUrl(string url)
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
                                thumbnail = products[j].SelectNodes("//img")[j].GetAttributeValue("src", ""),
                                productUrl = "https://re-ground.co.kr/" + products[j].SelectNodes("//div/a")[j].GetAttributeValue("href", ""),

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

            // 저장
            (new Repositories()).AddProductList(productList);
        }
    }
}

