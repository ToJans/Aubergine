using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using Aubergine.Model;

namespace Aubergine.Tests.Website.Contexts
{
    public class BrowserContext
    {
        public string Url { get; set; }
        public string Result { get; set; }

        private WebClient wc = new WebClient();
            
        [DSL("the current url is '(?<url>.*)'")]
        void SetUrl(string url)
        {
            Url = url;
        }

        [DSL("searching for '(?<keywords>.*)'")]
        void SearchForKeyWords(string keywords)
        {
            Result = wc.DownloadString(Url + HttpUtility.UrlEncode(keywords));
        }

        [DSL("searching for the following keywords")]
        void SearchForKeyWords(string[] keywords)
        {
            Result = wc.DownloadString(Url + HttpUtility.UrlEncode(string.Join(" ",keywords)));
        }


        [DSL("the result should contain '(?<myurl>.*)'")]
        bool ResultShouldContain(string myurl)
        {
            return (Result??"").Contains(myurl);
        }

        [DSL("the result should contain '(?<avalue>.+)' and the following markup elements")]
        bool ResultShouldContain(string avalue,string[] type,string[]inner)
        {
            if (string.IsNullOrEmpty(Result) ||type.Length==0) 
                return false;
            if (!Result.Contains(avalue)) return false;
            for (var i = 0;i < type.Length ; i++)
            {
                var searchstring = string.Format("<{0}>{1}</{0}>", type[i], inner[i]);
                if (!Result.Contains(searchstring))
                    return false;
            }
            return true;
        }
    }
}
