using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using D_Parser;
using D_Parser.Dom;
using D_Parser.Misc;
using D_Parser.Parser;
using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Server;

namespace D_Parserver.DHandler
{
    public class TextDocumentHandler : ITextDocumentSyncHandler
    {
        private readonly ILogger<TextDocumentHandler> _logger;
        private readonly ILanguageServerConfiguration _configuration;

        public static readonly ConcurrentDictionary<Uri, DModuleDocument> OpenFiles = new ConcurrentDictionary<Uri, DModuleDocument>();

        public class DModuleDocument
        {
            public readonly string Code;
            public readonly long Version;
            public readonly DModule Module;

            public DModuleDocument(string code, long version, DModule module)
            {
                Code = code;
                Version = version;
                Module = module;
            }
        }
        
        public TextDocumentSyncKind Change { get; } = TextDocumentSyncKind.Incremental;
        
        public static readonly DocumentSelector DocumentSelector = new DocumentSelector(
            new DocumentFilter {
                Pattern = "**/*.{d,di}",
                Language = "d"
            }
        );
        
        public TextDocumentHandler(ILogger<TextDocumentHandler> logger,
            ILanguageServerConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        TextDocumentChangeRegistrationOptions IRegistration<TextDocumentChangeRegistrationOptions>.
            GetRegistrationOptions()
        {
            return new TextDocumentChangeRegistrationOptions {
                DocumentSelector = DocumentSelector,
                SyncKind = Change
            };
        }

        TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentSaveRegistrationOptions {
                DocumentSelector = DocumentSelector,
                IncludeText = true
            };
        }

        TextDocumentRegistrationOptions IRegistration<TextDocumentRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentRegistrationOptions {
                DocumentSelector = DocumentSelector
            };
        }

        public async Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
        {
            await Task.Yield();
            var code = OpenFiles[request.TextDocument.Uri].Code;
            
            foreach (var change in request.ContentChanges)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                
                var range = change.Range;
                if (range != null)
                {
                    var caret = new CodeLocation((int)range.Start.Character + 1, (int)range.Start.Line + 1);
                    var end = new CodeLocation((int)range.End.Character + 1, (int)range.End.Line + 1);
                    var startOffset = DocumentHelper.LocationToOffset(code, caret);
                    var endOffset = DocumentHelper.GetOffsetByRelativeLocation(code, caret, startOffset, end);

                    code = code.Substring(0, startOffset) + change.Text + code.Substring(endOffset);
                }
                else
                {
                    code = change.Text;
                }
            }

            OpenFiles[request.TextDocument.Uri] = new DModuleDocument(code, request.TextDocument.Version, ParseFile(request.TextDocument.Uri, code));
            return Unit.Value;
        }

        static DModule ParseFile(Uri uri, string code)
        {
            var module = DParser.ParseString(code);
            module.FileName = uri.AbsolutePath;
            GlobalParseCache.AddOrUpdateModule(module);
            return module;
        }

        public void SetCapability(SynchronizationCapability capability)
        {
            capability.DidSave = Supports.OfBoolean<bool>(true);
        }

        public async Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            await Task.Yield();
            OpenFiles[request.TextDocument.Uri] = new DModuleDocument(request.TextDocument.Text, 
                request.TextDocument.Version, ParseFile(request.TextDocument.Uri, request.TextDocument.Text));
            return Unit.Value;
        }

        public Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
        {
            OpenFiles.Remove(request.TextDocument.Uri, out _);
            
            return Unit.Task;
        }

        public Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }

        public TextDocumentAttributes GetTextDocumentAttributes(Uri uri)
        {
            return new TextDocumentAttributes(uri, "d");
        }
    }
}