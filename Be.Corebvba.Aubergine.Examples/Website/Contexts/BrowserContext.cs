using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;

namespace Be.Corebvba.Aubergine.Examples.Website.Contexts
{
    public class BrowserContext
    {
        public string Url { get; set; }
        public string Result { get; set; }

        private WebClient wc = new WebClient();
            
        [DSL("current_url_is_(.*)")]
        void SetUrl(string url)
        {
            Url = url;
        }

        [DSL("searching_for_(.*)")]
        void SearchForKeyWords(string keywords)
        {
            Result = wc.DownloadString(Url + HttpUtility.UrlEncode(keywords));
        }

        [DSL("result_should_contain_(.*)")]
        void ResultShouldContain(string myurl)
        {
            Result.Contains(myurl).ShouldEqual(true);
        }
    }
}
