using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq.Expressions;
using Telerik.JustMock.Expectations;
using Telerik.JustMock.Helpers;

namespace Telerik.JustMock.Mef
{
	public class MockExportProvider : ExportProvider
	{
		private readonly Dictionary<Type, object> mocks = new Dictionary<Type, object>();
		private readonly Dictionary<string, Type> importables = new Dictionary<string, Type>();

		public T Compose<T>(string contractName = null)
		{
			var typeCatalog = new TypeCatalog(typeof(T));
			var container = new CompositionContainer(typeCatalog, this);
			return container.GetExportedValue<T>(contractName);
		}

		public T Arrange<T>(string contractName = null)
		{
			var type = typeof(T);
			var name = contractName ?? AttributedModelServices.GetContractName(type);
			importables[name] = type;
			return this.GetExportedValue<T>(contractName);
		}

		public FuncExpectation<object> Arrange<TObject>(Expression<Func<TObject, object>> expression)
		{
			return this.Arrange(null, expression);
		}

		public FuncExpectation<object> Arrange<TObject>(string contractName, Expression<Func<TObject, object>> expression)
		{
			return this.Arrange<TObject>(contractName).Arrange(expression);
		}

		public ActionExpectation Arrange<TObject>(Expression<Action<TObject>> expression)
		{
			return this.Arrange(null, expression);
		}

		public ActionExpectation Arrange<TObject>(string contractName, Expression<Action<TObject>> expression)
		{
			return this.Arrange<TObject>(contractName).Arrange(expression);
		}

		public void AssertAll()
		{
			foreach (var mock in mocks.Values)
			{
				mock.AssertAll();
			}
		}

		public void Assert()
		{
			foreach (var mock in mocks.Values)
			{
				mock.Assert();
			}
		}

		public void Assert<TService>(Expression<Action<TService>> expression)
		{
			this.GetExportedValue<TService>().Assert(expression);
		}

		public void Assert<TService>(Expression<Func<TService, object>> expression)
		{
			this.GetExportedValue<TService>().Assert(expression);
		}

		public void Assert<TService>()
		{
			this.GetExportedValue<TService>().Assert();
		}

		public void Assert<TService>(Expression<Func<TService, object>> expression, Occurs occurs)
		{
			this.GetExportedValue<TService>().Assert(expression, occurs);
		}

		public void Assert<TService>(Expression<Action<TService>> expression, Occurs occurs)
		{
			this.GetExportedValue<TService>().Assert(expression, occurs);
		}

		public void Assert<TService>(string contractName)
		{
			this.GetExportedValue<TService>(contractName).Assert();
		}

		public void Assert<TService>(string contractName, Expression<Func<TService, object>> expression)
		{
			this.GetExportedValue<TService>(contractName).Assert(expression);
		}

		public void Assert<TService>(string contractName, Expression<Action<TService>> expression)
		{
			this.GetExportedValue<TService>(contractName).Assert(expression);
		}

		public void Assert<TService>(string contractName, Expression<Func<TService, object>> expression, Occurs occurs)
		{
			this.GetExportedValue<TService>(contractName).Assert(expression, occurs);
		}

		public void Assert<TService>(string contractName, Expression<Action<TService>> expression, Occurs occurs)
		{
			this.GetExportedValue<TService>(contractName).Assert(expression, occurs);
		}

		protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
		{
			Type importType;
			if (!importables.TryGetValue(definition.ContractName, out importType))
			{
				yield break;
			}

			var metadata = new Dictionary<string, object>();
			metadata.Add("System.ComponentModel.Composition.CreationPolicy", CreationPolicy.Shared);
			var cbid = definition as ContractBasedImportDefinition;
			if (cbid != null)
			{
				metadata.Add("ExportTypeIdentity", cbid.RequiredTypeIdentity);
			}

			var mockContract = "Telerik.JustMock.Internal.MefMocks." + definition.ContractName;
			yield return new Export(new ExportDefinition(mockContract, metadata), () => this.GetMock(importType));
		}

		private object GetMock(Type type)
		{
			object value;
			if (!mocks.TryGetValue(type, out value))
			{
				value = Mock.Create(type);
				mocks.Add(type, value);
			}
			return value;
		}
	}
}
