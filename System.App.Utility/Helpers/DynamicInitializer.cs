using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace System.App.Utility.Helpers
{
    /// <summary>
    /// The quickest way to instantiate an object with a default constructor through Reflection
    /// </summary>
    public class DynamicInitializer
    {
        public static V NewInstance<V>() where V : class
        {
            return ObjectGenerator(typeof(V)) as V;
        }

        private static object ObjectGenerator(Type type)
        {
            var target = type.GetConstructor(Type.EmptyTypes);
            var dynamic = new DynamicMethod(string.Empty,
                          type,
                          new Type[0],
                          target.DeclaringType);
            var il = dynamic.GetILGenerator();
            il.DeclareLocal(target.DeclaringType);
            il.Emit(OpCodes.Newobj, target);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);

            var method = (Func<object>)dynamic.CreateDelegate(typeof(Func<object>));
            return method();
        }

        public static object NewInstance(Type type)
        {
            return ObjectGenerator(type);
        }
    }
}
