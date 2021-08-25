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
        public void GetDetail(List<ProductList> product) // 원래 product url, seller_id 가 get detail 인풋 파라미터에 있었음
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
                        //productlist 형식의 product 에 i번째 있는 producturl을 가져와
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
                        productDetails.description = document.DocumentNode.SelectSingleNode("//*[@id='prod_detail_body']").InnerHtml;
                        //예외처리 추가 brand 값이 없을 때, 브랜드 네임에 "빈칸" 넣어줘
                        if (jarray["brand"] == null)
                        {
                            productDetails.brandName = "";
                        }
                        else
                        // 그렇지 않으면 브랜드네임 가져와
                        {
                            productDetails.brandName = jarray["brand"]["name"].ToString();
                        }

                        //productDetails.brandName = jarray["brand"]["name"].ToString(); 원래 이거였음 
                        productDetails.price = int.Parse(jarray["offers"]["price"].ToString());
                        productDetails.sellerId = product[i].seller_id;
                        productDetails.productUrl = product[i].productUrl;
                        //productDetails.ecoCertifications = new List<EcoCertifications>();

                        //status int 형식을 변경 뒤에 1 넣어줌
                        productDetails.status = 1;

                        productDetails.option = new Dictionary<int, List<string>>();
                        //productDetails.option.Add(0, document.DocumentNode.SelectNodes("//*[@id='prod_options']/div/div/div[2]/a").ToList());I
                        //dictionary 에 담을 list<string> 형태 변수 선언 
                        List<string> sizes = new List<string>();
                        var temp = document.DocumentNode.SelectNodes("//*[@id=prod'_options']/div/div/div[2]/div");
                        foreach (var item in temp)
                        {
                            sizes.Add(item.InnerText);
                        }
                        productDetails.option.Add(0, sizes); // 0은 사이즈


                        productDetails.deliveryTime = document.DocumentNode.SelectSingleNode("//*[@id='prod_goods_form']/div[3]/div/div[3]/div/div[2]/div").InnerText;
                        // 숫자 가져오는 정규식 문법 regex 사용
                        productDetails.shippingFee = Regex.Replace(document.DocumentNode.SelectSingleNode("//*[@id='prod_goods_form']/div[3]/div/div[1]/div[8]/div[2]/span/text()").InnerText, @"D", "");


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
                    //GetRegroundProducts를 통해 얻어온 아이템들이 public void GetRegroundDetail(string productUrl, int seller_id)의 productUrl에 들어가있고, 이 url을 가져옴
                    driver.Url = productUrl;
                    driver.Navigate();
                    Thread.Sleep(300);

                    //Xpath 검증필요
                    // 이동한 url의 상품 디테일 구역
                    string productNodeXpath = "//*[@id='prod_detail']/div";

                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(driver.PageSource);


                    var products = document.DocumentNode.SelectSingleNode(productNodeXpath);

                    //아래로 수정 필요
                    Detail details = new Detail();
                    details.brand = "리그라운드";
                    details.Manufacturer = "리그라운드";
                    details.Origin = products.SelectSingleNode("//*[@id='prod_goods_form']/div[3]/div/div[1]/div[1]/div[2]/span").InnerText;




                    ProductDetail product = new ProductDetail();


                    //추가 필요
                    product.sellerId = seller_id;
                    product.productUrl = productUrl;
                    product.status = 1;

                    product.brandName = "리그라운드";
                    product.name = products.SelectSingleNode("//div[@class='view_tit no-margin-top ']").InnerText;
                    product.mainImage = products.SelectSingleNode("//div[@class='item _item']/img").GetAttributeValue("src", "");
                    product.price = int.Parse(products.SelectSingleNode("//span[@class='real_price']").InnerText.Replace(",", "").Replace("원", ""));
                    product.description = products.SelectSingleNode("//*[@class='detail_detail_wrap ']").InnerHtml;

                    product.deliveryTime = products.SelectSingleNode("//div[@class='type01']/strong").InnerText;
                    product.shippingFee = products.SelectSingleNode("//*[@id='prod_goods_form']/div[3]/div/div[1]/div[6]/div[2]/span").InnerText;
                    product.detail = new List<Detail> { details };
                    //url에 상품코드가 있는 경우
                    //1. 스플릿 등 지지고볶는 방법
                    //2. 정규표현식을 사용하는 방법
                    product.productCode = productUrl;
                    //product.option = new Dictionary<int, List<string>>() { { 0, products.SelectNodes("//div[@class='dropdown-menu']/a/span").Select(d => d.InnerText).ToList() } };

                    //KEY값(INT)과, VALUE(LIST)가 있음
                    //KEY = 0 

                    product.ecoCertifications = null;

                    productDetail.Add(product);
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }

            }
            // 저장

            (new Repositories()).AddProductDetail(productDetail);
        }




        public List<ProductList> GetLowlesDetail()
        {

            List<ProductList> products = (new Repositories()).GetProductListsById(3);

            GetLowlesDetail(products);

            return products;
        }



        /// <summary>
        /// getdetail 특정셀러 상세정보 가져오기 
        /// </summary>
        /// <param name="productInfo"> 상품정보 </param>
        /// <param name="url">상세페이지 url</param>
        /// <param name="InnerText"> 세부정보 </param>
        public void GetLowlesDetail(List<ProductList> product) // 원래 product url, seller_id 가 get detail 인풋 파라미터에 있었음
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
                        //productlist 형식의 product 에 i번째 있는 producturl을 가져와
                        driver.Url = product[i].productUrl;
                        driver.Navigate();
                        HtmlDocument document = new HtmlDocument();
                        document.LoadHtml(driver.PageSource);
                        productDetails.id = product[i].id;
                        productDetails.name = document.DocumentNode.SelectSingleNode("//*[@id='wide_contents']/div[3]/div/div[1]/div[2]/div[1]/h2").InnerText;
                        productDetails.mainImage = document.DocumentNode.SelectSingleNode("//*[@id='prdDetail']/div/div[1]/img[2]").InnerText;
                        productDetails.productCode = null;
                        productDetails.description = document.DocumentNode.SelectSingleNode("//*[@id='prdDetail']/div/div[2]/p[2]/img").InnerHtml;
                        productDetails.brandName = null;
                        productDetails.price = int.Parse(document.DocumentNode.SelectSingleNode("//*[@id='span_product_price_text']").InnerText);
                        productDetails.sellerId = product[i].seller_id;
                        productDetails.productUrl = product[i].productUrl;
                        //productDetails.ecoCertifications = new List<EcoCertifications>();

                        //status int 형식을 변경 뒤에 1 넣어줌
                        productDetails.status = 1;

                        productDetails.option = new Dictionary<int, List<string>>();
                        //productDetails.option.Add(0, document.DocumentNode.SelectNodes("//*[@id='prod_options']/div/div/div[2]/a").ToList());I
                        //dictionary 에 담을 list<string> 형태 변수 선언 
                        List<string> sizes = new List<string>();
                        var temp = document.DocumentNode.SelectNodes("//*[@id='product_option_id1']");
                        foreach (var item in temp)
                        {
                            sizes.Add(item.InnerText);
                        }
                        productDetails.option.Add(0, sizes); // 0은 사이즈

                        productDetails.deliveryTime = document.DocumentNode.SelectSingleNode("//*[@id='wide_contents']/div[4]/div[3]/div[1]/div[2]/text()[5]").InnerText;
                        productDetails.shippingFee = document.DocumentNode.SelectSingleNode("//*[@id='wide_contents']/div[4]/div[3]/div[1]/div[2]/text()[4]").InnerText;


                        //디테일 수정필요 
                        //Detail details = new Detail();
                        //details.brand = ;
                        //details.Manufacturer = document.DocumentNode.SelectSingleNode("//*[@id='prod_goods_form']/div[3]/div/div[1]/div[1]/div[2]/span").InnerText;
                        //details.Origin = "정보없음";
                        //productDetails.detail = new List<Detail>();
                        //productDetails.detail.Add(details);


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

        public void GetNeezmallDetail(List<ProductList> product) // 원래 product url, seller_id 가 get detail 인풋 파라미터에 있었음
        {
            string error = string.Empty;
            ProductDetail productDetails = new ProductDetail();
            List<ProductDetail> products = new List<ProductDetail>();
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(4);
                for (int i = 0; i < product.Count; i++)
                {
                    productDetails = new ProductDetail();
                    try
                    {
                        //productlist 형식의 product 에 i번째 있는 producturl을 가져와
                        driver.Url = product[i].productUrl;
                        driver.Navigate();
                        HtmlDocument document = new HtmlDocument();
                        document.LoadHtml(driver.PageSource);
                        productDetails.id = product[i].id;
                        productDetails.name = document.DocumentNode.SelectSingleNode("//*[@id='Frm']/div/div[2]/div/h1").InnerText;
                        productDetails.mainImage = document.DocumentNode.SelectSingleNode("//*[@id='mainImg']/li/span").InnerText;
                        productDetails.productCode = document.DocumentNode.SelectSingleNode("//*[@id='Frm']/div/div[2]/div/div[9]/dl[1]/dd").InnerText;
                        productDetails.description = document.DocumentNode.SelectSingleNode("//*[@id='tbContent']/tbody/tr/td/div/p/a/img").InnerHtml;
                        productDetails.brandName = null;
                        productDetails.price = int.Parse(document.DocumentNode.SelectSingleNode("//*[@id='TotalGoodsPrice']").InnerText);
                        productDetails.sellerId = product[i].seller_id;
                        productDetails.productUrl = product[i].productUrl;
                        //productDetails.ecoCertifications = new List<EcoCertifications>();

                        //status int 형식을 변경 뒤에 1 넣어줌
                        productDetails.status = 1;

                        productDetails.option = new Dictionary<int, List<string>>();
                        //productDetails.option.Add(0, document.DocumentNode.SelectNodes("//*[@id='prod_options']/div/div/div[2]/a").ToList());I
                        //dictionary 에 담을 list<string> 형태 변수 선언 
                        List<string> sizes = new List<string>();
                        var temp = document.DocumentNode.SelectNodes("//*[@id='optList']"); //옵션 재확인 필요 
                        foreach (var item in temp)
                        {
                            sizes.Add(item.InnerText);
                        }
                        productDetails.option.Add(0, sizes); // 0은 사이즈

                        productDetails.deliveryTime = document.DocumentNode.SelectSingleNode("/html/body/div[2]/div[1]/div[5]/div[1]/div[6]/table[1]/tbody/tr[3]/td/table/tbody/tr[2]/td/span").InnerText;
                        productDetails.shippingFee = document.DocumentNode.SelectSingleNode("/html/body/div[2]/div[1]/div[5]/div[1]/div[6]/table[1]/tbody/tr[3]/td/table/tbody/tr[1]/td").InnerText;

                        Detail details = new Detail();
                        details.brand = "정보없음";
                        details.Manufacturer = "정보없음";
                        details.Origin = document.DocumentNode.SelectSingleNode("//*[@id='Frm']/div/div[2]/div/div[9]/dl[2]/dd").InnerText;
                        productDetails.detail = new List<Detail>();
                        productDetails.detail.Add(details);

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
    }
}
