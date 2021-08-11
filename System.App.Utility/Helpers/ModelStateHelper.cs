using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace System.App.Utility.Helpers
{
    public static class ModelStateHelper
    {
        public static string GetErrors(ModelStateDictionary modelState)
        {
            if (modelState == null)
                return string.Empty;

            var msg = new List<string>();

            foreach(var x in modelState.Values)
            {
                foreach (var y in x.Errors)
                {
                    if(string.IsNullOrWhiteSpace(y.ErrorMessage) == false)
                    {
                        msg.Add(y.ErrorMessage);
                    }
                }
            }

            if (msg.Count() <= 0)
                return string.Empty;

            return msg.Aggregate((x, y) => x + Environment.NewLine + y);
        }
    }
}
