using System;

namespace EV5.Mvc.Services
{
    public interface  IDocumentHelperFactory
    {
        public  IDocumentHelper<IDocument> CreateDocumentHelper();
        public IDocumentHelper<IDocument> CreateDocumentHelper(IDocument document);
        public IDocument CreateDocument();
    }
}
