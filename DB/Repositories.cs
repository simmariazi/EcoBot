using EcoBot.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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

            var jobData = callDb.Select($"SELECT * FROM job j WHERE j.seller_id = {sellerId}");

            foreach (DataRow data in jobData.Tables[0].Rows)
            {
                jobs.Add(new Job
                {
                    id = int.Parse(data["id"].ToString()),
                    seller_id = int.Parse(data["seller_id"].ToString()),
                    category_id = int.Parse(data["category_id"].ToString()),
                    url = data["url"].ToString(),
                });
            }

            return jobs;
        }

        public List<Job> GetJobs()
        {
            List<Job> jobs = new List<Job>();

            CallDb callDb = new CallDb();

            var jobData = callDb.Select($"SELECT j.*, s.cycle FROM job j, seller s WHERE s.id = j.seller_id");

            foreach (DataRow data in jobData.Tables[0].Rows)
            {
                jobs.Add(new Job
                {
                    id = int.Parse(data["id"].ToString()),
                    seller_id = int.Parse(data["seller_id"].ToString()),
                    category_id = int.Parse(data["category_id"].ToString()),
                    url = data["url"].ToString(),
                    last_crawling_date = Convert.ToDateTime(data["last_crawling_date"].ToString()),
                    cycle = double.Parse(data["cycle"].ToString()),
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
                query += $"('{productLists[i].thumbnail}','{productLists[i].productUrl}',{productLists[i].seller_id})";
                if (i < productLists.Count - 1)
                {
                    query += ",";
                }

            }

            return callDb.Insert(query);
        }

        public bool DeleteConfirmList(List<ProductList> deleteProducts)
        {
            CallDb callDb = new CallDb();

            string query = string.Empty;
            string deleteList = string.Empty;

            for (int i = 0; i < deleteProducts.Count; i++)
            {
                deleteList += deleteProducts[i].id;
                if (i < deleteProducts.Count - 1)
                {
                    deleteList += ",";
                }
            }

            query = $"DELETE FROM productList WHERE id IN ( {deleteList})";

            return callDb.Update(query);
        }

        public bool DeleteProductList(List<ProductList> productLists)
        {
            CallDb callDb = new CallDb();

            //LINQ
            string query_where = string.Join(',', productLists.Select(p => "'" + p.productUrl + "'").ToList());

            string query = $"UPDATE product_list SET is_used=0 WHERE productUrl IN({query_where})";

            return callDb.Update(query);
        }

        public List<ProductList> GetProductListsById(int id)
        {
            List<ProductList> productLists = new List<ProductList>();

            CallDb callDb = new CallDb();

            var productData = callDb.Select($"SELECT * FROM product_list WHERE is_used = 1 AND seller_id = {id}");

            foreach (DataRow data in productData.Tables[0].Rows)
            {
                productLists.Add(new ProductList
                {
                    id = int.Parse(data["id"].ToString()),
                    seller_id = int.Parse(data["seller_id"].ToString()),
                    thumbnail = data["thumbnail"].ToString(),
                    productUrl = data["productUrl"].ToString(),
                });
            }

            return productLists;
        }
        public bool AddProductDetail(List<ProductDetail> productDetails)
        {
            CallDb callDb = new CallDb();

            string query = string.Empty;

            query += "INSERT INTO product (id, name, productcode, mainImage, description, detail, deliveryTime, shippingFee, price, product.option, seller_id, product_url, status) VALUES ";

            for (int i = 0; i < productDetails.Count; i++)
            {
                //옵션 데이터 확인할 것 
                query += $"({productDetails[i].id},'{productDetails[i].name}','{productDetails[i].productCode}','{productDetails[i].mainImage}'," +
                         $"'{productDetails[i].description}', '{string.Join('|', productDetails[i].detail.Select(d => "[{\"brand\" : \"" + d.brand + "\",\"manufacturer\":\"" + d.Manufacturer + "\",\"origin\" : " + "\"" + d.Origin + "\"}]").ToList())}','{productDetails[i].deliveryTime}', '{productDetails[i].shippingFee}', {productDetails[i].price}, '{string.Join(',', productDetails[i].option[0])}', {productDetails[i].sellerId}, '{productDetails[i].productUrl}',{productDetails[i].status})";
                if (i < productDetails.Count - 1)
                {
                    query += ",";
                }

            }

            return callDb.Insert(query);
        }
        public int AddDeliveryinfo(string deliveryTime, int shippingFee)
        {
            CallDb callDb = new CallDb();

            string query = string.Empty;

            query += "INSERT INTO deliveryinfo (deliveryTime, shippingFee) VALUES ";
            query += $"('{deliveryTime}', {shippingFee})";

            callDb.Insert(query);
            var productData = callDb.Select("SELECT MAX(id) as id from deliveryinfo");

            int id = 0;
            foreach (DataRow data in productData.Tables[0].Rows)
            {
                id = int.Parse(data["id"].ToString());

            }

            return id;
        }


        public bool UpdateJobCrawlingDate(int id)
        {
            CallDb callDb = new CallDb();

            string query = string.Empty;

            query += $"UPDATE job SET last_crawling_date = NOW() WHERE id = {id}";


            return callDb.Update(query);
        }
    }
}
