using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using System.Text;
using System.Threading;

namespace Pulsus.Internal
{
	internal static class StackTraceHelper
	{
		private static Func<object> stackTraceGetter;

		static StackTraceHelper()
		{
			// Taken from http://ayende.com/blog/3879/reducing-the-cost-of-getting-a-stack-trace
			var stackFrameHelperType = typeof(object).Assembly.GetType("System.Diagnostics.StackFrameHelper");
			var getStackFramesInternal = Type.GetType("System.Diagnostics.StackTrace, mscorlib").GetMethod("GetStackFramesInternal", BindingFlags.Static | BindingFlags.NonPublic);

			var method = new DynamicMethod("GetStackTraceFast", typeof(object), new Type[0], typeof(StackTrace), true);

			var generator = method.GetILGenerator();
			generator.DeclareLocal(stackFrameHelperType);
			generator.Emit(OpCodes.Ldc_I4_0);
			generator.Emit(OpCodes.Ldnull);
			generator.Emit(OpCodes.Newobj, stackFrameHelperType.GetConstructor(new[] { typeof(bool), typeof(Thread) }));
			generator.Emit(OpCodes.Stloc_0);
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Ldc_I4_0);
			generator.Emit(OpCodes.Ldnull);
			generator.Emit(OpCodes.Call, getStackFramesInternal);
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Ret);
			stackTraceGetter = (Func<object>)method.CreateDelegate(typeof(Func<object>));
		}

		public static string GetStackTrace()
		{
            //return stackTraceGetter.Invoke();

			var stackTrace = new StackTrace(true);
            var frames = stackTrace.GetFrames();
            if (frames == null)
                return string.Empty;

            var builder = new StringBuilder(0xff);
            const string resourceString = "at";
            const string format = "in {0}:line {1}";
            var flag = true;

            

            foreach (var frame in frames)
            {
                var method = frame.GetMethod();
                if (method == null)
                    continue;

                var assemblyName = method.Module.Assembly.GetName().Name;

                // ignore Pulsus methods
                if (assemblyName == "Pulsus" || assemblyName == "Pulsus.Mvc")
                    continue;

                if (flag)
                    flag = false;
                else
                    builder.Append(Environment.NewLine);

                builder.AppendFormat(CultureInfo.InvariantCulture, "   {0} ", new object[] { resourceString });
                var declaringType = method.DeclaringType;
                if (declaringType != null)
                {
                    builder.Append(declaringType.FullName.Replace('+', '.'));
                    builder.Append(".");
                }

                builder.Append(method.Name);
                if (method is MethodInfo && method.IsGenericMethod)
                {
                    var genericArguments = method.GetGenericArguments();
                    builder.Append("[");
                    var index = 0;
                    var flag2 = true;
                    while (index < genericArguments.Length)
                    {
                        if (!flag2)
                            builder.Append(",");
                        else
                            flag2 = false;
                        builder.Append(genericArguments[index].Name);
                        index++;
                    }
                    builder.Append("]");
                }

                builder.Append("(");
                var parameters = method.GetParameters();
                var flag3 = true;
                for (var j = 0; j < parameters.Length; j++)
                {
                    if (!flag3)
                        builder.Append(", ");
                    else
                        flag3 = false;
                    var name = "<UnknownType>";
                    if (parameters[j].ParameterType != null)
                        name = parameters[j].ParameterType.Name;
                    builder.Append(name + " " + parameters[j].Name);
                }
                builder.Append(")");

                if (frame.GetILOffset() != -1)
                {
                    string fileName = null;
                    try
                    {
                        fileName = frame.GetFileName();
                    }
                    catch (SecurityException)
                    {
                    }
                    if (fileName != null)
                    {
                        builder.Append(' ');
                        builder.AppendFormat(CultureInfo.InvariantCulture, format, new object[] { fileName, frame.GetFileLineNumber() });
                    }
                }
            }

            return builder.ToString();
		}
	}
}
