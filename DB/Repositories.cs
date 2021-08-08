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

            var jobData = callDb.Select("SELECT seller_url FROM seller WHERE id = " + sellerId);

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
    }
}
