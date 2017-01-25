using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core
{
    public static class PageLinqExtensions
    {
        public static PageList<T> ToPagedList<T>
            (
                this IQueryable<T> allItems,
                int pageIndex,
                int pageSize
            )
        {
            if (pageIndex < 1)
                pageIndex = 1;
            var itemIndex = (pageIndex - 1) * pageSize;
            var pageOfItems = allItems.Skip(itemIndex).Take(pageSize);
            var totalItemCount = allItems.Count();
            return new PageList<T>(pageOfItems, pageIndex, pageSize, totalItemCount);
        }

        public static IQueryable<T> MyOrder<T>
           (
               this IQueryable<T> MyQuery,
               string OrderFiled,
               bool IsDesc
           )
        {
            // 排序
            //var ps = OrderFiled.Split('.');

            var property = typeof(T).GetProperty(OrderFiled);
            var parameter = Expression.Parameter(typeof(T), "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);

            string methodName = IsDesc ? "OrderByDescending" : "OrderBy";
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(T), property.PropertyType }, MyQuery.Expression, Expression.Quote(orderByExp));
            return MyQuery.Provider.CreateQuery<T>(resultExp);
        }

        public static PageList<T> ToPagedList<T>
            (
                this IEnumerable<T> allItems,
                int pageIndex,
                int pageSize
            )
        {
            if (pageIndex < 1)
                pageIndex = 1;
            var itemIndex = (pageIndex - 1) * pageSize;
            var pageOfItems = allItems.Skip(itemIndex).Take(pageSize);
            var totalItemCount = allItems.Count();
            return new PageList<T>(pageOfItems, pageIndex, pageSize, totalItemCount);
        }

        public static PageList<T> ToPagedList<T>
            (
                this IQueryable<T> allItems,
                int pageIndex,
                int pageSize,
                int totalCount
            )
        {
            if (pageIndex < 1)
                pageIndex = 1;
            var totalItemCount = totalCount;
            return new PageList<T>(allItems, pageIndex, pageSize, totalItemCount);
        }

        public static PageList<T> ToPagedList<T>
            (
                this IEnumerable<T> allItems,
                int pageIndex,
                int pageSize,
                int totalCount
            )
        {
            if (pageIndex < 1)
                pageIndex = 1;
            var totalItemCount = totalCount;
            return new PageList<T>(allItems, pageIndex, pageSize, totalItemCount);
        }


        public static IQueryable<T> ToPagedList<T>
        (
            this IQueryable<T> allItems,
            int pageIndex,
            int pageSize,
            out int totalCount
        )
        {
            if (pageIndex < 1)
                pageIndex = 1;
            int Size = pageSize == 0 ? 10 : pageSize;
            var itemIndex = (pageIndex - 1) * pageSize;
            totalCount = allItems.Count();
            return allItems.Skip(itemIndex).Take(Size);
        }

    }
}
