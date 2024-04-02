using CommandLine;

namespace Nettle
{
    public class Options
    {
        [Option('f', "file", Required = false, HelpText = "Input csv [#,time,description,coords] file to be processed.", Default ="nettle.csv")]
        public string InputFile { get; set; }

        [Option('e', "epsg", Default = 5565, Required = false,  HelpText = "Id of SRID to convert from. Default: https://epsg.io/5565")]
        public int EspgId{ get; set; }

        [Option('o', "output", Required = false, HelpText = "Path to sqlite db file", Default = "result.sqlite")]   
        public string SqliteDbOutputPath { get; set; }
        [Option('s', "swap", Required = false, HelpText = "Swap X and Y from input or not", Default = true)]
        public bool ShouldSwapXandY { get; set; }
        [Option('d', "delimeter", Required = false, HelpText = "CSV file delimeter", Default = ";")]
        public string Delimeter { get; set; }
    }
}
