using System;

namespace LIC.Malone.Core
{
	public class Header : IEquatable<Header>
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

		#region Equality
		
		// There seems to be conflicting advice on what you need to override to support equality comparisons. The IEquatable<T> interface
		// only requires implementing Equals(T), however MSDN and others recommend overriding Equals(object) and GetHashCode():
		//
		//		http://stackoverflow.com/questions/13741737/is-it-important-to-override-equals-if-im-implementing-iequatablet
		//		http://blogs.msdn.com/b/jaredpar/archive/2009/01/15/if-you-implement-iequatable-t-you-still-must-override-object-s-equals-and-gethashcode.aspx
		//
		// Others suggest *not* overriding GetHashCode() if your object is mutable:
		//
		//		http://www.aaronstannard.com/overriding-equality-in-dotnet/
		//		http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx
		//
		// While generally I'd go with what Eric Lippert says, he does "allow" for it if you can guarantee that the object won't mutate while
		// part of a hash table (List<Header>). And since it seems that you need the hash code for IEnumerable.SequenceEquals<T, U>(), I'll take
		// the risk of GetHashCode() and revisit if it proves problematic.

		public bool Equals(Header other)
		{
			if (ReferenceEquals(other, null))
				return false;

			if (ReferenceEquals(this, other))
				return true;

			return Name.Equals(other.Name) && Value.Equals(other.Value);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Header);
		}

		public override int GetHashCode()
		{
			// http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
			return new {Name, Value}.GetHashCode();
		}

		#endregion

		public override string ToString()
		{
			return string.Format("{0}: {1}", Name, Value);
		}
	}
}