using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBot.Crawling
{
    public class BaseCrawler
    {
        // 상속 예시를 위한 예제 함수 작성
        //public string InheritanceExample(string lalla)
        //{
        //    string result = string.Empty;

        //    result = lalla + lalla;

        //    return result;
        //}
        public string RepacUrl()
        {
            //url = job의 셀러아이디 1 
            string url = "";
            ProductListCrawler getRepac = new ProductListCrawler();
            getRepac.GetUrl(url);
            return url;
        }
    }
}
