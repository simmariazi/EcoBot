using System;
using EcoBot.Crawling;

namespace EcoBot
{
    class Program
    {
        static void Main(string[] args)
        {
            ProductDetailCrawler productdetailCrawler = new ProductDetailCrawler();
            //productListCrawler.RepacUrl();
            //productListCrawler.RegroundUrl();
            //productListCrawler.LowlesUrl();
            productdetailCrawler.GetProductUrl();
            //productdetailCrawler.GetRegroundProducts();
            //productListCrawler.BaseUrl();

        }
    }
}