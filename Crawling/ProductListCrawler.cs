using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBot.Crawling
{
    public class ProductListCrawler : BaseCrawler
    {
        public void memo()
        {
            // 상속 받은 BaseCrawler 클래스에서 InheritanceExample 함수 사용 예시
            string sample = InheritanceExample("랄라");

            // 의미 없는 for문 테스트
            for (int i = 0; i < sample.Length; i++)
            {
                Console.WriteLine(sample);
            }
        }
    }
}
