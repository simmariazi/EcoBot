using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBot
{
    public class ProductList
    {
        private string _thumbnail;
        public string thumbnail 
        {
            get
            {
                if (seller_id == 4)
                {
                    return "https://www.neezmall.com" + _thumbnail;
                }
                return _thumbnail;
            }
            set
            {
                _thumbnail = value;
            }
        }
        public string productUrl { get; set; }
        public int id { get; set; }
        public int seller_id { get; set; }

        public int category_id { get; set; }
    }
}
