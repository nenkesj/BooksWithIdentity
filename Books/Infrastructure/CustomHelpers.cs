using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Web.Mvc;
using HtmlHelper = System.Web.Mvc.HtmlHelper;
using TagBuilder = System.Web.Mvc.TagBuilder;

namespace Books.Infrastructure
{
    public static class CustomHelpers
    {

        public static MvcHtmlString ListArrayItems(this HtmlHelper html, string[] list)
        {

            TagBuilder tag = new TagBuilder("ul");

            foreach (string str in list)
            {
                TagBuilder itemTag = new TagBuilder("li");
                itemTag.SetInnerText(str);
                tag.InnerHtml += itemTag.ToString();
            }

            return new MvcHtmlString(tag.ToString());
        }

        public static MvcHtmlString MathsFormula(this HtmlHelper html, string formula)
        {
            //string encodedMessage = html.Encode(formula);
            string result = String.Format("{0}", formula);
            return new MvcHtmlString(result);
        }

        public static string EncodeIt(this HtmlHelper html, string text)
        {
            string encodedMessage = html.Encode(text);
            string result = String.Format("{0}", encodedMessage);
            return result;
        }
        public static MvcHtmlString ContainsHtml(this HtmlHelper html, string paragraph)
        {
            //string encodedParagraph = html.Encode(paragraph);
            string result = String.Format("{0}", paragraph);
            return new MvcHtmlString(result);
        }
    }
}