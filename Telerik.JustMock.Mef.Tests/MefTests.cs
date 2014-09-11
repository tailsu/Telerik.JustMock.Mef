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
			mocker.Arrange<ICounter>();
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
			mocker.Arrange<ICounter>();
			var greeter = mocker.Compose<Greeter>();

			greeter.Greet();

			Assert.AreEqual("0: foo", msg);
		}

		[TestMethod]
		public void ShouldRespectContractNames()
		{
			var mocker = new MockExportProvider();
			mocker.Arrange<IMessage>("msg", msg => msg.Message).Returns("yep");
			var reader = mocker.Compose<MessageReader>("reader");
			Assert.AreEqual("yep", reader.Message);
		}

		[TestMethod]
		public void ShouldAssertExpectationsOnAllMocks()
		{
			var mocker = new MockExportProvider();
			mocker.Arrange<IMessage>();
			mocker.Arrange<ICounter>(counter => counter.Next).Returns(5);
			mocker.Arrange<ILogger>(log => log.Log(Arg.AnyString)).MustBeCalled();

			AssertEx.Throws<AssertFailedException>(() => mocker.Assert());
			AssertEx.Throws<AssertFailedException>(() => mocker.AssertAll());
			AssertEx.Throws<AssertFailedException>(() => mocker.Assert<ILogger>());
			mocker.Arrange<ICounter>().Assert();
			AssertEx.Throws<AssertFailedException>(() => mocker.Arrange<ICounter>().AssertAll());

			var greeter = mocker.Compose<Greeter>();
			greeter.Greet();

			mocker.Assert();
			mocker.AssertAll();
			mocker.Assert<ILogger>();
			mocker.Arrange<ICounter>().Assert();
			mocker.Arrange<ICounter>().AssertAll();
		}

		[TestMethod]
		public void ShouldSupportUserContainer()
		{
			var assemblyCatalog = new AssemblyCatalog(typeof(MefTests).Assembly);

			var mocks = new MockExportProvider();
			mocks.Arrange<ILogger>();
			mocks.Arrange<IMessage>();
			mocks.Arrange<ICounter>();

			var container = new CompositionContainer(assemblyCatalog, mocks);
			var greeter = container.GetExportedValue<Greeter>();
			greeter.Greet();
		}
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

	[Export("reader")]
	public class MessageReader
	{
		private IMessage message;

		[ImportingConstructor]
		public MessageReader([Import("msg")] IMessage message)
		{
			this.message = message;
		}

		public string Message
		{
			get { return this.message.Message; }
		}
	}
}
