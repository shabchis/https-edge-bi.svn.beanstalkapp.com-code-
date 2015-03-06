//------------------------------------------------------------------------------
// <copyright file="CSSqlStoredProcedure.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------


using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.Server;
using System.Text;


public partial class StoredProcedures
{

	[Microsoft.SqlServer.Server.SqlProcedure]
	public static void CLR_GetEasyForexReport(DateTime ToDay)
	{

		string fromDate = string.Format("{0}{1}{2}", ToDay.Year, ToDay.Month.ToString().Length == 1 ? "0" + ToDay.Month.ToString() : ToDay.Month.ToString(), "01");
		string toDate = ToDay.ToString("yyyyMMdd");


		try
		{
			StringBuilder selectMdxBuilder = new StringBuilder();
			selectMdxBuilder.Append("Select {[Measures].[Bo New Leads],[Measures].[regs],[Measures].[actives],[Measures].[SAT],[Measures].[% actives/regs],[Measures].[Cost/reg],[Measures].[Clicks/actives]} ");
			selectMdxBuilder.Append("On Columns,HIERARCHIZE( NONEMPTYCROSSJOIN ({[Getways Dim].[Region].[Region].AllMembers},{Filter ({[Getways Dim].[Gateways].[Gateway].members},(([Measures].[actives]) >  0) OR ([Measures].[regs] >  0))}");
			selectMdxBuilder.Append(")) On Rows From [BOEasyForex2] ");
			StringBuilder fromMdxBuilder = new StringBuilder();
			fromMdxBuilder.Append("WHERE ( [Accounts Dim].[Accounts].[client].&[7] ,{[Time Dim].[Time Dim].[Day].&["+fromDate+"]:[Time Dim].[Time Dim].[Day].&["+toDate+"]} )");
		
			SqlContext.Pipe.Send(selectMdxBuilder.ToString());
			SqlContext.Pipe.Send(fromMdxBuilder.ToString());

			#region Creating Command
			SqlCommand command = new SqlCommand("dbo.SP_ExecuteMDX");
			command.CommandType = CommandType.StoredProcedure;
			SqlParameter withMDX = new SqlParameter("WithMDX", string.Empty);
			command.Parameters.Add(withMDX);

			SqlParameter selectMDX = new SqlParameter("SelectMDX", selectMdxBuilder.ToString());
			command.Parameters.Add(selectMDX);

			SqlParameter fromMDX = new SqlParameter("FromMDX", fromMdxBuilder.ToString());
			command.Parameters.Add(fromMDX);
			#endregion

			using (SqlConnection conn = new SqlConnection("context connection=true"))
			{
				conn.Open();
				command.Connection = conn;
				using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
				{
					SqlContext.Pipe.Send(reader);
				}
			}
		}
		catch (Exception e)
		{
			throw new Exception(".Net Exception : " + e.ToString(), e);
		}
	}
}
