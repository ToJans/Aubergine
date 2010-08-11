using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aubergine.Services;
using Aubergine.Services.Extractors;
using Aubergine.Interfaces;
using Aubergine.Extensions;
using System.IO;

namespace Aubergine.ConsoleRunner
{
    class Program
    {
        static int Main(string[] args)
        {
            var p = new CommandLineParameters();
            p.Define().Name("html").Describe("[Experimental] Output to html unordered lists").Allow(ParameterType.Flag);
            p.Define().Name("fmthdr", "formatheader").Describe("Header to put before starting the children output; example \"<ul>\"");
            p.Define().Name("fmt", "formatter").Describe("Named storyformatter; example \"<li>{Type} {Description} <a href='#' title='{StatusInfo}'>{StatusText}</a></li>\"");
            p.Define().Name("fmtftr", "formatfooter").Describe("Footer to put after ending the children output; example \"</ul>\"");
            p.Define().Name("h", "?", "help").Describe("Show the syntax of the commandline parameters").Allow(ParameterType.Flag);
            p.Define().Name("v", "verbose").Describe("Show verbose output").Allow(ParameterType.Flag);
            p.Define().Name("p", "parseonly").Describe("Only parse the files to look for syntax errors").Allow(ParameterType.Flag);
            p.Define().Name("d", "dsl").Describe("Show the available DSL definitions").Allow(ParameterType.Flag);
            p.Define().Name("c", "context").Describe("The full classname of the default context class to use").Allow(ParameterType.Single);
            p.Define().Name("a", "assembly").Describe("The full name of the assembly the default context class is in").Allow(ParameterType.Single);
            p.Define().Name("db","debugbreak").Describe("Invoke an assert so you can attach a debugger").Allow(ParameterType.Flag);
            //p.Define().Name("s", "syntax").Describe("Filename of a syntax definition file").Allow(ParameterType.Single);
            p.ParseLine(string.Join(" ", args));

            if (p["help"].Count > 0)
            {
                ShowHeader("","");
                ShowUsage(p);
                return 0;
            }

            if (p.Unnamed.Count == 0 && p["d"].Count ==0)
            {
                ShowHeader("","");
                Console.WriteLine("Error : no story files defined or dsl requested; use the switch /h for usage info");
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


            ShowHeader(fmthdr[0],fmtftr[0]);


            var storyextractor = new TextStoryExtractor();
            var dslextractor = new TextDSLExtractor();

            if (p["a"].Count > 0 && p["c"].Count > 0)
            {
                dslextractor.AddDSL("default", p["c"][0], p["a"][0]);
            }


            Console.Write(fmthdr[0]);
            Console.WriteLine("Parsing files");
            foreach (var arg in p.Unnamed)
            {
                Console.Write(fmthdr[0]);
                Console.WriteLine("  Processing file(s) : " + arg);
                foreach (var fn in Directory.GetFiles(Directory.GetCurrentDirectory(), arg))
                {
                    Console.Write(fmthdr[0]);
                    Console.WriteLine("    Parsing file : " + fn);
                    foreach (var line in File.ReadAllLines(fn))
                    {
                        if (string.IsNullOrEmpty(line) || line.Trim() == "") continue;
                        var st = "Skipped";
                        if (dslextractor.ParseLine(line))
                        { st = "Parsed DSL"; }
                        else if (storyextractor.ParseLine(line))
                        { st = "Parsed element"; }
                        if (p["v"].Count > 0)
                        {
                            Console.Write(fmthdr[0]);
                            Console.WriteLine("      "+st+" : " + line);
                            Console.Write(fmtftr[0]);
                        }
                    }
                    Console.Write(fmtftr[0]);
                }
                Console.Write(fmtftr[0]);
            }
            Console.Write(fmtftr[0]);
            if (p["v"].Count + p["p"].Count > 0)
            {
                Console.Write(fmthdr[0]);
                Console.WriteLine("Stories:");
                foreach (var sc in storyextractor.Stories)
                {
                    Console.Write(fmthdr[0]);
                    Console.Write("  "+sc.Story.Description);
                    Console.WriteLine(fmtftr[0]);
                }
                Console.WriteLine(fmtftr[0]);
            }
            if (p["v"].Count + p["d"].Count > 0)
            {
                Console.Write(fmthdr[0]);
                Console.WriteLine("DSLs:");
                foreach (var sc in dslextractor.DSLs.Values)
                {
                    Console.Write(fmthdr[0]);
                    Console.WriteLine("  "+ sc.Name + " [" + sc.ContextType.FullName + "]" );
                    foreach (var r in sc.Elements)
                    {
                        Console.Write(fmthdr[0]);
                        Console.Write("    "+r.Description);
                        Console.WriteLine(fmtftr[0]);
                    }
                    Console.WriteLine(fmtftr[0]);
                }
                Console.WriteLine(fmtftr[0]);
            }

            if (p["db"].Count > 0)
            {
                Console.WriteLine("Please attach the debugger to this process and press return");
                Console.ReadLine();
            }

            if (p["p"].Count == 0)
            {
                var runner = new SpecRunner();

                Console.Write(fmthdr[0]);
                Console.WriteLine("Running Tests");
                foreach (var sc in storyextractor.Stories)
                {
                    Console.WriteLine(fmthdr[0]);
                    var dsl = runner.GetDSL(sc.Story, dslextractor.DSLs);
                    dsl = dsl ?? dslextractor.DSLs["default"];
                    PrettyPrint(runner.RunStory(sc.Story, dsl, sc.ColumnToken), 1, fmthdr[0], fmt[0], fmtftr[0]);
                    Console.WriteLine(fmtftr[0]);
                }
                Console.Write(fmtftr[0]);
            }
            return 0;
        }

        static void PrettyPrint(ISpecElement v, int depth,string header,string fmt,string footer)
        {
            var whitespace = "".PadLeft(depth * 4);
            Console.WriteLine(whitespace+fmt.FormatWith(v).Replace("GivenIdid","Given I did"));
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
