using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using HtmlAgilityPack;
using MySql.Data.MySqlClient;
using System.Data;
using EcoBot.DB;
using Newtonsoft.Json.Linq;

namespace EcoBot.Crawling
{
    public class ProductDetailCrawler : BaseCrawler
    {
        // productlist 테이블에 있는 producturl불러오기
        public IEnumerable<ProductList> GetProductUrl()
        {

            List<ProductList> products = (new Repositories()).GetProductListsById(1);
            //1. products 에서 url 활용하기  
            //2. getdetail 사용하기 
            foreach (var item in products)
            {
                GetDetail(item.productUrl);
            }

            return products;
        }



        /// <summary>
        /// getdetail 특정셀러 상세정보 가져오기 
        /// </summary>
        /// <param name="productInfo"> 상품정보 </param>
        /// <param name="url">상세페이지 url</param>
        /// <param name="InnerText"> 세부정보 </param>
        public void GetDetail(string url)
        {
            string error = string.Empty;
            ProductDetail productDetails = new ProductDetail();
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                try
                {
                    driver.Url = url;
                    driver.Navigate();
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(driver.PageSource);

                    string productData = document.DocumentNode.SelectSingleNode("//script[@type='application/ld+json']").InnerText;
                    JObject jarray = JObject.Parse(productData);

                    productDetails.name = jarray["name"].ToString();
                    productDetails.mainImage = jarray["image"][0].ToString();
                    productDetails.productCode = null;
                    productDetails.description = document.DocumentNode.SelectSingleNode("//*[@id='prod_detail_body']").InnerText;
                    productDetails.brandName = jarray["brand"]["name"].ToString();
                    productDetails.price = int.Parse(jarray["offers"]["price"].ToString());
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }

            }
        }
    }
}
