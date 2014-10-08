using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using Telerik.JustMock.Helpers;

namespace Telerik.JustMock.Mef
{
	public class MockExportProvider : ExportProvider
	{
		private class Importable
		{
			public Type Type;
			public string ContractName;
			public object Instance;
			public Dictionary<string, object> Metadata;
		}

		private readonly Dictionary<string, List<Importable>> importables = new Dictionary<string, List<Importable>>();

		public T Compose<T>(string contractName = null)
		{
			var typeCatalog = new TypeCatalog(typeof(T));
			var container = new CompositionContainer(typeCatalog, this);
			return container.GetExportedValue<T>(contractName);
		}

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

		public void AssertAll()
		{
			ForEachMock(mock => mock.AssertAll());
		}

		public void Assert()
		{
			ForEachMock(mock => mock.Assert());
		}

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
