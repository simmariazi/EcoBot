﻿using System;
using EcoBot.Crawling;

namespace EcoBot
{
    class Program
    {
        static void Main(string[] args)
        {
            ProductDetailCrawler productdetailCrawler = new ProductDetailCrawler();
            ProductListCrawler productListCrawler = new ProductListCrawler();
            //productListCrawler.RepacUrl();
            //productListCrawler.RegroundUrl();
            //productListCrawler.LowlesUrl();
            //productdetailCrawler.GetProductUrl();
            //productdetailCrawler.GetRegroundUrl();
            //productdetailCrawler.GetLowlesDetail();
            productdetailCrawler.GetNeezmallUrl();
            //productListCrawler.BaseUrl();

        }
    }
}