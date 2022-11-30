using CuttingEdge.Conditions;

namespace NewEggAccess.Models
{
	public class NewEggEnvelopeWrapper<T> where T : class, new()
	{
		public NewEggEnvelope<T> NeweggEnvelope { get; set; }
	}

	public class NewEggEnvelope<T> where T : class, new()
	{
		public NewEggEnvelopeHeader Header { get; set; }
		public string MessageType { get; set; }
		public T Message { get; set; }

		public NewEggEnvelope(string messageType, T message)
		{
			Condition.Requires(messageType, "messageType").IsNotNullOrWhiteSpace();

			this.Header = new NewEggEnvelopeHeader(2.0M);
			this.MessageType = messageType;
			this.Message = message;
		}
	}

	public class NewEggEnvelopeHeader
	{
		public decimal DocumentVersion { get; private set; }

		public NewEggEnvelopeHeader(decimal documentVersion)
		{
			Condition.Requires(documentVersion, "documentVersion").IsGreaterOrEqual(1.0M);

			this.DocumentVersion = documentVersion;
		}
	}
}