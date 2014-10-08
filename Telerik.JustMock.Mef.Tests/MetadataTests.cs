using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock.Helpers;

namespace Telerik.JustMock.Mef.Tests
{
	[TestClass]
	public class MetadataTests
	{
		[TestMethod]
		public void ShouldRespectMetadataOnImports()
		{
			var container = new CompositionContainer(new AssemblyCatalog(typeof(MetadataTests).Assembly));
			var printer = container.GetExportedValue<MessagePrinter>();
			var value = printer.Print();
			var expected = "Error: Failed\nWarning: Be careful\nTrace: Stuff happened";
			Assert.AreEqual(expected, value);
		}

		[TestMethod]
		public void ShouldAssignMetadataOnImports()
		{
			var container = new MockExportProvider();

			container.ImportMock<IMessage>(metadata: new Dictionary<string, object> { { "Severity", Severity.Warning } })
				.Arrange(message => message.Message).Returns("Be careful");
			container.ImportMock<IMessage>(metadata: new Dictionary<string, object> { { "Severity", Severity.Error } })
				.Arrange(message => message.Message).Returns("Failed");

			var printer = container.Compose<MessagePrinter>();
			var result = printer.Print();
			var expected = "Error: Failed\nWarning: Be careful";
			Assert.AreEqual(expected, result);
		}

		public enum Severity
		{
			Trace,
			Warning,
			Error
		}

		public interface IMessage
		{
			string Message { get; }
		}

		[Export(typeof(IMessage))]
		[ExportMetadata("Severity", Severity.Trace)]
		public class TraceMessage : IMessage
		{
			public string Message
			{
				get { return "Stuff happened"; }
			}
		}

		[Export(typeof(IMessage))]
		[ExportMetadata("Severity", Severity.Warning)]
		public class WarningMessage : IMessage
		{
			public string Message
			{
				get { return "Be careful"; }
			}
		}

		[Export(typeof(IMessage))]
		[ExportMetadata("Severity", Severity.Error)]
		public class ErrorMessage : IMessage
		{
			public string Message
			{
				get { return "Failed"; }
			}
		}

		public interface IMessageMetadata
		{
			Severity Severity { get; }
		}

		[Export]
		public class MessagePrinter
		{
			[ImportMany]
			public IEnumerable<Lazy<IMessage, IMessageMetadata>> Messages { get; set; }

			public string Print()
			{
				return String.Join("\n", Messages.OrderByDescending(msg => msg.Metadata.Severity)
					.Select(msg => String.Format("{0}: {1}", msg.Metadata.Severity, msg.Value.Message)));
			}
		}
	}
}
