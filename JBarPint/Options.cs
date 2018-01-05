using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;
using CommandLine.Text;


namespace JBarPint
{
    class Options
    {
        [Option('s', "Serial", Required = true,
            HelpText = "Board Serial")]
        public string BoardSerial { get; set; }

        [Option('p', "Printer", Required = true, 
            HelpText = "Printer Name")]
        public string PrinterName { get; set; }

        [Option('r', "Paper", Required = true, DefaultValue = "THT-103-423",
            HelpText = "Paper Name")]
        public string PaperName { get; set; }

    }
}
