using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Workflow.Activities.Rules;
using System.Workflow.ComponentModel.Serialization;
using System.CodeDom;

namespace App.Utility.Helpers
{
    /// <summary>
    /// Used for WWF rule engine, which is later replaced by ANTLR.
    /// Now is obsolete.
    /// </summary>
    public static class RuleSetHelper
    {
        public static string Serialize(RuleSet ruleSet)
        {
            var stream = new MemoryStream();
            // NOTE: When use UTF8 Encoding, the generated string will have "EF BB BF" in header (BOM)
            // It may cause deserialization failure if not matching.
            // here, use a UTF8Encoding instance by turning off BOM
            // XmlTextWriter writer = new XmlTextWriter(stream, System.Text.Encoding.UTF8);
            var writer = new XmlTextWriter(stream, new System.Text.UTF8Encoding(false));
            new WorkflowMarkupSerializer().Serialize(writer, ruleSet);
            String serializedString = System.Text.Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Position);
            writer.Flush();
            writer.Close();

            return serializedString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>RuleSet</returns>
        public static RuleSet Deserialize(string serializedString)
        {
            var obj = (new WorkflowMarkupSerializer()).Deserialize(new XmlTextReader(new StringReader(serializedString))) as RuleSet;
            return obj;
        }
        
        /// <summary>
        /// Return a layman readable rule string
        /// For clinical use.
        /// NOTE: The basic assumption is the RuleSet only contains one rule.
        /// This function is intended to be only used in this application.
        /// </summary>
        /// <param name="serializedString">serialized string of RuleSet object</param>
        /// <returns>e.g. 肱动脉收缩压 >= 140 || 肱动脉舒张压 >= 90</returns>
        public static string GetLaymanConditionString(string serializedString)
        {
            try
            {
                var obj = Deserialize(serializedString);
                var rules = obj.Rules.ToList();
                if (rules.Count >= 1)
                {
                    string str = rules[0].Condition.ToString()
                        .Replace("this.Context.NumericParameters.get_Item(\"", "")
                        .Replace("this.Context.BooleanParameters.get_Item(\"", "")
                        .Replace("this.Context.StringParameters.get_Item(\"", "")
                        .Replace("\")", "");
                    return str;
                }
            }
            catch { }
            return "";
        }
    }
}
