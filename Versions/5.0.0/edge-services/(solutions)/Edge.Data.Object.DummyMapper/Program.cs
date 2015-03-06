using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Edge.Data.Objects;

namespace Edge.Data.Object.DummyDB
{
	public class Program
	{
		static void Main(string[] args)
		{
			Type type = Type.GetType("TextCreative");
			Type type2 = Type.GetType("Edge.Data.Objects.TextCreative,TextCreative");
			Type type3 = Type.GetType("TextCreative,Edge.Data.Objects.TextCreative");
			Type type4 = Type.GetType("Edge.Data.Objects.TextCreative,Edge.Data.Objects", true);

			string assemblyName = typeof(Creative).Assembly.FullName;
		}


	}
}
