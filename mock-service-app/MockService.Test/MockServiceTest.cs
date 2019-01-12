using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace MockService.Test
{
    [TestClass]
    public class MockServiceTest
    {
        private Dictionary<string, string> configuration;

        [TestInitialize]
        public void Setup()
        {
            configuration = new Dictionary<string, string>();
            configuration["inFolder"] = "";
            configuration["outFolder"] = "";
        }

        [TestMethod]
        public void FileHasBeenMovedFromIn2Out()
        {
            // This should be taken form configuration file/tfs variables
            var infolder = configuration["inFolder"];
            var outFolder = configuration["outFolder"];
            var myparam = "this is my comment"; // sample patemter to pass through

            var testFileName = string.Format("test{0}.csv", DateTime.Now.Ticks);
            var InPpath = Path.Combine(infolder, testFileName);
            var OutPpath = Path.Combine(outFolder, testFileName);

            // save data to file
            File.WriteAllText(InPpath, string.Format(string.Format("{0},{1}", DateTime.UtcNow, myparam)));

            // Wait until ready
            var re = new ManualResetEvent(false);
            var cs = new CancellationTokenSource();
            var isSignalled = false;

            var t = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (File.Exists(OutPpath))
                    {
                        re.Set();
                        cs.Cancel();
                    };
                }
            }, cs.Token);

            isSignalled = re.WaitOne(10000);

            //TODO: Processing the file content to enricha assertion
            Console.WriteLine("Has been the file found within output folder " + isSignalled);   // Assert if file exists
        }
    }
}
