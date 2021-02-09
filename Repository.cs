using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Data.SqlClient;
using FarmacyControl.Models;
using System.Diagnostics;

namespace FarmacyControl
{
    public class Repository : IRepository
    {
        private string _configuration;

        public Repository(IConfiguration configuration)
        {
            _configuration = configuration.GetValue<string>("DataBaseInfo:ConnectionString");
        }

        internal IDbConnection dbConnection
        {
            get
            {
                return new SqlConnection(_configuration);
            }

        }

        public string GetSomeTestData()
        {
            using (IDbConnection connection = dbConnection)
            {
                return connection.Execute("SELECT Number FROM Documents.OrderHeaders WHERE StoreId = 'B2D6E185-FC65-423F-8188-38F5CFD3E3D2'").ToString();

            }
        }        

        public List<Mrc> GetMrc()
        {
            using (IDbConnection connection = dbConnection)
            {
                return connection.Query<Mrc>("SELECT  *  FROM [References].[ManufactorGoodsPrices]").ToList();
            }
        }

        public void WriteToDb(Mrc mrcs)
        {
            using (IDbConnection connection = dbConnection)
            {
                connection.Execute("Insert into [References].[ManufactorGoodsPrices] (NNT, Price) Values(@Nnt, @Price)", mrcs);
            }
        }
        public bool GetMissedOrders()
        {
            using (IDbConnection connection = dbConnection)
            {
                string sqlCommand = ($@"SELECT COUNT(DISTINCT s.StoreId)
                                    FROM[Documents].[OrderStatuses] s
                                    JOIN[Documents].[OrderHeaders] h ON h.OrderId = s.OrderId
                                    INNER JOIN[References].PN_PharmacySync p ON p.TableRowGUID = h.StoreId
                                    INNER JOIN[Documents].[OrderRows] r ON r.OrderId = s.OrderId
                                    JOIN[References].[ProductsSync] pr ON pr.id_ISS = r.Nnt
                                    JOIN[References].[UnionNetSync] n ON n.id = p.id_pn_unionnet
                                    WHERE n.Real_Net_Guid = 'efb05410-ba92-4a73-a37f-f05f9a499ded'
                                    AND s.TimeStamp > '2021-01-01'
                                    AND s.TimeStamp < '2021-01-04'
                                    HAVING COUNT(DISTINCT s.StoreId) > 0");
                string spName = "[Monitoring].[ExecQueryShards]";

                SqlCommand command = new SqlCommand(sqlCommand, (SqlConnection)connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = spName;
                SqlParameter parameter = new SqlParameter();
                parameter.ParameterName = "@text";
                parameter.SqlDbType = SqlDbType.NVarChar;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = sqlCommand;
                command.Parameters.AddWithValue("@text", sqlCommand);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Debug.WriteLine("{0}", reader[0]);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                    reader.Close();
                }               
            }
            return true;
        }


    }
}


