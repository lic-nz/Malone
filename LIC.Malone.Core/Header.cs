namespace LIC.Malone.Core
{
	public class Header
	{
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