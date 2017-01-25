using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core
{

    public delegate IQueryable<TEntity> OrderCallBack<TEntity>(IQueryable<TEntity> query);

    public class BaseRepository<TEntity> where TEntity : class
    {
        private DbContext _dbContext;

        public BaseRepository(DbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public DbContext DbContext
        {
            get
            {
                return this._dbContext;
            }
            set
            {
                this._dbContext = value;
            }
        }

        #region Implementation of IRepository<TEntity>

        /// <summary>
        /// 获取IQueryable
        /// </summary>
        /// <returns>IQueryable</returns>
        public IQueryable<TEntity> All()
        {
            return DbContext.Set<TEntity>();
        }

        /// <summary>
        /// 根据条件获取IQueryable
        /// </summary>
        /// <param name="predicate">查询条件表达式</param>
        /// <returns>IQueryable</returns>
        public IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> predicate)
        {
            return DbContext.Set<TEntity>().Where(predicate);
        }

        /// <summary>
        /// 根据主键属性得到实体
        /// </summary>
        /// <param name="objectId">主键属性值</param>
        /// <returns>实体</returns>
        public TEntity Get(object objectId)
        {
            return DbContext.Set<TEntity>().Find(objectId);
        }

        /// <summary>
        /// 根据条件获取唯一的实体
        /// </summary>
        /// <param name="predicate">查询条件表达式</param>
        /// <returns>实体</returns>
        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return DbContext.Set<TEntity>().SingleOrDefault(predicate);
        }

        /// <summary>
        /// 根据条件获取第一条实体
        /// </summary>
        /// <param name="predicate">查询条件表达式</param>
        /// <returns>实体</returns>
        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return DbContext.Set<TEntity>().FirstOrDefault(predicate);
        }

        /// <summary>
        /// 根据条件获取列表
        /// </summary>
        /// <param name="predicate">查询条件表达式</param>
        /// <returns>实体列表</returns>
        public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate)
        {
            return DbContext.Set<TEntity>().Where(predicate);
        }

        /// <summary>
        /// 根据条件获取分页列表
        /// </summary>
        /// <param name="predicate">查询条件表达式</param>
        /// <param name="condition">条件</param>
        /// <returns>实体分页列表</returns>
        public IQueryable<TEntity> QueryPagedList(Expression<Func<TEntity, bool>> predicate, PageQueryParam condition)
        {
            IQueryable<TEntity> query = DbContext.Set<TEntity>().Where(predicate);

            if (!String.IsNullOrEmpty(condition.OrderFiled))
            {
                query = query.OrderBy(condition.OrderFiled);
                query = query.OrderByDescending(t => condition.OrderFiled);
            }

            int totalCount = 0;
            var queryList = query.ToPagedList(condition.PageIndex, condition.PageSize, out totalCount);

            return queryList;
        }


        public IQueryable<TEntity> QueryPagedList(Expression<Func<TEntity, bool>> predicate, PageQueryParam condition,OrderCallBack<TEntity> CallBack)
        {
            IQueryable<TEntity> query = DbContext.Set<TEntity>().Where(predicate);

            query = CallBack(query);

            int totalCount = 0;
            var queryList = query.ToPagedList(condition.PageIndex, condition.PageSize, out totalCount);

            return queryList;
        }

        /// <summary>
        /// 根据条件获取分页列表
        /// </summary>
        /// <param name="predicate">查询条件表达式</param>
        /// <param name="condition">条件</param>
        /// <returns>实体分页列表</returns>
        public PageList<TEntity> GetPagedList(Expression<Func<TEntity, bool>> predicate, PageQueryParam condition)
        {
            IQueryable<TEntity> query = DbContext.Set<TEntity>().Where(predicate);

            if (!String.IsNullOrEmpty(condition.OrderFiled))
            {
                query = query.OrderBy(condition.OrderFiled);
            }

            var result = query.AsEnumerable().ToPagedList(condition.PageIndex, condition.PageSize);

            condition.Count = result.TotalItemCount;

            return result;
        }

        public PageList<TEntity> GetPagedList(Expression<Func<TEntity, bool>> predicate, PageQueryParam condition, OrderCallBack<TEntity> CallBack)
        {
            IQueryable<TEntity> query = DbContext.Set<TEntity>().Where(predicate);

            query = CallBack(query);

            var result = query.AsEnumerable().ToPagedList(condition.PageIndex, condition.PageSize);

            condition.Count = result.TotalItemCount;

            return result;
        }

        public PageList<TEntity> GetOrderPagedList(Expression<Func<TEntity, bool>> Condition, PageQueryParam Param)
        {
            var property = typeof(TEntity).GetProperty(Param.OrderFiled);
            var parameter = Expression.Parameter(typeof(TEntity), "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            var query = DbContext.Set<TEntity>().Where(Condition);

            string methodName = Param.IsDesc ? "OrderByDescending" : "OrderBy";
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(TEntity), property.PropertyType }, query.Expression, Expression.Quote(orderByExp));
            query = query.Provider.CreateQuery<TEntity>(resultExp);

            var result = query.AsEnumerable().ToPagedList(Param.PageIndex, Param.PageSize);

            Param.Count = result.TotalItemCount;

            return result;
        }


        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体</param>
        public void Add(TEntity entity)
        {
            DbContext.Set<TEntity>().Add(entity);
            DbContext.SaveChanges();
        }


        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="entity">实体</param>
        public void Edit(TEntity entity)
        {
            DbContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            DbContext.SaveChanges();
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体</param>
        public void Delete(TEntity entity)
        {
            DbContext.Set<TEntity>().Remove(entity);
            DbContext.SaveChanges();
        }

        /// <summary>
        /// 根据主键删除实体
        /// </summary>
        /// <param name="entity">实体</param>
        public void Delete(object objectId)
        {
            DbContext.Set<TEntity>().Remove(Get(objectId));
            DbContext.SaveChanges();
        }

        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <param name="predicate">查询条件表达式</param>
        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var list = DbContext.Set<TEntity>().Where(predicate);
            foreach (var entity in list)
            {
                DbContext.Set<TEntity>().Remove(entity);
            }
            DbContext.SaveChanges();
        }

        /// <summary>
        /// 获取总数
        /// </summary>
        /// <returns>总数</returns>
        public int Count()
        {
            return DbContext.Set<TEntity>().Count();
        }

        /// <summary>
        /// 根据条件获取总数
        /// </summary>
        /// <param name="predicate">查询条件表达式</param>
        /// <returns>总数</returns>
        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return DbContext.Set<TEntity>().Count(predicate);
        }

        public T Max<T>(Func<TEntity, T> call)
        {
            return DbContext.Set<TEntity>().Max(call);
        }
        #endregion


        #region TSQL To Database

        /// <summary>
        /// 执行SQL 返回影响行数
        /// </summary>
        /// <param name="Sql"></param>
        /// <param name="Parameter"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string Sql, params System.Data.SqlClient.SqlParameter[] Parameter)
        {
            return DbContext.Database.ExecuteSqlCommand(Sql, Parameter);
        }

        /// <summary>
        /// 返回查询实例
        /// </summary>
        /// <param name="Sql"></param>
        /// <param name="Parameter"></param>
        /// <returns></returns>
        public IEnumerable SqlQuery(string Sql, params System.Data.SqlClient.SqlParameter[] Parameter)
        {
            return DbContext.Set<TEntity>().SqlQuery(Sql, Parameter);
        }

        #endregion


    }
}
