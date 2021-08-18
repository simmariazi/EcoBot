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
using System.Text.RegularExpressions;
using System.Threading;

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

            GetDetail(products);

            return products;
        }



        /// <summary>
        /// getdetail 특정셀러 상세정보 가져오기 
        /// </summary>
        /// <param name="productInfo"> 상품정보 </param>
        /// <param name="url">상세페이지 url</param>
        /// <param name="InnerText"> 세부정보 </param>
        public void GetDetail(List<ProductList> product)
        {
            string error = string.Empty;
            ProductDetail productDetails = new ProductDetail();
            List<ProductDetail> products = new List<ProductDetail>();
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                for (int i = 0; i < product.Count; i++)
                {
                    productDetails = new ProductDetail();
                    try
                    {
                        driver.Url = product[i].productUrl;
                        driver.Navigate();
                        HtmlDocument document = new HtmlDocument();
                        document.LoadHtml(driver.PageSource);

                        string productData = document.DocumentNode.SelectSingleNode("//script[@type='application/ld+json']").InnerText;
                        JObject jarray = JObject.Parse(productData);
                        productDetails.id = product[i].id;
                        productDetails.name = jarray["name"].ToString();
                        string a = productDetails.name;
                        productDetails.mainImage = jarray["image"][0].ToString();
                        productDetails.productCode = null;
                        productDetails.description = document.DocumentNode.SelectSingleNode("//*[@id='prod_detail_body']").InnerText;
                        //예외처리 추가
                        if (jarray["brand"] == null)
                        {
                            productDetails.brandName = "";
                        }
                        else
                        {
                            productDetails.brandName = jarray["brand"]["name"].ToString();
                        }

                        //productDetails.brandName = jarray["brand"]["name"].ToString(); 원래 이거였음 
                        productDetails.price = int.Parse(jarray["offers"]["price"].ToString());
                        productDetails.sellerId = product[i].seller_id;
                        productDetails.productUrl = product[i].productUrl;
                        //productDetails.ecoCertifications = new List<EcoCertifications>();


                        productDetails.status = 1;

                        productDetails.option = new Dictionary<int, List<string>>();
                        //productDetails.option.Add(0, document.DocumentNode.SelectNodes("//*[@id='prod_options']/div/div/div[2]/a").ToList());I
                        //dictionary 에 담을 list<string> 형태 변수 선언 
                        List<string> sizes = new List<string>();
                        var temp = document.DocumentNode.SelectNodes("//*[@id='prod_options']/div/div/div[2]/a");
                        foreach (var item in temp)
                        {
                            sizes.Add(item.InnerText);
                        }
                        productDetails.option.Add(0, sizes); // 0은 사이즈


                        productDetails.deliveryInfo = new DeliveryInfo();
                        productDetails.deliveryInfo.deliveryTime = document.DocumentNode.SelectSingleNode("//*[@id='prod_goods_form']/div[3]/div/div[3]/div/div[2]/div/div[2]").InnerText;
                        //deliveryinfo.shippingFee = int.Parse(document.DocumentNode.SelectSingleNode("//*[@id='prod_goods_form']/div[3]/div/div[1]/div[7]/div[2]/span/text()").InnerText);
                        // 숫자 가져오는 정규식 문법 regex 사용
                        productDetails.deliveryInfo.shippingFee = int.Parse(Regex.Replace( document.DocumentNode.SelectSingleNode("//span[@class='option_data'").InnerText,@"D",""));


                        Detail details = new Detail();
                        details.brand = jarray["brand"]["name"].ToString();
                        details.Manufacturer = document.DocumentNode.SelectSingleNode("//*[@id='prod_goods_form']/div[3]/div/div[1]/div[1]/div[2]/span").InnerText;
                        details.Origin = "정보없음";
                        productDetails.detail = new List<Detail>();
                        productDetails.detail.Add(details);


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
                    products.Add(productDetails);
                }
                

            }
            (new Repositories()).AddProductDetail(products);


        }



        public void GetRegroundProducts()
        {

            List<ProductList> products = (new Repositories()).GetProductListsById(2);
            //1. products 에서 url 활용하기  
            //2. getdetail 사용하기 
            foreach (var item in products)
            {
                GetRegroundDetail(item.productUrl, item.seller_id);
            }
        }

        public void GetRegroundDetail(string productUrl, int seller_id)
        {
            List<ProductList> confirm = (new Repositories()).GetProductListsById(2);
            List<ProductDetail> productDetail = new List<ProductDetail>();
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

                try
                {
                    driver.Url = productUrl;
                    driver.Navigate();
                    Thread.Sleep(300);

                    //Xpath 검증필요
                    string productNodeXpath = "//*[@id='prod_detail']/div";

                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(driver.PageSource);

                    var products = document.DocumentNode.SelectNodes(productNodeXpath);


                    //아래로 수정 필요
                    ProductDetail product = new ProductDetail();
                    product = new ProductDetail()
                    {
                        //추가 필요
                        detailUrl = productUrl,
                        sellerId = seller_id,
                        name = "",
                        productCode = "",
                        detail = { },
                        description = "",
                        deliveryInfo = { },
                        price = 0,
                        option = { },
                        brandName = "",
                        ecoCertifications = { },
                    };
                }
                catch
                {
                }

            }
            // 저장
            (new Repositories()).AddProductDetail(productDetail);
        }
    }
}
