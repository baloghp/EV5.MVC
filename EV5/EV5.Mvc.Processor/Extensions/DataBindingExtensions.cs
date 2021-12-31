using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV5.Mvc.Processor.Extensions
{

    /// <summary>
    /// Extension methods for databinding
    /// </summary>
    public static class DataBindingExtensions
    {

        public const string EvalAttribute = "eve-eval";
        /// <summary>
        /// Processes the html document's tags with EvalAttribute ("eve-eval"), 
        /// by evaluating the given attribute value on the given Model, and inserting the result into the tag
        /// </summary>
        /// <param name="documentHelper">Document to attach this functionality on</param>
        /// <param name="Model">Model on which the evaluation is done</param>
        /// <returns></returns>
        public static IDocument ProcessEvals(this IDocument document, object Model)
        {
            document.ProcessNodesWithAttribute(EvalAttribute, new Func<IDocumentNode, string>(a =>
            {
                //var evalPath = a.Attributes[EvalAttribute].Value;
                var evalPath = a.GetAttributeValue(EvalAttribute);
                //eval the new value on the current model
                if (Model != null && !string.IsNullOrWhiteSpace(evalPath))
                {
                    var evalValue = ViewDataEvaluator.Eval(Model, evalPath.Trim());
                    return evalValue == null ? string.Empty : evalValue.Value.ToString();
                }
                return null;
            }
               ));
            return document;
        }
    }
}
