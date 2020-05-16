using D_Parser.Dom;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace D_Parserver.DHandler.Resolution
{
    public static class SyntaxRegionExtensions
    {
        public static Range ToRange(this ISyntaxRegion syntaxRegion)
        {
            return syntaxRegion == null ? null
                : new Range(
                    new Position(syntaxRegion.Location.Line - 1, syntaxRegion.Location.Column - 1),
                    new Position(syntaxRegion.EndLocation.Line - 1, syntaxRegion.EndLocation.Column - 1));
        }
        
        public static Range ToNameLocationRange(this INode node)
        {
            return node == null ? null : new Range(
                new Position(node.NameLocation.Line - 1, node.NameLocation.Column - 1),
                new Position(node.NameLocation.Line - 1, node.NameLocation.Column + node.Name.Length - 1));
        }
        
        public static Range ToRange(this INode node)
        {
            return node == null ? null : new Range(
                new Position(node.Location.Line - 1, node.Location.Column - 1), 
                new Position(node.EndLocation.Line - 1, node.EndLocation.Column - 1));
        }
    }
}