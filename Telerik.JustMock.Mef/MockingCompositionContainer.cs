using System;

namespace Telerik.JustMock.Mef
{
	/// <summary>
	/// A typed composition container that can compose an instance of type T once. It is designed
	/// to mimick the way Telerik.JustMock.AutoMock.MockingContainer feels.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class MockingCompositionContainer<T> : MockExportProvider
	{
		private readonly Lazy<T> instance;
		private readonly string contractName;

		/// <summary>
		/// Creates a new MockingCompositionContainer instance.
		/// </summary>
		/// <param name="contractName">The contract name of the exported instance.</param>
		public MockingCompositionContainer(string contractName = null)
		{
			this.contractName = contractName;
			this.instance = new Lazy<T>(() => base.Compose<T>(this.contractName), isThreadSafe: true);
		}

		/// <summary>
		/// The composed instance. You can get this property only if you
		/// have called Arrange or ExportMock for every import on U.
		/// </summary>
		public T Instance
		{
			get
			{
				return this.instance.Value;
			}
		}

		/// <summary>
		/// Creates a new instance of type U and satisfies all imports on the instance.
		/// </summary>
		/// <remarks>
		/// If U is the same type as T, then the instance returned is the same as the one
		/// returned by <see cref="Instance"/>.
		/// The type must be marked with an [Export] attribute and the contract name of the export
		/// must match the contractName argument passed this method.
		/// You must call Arrange or ExportMock for every import on U.
		/// </remarks>
		/// <typeparam name="U">The type of the instance to create and compose.</typeparam>
		/// <param name="contractName">The contract name of the export.</param>
		/// <returns>The composed instance.</returns>
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
