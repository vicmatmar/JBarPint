﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zen.Barcode;

using System.Drawing;
using System.Drawing.Printing;


// -p "Brady IP300 Printer 3.29"
//https://github.com/barnhill/barcodelib

namespace JBarPint
{

    class Program
    {
        static string _board_serial;

        static int Main(string[] args)
        {
            var options = new Options();
            var parser = new CommandLine.Parser(s => { s.MutuallyExclusive = false; });

            var isValid = parser.ParseArguments(args, options);
            if (!isValid)
            {
                Console.WriteLine(CommandLine.Text.HelpText.AutoBuild(options).ToString());
                return -1;
            }

            try
            {
                _board_serial = options.BoardSerial;

                // Set printer
                string name = "";
                foreach (string printer in PrinterSettings.InstalledPrinters)
                {
                    if (printer == options.PrinterName)
                    {
                        name = printer;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(name))
                {
                    Console.WriteLine(options.PrinterName + " printer not found.  Make sure printer is installed...");
                    return -1;
                }

                PrintDocument pdoc = new PrintDocument();
                pdoc.PrinterSettings.PrinterName = name;
                pdoc.PrintPage += Pdoc_PrintPage;

                // Set Paper
                pdoc.PrintController = new StandardPrintController();
                name = "";
                foreach (PaperSize ps in pdoc.PrinterSettings.PaperSizes)
                {
                    //string sz = string.Format("{0} ({1:0.000} x {2:0.000})", ps.PaperName, (float)ps.Width / 100, (float)ps.Height / 100);
                    if (ps.PaperName == options.PaperName)
                    {
                        name = ps.PaperName;
                        pdoc.DefaultPageSettings.PaperSize = ps;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(name))
                {
                    Console.WriteLine(options.PaperName + " paper not found");
                    return -1;
                }

                pdoc.Print();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                return -2;
            }


            return 0;
        }

        private static void Pdoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Draw the serial as text on top of label
            Font font = new Font("Lucida Console", 4, FontStyle.Regular);
            SizeF fsize = e.Graphics.MeasureString(_board_serial, font);
            SolidBrush sb = new SolidBrush(Color.Black);
            int x = (int) ( (e.PageBounds.Width - fsize.Width) / 2 );
            int y = 2;
            e.Graphics.DrawString(_board_serial, font, sb, x, y);

            // Draw barcode below it
            Code128BarcodeDraw barcode = BarcodeDrawFactory.Code128WithChecksum;

            int h = (int)(e.PageBounds.Height - fsize.Height - y - 2);
            Image bi = barcode.Draw(_board_serial, h);
            if (bi.Width > e.PageBounds.Width)
                throw new Exception("Barcode too large to fit on selected paper");

            x = (e.PageBounds.Width - bi.Width) / 2;
            y += (int)(fsize.Height);
            e.Graphics.DrawImage(bi, x+1, y);

            e.Graphics.Dispose();
        }
    }
}
