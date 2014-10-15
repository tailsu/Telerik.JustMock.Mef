using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using Telerik.JustMock.Helpers;

namespace Telerik.JustMock.Mef
{
	/// <summary>
	/// A MEF export provider that can create mocks for importing in place
	/// of actual exports. You must call Arrange or ExportMock for every import
	/// that will need to be satisfied.
	/// </summary>
	public class MockExportProvider : ExportProvider
	{
		private class Importable
		{
			public Type Type;
			public object Instance;
			public Dictionary<string, object> Metadata;
		}

		private readonly Dictionary<string, List<Importable>> importables = new Dictionary<string, List<Importable>>();

		/// <summary>
		/// Creates a new instance of type T and satisfies all imports on the instance.
		/// </summary>
		/// <remarks>
		/// The type must be marked with an [Export] attribute and the contract name of the export
		/// must match the contractName argument passed this method.
		/// You must call Arrange or ExportMock for every import on T.
		/// </remarks>
		/// <typeparam name="T">The type of the instance to create and compose.</typeparam>
		/// <param name="contractName">The contract name of the export.</param>
		/// <returns>The composed instance.</returns>
		public T Compose<T>(string contractName = null)
		{
			var typeCatalog = new TypeCatalog(typeof(T));
			var container = new CompositionContainer(typeCatalog, this);
			return container.GetExportedValue<T>(contractName);
		}

		/// <summary>
		/// Exports a mock of type T. The exported mock is then ready for importing.
		/// </summary>
		/// <remarks>
		/// Mock exports are always [Shared], i.e. only a single instance of a mock for a given contract name
		/// is created and reused during composition. You can create two different mocks of the same type
		/// if you use different contract names.
		/// </remarks>
		/// <typeparam name="T">The type of the mock to export.</typeparam>
		/// <param name="contractName">The contract name of the export.
		/// The CreationPolicySystem.ComponentModel.Composition.CreationPolicy metadata cannot be set.</param>
		/// <param name="metadata">Additional export metadata.</param>
		/// <returns>The exported mock instance. The returned value may be used
		/// to further arrange and assert expectations on the mock.</returns>
		public T ExportMock<T>(string contractName = null, IDictionary<string, object> metadata = null)
		{
			var type = typeof(T);

			var name = contractName ?? AttributedModelServices.GetContractName(type);

			List<Importable> contractImportables;
			if (!importables.TryGetValue(name, out contractImportables))
			{
				contractImportables = new List<Importable>();
				importables.Add(name, contractImportables);
			}

			var importable = contractImportables.FirstOrDefault(imp => imp.Metadata.DictionaryEquals(metadata));
			if (importable == null)
			{
				importable = new Importable { Type = type };
				if (metadata != null)
				{
					importable.Metadata = new Dictionary<string, object>(metadata);
				}
				contractImportables.Add(importable);
				this.GetExportedValues<T>(contractName).ToArray();
			}

			return (T)importable.Instance;
		}

		/// <summary>
		/// Asserts all explicit and implicit expectations on all mocks exported by this provider.
		/// </summary>
		public void AssertAll()
		{
			ForEachMock(mock => mock.AssertAll());
		}

		/// <summary>
		/// Asserts all explicit expectations on all mocks exported by this provider.
		/// </summary>
		public void Assert()
		{
			ForEachMock(mock => mock.Assert());
		}

		/// <summary>
		/// Entry point into MEF for creating mock exports.
		/// </summary>
		/// <param name="definition"></param>
		/// <param name="atomicComposition"></param>
		/// <returns></returns>
		protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
		{
			List<Importable> contractImportables;
			if (!importables.TryGetValue(definition.ContractName, out contractImportables))
			{
				yield break;
			}

			var mockContract = "Telerik.JustMock.DynamicMefMocks." + definition.ContractName;

			foreach (var importable in contractImportables)
			{
				var metadata = new Dictionary<string, object>();
				if (importable.Metadata != null)
				{
					foreach (var kvp in importable.Metadata)
					{
						metadata.Add(kvp.Key, kvp.Value);
					}
				}

				metadata["System.ComponentModel.Composition.CreationPolicy"] = CreationPolicy.Shared;
				var cbid = definition as ContractBasedImportDefinition;
				if (cbid != null)
				{
					metadata["ExportTypeIdentity"] = cbid.RequiredTypeIdentity;
				}

				yield return new Export(new ExportDefinition(mockContract, metadata), () => GetMock(importable));
			}
		}

		private static object GetMock(Importable importable)
		{
			if (importable.Instance == null)
			{
				importable.Instance = Mock.Create(importable.Type);
			}
			return importable.Instance;
		}

		private void ForEachMock(Action<object> action)
		{
			foreach (var importable in importables.Values.SelectMany(list => list))
			{
				var instance = importable.Instance;
				if (instance != null)
				{
					action(instance);
				}
			}
		}
	}
}
