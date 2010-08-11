/*
 * Source from :
 *  http://jake.ginnivan.net/2009/07/c-argument-parser/
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Aubergine.Services
{
    public enum ParameterType
    {
        Flag,
        Single,
        Multiple
    }

    public class CommandLineParameter
    {
        public string[] Names { get; set; }
        public string HelpName { get; set; }
        public string Description { get; set; }
        public ParameterType Type { get; set; }
        public List<String> Occurences { get; set; }

        public CommandLineParameter()
        {
            Occurences = new List<string>();
            Type = ParameterType.Single;
        }

        public CommandLineParameter Name(params string[] names)
        {
            Names = names;
            return this;
        }

        public CommandLineParameter Describe(string description)
        {
            Description = description;
            return this;
        }

        public CommandLineParameter Allow(ParameterType pt)
        {
            Type = pt;
            return this;
        }

        public string ExtractValues(string input)
        {
            var oldinput = input;
            var morethenonce = "The following parameter can only be defined once (" + string.Join(",",Names)+")";

            string delims = @"[/-]";
            string regexoption = delims + @"(?<name>{0})";
            string QuotedValue = @"(?<qouted>""(([^""])|(""""))*"")";
            string NonQuotedValue = @"([^\s]+)";
            string SingleValue = "(?<firstvalue>" +NonQuotedValue + ")";
            string ExtraValues = "(," + SingleValue.Replace("first", "extra") + ")*";
            string MultipleValues = "(" + SingleValue + ExtraValues + ")";
            string AssignValues = "[ =:]";
            string RegexFlag = regexoption + @"\s";
            string RegexSingle = regexoption + AssignValues+SingleValue ;
            string RegexMultiple = regexoption + AssignValues + MultipleValues;

            string quoted = "AEADDSFDSFSDSFDFSS";

            var quotedict = new Dictionary<string, string>();

            var rex = "";
            if (Type == ParameterType.Flag) rex = RegexFlag;
            else if (Type == ParameterType.Single) rex = RegexSingle;
            else if (Type == ParameterType.Multiple) rex = RegexMultiple;
            rex = string.Format(rex, ("(" + string.Join( ")|(",Names) + ")").Replace("?","\\?"));
            foreach (var q in Regex.Matches(input, QuotedValue).Cast<Match>().Select(o=>o.Value))
            {
                
                int i = quotedict.Count;
                var n = quoted + quotedict.Count;
                quotedict.Add(n,q);
                input = input.Replace(q, n);
            }
            var mc = Regex.Match(input, rex);
            if (!mc.Success) return oldinput;
            var vals = mc.Groups["firstvalue"].Captures ;
            if (Type == ParameterType.Flag)
            {
                if (mc.Captures.Count==1)
                Occurences.Add("true");
            } 
            else 
            {
                if (vals.Count == 0) return input;
                var vals2 = mc.Groups["extravalue"].Captures;
                if (Type == ParameterType.Single && Occurences.Count + vals.Count + vals2.Count > 1)
                    throw new ArgumentOutOfRangeException(morethenonce);
                Occurences.AddRange((vals.Cast<Capture>().Union(vals2.Cast<Capture>())).Select(o=>o.Value).Select(k=>quotedict.ContainsKey(k)?quotedict[k].Trim('"'):k).Cast<string>());
            }
            input = Regex.Replace(input, rex, " "); 
            foreach (var x in quotedict.Keys)
                input = input.Replace(x,quotedict[x]);
            return input;
        }

        public static IEnumerable<string> ExtractUnnamedValues(string input)
        {
            string QuotedValue = @"(""(([^""])|(""""))*"")";
            string NonQuotedValue = @"([^\s/\-]+)";
            string SingleValue = "(?<firstvalue>" + NonQuotedValue + ")";

            var quotedict = new Dictionary<string,string>();
            var quoted = "hklazejhazzalazeleaz";

            foreach (var q in Regex.Matches(input, QuotedValue).Cast<Match>().Select(o => o.Value))
            {

                int i = quotedict.Count;
                var n = quoted + quotedict.Count;
                quotedict.Add(n, q);
                input = input.Replace(q, n);
            }

            foreach (var q in Regex.Matches(input, SingleValue).Cast<Match>())
            {
                foreach(var c in q.Groups["firstvalue"].Captures.Cast<Capture>().Select(x => x.Value))
                    if (quotedict.ContainsKey(c))
                        yield return quotedict[c].Trim('"');
                    else
                        yield return c;
            }
        }

    }

    public class CommandLineParameters : List<CommandLineParameter>
    {
        private List<string> unnamed = new List<string>();

        public CommandLineParameter Define() { 
            var x = new CommandLineParameter();
            this.Add(x);
            return x;
        }

        public List<string> Unnamed { get { return unnamed; } }

        public static CommandLineParameters Test(string line)
        {
            var x = new CommandLineParameters();
            x.Define().Name("h", "?", "help").Describe("Show the syntax of the commandline parameters").Allow(ParameterType.Flag);
            x.Define().Name("v","verbose").Describe("Show verbose output").Allow(ParameterType.Flag);
            x.Define().Name("s","syntax").Describe("filename of a syntax definition file").Allow(ParameterType.Single);
            x.ParseLine(line);
            return x;
        }

        public List<string> this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name)) return unnamed;
                var l = name.ToLower();
                foreach (var x in this)
                    if (x.Names.Contains(l))
                        return x.Occurences;
                throw new KeyNotFoundException("Unknown parameter : " + name);
            }
        }

        public void ParseLine(string line)
        {
            line += " ";
            foreach (var q in this)
            {
                q.Occurences.Clear();
                line = q.ExtractValues(line);
            }
            unnamed.Clear();
            unnamed.AddRange(CommandLineParameter.ExtractUnnamedValues(line));
        }
    }

}
