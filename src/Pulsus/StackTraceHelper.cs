using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Pulsus
{
	public static class StackTraceHelper
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
			var stackTrace = new StackTrace(true);
			return stackTrace.ToString();

			//return stackTraceGetter.Invoke();
		}
	}
}
