﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Pulsus.Internal
{
    using System;
    using System.Collections.Generic;
    
    #line 2 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
    using System.Linq;
    
    #line default
    #line hidden
    using System.Text;
    
    #line 3 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
    using Internal;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    internal partial class EmailTemplate : RazorTemplateBase<EmailTemplateModel>
    {
#line hidden

        public override void Execute()
        {


WriteLiteral("\r\n");




WriteLiteral("<html>\r\n<head>\r\n    <meta charset=\"utf-8\" />\r\n    <title>");


            
            #line 8 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
      Write(Model.Title);

            
            #line default
            #line hidden
WriteLiteral("</title>\r\n    <style type=\"text/css\"><!--\r\n        body { font-family:\"Segoe UI\"," +
"Verdana,arial,sans-serif;font-size:8pt;color:#444;padding:5px;background-color:#" +
"fff; }\r\n        pre { display:block;width:100%;font-family:Consolas,\"Courier New" +
"\";background:#f4f4f4;border:1px solid #999;padding:10px;word-break:break-all;wor" +
"d-wrap:break-word;display:block; }\r\n        h1,h2,h3 { font-family:\"Segoe UI\",Ve" +
"rdana,arial,sans-serif;color:#666; }\r\n        h1 { font-size:18pt; }\r\n        h2" +
" { font-size:10pt;margin:10px 4px 5px 4px; }\r\n        table { display:table;widt" +
"h:100%;border:1px solid #999;width:100%;table-layout:fixed; }\r\n        table th " +
"{ background:#333;color:#fff;padding:5px;min-width:100px;text-align:left; }\r\n   " +
"     table td { padding:5px;word-break:break-all; }\r\n        table td.first-col " +
"{ word-break:normal;word-wrap:normal; }\r\n        table tr.alt td { background-co" +
"lor:#f4f4f4; }\r\n        table.title { border:0; }\r\n        table.title td.level " +
"{ width:8px; }\r\n        table.title td.red { background-color:#ff2727; }\r\n      " +
"  table.title td.yellow { background-color:#ffcb00; }\r\n        table.title td.gr" +
"een { background-color:#008e1d; }\r\n        table.title td.blue { background-colo" +
"r:#1f2aef; }\r\n        table.title td { padding-left:12px; }\r\n        table.title" +
" h1 { color:#444; }\r\n        a.link, a.link:hover, a.link:visited { color:#444; " +
"}\r\n    --></style>\r\n</head>\r\n    <body>\r\n        <table cellpadding=\"0\" cellspac" +
"ing=\"0\" class=\"title\">\r\n            <tr>\r\n                <td width=\"8\" style=\"\"" +
" class=\"first-col level ");


            
            #line 34 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                                                         Write(Model.LevelClass);

            
            #line default
            #line hidden
WriteLiteral("\">&nbsp;</td>\r\n                <td><h1>");


            
            #line 35 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                   Write(Model.GetFormattedTitle());

            
            #line default
            #line hidden
WriteLiteral("</h1></td>\r\n            </tr>\r\n        </table>\r\n    \r\n        <h2>General</h2>\r\n" +
"        <table cellpadding=\"0\" cellspacing=\"0\">\r\n            <tr>\r\n             " +
"   <td class=\"first-col\">Date</td>\r\n                <td>");


            
            #line 43 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
               Write(Model.LoggingEvent.Date);

            
            #line default
            #line hidden
WriteLiteral(" UTC</td>\r\n            </tr>\r\n            <tr class=\"alt\">\r\n                <td c" +
"lass=\"first-col\">LogKey</td>\r\n                <td>");


            
            #line 47 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
               Write(Model.LoggingEvent.LogKey);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n            </tr>\r\n");


            
            #line 49 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
             if (!Model.LoggingEvent.ApiKey.IsNullOrEmpty()) {

            
            #line default
            #line hidden
WriteLiteral("                <tr>\r\n                    <td class=\"first-col\">ApiKey</td>\r\n    " +
"                <td>");


            
            #line 52 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                   Write(Model.LoggingEvent.ApiKey);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                </tr>\r\n");


            
            #line 54 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteral("            <tr class=\"alt\">\r\n                <td class=\"first-col\">ID</td>\r\n    " +
"            <td>");


            
            #line 57 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
               Write(Model.LoggingEvent.EventId);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n            </tr>\r\n            <tr>\r\n                <td class=\"first-col\"" +
">Level</td>\r\n                <td>");


            
            #line 61 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
               Write(Model.LevelText);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n            </tr>\r\n            <tr class=\"alt\">\r\n                <td class" +
"=\"first-col\">User</td>\r\n                <td>");


            
            #line 65 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                Write(Model.LoggingEvent.User ?? "(none)");

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n            </tr>\r\n            <tr>\r\n                <td class=\"first-col\"" +
">Tags</td>\r\n                <td>");


            
            #line 69 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
               Write(string.Join(" ", Model.LoggingEvent.Tags.ToArray()));

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n            </tr>\r\n            <tr class=\"alt\">\r\n                <td class" +
"=\"first-col\">MachineName</td>\r\n                <td>");


            
            #line 73 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
               Write(Model.LoggingEvent.MachineName);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n            </tr>\r\n");


            
            #line 75 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
             if (!Model.LoggingEvent.Psid.IsNullOrEmpty()) {

            
            #line default
            #line hidden
WriteLiteral("                <tr>\r\n                    <td class=\"first-col\"><abbr title=\"Sess" +
"ion ID\">PSID</abbr></td>\r\n                    <td>");


            
            #line 78 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                   Write(Model.LoggingEvent.Psid);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                </tr>\r\n");


            
            #line 80 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
            }

            
            #line default
            #line hidden

            
            #line 81 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
             if (!Model.LoggingEvent.Ppid.IsNullOrEmpty()) {

            
            #line default
            #line hidden
WriteLiteral("                <tr class=\"alt\">\r\n                    <td class=\"first-col\"><abbr" +
" title=\"Persistent Session ID\">PPID</abbr></td>\r\n                    <td>");


            
            #line 84 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                   Write(Model.LoggingEvent.Ppid);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                </tr>\r\n");


            
            #line 86 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteral("        </table>\r\n        \r\n");


            
            #line 89 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
         if (Model.CustomData != null) {

            
            #line default
            #line hidden
WriteLiteral("            <h2>Custom Data</h2>\r\n");


            
            #line 91 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
            
            
            #line default
            #line hidden
            
            #line 91 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
       Write(WriteTable(Model.CustomData));

            
            #line default
            #line hidden
            
            #line 91 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                                         
        }

            
            #line default
            #line hidden
WriteLiteral("    \r\n");


            
            #line 94 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
         if (Model.HttpContextInformation != null) {

            
            #line default
            #line hidden
WriteLiteral("            <h2>Request</h2>\r\n");



WriteLiteral("            <table cellpadding=\"0\" cellspacing=\"0\">\r\n                <tr>\r\n      " +
"              <td class=\"first-col\">URL</td>\r\n                    <td><a href=\"");


            
            #line 99 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                            Write(Model.HttpContextInformation.Url);

            
            #line default
            #line hidden
WriteLiteral("\">");


            
            #line 99 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                                                               Write(Model.HttpContextInformation.Url);

            
            #line default
            #line hidden
WriteLiteral("</a></td>\r\n                </tr>\r\n                <tr class=\"alt\">\r\n             " +
"       <td class=\"first-col\">Method</td>\r\n                    <td>");


            
            #line 103 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                   Write(Model.HttpContextInformation.Method);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td class" +
"=\"first-col\">Host</td>\r\n                    <td>");


            
            #line 107 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                   Write(Model.HttpContextInformation.Host);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                </tr>\r\n                <tr class=\"alt\">\r\n                 " +
"   <td class=\"first-col\">Referer</td>\r\n                    <td>");


            
            #line 111 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                   Write(Model.HttpContextInformation.Referer);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td class" +
"=\"first-col\">IP Address</td>\r\n                    <td>");


            
            #line 115 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                   Write(Model.GetFormattedIpAddress(Model.HttpContextInformation.IpAddress));

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                </tr>\r\n                <tr class=\"alt\">\r\n                 " +
"   <td class=\"first-col\">User Agent</td>\r\n                    <td>");


            
            #line 119 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                   Write(Model.GetFormattedUserAgent(Model.HttpContextInformation.UserAgent));

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                </tr>\r\n            </table>\r\n");


            
            #line 122 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
      
            if (Model.HttpContextInformation.RouteValues != null) {

            
            #line default
            #line hidden
WriteLiteral("                <h2>Route Values</h2>\r\n");


            
            #line 125 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                
            
            #line default
            #line hidden
            
            #line 125 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
           Write(WriteTable(Model.HttpContextInformation.RouteValues));

            
            #line default
            #line hidden
            
            #line 125 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                                                                     
            }
     
            if (Model.HttpContextInformation.QueryString != null) {

            
            #line default
            #line hidden
WriteLiteral("                <h2>QueryString</h2>\r\n");


            
            #line 130 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                
            
            #line default
            #line hidden
            
            #line 130 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
           Write(WriteTable(Model.HttpContextInformation.QueryString));

            
            #line default
            #line hidden
            
            #line 130 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                                                                     
            }
        
            if (Model.HttpContextInformation.PostedValues != null) {

            
            #line default
            #line hidden
WriteLiteral("                <h2>Posted Values</h2>\r\n");


            
            #line 135 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                
            
            #line default
            #line hidden
            
            #line 135 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
           Write(WriteTable(Model.HttpContextInformation.PostedValues));

            
            #line default
            #line hidden
            
            #line 135 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                                                                      
            }
        
            if (Model.HttpContextInformation.PostedFiles != null) {

            
            #line default
            #line hidden
WriteLiteral("                <h2>Posted Files</h2>\r\n");



WriteLiteral(@"                <table cellpadding=""0"" cellspacing=""0"">
                    <tr>
                        <th class=""first-col"" align=""left"">Name</th>
                        <th align=""left"">Content Type</th>
                        <th align=""left"">Size</th>
                    </tr>
");


            
            #line 146 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                       var i = 0;  

            
            #line default
            #line hidden

            
            #line 147 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                     foreach (var postedFile in Model.HttpContextInformation.PostedFiles) {
                        var classAtt = i % 2 > 0 ? "class=\"alt\"" : string.Empty;

            
            #line default
            #line hidden
WriteLiteral("                        <tr ");


            
            #line 149 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                       Write(classAtt);

            
            #line default
            #line hidden
WriteLiteral(">\r\n                            <td class=\"first-col\">");


            
            #line 150 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                                             Write(postedFile.Name);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                            <td>");


            
            #line 151 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                           Write(postedFile.ContentType);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                            <td>");


            
            #line 152 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                           Write(FileSizeFormatProvider.FileSize(postedFile.ContentLength));

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                        </tr>\r\n");


            
            #line 154 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                        i++;
                    }

            
            #line default
            #line hidden
WriteLiteral("                </table>\r\n");


            
            #line 157 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
            }
        
            if (Model.HttpContextInformation.Cookies != null) {

            
            #line default
            #line hidden
WriteLiteral("                <h2>Cookies</h2>\r\n");


            
            #line 161 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                
            
            #line default
            #line hidden
            
            #line 161 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
           Write(WriteTable(Model.HttpContextInformation.Cookies));

            
            #line default
            #line hidden
            
            #line 161 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                                                                 
            }
        
            if (Model.HttpContextInformation.Headers != null) {

            
            #line default
            #line hidden
WriteLiteral("                <h2>Headers</h2>\r\n");


            
            #line 166 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                
            
            #line default
            #line hidden
            
            #line 166 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
           Write(WriteTable(Model.HttpContextInformation.Headers));

            
            #line default
            #line hidden
            
            #line 166 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                                                                 
            }
        
            if (Model.HttpContextInformation.ServerVariables != null) {

            
            #line default
            #line hidden
WriteLiteral("                <h2>Server Variables</h2>\r\n");


            
            #line 171 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                
            
            #line default
            #line hidden
            
            #line 171 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
           Write(WriteTable(Model.HttpContextInformation.ServerVariables));

            
            #line default
            #line hidden
            
            #line 171 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                                                                         
            }
        
            if (Model.HttpContextInformation.Session != null) {

            
            #line default
            #line hidden
WriteLiteral("                <h2>Session Variables</h2>\r\n");


            
            #line 176 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                
            
            #line default
            #line hidden
            
            #line 176 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
           Write(WriteTable(Model.HttpContextInformation.Session));

            
            #line default
            #line hidden
            
            #line 176 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                                                                 
            }
        }

            
            #line default
            #line hidden
WriteLiteral("\r\n");


            
            #line 180 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
         if (Model.ExceptionInformation != null) {

            
            #line default
            #line hidden
WriteLiteral("            <h2>Exception</h2>\r\n");



WriteLiteral("            <table cellpadding=\"0\" cellspacing=\"0\">\r\n                <tr>\r\n      " +
"              <td class=\"first-col\">Message</td>\r\n                    <td>");


            
            #line 185 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                   Write(Model.ExceptionInformation.Message);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                </tr>\r\n                <tr class=\"alt\">\r\n                 " +
"   <td class=\"first-col\">Type</td>\r\n                    <td>");


            
            #line 189 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                   Write(Model.ExceptionInformation.Type);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td class" +
"=\"first-col\">Source</td>\r\n                    <td>");


            
            #line 193 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                   Write(Model.ExceptionInformation.Source);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                </tr>\r\n                <tr class=\"alt\">\r\n                 " +
"   <td class=\"first-col\">StatusCode</td>\r\n                    <td>");


            
            #line 197 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                   Write(Model.ExceptionInformation.StatusCode);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                </tr>\r\n            </table>\r\n");


            
            #line 200 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    \r\n");


            
            #line 202 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
         if (Model.StackTrace != null) {

            
            #line default
            #line hidden
WriteLiteral("            <h2>StackTrace</h2>\r\n");



WriteLiteral("            <pre class=\"stacktrace\">");


            
            #line 204 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                               Write(Model.StackTrace);

            
            #line default
            #line hidden
WriteLiteral("</pre>\r\n");


            
            #line 205 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    \r\n");


            
            #line 207 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
         if (Model.SqlInformation != null) {

            
            #line default
            #line hidden
WriteLiteral("            <h2>SQL</h2>\r\n");


            
            #line 209 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
            
            
            #line default
            #line hidden
            
            #line 209 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
       Write(WriteTable(Model.SqlInformation.Parameters));

            
            #line default
            #line hidden
            
            #line 209 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                                                        

            
            #line default
            #line hidden
WriteLiteral("            <pre class=\"sql\">");


            
            #line 210 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
                        Write(Model.SqlInformation.SQL);

            
            #line default
            #line hidden
WriteLiteral("</pre>\r\n");


            
            #line 211 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("        <br />\r\n        ");


            
            #line 213 "E:\Projects\codeplex\pulsus\src\Pulsus\Internal\EmailTemplate.cshtml"
   Write(Model.Footer);

            
            #line default
            #line hidden
WriteLiteral("\r\n    </body>\r\n</html>");


        }
    }
}
#pragma warning restore 1591
