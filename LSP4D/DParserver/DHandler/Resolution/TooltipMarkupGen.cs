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
            return new TooltipMarkupGen ().GenTooltipSignature (t, templateParamCompletion, currentMethodParam);
        }
        
        public static string CreateSignatureMarkdown(DNode dn, bool templateParamCompletion = false, int currentMethodParam = -1)
        {
            return new TooltipMarkupGen ().GenTooltipSignature (dn, templateParamCompletion, currentMethodParam);
        }
    }
}