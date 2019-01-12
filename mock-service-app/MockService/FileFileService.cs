using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace MockService
{
    public class FileFileService:BaseService
    {
        public FileFileService(IConfigurationRoot configuration) 
            : base(configuration) { }       


        protected override void Process()
        {
            var infolder = configuration[ConfigValues.IN_FOLDER];
            var outFolder = configuration[ConfigValues.OUT_FOLDER];            

            var files = Directory.GetFiles(infolder);
            

            foreach (var f in files)
            {
                Console.WriteLine("Processing file: {0}", f);               
                
                var fileName = Path.GetFileName(f);
                var archfile = Path.GetFullPath(fileName, string.Format(@"{0}\archive", Path.GetFullPath(infolder)));

                File.AppendAllLines(

                    Path.GetFullPath(fileName, Path.GetFullPath(outFolder)),
                    File.ReadAllLines(f).ToList().Select(x => string.Format("{0},{1},{2}", x, fileName, DateTime.UtcNow))
                );

                Console.WriteLine("Archiving file: {0}", archfile);
                
                if (File.Exists(archfile))
                {
                    File.Delete(archfile);
                }

                File.Move(f, archfile);
            }
        }
    }
}
