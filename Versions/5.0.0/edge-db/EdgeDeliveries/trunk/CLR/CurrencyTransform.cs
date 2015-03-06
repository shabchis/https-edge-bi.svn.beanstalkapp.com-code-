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
using Edge.Data.Objects;
using System.Text;

public partial class StoredProcedures
{
	[Microsoft.SqlServer.Server.SqlProcedure]
	public static void CurrencyTransform(int deliveryID)
	{
		//GET MEASUERS
		//*********************************************************//

		//TO DO : Get measuers Dictionary<string, Measure> measurs = Measure.GetMeasures(accountID, channelID);
		//1.Fillter measures on currency measures , using LINQ
		//2.Get currency measuers names
		//
		List<string> measures = new List<string>();
		//*********************************************************//


		//GET DELIVERY TABLE NAME
		string cmdText = "SELECT [Name] FROM sys.tables WHERE NAME LIKE '%_@DeliveryID_%'";
		SqlCommand deliveryTableCmd = new SqlCommand(cmdText);
		SqlParameter sql_deliveryID = new SqlParameter("@DeliveryID", deliveryID);
		deliveryTableCmd.Parameters.Add(sql_deliveryID);

		string tableName = string.Empty;
		try
		{
			using (SqlConnection conn = new SqlConnection("context connection=true"))
			{
				conn.Open();
				deliveryTableCmd.Connection = conn;

				using (SqlDataReader reader = deliveryTableCmd.ExecuteReader())
				{
					if (reader.Read())
						//TO DO: Get metrics table only
						tableName = Convert.ToString(reader[0]);
				}
			}
		}
		catch (Exception e)
		{
			throw new Exception("Could not get Delivery Table Name from sql server", e);
		}


		//UPDATE DELIVERY TABLE CURRENCY
		StringBuilder setString = new StringBuilder();
		setString.Append( "Update [@DeliveryMetricsTable] set M.currencyRate = Rate," ); // TO DO : Create text

		foreach (string measureName in measures)
		{
			setString.Append(string.Format("{0}_Converted = CASE when M.CurrencyCode = 'USD' then 1 else {0}*C.rate END,", measureName));
		}

		setString.Remove(setString.Length - 1, 1);//removing last comma 

		setString.Append(" from M.currency from [@DeliveryMetricsTable] as M left outer job Currency C ");
		setString.Append("on C.currencyCode = M.currencyCode and C.rateDate = m.TargetDate ");
		
		SqlCommand updateDeliveryCmd = new SqlCommand(setString.ToString());
		SqlParameter sql_DeliveryTableName = new SqlParameter("@DeliveryMetricsTable", tableName);

		updateDeliveryCmd.Parameters.Add(sql_DeliveryTableName);

		try
		{
			using (SqlConnection conn = new SqlConnection("context connection=true"))
			{
				conn.Open();
				deliveryTableCmd.ExecuteNonQuery();
			}
		}
		catch (Exception e)
		{
			throw new Exception("Could update delivery table with converted currency", e);
		}
	}

	public bool TryGetCurrencyRateValue(DateTime rateDate, string rateSymbol, out double rate)
	{
		string cmdText = "Select RateValue from Currency where RateDate = @rateDate and RateSymbol = @currency";
		SqlCommand cmd = new SqlCommand(cmdText);

		SqlParameter date = new SqlParameter("@rateDate", rateDate);
		SqlParameter currency = new SqlParameter("@currency", rateSymbol);

		rate = -1;

		try
		{
			using (SqlConnection conn = new SqlConnection("context connection=true"))
			{
				conn.Open();
				cmd.Connection = conn;

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
						rate = Convert.ToDouble(reader[0]);
				}
			}
		}
		catch (Exception e)
		{
			throw new Exception("TryGetCurrencyRateValue : Could not get data from sql server", e);
		}

		if (rate < 0) return false;
		else return true;
	}


}
