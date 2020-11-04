namespace Zaaml.UI.Windows
{
	public sealed class MessageWindowResult
	{
		#region Ctors

		public MessageWindowResult(MessageWindowResultKind result, MessageWindowOptions options)
		{
			Result = result;
			Options = options;
		}

		#endregion

		#region Properties

		public MessageWindowOptions Options { get; private set; }

		public MessageWindowResultKind Result { get; private set; }

		#endregion
	}
}