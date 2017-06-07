using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text;

public static class MySqlUtil
{
	public enum MySqlUtilResult
	{
		eOK					= 1,
		eConnectionError	= 2,
	}

	////////////////////////////////////////
	// variables
	////////////////////////////////////////
	private static MySqlConnection		_cConnection				= null;
	private static List<string>			_sListConnectionFailError	= new List<string>();
	private static string				_sTableName					= "*";

	////////////////////////////////////////
	// get, set
	////////////////////////////////////////
	public static MySqlConnection		Connection
	{
		get
		{
			ConnectToMySqlServer();
			return _cConnection;
		}
	}

	public static string TableName
	{
		get { return _sTableName; }
	}

	////////////////////////////////////////
	// functions
	////////////////////////////////////////
	public static string ReConnectionDB()
	{
		string sError = "";
		MySqlUtilResult eResult = ConnectToDB(ref sError);

		if (MySqlUtilResult.eOK == eResult)
			return "OK";

		return "Error - " + sError;
	}

	public static Enums.ErrorCode GetAccountIDWhereSNSID(string sFieldName, string sFacebookID, out int rOutAccountID, ref string rError)
	{
		Shik.LastUpdate();

		rOutAccountID = -1;

		MySqlCommand cCommand	= new MySqlCommand();
		cCommand.Connection		= Connection;
		cCommand.CommandText	= string.Format("SELECT id FROM game WHERE {0}=\"{1}\";", sFieldName, sFacebookID);

		try
		{
			object c = cCommand.ExecuteScalar();
			Int32 iTemp = Convert.ToInt32(c.ToString());
			rOutAccountID = (int)iTemp;
		}
		catch (Exception cEx)
		{
			cCommand.Dispose();
			rError = cEx.ToString();
			return Enums.ErrorCode.eDB_ExecuteError;
		}

		cCommand.Dispose();
		return Enums.ErrorCode.eNone;
	}

	public static Enums.ErrorCode CreateNewRecord(Player.Instance cPlayer, ref string rError)
	{
		Shik.LastUpdate();

		string sInsertInto	= "INSERT INTO " + _sTableName + "(*, *, *, *, *, *, *, *, *, *, *, *, *, *, *, *, *, *, *, *) ";
		string sValues		= "VALUES (@*, @*, @*, @*, @*, @*, @*, @*, @*, @*, @*, @*, @*, @*, @*, @*, @*, @*, @*, @*)";

		MySqlCommand cCommand = new MySqlCommand();
		cCommand.Connection = Connection;
		cCommand.CommandText = sInsertInto + sValues;

		cCommand.Parameters.Add("@*",	MySqlDbType.VarChar);
		cCommand.Parameters.Add("@*",	MySqlDbType.VarChar);
		cCommand.Parameters.Add("@*",	MySqlDbType.VarChar);
		cCommand.Parameters.Add("@*",	MySqlDbType.VarChar);
		cCommand.Parameters.Add("@*",	MySqlDbType.Text);
		cCommand.Parameters.Add("@*",	MySqlDbType.Text);
		cCommand.Parameters.Add("@*",	MySqlDbType.Text);
		cCommand.Parameters.Add("@*",	MySqlDbType.Text);
		cCommand.Parameters.Add("@*",	MySqlDbType.Text);
		cCommand.Parameters.Add("@*",	MySqlDbType.Text);
		cCommand.Parameters.Add("@*",	MySqlDbType.Text);
		cCommand.Parameters.Add("@*",	MySqlDbType.Text);
		cCommand.Parameters.Add("@*",	MySqlDbType.Text);
		cCommand.Parameters.Add("@*",	MySqlDbType.Text);
		cCommand.Parameters.Add("@*",	MySqlDbType.Text);
		cCommand.Parameters.Add("@*",	MySqlDbType.Text);
		cCommand.Parameters.Add("@*",	MySqlDbType.Text);
		cCommand.Parameters.Add("@*",	MySqlDbType.Date);
		cCommand.Parameters.Add("@*",	MySqlDbType.Date);
		cCommand.Parameters.Add("@*",	MySqlDbType.Bit);

		cCommand.Parameters[0].Value	= cPlayer.SuperBitID;
		cCommand.Parameters[1].Value	= cPlayer.FacebookID;
		cCommand.Parameters[2].Value	= cPlayer.GooglePlusID;
		cCommand.Parameters[3].Value	= cPlayer.GamecenterID;
		cCommand.Parameters[4].Value	= cPlayer.AccountInstance.ToJsonToken();
		cCommand.Parameters[5].Value	= cPlayer.MoneysInstance.ToJsonToken();
		cCommand.Parameters[6].Value	= cPlayer.ScoreInstance.ToJsonToken();
		cCommand.Parameters[7].Value	= cPlayer.AchievementsInstance.ToJsonToken();
		cCommand.Parameters[8].Value	= cPlayer.CharacterInstance.ToJsonToken();
		cCommand.Parameters[9].Value	= cPlayer.CookingInstance.ToJsonToken();
		cCommand.Parameters[10].Value	= cPlayer.IngredientsInstance.ToJsonToken();
		cCommand.Parameters[11].Value	= cPlayer.CookingLevelInstance.ToJsonToken();
		cCommand.Parameters[12].Value	= cPlayer.MissionInstance.ToJsonToken();
		cCommand.Parameters[13].Value	= cPlayer.PlayerLevelInstance.ToJsonToken();
		cCommand.Parameters[14].Value	= cPlayer.PlayerSkillInstance.ToJsonToken();
		cCommand.Parameters[15].Value	= cPlayer.ShopInstance.ToJsonToken();
		cCommand.Parameters[16].Value	= cPlayer.TutorialInstance.ToJsonToken();
		cCommand.Parameters[17].Value	= DateTimeUtil.ToSqlDate(DateTime.Now);
		cCommand.Parameters[18].Value	= DateTimeUtil.ToSqlDate(DateTime.Now);
		cCommand.Parameters[19].Value	= 0;
		
		int iResult = 0;
		
		try
		{
			//Debug.Log("    - [MySqlUtil : CreateNewRecord] open : " + cPlayer.AccountID.ToString());
			iResult = cCommand.ExecuteNonQuery();
			//Debug.Log("    - [MySqlUtil : CreateNewRecord] close : " + cPlayer.AccountID.ToString());
		}
		catch (Exception ce)
		{
			rError = ce.ToString();
			cCommand.Dispose();
			return Enums.ErrorCode.eDB_ExecuteError;
		}

		cCommand.Dispose();

		if (!iResult.Equals(1))
			return Enums.ErrorCode.eDB_FailedInsert;

		return Enums.ErrorCode.eNone;
	}

	public static Enums.ErrorCode RemoveRecord(int iAccountID, ref string rError)
	{
		Shik.LastUpdate();

		MySqlCommand cCommand = new MySqlCommand();
		cCommand.Connection  = Connection;
		cCommand.CommandText = "DELETE FROM " + _sTableName + " WHERE id=@AccountID;";

		cCommand.Parameters.Add("@AccountID", MySqlDbType.Int32, 10);
		cCommand.Parameters["@AccountID"].Value = iAccountID;

		try
		{
			//Debug.Log("    - [MySqlUtil : RemoveRecord] open : " + iAccountID.ToString());
			int iResult = cCommand.ExecuteNonQuery();
			if (!iResult.Equals(1))
			{
				cCommand.Dispose();
				//Debug.Log("    - [MySqlUtil : RemoveRecord] close : " + iAccountID.ToString());
				return Enums.ErrorCode.eDB_ExecuteError;
			}
		}
		catch (Exception ce)
		{
			rError = ce.ToString();
			cCommand.Dispose();
			//Debug.Log("    - [MySqlUtil : RemoveRecord] close : " + iAccountID.ToString());
			return Enums.ErrorCode.eDB_ExecuteError;
		}

		cCommand.Dispose();
		//Debug.Log("    - [MySqlUtil : RemoveRecord] close : " + iAccountID.ToString());
		return Enums.ErrorCode.eNone;
	}

	public static Enums.ErrorCode FillPlayer(Player.Instance cPlayer, ref string rError)
	{
		Shik.LastUpdate();

		string sCommand = "";
		if ("" != cPlayer.SuperBitID)
			sCommand = string.Format("SELECT * FROM " + _sTableName + " WHERE *='{0}'", cPlayer.SuperBitID);
		else if ("" != cPlayer.FacebookID)
			sCommand = string.Format("SELECT * FROM " + _sTableName + " WHERE *='{0}'", cPlayer.FacebookID);
		else if ("" != cPlayer.GooglePlusID)
			sCommand = string.Format("SELECT * FROM " + _sTableName + " WHERE *='{0}'", cPlayer.GooglePlusID);

		MySqlDataAdapter	cAdapter	= new MySqlDataAdapter(sCommand, Connection);
		DataSet				cDataSet	= new DataSet();

		try
		{
			cAdapter.Fill(cDataSet);
		}
		catch (Exception ce)
		{
			rError = ce.ToString();
			cAdapter.Dispose();
			return Enums.ErrorCode.eDB_ExecuteError;
		}

		if (cDataSet.Tables.Count.Equals(0) || cDataSet.Tables[0].Rows.Count.Equals(0))
		{
			rError = "Do not found recode";
			cAdapter.Dispose();
			return Enums.ErrorCode.eDB_DoNotFoundRecord;
		}

		cPlayer.FillFromDataRow(cDataSet.Tables[0].Rows[0]);
		cAdapter.Dispose();
		
		return Enums.ErrorCode.eNone;
	}

	public static Enums.ErrorCode GetRankingData(out DataSet cDataSet)
	{
		Shik.LastUpdate();

		string sCommand = "select " + _sTableName + ".*, " + _sTableName + ".*, " + _sTableName + ".*, " + _sTableName + ".*, " + _sTableName + ".*, " + _sTableName + ".* from " + _sTableName + " where not disable=1;";

		MySqlDataAdapter cAdapter = new MySqlDataAdapter(sCommand, Connection);
		cDataSet = new DataSet();

		try
		{
			cAdapter.Fill(cDataSet);
		}
		catch (Exception ce)
		{
			FileLogUtil.Log("ERROR - GetRankingData" + ce.Message);
			cAdapter.Dispose();
			return Enums.ErrorCode.eDB_ExecuteError;
		}

		cAdapter.Dispose();
		return Enums.ErrorCode.eNone;
	}

	public static Enums.ErrorCode GetAccountID(Player.Instance cPlayer, ref string rError)
	{
		Shik.LastUpdate();

		MySqlCommand cCommand = new MySqlCommand();
		cCommand.Connection = Connection;
		cCommand.CommandText = string.Format("SELECT id from " + _sTableName + " where *='{0}'", cPlayer.SuperBitID);

		try
		{
			object cResult = cCommand.ExecuteScalar();
			cPlayer.AccountID = Convert.ToInt32(cResult);
		}
		catch (Exception ce)
		{
			rError = ce.ToString();
			cCommand.Dispose();
			return Enums.ErrorCode.eDB_ExecuteError;
		}

		return Enums.ErrorCode.eNone;
	}

	public static Enums.ErrorCode UpdateFinishGame(Player.Instance cPlayer, ref string rError)
	{
		Shik.LastUpdate();

		MySqlCommand cCommand = new MySqlCommand();
		cCommand.Connection = Connection;
		cCommand.CommandText = string.Format("UPDATE " + _sTableName + " SET *='{0}', *='{1}', *='{2}', *='{3}', *='{4}' WHERE id={5}",
											 cPlayer.ScoreInstance.ToJsonToken(),
											 cPlayer.MoneysInstance.ToJsonToken(),
											 cPlayer.PlayerLevelInstance.ToJsonToken(),
											 cPlayer.IngredientsInstance.ToJsonToken(),
											 DateTimeUtil.ToSqlDate(DateTime.Now),
											 cPlayer.AccountID);

		try
		{
			//Debug.Log("    - [MySqlUtil : UpdateFinishGame] open : " + cPlayer.AccountID.ToString());
			int iResult = cCommand.ExecuteNonQuery();
			if (!iResult.Equals(1))
			{
				cCommand.Dispose();
				//Debug.Log("    - [MySqlUtil : UpdateFinishGame] close : " + cPlayer.AccountID.ToString());
				return Enums.ErrorCode.eDB_ExecuteError;
			}
		}
		catch (Exception ce)
		{
			//Debug.Log("    - [MySqlUtil : UpdateFinishGame] Exception : + " + ce.ToString());

			rError = ce.ToString();
			cCommand.Dispose();
			//Debug.Log("    - [MySqlUtil : UpdateFinishGame] close : " + cPlayer.AccountID.ToString());
			return Enums.ErrorCode.eDB_ExecuteError;
		}

		cCommand.Dispose();
		//Debug.Log("    - [MySqlUtil : UpdateFinishGame] close : " + cPlayer.AccountID.ToString());
		return Enums.ErrorCode.eNone;
	}

	public static Enums.ErrorCode UpdateField(int iAccountID, List<BaseClass.Instance> cList, ref string rError)
	{
		Shik.LastUpdate();

		MySqlCommand cCommand = new MySqlCommand();

		StringBuilder cBuilder = new StringBuilder();
		cBuilder.Append("UPDATE " + _sTableName + " SET ");
		
		StringBuilder cDebugBuilder = new StringBuilder();
		
		int iSize = cList.Count;

		for (int i = 0; i < iSize; ++i)
		{
			if (0 < i)
				cBuilder.Append(", ");
			cBuilder.Append(cList[i].FieldName + "=@" + cList[i].FieldName);

			cCommand.Parameters.Add("@" + cList[i].FieldName, cList[i].MySqlDBType);
			cCommand.Parameters[i].Value = cList[i].ToJsonTokenForDB();

			cDebugBuilder.Append(cList[i].FieldName + ", ");
		}

		cBuilder.Append(", *=@* WHERE *=@*;");

		cCommand.Parameters.Add("@*", MySqlDbType.DateTime);
		cCommand.Parameters.Add("@*", MySqlDbType.Int32, 10);

		cCommand.Parameters[iSize].Value		= DateTimeUtil.ToSqlDate(DateTime.Now);
		cCommand.Parameters[iSize + 1].Value	= iAccountID;

		cCommand.Connection = Connection;
		cCommand.CommandText = cBuilder.ToString();

		try
		{
			//Debug.Log("    - [MySqlUtil : UpdateFiled] open : " + cDebugBuilder.ToString());
			int iResult = cCommand.ExecuteNonQuery();
			if (!iResult.Equals(1))
			{
				cCommand.Dispose();
				//Debug.Log("    - [MySqlUtil : UpdateFiled] close : " + cDebugBuilder.ToString());
				return Enums.ErrorCode.eDB_ExecuteError;
			}
		}
		catch (Exception ce)
		{
			rError = ce.ToString();
			cCommand.Dispose();
			//Debug.Log("    - [MySqlUtil : UpdateFiled] close : " + cDebugBuilder.ToString());
			return Enums.ErrorCode.eDB_ExecuteError;
		}

		cCommand.Dispose();
		//Debug.Log("    - [MySqlUtil : UpdateFiled] close : " + cDebugBuilder.ToString());
		return Enums.ErrorCode.eNone;
	}

	public static Enums.ErrorCode UpdateSNSID(int iAccountID, string sFieldName, string sValue, ref string rError)
	{
		Shik.LastUpdate();

		MySqlCommand cCommand = new MySqlCommand();
		cCommand.Connection = Connection;
		cCommand.CommandText = string.Format("UPDATE " + _sTableName + " SET {0}=@s{1}, *=@* WHERE *=@*;", sFieldName, sFieldName);

		cCommand.Parameters.Add("@*" + sFieldName, MySqlDbType.VarChar);
		cCommand.Parameters.Add("@*", MySqlDbType.DateTime);
		cCommand.Parameters.Add("@*", MySqlDbType.Int32, 10);

		cCommand.Parameters[0].Value = sValue;
		cCommand.Parameters[1].Value = DateTimeUtil.ToSqlDate(DateTime.Now);
		cCommand.Parameters[2].Value = iAccountID;

		try
		{
			//Debug.Log("    - [MySqlUtil : UpdateSNSID] open : " + iAccountID.ToString());
			int iResult = cCommand.ExecuteNonQuery();
			if (!iResult.Equals(1))
			{
				cCommand.Dispose();
				//Debug.Log("    - [MySqlUtil : UpdateSNSID] close : " + iAccountID.ToString());
				return Enums.ErrorCode.eDB_ExecuteError;
			}
		}
		catch (Exception ce)
		{
			rError = ce.ToString();
			cCommand.Dispose();
			//Debug.Log("    - [MySqlUtil : UpdateSNSID] close : " + iAccountID.ToString());
			return Enums.ErrorCode.eDB_ExecuteError;
		}

		cCommand.Dispose();
		return Enums.ErrorCode.eNone;
	}

	public static void WriteLog(int iAccountID, Enums.LogMainType eMainType, Enums.LogSubType eSubType, string sIDParam = "", string sBeforeValue = "", string sNextValue = "")
	{
		MySqlCommand cCommand = new MySqlCommand();
		cCommand.Connection = Connection;

		if (null == _cConnection)
			return;

		cCommand.CommandText = "INSERT INTO *(*, *, *, *, *, *, *, *) VALUES (@*, @*, @*, @*, @*, @*, @*, @*);";
		cCommand.Parameters.Add("@*", MySqlDbType.Int32, 10);
		cCommand.Parameters.Add("@*", MySqlDbType.VarChar);
		cCommand.Parameters.Add("@*", MySqlDbType.VarChar);
		cCommand.Parameters.Add("@*", MySqlDbType.VarChar);
		cCommand.Parameters.Add("@*", MySqlDbType.VarChar);
		cCommand.Parameters.Add("@*", MySqlDbType.VarChar);
		cCommand.Parameters.Add("@*", MySqlDbType.Date);
		cCommand.Parameters.Add("@*", MySqlDbType.Time);

		cCommand.Parameters[0].Value = iAccountID;
		cCommand.Parameters[1].Value = eMainType.ToString();
		cCommand.Parameters[2].Value = eSubType.ToString();
		cCommand.Parameters[3].Value = sIDParam;
		cCommand.Parameters[4].Value = sBeforeValue;
		cCommand.Parameters[5].Value = sNextValue;
		cCommand.Parameters[6].Value = DateTimeUtil.ToSqlDate(DateTime.Now);
		cCommand.Parameters[7].Value = DateTimeUtil.ToSqlTime(DateTime.Now);

		try
		{
			//Debug.Log("    - [MySqlUtil : WriteLog] open : " + iAccountID.ToString());
			cCommand.ExecuteNonQuery();
		}
		catch (Exception ce)
		{
			FileLogUtil.Log("[MySqlUtil : WriteLog] Exception : " + ce.ToString());
			cCommand.Dispose();
			//Debug.Log("    - [MySqlUtil : WriteLog] close : " + iAccountID.ToString());
			return;
		}

		cCommand.Dispose();
		//Debug.Log("    - [MySqlUtil : WriteLog] close : " + iAccountID.ToString());
	}

	public static List<string> ConnectionFailErrorMessageList()
	{
		return _sListConnectionFailError;
	}

	public static void ClearConnectionFailErrorMessages()
	{
		_sListConnectionFailError.Clear();
	}

	public static MySqlUtilResult ConnectToDB(ref string sError)
	{
		_sTableName = "G";
		if (Enums.DBTableType.eTestGame == ServerState.Manager.DBTableType)
			_sTableName = "T";

		string sConnectionString = "";

		switch ((Enums.DBServerType)ServerState.Manager.DBType)
		{
			case Enums.DBServerType.A:
				sConnectionString += @"*";
				sConnectionString += @"*";
				sConnectionString += @"*";
				sConnectionString += @"*";
				sConnectionString += @"*";
				sConnectionString += @"*";
				sConnectionString += @"*";
				break;

			case Enums.DBServerType.B:
				sConnectionString += @"*";
				sConnectionString += @"*";
				sConnectionString += @"*";
				sConnectionString += @"*";
				sConnectionString += @"*";
				sConnectionString += @"*";
				sConnectionString += @"*";
				break;

			case Enums.DBServerType.C:
				sConnectionString += @"*";
				sConnectionString += @"*";
				sConnectionString += @"*";
				sConnectionString += @"*";
				sConnectionString += @"*";
				sConnectionString += @"*";
				sConnectionString += @"*";
				break;
		}

		_cConnection = new MySqlConnection(sConnectionString);

		try
		{
			_cConnection.Open();
		}
		catch (Exception c)
		{
			_sListConnectionFailError.Add(c.ToString());
			_cConnection = null;
			sError = c.ToString();
			return MySqlUtilResult.eConnectionError;
		}

		FileLogUtil.Log("Reconnection DB - DBType : " + ServerState.Manager.DBType.ToString() + ", DBTableType : " + _sTableName);

		return MySqlUtilResult.eOK;
	}

	private static void ConnectToMySqlServer()
	{
		if (null != _cConnection && _cConnection.State != ConnectionState.Closed)
			return;

		// 3번 트라이 한다
		string sError = "";

		ConnectToDB(ref sError);
		if (null == _cConnection || _cConnection.State == ConnectionState.Closed)
		{
			ConnectToDB(ref sError);

			if (null == _cConnection || _cConnection.State == ConnectionState.Closed)
				ConnectToDB(ref sError);
		}
	}
}