using System;
using EcoBot.Crawling;

namespace EcoBot
{
    class Program
    {
        static void Main(string[] args)
        {
            //ProductListCrawler productListCrawler = new ProductListCrawler();
            //productListCrawler.RepacUrl();
            ProductDetailCrawler productDetailCrawler = new ProductDetailCrawler();
            productDetailCrawler.GetProductUrl();
        }
    }
}