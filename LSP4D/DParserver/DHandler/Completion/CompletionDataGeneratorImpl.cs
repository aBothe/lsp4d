using System;
using D_Parser.Completion;
using D_Parser.Dom;
using D_Parser.Parser;
using D_Parserver.DHandler.Resolution;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace D_Parserver.DHandler.Completion
{
    public class CompletionDataGeneratorImpl : ICompletionDataGenerator
    {
        private readonly Action<CompletionItem> _enlistItemHandler;
        private string _suggestedName;

        public ISyntaxRegion TriggerSyntaxRegion { get; set; }

        public CompletionDataGeneratorImpl(Action<CompletionItem> enlistItemHandler)
        {
            _enlistItemHandler = enlistItemHandler;
        }

        private void AddItem(CompletionItem completionItem)
        {
            var insertText = completionItem.InsertText ?? completionItem.Label;
            if (!string.IsNullOrEmpty(_suggestedName) && insertText.StartsWith(_suggestedName))
            {
                completionItem.Preselect = true;
            }
            
            _enlistItemHandler(completionItem);
        }

        public void Add(byte token)
        {
            var tokenString = DTokens.GetTokenString(token);
            AddItem(new CompletionItem
            {
                Kind = CompletionItemKind.Keyword,
                Label = tokenString,
                TextEdit = CalculateTextEdit(tokenString)
            });
        }

        public void AddPropertyAttribute(string attributeText)
        {
            AddItem(new CompletionItem
            {
                Kind = CompletionItemKind.Keyword,
                Label = "@" + attributeText,
                InsertText = attributeText,
                TextEdit = CalculateTextEdit(attributeText)
            });
        }

        public void AddIconItem(string iconName, string text, string description)
        {
            AddItem(new CompletionItem
            {
                Kind = CompletionItemKind.Keyword,
                Label = text,
                Detail = description,
                TextEdit = CalculateTextEdit(text)
            });
        }

        public void AddTextItem(string text, string description)
        {
            AddItem(new CompletionItem
            {
                Kind = CompletionItemKind.Constant,
                Label = text,
                Detail = description,
                TextEdit = CalculateTextEdit(text)
            });
        }

        public void Add(INode n)
        {
            AddItem(new CompletionItem
            {
                Label = n.Name,
                Deprecated = n is DNode dNode && dNode.ContainsAnyAttribute(DTokens.Deprecated),
                TextEdit = CalculateTextEdit(n.Name),
                Kind = n.Accept(new CompletionItemKindVisitor())
            });
        }

        public void AddModule(DModule module, string nameOverride = null)
        {
            AddItem(new CompletionItem
            {
                Label = nameOverride ?? module.ModuleName,
                Kind = CompletionItemKind.Module,
                TextEdit = CalculateTextEdit(nameOverride ?? module.ModuleName)
            });
        }

        public void AddPackage(string packageName)
        {
            AddItem(new CompletionItem
            {
                Label = packageName,
                Kind = CompletionItemKind.Folder,
                TextEdit = CalculateTextEdit(packageName)
            });
        }

        public void AddCodeGeneratingNodeItem(INode node, string codeToGenerate)
        {
            AddItem(new CompletionItem
            {
                Label = node.Name,
                Kind = CompletionItemKind.Method,
                InsertText = codeToGenerate,
                TextEdit = CalculateTextEdit(codeToGenerate)
            });
        }

        public void SetSuggestedItem(string item)
        {
            _suggestedName = item;
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