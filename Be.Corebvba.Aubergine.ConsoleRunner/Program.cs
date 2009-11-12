using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Corebvba.Aubergine.Services;
using Be.Corebvba.Aubergine.Services.Extractors;
using Be.Corebvba.Aubergine.Interfaces;
using Be.Corebvba.Aubergine.Extensions;
using System.IO;

namespace Be.Corebvba.Aubergine.ConsoleRunner
{
    class Program
    {
        static int Main(string[] args)
        {
            var p = new CommandLineParameters();
            p.Define().Name("html").Describe("Output to html unordered lists").Allow(ParameterType.Flag);
            p.Define().Name("fmthdr", "formatheader").Describe("Header to put before starting the children output; example \"<ul>\"");
            p.Define().Name("fmt", "formatter").Describe("Named storyformatter; example \"<li>{Type} {Description} <a href='#' title='{StatusInfo}'>{StatusText}</a></li>\"");
            p.Define().Name("fmtftr", "formatfooter").Describe("Footer to put after ending the children output; example \"</ul>\"");
            p.Define().Name("h", "?", "help").Describe("Show the syntax of the commandline parameters").Allow(ParameterType.Flag);
            //p.Define().Name("v", "verbose").Describe("Show verbose output").Allow(ParameterType.Flag);
            //p.Define().Name("s", "syntax").Describe("Filename of a syntax definition file").Allow(ParameterType.Single);
            p.ParseLine(string.Join(" ", args));

            if (p["help"].Count > 0)
            {
                ShowHeader("","");
                ShowUsage(p);
                return 0;
            }

            if (p.Unnamed.Count == 0)
            {
                ShowHeader("","");
                Console.WriteLine("Error : no story files defined; use the switch /h for usage info");
                return -1;
            }
            
            var fmthdr = p["fmthdr"];
            var fmt = p["fmt"];
            var fmtftr = p["fmtftr"];
            
            if (p["html"].Count > 0)
            {
                fmthdr.Clear();fmthdr.Add("<ul>");
                fmt.Clear(); fmt.Add("<li>{Type} {Description} <a href='#' title='{StatusInfo}'>{StatusText}</a></li>");
                fmtftr.Clear(); fmtftr.Add("</ul>");
            }

            if (fmthdr.Count == 0) fmthdr.Add("");
            if (fmt.Count == 0) fmt.Add("{Type} {Description} =>{StatusText} {StatusInfo}");
            if (fmtftr.Count == 0) fmtftr.Add("");

            var extr = new TextExtractor();
            var runner = new ElementRunner();

            ShowHeader(fmthdr[0],fmtftr[0]);

            foreach (var arg in args)
            {
                Console.WriteLine("Processing file(s) : " + arg);
                Console.Write(fmthdr[0]);
                foreach (var fn in Directory.GetFiles(Directory.GetCurrentDirectory(), arg))
                {
                    if (fn.ToLower().Trim()!=arg.ToLower().Trim())
                        Console.WriteLine("  Processing file : " + fn);
                    Console.Write(fmthdr[0]);
                    foreach (var storycontext in extr.ExtractStories(fn))
                    {
                        Console.WriteLine(fmthdr[0]);
                        PrettyPrint(runner.RunStory(storycontext), 1,fmthdr[0],fmt[0],fmtftr[0]);
                        Console.WriteLine(fmtftr[0]);
                    }
                    Console.Write(fmtftr[0]);
                }
                Console.Write(fmtftr[0]);
            }
            return 0;
        }

        static void PrettyPrint(IElement v, int depth,string header,string fmt,string footer)
        {
            var whitespace = "".PadLeft(depth * 4);
            Console.WriteLine(whitespace+fmt.FormatWith(v));
            if (!string.IsNullOrEmpty(header))
                Console.WriteLine(whitespace+header);
            foreach (var c in v.Children)
                PrettyPrint(c, depth+1,header,fmt,footer);
            if (!string.IsNullOrEmpty(footer))
                Console.WriteLine(whitespace + footer);
            if (v.Children.Count() > 0)
                Console.WriteLine("");

        }

        static void ShowUsage(CommandLineParameters clps)
        {
              Console.WriteLine("Usage :\n\n\tConsoleRunner [wildcard or filename for story file][...] [parameters]");
              Console.WriteLine("\nWhere parameters can be one of the following :" );
              foreach(var clp in clps)
              {
                  var typedesc = "  ";
                  if (clp.Type != ParameterType.Flag)
                      typedesc += "XXXX";
                  if (clp.Type == ParameterType.Multiple)
                      typedesc+="[, ...]*";
                   
                  Console.WriteLine("\n\t -"+string.Join(",-",clp.Names) + typedesc);
                  Console.WriteLine("\t\t" + clp.Description);

              }
        }

        static void ShowHeader(string hdr,string ftr)
        {
            if (!string.IsNullOrEmpty(hdr))
                Console.WriteLine(hdr);
            Console.WriteLine("Aubergine Console Runner - Core bvba - Tom Janssens 2009\n");
            if (!string.IsNullOrEmpty(ftr))
                Console.WriteLine(ftr);
        }
    }
}
