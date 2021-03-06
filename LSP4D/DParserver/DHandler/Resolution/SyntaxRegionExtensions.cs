using D_Parser.Dom;
using D_Parser.Dom.Expressions;
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

        public static bool IsWithinRange(this ISyntaxRegion sr, Range range)
        {
            var startLine = sr.Location.Line - 1;
            var startCol = sr.Location.Column - 1;
            var endLine = sr.EndLocation.Line - 1;
            var endCol = sr.EndLocation.Column - 1;

            return (range.Start.Line < startLine ||
                    (range.Start.Line == startLine && range.Start.Character <= startCol))
                   && (range.End.Line > endLine || (range.End.Line == endLine && range.End.Character >= endCol));
        }

        public static Range GetNameRange(this ISyntaxRegion reference)
        {
            CodeLocation startLocation;
            CodeLocation endLocation;

            switch (reference)
            {
                case AbstractTypeDeclaration abstractTypeDeclaration:
                    startLocation = abstractTypeDeclaration.NonInnerTypeDependendLocation;
                    endLocation = abstractTypeDeclaration.EndLocation;
                    break;
                case IExpression _:
                    startLocation = reference.Location;
                    endLocation = reference.EndLocation;
                    break;
                case TemplateParameter templateParameter:
                    startLocation = templateParameter.NameLocation;
                    endLocation = new CodeLocation(templateParameter.NameLocation.Column + templateParameter.Name.Length, 
                        templateParameter.NameLocation.Line);
                    break;
                case INode n:
                    startLocation = n.NameLocation;
                    endLocation = new CodeLocation(n.NameLocation.Column + n.Name.Length, n.NameLocation.Line);
                    break;
                default:
                    return default;
            }

            return new Range(new Position(startLocation.Line - 1, startLocation.Column - 1),
                new Position(endLocation.Line - 1, endLocation.Column - 1));
        }
    }
}