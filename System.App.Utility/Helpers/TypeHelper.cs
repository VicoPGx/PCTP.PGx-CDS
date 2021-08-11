using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace System.App.Utility.Helpers
{
    public static class TypeHelper
    {
        public static object GetPropertyValue(object obj, string name)
        {
            foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
            {
                if (propertyInfo.Name == name && propertyInfo.CanRead)
                {
                    return propertyInfo.GetValue(obj, null);
                }
            }
            return null;
        }

        public static void SetPropertyValue(object obj, string name, object value)
        {
            foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
            {
                if (propertyInfo.Name == name && propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(obj, value, null);
                    return;
                }
            }
        }

        public static void SetPropertyValueByString(object obj, string name, string value)
        {
            foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
            {
                if (propertyInfo.Name == name && propertyInfo.CanWrite)
                {
                    if (propertyInfo.PropertyType == typeof(int?))
                    {
                        int v1;
                        int? nv1 = null;
                        var b1 = int.TryParse(value, out v1);
                        if (b1)
                            nv1 = v1;
                        propertyInfo.SetValue(obj, nv1, null);
                    }
                    else if (propertyInfo.PropertyType == typeof(float?))
                    {
                        float v2;
                        float? nv2 = null;
                        var b2 = float.TryParse(value, out v2);
                        if (b2)
                            nv2 = v2;
                        propertyInfo.SetValue(obj, nv2, null);
                    }
                    else if (propertyInfo.PropertyType == typeof(double?))
                    {
                        double v3;
                        double? nv3 = null;
                        var b3 = double.TryParse(value, out v3);
                        if (b3)
                            nv3 = v3;
                        propertyInfo.SetValue(obj, nv3, null);
                    }
                    else if (propertyInfo.PropertyType == typeof(DateTime?))
                    {
                        DateTime v4;
                        DateTime? nv4 = null;
                        var b4 = DateTime.TryParse(value, out v4);
                        if (b4)
                            nv4 = v4;
                        propertyInfo.SetValue(obj, nv4, null);
                    }
                    else if (propertyInfo.PropertyType == typeof(decimal?))
                    {
                        decimal v5;
                        decimal? nv5 = null;
                        var b5 = decimal.TryParse(value, out v5);
                        if (b5)
                            nv5 = v5;
                        propertyInfo.SetValue(obj, nv5, null);
                    }
                    else if (propertyInfo.PropertyType == typeof(string))
                    {
                        propertyInfo.SetValue(obj, value, null);
                    }
                    else
                    {
                        throw new Exception(propertyInfo.PropertyType.ToString());
                    }
                    return;
                }
            }
        }
        
        public static Type GetPropertyType(object obj, string name)
        {
            foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
            {
                if (propertyInfo.Name == name)
                {
                    return propertyInfo.PropertyType;
                }
            }
            return null;
        }

        /// <summary>
        /// Set property value by string. String will be converted to corresponding property type
        /// </summary>
        public static void SetPropertyStringValue(object obj, string name, string value)
        {
            foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
            {
                if (propertyInfo.Name == name && propertyInfo.CanWrite)
                {                    
                    // var converted = Convert.ChangeType(value, propertyInfo.PropertyType);
                    // Convert.ChangeType() fails on nullable type, use following method

                    Type nt=Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                    Type t = nt ?? propertyInfo.PropertyType;
                    object safeValue = (value == null || value=="NULL"||value=="null"||(nt!=null && value=="")) ? null : Convert.ChangeType(value, t);
                    propertyInfo.SetValue(obj, safeValue, null);
                    return;
                }
            }
        }

        public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            var typelist = assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();

            for (int i = 0; i < typelist.Length; i++)
            {
                Console.WriteLine(typelist[i].Name);
            }

            return typelist;
        }


        public static List<string> GetMembers(Type targetType)
        {
            List<string> members = new List<string>();

            if (targetType != null)
            {
                try
                {
                    PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    foreach (PropertyInfo property in properties)
                    {
                        members.Add(string.Format(CultureInfo.InvariantCulture, "{0}   ({1})", property.Name, property.PropertyType));
                    }

                    FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    foreach (FieldInfo field in fields)
                    {
                        members.Add(string.Format(CultureInfo.InvariantCulture, "{0}   ({1})", field.Name, field.FieldType));
                    }

                    members.Sort(); //sort all fields and properties as one, but exclude methods which will all be listed at the end

                    List<string> methodMembers = new List<string>();
                    MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    foreach (MethodInfo method in methods)
                    {
                        if (!method.Name.StartsWith("get_", StringComparison.Ordinal) && !method.Name.StartsWith("set_", StringComparison.Ordinal))
                        {
                            methodMembers.Add(method.ToString());
                        }
                    }

                    methodMembers.Sort();
                    members.AddRange(methodMembers);
                }
                catch{}
            }

            return members;
        }

        public static List<string> GetProperties(Type targetType)
        {
            List<string> members = new List<string>();

            if (targetType != null)
            {
                try
                {
                    PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    foreach (PropertyInfo property in properties)
                    {
                        members.Add(property.Name);
                    }
                    members.Sort();
                }
                catch { }
            }

            return members;
        }

        public static List<string> GetMethods(Type targetType)
        {
            List<string> list = new List<string>();

            if (targetType != null)
            {
                try
                {
                    var methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    foreach (var method in methods)
                    {
                        list.Add(method.Name);
                    }
                    list.Sort();
                }
                catch { }
            }

            return list;
        }

        public static MethodInfo GetMethod(Type targetType, string methodName)
        {
            MethodInfo ret = null;

            if (targetType != null)
            {
                try
                {
                    var methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    ret = methods.FirstOrDefault(x => x.Name == methodName);
                }
                catch { }
            }

            return ret;
        }

        /// <summary>
        /// Clone object according to property names. Properties with same name will be copied.
        /// </summary>
        /// <typeparam name="T1">destination type</typeparam>
        /// <typeparam name="T2">src type</typeparam>
        /// <param name="o1">destination object</param>
        /// <param name="o2">src object</param>
        /// <param name="skipId">whether to skip "Id". When src is an entity object, its "Id" is often not writable</param>
        public static void CloneByReflection<T1, T2>(T1 o1, T2 o2, bool skipId = false)
        {
            var properties = TypeHelper.GetProperties(o2.GetType());
            foreach (var p in TypeHelper.GetProperties(o1.GetType()))
            {
                if (p.ToLower() == "id" && skipId == true)
                    continue;
                if (properties.Contains(p))
                {
                    TypeHelper.SetPropertyValue(o1, p, TypeHelper.GetPropertyValue(o2, p));
                }
            }
        }

        /// <summary>
        /// Compare two objects by comparing property value.
        /// NOTE: Currently only use this method for type with primitive properties.
        /// For complex object comparison, use KellermanSoftware.CompareNETObjects
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="o1">object #1</param>
        /// <param name="o2">object #2</param>
        /// <param name="skipId">whether to skip "Id". When src or des is an entity object, "Id" should be omited.</param>
        /// <returns>true if equal; false unequal</returns>
        public static bool CompareByReflection<T>(T o1, T o2, bool skipId = false)
        {
            if (o1 == null && o2 == null)
                return true;

            if (o1 == null || o2 == null)
                return false;

            var properties = TypeHelper.GetProperties(typeof(T));
            foreach (var p in properties)
            {
                if (p.ToLower() == "id" && skipId == true)
                    continue;

                // This actually does a object reference comparison, not object value comparison.
                // if (TypeHelper.GetPropertyValue(o1, p) != TypeHelper.GetPropertyValue(o2, p))
                var v1 = TypeHelper.GetPropertyValue(o1, p);
                var v2 = TypeHelper.GetPropertyValue(o2, p);
                if (v1 == null && v2 == null)
                    continue;
                if (v1 == null || v2 == null)
                    return false;

                var t = v1.GetType();
                // The primitive types in C# are Boolean (bool), Byte (byte), SByte (sbyte), Int16 (short), UInt16, Int32 (int), UInt32 (uint), Int64 (long), UInt64 (ulong), IntPtr, UIntPtr, Char (char), Double (double), and Single (single). 
                if (t.IsPrimitive)
                {
                    if(v1.ToString()!=v2.ToString()) // as v1 and v2 and object reference, cannot compare directly, use ToString()
                        return false;
                }
                else if(t == typeof(Decimal))
                {
                    if((Decimal)v1!=(Decimal)v2)
                        return false;
                }
                else if (t == typeof(Decimal?))
                {
                    if ((Decimal?)v1 != (Decimal?)v2)
                        return false;
                }
                else if(t == typeof(String))
                {
                    if ((String)v1 != (String)v2)
                        return false;
                }
                else if (t == typeof(DateTime))
                {
                    if ((DateTime)v1 != (DateTime)v2)
                        return false;
                }
                else if (t == typeof(DateTime?))
                {
                    if ((DateTime?)v1 != (DateTime?)v2)
                        return false;
                }
            }
            return true;
        }

        public static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }
    }
}
