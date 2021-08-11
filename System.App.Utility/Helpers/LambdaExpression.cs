using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace System.App.Utility.Helpers
{
    /// <summary>
    /// a helper class of Lambda generic methods
    /// </summary>
    public static class LambdaExpression
    {
        public static Expression<Func<TEntity, bool>> ContainsPredicate<TEntity, T>(T[] arr, string fieldname) where TEntity : class
        {
            ParameterExpression entity = Expression.Parameter(typeof(TEntity), "entity");
            MemberExpression member = Expression.Property(entity, fieldname);

            var containsMethods = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(m => m.Name == "Contains");
            MethodInfo method = null;
            foreach (var m in containsMethods)
            {
                if (m.GetParameters().Count() == 2)
                {
                    method = m;
                    break;
                }
            }
            method = method.MakeGenericMethod(member.Type);
            var exprContains = Expression.Call(method, new Expression[] { Expression.Constant(arr), member });
            return Expression.Lambda<Func<TEntity, bool>>(exprContains, entity);
        }

        public static IQueryable<T> WhereStringContains<T>(this IQueryable<T> query, string propertyName, string contains)
        {
            var parameter = Expression.Parameter(typeof(T), "type");
            var propertyExpression = Expression.Property(parameter, propertyName);
            MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var someValue = Expression.Constant(contains, typeof(string));
            var containsExpression = Expression.Call(propertyExpression, method, someValue);

            return query.Where(Expression.Lambda<Func<T, bool>>(containsExpression, parameter));
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName)
        {
            var propertyType = typeof(T).GetProperty(propertyName).PropertyType;
            var parameter = Expression.Parameter(typeof(T), "type");
            var propertyExpression = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(propertyExpression, new[] { parameter });

            return typeof(Queryable).GetMethods()
                                    .Where(m => m.Name == "OrderBy" && m.GetParameters().Length == 2)
                                    .Single()
                                    .MakeGenericMethod(new[] { typeof(T), propertyType })
                                    .Invoke(null, new object[] { query, lambda }) as IOrderedQueryable<T>;
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string propertyName)
        {
            var propertyType = typeof(T).GetProperty(propertyName).PropertyType;
            var parameter = Expression.Parameter(typeof(T), "type");
            var propertyExpression = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(propertyExpression, new[] { parameter });

            return typeof(Queryable).GetMethods()
                                    .Where(m => m.Name == "OrderByDescending" && m.GetParameters().Length == 2)
                                    .Single()
                                    .MakeGenericMethod(new[] { typeof(T), propertyType })
                                    .Invoke(null, new object[] { query, lambda }) as IOrderedQueryable<T>;
        }
    }
}
