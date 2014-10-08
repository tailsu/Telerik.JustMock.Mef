using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock.Helpers;

namespace Telerik.JustMock.Mef.Tests
{
	[TestClass]
	public class MefTests
	{
		[TestMethod]
		public void TypedMockExportProvider()
		{
			string msg = null;

			var mocker = new MockingCompositionContainer<Greeter>();
			mocker.Arrange<ILogger>(log => log.Log(Arg.AnyString)).DoInstead((string log) => msg = log);
			mocker.Arrange<IMessage>(message => message.Message).Returns("foo");
			mocker.ImportMock<ICounter>();
			var greeter = mocker.Instance;

			greeter.Greet();

			Assert.AreEqual("0: foo", msg);
			Assert.AreSame(mocker.Instance, mocker.Instance);
			Assert.AreSame(mocker.Compose<Greeter>(), mocker.Compose<Greeter>());
		}

		[TestMethod]
		public void UntypedMockExportProvider()
		{
			string msg = null;

			var mocker = new MockExportProvider();
			mocker.Arrange<ILogger>(log => log.Log(Arg.AnyString)).DoInstead((string log) => msg = log);
			mocker.Arrange<IMessage>(message => message.Message).Returns("foo");
			mocker.ImportMock<ICounter>();
			var greeter = mocker.Compose<Greeter>();

			greeter.Greet();

			Assert.AreEqual("0: foo", msg);
		}

		[TestMethod]
		public void ShouldAssertExpectationsOnAllMocks()
		{
			var mocker = new MockExportProvider();
			mocker.ImportMock<IMessage>();
			mocker.Arrange<ICounter>(counter => counter.Next).Returns(5);
			mocker.Arrange<ILogger>(log => log.Log(Arg.AnyString)).MustBeCalled();

			AssertEx.Throws<AssertFailedException>(() => mocker.Assert());
			AssertEx.Throws<AssertFailedException>(() => mocker.AssertAll());
			AssertEx.Throws<AssertFailedException>(() => mocker.Assert<ILogger>());
			mocker.ImportMock<ICounter>().Assert();
			AssertEx.Throws<AssertFailedException>(() => mocker.ImportMock<ICounter>().AssertAll());

			var greeter = mocker.Compose<Greeter>();
			greeter.Greet();

			mocker.Assert();
			mocker.AssertAll();
			mocker.Assert<ILogger>();
			mocker.ImportMock<ICounter>().Assert();
			mocker.ImportMock<ICounter>().AssertAll();
		}

		[TestMethod]
		public void ShouldSupportUserContainer()
		{
			var assemblyCatalog = new AssemblyCatalog(typeof(MefTests).Assembly);

			var mocks = new MockExportProvider();
			mocks.ImportMock<ILogger>();
			mocks.ImportMock<IMessage>();
			mocks.ImportMock<ICounter>();

			var container = new CompositionContainer(assemblyCatalog, mocks);
			var greeter = container.GetExportedValue<Greeter>();
			greeter.Greet();
		}

		public interface ILogger
		{
			void Log(string message);
		}

		public interface IMessage
		{
			string Message { get; }
		}

		public interface ICounter
		{
			int Next { get; }
		}

		[Export]
		public class Greeter
		{
			private ILogger logger;
			private IMessage message;
			private ICounter counter;

			[ImportingConstructor]
			public Greeter(ILogger logger, IMessage message, ICounter counter)
			{
				this.logger = logger;
				this.message = message;
				this.counter = counter;
			}

			public void Greet()
			{
				this.logger.Log(string.Format("{0}: {1}", this.counter.Next, this.message.Message));
			}
		}

	}
}
