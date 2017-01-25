using Aspose.Words;
using Aspose.Words.Lists;
using DM.Base.Service;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var MyDir = "D:\\临时\\";

            Aspose.Words.Document srcDoc = new Aspose.Words.Document(MyDir + "变电一次.docx");
            DocumentBuilder builder = new DocumentBuilder(srcDoc);
            srcDoc.UpdateListLabels();

            //Aspose.Words.Document dstDoc = new Aspose.Words.Document();
            //int ctr = 0;
            //Aspose.Words.NodeImporter imp = new Aspose.Words.NodeImporter(srcDoc, dstDoc, Aspose.Words.ImportFormatMode.KeepSourceFormatting);

            foreach (Aspose.Words.Paragraph paragraph in srcDoc.GetChildNodes(Aspose.Words.NodeType.Paragraph, true))
            {
                if (paragraph.IsListItem)
                {
                    ListLabel label = paragraph.ListLabel;

                    Console.WriteLine(string.Format("{0}     {1}", label.LabelValue, paragraph.Range.Text));

                    //builder.MoveTo(paragraph);
                    //builder.StartBookmark("bookmark_" + label.LabelValue);
                    //builder.EndBookmark("bookmark_" + label.LabelValue);

                    //Aspose.Words.Node impNode = imp.ImportNode(paragraph, true);

                    //dstDoc.FirstSection.Body.RemoveAllChildren();
                    //dstDoc.FirstSection.Body.AppendChild(impNode);

                    //foreach (Bookmark bookmark in ((Aspose.Words.Paragraph)impNode).Range.Bookmarks)
                    //{
                    //    if (!bookmark.Name.StartsWith("bookmark_"))
                    //        continue;

                    //    String listLabel = bookmark.Name.Replace("bookmark_", "");

                    //    try
                    //    {
                    //        ((Aspose.Words.Paragraph)impNode).ListFormat.ListLevel.StartAt = Convert.ToInt32(listLabel);
                    //        break;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //    }
                    //}

                    //ctr++;
                    //dstDoc.Range.Bookmarks.Clear();
                }
            }

            //dstDoc.Save(MyDir + "out.docx");

            Console.ReadKey();
        }

    }
}
