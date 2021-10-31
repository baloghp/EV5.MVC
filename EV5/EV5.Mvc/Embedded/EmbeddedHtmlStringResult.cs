using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace EV5.Mvc
{
    /// <summary>
    /// ActionResult that can be used to serve embedded html strings. Since it implements IHttpActionResult, it can be used in WEB API as well.
    /// </summary>
    public class EmbeddedHtmlStringResult : ContentResult, IActionResult
    {

        string file;
        Assembly assembly;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="file">Resource name for the embedded html file</param>
        /// <param name="assemblyName">Assembly name where the embedded html file is stored. The Assembly must be loaded into the AppDomain</param>
        public EmbeddedHtmlStringResult(string file, string assemblyName)
        {

            this.file = file;
            this.assembly = (from a in AppDomain.CurrentDomain.GetAssemblies()
                             where a.FullName == assemblyName
                             select a).FirstOrDefault();
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="file">Resource name for the embedded html file</param>
        /// <param name="assemblyName">Assembly  where the embedded html file is stored.</param>
        public EmbeddedHtmlStringResult(string file, Assembly assembly)
        {

            this.file = file;
            this.assembly = assembly;
        }

        /// <summary>
        /// Gets the html string as string
        /// </summary>
        /// <returns>embedded html string</returns>
        public string GetResponseString()
        {

            return AssetManager.LoadResourceString(this.file, assembly);

        }

        public async override Task ExecuteResultAsync(ActionContext context)
        {
            ContentResult c = new ContentResult();
            c.Content = GetResponseString();
            c.ContentType = "text/html";
            //c.ContentEncoding = System.Text.Encoding.UTF8;
            await  c.ExecuteResultAsync(context);
        }

        /// <summary>
        ///   Enables processing of the result of an action method by retruning a ContentResult with the content of the embedded html.
        ///   Implementation override from ContentResult
        /// </summary>
        /// <param name="context">Current controller's context</param>
        public void ExecuteResult(ControllerContext context)
        {
            ContentResult c = new ContentResult();
            c.Content = GetResponseString();
            c.ContentType = "text/html";
            //c.ContentEncoding = System.Text.Encoding.UTF8;
            c.ExecuteResultAsync(context);
        }

        /// <summary>
        /// Implementation for IHttpActionResult gets the embedded html in an asynch way
        /// </summary>
        /// <param name="cancellationToken">Asynch cancellation token</param>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
            return Task.FromResult(GetResponseMessage());
        }
        /// <summary>
        /// Gets the embedded html as a HttpResponseMessage
        /// </summary>
        /// <returns>the embedded html as a HttpResponseMessage</returns>
        public HttpResponseMessage GetResponseMessage()
        {

            var html = AssetManager.LoadResourceString(this.file, assembly);

            //TODO: may need to provide further customization of StringContent
            return new HttpResponseMessage()
            {
                Content = new StringContent(html, System.Text.Encoding.UTF8, "text/html")
            };
        }

    }
}
