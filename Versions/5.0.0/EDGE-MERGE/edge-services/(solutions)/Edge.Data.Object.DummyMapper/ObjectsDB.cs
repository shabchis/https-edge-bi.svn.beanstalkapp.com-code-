using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Edge.Data.Objects;
using System.Data.SqlClient;

namespace Edge.Data.Object.DummyDB
{
	public class ObjectsDB
	{
		DummyMapper Mapper = new DummyMapper();
		string connS = "Data Source=localhost;Initial Catalog=EdgeObjects;Integrated Security=true";

		internal void Import(TextCreative Object)
		{
			using (SqlConnection conn = new SqlConnection(connS))
			{
				conn.Open();
				
				string insertTableName;
				if (Object.GetType().IsSubclassOf(typeof(Creative)))
				{
					insertTableName = "Creative";
				}
				else if(Object.GetType().IsSubclassOf(typeof(Target)))
				{
					insertTableName = "Target";
				}
				else
				{
					insertTableName = "EdgeObject";
				}
				
				
				StringBuilder sbCmd = new StringBuilder();
				sbCmd.Append("Insert into " + insertTableName);
				sbCmd.Append("()");


				SqlCommand cmd =
					new SqlCommand("");

				cmd.Connection = conn;
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						
					}
				}
			}
		}
	}
}
