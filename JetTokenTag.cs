// Copyright (c) Microsoft Corporation
// All rights reserved

namespace OokLanguage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITaggerProvider))]
    [ContentType("jet")]
    [TagType(typeof(JetTokenTag))]
    internal sealed class JetTokenTagProvider : ITaggerProvider
    {

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return new JetTokenTagger(buffer) as ITagger<T>;
        }
    }

    public class JetTokenTag : ITag 
    {
        public JetTokenTypes type { get; private set; }

        public JetTokenTag(JetTokenTypes type)
        {
            this.type = type;
        }
    }

    internal sealed class JetTokenTagger : ITagger<JetTokenTag>
    {

        ITextBuffer _buffer;
        IDictionary<string, JetTokenTypes> _ookTypes;

        internal JetTokenTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
            _ookTypes = new Dictionary<string, JetTokenTypes>();
            //_ookTypes["ook!"] = OokTokenTypes.OokExclaimation;
            //_ookTypes["ook."] = OokTokenTypes.OokPeriod;
            //_ookTypes["ook?"] = OokTokenTypes.OokQuestion;

            _ookTypes[" "] = JetTokenTypes.JetWhitespace;
            _ookTypes["\t"] = JetTokenTypes.JetWhitespace;

            _ookTypes["fun"] = JetTokenTypes.JetKeyword;
            _ookTypes["local"] = JetTokenTypes.JetKeyword;
            _ookTypes["trait"] = JetTokenTypes.JetKeyword;
            _ookTypes["struct"] = JetTokenTypes.JetKeyword;

            _ookTypes["switch"] = JetTokenTypes.JetKeyword;
            _ookTypes["for"] = JetTokenTypes.JetKeyword;
            _ookTypes["if"] = JetTokenTypes.JetKeyword;
            _ookTypes["else"] = JetTokenTypes.JetKeyword;
            _ookTypes["while"] = JetTokenTypes.JetKeyword;
            _ookTypes["return"] = JetTokenTypes.JetKeyword;

            _ookTypes["sizeof"] = JetTokenTypes.JetKeyword;
            _ookTypes["extern"] = JetTokenTypes.JetKeyword;
            _ookTypes["this"] = JetTokenTypes.JetKeyword;

            _ookTypes["int"] = JetTokenTypes.JetType;
            _ookTypes["double"] = JetTokenTypes.JetType;
            _ookTypes["void"] = JetTokenTypes.JetType;
            _ookTypes["bool"] = JetTokenTypes.JetType;
            _ookTypes["char"] = JetTokenTypes.JetType;
            _ookTypes["short"] = JetTokenTypes.JetType;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add { }
            remove { }
        }

        public IEnumerable<ITagSpan<JetTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {

            foreach (SnapshotSpan curSpan in spans)
            {
                ITextSnapshotLine containingLine = curSpan.Start.GetContainingLine();
                int curLoc = containingLine.Start.Position;
                string[] tokens = containingLine.GetText().ToLower().Split(' ');

                foreach (string tok in tokens)
                {
                    string ookToken = tok;
                    //eat leading whitespace
                    if (ookToken.Length > 0 && (ookToken[0] == ' ' || ookToken[0] == '\t'))
                    {
                        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(curLoc, 1));
                        if (tokenSpan.IntersectsWith(curSpan))
                            yield return new TagSpan<JetTokenTag>(tokenSpan, new JetTokenTag(JetTokenTypes.JetWhitespace));

                        curLoc += 1;
                        ookToken = ookToken.Substring(1);
                    }
                    if (_ookTypes.ContainsKey(ookToken))
                    {
                        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(curLoc, ookToken.Length));
                        if( tokenSpan.IntersectsWith(curSpan) )
                            yield return new TagSpan<JetTokenTag>(tokenSpan, new JetTokenTag(_ookTypes[ookToken]));
                    }
                    else 
                    { 
                        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(curLoc, ookToken.Length));
                        if( tokenSpan.IntersectsWith(curSpan) )
                            yield return new TagSpan<JetTokenTag>(tokenSpan, new JetTokenTag(JetTokenTypes.JetName));
                    }

                    var tokenSpan2 = new SnapshotSpan(curSpan.Snapshot, new Span(curLoc + ookToken.Length, 1));
                    if (tokenSpan2.IntersectsWith(curSpan))
                        yield return new TagSpan<JetTokenTag>(tokenSpan2, new JetTokenTag(JetTokenTypes.JetWhitespace));

                    //add an extra char location because of the space
                    curLoc += ookToken.Length + 1;
                }
            }
        }
    }
}
