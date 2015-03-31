namespace LIC.Malone.Core
{
	public class Header
	{
		public string Name { get; set; }
		public string Value { get; set; }

		public Header(string name, string value)
		{
			Name = name;
			Value = value;
		}
	}
}