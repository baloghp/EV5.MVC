using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using EV5.Mvc.ViewEngine;
using EV5.Mvc.Extensions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http.Features;

namespace EV5.Mvc
{
    /// <summary>
    /// Non-generic version of EmbeddedView<T>.
    /// </summary>
    public abstract class EmbeddedView : EmbeddedView<dynamic>
    {
    }

    /// <summary>
    /// EmbeddedView class. Center point of EmbeddedView engine implementation. This is what all views in the Eve.Mvc system must derive from. 
    /// </summary>
    /// <typeparam name="T">Type of the model for the view</typeparam>
    public abstract class EmbeddedView<T> : IModelContainer<T>, IEmbeddedView 
    {
        private IViewEngine _viewEngine;
        public IViewEngine ViewEngine
        {
            get { return _viewEngine; }
            set { _viewEngine = value; }
        }

        #region Model
        private T _model;

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        /// <exception cref="System.ApplicationException"></exception>

        public T Model
        {
            get
            {
                if (EqualityComparer<T>.Default.Equals(_model, default))
                {
                    if (this.ViewData!=null)  SetModel(this.ViewData.Model);
                    //if (this.ViewData.Model != null && !(this.ViewData.Model is T))
                    //{
                    //    throw new ApplicationException(String.Format("Model passed for this view MUST be of type:{0}, but it is {1}", typeof(T), this.ViewData.Model.GetType()));
                    //}
                    //_model = (T)this.ViewData.Model;
                }
                return _model;
            }
            set
            {
                _model = value;
            }
        }
        

        public void SetModel(object Model)
        {
            if (Model != null && !(Model is T))
            {
                throw new ApplicationException(String.Format("Model passed for this view MUST be of type:{0}, but it is {1}", typeof(T), this.ViewData.Model.GetType()));
            }
            else
            {
                if (Model is T t)
                _model = t;
            }
            
        }


        #endregion

        #region markup and internal html doc
        public string RawMarkup { get; set; }
        private IDocumentHelper<IDocument> _htmlDocument = null;
        /// <summary>
        /// Gets the HTML document.
        /// </summary>
        /// <value>
        /// The HTML document.
        /// </value>
        public IDocumentHelper<IDocument> HtmlDocument
        {
            get
            {
                return _htmlDocument;
            }
            private set
            {
                _htmlDocument = value;
            }
        }
        #endregion

        #region mastername
        private const string NOMASTERVIEWNAME = "___NO___MASTER___DISCOVERED****YET"; //pretty unique, hope no-one will want to name their master view like this
        private string _masterName = NOMASTERVIEWNAME;
        /// <summary>
        /// Gets or sets the name of the master view.
        /// </summary>
        /// <value>
        /// The name of the master view.
        /// </value>
        public string MasterName
        {
            get
            {
                if (_masterName == NOMASTERVIEWNAME)
                {
                    _masterName = DiscoverMasterNameFromAttribute();
                }
                return _masterName;
            }
            set
            {
                _masterName = value;
            }
        }

        private string DiscoverMasterNameFromAttribute()
        {
            if (this.GetType().GetCustomAttributes(typeof(MasterViewAttribute), true).FirstOrDefault() is MasterViewAttribute masterViewAttribute)
            {
                return masterViewAttribute.MasterViewName;
            }
            return null;
        }
        #endregion

        #region sections
        private List<ISection> _sections;
        /// <summary>
        /// Gets the list of sections defined in the markup.
        /// </summary>
        /// <value>
        /// The sections.
        /// </value>
        public IList<ISection> Sections
        {
            get
            {
                return _sections ??= new List<ISection>();
            }
        }
        #endregion

        public ViewContext ViewContext { get; set; }
        /// <summary>
        /// Gets or sets the name of the view.
        /// </summary>
        /// <value>
        /// The name of the view.
        /// </value>
        public string ViewName { get; set; }
        /// <summary>
        /// Gets or sets the view data dictionary.
        /// </summary>
        public ViewDataDictionary ViewData { get; set; }
        /// <summary>
        /// Gets the HTML helper.
        /// </summary>
        /// <value>
        /// The HTML.
        /// </value>
        public IHtmlHelper Html { get; internal set; }
        IViewEngine IEmbeddedView.ViewEngine { get => _viewEngine; set => _viewEngine=value; }

        HtmlHelper IEmbeddedView.Html => throw new NotImplementedException();

        //ViewDataDictionary ViewData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string Path => ViewName;

        public IEmbeddedView Parent { get ; set ; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedView{T}"/> class.
        /// </summary>
        public EmbeddedView()
        {
            
            //this.ViewData = new ViewDataDictionary<dynamic>(null, new { });
        }

        public async  Task RenderAsync(ViewContext context)
        {
            await Task.Run(()=> Render(context));
        }

        /// <summary>
        /// Renders the specified view context by using the specified the writer object.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <param name="writer">The writer object.</param>
        public void Render(ViewContext viewContext)
        {

            //HtmlDocument document;

            //init context sensitive fields
            var syncIOFeature = viewContext.HttpContext.Features.Get<IHttpBodyControlFeature>();
            if (syncIOFeature != null)
            {
                syncIOFeature.AllowSynchronousIO = true;
            }
            TextWriter writer = viewContext.Writer;
            this.ViewContext = viewContext;
            this.ViewData = viewContext.ViewData;
            this.Html = ServicesExtensions.HtmlHelper;
            this.SetModel(this.ViewData.Model);
            //if it has a master prepare that
            if (!string.IsNullOrWhiteSpace(MasterName))
            {
                PrepareMasterPage();
            }
            else
            {
                //if there is no master page, we prepare the raw markup as the current document
                var doc = DocumentHelperFactory.Factory.CreateDocument();
                if (!String.IsNullOrWhiteSpace(RawMarkup))
                    doc.LoadHtml(RawMarkup);
                HtmlDocument = DocumentHelperFactory.Factory.CreateDocumentHelper(doc);
            }


            //handle partial views

            HtmlDocument.ProcessNodesWithAttribute(EveMarkupAttributes.PartialView, GetPartialString, true);

            // collect sections
            // only when the page view is called, not during master or partial
            if (this == viewContext.View)
            {
                CreateSections();
            }

            //allow inheritance modifications to kick in
            BeforeProcessView(viewContext);

            //pass view manipulation to child
            ProcessView(viewContext);

            // insert sectionContent into html
            // only when the page view is called, not during master or partial
            if (this == viewContext.View)
            {
                ProcessSections();
            }

            //save doc to output stream
            HtmlDocument.Document.Save(writer);
        }

        private string GetPartialString(IDocumentNode partialNode)
        {
            //let's get the view name
            var partialName = partialNode.GetAttributeValue(EveMarkupAttributes.PartialView);
            //let's see if the user defined a model for this, by default we pass on the current model
            object partialModel = Model;
            if (partialNode.ContainsAttribute(EveMarkupAttributes.PartialModel))
            {
                var partialModelPath = partialNode.GetAttributeValue(EveMarkupAttributes.PartialModel);
                //eval the new partial model on the current one
                if (Model != null && !string.IsNullOrWhiteSpace(partialModelPath))
                {
                    //partialModel = DataBinder.Eval(Model, partialModelPath);
                    var evalResult = ViewDataEvaluator.Eval(Model, partialModelPath.Trim());
                    partialModel = evalResult.Value;
                }

            }
           
            IHtmlContent partialString;
            if (!GetPartialViewStringFromthisEngine(this.ViewContext, partialName, out partialString, partialModel))
            {
                //call partial MVC view process
                var viewData = new ViewDataDictionary(this.ViewData);
                viewData.Model = partialModel;
                //get the html for the master view, by calling MVC view resolution
                if (this.Html is IViewContextAware) (this.Html as IViewContextAware).Contextualize(ViewContext);
                var task = Task.Run(async () => await this.Html.PartialAsync(partialName, partialModel, viewData));
                task.Wait();
                partialString = task.Result;
            }

            return partialString.ToString();
            //return partialString.ToString();
        }

        private void ProcessSections()
        {
            foreach (var section in Sections)
            {
                var sectionNode = HtmlDocument.Document.SelectSingleNode(EveMarkupAttributes.GetAttributeByValueQuery(EveMarkupAttributes.SectionDefinition, section.Name));
                if (sectionNode != null)
                {
                    var content = string.Concat(section.Contents);
                    sectionNode.RenderValue(content);
                }
            }
        }

        private void CreateSections()
        {
            var sectionDefinitions = HtmlDocument.Document.SelectNodes(EveMarkupAttributes.GetAttributeQuery(EveMarkupAttributes.SectionDefinition));
            if (sectionDefinitions != null)
            {
                Sections.Clear();
                foreach (var item in sectionDefinitions)
                {
                    string sectionName = item.GetAttributeValue(EveMarkupAttributes.SectionDefinition);
                    if (Sections.Count(s => s.Name == sectionName) > 0)
                        throw new ApplicationException(String.Format("Duplicate section definition: {0}", sectionName));

                    ISection section = new Section()
                    {
                        Name = item.GetAttributeValue(EveMarkupAttributes.SectionDefinition),
                        RenderInstead = item.ContainsAttribute(EveMarkupAttributes.RenderInstead)
                    };
                    var sectionContents = HtmlDocument.Document.SelectNodes(EveMarkupAttributes.GetAttributeByValueQuery(EveMarkupAttributes.SectionContentFor, sectionName));
                    if (sectionContents != null)
                    {
                        foreach (var sectionContentNode in sectionContents)
                        {
                            if (!sectionContentNode.ContainsAttribute(EveMarkupAttributes.RenderOnlyContent))
                            {
                                section.Contents.Add(sectionContentNode.OuterHtml);
                            }
                            else
                            {
                                section.Contents.Add(sectionContentNode.InnerHtml);
                            }
                            sectionContentNode.Remove();
                        }

                    }

                    Sections.Add(section);
                }
            }
        }

        private IDocument PrepareMasterPage()
        {
            IHtmlContent masterString;
            //let's see if our own view engine can give us the master view string
            if (!GetPartialViewStringFromthisEngine(this.ViewContext, MasterName, out masterString, Model))
            {
                //get the html for the master view, by calling MVC view resolution
                //this is not nice as Html.Partial is falling out of favor now, but I could not find better way
               if (this.Html is IViewContextAware) (this.Html as IViewContextAware).Contextualize(ViewContext);
                var task = Task.Run(async () => await this.Html.PartialAsync(MasterName, Model, this.ViewData));
                task.Wait();
                masterString = task.Result;
            }

            //let's prepare that as our main doc
            var masterDoc = DocumentHelperFactory.Factory.CreateDocument();// new HtmlDocument();
            masterDoc.LoadHtml(masterString.ToString());

            //let's see where to put the current view
            var renderBodyNode = masterDoc.SelectSingleNode(EveMarkupAttributes.GetAttributeQuery(EveMarkupAttributes.RenderBody));
            //if we could not find the place there is trouble
            if (renderBodyNode == null)
                throw new ApplicationException("Master does not define node with 'eve-renderbody' attribute");
            //let's see if we should render the current view into the tag, or instead
            // we put the raw markup in there
            renderBodyNode.RenderValue(RawMarkup);

            // and we make the masterdoc our current operating document
            HtmlDocument = DocumentHelperFactory.Factory.CreateDocumentHelper(masterDoc);
            return masterDoc;
        }

        private bool GetPartialViewStringFromthisEngine(ViewContext viewContext, string viewName, out IHtmlContent result, object model = null)
        {
            ViewEngineResult viewResult = this.ViewEngine.FindView(viewContext, viewName, true);
            result = null;
            if (viewResult.View == null) return false;

            string rawHtml;
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter tw = new StreamWriter(ms))
            {
                var ev = viewResult.View as IEmbeddedView;
                    ev.Parent = this;
                    if (model != null)
                    {
                        
                        ev.SetModel(model);

                    }
                    //need to do the switcheroo as now the write is attached to the context
                    //this part is very much not nice, we have to find another way to be abel to render to writers outside of viewcontext
                    TextWriter old = viewContext.Writer;
                    viewContext.Writer = tw;
                    ev.Render(viewContext);
                    rawHtml = ev.HtmlDocument.Document.OuterHtml;
                    tw.Flush();
                    viewContext.Writer = old;
                    
                    //using (StreamReader sr = new StreamReader(ms))
                    //{
                    //    rawHtml = sr.ReadToEnd();
                    //}
                }
            }
            if (String.IsNullOrWhiteSpace(rawHtml)) return false;

            result = new HtmlString(rawHtml);
            return true;

        }

       



        /// <summary>
        /// Called the view is processed. Use this in inheritance to provide pre-processing before ProcessView method is called.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        protected virtual void BeforeProcessView(ViewContext viewContext)
        { }

        /// <summary>
        /// Override this in your view to process the view's document.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        public abstract void ProcessView(ViewContext viewContext);


        /// <summary>
        /// Cleans up internal resource intensive objects such as the document.
        /// </summary>
        public void CleanUp()
        {
            if (this.HtmlDocument is IDocumentHelper<IDocument> documentHelper) documentHelper.CleanUp();

            this.HtmlDocument = null;

        }

       

       
    }
}
