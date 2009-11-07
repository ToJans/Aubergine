using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Corebvba.Aubergine.Examples.Website.Contexts;

namespace Be.Corebvba.Aubergine.Examples.Website.Stories
{
    class Make_sure_my_website_gets_enough_visibility: Story<BrowserContext>
    {
        As_a website_owner;
        I_want to_make_sure_that_I_get_enough_visibility;
        So_that I_can_get_enough_traffic;

        [Cols("searchengine","search_url","keywords","my_url")]
        [Data("google","http://www.google.be/search?q=","core bvba tom janssens","www.corebvba.be")]
        [Data("google","http://www.google.be/search?q=","BDD .Net","www.corebvba.be")]
        [Data("bing","http://www.bing.com/search?q=","core bvba tom janssens","www.corebvba.be")]
        class Search_results_for_keywords_on_searchengine_should_contain_my_url : Scenario
        {
            Given current_url_is_search_url;
            When searching_for_keywords;
            Then result_should_contain_my_url;
        }
    }   
}
