namespace LIC.Malone.Core
{
	public class Header
	{
		public const string Accept = "Accept";
		public const string AcceptEncoding = "Accept-Encoding";
		public const string AcceptEncodingValue = "gzip,deflate";
		public const string Authorization = "Authorization";
		public const string ContentType = "Content-Type";

		public string Name { get; set; }
		public string Value { get; set; }

		public Header()
		{
			
		}

		public Header(string name, string value)
		{
			Name = name;
			Value = value;
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}", Name, Value);
		}
	}
}