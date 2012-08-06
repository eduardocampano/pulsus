using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Pulsus.Mvc
{
	public static class ExceptionManager
	{
		//public static ConcurrentDictionary<int, Expression<Func<ExceptionData, ActionResult>>> exceptionHandlers;

		public static CustomErrorsMode ErrorsMode { get; set; }

		//public static void AddHandler(Expression<Func<ExceptionData, ActionResult>> handler)
		//{
		//	exceptionHandlers[0] = handler;
		//}
	}
}
