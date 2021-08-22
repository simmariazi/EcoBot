using System;
using System.Collections.Generic;

namespace EcoBot
{
    public class ProductDetail
    {
        public int id { get; set; }

        public string productCode { get; set; } //판매처 상품코드

        public string name { get; set; }

        public string mainImage { get; set; }

        public string productUrl{get; set;}

        public List<Detail> detail { get; set; } //상품정보고시내용

        public string description { get; set; }

        public string deliveryTime { get; set; }
        public int shippingFee { get; set; }


        public int price { get; set; }

        public Dictionary<int, List<string>> option { get; set; }

        public int sellerId { get; set; }
        public string brandName { get; set; }

        public List<EcoCertifications> ecoCertifications { get; set; }
        
        //진열 = 1, 미진열처리= 0  
        public int status { get; set;} 




    }

    
}
