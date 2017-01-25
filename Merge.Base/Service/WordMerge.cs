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
    public class WordMerge
    {
        /// <summary>
        /// 设置页脚的页数
        /// </summary>
        /// <param name="doc"></param>
        public static void SetPageNumber(Document doc)
        {
            Aspose.Words.DocumentBuilder builder = new DocumentBuilder(doc);

            // Go to the primary footer
            builder.MoveToHeaderFooter(HeaderFooterType.FooterPrimary);
            builder.CurrentParagraph.RemoveAllChildren();

            builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;

            builder.PageSetup.PageStartingNumber = 1;   // 开始页码
            builder.PageSetup.RestartPageNumbering = true;  //       重新开始编号
            builder.PageSetup.DifferentFirstPageHeaderFooter = false;   // 奇偶页不同

            // 版式：页眉1.5cm，页脚：2cm
            builder.PageSetup.HeaderDistance = ConvertUtil.MillimeterToPoint(1.5 * 10);
            builder.PageSetup.FooterDistance = ConvertUtil.MillimeterToPoint(2 * 10);
            builder.PageSetup.PageNumberStyle = NumberStyle.Arabic;

            builder.Font.Name = "宋体";
            builder.Font.Bold = false;
            builder.Font.Size = 12;

            builder.Write("-");
            builder.InsertField("PAGE", "");
            builder.Write("-");

            // 设置正文样式 去除页眉横线
            builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
            builder.CurrentParagraph.RemoveAllChildren();
            builder.ParagraphFormat.ClearFormatting();
            //builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Normal;
        }

        // 页面设置
        public static void SetPage(Document doc)
        {
            foreach (Section section in doc)
            {
                // Millimeter 毫米
                // 页边距：上下2.5cm，左右2.3cm
                section.PageSetup.TopMargin = ConvertUtil.MillimeterToPoint(2.5 * 10);
                section.PageSetup.BottomMargin = ConvertUtil.MillimeterToPoint(2.5 * 10);
                section.PageSetup.LeftMargin = ConvertUtil.MillimeterToPoint(2.3 * 10);
                section.PageSetup.RightMargin = ConvertUtil.MillimeterToPoint(2.3 * 10);

                // 纸张：A4
                section.PageSetup.PaperSize = PaperSize.A4;

                // 方向：纵向
                section.PageSetup.Orientation = Orientation.Portrait;

                // 版式：页眉1.5cm，页脚：2cm
                section.PageSetup.HeaderDistance = ConvertUtil.MillimeterToPoint(1.5 * 10);
                section.PageSetup.FooterDistance = ConvertUtil.MillimeterToPoint(2 * 10);

                // 文档网格：每行33个字符，每页30行
            }
        }

        public static int GetDocIndex(Document doc)
        {
            var nodes = doc.GetChildNodes(NodeType.Paragraph, true);
            Regex regex1 = new Regex(@"第.+章");
            foreach (Paragraph para in nodes)
            {
                if (para.ParagraphFormat.StyleIdentifier == StyleIdentifier.Heading1 && regex1.IsMatch(para.Range.Text))
                {
                    var index = regex1.Match(para.Range.Text).Value.Replace("第", "").Replace("章", "");

                    return int.Parse(index);
                }
            }

            return -1;
        }

        public static List<string> CheckParagraphIndex(Document doc, MergeOption Options)
        {
            var paragraphs = doc.GetChildNodes(NodeType.Paragraph, true);

            Regex regex1 = new Regex(@"第[0-9]+?章");
            Regex regex2 = new Regex(@"^[0-9]\d*\.[0-9]\d*");
            Regex regex3 = new Regex(@"^[0-9]\d*\.[0-9]\d*\.[0-9]\d*");
            Regex regex4 = new Regex(@"^[0-9]\d*\.[0-9]\d*\.[0-9]\d*\.[0-9]\d*");
            Regex regex5 = new Regex(@"^[0-9]\d*）");
            Regex regex6 = new Regex(@"^（[0-9]\d*）");

            int index1 = 1, index2 = 1, index3 = 1, index4 = 1, index5 = 1, index6 = 1;

            var checkInfos = new List<string>();

            foreach (Paragraph para in paragraphs)
            {
                if (para.ParentNode.NodeType != NodeType.Body)
                {
                    continue;
                }

                // 是否需要章节序号检查
                if (!Options.ParagraphIndex)
                {
                    continue;
                }

                var m1 = regex1.Match(para.Range.Text.TrimStart());
                var m2 = regex2.Match(para.Range.Text.TrimStart());
                var m3 = regex3.Match(para.Range.Text.TrimStart());
                var m4 = regex4.Match(para.Range.Text.TrimStart());
                //var m5 = regex5.Match(para.Range.Text.TrimStart()); //(取消注释启用)
                //var m6 = regex6.Match(para.Range.Text.TrimStart()); //(取消注释启用)

                #region 一级标题
                if (para.ParagraphFormat.StyleIdentifier == StyleIdentifier.Heading1 && m1.Success)
                {
                    para.ParagraphFormat.Alignment = ParagraphAlignment.Center;

                    var index = m1.Value.Replace("第", "").Replace("章", "");

                    if (int.Parse(index) != index1)
                    {
                        checkInfos.Add(string.Format("一级标题序号错误,标题：{0}", para.Range.Text));
                        index1 = int.Parse(index) + 1;
                    }
                    else
                    {
                        para.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                        para.ParagraphFormat.LineSpacingRule = LineSpacingRule.Exactly; // 行间距 固定值
                        para.ParagraphFormat.LineSpacing = 23;      // 行间距 23磅

                        // 标题序号正确，则一级标题空三个字符
                        var newPage = para.Range.Text.StartsWith("\f");

                        var leftPart = regex1.Replace(para.Range.Text.Trim(new char[] { '\'', '\"', '\\', '\0', '\a', '\b', '\f', '\n', '\r', '\t', '\v' }), "").Trim();

                        para.Runs.Clear();

                        if (newPage)
                        {
                            para.ParagraphFormat.PageBreakBefore = true;
                        }

                        var r = new Run(doc)
                        {
                            Text = string.Format("{0}   {1}", m1.Value, leftPart)
                        };

                        r.Font.Name = "宋体";
                        r.Font.Size = 14;
                        r.Font.Bold = false;

                        para.Runs.Add(r);

                        index1++;
                    }

                    index2 = index3 = index4 = index5 = index6 = 1;

                    // 设置段落
                    //SetParaStyle(para, Options);
                }

                #endregion

                #region 二级标题
                else if (para.ParagraphFormat.StyleIdentifier == StyleIdentifier.Heading2 && m2.Success)
                {
                    var index = m2.Value.Split('.')[1];

                    if (int.Parse(index) != index2)
                    {
                        checkInfos.Add(string.Format("二级标题序号错误,标题：{0}", para.Range.Text));
                        index2 = int.Parse(index) + 1;

                    }
                    else
                    {
                        para.ParagraphFormat.Alignment = ParagraphAlignment.Left;
                        para.ParagraphFormat.LineSpacingRule = LineSpacingRule.Exactly; // 行间距 固定值
                        para.ParagraphFormat.LineSpacing = 23;      // 行间距 23磅

                        if (para.ParagraphFormat.LeftIndent == 0)
                        {
                            para.ParagraphFormat.FirstLineIndent = ConvertUtil.MillimeterToPoint(10);   // 每个段落首行缩进2个字符 = 1厘米
                        }

                        // 标题序号正确，则二级标题空两个字符
                        var newPage = para.Range.Text.StartsWith("\f");

                        var leftPart = regex2.Replace(para.Range.Text.Trim(new char[] { '\'', '\"', '\\', '\0', '\a', '\b', '\f', '\n', '\r', '\t', '\v' }), "").Trim();

                        para.Runs.Clear();

                        if (newPage)
                        {
                            para.ParagraphFormat.PageBreakBefore = true;
                        }

                        var r = new Run(doc)
                        {
                            Text = string.Format("{0}  {1}", m2.Value, leftPart)
                        };

                        r.Font.Name = "宋体";
                        r.Font.Size = 14;
                        r.Font.Bold = false;

                        para.Runs.Add(r);

                        index2++;
                    }

                    index3 = index4 = index5 = index6 = 1;
                }
                #endregion

                #region 四级标题
                else if (m4.Success && para.Range.Text.TrimStart().StartsWith(m4.Value))
                {
                    var index = m4.Value.Split('.')[3];

                    if (int.Parse(index) != index4)
                    {
                        checkInfos.Add(string.Format("四级标题序号错误,标题：{0}", para.Range.Text));

                        index4 = int.Parse(index) + 1;
                    }
                    else
                    {
                        index4++;
                    }

                    index5 = index6 = 1;

                    // 设置段落
                    SetParaStyle(para, Options);
                }
                #endregion

                #region 三级标题
                else if (m3.Success && para.Range.Text.TrimStart().StartsWith(m3.Value))
                {
                    var index = m3.Value.Split('.')[2];

                    if (int.Parse(index) != index3)
                    {
                        checkInfos.Add(string.Format("三级标题序号错误,标题：{0}", para.Range.Text));

                        index3 = int.Parse(index) + 1;

                    }
                    else
                    {
                        index3++;
                    }

                    index4 = index5 = index6 = 1;

                    // 设置段落
                    SetParaStyle(para, Options);
                }
                #endregion

                #region 五级标题(取消注释启用)
                //if (m5.Success && para.Range.Text.TrimStart().StartsWith(m5.Value))
                //{
                //    var index = int.Parse(m5.Value.Substring(0, m5.Value.Length - 1));

                //    if (index != index5)
                //    {
                //        Console.WriteLine("五级标题 序号 错误 ---" + para.Range.Text);
                //        index5 = index + 1;
                //    }
                //    else
                //    {
                //        index5++;
                //    }

                //    index6 = 1;

                //}
                #endregion

                #region 六级标题(取消注释启用)
                //if (m6.Success && para.Range.Text.TrimStart().StartsWith(m6.Value))
                //{
                //    var index = int.Parse(m6.Value.Substring(1, m6.Value.Length - 2));

                //    if (index != index6)
                //    {
                //        Console.WriteLine("六级标题 序号 错误 ---" + para.Range.Text);
                //        index6 = index + 1;
                //    }
                //    else
                //    {
                //        index6++;
                //    }


                //}
                #endregion

                #region 正文
                else
                {
                    // 设置段落
                    SetParaStyle(para, Options);
                }
                #endregion

            }

            return checkInfos;
        }

        private static Dictionary<string, string> _UnitMaps = new Dictionary<string, string>()
        {
            { "KV","kV"}, {"KM","km" }, {"KG","kg" }, {"KVAR","kvar" }, {"KW","kW" }, {"KPA","kPa" }, {"MPA","MPa" }
        };

        // 设置段落格式，文字样式，标点符号、单位替换
        public static void SetParaStyle(Paragraph para, MergeOption Options)
        {
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
            if (Options.UnitCheck)
            {
                if (text.Contains("m2") || text.Contains("m3"))
                {
                    // 单位上标
                    superscript(para);
                }

                foreach (var item in _UnitMaps)
                {
                    text = Strings.Replace(text, item.Key, item.Value, Compare: CompareMethod.Text);
                }
            }

            // 设置段落格式 并且段落内没有表格和图片
            if (Options.ParagraphStyle && para.GetChildNodes(NodeType.Shape, false).Count == 0 && para.GetChildNodes(NodeType.Table, false).Count == 0)
            {
                var regTitleTable = new Regex(@"^表[1-9]\d*");
                var regTitleImage = new Regex(@"^图[1-9]\d*");

                if ((regTitleTable.IsMatch(para.Range.Text) || regTitleImage.IsMatch(para.Range.Text)) &&
                    (IsImageOrTable(para.NextSibling) || IsImageOrTable(para.PreviousSibling)))
                {
                    // 表格或者图片的标题不设置首行缩进
                }
                else if (para.ParagraphFormat.Alignment != ParagraphAlignment.Center)
                {
                    // 居中不首行缩进

                    para.ParagraphFormat.FirstLineIndent = ConvertUtil.MillimeterToPoint(10);   // 每个段落首行缩进2个字符 = 1厘米
                    para.ParagraphFormat.LeftIndent = ConvertUtil.MillimeterToPoint(0);        // 缩进
                }

                para.ParagraphFormat.LineSpacingRule = LineSpacingRule.Exactly; // 行间距 固定值
                para.ParagraphFormat.LineSpacing = 23;      // 行间距 23磅
                //para.ParagraphFormat.StyleIdentifier = StyleIdentifier.Normal; // 正文
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

                if (Options.ParagraphStyle && !isFieldCode)
                {
                    para.Runs[i].Font.Name = "宋体";
                    para.Runs[i].Font.Size = 14;
                    para.Runs[i].Font.Bold = false;
                }

                // 标点符号英转中
                if (Options.PunctuationCheck && !isFieldCode)
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

        private static bool IsImageOrTable(Node node)
        {
            if (node.NodeType == NodeType.Shape || node.NodeType == NodeType.Table)
            {
                return true;
            }

            if (node.NodeType == NodeType.Paragraph)
            {
                var para = node as Paragraph;

                var imgs = para.GetChildNodes(NodeType.Shape, true);
                if (imgs.Count > 0)
                {
                    return true;
                }

                var tables = para.GetChildNodes(NodeType.Table, true);

                if (tables.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        // 设置段落格式，文字样式，标点符号、单位替换
        public static void SetTableParaStyle(Paragraph para, MergeOption Options, bool setLineSpacin = true)
        {
            if (para.Runs.Count == 0)
            {
                return;
            }

            var text = para.Range.Text;

            if (string.IsNullOrEmpty(text.Trim(new char[] { '\'', '\"', '\\', '\0', '\a', '\b', '\f', '\n', '\r', '\t', '\v' })))
            {
                return;
            }

            // 单位检查
            if (Options.UnitCheck)
            {
                if (text.Contains("m2") || text.Contains("m3"))
                {
                    // 单位上标
                    superscript(para);
                }

                foreach (var item in _UnitMaps)
                {
                    text = Strings.Replace(text, item.Key, item.Value, Compare: CompareMethod.Text);
                }
            }

            // 设置段落格式
            if (Options.ParagraphStyle && para.GetChildNodes(NodeType.Shape, false).Count == 0 && para.GetChildNodes(NodeType.Table, false).Count == 0)
            {
                para.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                para.ParagraphFormat.LeftIndent = 0;        // 缩进

                // 段落内都是文字才设置行距
                if (setLineSpacin && para.Runs.Count == para.ChildNodes.Count)
                {
                    para.ParagraphFormat.LineSpacingRule = LineSpacingRule.Exactly; // 行间距 固定值
                    para.ParagraphFormat.LineSpacing = 23;      // 行间距 23磅
                }
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

                if (Options.ParagraphStyle && !isFieldCode)
                {
                    para.Runs[i].Font.Name = "宋体";
                    para.Runs[i].Font.Size = 12;
                    para.Runs[i].Font.Bold = false;
                }

                // 标点符号英转中
                if (Options.PunctuationCheck && !isFieldCode)
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
        }

        // 设置附件段落格式
        public static void SetAttachParaStyle(Paragraph para, MergeOption Options)
        {

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
            if (Options.UnitCheck)
            {
                if (text.Contains("m2") || text.Contains("m3"))
                {
                    // 单位上标
                    superscript(para);
                }

                foreach (var item in _UnitMaps)
                {
                    text = Strings.Replace(text, item.Key, item.Value, Compare: CompareMethod.Text);
                }
            }


            // 设置段落格式 有图形的段落不设置行距
            if (Options.ParagraphStyle && para.GetChildNodes(NodeType.Shape, false).Count == 0 && para.GetChildNodes(NodeType.Table, false).Count == 0)
            {
                var regTitleTable = new Regex(@"^表[1-9]\d*");
                var regTitleImage = new Regex(@"^图[1-9]\d*");

                if ((regTitleTable.IsMatch(para.Range.Text) || regTitleImage.IsMatch(para.Range.Text)) &&
                    (IsImageOrTable(para.NextSibling) || IsImageOrTable(para.PreviousSibling)))
                {
                    // 表格或者图片的标题不设置首行缩进
                }
                else
                {
                    para.ParagraphFormat.FirstLineIndent = ConvertUtil.MillimeterToPoint(10);   // 每个段落首行缩进2个字符 = 1厘米
                    para.ParagraphFormat.LeftIndent = 0;        // 缩进
                }

                para.ParagraphFormat.LineSpacingRule = LineSpacingRule.Exactly; // 行间距 固定值
                para.ParagraphFormat.LineSpacing = 23;      // 行间距 23磅
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

                if (Options.ParagraphStyle && !isFieldCode)
                {
                    para.Runs[i].Font.Name = "宋体";
                    para.Runs[i].Font.Size = 14;
                    para.Runs[i].Font.Bold = false;
                }

                // 标点符号英转中
                if (Options.PunctuationCheck && !isFieldCode)
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

        private static void superscript(Paragraph para)
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

        // 表格设置 表格中字体原则上设置为宋体 小四。表格内边框0.5磅，外边框1.5磅
        public static void SetTable(Document doc, MergeOption Options)
        {
            var tables = doc.GetChildNodes(NodeType.Table, true);
            SetTable(tables, Options);
        }

        public static void SetTable(NodeCollection tables, MergeOption Options)
        {
            var regTitle = new Regex(@"^表[1-9]\d*");

            foreach (Table table in tables)
            {
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
                        // 设置表格内部段落样式,表格的行高是固定值不设置行距
                        SetTableParaStyle(p, Options, c.ParentRow.RowFormat.HeightRule != HeightRule.Exactly);
                    }
                }

                // 外边框1.5磅
                table.SetBorder(BorderType.Top, LineStyle.Single, 1.5, System.Drawing.Color.Black, true);
                table.SetBorder(BorderType.Bottom, LineStyle.Single, 1.5, System.Drawing.Color.Black, true);
                table.SetBorder(BorderType.Left, LineStyle.Single, 1.5, System.Drawing.Color.Black, true);
                table.SetBorder(BorderType.Right, LineStyle.Single, 1.5, System.Drawing.Color.Black, true);
            }
        }

        // 图片设置 要居页面之中，文字环绕选择嵌入型
        public static void SetImage(Document doc)
        {
            var images = doc.GetChildNodes(NodeType.Shape, true);
            SetImage(images);
        }

        public static void SetImage(NodeCollection images)
        {
            var regTitle = new Regex(@"^图[1-9]\d*");

            foreach (Aspose.Words.Drawing.Shape img in images)
            {
                if (img.IsImage)
                {
                    var paraTitleHead = img.ParentParagraph.PreviousSibling as Paragraph; // 图上标题
                    var paraTitleFoot = img.ParentParagraph.NextSibling as Paragraph;   // 图下标题

                    if (paraTitleHead != null && regTitle.IsMatch(paraTitleHead.Range.Text))
                    {
                        // 以图1开头的段落认为是图片的标题
                        paraTitleHead.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                    }
                    if (paraTitleFoot != null && regTitle.IsMatch(paraTitleFoot.Range.Text))
                    {
                        // 以图1开头的段落认为是图片的标题
                        paraTitleFoot.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                    }

                    // 图片所在段落居中
                    img.ParentParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                    
                    // 文字环绕 嵌入型
                    img.WrapType = Aspose.Words.Drawing.WrapType.Inline;

                    // 图片居中
                    img.HorizontalAlignment = Aspose.Words.Drawing.HorizontalAlignment.Center;
                    // 图片宽度100%
                    //img.Width = ConvertUtil.PointToInch(100);
                }
            }
        }

        // 生成目录
        public static void CreateList(Document doc)
        {
            Section sc = new Section(doc);
            doc.Sections.Insert(0, sc);

            sc.PageSetup.PaperSize = PaperSize.A4;
            sc.PageSetup.TopMargin = ConvertUtil.MillimeterToPoint(2.5 * 10);
            sc.PageSetup.BottomMargin = ConvertUtil.MillimeterToPoint(2.5 * 10);
            sc.PageSetup.LeftMargin = ConvertUtil.MillimeterToPoint(2.3 * 10);
            sc.PageSetup.RightMargin = ConvertUtil.MillimeterToPoint(2.3 * 10);

            // Create a document builder to insert content with into document.
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveToSection(0);

            //builder.PageSetup.DifferentFirstPageHeaderFooter = true;

            doc.FirstSection.Body.PrependChild(new Paragraph(doc));
            // Move DocumentBuilder cursor to the beginning.
            builder.MoveToDocumentStart();

            builder.CurrentParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;

            builder.Writeln("目录");

            // Insert a table of contents at the beginning of the document.
            builder.InsertTableOfContents("\\o \"1-3\" \\h \\z \\u");
            // Start the actual document content on the second page.

            //builder.InsertBreak(BreakType.SectionBreakNewPage);
        }

        // 设置目录格式
        public static void SetListStyle(Document doc)
        {
            var ps = doc.FirstSection.GetChildNodes(NodeType.Paragraph, true);

            Regex regex2 = new Regex(@"[0-9]\d*\.[0-9]\d*");

            foreach (Paragraph para in ps)
            {
                if (para.Range.Text != "目录\r")
                {
                    para.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                    para.ParagraphFormat.LineSpacingRule = LineSpacingRule.Exactly; // 行间距 固定值
                    para.ParagraphFormat.LineSpacing = 23;      // 行间距 23磅


                    if (regex2.IsMatch(para.Range.Text))
                    {
                        para.ParagraphFormat.LeftIndent = ConvertUtil.MillimeterToPoint(10);   // 每个段落首行缩进2个字符 = 1厘米
                    }

                    for (int i = 0; i < para.Runs.Count; i++)
                    {
                        para.Runs[i].Font.Name = "宋体";
                        para.Runs[i].Font.Size = 14;
                        para.Runs[i].Font.Bold = false;
                    }
                }
            }
        }

        public static void AppendAttachList(Document docSource, Document docAttachList, MergeOption Options)
        {
            Section sc = new Section(docSource);
            sc.PageSetup.PaperSize = PaperSize.A4;
            sc.PageSetup.TopMargin = ConvertUtil.MillimeterToPoint(2.5 * 10);
            sc.PageSetup.BottomMargin = ConvertUtil.MillimeterToPoint(2.5 * 10);
            sc.PageSetup.LeftMargin = ConvertUtil.MillimeterToPoint(2.3 * 10);
            sc.PageSetup.RightMargin = ConvertUtil.MillimeterToPoint(2.3 * 10);

            sc.AppendContent(docAttachList.FirstSection);

            var ps = sc.GetChildNodes(NodeType.Paragraph, true);

            foreach (Paragraph p in ps)
            {
                if (p.ParentNode.NodeType != NodeType.Body)
                {
                    continue;
                }

                if (p.Range.Text == "附件\r")
                {
                    continue;
                }

                WordMerge.SetParaStyle(p, Options);
            }

            // 表格
            //if (Options.Table)
            //{
            //    var tables = sc.GetChildNodes(NodeType.Table, true);

            //    WordMerge.SetTable(tables, Options);
            //}

            // 图片
            if (Options.Picture)
            {
                var images = sc.GetChildNodes(NodeType.Shape, true);
                WordMerge.SetImage(images);
            }

            docSource.Sections.Insert(0, sc);
        }

        public static void SetDocStyle(Document doc)
        {
            // 一级标题宋体4号居中
            var head1 = doc.Styles[StyleIdentifier.Heading1];
            head1.Font.Name = "宋体";
            head1.Font.Size = 14;
            head1.Font.Bold = false;
            head1.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            head1.ParagraphFormat.LeftIndent = ConvertUtil.MillimeterToPoint(0);

            // 二级标题宋体4号 首行缩进
            var head2 = doc.Styles[StyleIdentifier.Heading2];
            head2.Font.Name = "宋体";
            head2.Font.Size = 14;
            head2.Font.Bold = false;
            head2.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            head2.ParagraphFormat.FirstLineIndent = ConvertUtil.MillimeterToPoint(10);
            head2.ParagraphFormat.LeftIndent = ConvertUtil.MillimeterToPoint(0);

            // 正文宋体4号
            var contextNormal = doc.Styles[StyleIdentifier.Normal];
            contextNormal.Font.Name = "宋体";
            contextNormal.Font.Size = 14;
        }
    }
}
