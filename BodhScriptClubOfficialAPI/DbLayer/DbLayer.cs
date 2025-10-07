//using Npgsql;
//using System.Data;
//using System.Data.SqlClient;

//namespace BodhScriptClubOfficialAPI.DbLayer
//{
//    public class DbLayer
//    {
//        private readonly string _connectionString;
//        IConfiguration _configuration;

//        public DbLayer(IConfiguration configuration)
//        {
//            _connectionString = configuration.GetConnectionString("DefaultConnection");

//        }
//        public DataTable ExecuteDataTable(string query, string[] paramNames, object[] paramValues)
//        {
//            DataTable dt = new DataTable();
//            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
//            {
//                using (SqlCommand cmd = new SqlCommand(query, con))
//                {
//                    cmd.CommandType = CommandType.StoredProcedure; // important
//                    if (paramNames != null && paramValues != null && paramNames.Length == paramValues.Length)
//                    {
//                        for (int i = 0; i < paramNames.Length; i++)
//                        {
//                            cmd.Parameters.AddWithValue(paramNames[i], paramValues[i] ?? DBNull.Value);
//                        }
//                    }

//                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
//                    {
//                        da.Fill(dt);
//                    }
//                }
//            }
//            return dt;
//        }



//        public int ExecuteNonQuery(string query, string[] paramNames, string[] paramValues)
//        {
//            using (SqlConnection con = new SqlConnection(_connectionString))
//            using (SqlCommand cmd = new SqlCommand(query, con))
//            {
//                cmd.CommandType = CommandType.StoredProcedure;

//                // Add input parameters
//                if (paramNames != null && paramValues != null && paramNames.Length == paramValues.Length)
//                {
//                    for (int i = 0; i < paramNames.Length; i++)
//                    {
//                        cmd.Parameters.AddWithValue(paramNames[i], paramValues[i]);
//                    }
//                }

//                // Add return value parameter
//                SqlParameter returnParam = new SqlParameter();
//                returnParam.Direction = ParameterDirection.ReturnValue;
//                cmd.Parameters.Add(returnParam);

//                con.Open();
//                cmd.ExecuteNonQuery();

//                // Capture SP RETURN value
//                return (int)returnParam.Value;
//            }
//        }

//    }
//}

using Npgsql;
using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace BodhScriptClubOfficialAPI.DbLayer
{
    public class DbLayer
    {
        private readonly string _connectionString;

        public DbLayer(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Executes a PostgreSQL function that returns a single scalar value (int)
        /// </summary>
        public int ExecuteFunction(string funcName, string[] paramNames, string[] paramValues)
        {
            using var con = new NpgsqlConnection(_connectionString);
            using var cmd = new NpgsqlCommand
            {
                Connection = con,
                CommandType = CommandType.Text,
                CommandText = BuildFunctionQuery(funcName, paramNames)
            };

            for (int i = 0; i < paramNames.Length; i++)
            {
                // Ensure @ prefix for Npgsql
                cmd.Parameters.AddWithValue("@" + paramNames[i], paramValues[i]);
            }

            con.Open();
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Executes a stored procedure or function that returns rows
        /// </summary>
        public DataTable ExecuteProcedure(string procName, string[] paramNames, object[] paramValues)
        {
            DataTable dt = new DataTable();
            using var con = new NpgsqlConnection(_connectionString);

            string paramPlaceholders = "";
            if (paramNames != null && paramValues != null && paramNames.Length == paramValues.Length)
            {
                var placeholders = paramNames.Select(p => "@" + p);
                paramPlaceholders = string.Join(", ", placeholders);
            }

            string sql = $"SELECT * FROM {procName}({paramPlaceholders});";

            using var cmd = new NpgsqlCommand(sql, con)
            {
                CommandType = CommandType.Text
            };

            if (paramNames != null && paramValues != null && paramNames.Length == paramValues.Length)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    cmd.Parameters.AddWithValue("@" + paramNames[i], paramValues[i] ?? DBNull.Value);
                }
            }

            using var da = new NpgsqlDataAdapter(cmd);
            da.Fill(dt);

            return dt;
        }

        /// <summary>
        /// Builds the SQL query for calling a function with parameters
        /// </summary>
        private string BuildFunctionQuery(string funcName, string[] paramNames)
        {
            return $"SELECT {funcName}({string.Join(", ", paramNames.Select(p => "@" + p))})";
        }

       
    }

    
}


