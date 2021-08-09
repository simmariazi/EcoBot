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

        public string detailUrl{get; set;}

        public List<Detail> detail { get; set; } //상품정보고시내용

        public string description { get; set; }

        public List<DeliveryInfo> deliveryInfo { get; set; }

        public int price { get; set; }

        public Dictionary<int, List<string>> option { get; set; }

        public int sellerId { get; set; }
        public string brandName { get; set; }

        public List<EcoCertifications> ecoCertifications { get; set; }
        
    }

    
}
