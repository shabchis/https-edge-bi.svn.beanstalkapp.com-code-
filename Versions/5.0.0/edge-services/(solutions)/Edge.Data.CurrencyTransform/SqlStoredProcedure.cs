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

public partial class StoredProcedures
{
	[Microsoft.SqlServer.Server.SqlProcedure]
	public static void CurrencyTransform()
	{
		// Put your code here
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

	public bool TryGetCurrencyMeasurs(int accountID, int channelID, out List<string> measures)
	{

		Dictionary<string, Measure> measurs = Measure.GetMeasures(output.Account, output.Channel, sqlCon, MeasureOptions.IsBackOffice, OptionsOperator.And);
		
		
		//Get Measure from 
		string cmdText = "Select [DisplayName] from Currency where RateDate = @rateDate and RateSymbol = @currency";
		SqlCommand cmd = new SqlCommand(cmdText);

		

		measures = new List<string>();

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
