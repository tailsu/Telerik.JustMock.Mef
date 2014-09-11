using System;

namespace Telerik.JustMock.Mef
{
	public class MockingCompositionContainer<T> : MockExportProvider
	{
		private readonly Lazy<T> instance;
		private readonly string contractName;

		public MockingCompositionContainer(string contractName = null)
		{
			this.contractName = contractName;
			this.instance = new Lazy<T>(() => base.Compose<T>(this.contractName));
		}

		public T Instance
		{
			get
			{
				return this.instance.Value;
			}
		}

		public new U Compose<U>(string contractName = null)
		{
			if (typeof(U) == typeof(T) && contractName == this.contractName)
			{
				return (U)(object)this.Instance;
			}
			else
			{
				return base.Compose<U>(contractName);
			}
		}
	}
}
