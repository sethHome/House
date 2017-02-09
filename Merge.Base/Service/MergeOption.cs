using Aspose.Words;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merge.Base.Service
{
    public class MergeOption
    {
        /// <summary>
        /// 一级标题样式
        /// </summary>
        public FontStyle Head1Style { get; set; }

        /// <summary>
        /// 二级标题样式
        /// </summary>
        public FontStyle Head2Style { get; set; }

        /// <summary>
        /// 正文样式
        /// </summary>
        public FontStyle ContentStyle { get; set; }

        /// <summary>
        /// 目录样式
        /// </summary>
        public FontStyle ListStyle { get; set; }

        /// <summary>
        /// 页码样式
        /// </summary>
        public FontStyle PageNumberStyle { get; set; }

        /// <summary>
        /// 需要另起一页的章节
        /// </summary>
        public List<string> NewPageChapters { get; set; }

        

        /// <summary>
        /// 标题序号格式
        /// </summary>
        public HeadNumber HeadNumber { get; set; }
    }

    public class FontStyle
    {
        /// <summary>
        /// 字体
        /// </summary>
        public FontFamily FontFamily { get; set; }
        /// <summary>
        /// 字体大小
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// 是否加粗
        /// </summary>
        public bool Bold { get; set; }
        /// <summary>
        /// 是否斜体
        /// </summary>
        public bool Italic { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        public ParagraphAlignment Align { get; set; }

        /// <summary>
        /// 章节序号标题之间的空格数量
        /// </summary>
        public int ChapterSpaceCount { get; set; }
    }

    public enum HeadNumber
    {
        Chinese = 1,
        Number = 2,
    }

    public enum FontFamily
    {
        宋体 = 1,
        华文仿宋 = 2,
        楷体 = 3,
        黑体 = 4,
    }
}
