﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Db4objects.Db4o;
using Db4objects.Db4o.CS;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Linq;
using Edge.Core.Configuration;
using Edge.Core.Utilities;
using Edge.Core;
using System.Diagnostics;
using System.IO;
using System.Data.Common;
using System.Data.SqlClient;
using Edge.Core.Services;
using Edge.Core.Data;
using Edge.Data.Objects;
using System.Xml.Serialization;
using System.Xml;
using System.Data.SqlTypes;
using System.Runtime.Serialization;
using System.Collections;
using Newtonsoft.Json;




namespace Edge.Data.Pipeline
{
    internal class DeliveryDB
    {
        private static class ResultSetIndex
        {
            public const int Delivery = 0;
            public const int DeliveryParameters = 1;
            public const int DeliveryHistory = 2;
            public const int DeliveryHistoryParameters = 3;
            public const int DeliveryFile = 4;
            public const int DeliveryFileParameters = 5;
        }

        internal static Delivery Get(Guid deliveryID, bool deep = true, SqlConnection connection = null)
        {
            Delivery delivery = null;
            bool innerConnection = connection == null;

            if (innerConnection)
                connection = DeliveryDBClient.Connect();

            try
            {
                SqlCommand cmd = DataManager.CreateCommand("Delivery_Get(@deliveryID:Char, @deep:bit)", System.Data.CommandType.StoredProcedure);
                cmd.Connection = connection;
                cmd.Parameters["@deliveryID"].Value = deliveryID.ToString("N");
                cmd.Parameters["@deep"].Value = deep;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        #region Delivery

                        delivery = new Delivery(reader.Convert<string, Guid>("DeliveryID", s => Guid.Parse(s)));

                        delivery.Account = reader.Convert<int?, Account>("AccountID", id => id.HasValue ? new Account() { ID = id.Value } : null);
                        delivery.Channel = reader.Convert<int?, Channel>("ChannelID", id => id.HasValue ? new Channel() { ID = id.Value } : null);
                        delivery.Account.OriginalID = reader["OriginalID"].ToString();
                        //delivery.DateCreated=DateTime.Parse(reader["DateCreated"].ToString());
                        //delivery.DateModified=DateTime.Parse(reader["DateModified"].ToString());
                        delivery.Signature = reader["Signature"].ToString();
                        delivery.Description = reader["Description"].ToString();
                        delivery.TargetLocationDirectory = reader["TargetLocationDirectory"].ToString();
                        delivery.TargetPeriod = DateTimeRange.Parse(reader["TargetPeriodDefinition"].ToString());
                        //delivery.TargetPeriodStart=DateTime.Parse(reader["TargetPeriodStart"].ToString());
                        //delivery.TargetPeriodEnd=DateTime.Parse(reader["TargetPeriodEnd"].ToString());
                        #endregion
                        #region DeliveryParameters


                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {

                                delivery.Parameters.Add(reader["Key"].ToString(), DeSerialize(reader["Value"].ToString()));
                            }
                        }
                        #endregion
                        #region DeliveryHistory

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                                delivery.History.Add(new DeliveryHistoryEntry((DeliveryOperation)reader["Operation"], Convert.ToInt64(reader["ServiceInstanceID"]), new Dictionary<string, object>()));

                        }
                        #endregion
                        #region DeliveryHistoryParameters
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {

                                delivery.History[reader.Get<int>("Index")].Parameters.Add(reader["Key"].ToString(), DeSerialize(reader["Value"].ToString()));

                            }
                        }
                        #endregion
                        #region DeliveryFile

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                DeliveryFile deliveryFile = new DeliveryFile();
                                deliveryFile.Account = reader.Convert<int?, Account>("AccountID", id => id.HasValue ? new Account() { ID = id.Value } : null);
                                deliveryFile.FileID = reader.Convert<string, Guid>("DeliveryID", s => Guid.Parse(s));
                                deliveryFile.FileFormat = (FileCompression)reader["FileCompression"];
                                deliveryFile.SourceUrl = reader["SourceUrl"].ToString();
                                deliveryFile.Name = reader["Name"].ToString();
                                deliveryFile.Location = reader["Location"].ToString();
                                delivery.Files.Add(deliveryFile);
                            }

                        }
                        #endregion
                        #region DeliveryFileParameters

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {

                                DeliveryFile deliveryFile = delivery.Files[reader["Name"].ToString()];
                                deliveryFile.Parameters.Add(reader["Key"].ToString(), DeSerialize(reader["Value"].ToString()));
                            }

                        }
                        #endregion
                        #region DeliveryFileHistory

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                DeliveryFile deliveryFile = delivery.Files[reader["Name"].ToString()];
                                deliveryFile.History.Add(new DeliveryHistoryEntry((DeliveryOperation)reader["Operation"], Convert.ToInt64(reader["ServiceInstanceID"])));
                            }


                        }
                        #endregion
                        #region DeliveryFileHistoryParameters

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                DeliveryFile deliveryFile = delivery.Files[reader["Name"].ToString()];

                                deliveryFile.History[reader.Get<int>("Index")].Parameters.Add(reader["Key"].ToString(), reader.Get<object>("Value"));
                            }


                        }
                        #endregion





                    }
                }

            }
            finally
            {
                if (innerConnection)
                {
                    connection.Dispose();
                }
            }
            /*
            #if DEBUG
                        Log.Write(String.Format("Delivery - {3}found: {0} (activate: {2}, {1} results)\n", deliveryID, resultCount, activate, delivery == null ? "not " : "" ), LogMessageType.Information);

            #endif			
            */
            return delivery;
        }

        private static object DeSerialize(string json)
        {
            object toReturn = null;
            JsonSerializerSettings s = new JsonSerializerSettings();
            s.TypeNameHandling = TypeNameHandling.All;
            
            toReturn = JsonConvert.DeserializeObject(json, s);
            //if (toReturn is Newtonsoft.Json.Linq.JObject)
            //{
            //    toReturn = JsonConvert.DeserializeObject < Dictionary<string, double>>(json);
            //}



            return toReturn;


        }

        internal static Guid Save(Delivery delivery)
        {

            using (var client = DeliveryDBClient.Connect())
            {
                SqlTransaction transaction = client.BeginTransaction();
                Guid guid = delivery.DeliveryID;

                if (guid != Guid.Empty)
                {
                    #region Delete all Delivery
                    SqlCommand cmd = new SqlCommand("Delivery_Delete");

                    cmd.Connection = client;
                    cmd.Transaction = transaction;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@deliveryID", System.Data.SqlDbType.Char);
                    cmd.Parameters["@deliveryID"].Value = delivery.DeliveryID.ToString("N");
                    cmd.ExecuteNonQuery();
                    #endregion

                    #region Delivery
                    cmd = new SqlCommand(@"INSERT INTO [Edge_System].[dbo].[Delivery]
	       ([DeliveryID]
	       ,[AccountID]
	       ,[ChannelID]
		   ,[OriginalID]
	       ,[DateCreated]
	       ,[DateModified]
	       ,[Signature]
	       ,[Description]
	       ,[TargetLocationDirectory]
	       ,[TargetPeriodDefinition]
	       ,[TargetPeriodStart]
	       ,[TargetPeriodEnd])
	 VALUES
	       (@deliveryID,
	        @accountID,
	        @channelID,
		    @originalID,
	        @dateCreated,
	        @dateModified,
	        @signature,
	        @description,
	        @targetLocationDirectory,
	        @targetPeriodDefinition,
	        @targetPeriodStart,
	        @targetPeriodEnd)", client, transaction);

                    cmd.Connection = client;
                    cmd.Parameters.Add("@deliveryID", System.Data.SqlDbType.Char);
                    cmd.Parameters.Add("@accountID", System.Data.SqlDbType.Int);
                    cmd.Parameters.Add("@channelID", System.Data.SqlDbType.Int);
                    cmd.Parameters.Add("@originalID", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters.Add("@dateCreated", System.Data.SqlDbType.DateTime);
                    cmd.Parameters.Add("@dateModified", System.Data.SqlDbType.DateTime);
                    cmd.Parameters.Add("@signature", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters.Add("@description", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters.Add("@targetLocationDirectory", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters.Add("@targetPeriodDefinition", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters.Add("@targetPeriodStart", System.Data.SqlDbType.DateTime2); //must be date time since sql round or somthing, leave it!!
                    cmd.Parameters.Add("@targetPeriodEnd", System.Data.SqlDbType.DateTime2);//must be date time since sql round or somthing, leave it!!

                    cmd.Parameters["@deliveryID"].Value = delivery.DeliveryID.ToString("N");
                    cmd.Parameters["@accountID"].Value = delivery.Account.ID;
                    cmd.Parameters["@channelID"].Value = delivery.Channel.ID; ;
                    cmd.Parameters["@originalID"].Value = delivery.Account.OriginalID == null ? (object)DBNull.Value : delivery.Account.OriginalID;
                    cmd.Parameters["@dateCreated"].Value = delivery.DateCreated;
                    cmd.Parameters["@dateModified"].Value = delivery.DateModified;
                    cmd.Parameters["@signature"].Value = delivery.Signature;
                    cmd.Parameters["@description"].Value = delivery.Description == null ? (object)DBNull.Value : delivery.Description;
                    cmd.Parameters["@targetLocationDirectory"].Value = delivery.TargetLocationDirectory;
                    cmd.Parameters["@targetPeriodDefinition"].Value = delivery.TargetPeriod.ToString();
                    cmd.Parameters["@targetPeriodStart"].Value = delivery.TargetPeriodStart;
                    cmd.Parameters["@targetPeriodEnd"].Value = delivery.TargetPeriodEnd;

                    cmd.ExecuteNonQuery();
                    #endregion

                    #region DeliveryParameters

                    foreach (KeyValuePair<string, object> param in delivery.Parameters)
                    {

                        cmd = new SqlCommand(@"INSERT INTO [Edge_System].[dbo].[DeliveryParameters]
										([DeliveryID]
										,[Key]
										,[Value])
										 VALUES
									    (@deliveryID
									    ,@key
										,@value)");
                        cmd.Connection = client;
                        cmd.Transaction = transaction;
                        cmd.Parameters.Add("@deliveryID", System.Data.SqlDbType.Char);
                        cmd.Parameters.Add("@key", System.Data.SqlDbType.NVarChar);
                        cmd.Parameters.Add("@value", System.Data.SqlDbType.NVarChar);
                        cmd.Parameters["@deliveryID"].Value = delivery.DeliveryID.ToString("N");
                        cmd.Parameters["@key"].Value = param.Key;

                        cmd.Parameters["@value"].Value = Serialize(param.Value);
                        cmd.ExecuteNonQuery();

                    }
                    #endregion

                    #region DeliveryHistory

                    int index = 0;
                    foreach (DeliveryHistoryEntry historyEntry in delivery.History)
                    {
                        cmd = new SqlCommand(@"INSERT INTO [Edge_System].[dbo].[DeliveryHistory]
											   ([DeliveryID]
											   ,[ServiceInstanceID]
											   ,[Index]
											   ,[Operation]
											   ,[DateRecorded])
												 VALUES
											   (@deliveryID
											   ,@serviceInstanceID
											   ,@index
											   ,@operation
											   ,@dateRecorded)");
                        cmd.Connection = client;
                        cmd.Transaction = transaction;
                        cmd.Parameters.Add("@deliveryID", System.Data.SqlDbType.Char);
                        cmd.Parameters.Add("@serviceInstanceID", System.Data.SqlDbType.BigInt);
                        cmd.Parameters.Add("@index", System.Data.SqlDbType.Int);
                        cmd.Parameters.Add("@operation", System.Data.SqlDbType.Int);
                        cmd.Parameters.Add("@dateRecorded", System.Data.SqlDbType.DateTime);


                        cmd.Parameters["@deliveryID"].Value = delivery.DeliveryID.ToString("N");
                        cmd.Parameters["@index"].Value = index;
                        cmd.Parameters["@operation"].Value = historyEntry.Operation;
                        cmd.Parameters["@dateRecorded"].Value = historyEntry.DateRecorded;
                        cmd.Parameters["@serviceInstanceID"].Value = historyEntry.ServiceInstanceID;
                        cmd.ExecuteNonQuery();

                        index++;


                    }
                    #endregion

                    #region DeliveryHistoryParameters
                    index = 0;
                    if (delivery.History != null)
                    {
                        foreach (DeliveryHistoryEntry historyEntry in delivery.History)
                        {
                            if (historyEntry.Parameters != null)
                            {
                                foreach (KeyValuePair<string, object> param in historyEntry.Parameters)
                                {

                                    cmd = new SqlCommand(@"INSERT INTO [Edge_System].[dbo].[DeliveryHistoryParameters]
											   ([DeliveryID]
											   ,[Index]
											   ,[Key]
											   ,[Value])
											  VALUES
											   (@deliveryID
											   ,@index
											   ,@key
											   ,@value)");
                                    cmd.Connection = client;
                                    cmd.Transaction = transaction;
                                    cmd.Parameters.Add("@deliveryID", System.Data.SqlDbType.Char);
                                    cmd.Parameters.Add("@index", System.Data.SqlDbType.Int);
                                    cmd.Parameters.Add("@key", System.Data.SqlDbType.NVarChar);
                                    cmd.Parameters.Add("@value", System.Data.SqlDbType.NVarChar);


                                    cmd.Parameters["@deliveryID"].Value = delivery.DeliveryID.ToString("N");
                                    cmd.Parameters["@index"].Value = index;
                                    cmd.Parameters["@key"].Value = param.Key;
                                    cmd.Parameters["@value"].Value = Serialize(param.Value);


                                    cmd.ExecuteNonQuery();

                                }
                            }
                            index++;
                        }
                    }
                    #endregion

                    #region DeliveryFile
                    foreach (DeliveryFile file in delivery.Files)
                    {
                        if (file.FileID == Guid.Empty)
                            file.FileID = Guid.NewGuid();
                        cmd = new SqlCommand(@"INSERT INTO [Edge_System].[dbo].[DeliveryFile]
											   ([DeliveryID]
											   ,[FileID]
											   ,[Name]
											   ,[AccountID]
											   ,[ChannelID]
											   ,[DateCreated]
											   ,[DateModified]
											   ,[FileCompression]
											   ,[SourceUrl]
											   ,[Location])
										 VALUES
											   (@deliveryID
											   ,@fileID
											   ,@name
											   ,@accountID
											   ,@channelID
											   ,@dateCreated
											   ,@dateModified
											   ,@fileCompression
											   ,@sourceUrl
											   ,@location)");
                        cmd.Connection = client;
                        cmd.Transaction = transaction;
                        cmd.Parameters.Add("@deliveryID", System.Data.SqlDbType.Char);
                        cmd.Parameters.Add("@fileID", System.Data.SqlDbType.Char);
                        cmd.Parameters.Add("@name", System.Data.SqlDbType.NVarChar);
                        cmd.Parameters.Add("@accountID", System.Data.SqlDbType.Int);
                        cmd.Parameters.Add("@channelID", System.Data.SqlDbType.Int);
                        cmd.Parameters.Add("@dateCreated", System.Data.SqlDbType.DateTime);
                        cmd.Parameters.Add("@dateModified", System.Data.SqlDbType.DateTime);
                        cmd.Parameters.Add("@fileCompression", System.Data.SqlDbType.Int);
                        cmd.Parameters.Add("@sourceUrl", System.Data.SqlDbType.NVarChar);
                        cmd.Parameters.Add("@location", System.Data.SqlDbType.NVarChar);


                        cmd.Parameters["@deliveryID"].Value = file.Delivery.DeliveryID.ToString("N");
                        cmd.Parameters["@fileID"].Value = file.FileID.ToString("N");
                        cmd.Parameters["@name"].Value = file.Name;
                        cmd.Parameters["@accountID"].Value = delivery.Account.ID;
                        cmd.Parameters["@channelID"].Value = delivery.Channel.ID;
                        cmd.Parameters["@dateCreated"].Value = file.DateCreated;
                        cmd.Parameters["@dateModified"].Value = file.DateModified;
                        cmd.Parameters["@fileCompression"].Value = file.FileFormat;
                        cmd.Parameters["@sourceUrl"].Value = file.SourceUrl == null ? (object)DBNull.Value : file.SourceUrl;
                        cmd.Parameters["@location"].Value = file.Location == null ? (object)DBNull.Value : file.Location;

                        cmd.ExecuteNonQuery();



                    }

                    #endregion

                    #region DeliveryFileParameters
                    foreach (DeliveryFile file in delivery.Files)
                    {
                        foreach (KeyValuePair<string, object> param in file.Parameters)
                        {
                            cmd = new SqlCommand(@"INSERT INTO [Edge_System].[dbo].[DeliveryFileParameters]
												   ([DeliveryID]
												   ,[Name]
												   ,[Key]
												   ,[Value])
											 VALUES
												   (@deliveryID
												   ,@name
												   ,@key
												   ,@value)");
                            cmd.Connection = client;
                            cmd.Transaction = transaction;
                            cmd.Parameters.Add("@deliveryID", System.Data.SqlDbType.Char);
                            cmd.Parameters.Add("@name", System.Data.SqlDbType.NVarChar);
                            cmd.Parameters.Add("@key", System.Data.SqlDbType.NVarChar);
                            cmd.Parameters.Add("@value", System.Data.SqlDbType.NVarChar);

                            cmd.Parameters["@deliveryID"].Value = delivery.DeliveryID.ToString("N");
                            cmd.Parameters["@name"].Value = file.Name;
                            cmd.Parameters["@key"].Value = param.Key;

                            cmd.Parameters["@value"].Value = Serialize(param.Value);
                            cmd.ExecuteNonQuery();


                        }
                    }


                    #endregion

                    #region DeliveryFileHistory

                    foreach (DeliveryFile file in delivery.Files)
                    {
                        index = 0;
                        if (file.History != null)
                        {
                            foreach (DeliveryHistoryEntry historyEntry in file.History)
                            {
                                cmd = new SqlCommand(@"INSERT INTO [Edge_System].[dbo].[DeliveryFileHistory]
											   ([DeliveryID]
											   ,[Name]
											   ,[ServiceInstanceID]
											   ,[Index]
											   ,[Operation]
											   ,[DateRecorded])
										 VALUES
											   (@deliveryID
											   ,@name
											   ,@serviceInstanceID
											   ,@index
											   ,@operation
											   ,@dateRecorded)");
                                cmd.Connection = client;
                                cmd.Transaction = transaction;
                                cmd.Parameters.Add("@deliveryID", System.Data.SqlDbType.Char);
                                cmd.Parameters.Add("@name", System.Data.SqlDbType.NVarChar);
                                cmd.Parameters.Add("@serviceInstanceID", System.Data.SqlDbType.BigInt);
                                cmd.Parameters.Add("@index", System.Data.SqlDbType.Int);
                                cmd.Parameters.Add("@operation", System.Data.SqlDbType.Int);
                                cmd.Parameters.Add("@dateRecorded", System.Data.SqlDbType.DateTime);


                                cmd.Parameters["@deliveryID"].Value = delivery.DeliveryID.ToString("N");
                                cmd.Parameters["@name"].Value = file.Name;
                                cmd.Parameters["@serviceInstanceID"].Value = historyEntry.ServiceInstanceID;
                                cmd.Parameters["@index"].Value = index;
                                cmd.Parameters["@operation"].Value = historyEntry.Operation;
                                cmd.Parameters["@dateRecorded"].Value = historyEntry.DateRecorded;

                                cmd.ExecuteNonQuery();




                                index++;

                            }
                        }

                    }

                    #endregion

                    #region DeliveryFileHistoryParameters
                    foreach (DeliveryFile file in delivery.Files)
                    {
                        index = 0;
                        if (file.History != null)
                        {
                            foreach (DeliveryHistoryEntry historyEntry in file.History)
                            {
                                if (historyEntry.Parameters != null)
                                {
                                    foreach (KeyValuePair<string, object> param in historyEntry.Parameters)
                                    {

                                        cmd = new SqlCommand(@"INSERT INTO [Edge_System].[dbo].[DeliveryFileHistoryParameters]
													   ([DeliveryID]
													   ,[Name]
													   ,[Index]
													   ,[Key]
													   ,[Value])
												 VALUES
													   (@deliveryID
													   ,@name
													   ,@index
													   ,@key
													   ,@value)");
                                        cmd.Connection = client;
                                        cmd.Transaction = transaction;
                                        cmd.Parameters.Add("@deliveryID", System.Data.SqlDbType.Char);
                                        cmd.Parameters.Add("@name", System.Data.SqlDbType.NVarChar);
                                        cmd.Parameters.Add("@index", System.Data.SqlDbType.Int);
                                        cmd.Parameters.Add("@key", System.Data.SqlDbType.NVarChar);
                                        cmd.Parameters.Add("@value", System.Data.SqlDbType.NVarChar);

                                        cmd.Parameters["@deliveryID"].Value = delivery.DeliveryID.ToString("N");
                                        cmd.Parameters["@name"].Value = file.Name;
                                        cmd.Parameters["@index"].Value = index;
                                        cmd.Parameters["@key"].Value = param.Key;

                                        cmd.Parameters["@value"].Value = Serialize(param.Value);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                                index++;

                            }
                        }
                    }

                    #endregion



                    transaction.Commit();



                }
                else
                {
                    guid = Guid.NewGuid();
                    throw new NotSupportedException("In Pipeline 2.9, you cannot save a Delivery without first giving it a GUID.");
                }



                return guid;
            }
        }

        private static string Serialize(object param)
        {
            JsonSerializerSettings s = new JsonSerializerSettings();
            s.TypeNameHandling = TypeNameHandling.All;

            return JsonConvert.SerializeObject(param, Newtonsoft.Json.Formatting.None, s);
        }

        internal static void Delete(Delivery delivery)
        {
            using (SqlConnection connection = DeliveryDBClient.Connect())
            {
                using (SqlCommand cmd = new SqlCommand("Delivery_Delete"))
                {
                    cmd.Connection = connection;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@deliveryID", System.Data.SqlDbType.Char);
                    cmd.Parameters["@deliveryID"].Value = delivery.DeliveryID.ToString("N");
                    cmd.ExecuteNonQuery();

                }
            }
        }

        internal static Delivery[] GetBySignature(string signature, Guid[] exclude)
        {
            List<Delivery> deliveries = new List<Delivery>();
            using (var client = DeliveryDBClient.Connect())
            {
                // Select deliveries that match a signature but none of the guids in 'exclude'
                using (SqlCommand cmd = DataManager.CreateCommand("Delivery_GetBySignature(@signature:NvarChar,@exclude:NvarChar)", System.Data.CommandType.StoredProcedure))
                {
                    cmd.Connection = client;
                    cmd.Parameters["@signature"].Value = "'" + signature + "'";
                    cmd.Parameters["@exclude"].Value = GetGuidStringArray(exclude); //TODO: Talk with doron it's not the way to do it
                    using (SqlDataReader reader = cmd.ExecuteReader())//TODO: Talk with doron it's not the way to do it
                    {
                        while (reader.Read())
                            deliveries.Add(Get(Guid.Parse(reader.GetString(0))));
                    }
                }
            }
            return deliveries.ToArray();
        }

        private static string GetGuidStringArray(Guid[] exclude)
        {
            StringBuilder guidArray = new StringBuilder();
            foreach (Guid guid in exclude)
                guidArray.AppendFormat(guidArray.Length == 0 ? "'{0}'" : ",'{0}'", guid.ToString("N"));

            return guidArray.ToString();
        }

        internal static Delivery[] GetByTargetPeriod(int channelID, int accountID, DateTime start, DateTime end)
        {
            List<Delivery> deliveries = new List<Delivery>();
            List<string> deliveriesId = new List<string>();

            using (var client = DeliveryDBClient.Connect())
            {
                using (SqlCommand cmd = DataManager.CreateCommand("Delivery_GetByTargetPeriod(@channelID:Int,@accountID:Int,@targetPeriodStart:DateTime2,@targetPeriodEnd:DateTime2)", System.Data.CommandType.StoredProcedure))
                {
                    cmd.Connection = client;
                    cmd.Parameters["@channelID"].Value = channelID;
                    cmd.Parameters["@accountID"].Value = accountID;
                    cmd.Parameters["@targetPeriodStart"].Value = start;
                    cmd.Parameters["@targetPeriodEnd"].Value = end;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            //deliveriesId.Add(Get(Guid.Parse(reader.GetString(0))));
                            deliveriesId.Add(reader.GetString(0));
                    }
                }
                foreach (string id in deliveriesId)
                {
                    deliveries.Add(Get(Guid.Parse(id)));
                }
                return deliveries.ToArray();
            }


        }

        internal static class DeliveryDBClient
        {
            public static SqlConnection Connect()
            {
                SqlConnection connection = new SqlConnection(AppSettings.GetConnectionString(typeof(Delivery), "DB"));
                connection.Open();
                return connection;
            }
        }
        [DataContract()]
        public class DataItem
        {
            [DataMember]
            public string Key;


            [DataMember]
            public object Value;



            public DataItem(string key, object value)
            {

                Key = key;

                Value = value;

            }
        }
    }
}
