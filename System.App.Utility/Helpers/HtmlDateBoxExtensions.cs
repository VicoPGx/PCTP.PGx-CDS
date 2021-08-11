using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Linq.Expressions;

namespace System.App.Utility.Helpers
{
    public static class HtmlDateBoxExtensions
    {
        public static MvcHtmlString DateBoxFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, DateTime?>> property)
        {
            return helper.DateBoxFor(property, "d", null);
        }

        public static MvcHtmlString DateBoxFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, DateTime?>> property, string format, object htmlAttributes)
        {
            var viewData = helper.ViewContext.ViewData;
            var date = property.Compile().Invoke(viewData.Model);
            var value = date.HasValue ? date.Value.ToString(format) : string.Empty;
            var name = viewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(property));

            return helper.TextBox(name, value, htmlAttributes);
        }
    }
}
