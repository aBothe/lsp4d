using System;
using D_Parser.Completion;
using D_Parser.Dom;
using D_Parser.Parser;
using D_Parserver.DHandler.Completion;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace D_Parserver.DHandler.Resolution
{
    public class CompletionDataGeneratorImpl : ICompletionDataGenerator
    {
        private readonly Action<CompletionItem> enlistItemHandler;

        public ISyntaxRegion TriggerSyntaxRegion { get; set; }

        public CompletionDataGeneratorImpl(Action<CompletionItem> enlistItemHandler)
        {
            this.enlistItemHandler = enlistItemHandler;
        }

        public void Add(byte Token)
        {
            var tokenString = DTokens.GetTokenString(Token);
            enlistItemHandler(new CompletionItem
            {
                Kind = CompletionItemKind.Keyword,
                Label = tokenString,
                TextEdit = CalculateTextEdit(tokenString)
            });
        }

        public void AddPropertyAttribute(string AttributeText)
        {
            enlistItemHandler(new CompletionItem
            {
                Kind = CompletionItemKind.Keyword,
                Label = "@" + AttributeText,
                InsertText = AttributeText,
                TextEdit = CalculateTextEdit(AttributeText)
            });
        }

        public void AddIconItem(string iconName, string text, string description)
        {
            enlistItemHandler(new CompletionItem
            {
                Kind = CompletionItemKind.Keyword,
                Label = text,
                Detail = description,
                TextEdit = CalculateTextEdit(text)
            });
        }

        public void AddTextItem(string Text, string Description)
        {
            enlistItemHandler(new CompletionItem
            {
                Kind = CompletionItemKind.Constant,
                Label = Text,
                Detail = Description,
                TextEdit = CalculateTextEdit(Text)
            });
        }

        public void Add(INode n)
        {
            enlistItemHandler(new CompletionItem
            {
                Label = n.Name,
                Deprecated = n is DNode dNode && dNode.ContainsAnyAttribute(DTokens.Deprecated),
                TextEdit = CalculateTextEdit(n.Name),
                Kind = n.Accept(new CompletionItemKindVisitor())
            });
        }

        public void AddModule(DModule module, string nameOverride = null)
        {
            enlistItemHandler(new CompletionItem
            {
                Label = nameOverride ?? module.ModuleName,
                Kind = CompletionItemKind.Module,
                TextEdit = CalculateTextEdit(nameOverride ?? module.ModuleName)
            });
        }

        public void AddPackage(string packageName)
        {
            enlistItemHandler(new CompletionItem
            {
                Label = packageName,
                Kind = CompletionItemKind.Folder,
                TextEdit = CalculateTextEdit(packageName)
            });
        }

        public void AddCodeGeneratingNodeItem(INode node, string codeToGenerate)
        {
            enlistItemHandler(new CompletionItem
            {
                Label = "(implement) " + node.Name,
                Kind = CompletionItemKind.Method,
                InsertText = codeToGenerate,
                TextEdit = CalculateTextEdit(codeToGenerate)
            });
        }

        public void SetSuggestedItem(string item)
        {
            throw new NotImplementedException();
        }

        public void NotifyTimeout()
        {
        }

        private TextEdit CalculateTextEdit(string insertText)
        {
            return TriggerSyntaxRegion == null ? null : new TextEdit
            {
                NewText = insertText,
                Range = TriggerSyntaxRegion.ToRange()
            };
        }
    }
}