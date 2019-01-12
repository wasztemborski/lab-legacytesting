using System;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace MockService
{
    public class SqlFileService : BaseService
    {
        public SqlFileService(IConfigurationRoot configuration) 
            : base(configuration) { }       



        // Process checks in the loop i there is no new etry
        // and then process 
        protected override void Process()
        {
            try { 
                var inFolder = configuration[ConfigValues.IN_FOLDER];                
                var connectionString = configuration[ConfigValues.CONNECTION_STRING];                
                var selectSql = configuration[ConfigValues.SELECT_SQL_COMMAND];                
                var insertSql = configuration[ConfigValues.INSERT_SQL_COMMAND];                
                
                // connectionString = "Data Source=mssql;Initial Catalog=xgen;User ID=sa;Password=Password1";               
                //selectSql = @"SELECT TOP 2000 Shareclass, Amount, TradeDate FROM dbo.Deal";
                //insertSql = @"INSERT INTO dbo.Deal Select Shareclass, Amount, TradeDate from  dbo.DealQueue; Delete from dbo.DealQueue;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Connection to {0}", connectionString);
                    
                    int noNewDeals = 0;

                    using (SqlCommand command = new SqlCommand(insertSql, connection))
                    {
                        noNewDeals  = command.ExecuteNonQuery();
                        Console.Write("Detected {0} entries within DealQueue\n", noNewDeals);
                    }                 

                    if (noNewDeals > 0)
                    {
                        using (SqlCommand command = new SqlCommand(selectSql, connection))
                        {
                            var ds = new DataSet();
                            var adapter = new SqlDataAdapter(command);
                            var path = Path.Combine(inFolder, "SQLData.xml");

                            adapter.Fill(ds);

                            var xml = ds.GetXml();
                            
                            File.WriteAllText(path, xml);
                            Console.WriteLine("File {0} populated", path);
                        }    
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
