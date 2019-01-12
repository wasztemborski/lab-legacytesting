using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MockService
{
    public class BaseService
    {
        protected CancellationTokenSource cts;
        protected TimeSpan timeout = TimeSpan.FromSeconds(5);
        protected IConfigurationRoot configuration;

        public BaseService(IConfigurationRoot configuration)
        {
            this.configuration = configuration;
        }

        public void Start()
        {
            Console.WriteLine("Service started..");            
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("Configuration to override by enviroments or args....");
            Console.WriteLine(" {0}: {1}", ConfigValues.MODE, configuration[ConfigValues.MODE]);
            Console.WriteLine(" {0}: {1}", ConfigValues.IN_FOLDER, configuration[ConfigValues.IN_FOLDER]);
            Console.WriteLine(" {0}: {1}", ConfigValues.OUT_FOLDER, configuration[ConfigValues.OUT_FOLDER]);
            Console.WriteLine(" {0}: {1}", ConfigValues.CONNECTION_STRING, configuration[ConfigValues.CONNECTION_STRING]);
            Console.WriteLine("---------------------------------------------------");

            //configuration.AsEnumerable().ToList().ForEach(p => Console.WriteLine("{0} : {1}", p.Key, p.Value));
            cts = new CancellationTokenSource();            
            Task.Factory.StartNew(()=> ProcessLoop(), cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void Stop()
        {
            if (!cts.IsCancellationRequested) cts.Cancel();
        }


        private void ProcessLoop()
        {
            for (; ; )
            {
                cts.Token.WaitHandle.WaitOne(timeout);
                Process();
            }
        }

        protected virtual void Process() { }
        
    }
}
