using Aspose.Words;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Office
{
    public class WordGenerator
    {
        public static System.IO.MemoryStream FillStream(string templateFile, String[] fieldNames, Object[] fieldValues)
        {
            Document doc = new Aspose.Words.Document(templateFile);
            //DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);


            //合并模版，相当于页面的渲染
            doc.MailMerge.Execute(fieldNames, fieldValues);

            System.IO.MemoryStream s = new System.IO.MemoryStream();
            doc.Save(s, Aspose.Words.SaveFormat.Doc);
            return s;
        }

        public static System.IO.MemoryStream ConvertHtml(string Path)
        {
            //Load the document into Aspose.Words.
            Document doc = new Document(Path);

            HtmlSaveOptions options = new HtmlSaveOptions();
            options.SaveFormat = SaveFormat.Html;
            options.ImagesFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OfficeImage");

            System.IO.MemoryStream s = new System.IO.MemoryStream();

            doc.Save(s, options);

            return s;
        }

        public static void Create(string templateFile, string saveFile, Dictionary<string, string> Datas)
        {
            Document doc = new Aspose.Words.Document(templateFile);
            DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);

            foreach (Bookmark bookmark in doc.Range.Bookmarks)
            {
                bookmark.Text = Datas[bookmark.Name];
            }

            doc.Save(saveFile, Aspose.Words.SaveFormat.Docx);
        }

        private static void fillTable<T>(string bookMark, DocumentBuilder builder, List<T> source, List<string> columes)
        {
            List<double> widthList = new List<double>();

            for (int i = 0; i < columes.Count; i++)
            {
                builder.MoveToCell(0, 0, i, 0); //移动单元格
                double width = builder.CellFormat.Width;//获取单元格宽度
                widthList.Add(width);
            }

            if (builder.MoveToBookmark(bookMark))        //开始添加值
            {
                for (var i = 0; i < source.Count; i++)
                {
                    var type = source[i].GetType();

                    for (var j = 0; j < columes.Count; j++)
                    {
                        builder.InsertCell();// 添加一个单元格                    
                        builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                        builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                        builder.CellFormat.Width = widthList[j];
                        builder.CellFormat.VerticalMerge = Aspose.Words.Tables.CellMerge.None;
                        builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;//垂直居中对齐
                        builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;//水平居中对齐

                        var props = type.GetProperty(columes[j]);

                        builder.Write(props.GetValue(source[i]).ToString());
                    }

                    builder.EndRow();
                }
            }


            //doc.Range.Bookmarks["table"].Text = "";    // 清掉标示  
        }
    }
}
