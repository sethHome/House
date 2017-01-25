using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Microsoft.Practices.Unity;

namespace Api.Framework.Core.Tag
{
    /// <summary>
    /// ObjectTag 服务
    /// </summary>
    public partial class ObjectTagService : IObjectTagService
    {
        private BaseRepository<ObjectTagEntity> _DB;

        [Dependency]
        public ISysTagService _ISysTagService { get; set; }

        public ObjectTagService()
        {
            this._DB = new BaseRepository<ObjectTagEntity>(new SystemContext());
        }

        public PageSource<ObjectTagEntity> GetPagedList(PageQueryParam PageParam)
        {
            Expression<Func<ObjectTagEntity, bool>> expression = c => true;

            foreach (DictionaryEntry filter in PageParam.FilterCondtion)
            {
                var val = filter.Value.ToString();

                if (string.IsNullOrEmpty(val))
                {
                    continue;
                }

                switch (filter.Key.ToString())
                {
                    //case "ID":
                    //    {
                    //        var id = int.Parse(val);
                    //        expression = expression.And(c => c.ID == id);
                    //        break;
                    //    }
                    default:
                        break;
                }
            }

            var source = this._DB.GetPagedList(expression, PageParam);

            return new PageSource<ObjectTagEntity>(source);
        }

        public ObjectTagEntity Get(int ID)
        {
            return this._DB.Get(ID);
        }

        public void Add(string Key, int ID, List<ObjectTagInfo> Tags)
        {
            if (Tags == null || Tags.Count == 0)
            {
                throw new Exception("空的标签");
            }

            // 先清空对象的标签
            _DB.Delete(t => t.ObjectID == ID && t.ObjectKey == Key);

            Tags.ForEach(t => {
                Add(Key, ID,t);
            });
        }

        private void Add(string ObjectKey, int ObjectID, ObjectTagInfo ObjectTag)
        {
            if (ObjectTag.ObjectID == 0 && string.IsNullOrEmpty(ObjectTag.ObjectKey))
            {
                throw new ArgumentException("业务对象错误");
            }

            if (ObjectTag.ID > 0)
            {
                this._DB.Add(new ObjectTagEntity()
                {
                    ObjectID = ObjectID,
                    ObjectKey = ObjectKey,
                    TagID = ObjectTag.ID,
                });
            }
            else if (!string.IsNullOrEmpty(ObjectTag.TagName))
            {
                var tag = new SysTagEntity()
                {
                    TagName = ObjectTag.TagName,
                    IsDelete = false,
                    ObjectKey = ObjectKey
                };
                _ISysTagService.Add(tag);

                this._DB.Add(new ObjectTagEntity()
                {
                    ObjectID = ObjectID,
                    ObjectKey = ObjectKey,
                    TagID = tag.ID,
                });
            }
        }

        public void Update(int ID, ObjectTagEntity ObjectTag)
        {
            var entity = this._DB.Get(ID);

            entity.SetEntity(ObjectTag);

            this._DB.Edit(entity);
        }

        public void Delete(int ID)
        {
            this._DB.Delete(ID);
        }

        public List<SysTagEntity> GetObjectTags(string ObjectKey, int ObjectID)
        {
            var ls = _DB.GetList(t => t.ObjectKey == ObjectKey && t.ObjectID == ObjectID);

            var result = new List<SysTagEntity>();

            foreach (var item in ls)
            {
                result.Add(_ISysTagService.Get(item.TagID));
            }

            return result;
        }
    }
}
