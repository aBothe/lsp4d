using System;
using D_Parser.Dom;
using D_Parser.Parser;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace D_Parserver.DHandler.Completion
{
    public class CompletionItemKindVisitor : NodeVisitor<CompletionItemKind>
    {
        public CompletionItemKind Visit(DEnumValue n)
        {
            return CompletionItemKind.EnumMember;
        }

        public CompletionItemKind VisitDVariable(DVariable n)
        {
            return CompletionItemKind.Variable;
        }

        public CompletionItemKind Visit(DMethod n)
        {
            return CompletionItemKind.Method;
        }

        public CompletionItemKind Visit(DClassLike n)
        {
            switch (n.ClassType)
            {
                case DTokens.Interface:
                    return CompletionItemKind.Interface;
                case DTokens.Struct:
                case DTokens.Union:
                    return CompletionItemKind.Struct;
                default:
                    return CompletionItemKind.Class;
            }
        }

        public CompletionItemKind Visit(DEnum n)
        {
            return CompletionItemKind.Enum;
        }

        public CompletionItemKind Visit(DModule n)
        {
            return CompletionItemKind.Module;
        }

        public CompletionItemKind Visit(DBlockNode n)
        {
            throw new NotImplementedException();
        }

        public CompletionItemKind Visit(TemplateParameter.Node n)
        {
            return CompletionItemKind.TypeParameter;
        }

        public CompletionItemKind Visit(NamedTemplateMixinNode n)
        {
            return CompletionItemKind.Variable;
        }

        public CompletionItemKind VisitAttribute(Modifier a)
        {
            throw new NotImplementedException();
        }

        public CompletionItemKind VisitAttribute(DeprecatedAttribute a)
        {
            throw new NotImplementedException();
        }

        public CompletionItemKind VisitAttribute(PragmaAttribute attr)
        {
            throw new NotImplementedException();
        }

        public CompletionItemKind VisitAttribute(BuiltInAtAttribute a)
        {
            throw new NotImplementedException();
        }

        public CompletionItemKind VisitAttribute(UserDeclarationAttribute a)
        {
            throw new NotImplementedException();
        }

        public CompletionItemKind VisitAttribute(VersionCondition a)
        {
            throw new NotImplementedException();
        }

        public CompletionItemKind VisitAttribute(DebugCondition a)
        {
            throw new NotImplementedException();
        }

        public CompletionItemKind VisitAttribute(StaticIfCondition a)
        {
            throw new NotImplementedException();
        }

        public CompletionItemKind VisitAttribute(NegatedDeclarationCondition a)
        {
            throw new NotImplementedException();
        }

        public CompletionItemKind Visit(EponymousTemplate n)
        {
            return CompletionItemKind.TypeParameter;
        }

        public CompletionItemKind Visit(ModuleAliasNode n)
        {
            return CompletionItemKind.Module;
        }

        public CompletionItemKind Visit(ImportSymbolNode n)
        {
            return CompletionItemKind.Reference;
        }

        public CompletionItemKind Visit(ImportSymbolAlias n)
        {
            return CompletionItemKind.Reference;
        }
    }
}