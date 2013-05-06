using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace TabInRichTextBox.Behaviors
{
    class TabToHyperlinksInRichTextBox : Behavior<RichTextBox>
    {
        private Hyperlink _previousHyperlinkFocus;

        /// <summary>
        /// Override OnAttached so we can attach the KeyDown event
        /// </summary>
        protected override void OnAttached()
        {
            // Capture keydown to capture the tab key
            AssociatedObject.KeyDown += AssociatedObjectOnKeyDown;
        }

        /// <summary>
        /// The key down function which do what we want instead of what windows wants
        /// </summary>
        /// <param name="sender">The attached object, a richtextbox here</param>
        /// <param name="e">The KeyEventArgs which contains which key where pressed</param>
        private void AssociatedObjectOnKeyDown(object sender, KeyEventArgs e)
        {
            var rtf = sender as RichTextBox;
            // if not Tab key is pressed or there are no Hyperlinks in the RichTextBox let windows handle it
            if (e.Key != Key.Tab || rtf == null || !ListOfAllHyperlinks(rtf.Document).Any()) 
                return;

            // Check if Shift is down
            var reverse = (Keyboard.Modifiers & ModifierKeys.Shift) > 0;
            // if not first Hyperlink has focus or if not _previousHyperlinkFocus is first Hyperlink when reverse is true
            if (!(HasFirstLastHyperlinkFocus(rtf.Document, !reverse)  ||
               (reverse && _previousHyperlinkFocus != null && _previousHyperlinkFocus.Equals(ListOfAllHyperlinks(rtf.Document).FirstOrDefault()))))
            {
                SetFocusToNextLink(rtf.Document, reverse);
                e.Handled = true;
            }

            _previousHyperlinkFocus = GetFocusedHyperlink(rtf.Document);
        }

        /// <summary>
        /// Move focus to next/pervious link
        /// </summary>
        /// <param name="document">The flowdocument in the RichTextBox</param>
        /// <param name="reverse">true if we should move focus backwards</param>
        private static void SetFocusToNextLink(FlowDocument document, bool reverse)
        {
            if (document == null)
                return;
            var setFocusToNext = !HasAnyHyperLinkFocus(document);
            var list = ListOfAllHyperlinks(document);
            if (reverse)
                list.Reverse();

            foreach (var link in list)
            {
                if (setFocusToNext)
                {
                    link.Focus();
                    return;
                }
                if (link.IsKeyboardFocusWithin)
                    setFocusToNext = true;
            }
        }

        /// <summary>
        /// Get a list of all hyperlinks 
        /// </summary>
        /// <param name="document">The flowdocument in the RichTextBox</param>
        /// <returns>List of all hyperlinks</returns>
        private static List<Hyperlink> ListOfAllHyperlinks(FlowDocument document)
        {
            return document == null ? null : document.Blocks.OfType<Paragraph>().Where(para => para != null).SelectMany(para => para.Inlines).OfType<Hyperlink>().ToList();
        }

        /// <summary>
        /// Get the focused hyperlink
        /// </summary>
        /// <param name="document">The flowdocument in the RichTextBox</param>
        /// <returns>The hyperlink which has focus or null is no focus</returns>
        private static Hyperlink GetFocusedHyperlink(FlowDocument document)
        {
            return document == null ? null : ListOfAllHyperlinks(document).FirstOrDefault(link => link.IsKeyboardFocusWithin);
        }

        /// <summary>
        /// See if any hyperlink has focus
        /// </summary>
        /// <param name="document">The flowdocument in the RichTextBox</param>
        /// <returns>true if any hyperlink has focus</returns>
        private static bool HasAnyHyperLinkFocus(FlowDocument document)
        {
            return document != null && ListOfAllHyperlinks(document).Any(link => link != null && link.IsKeyboardFocusWithin);
        }

        /// <summary>
        /// See if first (or last if reverse is true) hyperlink has focus
        /// </summary>
        /// <param name="document">The flowdocument in the RichTextBox</param>
        /// <param name="reverse">true if we should test last hyperlink</param>
        /// <returns>true if first link has focus</returns>
        private static bool HasFirstLastHyperlinkFocus(FlowDocument document, bool reverse)
        {
            var list = ListOfAllHyperlinks(document);
            if (reverse)
                list.Reverse();
            return document != null && list.Select(link => link.IsKeyboardFocusWithin).FirstOrDefault();
        }
    }
}
