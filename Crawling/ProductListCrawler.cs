using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        public IEnumerable<Job> GetJobList()
        {

            DataSet dsResult = new DataSet();
            // 데이터를 불러온다
            // DB 연결
            // DB 연결 정보
            string connectionString = "Server=121.166.4.186;Port=3152;Database=eco_bot;Uid=root;Pwd=rladndwo3";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();


                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM job", connection);

                da.Fill(dsResult);


            }
            List<Job> job = new List<Job>();
            foreach (DataRow data in dsResult.Tables[0].Rows)
            {
                job.Add(new Job
                {
                    id = int.Parse(data["id"].ToString()),
                    seller_id = int.Parse(data["seller_id"].ToString()),
                    category_id = int.Parse(data["category_id"].ToString()),
                    url = data["url"].ToString(),



                });
            }


            return job;
        }


        //잡에있는 데이터 중 리팩의 url만 불러오ㅑ
        public string RepacUrl(string url)
        {
            //url = job의 셀러아이디 1 
            
            ProductListCrawler getRepac = new ProductListCrawler();
            {

                
                //for (int i = 0; i < job.Count; i++)
                //{
                //    if (job.seller_id == 1)
                //    {
                        
                //    }
                //}
            }
            getRepac.GetRepacUrl(url);
            return url;
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

