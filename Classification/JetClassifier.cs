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
    [TagType(typeof(ClassificationTag))]
    internal sealed class JetClassifierProvider : ITaggerProvider
    {

        [Export]
        [Name("jet")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition OokContentType = null;

        [Export]
        [FileExtension(".jet")]
        [ContentType("jet")]
        internal static FileExtensionToContentTypeDefinition OokFileType = null;

        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        [Import]
        internal IBufferTagAggregatorFactoryService aggregatorFactory = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {

            ITagAggregator<JetTokenTag> ookTagAggregator = 
                                            aggregatorFactory.CreateTagAggregator<JetTokenTag>(buffer);

            return new JetClassifier(buffer, ookTagAggregator, ClassificationTypeRegistry) as ITagger<T>;
        }
    }

    internal sealed class JetClassifier : ITagger<ClassificationTag>
    {
        ITextBuffer _buffer;
        ITagAggregator<JetTokenTag> _aggregator;
        IDictionary<JetTokenTypes, IClassificationType> _ookTypes;

        internal JetClassifier(ITextBuffer buffer, 
                               ITagAggregator<JetTokenTag> ookTagAggregator, 
                               IClassificationTypeRegistryService typeService)
        {
            _buffer = buffer;
            _aggregator = ookTagAggregator;
            _ookTypes = new Dictionary<JetTokenTypes, IClassificationType>();
            _ookTypes[JetTokenTypes.JetKeyword] = typeService.GetClassificationType("jkeyword");
            _ookTypes[JetTokenTypes.JetType] = typeService.GetClassificationType("type");
            _ookTypes[JetTokenTypes.JetWhitespace] = typeService.GetClassificationType("whitespace");

            _ookTypes[JetTokenTypes.JetName] = typeService.GetClassificationType("name");
            _ookTypes[JetTokenTypes.JetComment] = typeService.GetClassificationType("jcomment");
            _ookTypes[JetTokenTypes.JetString] = typeService.GetClassificationType("jstring");
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add { }
            remove { }
        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {

            foreach (var tagSpan in this._aggregator.GetTags(spans))
            {
                var tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
                yield return 
                    new TagSpan<ClassificationTag>(tagSpans[0], 
                                                   new ClassificationTag(_ookTypes[tagSpan.Tag.type]));
            }
        }
    }
}
