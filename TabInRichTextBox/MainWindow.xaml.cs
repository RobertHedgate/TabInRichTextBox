using System.Windows.Controls;
using System.Windows.Documents;

namespace TabInRichTextBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var myFlowDoc = new FlowDocument();
            AddTextToRTF(myFlowDoc, "Text1");
            AddHyperLinkToRTF(myFlowDoc, "HyperLink1");
            AddTextToRTF(myFlowDoc, "Text2");
            AddHyperLinkToRTF(myFlowDoc, "HyperLink2");
            AddTextToRTF(myFlowDoc, "Text3");
            RTB.Document = myFlowDoc;
        }

        private static void AddHyperLinkToRTF(FlowDocument myFlowDoc, string hyperlink)
        {
            var para = new Paragraph();
            var run = new Run(hyperlink);
            var link = new Hyperlink();
            link.Inlines.Add(run);
            para.Inlines.Add(link);
            myFlowDoc.Blocks.Add(para);
        }

        private static void AddTextToRTF(FlowDocument myFlowDoc, string text)
        {
            var para = new Paragraph();
            var run = new Run(text);
            para.Inlines.Add(run);
            myFlowDoc.Blocks.Add(para);
        }
    }
}
