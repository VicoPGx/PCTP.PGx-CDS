using System;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;

namespace System.App.Utility.Helpers
{
    public static class MathExpressionHelper
    {
        /// <summary>
        /// A simple function to get the result of a C# expression (basic and advanced math possible)
        /// </summary>
        /// <param name="expression">String value containing an expression that can evaluate to a double.</param>
        /// <returns>a Double value after evaluating the command string.</returns>
        public static double Evaluate(string expression)
        {
            //Create a C# Code Provider
            using (var codeProvider = new CSharpCodeProvider())
            {
                // Build the parameters for source compilation.
                CompilerParameters cp = new CompilerParameters();
                cp.GenerateExecutable = false;//No need to make an EXE file here.
                cp.GenerateInMemory = true;   //But we do need one in memory.
                cp.OutputAssembly = "TempModule"; //This is not necessary, however, if used repeatedly, causes the CLR to not need to load a new assembly each time the function is run.
                //The below string is basically the shell of a C# program, that does nothing, but contains an Evaluate() method for our purposes.  I realize this leaves the app open to injection attacks, But this is a simple demonstration.
                string TempModuleSource =
                    "namespace ns{" +
                    "using System;" +
                    "class class1{" +
                    "public static double Evaluate(){return " +
                    expression + ";}}} ";

                //Our actual Expression evaluator
                CompilerResults cr = codeProvider.CompileAssemblyFromSource(cp, TempModuleSource);
                if (cr.Errors.Count > 0)
                {
                    //If a compiler error is generated, we will throw an exception because the syntax was wrong - again, this is left up to the implementer to verify syntax before calling the function.  The calling code could trap this in a try loop, and notify a user the command was not understood, for example. 
                    throw new ArgumentException("Expression cannot be evaluated, please use a valid C# xpression");
                }
                else
                {
                    MethodInfo Methinfo = cr.CompiledAssembly.GetType("ns.class1").GetMethod("Evaluate");
                    return (double)Methinfo.Invoke(null, null);
                }
            }
        }
    }
}