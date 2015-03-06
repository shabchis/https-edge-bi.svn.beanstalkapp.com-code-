using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Collections.Generic;


public partial class StoredProcedures
{
	[Microsoft.SqlServer.Server.SqlProcedure]
	public static void SP_CLR_SetAnalysisSettingColumn()
	{
		try
		{
			using (SqlConnection conn = new SqlConnection("context connection=true"))
			{
				

				conn.Open();
				SqlCommand cmd =
					new SqlCommand(string.Format("SELECT [Account_ID],[AccountSettings], FROM [Seperia].[dbo].[User_GUI_Account]"));

				cmd.Connection = conn;
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						Dictionary<string, string> AccountSettingsDic = new Dictionary<string, string>();
						string[] AccountSettingsArr = Convert.ToString(reader[1]).Split(';');
						foreach (string pair in AccountSettingsArr)
						{
							string[] keyValue = pair.Split(':');
							AccountSettingsDic.Add(keyValue[0], keyValue[1]);
						}

						SqlCommand upadateCmd =
							new SqlCommand(string.Format("update [Seperia].[dbo].[User_GUI_Account] set [AnalysisSettings] = '<AnalysisSettings CubeName=\"{0}\"", AccountSettingsDic["Book"]));
						upadateCmd.Connection = conn;
						upadateCmd.ExecuteNonQuery();

					}
				}
			}
		}
		catch (Exception e)
		{
			
		}
	}
};
