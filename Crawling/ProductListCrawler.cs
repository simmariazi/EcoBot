using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
        //잡 테이블에 있는 데이터불러오기

        //잡에있는 데이터 중 리팩의 url만 불러옴
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
            List<Job> jobs;
            DateTime now;
            for (; ; )
            {
                // 잡 가져오기
                jobs = (new Repositories()).GetJobs();

                //DateTime now = DateTime.ParseExact(DateTime.Now.ToString(), "yyyyMMdd HHmmss", null);
                now = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                for (int i = 0; i < jobs.Count; i++)
                {
                    if (jobs[i].seller_id == 1)
                    {
                        //[잡_라스트 크롤링 데이트]로부터 [셀러_사이클]시간이 경과했으면 last_crawling_date
                        //셀러 사이클은 셀러테이블에서 불러옴

                        // last_crawling_date + cycle >= NOW  >>실행안함
                        // last_crawling_date + cycle <= NOW  >> 실행함
                        // last_crawling_date = null  >> 실행함
                        if ((now - jobs[i].last_crawling_date).TotalHours <= jobs[i].cycle)
                        {
                            continue;
                        }
                        GetRepacUrl(jobs[i].url, jobs[i].seller_id);
                        //수집시간을 [잡_라스트 크롤링 데이트]에 인서트
                        (new Repositories()).UpdateJobCrawlingDate(jobs[i].id);
                    }
                    else if (jobs[i].seller_id == 2)
                    {
                        if ((now - jobs[i].last_crawling_date).TotalHours <= jobs[i].cycle)
                        {
                            continue;
                        }
                        GetRegroundUrl(jobs[i].url, jobs[i].seller_id);
                        (new Repositories()).UpdateJobCrawlingDate(jobs[i].id);
                    }
                    else if (jobs[i].seller_id == 3)
                    {
                        if ((now - jobs[i].last_crawling_date).TotalHours <= jobs[i].cycle)
                        {
                            continue;
                        }
                        GetLowlesUrl(jobs[i].url, jobs[i].seller_id);
                        (new Repositories()).UpdateJobCrawlingDate(jobs[i].id);
                    }
                    else if (jobs[i].seller_id == 4)
                    {
                        if ((now - jobs[i].last_crawling_date).TotalHours <= jobs[i].cycle)
                        {
                            continue;
                        }
                        GetNeezmallUrl(jobs[i].url, jobs[i].seller_id);
                        (new Repositories()).UpdateJobCrawlingDate(jobs[i].id);
                    }
                    else if (jobs[i].seller_id == 5)
                    {
                        if ((now - jobs[i].last_crawling_date).TotalHours <= jobs[i].cycle)
                        {
                            continue;
                        }
                        GetRichbowlUrl(jobs[i].url, jobs[i].seller_id);
                        (new Repositories()).UpdateJobCrawlingDate(jobs[i].id);
                    }
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
            DeleteProduct(confirm, productList);

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
                    Thread.Sleep(1000);


                    string productNodeXpath = "//div[@class='shop-grid']/div/div/div[@class='shop-item _shop_item']";


                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(driver.PageSource);

                    var products = document.DocumentNode.SelectNodes(productNodeXpath);

                    ProductList product = new ProductList();
                    for (int j = 0; j < products.Count / 2; j++)
                    {
                        product = new ProductList()
                        {
                            thumbnail = products[j].SelectNodes("//img[@class='_org_img org_img _lazy_img']")[j].GetAttributeValue("data-src", ""),
                            productUrl = "https://re-ground.co.kr/" + products[j].SelectNodes("//div[@class='item-thumbs']/a")[j].GetAttributeValue("href", ""),
                            seller_id = sellerId,
                        };
                        if (!product.thumbnail.Contains("http"))
                        {
                            int a = 0;
                        }


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
            DeleteProduct(confirm, productList);
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
                            productUrl = "https://lowles.kr" + products[j].SelectNodes("//div[@class='thumbnail']/a")[j].GetAttributeValue("href", ""),
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
            DeleteProduct(confirm, productList);
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
                        Thread.Sleep(3000);

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

                        count = 0;

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
            DeleteProduct(confirm, productList);

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
            DeleteProduct(confirm, productList);

       (new Repositories()).AddProductList(productList);
        }

        private void DeleteProduct(List<ProductList> confirmProducts, List<ProductList> products)
        {
            List<ProductList> deleteProducts = new List<ProductList>();
            int confirmCount = 0;
            // 3.confirm에는 있는데 product의 url에는 없으면 데이터베이스에서 confirm에만 있는 데이터를 삭제
            //confirm 리스트에 있는 값 - product 리스트에 있는 값 = (a)
            for (int i = 0; i < confirmProducts.Count; i++)
            {
                confirmCount = 0;
                for (int j = 0; j < products.Count; j++)
                {
                    if (confirmProducts[i].productUrl.Equals(confirmProducts[j].productUrl))
                    {
                        confirmCount++;
                    }

                    if (confirmCount == 0)
                    {
                        deleteProducts.Add(confirmProducts[i]);
                    }
                }
            }
               //a를 신규 리스트에 임시저장하고, 임시 저장한 a값을 데이터에서 삭제하는 로직 추가
               (new Repositories()).DeleteConfirmList(deleteProducts);
        }
    }
}

