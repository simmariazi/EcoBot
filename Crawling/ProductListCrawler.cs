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

            for (int i = 0; i < jobs.Count; i++)
            {
                GetRepacUrl(jobs[i].url);
            }
        }
        // productList를 실행하고, 데이터테이블(product_list)을 비교한다
        // 잡테이블에서
        // 셀러 아이디가 1인 애들중에
        // productUrl을 비교해야해
        // 데이터테이블(product_list)에 있으면 넘어가고 (추가하지않고)
        // 데이터테이블(product_list)에 없으면 신규 추가한다.
        // 근데 productList에 없으면 데이터테이블(product_list)에도 제거(노출을 안한다)한다.업데이트(is_used y,n)
        // 일정 주기마다 반복한다.

        public void GetRepacUrl(string url)
        {
            List<ProductList> productList = new List<ProductList>();
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

                try
                {
                    driver.Url = url;
                    driver.Navigate();
                    Thread.Sleep(300);
                    string productNodeXpath = "//*[@id='container_w202004085670d4034bd2d']/div";

                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(driver.PageSource);

                    var products = document.DocumentNode.SelectNodes(productNodeXpath);

                    List<ProductList> confirm = (new Repositories()).GetProductListsById(1);


                    ProductList product = new ProductList();
                    for (int j = 0; j < products.Count; j++)
                    {
                        product = new ProductList()
                        {
                            thumbnail = products[j].SelectNodes("//img[@class='_org_img org_img _lazy_img']")[j].GetAttributeValue("src", ""),
                            productUrl = "https://repac.co.kr" + products[j].SelectNodes("//div[@class='item-wrap']/a")[j].GetAttributeValue("href", ""),
                            seller_id = 1,

                        };
                        int ignore = 0;
                        for (int i = 0; i < confirm.Count; i++)
                        {
                            // 둘다 있어 > 추가하지않고 넘어감
                            if (product.productUrl == confirm[i].productUrl)
                            {
                                ignore = 1;
                                break;
                            }
                        }



                        //데이터테이블(product_list)에 없으면 신규 추가한다.
                        if (ignore == 0)
                        {
                            productList.Add(product);
                        }

                    }
                }
                catch
                {

                }


            }

            
            //근데 productList에 없으면 데이터테이블(product_list)에도 제거(노출을 안한다)한다.업데이트(is_used y,n)
            //    if (product.productUrl != confirm[i].productUrl)
            //    {
            // 저장
            (new Repositories()).AddProductList(productList);
        }


    }
}

