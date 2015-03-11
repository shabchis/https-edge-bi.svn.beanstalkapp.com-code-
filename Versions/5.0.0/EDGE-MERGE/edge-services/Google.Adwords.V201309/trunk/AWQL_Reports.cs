using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Api.Ads.AdWords.Lib;
using Google.Api.Ads.AdWords.Util.Reports;
using Google.Api.Ads.AdWords.v201302;

namespace Edge.Services.Google.AdWords
{
	public static class AWQL_Reports
	{

		public static void DownloadReport(AdWordsUser user, string fileName)
		{

			StringBuilder sb = new StringBuilder();
			sb.Append("SELECT ");
			foreach (string item in GoogleStaticReportFields.AD_PERFORMANCE_REPORT_FIELDS)
			{
				sb.Append(item);
				sb.Append(",");
			}
			sb.Remove(sb.Length - 1, 1); // removing last ","
			sb.Append(" FROM " + ReportDefinitionReportType.AD_PERFORMANCE_REPORT.ToString());
			sb.Append(" DURING YESTERDAY ");
		  //  string query = "SELECT CampaignId, AdGroupId, Id, Criteria, CriteriaType, Impressions, " +
		  //"Clicks, Cost FROM CRITERIA_PERFORMANCE_REPORT WHERE Status IN [ACTIVE, PAUSED] " +
		  //"DURING LAST_7_DAYS";

			string filePath = "D:\\" +  fileName;

			try
			{
				ReportUtilities utilities = new ReportUtilities(user);
				utilities.ReportVersion = "v201302";
				utilities.DownloadClientReport(sb.ToString(), DownloadFormat.GZIPPED_CSV.ToString(), filePath);
				Console.WriteLine("Report was downloaded to '{0}'.", filePath);
			}
			catch (Exception ex)
			{
				throw new System.ApplicationException("Failed to download report.", ex);
			}
		}
		
	}
}
