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
                GetDetail(item.productUrl, item.seller_id);
            }

            return products;
        }



        /// <summary>
        /// getdetail 특정셀러 상세정보 가져오기 
        /// </summary>
        /// <param name="productInfo"> 상품정보 </param>
        /// <param name="url">상세페이지 url</param>
        /// <param name="InnerText"> 세부정보 </param>
        public void GetDetail(string url, int seller_id)
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
                    productDetails.sellerId = seller_id;
                    productDetails.option = new Dictionary<int, List<string>>();
                    //productDetails.option.Add(0, document.DocumentNode.SelectNodes("//*[@id='prod_options']/div/div/div[2]/a").ToList());
                    //dictionary 에 담을 list<string> 형태 변수 선언 
                    List<string> sizes = new List<string>();
                    var temp = document.DocumentNode.SelectNodes("//*[@id='prod_options']/div/div/div[2]/a");
                    foreach (var item in temp) 
                    {
                        sizes.Add(item.InnerText);
                    }
                    productDetails.option.Add(0, sizes); // 0은 사이즈


                    DeliveryInfo deliveryinfo = new DeliveryInfo();
                    deliveryinfo.deliveryTime = document.DocumentNode.SelectSingleNode("").InnerText;
                    deliveryinfo.shippingFee = int.Parse(document.DocumentNode.SelectSingleNode("//*[@id='prod_goods_form']/div[3]/div/div[1]/div[7]/div[2]/span/text()").InnerText);
                    productDetails.deliveryInfo = deliveryinfo;
                    

                    // deliveryinfo함수랑 객체 연습하고 나서  deliveryinfo는 deliveryinfo type이고, deliveryinfo 안에 deliverytime, shippingfee 있음 
                    // deliverytime 은 string , shippingfee 는 int 에서 string 으로 바꿔줌 
                    // 옵션 : 샘플 참고 
                    // productdetails db에 product table에 insert 하기 
                    // Addproductdetail 함수 사용하여 sql data insert하기  
                    //productDetails.deliveryInfo = 
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }

 
            }
            List<ProductDetail> products = new List <ProductDetail>();
            (new Repositories()).AddProductDetail(products);


        }
    }
}
