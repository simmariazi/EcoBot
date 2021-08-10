using System;
using EcoBot.Crawling;

namespace EcoBot
{
    class Program
    {
        static void Main(string[] args)
        {
            ProductListCrawler productListCrawler = new ProductListCrawler();
            //productListCrawler.RepacUrl();
            //productListCrawler.RegroundUrl();
            productListCrawler.LowlesUrl();
            //productListCrawler.BaseUrl();

        }
    }
}