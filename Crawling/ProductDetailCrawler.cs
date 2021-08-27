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
                        productDetails.description = document.DocumentNode.SelectSingleNode("//*[@id='prod_detail_body']").InnerHtml.Replace("'", "");
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
                        var temp = document.DocumentNode.SelectNodes("//*[@id='prod_options']/div/div/div/div");
                        foreach (var item in temp)
                        {
                            sizes.Add(item.InnerText.Trim());
                        }
                        productDetails.option.Add(0, sizes); // 0은 사이즈


                        productDetails.deliveryTime = document.DocumentNode.SelectSingleNode("//*[@id='prod_goods_form']/div[3]/div/div[3]/div/div[2]/div").InnerText.Trim();
                        // 숫자 가져오는 정규식 문법 regex 사용
                        productDetails.shippingFee = document.DocumentNode.SelectSingleNode("//span[contains(text(),'배송비')]/parent::div/parent::div/div/span[contains(@class, 'option_data')]").InnerText;
                        productDetails.shippingFee = productDetails.shippingFee.Replace("popover", "");


                        Detail details = new Detail();
                        details.brand = productDetails.brandName;
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



        public IEnumerable<ProductList> GetRegroundUrl()
        {

            List<ProductList> products = (new Repositories()).GetProductListsById(2);

            GetRegroundDetail(products);
            return products;
        }

        public void GetRegroundDetail(List<ProductList> products)
        {
            List<ProductList> confirm = (new Repositories()).GetProductListsById(2);
            List<ProductDetail> productDetail = new List<ProductDetail>();
            ProductDetail product;
            Detail details;
            List<string> options;
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                int count = 0;
                for (int i = 0; i < products.Count; i++)
                {

                    try
                    {

                        //GetRegroundProducts를 통해 얻어온 아이템들이 public void GetRegroundDetail(string productUrl, int seller_id)의 productUrl에 들어가있고, 이 url을 가져옴
                        driver.Url = products[i].productUrl;
                        driver.Navigate();
                        Thread.Sleep(700);

                        //Xpath 검증필요
                        // 이동한 url의 상품 디테일 구역
                        string productNodeXpath = "//*[@id='prod_detail']/div";

                        HtmlDocument document = new HtmlDocument();
                        document.LoadHtml(driver.PageSource);

                        var productDocument = document.DocumentNode.SelectSingleNode(productNodeXpath);
                        if (productDocument == null)
                        {
                            Thread.Sleep(500);
                            productDocument = document.DocumentNode.SelectSingleNode(productNodeXpath);

                            if (productDocument == null)
                            {
                                continue;
                            }
                        }
                       
                        product = new ProductDetail();
                        //아래로 수정 필요
                        details = new Detail();
                        details.brand = "리그라운드";
                        details.Manufacturer = "리그라운드";
                        details.Origin = productDocument.SelectSingleNode("//*[@id='prod_goods_form']/div[3]/div/div[1]/div[1]/div[2]/span").InnerText;


                        //추가 필요
                        product.id = products[i].id;
                        product.sellerId = products[i].seller_id;
                        product.productUrl = products[i].productUrl;
                        product.status = 1;

                        product.brandName = "리그라운드";
                        product.name = productDocument.SelectSingleNode("//div[@class='view_tit no-margin-top ']").InnerText.Trim();
                        if (product.name.Contains("SOLDOUT"))
                        {
                            product.status = 0;
                        }

                        product.mainImage = productDocument.SelectSingleNode("//div[@class='item _item']/img").GetAttributeValue("src", "");
                        product.price = int.Parse(productDocument.SelectSingleNode("//span[@class='real_price']").InnerText.Replace(",", "").Replace("원", ""));
                        product.description = productDocument.SelectSingleNode("//*[@class='detail_detail_wrap ']").InnerHtml.Replace("'", "").Trim();

                        product.deliveryTime = productDocument.SelectSingleNode("//div[@class='type01']/strong").InnerText.Trim();
                        product.shippingFee = productDocument.SelectSingleNode("//*[@id='prod_goods_form']/div[3]/div/div[1]/div[6]/div[2]/span").InnerText;
                        product.shippingFee = product.shippingFee.Replace("popover", "").Trim();

                        product.detail = new List<Detail> { details };
                        //url에 상품코드가 있는 경우
                        //1. 스플릿 등 지지고볶는 방법
                        //2. 정규표현식을 사용하는 방법
                        product.productCode = products[i].productUrl.Split('=')[1];
                        //https://re-ground.co.kr//shop_view/?idx=81
                        //split을 하게 되면 배열이 생성됨. =을 기준으로 2개가 생성되었기 때문에, 두번째 값인 1을 사용하게 됨
                        //예를 들어 = 이 3개면 4개짜리 배열이 생성됨


                        product.option = new Dictionary<int, List<string>>();

                        options = new List<string>();
                        var temp = document.DocumentNode.SelectNodes("//div[@class='dropdown-menu']/div/a/span");
                        if (temp != null)
                        {
                            foreach (var item in temp)
                            {
                                options.Add(item.InnerText.Trim());
                            }
                        }

                        product.option.Add(0, options);

                        product.ecoCertifications = null;

                        productDetail.Add(product);
                    }
                    catch (Exception ex)
                    {
                        string error = ex.Message;
                    }
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
                        productDetails.mainImage = "https:" + product[i].thumbnail;
                        productDetails.productCode = document.DocumentNode.SelectSingleNode("//a[@class='size_guide_info']").GetAttributeValue("product_no", "");
                        productDetails.description = document.DocumentNode.SelectSingleNode("//div[@id='prdDetail']/div").InnerHtml;
                        productDetails.brandName = "lowles";
                        productDetails.price = int.Parse(document.DocumentNode.SelectSingleNode("//*[@id='span_product_price_text']").InnerText.Replace("원","").Replace(",",""));
                        productDetails.sellerId = product[i].seller_id;
                        productDetails.productUrl = product[i].productUrl;
                        //productDetails.ecoCertifications = new List<EcoCertifications>();

                        //status int 형식을 변경 뒤에 1 넣어줌
                        productDetails.status = 1;

                        productDetails.option = new Dictionary<int, List<string>>();
                        //productDetails.option.Add(0, document.DocumentNode.SelectNodes("//*[@id='prod_options']/div/div/div[2]/a").ToList());I
                        //dictionary 에 담을 list<string> 형태 변수 선언 
                        List<string> option = new List<string>();
                        var temp = document.DocumentNode.SelectNodes("//optgroup[@label='옵션']/option");
                        foreach (var item in temp)
                        {
                            option.Add(item.InnerText);
                        }
                        productDetails.option.Add(0, option); // 0은 사이즈

                        productDetails.deliveryTime = document.DocumentNode.SelectSingleNode("//*[@id='wide_contents']/div[4]/div[3]/div[1]/div[2]/text()[6]").InnerText;
                        productDetails.shippingFee = document.DocumentNode.SelectSingleNode("//*[@id='wide_contents']/div[4]/div[3]/div[1]/div[2]/text()[5]").InnerText;

                        
                        //디테일 수정필요 
                        Detail details = new Detail();
                        details.brand = "lowles";
                        details.Manufacturer = "제품 내 별도표기";
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
        public void GetRichbowlDetail(List<ProductList> product) // 원래 product url, seller_id 가 get detail 인풋 파라미터에 있었음
        {
            string error = string.Empty;
            ProductDetail productDetails = new ProductDetail();
            List<ProductDetail> products = new List<ProductDetail>();
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
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
                        productDetails.name = document.DocumentNode.SelectSingleNode("//*[@id='sit_title;]").InnerText;
                        productDetails.mainImage = document.DocumentNode.SelectSingleNode("//*[@id='sit_pvi_big']/div[1]/div/div/div/a/img").InnerText;
                        productDetails.productCode = null;
                        productDetails.description = document.DocumentNode.SelectSingleNode("//*[@id='sit_inf_explan']/center[6]/p[1]/img").InnerText;
                        productDetails.brandName = null;
                        productDetails.price = int.Parse(document.DocumentNode.SelectSingleNode("//*[@id='sit_ov']/div[2]/table/tbody/tr[1]/td").InnerText);
                        productDetails.sellerId = product[i].seller_id;
                        productDetails.productUrl = product[i].productUrl;
                        //productDetails.ecoCertifications = new List<EcoCertifications>();

                        //status int 형식을 변경 뒤에 1 넣어줌
                        productDetails.status = 1;

                        productDetails.option = new Dictionary<int, List<string>>();
                        //productDetails.option.Add(0, document.DocumentNode.SelectNodes("//*[@id='prod_options']/div/div/div[2]/a").ToList());I
                        //dictionary 에 담을 list<string> 형태 변수 선언 
                        List<string> sizes = new List<string>();
                        var temp = document.DocumentNode.SelectNodes("//*[@id='it_option_1']"); //옵션 재확인 필요 
                        foreach (var item in temp)
                        {
                            sizes.Add(item.InnerText);
                        }
                        productDetails.option.Add(0, sizes); // 0은 사이즈

                        productDetails.deliveryTime = "영업일 기준 16시 이전 주문건에 한하여 당일 출고";
                        productDetails.shippingFee = document.DocumentNode.SelectSingleNode("//*[@id='sit_ov']/div[2]/table/tbody/tr[3]/td").InnerText;

                        Detail details = new Detail();
                        details.brand = "정보없음";
                        details.Manufacturer = "정보없음";
                        details.Origin = "정보없음";
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
