using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Telerik.JustMock.Expectations;
using Telerik.JustMock.Helpers;

namespace Telerik.JustMock.Mef
{
	/// <summary>
	/// Extension methods for <see cref="MockExportProvider"/> for common Arrange and Assert operations.
	/// </summary>
	public static class MockExportProviderExtensions
	{
		/// <summary>
		/// Export a mocked type and arrange a method on it.
		/// </summary>
		/// <typeparam name="TObject">The type of mock to export.</typeparam>
		/// <param name="this">The mocking container.</param>
		/// <param name="expression">The method to arrange.</param>
		/// <param name="metadata">Optional metadata to identify the export.</param>
		/// <returns>Fluent interface to further configure the behavior of this arrangement.</returns>
		public static FuncExpectation<object> Arrange<TObject>(this MockExportProvider @this, Expression<Func<TObject, object>> expression, IDictionary<string, object> metadata = null)
		{
			return @this.Arrange(null, expression, metadata);
		}

		/// <summary>
		/// Export a mocked type and arrange a method on it.
		/// </summary>
		/// <typeparam name="TObject">The type of mock to export.</typeparam>
		/// <param name="this">The mocking container.</param>
		/// <param name="contractName">The contract name of the mock export.</param>
		/// <param name="expression">The method to arrange.</param>
		/// <param name="metadata">Optional metadata to identify the export.</param>
		/// <returns>Fluent interface to further configure the behavior of this arrangement.</returns>
		public static FuncExpectation<object> Arrange<TObject>(this MockExportProvider @this, string contractName, Expression<Func<TObject, object>> expression, IDictionary<string, object> metadata = null)
		{
			return @this.ExportMock<TObject>(contractName, metadata).Arrange(expression);
		}

		/// <summary>
		/// Export a mocked type and arrange a method on it.
		/// </summary>
		/// <typeparam name="TObject">The type of mock to export.</typeparam>
		/// <param name="this">The mocking container.</param>
		/// <param name="expression">The method to arrange.</param>
		/// <param name="metadata">Optional metadata to identify the export.</param>
		/// <returns>Fluent interface to further configure the behavior of this arrangement.</returns>
		public static ActionExpectation Arrange<TObject>(this MockExportProvider @this, Expression<Action<TObject>> expression, IDictionary<string, object> metadata = null)
		{
			return @this.Arrange(null, expression, metadata);
		}

		/// <summary>
		/// Export a mocked type and arrange a method on it.
		/// </summary>
		/// <typeparam name="TObject">The type of mock to export.</typeparam>
		/// <param name="this">The mocking container.</param>
		/// <param name="contractName">The contract name of the mock export.</param>
		/// <param name="expression">The method to arrange.</param>
		/// <param name="metadata">Optional metadata to identify the export.</param>
		/// <returns>Fluent interface to further configure the behavior of this arrangement.</returns>
		public static ActionExpectation Arrange<TObject>(this MockExportProvider @this, string contractName, Expression<Action<TObject>> expression, IDictionary<string, object> metadata = null)
		{
			return @this.ExportMock<TObject>(contractName, metadata).Arrange(expression);
		}

		/// <summary>
		/// Assert a method on an exported mock.
		/// </summary>
		/// <typeparam name="TObject">The type of the exported mock.</typeparam>
		/// <param name="this">The mocking container.</param>
		/// <param name="expression">The method to assert.</param>
		public static void Assert<TObject>(this MockExportProvider @this, Expression<Action<TObject>> expression)
		{
			@this.GetExportedValue<TObject>().Assert(expression);
		}

		/// <summary>
		/// Assert a method on an exported mock.
		/// </summary>
		/// <typeparam name="TObject">The type of the exported mock.</typeparam>
		/// <param name="this">The mocking container.</param>
		/// <param name="expression">The method to assert.</param>
		public static void Assert<TObject>(this MockExportProvider @this, Expression<Func<TObject, object>> expression)
		{
			@this.GetExportedValue<TObject>().Assert(expression);
		}

		/// <summary>
		/// Assert an exported mock.
		/// </summary>
		/// <typeparam name="TObject">The type of the exported mock.</typeparam>
		/// <param name="this">The mocking container.</param>
		public static void Assert<TObject>(this MockExportProvider @this)
		{
			@this.GetExportedValue<TObject>().Assert();
		}

		/// <summary>
		/// Assert a method on an exported mock.
		/// </summary>
		/// <typeparam name="TObject">The type of the exported mock.</typeparam>
		/// <param name="this">The mocking container.</param>
		/// <param name="expression">The method to assert.</param>
		/// <param name="occurs">Occurrence expectation.</param>
		public static void Assert<TObject>(this MockExportProvider @this, Expression<Func<TObject, object>> expression, Occurs occurs)
		{
			@this.GetExportedValue<TObject>().Assert(expression, occurs);
		}

		/// <summary>
		/// Assert a method on an exported mock.
		/// </summary>
		/// <typeparam name="TObject">The type of the exported mock.</typeparam>
		/// <param name="this">The mocking container.</param>
		/// <param name="expression">The method to assert.</param>
		/// <param name="occurs">Occurrence expectation.</param>
		public static void Assert<TObject>(this MockExportProvider @this, Expression<Action<TObject>> expression, Occurs occurs)
		{
			@this.GetExportedValue<TObject>().Assert(expression, occurs);
		}

		/// <summary>
		/// Assert an exported mock.
		/// </summary>
		/// <typeparam name="TObject">The type of the exported mock.</typeparam>
		/// <param name="this">The mocking container.</param>
		/// <param name="contractName">The contract name identifying the exported mock.</param>
		public static void Assert<TObject>(this MockExportProvider @this, string contractName)
		{
			@this.GetExportedValue<TObject>(contractName).Assert();
		}

		/// <summary>
		/// Assert a method on an exported mock.
		/// </summary>
		/// <typeparam name="TObject">The type of the exported mock.</typeparam>
		/// <param name="this">The mocking container.</param>
		/// <param name="contractName">The contract name identifying the exported mock.</param>
		/// <param name="expression">The method to assert.</param>
		public static void Assert<TObject>(this MockExportProvider @this, string contractName, Expression<Func<TObject, object>> expression)
		{
			@this.GetExportedValue<TObject>(contractName).Assert(expression);
		}

		/// <summary>
		/// Assert a method on an exported mock.
		/// </summary>
		/// <typeparam name="TObject">The type of the exported mock.</typeparam>
		/// <param name="this">The mocking container.</param>
		/// <param name="contractName">The contract name identifying the exported mock.</param>
		/// <param name="expression">The method to assert.</param>
		public static void Assert<TObject>(this MockExportProvider @this, string contractName, Expression<Action<TObject>> expression)
		{
			@this.GetExportedValue<TObject>(contractName).Assert(expression);
		}

		/// <summary>
		/// Assert a method on an exported mock.
		/// </summary>
		/// <typeparam name="TObject">The type of the exported mock.</typeparam>
		/// <param name="this">The mocking container.</param>
		/// <param name="contractName">The contract name identifying the exported mock.</param>
		/// <param name="expression">The method to assert.</param>
		/// <param name="occurs">Occurrence expectation.</param>
		public static void Assert<TObject>(this MockExportProvider @this, string contractName, Expression<Func<TObject, object>> expression, Occurs occurs)
		{
			@this.GetExportedValue<TObject>(contractName).Assert(expression, occurs);
		}

		/// <summary>
		/// Assert a method on an exported mock.
		/// </summary>
		/// <typeparam name="TObject">The type of the exported mock.</typeparam>
		/// <param name="this">The mocking container.</param>
		/// <param name="contractName">The contract name identifying the exported mock.</param>
		/// <param name="expression">The method to assert.</param>
		/// <param name="occurs">Occurrence expectation.</param>
		public static void Assert<TObject>(this MockExportProvider @this, string contractName, Expression<Action<TObject>> expression, Occurs occurs)
		{
			@this.GetExportedValue<TObject>(contractName).Assert(expression, occurs);
		}
	}
}
