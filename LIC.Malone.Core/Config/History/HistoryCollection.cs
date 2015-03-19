using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIC.Malone.Core.Config.History
{
	public class HistoryCollection
	{
		public int Version { get; set; }
		public List<Request> Requests { get; set; }
	}
}
