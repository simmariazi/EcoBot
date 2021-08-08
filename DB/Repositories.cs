using EcoBot.Model;
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
        public List<Job> GetJobs(int sellerId)
        {
            List<Job> jobs = new List<Job>();

            CallDb callDb = new CallDb();

            var jobData = callDb.Select("SELECT * FROM job WHERE seller_id = " + sellerId);

            foreach (DataRow data in jobData.Tables[0].Rows)
            {
                jobs.Add(new Job
                {
                    id = int.Parse(data["id"].ToString()),
                    seller_id = int.Parse(data["seller_id"].ToString()),
                    category_id = int.Parse(data["category_id"].ToString()),
                    url = data["url"].ToString()
                });
            }

            return jobs;
        }

        public bool AddProductList(List<ProductList> productLists)
        {
            CallDb callDb = new CallDb();

            string query = string.Empty;

            query += "INSERT INTO product_list (thumbnail, productUrl, seller_id) VALUES ";

            for (int i = 0; i < productLists.Count; i++)
            {
                query += "('" + productLists[i].thumbnail + "','" + productLists[i].productUrl + "'," + productLists[i].seller_id + ")";
                if (i < productLists.Count-1)
                {
                    query += ",";
                }

            }

            return callDb.Insert(query);
        }
        public List<ProductList> GetProductListsById(int id)
        {
            List<ProductList> productLists = new List<ProductList>();

            CallDb callDb = new CallDb();

            var productData = callDb.Select("SELECT * FROM product_list WHERE seller_id = " + id);

            foreach (DataRow data in productData.Tables[0].Rows)
            {
                productLists.Add(new ProductList
                {
                    id = int.Parse(data["id"].ToString()),
                    thumbnail = data["thumbnail"].ToString(),
                    productUrl = data["productUrl"].ToString(),
                });
            }

            return productLists;
        }
    }
}
