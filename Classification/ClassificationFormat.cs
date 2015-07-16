using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Language;

namespace OokLanguage
{
    #region Format definition

    /// <summary>
    /// Defines an editor format for the OrdinaryClassification type that has a purple background
    /// and is underlined.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "jkeyword")]
    [Name("jkeyword")]
    //this should be visible to the end user
    [UserVisible(true)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.High)]
    [Order(After = Priority.High)]
    internal sealed class JetKeyword : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public JetKeyword()
        {
            this.DisplayName = "Jet Keyword"; //human readable version of the name
            this.ForegroundColor = Colors.Blue;
            this.BackgroundColor = Colors.White;
        }
    }

    /// <summary>
    /// Defines an editor format for the OrdinaryClassification type that has a purple background
    /// and is underlined.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "type")]
    [Name("type")]
    //this should be visible to the end user
    [UserVisible(true)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.High)]
    internal sealed class JetType : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public JetType()
        {

            //this.TextDecorations = System.Windows.TextDecorations.; 
            this.DisplayName = "Jet Type"; //human readable version of the name
            this.ForegroundColor = Colors.CornflowerBlue;
            this.BackgroundColor = Colors.White;

            this.TextDecorations = null;
        }
    }

    /// <summary>
    /// Defines an editor format for the OrdinaryClassification type that has a purple background
    /// and is underlined.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "whitespace")]
    [Name("whitespace")]
    //this should be visible to the end user
    [UserVisible(true)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.High)]
    [Order(After = Priority.High)]
    internal sealed class JetWhitespace : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public JetWhitespace()
        {
            this.DisplayName = "Jet Whitespace"; //human readable version of the name
            this.ForegroundColor = Colors.Black;
            this.BackgroundColor = Colors.White;
        }
    }

    /// <summary>
    /// Defines an editor format for the OrdinaryClassification type that has a purple background
    /// and is underlined.
    /// </summary>
   

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "name")]
    [Name("name")]
    //this should be visible to the end user
    [UserVisible(true)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.High)]
    [Order(After = Priority.High)]
    internal sealed class JetName : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public JetName()
        {
            this.DisplayName = "Jet Identifier"; //human readable version of the name
            this.ForegroundColor = Colors.Black;
            this.BackgroundColor = Colors.White;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "jcomment")]
    [Name("jcomment")]
    //this should be visible to the end user
    [UserVisible(true)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.High)]
    [Order(After = Priority.High)]
    internal sealed class JetComment : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public JetComment()
        {
            this.DisplayName = "Jet Comment"; //human readable version of the name
            this.ForegroundColor = Colors.Green;
            this.BackgroundColor = Colors.White;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "jstring")]
    [Name("jstring")]
    //this should be visible to the end user
    [UserVisible(true)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.High)]
    [Order(After = Priority.High)]
    internal sealed class JetString : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public JetString()
        {
            this.DisplayName = "Jet String"; //human readable version of the name
            this.ForegroundColor = Colors.Red;
            this.BackgroundColor = Colors.White;
        }
    }
    #endregion //Format definition
}
