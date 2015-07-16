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

namespace OokLanguage
{
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("jet")]
    [Name("jetCompletion")]
    class JetCompletionSourceProvider : ICompletionSourceProvider
    {

        [Import]
        internal IGlyphService GlyphService { get; set; }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new JetCompletionSource(textBuffer, GlyphService);
        }
    }

    class JetCompletionSource : ICompletionSource
    {
        private ITextBuffer _buffer;
        private bool _disposed = false;

        public JetCompletionSource(ITextBuffer buffer, IGlyphService gly)
        {
            _buffer = buffer;
            glyphService = gly;
        }


        private readonly IGlyphService glyphService;
        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            if (_disposed)
                throw new ObjectDisposedException("OokCompletionSource");
            
            List<Completion> completions = new List<Completion>()
            {
                new Completion("Ook!"),
                new Completion("Ook."),
                new Completion("Ook?", "Ook?", "", glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupEnum, StandardGlyphItem.GlyphItemPublic), "Ook?")
            };
            
            ITextSnapshot snapshot = _buffer.CurrentSnapshot;
            var triggerPoint = (SnapshotPoint)session.GetTriggerPoint(snapshot);

            if (triggerPoint == null)
                return;

            var line = triggerPoint.GetContainingLine();
            SnapshotPoint start = triggerPoint;

            while (start > line.Start && !char.IsWhiteSpace((start - 1).GetChar()))
            {
                start -= 1;
            }

            var applicableTo = snapshot.CreateTrackingSpan(new SnapshotSpan(start, triggerPoint), SpanTrackingMode.EdgeInclusive);
            var text = applicableTo.GetText(snapshot);
            //refine here
            completionSets.Add(new CompletionSet("All", "All", applicableTo, completions, Enumerable.Empty<Completion>()));
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}

