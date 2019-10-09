﻿using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SqrlForNet
{
    public static class SqrlHtmlHelper
    {
        
        public static HtmlString SqrlLink<TModel>(this IHtmlHelper<TModel> helper, HttpRequest request)
        {
            return SqrlLink(helper, request, "SQRL Login");
        }

        public static HtmlString SqrlLink<TModel>(this IHtmlHelper<TModel> helper, HttpRequest request, string text)
        {
            return SqrlLink(helper, request, text, true);
        }

        public static HtmlString SqrlLink<TModel>(this IHtmlHelper<TModel> helper, HttpRequest request, string text, bool poll)
        {
            return SqrlLink(helper, request, text, poll, int.Parse(request.HttpContext.Items["CheckMillieSeconds"].ToString()));
        }

        public static HtmlString SqrlLink<TModel>(this IHtmlHelper<TModel> helper, HttpRequest request, string text, bool poll, int pollTime)
        {
            var linkTag = new TagBuilder("a");
            linkTag.MergeAttribute("href", request.HttpContext.Items["CallbackUrl"].ToString());
            linkTag.MergeAttribute("onclick", "CpsProcess(this);");
            linkTag.InnerHtml.Append(text);

            var script = new TagBuilder("script");
            script.InnerHtml.AppendHtml("function CpsProcess(e)");
            script.InnerHtml.AppendHtml("{");
            script.InnerHtml.AppendHtml("var gifProbe = new Image();");
            script.InnerHtml.AppendHtml("gifProbe.onload = function() {");
            script.InnerHtml.AppendHtml("document.location.href = \"http://localhost:25519/\"+ btoa(e.getAttribute(\"href\"));");
            script.InnerHtml.AppendHtml("};");
            script.InnerHtml.AppendHtml("gifProbe.onerror = function() {");
            script.InnerHtml.AppendHtml("setTimeout( function(){ gifProbe.src = \"http://localhost:25519/\" + Date.now() + '.gif';}, 250 );");
            script.InnerHtml.AppendHtml("};");
            script.InnerHtml.AppendHtml("gifProbe.onerror();");
            script.InnerHtml.AppendHtml("};");
            if (poll) { 
                script.InnerHtml.AppendHtml("function CheckAuto()");
                script.InnerHtml.AppendHtml("{");
                script.InnerHtml.AppendHtml("var xhttp = new XMLHttpRequest();");
                script.InnerHtml.AppendHtml("xhttp.onreadystatechange = function()");
                script.InnerHtml.AppendHtml("{");
                script.InnerHtml.AppendHtml("if (this.readyState == 4 && this.status == 200)");
                script.InnerHtml.AppendHtml("{");
                script.InnerHtml.AppendHtml("if(this.responseText !== \"false\")");
                script.InnerHtml.AppendHtml("{");
                script.InnerHtml.AppendHtml("window.location = \"" + request.HttpContext.Items["CheckUrl"] + "\";");
                script.InnerHtml.AppendHtml("}");
                script.InnerHtml.AppendHtml("}");
                script.InnerHtml.AppendHtml("};");
                script.InnerHtml.AppendHtml("xhttp.open(\"GET\", \""+ request.HttpContext.Items["CheckUrl"] + "\", true);");
                script.InnerHtml.AppendHtml("xhttp.send();");
                script.InnerHtml.AppendHtml("};");
                script.InnerHtml.AppendHtml("document.onload = setInterval(function(){ CheckAuto(); }, " + pollTime + ");");
            }

            var stringWriter = new System.IO.StringWriter();
            linkTag.WriteTo(stringWriter, HtmlEncoder.Default);
            script.WriteTo(stringWriter, HtmlEncoder.Default);
            return new HtmlString(stringWriter.ToString());
        }

        public static HtmlString SqrlQrImage<TModel>(this IHtmlHelper<TModel> helper, HttpRequest request)
        {
            return SqrlQrImage(helper, request, true);
        }

        public static HtmlString SqrlQrImage<TModel>(this IHtmlHelper<TModel> helper, HttpRequest request, bool poll)
        {
            return SqrlQrImage(helper, request, poll, int.Parse(request.HttpContext.Items["CheckMillieSeconds"].ToString()));
        }

        public static HtmlString SqrlQrImage<TModel>(this IHtmlHelper<TModel> helper, HttpRequest request, bool poll, int pollTime)
        {
            var stringWriter = new System.IO.StringWriter();
            var imgTag = new TagBuilder("img");
            imgTag.MergeAttribute("src", "data:image/bmp;base64," + request.HttpContext.Items["QrData"].ToString());
            imgTag.WriteTo(stringWriter, HtmlEncoder.Default);
            if (poll)
            {
                var script = new TagBuilder("script");
                script.InnerHtml.AppendHtml("function CheckAuto()");
                script.InnerHtml.AppendHtml("{");
                script.InnerHtml.AppendHtml("var xhttp = new XMLHttpRequest();");
                script.InnerHtml.AppendHtml("xhttp.onreadystatechange = function()");
                script.InnerHtml.AppendHtml("{");
                script.InnerHtml.AppendHtml("if (this.readyState == 4 && this.status == 200)");
                script.InnerHtml.AppendHtml("{");
                script.InnerHtml.AppendHtml("if(this.responseText !== \"false\")");
                script.InnerHtml.AppendHtml("{");
                script.InnerHtml.AppendHtml("window.location = \"" + request.HttpContext.Items["CheckUrl"] + "\";");
                script.InnerHtml.AppendHtml("}");
                script.InnerHtml.AppendHtml("}");
                script.InnerHtml.AppendHtml("};");
                script.InnerHtml.AppendHtml("xhttp.open(\"GET\", \"" + request.HttpContext.Items["CheckUrl"] + "\", true);");
                script.InnerHtml.AppendHtml("xhttp.send();");
                script.InnerHtml.AppendHtml("};");
                script.InnerHtml.AppendHtml("document.onload = setInterval(function(){ CheckAuto(); }, " + pollTime + ");");
                script.WriteTo(stringWriter, HtmlEncoder.Default);
            }
            return new HtmlString(stringWriter.ToString());
        }

    }
}
