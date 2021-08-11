using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Statements;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xaml;
using Microsoft.VisualBasic.Activities;

namespace System.App.Utility.Helpers
{
    /// <summary>
    /// Provide serialization and deserialization functions for Activity (System.Activities.Activity).
    /// Note: System.Activities.Activity is for WF4 and System.Workflow.ComponentModel.Activity is for WF3 and below.
    /// </summary>
    public static class WorkflowHelper
    {
        public static string Serialize(object ab, bool writeLogFile=false)
        {
            if (ab == null)
                throw new NullReferenceException("ActivityBuilder parameter is null");

            // Serialize the workflow to XAML and store it in a string.
            var sb = new System.Text.StringBuilder();
            var tw = new System.IO.StringWriter(sb);
            var xw = ActivityXamlServices.CreateBuilderWriter(new XamlXmlWriter(tw, new XamlSchemaContext()));
            XamlServices.Save(xw, ab);
            string serializedAB = sb.ToString();

            if (writeLogFile == true)
            {
                // Serialize the workflow to XAML and save it to a file.
                using (var sw = System.IO.File.CreateText(@"C:\add.xaml"))
                {
                    XamlWriter xw2 = ActivityXamlServices.CreateBuilderWriter(new XamlXmlWriter(sw, new XamlSchemaContext()));
                    XamlServices.Save(xw2, ab);
                }
            }

            return serializedAB;
        }

        public static Activity Deserialize(string s)
        {
            // Load the workflow definition from XAML and invoke it.
            using (var sr = new StringReader(s))
            {
                return ActivityXamlServices.Load(sr);
            }
        }

        public static void InspectActivity(Activity root, int indent)
        {
            // Inspect the activity tree using WorkflowInspectionServices.
            IEnumerator<Activity> activities = WorkflowInspectionServices.GetActivities(root).GetEnumerator();

            Debug.WriteLine("{0}{1}", new string(' ', indent), root.DisplayName);

            while (activities.MoveNext())
            {
                InspectActivity(activities.Current, indent + 2);
            }
        }

        public static void Test()
        {
            //
            // Demostractrate Serialization
            //
            var ab = new ActivityBuilder<int>();
            ab.Name = "Add";
            ab.Properties.Add(new DynamicActivityProperty { Name = "Operand1", Type = typeof(InArgument<int>) });
            ab.Properties.Add(new DynamicActivityProperty { Name = "Operand2", Type = typeof(InArgument<int>) });
            ab.Implementation = new Sequence
            {
                Activities = 
                {
                    new WriteLine
                    {
                        Text = new VisualBasicValue<string>("Operand1.ToString() + \" + \" + Operand2.ToString()")
                    },
                    new Assign<int>
                    {
                        To = new ArgumentReference<int> { ArgumentName = "Result" },
                        Value = new VisualBasicValue<int>("Operand1 + Operand2")
                    }
                }
            };

            var s = Serialize(ab as object);

            //
            // Demostractrate Deserialization
            //
            // Load the workflow definition from XAML and invoke it.
            var wf = ActivityXamlServices.Load(new StringReader(s)) as DynamicActivity<int>;
            Dictionary<string, object> wfParams = new Dictionary<string, object>
                    {
                        { "Operand1", 25 },
                        { "Operand2", 15 }
                    };
            int result = WorkflowInvoker.Invoke(wf, wfParams);
            Debug.WriteLine(result);

            //
            // Demonstrate InspectActivity
            //
            InspectActivity(wf, 0);
        }

        public static void Test2()
        {
            //
            // Demonstrate Serialization
            //
            Variable<List<string>> items = new Variable<List<string>>
            {
                Default = new VisualBasicValue<List<string>>("New List(Of String)()")
            };

            DelegateInArgument<string> item = new DelegateInArgument<string>();

            Activity wf = new Sequence
            {
                Variables = { items },
                Activities =
                {
                    new While((env) => items.Get(env).Count < 5)
                    {
                        Body = new AddToCollection<string>
                        {
                            Collection = new InArgument<ICollection<string>>(items),
                            Item = new InArgument<string>((env) => "List Item " + (items.Get(env).Count + 1))
                        }
                    },
                    new ForEach<string>
                    {
                        Values = new InArgument<IEnumerable<string>>(items),
                        Body = new ActivityAction<string>
                        {
                            Argument = item,
                            Handler = new WriteLine
                            {
                                Text = item
                            }
                        }
                    },
                    new Sequence
                    {
                        Activities = 
                        {
                            new WriteLine
                            {
                                Text = "Items added to collection."
                            }
                        }
                    }
                }
            };

            //
            // Demonstrate Deserialization
            //
            WorkflowInvoker.Invoke(wf);

            //
            // Demonstrate InspectActivity
            //
            InspectActivity(wf, 0);
        }
    }
}