using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace LIBRA.Scan.Data.Extensions
{
    public static class DataContextExtensions
    {
        public static int CountByRawSql(this DbContext dbContext, string sql, params object[] parameters)
        {
            int result = -1;
            SqlConnection connection = dbContext.Database.GetDbConnection() as SqlConnection;

            try
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.AddRange(parameters);
                    using (DbDataReader dataReader = command.ExecuteReader())
                        if (dataReader.HasRows)
                            while (dataReader.Read())
                                result = dataReader.GetInt32(0);
                }
            }

            // We should have better error handling here
            catch (System.Exception e) { }

            finally { connection.Close(); }

            return result;
        }

        public static System.Data.DataTable ExecuteDataTable(this DbContext dbContext, string storedProcedureName,
                                     params SqlParameter[] arrParam)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            // Open the connection
            using (SqlConnection cnn = dbContext.Database.GetDbConnection() as SqlConnection)
            {
                cnn.Open();

                // Define the command
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cnn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = storedProcedureName;

                    // Handle the parameters
                    if (arrParam != null)
                    {
                        foreach (SqlParameter param in arrParam)
                            cmd.Parameters.Add(param);
                    }

                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }
    }
}
