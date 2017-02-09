using Api.Framework.Core;
using Api.Framework.Core.Attach;
using Api.Framework.Core.BaseData;
using Api.Framework.Core.File;
using Api.Framework.Core.Safe;
using Aspose.Words;
using Aspose.Words.Lists;
using DM.Base.Entity;
using Merge.Base.Entitys;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Merge.Base.Service
{
    public class MergeTaskService : IMergeTaskService
    {
        private BaseRepository<ProjectSpecialtyEntity> _DB;
        private DBContext _DBContext;

        [Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        [Dependency]
        public IFileService _IFileService { get; set; }

        [Dependency]
        public IEnum _IEnum { get; set; }

        public MergeTaskService()
        {
            _DBContext = new DBContext();

            this._DB = new BaseRepository<ProjectSpecialtyEntity>(_DBContext);
        }

        public void TaskFinish(int ID, int AttachID)
        {
            var attachInfo = _IObjectAttachService.Get(AttachID);

            var task = _DB.Get(ID);
            task.IsFinish = true;
            task.IsMerge = true;
            task.LastUpdateDate = attachInfo.UploadDate;

            _DBContext.Entry(task).State = System.Data.Entity.EntityState.Modified;
            _DBContext.SaveChanges();
        }

        public MergeResult MergeDoc(int ID, int UserID, MergeOption Options)
        {
            var projInfo = _DBContext.ProjectEntity.Find(ID);

            MergeResult result = null;
            if (projInfo.Phase == 1)
            {
                result = MergeDoc_KY(ID, UserID, Options);
            }
            else
            {
                result = MergeDoc_CS(ID, UserID, Options);
            }

            // 更新每个专业的状态为已经合并过
            var specils = _DBContext.ProjectSpecialtyEntity.Where(s => s.ProjectID == ID);

            foreach (var s in specils)
            {
                s.IsMerge = false;
                _DBContext.Entry(s).State = System.Data.Entity.EntityState.Modified;
            }

            _DBContext.SaveChanges();

            return result;
        }


        private MergeResult MergeDoc_CS(int ID, int UserID, MergeOption Options)
        {
            var result = new MergeResult()
            {
                IsSuccess = true,
                ParaIndexCheck = new List<string>()
            };

            var projInfo = _DBContext.ProjectEntity.Find(ID);

            var areas = _IEnum.GetEnumInfo("System4", "region").Items;

            var areaName = areas.FirstOrDefault(a => a.Key == projInfo.Area.ToString()).Text;

            var otherAreaNames = areas.Where(a => a.Key != projInfo.Area.ToString()).Select(a => a.Text);

            var regAreas = string.Format("({0})", string.Join(")|(", otherAreaNames));

            var ids = _DB.GetQuery(c => c.ProjectID == ID).Select(c => c.ID);

            var files = _IObjectAttachService.GetAttachFiles("ProjectCover", ID);
            var cover = files.Count() > 0 ? files.First() : null;

            files = _IObjectAttachService.GetAttachFiles("ProjectAttachLs", ID);
            var attachLs = files.Count() > 0 ? files.First() : null;

            files = _IObjectAttachService.GetAttachFiles("ProjectAttach", ID);
            var attach = files.Count() > 0 ? files.First() : null;

            var attachs = _IObjectAttachService.GetAttachFiles("MergeProjectDoc", ids.ToList());

            if (attachs.Count == 0)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "项目没有专业文档，无法合并";
                return result;
            }

            var allNode = new Dictionary<string, List<DocNode>>();

            var _WordMergeService = new WordMergeService(Options);

            foreach (var a in attachs)
            {
                _WordMergeService.SetDocNode(new Document(a.Path), allNode);
            }

            // 创建新的空白文档
            var doc = new Document();
            _WordMergeService.SetDocStyle(doc);

            doc.RemoveAllChildren();
            var section = _WordMergeService.CreateSection(doc);
            doc.Sections.Add(section);

            // 按照章节号对段落排序
            var items = allNode.OrderBy(d => double.Parse(d.Key.Replace("_", "")));

            int index1 = 1, index2 = 0, index3 = 0, index4 = 0;

            // 将排序好的节点插入新的文档中
            foreach (var item in items)
            {
                foreach (var node in item.Value)
                {
                    Node dstNode = doc.ImportNode(node.Node, true, ImportFormatMode.UseDestinationStyles);

                    if (dstNode.NodeType == NodeType.Paragraph)
                    {
                        var para = dstNode as Paragraph;

                        #region 章节序号检查、重设样式

                        if (para.ParagraphFormat.StyleIdentifier == StyleIdentifier.Heading1)
                        {
                            // 清除原先格式，重新设置样式
                            para.ParagraphFormat.ClearFormatting();
                            para.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;

                            if (item.Key.Contains("_"))
                            {
                                result.ParaIndexCheck.Add(string.Format("一级标题序号重复,标题：{0}", para.Range.Text));
                            }
                            else if (int.Parse(item.Key) != index1)
                            {
                                result.ParaIndexCheck.Add(string.Format("一级标题序号错误,标题：{0}", para.Range.Text));
                                index1 = int.Parse(item.Key) + 1;
                            }
                            else
                            {
                                index1++;
                            }

                            index2 = index3 = index4 = 1;
                        }
                        else if (para.ParagraphFormat.StyleIdentifier == StyleIdentifier.Heading2)
                        {
                            // 清除原先格式，重新设置样式
                            para.ParagraphFormat.ClearFormatting();
                            para.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading2;

                            var index = item.Key.Split('.')[1];

                            if (item.Key.Contains("_"))
                            {
                                result.ParaIndexCheck.Add(string.Format("二级标题序号重复,标题：{0}", para.Range.Text));
                            }
                            else if (int.Parse(index) != index2)
                            {
                                result.ParaIndexCheck.Add(string.Format("二级标题序号错误,标题：{0}", para.Range.Text));
                                index2 = int.Parse(index) + 1;
                            }
                            else
                            {
                                index2++;
                            }

                            index3 = index4 = 1;
                        }
                        else
                        {
                            if (node.IsListNode)
                            {
                                para.ListFormat.ListLevel.StartAt = node.ListValue;
                                para.ListLabel.Font.Bold = false;
                                para.ListLabel.Font.Size = 14;
                            }
                            else
                            {
                                // 清除原先格式，重新设置样式
                                para.ParagraphFormat.ClearFormatting();
                                para.ParagraphFormat.StyleIdentifier = StyleIdentifier.Normal;

                                var text = para.Range.Text.TrimStart();

                                var m3 = _WordMergeService.ParaReg3.Match(text);
                                var m4 = _WordMergeService.ParaReg4.Match(text);

                                if (m4.Success)
                                {
                                    var index = m4.Value.Split('.')[3];

                                    if (int.Parse(index) != index4)
                                    {
                                        result.ParaIndexCheck.Add(string.Format("四级标题序号错误,标题：{0}", para.Range.Text));

                                        index4 = int.Parse(index) + 1;
                                    }
                                    else
                                    {
                                        index4++;
                                    }
                                }
                                else if (m3.Success)
                                {
                                    var index = m3.Value.Split('.')[2];

                                    if (int.Parse(index) != index3)
                                    {
                                        result.ParaIndexCheck.Add(string.Format("三级标题序号错误,标题：{0}", para.Range.Text));

                                        index3 = int.Parse(index) + 1;

                                    }
                                    else
                                    {
                                        index3++;
                                    }

                                    index4 = 1;
                                }
                            }
                        }

                        #endregion

                        _WordMergeService.SetChapter(item.Key.Replace("_", ""), para);

                        _WordMergeService.SetParaStyle(para);


                    }

                    section.Body.AppendChild(dstNode);
                }
            }

            if (attach != null)
            {
                _WordMergeService.AppendAttachDoc(doc, new Document(attach.Path));
            }

            // 地区检查
            _WordMergeService.CheckArea(doc, areaName, regAreas, result);

            // 禁用内容检查
            _WordMergeService.CheckDisableContent(doc, projInfo.DisableWord, result);

            _WordMergeService.SetTable(doc);

            _WordMergeService.SetImage(doc);

            _WordMergeService.SetPageNumber(doc);

            if (attachLs != null)
            {
                _WordMergeService.AppendDoc(doc, new Document(attachLs.Path));
            }

            _WordMergeService.CreateList(doc);

            var filePath = getFilePath();

            if (cover != null)
            {
                var docCover = new Document(cover.Path);
                docCover.AppendDocument(doc, ImportFormatMode.UseDestinationStyles);
                docCover.Save(filePath);
            }
            else
            {
                doc.Save(filePath);
            }

            FileStream fs = new FileStream(filePath, FileMode.Open);

            var md5Str = Md5.GetMd5Hash(fs);

            var newAttach = new SysAttachFileEntity()
            {
                Md5 = md5Str,
                Extension = ".docx",
                FileDate = DateTime.Now,
                Name = "合并文档.docx",
                Path = filePath,
                Size = fs.Length,
                Type = (int)EnumAttachType.World,
                UploadDate = DateTime.Now,
                UploadUser = UserID
            };

            fs.Close();

            _IFileService.Add(newAttach);

            _IObjectAttachService.Add(new ObjectAttachEntity()
            {
                AttachID = newAttach.ID,
                ObjectID = ID,
                ObjectKey = "MergeProjectDocResult"
            });
            result.Attach = newAttach;

            return result;
        }

        private MergeResult MergeDoc_KY(int ID, int UserID, MergeOption Options)
        {
            var result = new MergeResult()
            {
                IsSuccess = true,
                ParaIndexCheck = new List<string>()
            };

            var projInfo = _DBContext.ProjectEntity.Find(ID);

            var areas = _IEnum.GetEnumInfo("System4", "region").Items;

            var areaName = areas.FirstOrDefault(a => a.Key == projInfo.Area.ToString()).Text;

            var otherAreaNames = areas.Where(a => a.Key != projInfo.Area.ToString()).Select(a => a.Text);

            var regAreas = string.Format("({0})", string.Join(")|(", otherAreaNames));

            var ids = _DB.GetQuery(c => c.ProjectID == ID).Select(c => c.ID);

            var files = _IObjectAttachService.GetAttachFiles("ProjectCover", ID);
            var cover = files.Count() > 0 ? files.First() : null;

            files = _IObjectAttachService.GetAttachFiles("ProjectAttachLs", ID);
            var attachLs = files.Count() > 0 ? files.First() : null;

            files = _IObjectAttachService.GetAttachFiles("ProjectAttach", ID);
            var attach = files.Count() > 0 ? files.First() : null;

            var attachs = _IObjectAttachService.GetAttachFiles("MergeProjectDoc", ids.ToList());

            if (attachs.Count == 0)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "项目没有专业文档，无法合并";
                return result;
            }

            var _KYWordMergeService = new KYWordMergeService(Options);

            var allNode = new Dictionary<string, List<DocNode>>();

            foreach (var a in attachs)
            {
                _KYWordMergeService.SetDocNode(new Document(a.Path), allNode);
            }

            var doc = new Document();
            _KYWordMergeService.SetDocStyle(doc);

            doc.RemoveAllChildren();
            var section = _KYWordMergeService.CreateSection(doc);
            doc.Sections.Add(section);

            // 按照章节号对段落排序
            var items = allNode.OrderBy(d => getOrderIndex(d.Key));

            int index1 = 1, index2 = 0, index3 = 0, index4 = 0,index5 = 0;

            // 将排序好的节点插入新的文档中
            foreach (var item in items)
            {
                foreach (var node in item.Value)
                {
                    Node dstNode = doc.ImportNode(node.Node, true, ImportFormatMode.UseDestinationStyles);

                    if (dstNode.NodeType == NodeType.Paragraph)
                    {
                        var para = dstNode as Paragraph;

                        #region 章节序号检查、重设样式

                        if (para.ParagraphFormat.StyleIdentifier == StyleIdentifier.Heading1)
                        {
                            // 清除原先格式，重新设置样式
                            para.ParagraphFormat.ClearFormatting();
                            para.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;

                            if (item.Key.Contains("_"))
                            {
                                result.ParaIndexCheck.Add(string.Format("一级标题序号重复,标题：{0}", para.Range.Text));
                            }
                            else if (int.Parse(item.Key) != index1)
                            {
                                result.ParaIndexCheck.Add(string.Format("一级标题序号错误,标题：{0}", para.Range.Text));
                                index1 = int.Parse(item.Key) + 1;
                            }
                            else
                            {
                                index1++;
                            }

                            index2 = index3 = index4 = index5 = 1;
                        }
                        else if (para.ParagraphFormat.StyleIdentifier == StyleIdentifier.Heading2)
                        {
                            // 清除原先格式，重新设置样式
                            para.ParagraphFormat.ClearFormatting();
                            para.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading2;

                            var index = item.Key.Split('.')[1];

                            if (item.Key.Contains("_"))
                            {
                                result.ParaIndexCheck.Add(string.Format("二级标题序号重复,标题：{0}", para.Range.Text));
                            }
                            else if (int.Parse(index) != index2)
                            {
                                result.ParaIndexCheck.Add(string.Format("二级标题序号错误,标题：{0}", para.Range.Text));
                                index2 = int.Parse(index) + 1;
                            }
                            else
                            {
                                index2++;
                            }

                            index3 = index4 = index5 = 1;
                        }
                        else
                        {
                            if (node.IsListNode)
                            {
                                para.ListFormat.ListLevel.StartAt = node.ListValue;
                                para.ListLabel.Font.Bold = Options.ContentStyle.Bold;
                                para.ListLabel.Font.Size = Options.ContentStyle.Size;
                            }
                            else
                            {
                                // 清除原先格式，重新设置样式
                                para.ParagraphFormat.ClearFormatting();
                                para.ParagraphFormat.StyleIdentifier = StyleIdentifier.Normal;

                                var text = para.Range.Text.TrimStart();
                                var m3 = _KYWordMergeService.ParaReg3.Match(text);
                                var m4 = _KYWordMergeService.ParaReg4.Match(text);
                                var m5 = _KYWordMergeService.ParaReg5.Match(text);

                                if (m5.Success)
                                {
                                    para.ParagraphFormat.FirstLineIndent = 0;

                                    var index = m5.Value.Split('.')[4];

                                    if (int.Parse(index) != index5)
                                    {
                                        result.ParaIndexCheck.Add(string.Format("五级标题序号错误,标题：{0}", para.Range.Text));

                                        index5 = int.Parse(index) + 1;
                                    }
                                    else
                                    {
                                        index5++;
                                    }
                                }
                                else if (m4.Success)
                                {
                                    para.ParagraphFormat.FirstLineIndent = 0;

                                    var index = m4.Value.Split('.')[3];

                                    if (int.Parse(index) != index4)
                                    {
                                        result.ParaIndexCheck.Add(string.Format("四级标题序号错误,标题：{0}", para.Range.Text));

                                        index4 = int.Parse(index) + 1;
                                    }
                                    else
                                    {
                                        index4++;
                                    }

                                    index5 = 1;
                                }
                                else if (m3.Success)
                                {
                                    para.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading3;

                                    var index = m3.Value.Split('.')[2];

                                    if (int.Parse(index) != index3)
                                    {
                                        result.ParaIndexCheck.Add(string.Format("三级标题序号错误,标题：{0}", para.Range.Text));

                                        index3 = int.Parse(index) + 1;
                                    }
                                    else
                                    {
                                        index3++;
                                    }

                                    index4 = index5 = 1;
                                }
                            }
                        }

                        #endregion

                        _KYWordMergeService.SetChapter(item.Key.Replace("_", ""), para);

                        _KYWordMergeService.SetParaStyle(para);
                    }

                    section.Body.AppendChild(dstNode);
                }
            }

            if (attach != null)
            {
                _KYWordMergeService.AppendAttachDoc(doc, new Document(attach.Path));
            }

            // 地区检查
            _KYWordMergeService.CheckArea(doc, areaName, regAreas, result);

            // 禁用内容检查
            _KYWordMergeService.CheckDisableContent(doc, projInfo.DisableWord, result);

            _KYWordMergeService.SetTable(doc);

            _KYWordMergeService.SetImage(doc);

            _KYWordMergeService.SetPageNumber(doc);

            _KYWordMergeService.CreateList(doc);

            var filePath = getFilePath();

            if (cover != null)
            {
                var docCover = new Document(cover.Path);
                docCover.AppendDocument(doc, ImportFormatMode.UseDestinationStyles);
                docCover.Save(filePath);
            }
            else
            {
                doc.Save(filePath);
            }

            FileStream fs = new FileStream(filePath, FileMode.Open);

            var md5Str = Md5.GetMd5Hash(fs);

            var newAttach = new SysAttachFileEntity()
            {
                Md5 = md5Str,
                Extension = ".docx",
                FileDate = DateTime.Now,
                Name = "合并文档.docx",
                Path = filePath,
                Size = fs.Length,
                Type = (int)EnumAttachType.World,
                UploadDate = DateTime.Now,
                UploadUser = UserID
            };

            fs.Close();

            _IFileService.Add(newAttach);

            _IObjectAttachService.Add(new ObjectAttachEntity()
            {
                AttachID = newAttach.ID,
                ObjectID = ID,
                ObjectKey = "MergeProjectDocResult"
            });
            result.Attach = newAttach;

            return result;
        }

        private string getOrderIndex(string chapterIndex)
        {
            var indexArray = chapterIndex.Replace("_", "").Split('.');

            return string.Join("", indexArray.Select(a => a.PadLeft(2, '0')));

        }

        private string getFilePath()
        {
            var uploadPath = System.Configuration.ConfigurationManager.AppSettings["UploadFilePath"];

            var date = DateTime.Now;

            var fullPath = Path.Combine(uploadPath, "DocMerge", date.Year.ToString(), date.Month.ToString());

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            return string.Format("{0}\\{1}.docx", fullPath, Guid.NewGuid());
        }
    }
}
