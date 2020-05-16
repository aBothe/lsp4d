using System.Collections.Generic;
using System.Text;
using D_Parser.Completion.ToolTips;
using D_Parser.Dom;
using D_Parser.Resolver;

namespace D_Parserver.DHandler.Resolution
{
    public class TooltipMarkupGen : NodeTooltipRepresentationGen
    {
        // https://help.github.com/articles/creating-and-highlighting-code-blocks/#syntax-highlighting

        private TooltipMarkupGen()
        {
        }
        
        protected override string DCodeToMarkup(string code)
        {
            //TODO: Keep html markups by only putting innerTexts into code blocks
            if (!code.Contains("\n")) // SingleLine
            {
                return "`" + HtmlEncode(code) + "`";
            }
            return "```\n" + HtmlEncode(code) + "\n```";
        }

        private static string HtmlEncode(string code)
        {
            return code.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;"); //TODO
        }
        
        public static string CreateSignatureMarkdown(AbstractType t, bool templateParamCompletion = false, int currentMethodParam = -1)
        {
            var markupGen = new TooltipMarkupGen ();
            var sb = new StringBuilder();
            
            sb.AppendLine(markupGen.GenTooltipSignature (t, templateParamCompletion, currentMethodParam));

            if (t is DSymbol ds && ds.Definition is { } n)
                AppendTooltipBody(markupGen, n, sb);
            
            return sb.ToString();
        }
        
        public static string CreateSignatureMarkdown(DNode dn, bool templateParamCompletion = false, int currentMethodParam = -1)
        {
            return new TooltipMarkupGen ().GenTooltipSignature (dn, templateParamCompletion, currentMethodParam);
        }
        
        static void AppendTooltipBody(TooltipMarkupGen markupGen, DNode dn, StringBuilder sb)
        {
            markupGen.GenToolTipBody (dn, out string summary, out Dictionary<string, string> categories);

            if (summary != null)
            {
                sb.AppendLine();
                sb.AppendLine(summary);
            }

            if (categories != null)
            {
                sb.AppendLine();
                foreach (var kv in categories)
                    sb.Append("**").Append(kv.Key).Append(":** ").AppendLine(kv.Value);
            }
        }
    }
}