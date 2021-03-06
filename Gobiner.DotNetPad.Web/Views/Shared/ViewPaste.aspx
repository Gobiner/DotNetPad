<%@ Page Title="" Language="C#" MasterPageFile="~/Master.Master" Inherits="System.Web.Mvc.ViewPage" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	.NET Pad
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<% var paste = ((Gobiner.CSharpPad.Web.Models.Paste)ViewData.Model); %>
    <div class="code">
        <table class="outer">
            <tr><td style="width: 30px;">
                <% int linesOfCode = paste.Code.Split(new string[] { "\n" }, StringSplitOptions.None).Length; %>
                    <table class="linenumbers">
                        <%
							for (int i = 1; i <= linesOfCode; i++)
                            {
                        %>
                        <tr><td><%= i %></td></tr>
                        <%    
                            }
                        %>
                    </table>
            </td>
            <td>
<pre class="prettyprint">
<%= Server.HtmlEncode(paste.Code.Replace("\t", "    ").Replace("\r","")) %>
</pre>
           </td></tr>
       </table>
    </div>



<ul class="middle-action-bar">
<% if (paste.ILDisassemblyText != null && paste.ILDisassemblyText.Length > 0)
   { %>
    <li><a id="dissasembly-toggle" href="#">Toggle Disassembly</a></li>
<% } %>
    <li><a href="/EditPaste/<%= paste.Slug %>">Fork this code</a></li>
</ul>




<% if (paste.ILDisassemblyText != null && paste.ILDisassemblyText.Length > 0)
   { %>
<div class="disassembly">

<table class="outer">
    <tr><td style="width: 30px;">
        <table class="linenumbers">
            <% for (int i = 1; i <= paste.ILDisassemblyText.Count(); i++)
               { %>
            <tr><td><%= i%></td></tr>
            <% } %>
        </table>
    </td><td>
        <pre><%= Server.HtmlEncode(string.Join("\r", paste.ILDisassemblyText.Select(ILD=>ILD.Text.Replace("\t","    ")))) %></pre>
    </td></tr>
</table>

</div>
<% } %>


<% if (paste.Output.Length > 0)
   { %>
<div class="output">
        <table>
            <tr><td>
                <% int linesOfOutput = paste.Output.Split(new string[] { "\n" }, StringSplitOptions.None).Length; %>
                    <table class="linenumbers">
                        <%
							for (int i = 1; i <= linesOfOutput; i++)
                            {
                        %>
                        <tr><td><%= i%></td></tr>
                        <%    
                            }
                        %>
                    </table>
            </td>
            <td>
<pre>
<%=  Server.HtmlEncode(paste.Output) %>
</pre>
           </td></tr>
       </table></div>
<% } %>

<% if (paste.Errors.Count() > 0)
   { %>
<div class="errors">
<pre class="errors"><% foreach (var line in paste.Errors)
   { %>
Line <%= line.Line %> : <%= Server.HtmlEncode(line.ErrorText) %><% } %></pre></div>
<% } %>

</asp:Content>
