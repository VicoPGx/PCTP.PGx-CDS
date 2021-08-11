using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
namespace PGx.Model.Enums
{
    public enum Iupac
    {
        //[Description("A, A")]
        //[Description("A"),Description("A")]
        [Description("A,A")]
        A,
        [Description("C,C")]
        C,
        [Description("G,G")]
        G,
        [Description("T,T")]
        T,
        [Description("R,[AG]")]
        R,
        [Description("Y,[CT]")]
        Y,
        [Description("S,[GC]")]
        S,
        [Description("W,[AT]")]
        W,
        [Description("K,[GT]")]
        K,
        [Description("M,[AC]")]
        M,
        [Description("B,[CGT]")]
        B,
        [Description("D,[AGT]")]
        D,
        [Description("H,[CT]")]
        H,
        [Description("V,[ACG]")]
        V,
        [Description("N,[ACGT]")]
        N,
        [Description("-,del")]
        DEL

    }

    public static class IupacExtention
    {

        private static String m_code;
        private static String m_pattern;


        private static void IupacConstruct(this Iupac iupacEnum)
        {
            Type type = typeof(Iupac);
            FieldInfo info = type.GetField(iupacEnum.ToString());
            //DescriptionAttribute[] a = (DescriptionAttribute[])Attribute.GetCustomAttributes(info, type);
            DescriptionAttribute [] a= (DescriptionAttribute [])info.GetCustomAttributes(typeof(DescriptionAttribute), false);
            m_code = a[0].Description.Split(',')[0];
            m_pattern = a[0].Description.Split(',')[1];
        }

        public static String getCode(this Iupac iupacEnum)
        {
            IupacConstruct(iupacEnum);
            return m_code;
        }

        public static String getRegex(this Iupac iupacEnum)
        {
            IupacConstruct(iupacEnum);
            return m_pattern;
        }


        public static Iupac lookup(String value)
        {
            //Preconditions.checkNotNull(value);

            if (value.Equals("-"))
            {
                return Iupac.DEL;
            }
            return (Iupac)Enum.Parse(typeof(Iupac), value.ToUpper());
        }
    }
}
