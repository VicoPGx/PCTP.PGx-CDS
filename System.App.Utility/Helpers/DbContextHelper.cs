using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Reflection;

namespace System.App.Utility.Helpers
{
    public static class DbContextHelper
    {
        /// <summary>
        /// Get the DbSet of T 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <returns></returns>
        public static object GetDbSet<T>(DbContext db) where T:class
        {
            foreach (PropertyInfo propertyInfo in db.GetType().GetProperties())
            {
                if (propertyInfo.Name.StartsWith(typeof(T).Name) && (propertyInfo.Name.Length - typeof(T).Name.Length <= 3))// || propertyInfo.Name == typeof(T).Name + "S" || propertyInfo.Name == typeof(T).Name + "s" || propertyInfo.Name == typeof(T).Name + "SET" || propertyInfo.Name == typeof(T).Name + "set") //&& propertyInfo.? == typeof(System.Data.Entity.DbSet)
                {
                    return propertyInfo.GetValue(db, null);
                }
            }
            return null;
        }

        /// <summary>
        /// Get associated items from DBContext by specified property name and value 
        /// Requires the property be string type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        /// <remarks>Only use this generic funciton for small-size data volume</remarks>
        public static IEnumerable<T> GetAssociatedItems<T>(string name, string value, DbContext db) where T : class
        {
            var list = new List<T>();
            if (string.IsNullOrEmpty(name) || value == null || db == null)
                return list;

            var dbSet = DbContextHelper.GetDbSet<T>(db);
            if (dbSet == null)
                throw new NotSupportedException("DbContext doesnot have corrsponding DbSet");

            foreach (var x in (DbSet<T>)dbSet)
            {
                var val = (string)TypeHelper.GetPropertyValue(x, name);
                if (val != null && val == value)
                {
                    list.Add(x);
                }
            }
            return list;
        }
    }
}
