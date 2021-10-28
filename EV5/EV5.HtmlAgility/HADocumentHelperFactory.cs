using EV5.Mvc.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV5.Mvc.HtmlAgility
{
    public class HADocumentHelperFactory : IDocumentHelperFactory
    {
        public  Mvc.IDocument CreateDocument()
        {
            return new HADocument();
        }

        public  IDocumentHelper<IDocument> CreateDocumentHelper()
        {
            return new HADocumentHelper();
        }

        public  IDocumentHelper<IDocument> CreateDocumentHelper(IDocument document)
        {
            return new HADocumentHelper(document);
        }
    }
}
