using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Generic;

namespace System.App.Utility.Helpers
{
    public static class EnumHelper
    {
        /// <summary>
        /// Get description annotation
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription<TEnum>(TEnum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if ((attributes != null) && (attributes.Length > 0))
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static TEnum GetEnumFromDescription<TEnum>(string str)
        {
            var values = Enum.GetValues(typeof(TEnum));
            foreach (TEnum value in values)
            {
                if (GetEnumDescription<TEnum>(value) == str)
                    return value;
            }
            return default(TEnum);
        }

        public static List<string> GetEnumDescriptions<TEnum>()
        {
            var values = Enum.GetValues(typeof(TEnum));
            var ret = new List<string>();
            foreach (var v in values)
            {
                ret.Add(GetEnumDescription<TEnum>((TEnum)v));
            }
            return ret;
        }
    }
    
}