using Lucene.Net.Analysis;
using Lucene.Net.Analysis.PanGu;
using PanGu;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
   public class StringSplitHelper
    {
        public static List<string> SplitWords(string content)
        {
            List<string> strList = new List<string>();
            using (Analyzer analyzer = new PanGuAnalyzer())//指定使用盘古 PanGuAnalyzer 分词算法
            {
                using (System.IO.StringReader reader = new System.IO.StringReader(content))
                {
                    Lucene.Net.Analysis.TokenStream ts = analyzer.TokenStream(content, reader);

                    while (ts.IncrementToken())
                    {
                        var ita = ts.GetAttribute<Lucene.Net.Analysis.Tokenattributes.ITermAttribute>();
                        strList.Add(ita.Term);
                    }
                    ts.CloneAttributes();
                }
            }
            
            return strList;
        }

        //需要添加PanGu.HighLight.dll的引用
        /// <summary>
        /// 搜索结果高亮显示
        /// </summary>
        /// <param name="keyword"> 关键字 </param>
        /// <param name="content"> 搜索结果 </param>
        /// <returns> 高亮后结果 </returns>
        public static string HightLight(string keyword, string content)
        {
            //创建HTMLFormatter,参数为高亮单词的前后缀
            PanGu.HighLight.SimpleHTMLFormatter simpleHTMLFormatter =
                new PanGu.HighLight.SimpleHTMLFormatter("<span class='hightlight'>", "</span>");
            //创建 Highlighter ，输入HTMLFormatter 和 盘古分词对象Semgent
            PanGu.HighLight.Highlighter highlighter =
                            new PanGu.HighLight.Highlighter(simpleHTMLFormatter,
                            new Segment());
            //设置每个摘要段的字符数
            highlighter.FragmentSize = 1000;
            //获取最匹配的摘要段
            var result = highlighter.GetBestFragment(keyword, content);

            if (string.IsNullOrEmpty(result))
            {
                return content;
            }

            return result;
        }
    }
}
