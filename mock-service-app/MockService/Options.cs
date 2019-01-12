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

namespace MockService {

    class Options {

        [Option('m', "mockServiceMode", Required = false, HelpText = "mMckServiceMode")]
        public string MockServiceMode {get; set;}

        [Option('c', "connectionString", Required = false, HelpText = "Connection string")]
        public string ConnectionString {get; set;}

        [Option('i', "inFolder", Required = false, HelpText = "inFolder")]
        public string inFolder {get; set;}

        [Option('o', "outFolder", Required = false, HelpText = "outFolder")]
        public string outFolder {get; set;}

        [Option("insertSqlCommand", Required = false, HelpText = "insertSql")]
        public string insertSql {get; set;}

        [Option("selectSqlCommand", Required = false, HelpText = "selectSql")]
        public string SelectSql {get; set;}

    }

}