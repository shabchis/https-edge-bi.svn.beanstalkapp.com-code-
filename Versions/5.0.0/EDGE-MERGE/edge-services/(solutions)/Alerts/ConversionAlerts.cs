//------------------------------------------------------------------------------
// <copyright file="CSSqlStoredProcedure.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class StoredProcedures
{
	public class campaign
	{
		public int ID;
		public string Name;
		public double Cost;
		public double Conv;
		public double CPA;

		public campaign(SqlDataReader reader)
		{
			ID = Convert.ToInt32(reader[0]);
			Name = Convert.ToString(reader[1]);
			Cost = Convert.ToDouble(reader[2]);
			Conv = Convert.ToDouble(reader[3]);

			if (Conv != 0)
				CPA = Cost / Conv;
			else
			{
				Conv = 1;
				CPA = Cost;
			}
		}

	}

	[Microsoft.SqlServer.Server.SqlProcedure]
	public static void ConversionAlerts(Int32 AccountID, Int32 Period, DateTime ToDay, Int32 ChannelID, Int32 threshold, string excludeIds)
	{

		string CONNECTION = "Data Source=BI_RND;Initial Catalog=testdb;Integrated Security=False;Pooling=False;";
		string Edge_conn = "Data Source=79.125.11.74;Initial Catalog=Seperia;Integrated Security=false;User ID=SeperiaServices;PWD=Asada2011!";

		string fromDate = ToDay.AddDays((Double)(-1 * Period)).ToString("yyyyMMdd");
		string toDate = ToDay.ToString("yyyyMMdd");

		try
		{
			string txtCmd = " select campaignid,campaign, sum(cost), sum(conv) " +
							  "FROM [dbo].[Paid_API_AllColumns_v29] " +
							   "where account_id = @accountID " +
							  "and day_code between @fromDayCode and @toDayCode " +
							  "and channel_id = @channelID ";



			if (!string.IsNullOrEmpty(excludeIds))
			{
				txtCmd += string.Format("and campaignid NOT IN({0}) ", excludeIds);
			}

			txtCmd += "group by campaignid,campaign order by campaign";

			SqlCommand cmd = new SqlCommand(txtCmd);
			SqlParameter accountID = new SqlParameter("@accountID", AccountID);
			SqlParameter fromDayCode = new SqlParameter("@fromDayCode", fromDate);
			SqlParameter toDayCode = new SqlParameter("@toDayCode", toDate);
			SqlParameter channelID = new SqlParameter("@channelID", ChannelID);
			cmd.Parameters.Add(accountID);
			cmd.Parameters.Add(fromDayCode);
			cmd.Parameters.Add(toDayCode);
			cmd.Parameters.Add(channelID);

			List<campaign> campaigns = new List<campaign>();

			using (SqlConnection conn = new SqlConnection(Edge_conn))
			{
				conn.Open();

				cmd.Connection = conn;
				cmd.CommandTimeout = 9000;
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						campaigns.Add(new campaign(reader));
					}
				}
			}

			double totalCost = 0;
			double totalConv = 0;
			double avgCpa = 0;

			foreach (campaign camp in campaigns)
			{
				totalCost += camp.Cost;
				totalConv += camp.Conv;
			}

			avgCpa = totalCost / totalConv / Period;

			var CPA_Alert = from c in campaigns
							where c.CPA > threshold * avgCpa
							select c;

			StringBuilder cmdSb = new StringBuilder();

			foreach (var item in CPA_Alert)
			{
				cmdSb.Append(string.Format("select '{3}' as CampaignID, '{0}' as 'Campaign' , {1} as 'Cost', {2} as 'Conv' Union "
					, item.Name, item.Cost, item.Conv, item.ID));
			}

			if (cmdSb.Length > 0)
			{
				cmdSb.Remove(cmdSb.Length - 6, 6);
				SqlCommand reasultsCmd = new SqlCommand(cmdSb.ToString());
				using (SqlConnection conn2 = new SqlConnection("context connection=true"))
				{
					conn2.Open();
					reasultsCmd.Connection = conn2;
					reasultsCmd.CommandTimeout = 9000;
					using (SqlDataReader reader = reasultsCmd.ExecuteReader())
					{
						SqlContext.Pipe.Send(reader);
					}
				}
			}


		}
		catch (Exception e)
		{
			throw new Exception("Could not get table data " + e.ToString(), e);
		}
	}
}
