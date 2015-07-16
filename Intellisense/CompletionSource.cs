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
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.IO;

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
            var str = GetFilePath(buffer);
            _buffer = buffer;
            glyphService = gly;
        }

        internal static string GetFilePath(ITextBuffer buffer)
        {
            ITextDocument doc;
            var rc = buffer.Properties.TryGetProperty<ITextDocument>(typeof(ITextDocument), out doc);
            return doc.FilePath;
        }

        internal static string GetFilePathInternal(object obj)
        {
            string fname = null;
            int hr = 0;
            IVsUserData ud = obj as IVsUserData;
            if (ud != null)
            {
                object oname;
                Guid GUID_VsBufferMoniker = typeof(IVsUserData).GUID;
                hr = ud.GetData(ref GUID_VsBufferMoniker, out oname);
                if (ErrorHandler.Succeeded(hr) && oname != null)
                    fname = oname.ToString();
            }
            if (string.IsNullOrEmpty(fname))
            {
                IPersistFileFormat fileFormat = obj as IPersistFileFormat;
                if (fileFormat != null)
                {
                    uint format;
                    hr = fileFormat.GetCurFile(out fname, out format);
                }
            }

            return fname;
        }

        // [DllImport("Racer.dll")]
        //
        //private static extern unsafe char* GetFunction(char* file, int line);

        [DllImport("Racer.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetFunction(string file, int line);

        [DllImport("Racer.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetAutoCompletes(string project, string file, int line);

        private readonly IGlyphService glyphService;
        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            if (_disposed)
                throw new ObjectDisposedException("OokCompletionSource");

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

            //IntPtr retVal = GetFunction("vector.jet", line.LineNumber + 1);
            //string retValStr = Marshal.PtrToStringAnsi(retVal);


            List<Completion> completions = new List<Completion>()
            {
                //new Completion(retValStr),
                //new Completion("Ook!"),
                //new Completion("Ook."),
                //new Completion("Ook?", "Ook?", "", glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupEnum, StandardGlyphItem.GlyphItemPublic), "Ook?")
            };
            //need to somehow pass the filename
            var path = GetFilePath(this._buffer);
            var name = Path.GetFileName(path);
            // var name = path.GetFileName();
            IntPtr retVal2 = GetAutoCompletes(path, name, line.LineNumber + 1);
            string retValStr2 = Marshal.PtrToStringAnsi(retVal2);

            //append a char that identifies the type info
            string[] suggestions = retValStr2.Split('/');
            for (int i = 0; i < suggestions.Length; i++)
            {
                var str = suggestions[i];
                if (str != "/" && str.Length > 0)
                {
                    string sname = str.Substring(0, str.Length - 1);
                    string desc = suggestions[++i];
                    completions.Add(new Completion(sname, sname, desc, GetGlyphForCode(str[str.Length - 1]), sname));
                }
            }

            //refine here
            completionSets.Add(new CompletionSet("All", "All", applicableTo, completions, Enumerable.Empty<Completion>()));
        }

        public System.Windows.Media.ImageSource GetGlyphForCode(char c)
        {
            switch (c)
            {
                case 'L':
                    return glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupVariable, StandardGlyphItem.GlyphItemPublic);
                case 'F':
                    return glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupMethod, StandardGlyphItem.GlyphItemPublic);
                default:
                    return glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupEnum, StandardGlyphItem.GlyphItemPublic);
            }
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}

