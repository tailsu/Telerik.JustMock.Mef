using System.ComponentModel.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Telerik.JustMock.Mef.Tests
{
	[TestClass]
	public class ContractTests
	{
		[TestMethod]
		public void ShouldRespectContractNames()
		{
			var mocker = new MockExportProvider();
			mocker.Arrange<IMessage>("msg", msg => msg.Message).Returns("yep");
			var reader = mocker.Compose<MessageReader>("reader");
			Assert.AreEqual("yep", reader.Message);
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

		public interface IMessage
		{
			string Message { get; }
		}
	}
}
