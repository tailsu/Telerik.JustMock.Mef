using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Telerik.JustMock.Expectations;
using Telerik.JustMock.Helpers;

namespace Telerik.JustMock.Mef
{
	public static class MockExportProviderExtensions
	{
		public static FuncExpectation<object> Arrange<TObject>(this MockExportProvider @this, Expression<Func<TObject, object>> expression, IDictionary<string, object> metadata = null)
		{
			return @this.Arrange(null, expression, metadata);
		}

		public static FuncExpectation<object> Arrange<TObject>(this MockExportProvider @this, string contractName, Expression<Func<TObject, object>> expression, IDictionary<string, object> metadata = null)
		{
			return @this.ImportMock<TObject>(contractName, metadata).Arrange(expression);
		}

		public static ActionExpectation Arrange<TObject>(this MockExportProvider @this, Expression<Action<TObject>> expression, IDictionary<string, object> metadata = null)
		{
			return @this.Arrange(null, expression, metadata);
		}

		public static ActionExpectation Arrange<TObject>(this MockExportProvider @this, string contractName, Expression<Action<TObject>> expression, IDictionary<string, object> metadata = null)
		{
			return @this.ImportMock<TObject>(contractName, metadata).Arrange(expression);
		}

		public static void Assert<TService>(this MockExportProvider @this, Expression<Action<TService>> expression)
		{
			@this.GetExportedValue<TService>().Assert(expression);
		}

		public static void Assert<TService>(this MockExportProvider @this, Expression<Func<TService, object>> expression)
		{
			@this.GetExportedValue<TService>().Assert(expression);
		}

		public static void Assert<TService>(this MockExportProvider @this)
		{
			@this.GetExportedValue<TService>().Assert();
		}

		public static void Assert<TService>(this MockExportProvider @this, Expression<Func<TService, object>> expression, Occurs occurs)
		{
			@this.GetExportedValue<TService>().Assert(expression, occurs);
		}

		public static void Assert<TService>(this MockExportProvider @this, Expression<Action<TService>> expression, Occurs occurs)
		{
			@this.GetExportedValue<TService>().Assert(expression, occurs);
		}

		public static void Assert<TService>(this MockExportProvider @this, string contractName)
		{
			@this.GetExportedValue<TService>(contractName).Assert();
		}

		public static void Assert<TService>(this MockExportProvider @this, string contractName, Expression<Func<TService, object>> expression)
		{
			@this.GetExportedValue<TService>(contractName).Assert(expression);
		}

		public static void Assert<TService>(this MockExportProvider @this, string contractName, Expression<Action<TService>> expression)
		{
			@this.GetExportedValue<TService>(contractName).Assert(expression);
		}

		public static void Assert<TService>(this MockExportProvider @this, string contractName, Expression<Func<TService, object>> expression, Occurs occurs)
		{
			@this.GetExportedValue<TService>(contractName).Assert(expression, occurs);
		}

		public static void Assert<TService>(this MockExportProvider @this, string contractName, Expression<Action<TService>> expression, Occurs occurs)
		{
			@this.GetExportedValue<TService>(contractName).Assert(expression, occurs);
		}
	}
}
