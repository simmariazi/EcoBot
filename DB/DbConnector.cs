using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBot.DB
{
    public class DbConnector
    {
        public MySqlConnection ConnectionDB()
        {
            string connectionString = ConfigurationManager.AppSettings.Get("CONNECTION_STRING");

            MySqlConnection connection = new MySqlConnection(connectionString);

            return connection;
        }
    }
}
