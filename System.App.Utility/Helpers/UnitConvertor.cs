using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.App.Utility.UnitsConversion;

namespace System.App.Utility
{
    public static class UnitConvertor
    {
        public static bool TryConvert(string unitSrc, string unitDes, float valueSrc, out float valueDes)
        {
            valueDes = valueSrc;

            if (string.IsNullOrWhiteSpace(unitSrc) && string.IsNullOrWhiteSpace(unitDes))
                return true;

            if (string.IsNullOrWhiteSpace(unitSrc) || string.IsNullOrWhiteSpace(unitDes))
                return false;

            unitSrc = unitSrc.Trim();
            unitDes = unitDes.Trim();

            try
            {
                UnitTable table = UnitTable.ClinicalUnitTable;
                valueDes = table.GetConversion(table.GetUnitCode(unitSrc), table.GetUnitCode(unitDes)).Convert(valueSrc);
                return true;
            }
            catch 
            {
                return false;
            }
        }
    }
}
