using Aspose.Words;
using Aspose.Words.Tables;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Merge.Base.Service
{
    public class KYWordMergeService
    {
        private MergeOption _MergeOption;

        private string[] _NumberChinesMap;

        public KYWordMergeService(MergeOption Option)
        {
            this._MergeOption = Option;

            if (this._MergeOption == null)
            {
                this._MergeOption = new MergeOption();

                this._MergeOption.Head1Style = new FontStyle();
                this._MergeOption.Head1Style.Align = ParagraphAlignment.Left;
                this._MergeOption.Head1Style.ChapterSpaceCount = 3;
                this._MergeOption.Head1Style.FontFamily = FontFamily.宋体;
                this._MergeOption.Head1Style.Size = 14;

                this._MergeOption.Head2Style = new FontStyle();
                this._MergeOption.Head2Style.Align = ParagraphAlignment.Left;
                this._MergeOption.Head2Style.ChapterSpaceCount = 3;
                this._MergeOption.Head2Style.FontFamily = FontFamily.宋体;
                this._MergeOption.Head2Style.Size = 14;

                this._MergeOption.ContentStyle = new FontStyle();
                this._MergeOption.ContentStyle.Align = ParagraphAlignment.Left;
                this._MergeOption.ContentStyle.FontFamily = FontFamily.宋体;
                this._MergeOption.ContentStyle.Size = 14;

                this._MergeOption.ListStyle = new FontStyle();
                this._MergeOption.ListStyle.FontFamily = FontFamily.宋体;
                this._MergeOption.ListStyle.Size = 14;

                this._MergeOption.PageNumberStyle = new FontStyle();
                this._MergeOption.PageNumberStyle.FontFamily = FontFamily.宋体;
                this._MergeOption.PageNumberStyle.Size = 14;

                this._MergeOption.HeadNumber = HeadNumber.Chinese;
            }
            if (this._MergeOption.Head1Style == null)
            {
                this._MergeOption.Head1Style = new FontStyle();
                this._MergeOption.Head1Style.Align = ParagraphAlignment.Left;
                this._MergeOption.Head1Style.ChapterSpaceCount = 3;
                this._MergeOption.Head1Style.FontFamily = FontFamily.宋体;
                this._MergeOption.Head1Style.Size = 14;
            }
            if (this._MergeOption.Head2Style == null)
            {
                this._MergeOption.Head2Style = new FontStyle();
                this._MergeOption.Head2Style.Align = ParagraphAlignment.Left;
                this._MergeOption.Head2Style.ChapterSpaceCount = 3;
                this._MergeOption.Head2Style.FontFamily = FontFamily.宋体;
                this._MergeOption.Head2Style.Size = 14;
            }
            if (this._MergeOption.ContentStyle == null)
            {
                this._MergeOption.ContentStyle = new FontStyle();
                this._MergeOption.ContentStyle.FontFamily = FontFamily.宋体;
                this._MergeOption.ContentStyle.Size = 14;
            }
            if (this._MergeOption.ListStyle == null)
            {
                this._MergeOption.ListStyle = new FontStyle();
                this._MergeOption.ListStyle.FontFamily = FontFamily.宋体;
                this._MergeOption.ListStyle.Size = 14;
            }
            if (this._MergeOption.PageNumberStyle == null)
            {
                this._MergeOption.PageNumberStyle = new FontStyle();
                this._MergeOption.PageNumberStyle.FontFamily = FontFamily.宋体;
                this._MergeOption.PageNumberStyle.Size = 14;
            }

            if (this._MergeOption.NewPageChapters == null)
            {
                this._MergeOption.NewPageChapters = new List<string>();
            }

            var total = 30;

            this._NumberChinesMap = new string[total];

            for (int i = 0; i < total; i++)
            {
                this._NumberChinesMap[i] = numberToChinese(i + 1);
            }
        }

        /// <summary>
        /// 需要检查和替换的单位
        /// </summary>
        private Dictionary<string, string> _UnitMaps = new Dictionary<string, string>()
        {
            { "KV","kV"}, {"KM","km" }, {"KG","kg" },{"AKG","AKG" }, {"KVAR","kvar" }, {"KW","kW" }, {"KPA","kPa" }, {"MPA","MPa" }
        };

        public Regex ParaReg1 = new Regex(@"[0-9]*");
        public Regex ParaReg1Index = new Regex(@"[0-9]*");
        public Regex ParaReg2 = new Regex(@"^[0-9]\d*\.[0-9]\d*");
        public Regex ParaReg3 = new Regex(@"^[0-9]\d*\.[0-9]\d*\.[0-9]\d*");
        public Regex ParaReg4 = new Regex(@"^[0-9]\d*\.[0-9]\d*\.[0-9]\d*\.[0-9]\d*");
        public Regex ParaReg5 = new Regex(@"^[0-9]\d*\.[0-9]\d*\.[0-9]\d*\.[0-9]\d*\.[0-9]\d*");

        public void SetDocNode(Document doc, Dictionary<string, List<DocNode>> allNode)
        {
            doc.UpdateListLabels();

            foreach (Section sc in doc.Sections)
            {
                var docAllNodes = sc.Body.GetChildNodes(NodeType.Any, false);
                foreach (Node node in docAllNodes)
                {
                    if (node.NodeType == NodeType.Paragraph)
                    {
                        var para = node as Paragraph;

                        if (para.ParagraphFormat.StyleIdentifier == StyleIdentifier.Heading1)
                        {
                            var m1 = ParaReg1.Match(para.Range.Text.TrimStart());
                            if (m1.Success)
                            {
                                // 一级标题
                                //var indexStr = ParaReg1Index.Match(m1.Value, 1).Value;
                                AddNode(allNode, node, m1.Value);
                            }
                        }
                        else if (para.ParagraphFormat.StyleIdentifier == StyleIdentifier.Heading2)
                        {
                            var m2 = ParaReg2.Match(para.Range.Text.TrimStart());
                            if (m2.Success)
                            {
                                // 二级标题
                                var indexStr = m2.Value;
                                AddNode(allNode, node, indexStr);
                            }
                        }
                        else
                        {
                            if (para.IsListItem)
                            {
                                var c = para.ListLabel.LabelValue;

                                allNode.Last().Value.Add(new DocNode() { Node = node, IsListNode = true, ListValue = para.ListLabel.LabelValue });
                            }
                            else
                            {
                                allNode.Last().Value.Add(new DocNode() { Node = node });
                            }

                        }
                    }
                    else if (node.NodeType == NodeType.Table)
                    {
                        allNode.Last().Value.Add(new DocNode() { Node = node });
                    }
                }
            }

        }

        public void AddNode(Dictionary<string, List<DocNode>> allNode, Node node, string indexStr)
        {
            //var index = 0;
            //if (int.TryParse(indexStr, out index))
            //{

            //}

            if (allNode.ContainsKey(indexStr))
            {
                indexStr += "_";
            }

            allNode.Add(indexStr, new List<DocNode>() { new DocNode { Node = node } });
        }

        /// <summary>
        /// 设置文档标准样式
        /// </summary>
        /// <param name="doc"></param>
        public void SetDocStyle(Document doc)
        {
            // 一级标题宋体4号居中
            var head1 = doc.Styles[StyleIdentifier.Heading1];
            head1.Font.Name = this._MergeOption.Head1Style.FontFamily.ToString();
            head1.Font.Size = this._MergeOption.Head1Style.Size;
            head1.Font.Bold = this._MergeOption.Head1Style.Bold;
            head1.Font.Italic = this._MergeOption.Head1Style.Italic;
            head1.ParagraphFormat.Alignment = this._MergeOption.Head1Style.Align;
            head1.ParagraphFormat.LeftIndent = ConvertUtil.MillimeterToPoint(0);
            head1.ParagraphFormat.LineSpacingRule = LineSpacingRule.Exactly; // 行间距 固定值
            head1.ParagraphFormat.LineSpacing = 23;      // 行间距 23磅
            head1.ParagraphFormat.SpaceBefore = 0;      // 段前
            head1.ParagraphFormat.SpaceAfter = 0;       // 段后

            // 二级标题宋体4号 首行缩进
            var head2 = doc.Styles[StyleIdentifier.Heading2];
            head2.Font.Name = this._MergeOption.Head2Style.FontFamily.ToString();
            head2.Font.Size = this._MergeOption.Head2Style.Size;
            head2.Font.Bold = this._MergeOption.Head2Style.Bold;
            head2.Font.Italic = this._MergeOption.Head2Style.Italic;
            head2.ParagraphFormat.Alignment = this._MergeOption.Head2Style.Align;
            head2.ParagraphFormat.FirstLineIndent = ConvertUtil.MillimeterToPoint(10);
            head2.ParagraphFormat.LeftIndent = ConvertUtil.MillimeterToPoint(0);
            head2.ParagraphFormat.LineSpacingRule = LineSpacingRule.Exactly; // 行间距 固定值
            head2.ParagraphFormat.LineSpacing = 23;      // 行间距 23磅
            head2.ParagraphFormat.SpaceBefore = 0;      // 段前
            head2.ParagraphFormat.SpaceAfter = 0;       // 段后

            // 正文宋体4号
            var contextNormal = doc.Styles[StyleIdentifier.Normal];
            contextNormal.Font.Name = this._MergeOption.ContentStyle.FontFamily.ToString();
            contextNormal.Font.Size = this._MergeOption.ContentStyle.Size;
            contextNormal.ParagraphFormat.Alignment = this._MergeOption.ContentStyle.Align;
            contextNormal.ParagraphFormat.FirstLineIndent = ConvertUtil.MillimeterToPoint(10);
            contextNormal.ParagraphFormat.LeftIndent = ConvertUtil.MillimeterToPoint(0);
            contextNormal.ParagraphFormat.LineSpacingRule = LineSpacingRule.Exactly; // 行间距 固定值
            contextNormal.ParagraphFormat.LineSpacing = 23;      // 行间距 23磅

            doc.UpdatePageLayout();
        }

        /// <summary>
        /// 给文档创建一个新的节
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public Section CreateSection(Document doc)
        {
            Section section = new Section(doc);
            section.PageSetup.PaperSize = PaperSize.A4;
            section.PageSetup.TopMargin = ConvertUtil.MillimeterToPoint(2.5 * 10);
            section.PageSetup.BottomMargin = ConvertUtil.MillimeterToPoint(2.5 * 10);
            section.PageSetup.LeftMargin = ConvertUtil.MillimeterToPoint(2.3 * 10);
            section.PageSetup.RightMargin = ConvertUtil.MillimeterToPoint(2.3 * 10);

            section.AppendChild(new Body(doc));

            return section;
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="doc"></param>
        public void CreateList(Document doc)
        {
            Section sc = new Section(doc);
            doc.Sections.Insert(0, sc);

            sc.PageSetup.PaperSize = PaperSize.A4;
            sc.PageSetup.TopMargin = ConvertUtil.MillimeterToPoint(2.5 * 10);
            sc.PageSetup.BottomMargin = ConvertUtil.MillimeterToPoint(2.5 * 10);
            sc.PageSetup.LeftMargin = ConvertUtil.MillimeterToPoint(2.3 * 10);
            sc.PageSetup.RightMargin = ConvertUtil.MillimeterToPoint(2.3 * 10);

            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveToSection(0);

            doc.FirstSection.Body.PrependChild(new Paragraph(doc));
            builder.MoveToDocumentStart();

            builder.CurrentParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;

            builder.Writeln("目录");

            builder.InsertTableOfContents("\\o \"1-3\" \\h \\z \\u");

            doc.UpdateFields();


            var ps = sc.Body.GetChildNodes(NodeType.Paragraph, false);

            Regex regex2 = new Regex(@"[0-9]\d*\.[0-9]\d*");

            foreach (Paragraph para in ps)
            {
                if (para.Range.Text != "目录\r")
                {
                    para.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                    para.ParagraphFormat.LineSpacingRule = LineSpacingRule.Exactly; // 行间距 固定值
                    para.ParagraphFormat.LineSpacing = 23;      // 行间距 23磅

                    para.ParagraphFormat.FirstLineIndent = 0;
                    para.ParagraphFormat.LeftIndent = 0;

                    if (regex2.IsMatch(para.Range.Text))
                    {
                        para.ParagraphFormat.LeftIndent = ConvertUtil.MillimeterToPoint(10);   // 每个段落首行缩进2个字符 = 1厘米
                    }

                    for (int i = 0; i < para.Runs.Count; i++)
                    {
                        para.Runs[i].Font.Name = this._MergeOption.ListStyle.FontFamily.ToString();
                        para.Runs[i].Font.Size = this._MergeOption.ListStyle.Size;
                        para.Runs[i].Font.Bold = this._MergeOption.ListStyle.Bold;
                        para.Runs[i].Font.Italic = this._MergeOption.ListStyle.Italic;
                    }
                }
            }
        }

        /// <summary>
        /// 设置段落格式，文字样式，标点符号、单位替换
        /// </summary>
        /// <param name="para"></param>
        public void SetParaStyle(Paragraph para)
        {
            // 1. 设置段落格式
            if (para.GetChildNodes(NodeType.Shape, false).Count > 0 ||
               para.GetChildNodes(NodeType.Table, false).Count > 0)
            {
                // 段落内有表格和图片，不设置行距，不设置缩进
                para.ParagraphFormat.FirstLineIndent = 0;
                para.ParagraphFormat.LeftIndent = 0;
                para.ParagraphFormat.LineSpacingRule = LineSpacingRule.AtLeast;
            }

            // 段落居中不设置缩进
            if (para.ParagraphFormat.Alignment == ParagraphAlignment.Center)
            {
                para.ParagraphFormat.FirstLineIndent = 0;   // 每个段落首行缩进2个字符 = 1厘米
                para.ParagraphFormat.LeftIndent = 0;        // 缩进
            }

            // 2. 设置段落内容
            setParaContent(para);
        }

        /// <summary>
        /// 设置章节号
        /// </summary>
        /// <param name="index"></param>
        /// <param name="para"></param>
        public void SetChapter(string index, Paragraph para)
        {
            if (para.ParagraphFormat.StyleIdentifier == StyleIdentifier.Heading1)
            {
                // 一级标题

                var newPage = para.Range.Text.StartsWith("\f");

                var leftPart = ParaReg1.Replace(para.Range.Text.Trim().Trim(new char[] { '\'', '\"', '\\', '\0', '\a', '\b', '\f', '\n', '\r', '\t', '\v' }), "").Trim();


                // 第4章前后,工程造价分析章节 分页
                if (newPage || this._MergeOption.NewPageChapters.Contains(index))
                {
                    para.ParagraphFormat.PageBreakBefore = true;
                }

                para.Runs.Clear();

                var intIndex = int.Parse(index.Trim());

                var r = new Run(para.Document)
                {
                    Text = string.Format("{0}{1}{2}",
                    this._MergeOption.HeadNumber == HeadNumber.Chinese ? this._NumberChinesMap[intIndex - 1] : index.ToString(),
                    "".PadLeft(this._MergeOption.Head1Style.ChapterSpaceCount, ' '),
                    leftPart)
                };

                
                para.Runs.Add(r);
            }
            else if (para.ParagraphFormat.StyleIdentifier == StyleIdentifier.Heading2)
            {
                // 二级标题

                var leftPart = ParaReg2.Replace(para.Range.Text.Trim().Trim(new char[] { '\'', '\"', '\\', '\0', '\a', '\b', '\f', '\n', '\r', '\t', '\v' }), "").Trim();

                para.Runs.Clear();

                var newPage = para.Range.Text.StartsWith("\f");

                if (newPage)
                {
                    para.ParagraphFormat.PageBreakBefore = true;
                }

                var r = new Run(para.Document)
                {
                    Text = string.Format("{0}{1}{2}", index, "".PadLeft(this._MergeOption.Head2Style.ChapterSpaceCount, ' '), leftPart)
                };

                para.Runs.Add(r);
            }
        }

        /// <summary>
        /// 设置文档页码
        /// </summary>
        /// <param name="doc"></param>
        public void SetPageNumber(Document doc)
        {
            Aspose.Words.DocumentBuilder builder = new DocumentBuilder(doc);

            // Go to the primary footer
            builder.MoveToHeaderFooter(HeaderFooterType.FooterPrimary);
            builder.CurrentParagraph.RemoveAllChildren();

            builder.ParagraphFormat.Alignment = this._MergeOption.PageNumberStyle.Align;
            builder.ParagraphFormat.FirstLineIndent = 0;

            builder.PageSetup.PageStartingNumber = 1;   // 开始页码
            builder.PageSetup.RestartPageNumbering = true;  //       重新开始编号
            builder.PageSetup.DifferentFirstPageHeaderFooter = false;   // 奇偶页不同

            // 版式：页眉1.5cm，页脚：2cm
            builder.PageSetup.HeaderDistance = ConvertUtil.MillimeterToPoint(1.5 * 10);
            builder.PageSetup.FooterDistance = ConvertUtil.MillimeterToPoint(2 * 10);
            builder.PageSetup.PageNumberStyle = NumberStyle.Arabic;

            builder.Font.Name = this._MergeOption.PageNumberStyle.FontFamily.ToString();
            builder.Font.Size = this._MergeOption.PageNumberStyle.Size;
            builder.Font.Bold = this._MergeOption.PageNumberStyle.Bold;
            builder.Font.Italic = this._MergeOption.PageNumberStyle.Italic;

            //builder.Write("-");
            builder.InsertField("PAGE", "");
            //builder.Write("-");

            // 设置正文样式 去除页眉横线
            builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
            builder.CurrentParagraph.RemoveAllChildren();
            builder.ParagraphFormat.ClearFormatting();
            //builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Normal;
        }

        /// <summary>
        /// 设置表格
        /// </summary>
        /// <param name="doc"></param>
        public void SetTable(Document doc)
        {
            var tables = doc.GetChildNodes(NodeType.Table, true);
            SetTable(tables);
        }

        /// <summary>
        /// 设置表格
        /// </summary>
        /// <param name="table"></param>
        public void SetTable(NodeCollection tables)
        {
            foreach (Table table in tables)
            {
                var regTitle = new Regex(@"^表[1-9]\d*");

                var paraTitleHead = table.PreviousSibling as Paragraph; // 上标题
                var paraTitleFoot = table.NextSibling as Paragraph;   // 下标题

                if (paraTitleHead != null && regTitle.IsMatch(paraTitleHead.Range.Text.Trim()))
                {
                    // 以表1开头的段落认为是表格的标题
                    paraTitleHead.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                }

                if (paraTitleFoot != null && regTitle.IsMatch(paraTitleFoot.Range.Text.Trim()))
                {
                    // 以表1开头的段落认为是表格的标题
                    paraTitleFoot.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                }

                // 表格居中
                table.Alignment = Aspose.Words.Tables.TableAlignment.Center;
                // 表格宽度100%
                table.PreferredWidth = PreferredWidth.FromPercent(100);

                var cells = table.GetChildNodes(NodeType.Cell, true);

                foreach (Cell c in cells)
                {
                    // 表格内边框0.5磅
                    c.CellFormat.Borders.LineWidth = 0.5;
                    // 内容垂直居中
                    c.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;


                    // 单元格内段落居中
                    foreach (Paragraph p in c.Paragraphs)
                    {
                        // 表格内段落居中、不缩进，行距最小值
                        p.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                        p.ParagraphFormat.FirstLineIndent = 0;
                        p.ParagraphFormat.LeftIndent = 0;
                        p.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                        p.ParagraphFormat.LineSpacing = 12;  // 单倍行距

                        // 设置表格内段落内容
                        setParaContent(p);
                    }
                }

                // 外边框1.5磅
                table.SetBorder(BorderType.Top, LineStyle.Single, 1.5, System.Drawing.Color.Black, true);
                table.SetBorder(BorderType.Bottom, LineStyle.Single, 1.5, System.Drawing.Color.Black, true);
                table.SetBorder(BorderType.Left, LineStyle.Single, 1.5, System.Drawing.Color.Black, true);
                table.SetBorder(BorderType.Right, LineStyle.Single, 1.5, System.Drawing.Color.Black, true);
            }
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="doc"></param>
        public void SetImage(Document doc)
        {
            var images = doc.GetChildNodes(NodeType.Shape, true);
            SetImage(images);
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="images"></param>
        public void SetImage(NodeCollection images)
        {
            var regTitle = new Regex(@"^图[1-9]\d*");

            foreach (Aspose.Words.Drawing.Shape img in images)
            {
                if (img.IsImage)
                {
                    if (img.ParentParagraph != null)
                    {
                        var paraTitleHead = img.ParentParagraph.PreviousSibling as Paragraph; // 图上标题
                        var paraTitleFoot = img.ParentParagraph.NextSibling as Paragraph;   // 图下标题

                        if (paraTitleHead != null && regTitle.IsMatch(paraTitleHead.Range.Text.Trim()))
                        {
                            // 以图1开头的段落认为是图片的标题
                            paraTitleHead.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                        }
                        if (paraTitleFoot != null && regTitle.IsMatch(paraTitleFoot.Range.Text.Trim()))
                        {
                            // 以图1开头的段落认为是图片的标题
                            paraTitleFoot.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                        }

                        // 有图片的段落行距最小值,居中，不缩进
                        img.ParentParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                        img.ParentParagraph.ParagraphFormat.LineSpacingRule = LineSpacingRule.AtLeast;
                        img.ParentParagraph.ParagraphFormat.FirstLineIndent = 0;
                        img.ParentParagraph.ParagraphFormat.LeftIndent = 0;
                    }

                    // 文字环绕 嵌入型
                    img.WrapType = Aspose.Words.Drawing.WrapType.Inline;

                    // 图片居中
                    img.HorizontalAlignment = Aspose.Words.Drawing.HorizontalAlignment.Center;
                }
            }
        }

        /// <summary>
        /// 附加文件内容
        /// </summary>
        /// <param name="docSource"></param>
        /// <param name="docAttachList"></param>
        public void AppendDoc(Document docSource, Document docAttachList, int index = 0)
        {
            var sc = CreateSection(docSource);

            foreach (Section item in docAttachList.Sections)
            {
                sc.AppendContent(item);
            }

            var paras = sc.GetChildNodes(NodeType.Paragraph, true);

            foreach (Paragraph p in paras)
            {
                SetParaStyle(p);
            }

            docSource.Sections.Insert(0, sc);
        }

        public void AppenCover(Document docSource, Document docCover)
        {
            var sc = CreateSection(docSource);

            var tables = docCover.GetChildNodes(NodeType.Table, true);

            foreach (Node table in tables)
            {
                Node dstNode = docSource.ImportNode(table, true, ImportFormatMode.KeepSourceFormatting);
                sc.Body.AppendChild(dstNode);
            }

            docSource.Sections.Add(sc);
        }

        /// <summary>
        /// 添加附件
        /// </summary>
        /// <param name="docSource"></param>
        /// <param name="docAttachList"></param>
        public void AppendAttachDoc(Document docSource, Document attchDoc)
        {
            var ps = attchDoc.GetChildNodes(NodeType.Paragraph, true);

            int index0 = 0, index1 = 0, index2 = 0;

            foreach (Paragraph p in ps)
            {
                if (p.ParentNode.NodeType != NodeType.Body)
                {
                    continue;
                }

                if (p.IsListItem)
                {
                    var r = new Run(attchDoc);

                    var l = p.ListFormat.ListLevel.NumberFormat.Split('.').Length;

                    switch (p.ListFormat.ListLevelNumber)
                    {
                        case 0:
                            {
                                index0++;
                                index1 = index2 = 0;
                            }
                            break;
                        case 1:
                            {
                                index1++;
                                index2 = 0;
                            }
                            break;
                        default:
                            {
                                index2++;
                            }
                            break;
                    }

                    switch (l)
                    {
                        case 1:
                            {
                                r.Text = index0.ToString();
                                //Console.WriteLine(string.Format("{0}  {1}", index0, p.Range.Text));
                            }
                            break;
                        case 2:
                            {
                                r.Text = string.Format("{0}.{1}", index0, index1);
                                //Console.WriteLine(string.Format("{0}.{1}  {2}", index0, index1, p.Range.Text));
                            }
                            break;
                        default:
                            {
                                r.Text = string.Format("{0}.{1}.{2}", index0, index1, index2);
                                //Console.WriteLine(string.Format("{0}.{1}.{2}  {3}", index0, index1, index2, p.Range.Text));
                            }
                            break;
                    }

                    p.Runs.Insert(0, r);
                }

                p.ParagraphFormat.ClearFormatting();
                // 段落所有的样式设置为正文
                p.ParagraphFormat.StyleIdentifier = StyleIdentifier.Normal;

                SetParaStyle(p);
            }

            // 重新设置页码
            SetPageNumber(attchDoc);

            attchDoc.FirstSection.PageSetup.RestartPageNumbering = false;

            docSource.AppendDocument(attchDoc, ImportFormatMode.UseDestinationStyles);
        }

        /// <summary>
        /// 区域检查
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="areaName"></param>
        public void CheckArea(Document doc, string areaName, string regAreas, MergeResult result)
        {
            result.ContainsArea = doc.Range.Text.Contains(areaName);

            Regex regex = new Regex(regAreas);

            var matchs = regex.Matches(doc.Range.Text);

            if (matchs.Count > 0)
            {
                result.OtherAreas = new Dictionary<string, int>();

                foreach (Match m in matchs)
                {
                    if (result.OtherAreas.ContainsKey(m.Value))
                    {
                        result.OtherAreas[m.Value]++;
                    }
                    else
                    {
                        result.OtherAreas.Add(m.Value, 1);
                    }

                }
            }
        }

        /// <summary>
        /// 禁用内容检查
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="disableWord"></param>
        /// <param name="result"></param>
        public void CheckDisableContent(Document doc, string disableWord, MergeResult result)
        {
            if (string.IsNullOrEmpty(disableWord))
            {
                return;
            }

            var disableWordRegStr = string.Format("({0})", disableWord.Replace(",", ")|("));
            Regex regex = new Regex(disableWordRegStr);

            var matchs = regex.Matches(doc.Range.Text);

            if (matchs.Count > 0)
            {
                result.DisableWordMatchs = new List<string>();

                foreach (Match m in matchs)
                {
                    result.DisableWordMatchs.Add(m.Value);
                }
            }
        }

        /// <summary>
        /// 设置段落内容
        /// </summary>
        /// <param name="para"></param>
        private void setParaContent(Paragraph para)
        {
            // 2. 设置段落文字
            if (para.Runs.Count == 0)
            {
                return;
            }

            // 将段落中的图形中的文字排除
            var sb = new StringBuilder();
            foreach (Run r in para.Runs)
            {
                sb.Append(r.Text);
            }

            var text = sb.ToString();

            if (string.IsNullOrEmpty(text.Trim().Trim(new char[] { '\'', '\"', '\\', '\0', '\a', '\b', '\f', '\n', '\r', '\t', '\v' })))
            {
                return;
            }

            // 单位检查
            if (text.Contains("m2") || text.Contains("m3"))
            {
                // 单位上标
                superscript(para);
            }

            // 单位标准替换
            foreach (var item in _UnitMaps)
            {
                text = Strings.Replace(text, item.Key, item.Value, Compare: CompareMethod.Text);
            }

            int start = 0; int len = text.Length;

            var isFieldCode = false;

            for (int i = 0; i < para.Runs.Count && start < len; i++)
            {
                var length = para.Runs[i].Text.Length;

                para.Runs[i].Text = text.Substring(start, length);

                if (para.Runs[i].PreviousSibling != null && para.Runs[i].PreviousSibling.NodeType == NodeType.FieldStart)
                {
                    isFieldCode = true;
                }

                if (para.Runs[i].PreviousSibling != null && para.Runs[i].PreviousSibling.NodeType == NodeType.FieldEnd)
                {
                    isFieldCode = false;
                }

                if (!isFieldCode)
                {
                    if (para.ParagraphFormat.StyleIdentifier == StyleIdentifier.Heading1)
                    {
                        para.Runs[i].Font.Name = this._MergeOption.Head1Style.FontFamily.ToString();
                        para.Runs[i].Font.Size = this._MergeOption.Head1Style.Size;
                        para.Runs[i].Font.Bold = this._MergeOption.Head1Style.Bold;
                    }
                    else if (para.ParagraphFormat.StyleIdentifier == StyleIdentifier.Heading2)
                    {
                        para.Runs[i].Font.Name = this._MergeOption.Head2Style.FontFamily.ToString();
                        para.Runs[i].Font.Size = this._MergeOption.Head2Style.Size;
                        para.Runs[i].Font.Bold = this._MergeOption.Head2Style.Bold;
                    }
                    else
                    {
                        para.Runs[i].Font.Name = this._MergeOption.ContentStyle.FontFamily.ToString();
                        para.Runs[i].Font.Size = this._MergeOption.ContentStyle.Size;
                    }
                }

                // 标点符号英转中
                if (!isFieldCode)
                {
                    para.Runs[i].Text = para.Runs[i].Text
                       .Replace(',', '，')
                       .Replace(';', '；')
                       .Replace(':', '：')
                       .Replace('(', '（')
                       .Replace(')', '）')
                       .Replace('~', '～');
                }

                start += length;
            }

            // 移除开始的空格
            foreach (Run r in para.Runs)
            {
                if (string.IsNullOrEmpty(r.Text.Trim()))
                {
                    r.Remove();
                }
                else
                {
                    r.Text = r.Text.TrimStart();
                    break;
                }
            }
        }

        /// <summary>
        /// 标点符号替换
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private string punctuation(string content)
        {
            return content
                .Replace(',', '，')
                .Replace(';', '；')
                .Replace(':', '：')
                .Replace('(', '（')
                .Replace(')', '）')
                .Replace('~', '～');
        }

        /// <summary>
        /// m2,m3上标
        /// </summary>
        /// <param name="para"></param>
        private void superscript(Paragraph para)
        {
            for (int i = 0; i < para.Runs.Count;)
            {
                string text = para.Runs[i].Text;

                if (text.Contains("m2") || text.Contains("m3"))
                {
                    #region 单位就在Run里面

                    para.Runs.RemoveAt(i);

                    var items = text.Split(new string[] { "m2", "m3" }, StringSplitOptions.RemoveEmptyEntries);

                    if (items.Length == 0)
                    {
                        var txts = text.Split(new string[] { "2", "3" }, StringSplitOptions.RemoveEmptyEntries);
                        var nums = text.Replace("m", "").ToArray();

                        foreach (var n in nums)
                        {
                            var run1 = new Run(para.Document);
                            run1.Text = "m";
                            para.Runs.Insert(i, run1);

                            var run2 = new Run(para.Document);
                            run2.Font.Superscript = true;
                            run2.Text = n.ToString();
                            para.Runs.Insert(i + 1, run2);

                            i += 2;
                        }
                    }
                    else
                    {
                        var index = 0;

                        foreach (var item in items)
                        {
                            index += item.Length + 1;

                            if (items.Last() != item || text.EndsWith("m2") || text.EndsWith("m3"))
                            {
                                var run1 = new Run(para.Document);
                                run1.Text = item + "m";
                                para.Runs.Insert(i, run1);

                                var run2 = new Run(para.Document);
                                run2.Font.Superscript = true;
                                run2.Text = text.Substring(index, 1);
                                para.Runs.Insert(i + 1, run2);

                                i += 2;
                            }
                            else
                            {
                                var run1 = new Run(para.Document);
                                run1.Text = item;
                                para.Runs.Insert(i, run1);

                                i += 1;
                            }

                            index++;
                        }
                    }
                    #endregion
                }
                else if (text.EndsWith("m") && i < para.Runs.Count - 1 && (para.Runs[i + 1].Text.StartsWith("2") || para.Runs[i + 1].Text.StartsWith("3")))
                {
                    #region m2 m3被拆分在两个Run的结尾开头

                    var n = para.Runs[i + 1].Text.Substring(0, 1);
                    para.Runs[i + 1].Text = para.Runs[i + 1].Text.Substring(1);

                    var run2 = new Run(para.Document);
                    run2.Font.Superscript = true;
                    run2.Text = n;
                    para.Runs.Insert(i + 1, run2);

                    i += 2;

                    #endregion

                }
                else
                {
                    i++;
                }
            }
        }

        /// <summary>
        /// 数字转中文
        /// </summary>
        /// <param name="number">eg: 22</param>
        /// <returns></returns>
        private string numberToChinese(int number)
        {
            string res = string.Empty;
            string str = number.ToString();
            string schar = str.Substring(0, 1);
            switch (schar)
            {
                case "1":
                    res = "一";
                    break;
                case "2":
                    res = "二";
                    break;
                case "3":
                    res = "三";
                    break;
                case "4":
                    res = "四";
                    break;
                case "5":
                    res = "五";
                    break;
                case "6":
                    res = "六";
                    break;
                case "7":
                    res = "七";
                    break;
                case "8":
                    res = "八";
                    break;
                case "9":
                    res = "九";
                    break;
                default:
                    res = "零";
                    break;
            }
            if (str.Length > 1)
            {
                switch (str.Length)
                {
                    case 2:
                    case 6:
                        res += "十";
                        break;
                    case 3:
                    case 7:
                        res += "百";
                        break;
                    case 4:
                        res += "千";
                        break;
                    case 5:
                        res += "万";
                        break;
                    default:
                        res += "";
                        break;
                }
                res += numberToChinese(int.Parse(str.Substring(1, str.Length - 1)));
            }
            return res;
        }
    }
}
