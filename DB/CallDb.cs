using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBot.DB
{
    public class CallDb
    {
        /// <summary>
        /// SELECT 호출 함수
        /// </summary>
        /// <param name="query">호출 쿼리</param>
        /// <returns>조회 데이터</returns>
        public DataSet Select(string query)
        {
            DataSet dsResult = new DataSet();

            DbConnector connector = new DbConnector();
            var conn = connector.ConnectionDB();
            conn.Open();

            MySqlDataAdapter da = new MySqlDataAdapter(query, conn);

            da.Fill(dsResult);

            // 꼭 해제
            conn.Close();

            return dsResult;
        }

        /// <summary>
        /// UPDATE 호출 함수
        /// </summary>
        /// <param name="query">호출 쿼리</param>
        /// <returns>성공 유무</returns>
        public bool Update(string query)
        {
            bool result = false;

            DbConnector connector = new DbConnector();
            var conn = connector.ConnectionDB();
            conn.Open();

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }

            // 반드시 해제 해준다.
            conn.Close();

            return result;
        }

        /// <summary>
        /// INSERT 호출 함수
        /// </summary>
        /// <param name="query">호출 쿼리</param>
        /// <returns>성공 유무</returns>
        public bool Insert(string query)
        {
            bool result = false;

            DbConnector connector = new DbConnector();
            var conn = connector.ConnectionDB();
            conn.Open();

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }

            // 반드시 해제 해준다.
            conn.Close();

            return result;
        }

        /// <summary>
        /// DELETE 호출 함수
        /// </summary>
        /// <param name="query">호출 쿼리</param>
        /// <returns>성공 유무</returns>
        public bool Delete(string query)
        {
            bool result = false;

            DbConnector connector = new DbConnector();
            var conn = connector.ConnectionDB();
            conn.Open();

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }

            // 반드시 해제 해준다.
            conn.Close();

            return result;
        }
    }
}
