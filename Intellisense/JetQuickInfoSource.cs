using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using System.Runtime.InteropServices;
using System.IO;

namespace OokLanguage
{
    [Export(typeof(IQuickInfoSourceProvider))]
    [ContentType("jet")]
    [Name("jetQuickInfo")]
    class JetQuickInfoSourceProvider : IQuickInfoSourceProvider
    {

        [Import]
        IBufferTagAggregatorFactoryService aggService = null;

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new JetQuickInfoSource(textBuffer, aggService.CreateTagAggregator<JetTokenTag>(textBuffer));
        }
    }

    class JetQuickInfoSource : IQuickInfoSource
    {
        private ITagAggregator<JetTokenTag> _aggregator;
        private ITextBuffer _buffer;
        private bool _disposed = false;


        public JetQuickInfoSource(ITextBuffer buffer, ITagAggregator<JetTokenTag> aggregator)
        {
            _aggregator = aggregator;
            _buffer = buffer;
        }

        //set this up
        [DllImport("Racer.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetSymbolInfo(string project, string file, string symbol, int line);

        internal static string GetFilePath(ITextBuffer buffer)
        {
            ITextDocument doc;
            var rc = buffer.Properties.TryGetProperty<ITextDocument>(typeof(ITextDocument), out doc);
            return doc.FilePath;
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
        {
            applicableToSpan = null;

            if (_disposed)
                throw new ObjectDisposedException("TestQuickInfoSource");

            var triggerPoint = (SnapshotPoint)session.GetTriggerPoint(_buffer.CurrentSnapshot);

            if (triggerPoint == null)
                return;

            foreach (IMappingTagSpan<JetTokenTag> curTag in _aggregator.GetTags(new SnapshotSpan(triggerPoint, triggerPoint)))
            {
                if (curTag.Tag.type == JetTokenTypes.JetType)
                {
                    var tagSpan = curTag.Span.GetSpans(_buffer).First();
                    applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(tagSpan, SpanTrackingMode.EdgeExclusive);
                    quickInfoContent.Add(tagSpan.GetText());
                }
                else if (curTag.Tag.type == JetTokenTypes.JetName)
                {
                    var tagSpan = curTag.Span.GetSpans(_buffer).First();
                    applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(tagSpan, SpanTrackingMode.EdgeExclusive);
                   

                    //look it up yo
                    //string path = GetFilePath(_buffer);
                    //need to somehow pass the filename
                    var path = GetFilePath(this._buffer);
                    var name = Path.GetFileName(path);
                    var symbol = tagSpan.GetText();
                    if (symbol != " ")
                    {
                        var res = GetSymbolInfo(path, name, symbol, tagSpan.Snapshot.GetLineFromPosition(tagSpan.Span.Start).LineNumber + 1);
                        string info = Marshal.PtrToStringAnsi(res);

                        quickInfoContent.Add(info);
                    }
                }
            }
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}

