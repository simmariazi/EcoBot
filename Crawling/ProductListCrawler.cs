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
            List<Job> jobs = (new Repositories()).GetJobs(1);

            for (int i = 0; i < jobs.Count; i++)
            {
                GetRepacUrl(jobs[i].url, jobs[i].seller_id);
            }
        }

        public void RegroundUrl()
        {
            List<Job> jobs = (new Repositories()).GetJobs(2);

            for (int i = 0; i < jobs.Count; i++)
            {
                GetRegroundUrl(jobs[i].url, jobs[i].seller_id);
            }
        }

        public void LowlesUrl()
        {
            List<Job> jobs = (new Repositories()).GetJobs(3);

            for (int i = 0; i < jobs.Count; i++)
            {
                GetLowlesUrl(jobs[i].url, jobs[i].seller_id);
            }
        }


        public void NeezmallUrl()
        {
            List<Job> jobs = (new Repositories()).GetJobs(4);

            for (int i = 0; i < jobs.Count; i++)
            {
                GetNeezmallUrl(jobs[i].url, jobs[i].seller_id);
            }
        }


        public void RichbowlUrl()
        {
            List<Job> jobs = (new Repositories()).GetJobs(5);

            for (int i = 0; i < jobs.Count; i++)
            {
                GetRichbowlUrl(jobs[i].url, jobs[i].seller_id);
            }
        }

        public void BaseUrl()
        {
            // 잡 가져오기
            List<Job> jobs = (new Repositories()).GetJobs();

            for (int i = 0; i < jobs.Count; i++)
            {
                if (jobs[i].seller_id == 1)
                {
                    GetRepacUrl(jobs[i].url, jobs[i].seller_id);
                }
                else if (jobs[i].seller_id == 2)
                {
                    GetRegroundUrl(jobs[i].url, jobs[i].seller_id);
                }
                else if (jobs[i].seller_id == 3)
                {
                    GetLowlesUrl(jobs[i].url, jobs[i].seller_id);
                }
                else if (jobs[i].seller_id == 5)
                {
                    GetRichbowlUrl(jobs[i].url, jobs[i].seller_id);
                }
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

        public void GetRepacUrl(string url, int sellerId)
        {
            List<ProductList> confirm = (new Repositories()).GetProductListsById(1);
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

                    ProductList product = new ProductList();
                    for (int j = 0; j < products.Count; j++)
                    {
                        product = new ProductList()
                        {
                            thumbnail = products[j].SelectNodes("//img[@class='_org_img org_img _lazy_img']")[j].GetAttributeValue("src", ""),
                            productUrl = "https://repac.co.kr" + products[j].SelectNodes("//div[@class='item-wrap']/a")[j].GetAttributeValue("href", ""),
                            seller_id = sellerId,
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

            //List<ProductList> result = new List<ProductList>();

            // result는 is_used 0으로 만들 애들.
            //근데 productList에 없으면 데이터테이블(product_list)에도 제거(노출을 안한다)한다.업데이트(is_used y,n)
            //result = confirm.Where(p => productList.Count(s => p.productUrl.Contains(s.productUrl)) > 0).ToList();

            //업데이트(soft delete)
            //(new Repositories()).DeleteProductList(result);

            // 저장
            (new Repositories()).AddProductList(productList);
        }


        public void GetRegroundUrl(string url, int sellerId)
        {
            List<ProductList> confirm = (new Repositories()).GetProductListsById(2);
            List<ProductList> productList = new List<ProductList>();
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

                try
                {
                    driver.Url = url;
                    driver.Navigate();
                    Thread.Sleep(300);


                    string productNodeXpath = "//div[@class='shop-grid']/div/div/div[@class='shop-item _shop_item']";


                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(driver.PageSource);

                    var products = document.DocumentNode.SelectNodes(productNodeXpath);

                    ProductList product = new ProductList();
                    for (int j = 0; j < products.Count / 2; j++)
                    {
                        product = new ProductList()
                        {
                            thumbnail = products[j].SelectNodes("//img[@class='_org_img org_img _lazy_img']")[j].GetAttributeValue("src", ""),
                            productUrl = "https://re-ground.co.kr/" + products[j].SelectNodes("//div[@class='item-thumbs']/a")[j].GetAttributeValue("href", ""),
                            seller_id = sellerId,
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
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
            }

            //List<ProductList> result = new List<ProductList>();

            //// result는 is_used 0으로 만들 애들.
            ////근데 productList에 없으면 데이터테이블(product_list)에도 제거(노출을 안한다)한다.업데이트(is_used y,n)
            //result = confirm.Where(p => productList.Count(s => p.productUrl.Contains(s.productUrl)) > 0).ToList();

            ////업데이트(soft delete)
            //(new Repositories()).DeleteProductList(result);

            // 저장
            (new Repositories()).AddProductList(productList);
        }

        public void GetLowlesUrl(string url, int sellerId)
        {
            List<ProductList> confirm = (new Repositories()).GetProductListsById(3);
            List<ProductList> productList = new List<ProductList>();
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

                try
                {
                    driver.Url = url;
                    driver.Navigate();
                    Thread.Sleep(300);



                    string productNodeXpath = "//*[@id='wide_contents']/div[2]/div[1]/div[2]/ul/li";


                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(driver.PageSource);

                    var products = document.DocumentNode.SelectNodes(productNodeXpath);

                    ProductList product = new ProductList();
                    for (int j = 0; j < products.Count; j++)
                    {
                        product = new ProductList()
                        {
                            thumbnail = products[j].SelectNodes("//img[@class='hover']")[j].GetAttributeValue("src", ""),
                            productUrl = "https://lowles.kr/" + products[j].SelectNodes("//div[@class='thumbnail']/a")[j].GetAttributeValue("href", ""),
                            seller_id = sellerId,
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
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
            }

            // 저장
             (new Repositories()).AddProductList(productList);
        }


        public void GetNeezmallUrl(string url, int sellerId)
        {
            List<ProductList> confirm = (new Repositories()).GetProductListsById(4);
            List<ProductList> productList = new List<ProductList>();
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                int count = 0;
                for (int i = 1; ; i++)
                {

                    try
                    {
                        //To do 페이지처리 필요
                        //driver.Url = url;
                        driver.Url = url + "&pStart=" + (16 * (i - 1));
                        //i가 1일때는 0을 붙인다
                        //i가 2이상일 때는 16씩 증가하여 붙인다
                        //1==0 = 16*(i-1) 
                        //2==16 = 16*(i-1)
                        //3==32 = 16*(i-1)

                        driver.Navigate();
                        Thread.Sleep(1000);

                        string productNodeXpath = "//div[@class='gallery_list column4 pdBtnBoxWrap wrap new']/ul/li";

                        HtmlDocument document = new HtmlDocument();
                        document.LoadHtml(driver.PageSource);

                        var products = document.DocumentNode.SelectNodes(productNodeXpath);

                        if (products == null)
                        {
                            if (count == 3)
                            {
                                break;
                            }
                            i--;
                            count++;
                            continue;
                        }


                        ProductList product = new ProductList();
                        for (int j = 0; j < products.Count; j++)
                        {
                            product = new ProductList()
                            {
                                thumbnail = products[j].SelectNodes("//img[@class='_wtfull']")[j].GetAttributeValue("src", ""),
                                productUrl = "https://www.neezmall.com/" + products[j].SelectNodes("//a[@class='pdLink']")[j].GetAttributeValue("href", ""),
                                seller_id = sellerId,
                            };

                            int ignore = 0;
                            for (int k = 0; k < confirm.Count; k++)
                            {
                                // 둘다 있어 > 추가하지않고 넘어감
                                if (product.productUrl == confirm[k].productUrl)
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
                    catch (Exception ex)
                    {
                        string error = ex.Message;
                    }
                }
            }


            // 저장
            (new Repositories()).AddProductList(productList);
        }



        public void GetRichbowlUrl(string url, int sellerId)
        {
            List<ProductList> confirm = (new Repositories()).GetProductListsById(5);
            List<ProductList> productList = new List<ProductList>();
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

                try
                {
                    driver.Url = url;
                    driver.Navigate();
                    Thread.Sleep(300);

                    string productNodeXpath = "//*[@id='sct']/ul/li";

                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(driver.PageSource);

                    var products = document.DocumentNode.SelectNodes(productNodeXpath);

                    ProductList product = new ProductList();
                    for (int j = 0; j < products.Count; j++)
                    {
                        product = new ProductList()
                        {
                            //Todo 수정필요
                            thumbnail = products[j].SelectNodes("//a[@class='sct_a']/img")[j].GetAttributeValue("src", ""),
                            productUrl = products[j].SelectNodes("//a[@class='sct_a']")[j].GetAttributeValue("href", ""),
                            seller_id = sellerId,
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
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
            }

       // 저장
       (new Repositories()).AddProductList(productList);
        }
    }
}

