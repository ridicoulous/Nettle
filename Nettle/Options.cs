using CommandLine;

namespace Nettle
{
    public class Options
    {
        public Options()
        {
            
        }
        public Options(string file, int espg, string output, bool swap, string delimeter)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
            Espg = espg;
            Output = output ?? throw new ArgumentNullException(nameof(output));
            Swap = swap;
            Delimeter = delimeter ?? throw new ArgumentNullException(nameof(delimeter));
        }

        [Option('f', "file", Required = false, HelpText = "Input csv [#,time,description,coords] file to be processed.", Default ="nettle.csv")]
        public string File { get; private set; }

        [Option('e', "epsg", Default = 5565, Required = false,  HelpText = "Id of SRID to convert from. Default: https://epsg.io/5565")]
        public int Espg{ get; private set; }

        [Option('o', "output", Required = false, HelpText = "Path to sqlite db file", Default = "result.sqlite")]   
        public string Output { get; private set; }
        [Option('s', "swap", Required = false, HelpText = "Swap X and Y from input or not", Default = true)]
        public bool Swap { get; private set; }
        [Option('d', "delimeter", Required = false, HelpText = "CSV file delimeter", Default = ";")]
        public string Delimeter { get; private set; }
    }
}
