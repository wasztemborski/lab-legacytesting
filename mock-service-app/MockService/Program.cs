using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using CommandLine;

namespace MockService
{
    class Program
    {

        static void Main(string[] args)
        {
          
            //EXAMPLE:  dotnet MockSerice.dll mock-service-mode=file-file 

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())                
                .AddInMemoryCollection(ArgsToKeyValuePairs(args)) //Override with ARGS
                .AddEnvironmentVariables(); // Override with Enviroment if defined
                
            IConfigurationRoot configuration = builder.Build();

            Console.WriteLine(configuration["connectionString"] );
            Console.WriteLine(ConfigValues.MODE );

            var mode = (configuration[ConfigValues.MODE] == ConfigValues.FILE_FILE) ? ConfigValues.FILE_FILE : ConfigValues.SQL_FILE_FILE;
            var service = mode == ConfigValues.FILE_FILE ? (BaseService) new FileFileService(configuration) : (BaseService) new SqlFileService(configuration);
            service.Start();

            while (true) { };
        }

        private static IEnumerable<KeyValuePair<string, string>> ArgsToKeyValuePairs(string[] args)
        {
            var configPairs = new List<KeyValuePair<string, string>>();
            CommandLine.Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(opts => {

                configPairs.Add(new KeyValuePair<string, string>("mockServiceMode", opts.MockServiceMode));
                configPairs.Add(new KeyValuePair<string, string>("inFolder", opts.inFolder));
                configPairs.Add(new KeyValuePair<string, string>("outFolder", opts.outFolder));
                configPairs.Add(new KeyValuePair<string, string>("insertSqlCommand", opts.insertSql));
                configPairs.Add(new KeyValuePair<string, string>("selectSqlCommand", opts.SelectSql));
                configPairs.Add(new KeyValuePair<string, string>("connectionString", opts.ConnectionString));
      
            })
            .WithNotParsed<Options>((errs) => Console.WriteLine(errs));           
        
            return configPairs;
        }


    }
}
