using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBot.DB
{
    public class Repositories
    {
        public List<string> GetSellerUrl(int sellerId)
        {
            List<string> sellerUrls = new List<string>();

            CallDb callDb = new CallDb();

            callDb.Select("SELECT seller_url FROM seller WHERE id = " + sellerId);

            return sellerUrls;
        }
    }
}
