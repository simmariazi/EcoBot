using System;
namespace EcoBot.Model
{
    public class Job
    {
        public int id { get; set; }

        public int seller_id { get; set; }

        public int category_id { get; set; }

        public string url { get; set; }

        public DateTime last_crawling_date { get; set; }

        public double cycle { get; set; }
    }
}
