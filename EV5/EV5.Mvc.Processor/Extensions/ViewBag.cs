using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EV5.Mvc.Processor.Extensions
{
    public static class ViewBag
    {
        public const string ViewBagAttribute = "eve-viewbag";
        /// <summary>
        /// Processes the html document's tags with ViewBagAttribute ("eve-viewbag") attributes, 
        /// by evaluating the given attribute value on the given viewContext's viewbag, and inserting the result into the tag.
        /// </summary>
        /// <param name="documentHelper">Document this extension is attached on</param>
        /// <param name="viewContext">ViewContext of the ViewBag</param>
        /// <returns></returns>
        public static IDocumentHelper<T> ProcessViewBag<T>(this IDocumentHelper<T> documentHelper, ViewContext viewContext) where T : IDocument
        {
            if (viewContext == null)
                throw new ArgumentNullException("viewContext");
            if (viewContext.ViewBag == null)
                throw new ArgumentNullException("viewContext.ViewBag");
            //Sequential processing is required because the evaluation needs the http context that 
            // parallel implementation does not have on all threads
            documentHelper.ProcessNodesWithAttributeSequential(ViewBagAttribute, new Func<IDocumentNode, string>(a =>
            {
                var dynaPath = a.GetAttributeValue(ViewBagAttribute);
                var value = GetProperty(viewContext.ViewBag, dynaPath);
                //ViewDataEvaluator.Eval(viewContext.ViewBag, dynaPath);
                return value?.ToString();
            }
               ));
            return documentHelper;
        }

        public static object GetProperty(object o, string member)
        {
            if (o == null) throw new ArgumentNullException("o");
            if (member == null) throw new ArgumentNullException("member");
            Type scope = o.GetType();
            IDynamicMetaObjectProvider provider = o as IDynamicMetaObjectProvider;
            if (provider != null)
            {
                ParameterExpression param = Expression.Parameter(typeof(object));
                DynamicMetaObject mobj = provider.GetMetaObject(param);
                GetMemberBinder binder = (GetMemberBinder)Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, member, scope, new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(0, null) });
                DynamicMetaObject ret = mobj.BindGetMember(binder);
                BlockExpression final = Expression.Block(
                    Expression.Label(CallSiteBinder.UpdateLabel),
                    ret.Expression
                );
                LambdaExpression lambda = Expression.Lambda(final, param);
                Delegate del = lambda.Compile();
                return del.DynamicInvoke(o);
            }
            else
            {
                return o.GetType().GetProperty(member, BindingFlags.Public | BindingFlags.Instance).GetValue(o, null);
            }
        }
    }
}
