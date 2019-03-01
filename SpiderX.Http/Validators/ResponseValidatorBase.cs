namespace SpiderX.Http
{
	public class ResponseValidatorBase
	{
		public static readonly ResponseValidatorBase Base = new ResponseValidatorBase();

		public ResponseValidatorBase() : this(3)
		{
		}

		public ResponseValidatorBase(byte retryTimes)
		{
			RetryTimes = retryTimes;
		}

		public byte RetryTimes { get; }

		public virtual bool CheckPass(string responseText)
		{
			return string.IsNullOrWhiteSpace(responseText);
		}
	}
}