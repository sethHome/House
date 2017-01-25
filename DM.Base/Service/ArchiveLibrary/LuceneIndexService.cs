using Api.Framework.Core.Attach;
using DM.Base.Permission;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public class LuceneIndexService : ILuceneIndexService
    {
        [Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        [Dependency]
        public IDMPermissionCheck _IDMPermissionCheck { get; set; }

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="Datas"></param>
        public void CreateIndexByData(string ID, int AccessLevel, Dictionary<string, string> Datas)
        {
            //索引文档保存位置    
            var indexPath = System.Configuration.ConfigurationManager.AppSettings["IndexArchivePath"];

            using (FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NativeFSLockFactory()))
            {
                //IndexReader:对索引库进行读取的类
                bool isExist = IndexReader.IndexExists(directory); //是否存在索引库文件夹以及索引库特征文件
                if (isExist)
                {
                    //如果索引目录被锁定（比如索引过程中程序异常退出或另一进程在操作索引库），则解锁
                    //Q:存在问题 如果一个用户正在对索引库写操作 此时是上锁的 而另一个用户过来操作时 将锁解开了 于是产生冲突 --解决方法后续
                    if (IndexWriter.IsLocked(directory))
                    {
                        IndexWriter.Unlock(directory);
                    }
                }

                //创建向索引库写操作对象  IndexWriter(索引目录,指定使用盘古分词进行切词,最大写入长度限制)
                //补充:使用IndexWriter打开directory时会自动对索引库文件上锁
                using (IndexWriter writer = new IndexWriter(directory, new PanGuAnalyzer(), !isExist, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    //--------------------------------遍历数据源 将数据转换成为文档对象 存入索引库
                    Document document = new Document(); //new一篇文档对象 --一条记录对应索引库中的一个文档

                    //向文档中添加字段  Add(字段,值,是否保存字段原始值,是否针对该列创建索引)
                    document.Add(new Field("id", ID, Field.Store.YES, Field.Index.NOT_ANALYZED));//--所有字段的值都将以字符串类型保存 因为索引库只存储字符串类型数据

                    // 访问级别字段
                    var numberField = new NumericField("AccessLevel", Field.Store.YES, true);
                    numberField.SetIntValue(AccessLevel);
                    document.Add(numberField);

                    //Field.Store:表示是否保存字段原值。指定Field.Store.YES的字段在检索时才能用document.Get取出原值  
                    //Field.Index.NOT_ANALYZED:指定不按照分词后的结果保存--是否按分词后结果保存取决于是否对该列内容进行模糊查询
                    //WITH_POSITIONS_OFFSETS:指示不仅保存分割后的词 还保存词之间的距离
                    foreach (KeyValuePair<string, string> item in Datas)
                    {
                        document.Add(new Field(item.Key, item.Value, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
                    }

                    writer.AddDocument(document); //文档写入索引库
                }
            }
        }

        public void CreateIndexByFile(int FileID,int AccessLevel, int AttachID, Dictionary<string, string> Datas)
        {
            var attachInfo = _IObjectAttachService.Get(AttachID);

            // 2 : word, 8 : txt
            if (attachInfo.Type == 2 || attachInfo.Type == 8)
            {
                //索引文档保存位置    
                var indexPath = System.Configuration.ConfigurationManager.AppSettings["IndexFilePath"];

                using (FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NativeFSLockFactory()))
                {
                    //IndexReader:对索引库进行读取的类
                    bool isExist = IndexReader.IndexExists(directory); //是否存在索引库文件夹以及索引库特征文件
                    if (isExist)
                    {
                        //如果索引目录被锁定（比如索引过程中程序异常退出或另一进程在操作索引库），则解锁
                        //Q:存在问题 如果一个用户正在对索引库写操作 此时是上锁的 而另一个用户过来操作时 将锁解开了 于是产生冲突 --解决方法后续
                        if (IndexWriter.IsLocked(directory))
                        {
                            IndexWriter.Unlock(directory);
                        }
                    }

                    //创建向索引库写操作对象  IndexWriter(索引目录,指定使用盘古分词进行切词,最大写入长度限制)
                    //补充:使用IndexWriter打开directory时会自动对索引库文件上锁
                    using (IndexWriter writer = new IndexWriter(directory, new PanGuAnalyzer(), !isExist, IndexWriter.MaxFieldLength.UNLIMITED))
                    {
                        //--------------------------------遍历数据源 将数据转换成为文档对象 存入索引库
                        Document document = new Document(); //new一篇文档对象 --一条记录对应索引库中的一个文档

                        //向文档中添加字段  Add(字段,值,是否保存字段原始值,是否针对该列创建索引)
                        document.Add(new Field("id", FileID.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));//--所有字段的值都将以字符串类型保存 因为索引库只存储字符串类型数据

                        // 访问级别字段
                        var numberField = new NumericField("AccessLevel", Field.Store.YES, true);
                        numberField.SetIntValue(AccessLevel);
                        document.Add(numberField);

                        foreach (KeyValuePair<string, string> item in Datas)
                        {
                            document.Add(new Field(item.Key, item.Value, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
                        }

                        //Field.Store:表示是否保存字段原值。指定Field.Store.YES的字段在检索时才能用document.Get取出原值  
                        //Field.Index.NOT_ANALYZED:指定不按照分词后的结果保存--是否按分词后结果保存取决于是否对该列内容进行模糊查询
                        //WITH_POSITIONS_OFFSETS:指示不仅保存分割后的词 还保存词之间的距离

                        if (attachInfo.Type == 8)
                        {
                            // txt
                            var reader = File.OpenText(attachInfo.Path);
                             reader.ReadToEnd();

                            using (System.IO.StreamReader sr = new System.IO.StreamReader(attachInfo.Path, Encoding.Default ))
                            {
                                var txt = sr.ReadToEnd();
                                document.Add(new Field("content", txt, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
                            }

                        }
                        else
                        {
                            // word
                            var wordFile = new Aspose.Words.Document(attachInfo.Path);
                            var txt = wordFile.GetText();
                            document.Add(new Field("content", txt, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
                        }
                       
                        writer.AddDocument(document); //文档写入索引库
                    }
                }
            }
        }

        /// <summary>
        /// 从索引库中检索关键字
        /// </summary>
        public List<SearchData> SearchFromArchiveIndexData(List<string> Fields, string SearchKey,int UserID)
        {
            //索引文档保存位置    
            var indexPath = System.Configuration.ConfigurationManager.AppSettings["IndexArchivePath"];

            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NoLockFactory());
            IndexReader reader = IndexReader.Open(directory, true);
            IndexSearcher searcher = new IndexSearcher(reader);
            //搜索条件

            var datas = Fields.Select(f => SearchKey);

            var query = MultiFieldQueryParser.Parse(Lucene.Net.Util.Version.LUCENE_30, datas.ToArray(), Fields.ToArray(), new PanGuAnalyzer());

            // 访问级别
            var query2 = NumericRangeQuery.NewIntRange("AccessLevel", 1, _IDMPermissionCheck.GetUserAccessLevel(UserID), true, true);

            BooleanQuery booleanQuery = new BooleanQuery();
            booleanQuery.Add(query, Occur.MUST);
            booleanQuery.Add(query2, Occur.MUST);

            //TopScoreDocCollector盛放查询结果的容器
            TopScoreDocCollector collector = TopScoreDocCollector.Create(1000, true);
            searcher.Search(booleanQuery, null, collector);//根据query查询条件进行查询，查询结果放入collector容器

            //TopDocs 指定0到GetTotalHits() 即所有查询结果中的文档 如果TopDocs(20,10)则意味着获取第20-30之间文档内容 达到分页的效果
            ScoreDoc[] docs = collector.TopDocs(0, collector.TotalHits).ScoreDocs;
            
            //展示数据实体对象集合
            var searchDatas = new List<SearchData>();

            for (int i = 0; i < docs.Length; i++)
            {
                int docId = docs[i].Doc;//得到查询结果文档的id（Lucene内部分配的id）
                Document doc = searcher.Doc(docId);//根据文档id来获得文档对象Document

                var result = new SearchData();
                result.Datas = new Dictionary<string, string>();

                result.ID = Convert.ToInt32(doc.Get("id"));

                var docFields = doc.GetFields();

                foreach (Field f in docFields)
                {
                    result.Datas.Add(f.Name, StringSplitHelper.HightLight(SearchKey,f.StringValue));
                }

                searchDatas.Add(result);
            }

            return searchDatas;
        }

        /// <summary>
        /// 从索引库中检索关键字
        /// </summary>
        public List<SearchData> SearchFromFileIndexData(List<string> Fields, string SearchKey,int UserID)
        {
            //索引文档保存位置    
            var indexPath = System.Configuration.ConfigurationManager.AppSettings["IndexFilePath"];

            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NoLockFactory());
            IndexReader reader = IndexReader.Open(directory, true);
            IndexSearcher searcher = new IndexSearcher(reader);
            //搜索条件
            Fields.Insert(0, "content");

            var datas = Fields.Select(f => SearchKey);

            var query = MultiFieldQueryParser.Parse(Lucene.Net.Util.Version.LUCENE_30, datas.ToArray(), Fields.ToArray(), new PanGuAnalyzer());

            // 访问级别
            var query2 = NumericRangeQuery.NewIntRange("AccessLevel", 1, _IDMPermissionCheck.GetUserAccessLevel(UserID), true, true);

            BooleanQuery booleanQuery = new BooleanQuery();
            booleanQuery.Add(query, Occur.MUST);
            booleanQuery.Add(query2, Occur.MUST);

            //TopScoreDocCollector盛放查询结果的容器
            TopScoreDocCollector collector = TopScoreDocCollector.Create(1000, true);
            searcher.Search(booleanQuery, null, collector);//根据query查询条件进行查询，查询结果放入collector容器

            //TopDocs 指定0到GetTotalHits() 即所有查询结果中的文档 如果TopDocs(20,10)则意味着获取第20-30之间文档内容 达到分页的效果
            ScoreDoc[] docs = collector.TopDocs(0, collector.TotalHits).ScoreDocs;

            //展示数据实体对象集合

            var searchDatas = new List<SearchData>();

            for (int i = 0; i < docs.Length; i++)
            {
                int docId = docs[i].Doc;//得到查询结果文档的id（Lucene内部分配的id）
                Document doc = searcher.Doc(docId);//根据文档id来获得文档对象Document

                var result = new SearchData();
                result.Datas = new Dictionary<string, string>();

                result.ID = Convert.ToInt32(doc.Get("id"));

                var docFields = doc.GetFields();

                foreach (Field f in docFields)
                {
                    result.Datas.Add(f.Name, StringSplitHelper.HightLight(SearchKey, f.StringValue));
                }

                searchDatas.Add(result);
            }

            return searchDatas;
        }

    }
}
