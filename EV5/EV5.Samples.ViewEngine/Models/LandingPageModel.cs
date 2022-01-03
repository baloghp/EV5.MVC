using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV5.Samples.ViewEngine.Models
{
    public class LandingPageModel
    {
        public Header Header { get; set; }
        public IList<Content> Contents { get; set; }
        internal static LandingPageModel GetSample()
        {
            return new LandingPageModel()
            {
                Header = new Header()
                {
                    Header1 = "::Header 1 from Model - via view processing",
                    Header2 = "::Header 2 from Model - via databinding"
                },
                Contents = new List<Content>
                {
                    new Content(){
                        Heading = "::Content 1 Heading from Model",
                        Lead = "::Content 1 Lead from Model"
                    },
                    new Content(){
                        Heading = "::Content 2 Heading from Model",
                        Lead = "::Content 2 Lead from Model"
                    },
                      new Content(){
                       Heading = "::Content 3 Heading from Model",
                        Lead = "::Content 3 Lead from Model"
                    },
                },
            };
        }
    }


    public class Header
    {
        public string Header1 { get; set; }
        public string Header2 { get; set; }
    }

    public class Content
    {
        public string Heading { get; set; }
        public string Lead { get; set; }
    }


}
