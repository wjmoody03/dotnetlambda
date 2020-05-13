using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cXMLHandler
{
    public static class HtmlGenerator
    {
        static string cellStyle= @" style=""text-align: left;padding-right:15px;""";
        static string rowStyle=@" style=""border-bottom: 1px solid #ddd;""";
        static string headerStyle= @" style=""background-color: #4f76b5;color: white;text-align: left; padding-right:15px;""";

        public static string ToHtmlTable<T>(this IEnumerable<T> source)
        {
            var header = typeof(T).ToHtmlHeader();
            var body = source.ToHtmlBody();
            return $"<table style=\"border-collapse: collapse;\" >{header}{body}</table>";
        }

        private static string ToHtmlHeader(this Type source)
        {
            string headerFields = string.Join("", source.GetProperties().Select(p => $"<th{headerStyle}>{SplitCamelCaseToWords(p.Name)}</th>"));
            return $"<thead><tr>{headerFields}</tr></thead>";
        }

        private static string ToHtmlBody<T>(this IEnumerable<T> items)
        {
            var allRows = string.Join("", items.Select(i => i.ToHtmlRow<T>()));
            return $"<tbody>{allRows}</tbody>";
        }

        private static string ToHtmlRow<T>(this T source)
        {
            string rowFields = string.Join("", typeof(T).GetProperties().Select(p => $"<td{cellStyle}>{p.GetValue(source)}</td>"));
            return $"<tr{rowStyle}>{rowFields}</tr>";
        }

        public static string SplitCamelCaseToWords(string text)
        {
            string newText = text;
            for(int ix = text.Length-1; ix >= 0; ix--)
            {
                if (Char.IsUpper(text[ix]))
                {
                    newText = newText.Insert(ix, " ");
                }
            }
            newText = newText.Trim();
            newText = newText.Remove(0,1);
            newText = newText.Insert(0, Char.ToUpper(text[0]).ToString());
            return newText;
        }
    }
}
