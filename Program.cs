using System;
using EcoBot.Crawling;

namespace EcoBot
{
    class Program
    {
        static void Main(string[] args)
        {
<<<<<<< HEAD
            //ProductListCrawler productListCrawler = new ProductListCrawler();
            //productListCrawler.RepacUrl();
            ProductDetailCrawler productDetailCrawler = new ProductDetailCrawler();
            productDetailCrawler.GetProductUrl();
=======
            ProductListCrawler productListCrawler = new ProductListCrawler();
            productListCrawler.RepacUrl();
            productListCrawler.RegroundUrl();
>>>>>>> 396eef6 (리그라운드url 수집 로직  추가)
        }
    }
}