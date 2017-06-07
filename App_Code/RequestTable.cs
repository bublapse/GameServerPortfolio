using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class RequestTable
{
	////////////////////////////////////////////////////////////////////////////////
	// Instance
	////////////////////////////////////////////////////////////////////////////////
	public class Instance
	{
		////////////////////////////////////////
		// variables
		////////////////////////////////////////
		private Dictionary<string, string>	_cDicRequest	= new Dictionary<string,string>();
		private string						_sEncryptKey	= "";

		////////////////////////////////////////
		// get, set
		////////////////////////////////////////
		public string		Key				{ get { if (_cDicRequest.ContainsKey("*")) return _cDicRequest["*"];				return ""; } }
		public string		SuperBitID		{ get { if (_cDicRequest.ContainsKey("*")) return _cDicRequest["*"];				return ""; } }
		public string		JsonToken		{ get { if (_cDicRequest.ContainsKey("*")) return _cDicRequest["*"];				return ""; } }
		public string		SParam			{ get { if (_cDicRequest.ContainsKey("*")) return _cDicRequest["*"];				return ""; } }
		public int			IParam			{ get { if (_cDicRequest.ContainsKey("*")) return Int32.Parse(_cDicRequest["*"]);	return -1; } }

		public string		EncryptKey		{ get { return _sEncryptKey; } }

		////////////////////////////////////////
		// functions
		////////////////////////////////////////
		public Instance(bool bOnlyEncrypt = true, bool bGETMode = false)
		{
			char[] sCharsToTrim = { ' ', '\'', '\"' };

			Dictionary<string, string> cDicTemp = new Dictionary<string,string>();

			// GET requests
			if (Manager.IsDebugMode || bGETMode)
			{
				foreach (String sKey in System.Web.HttpContext.Current.Request.QueryString.Keys)
				{
					String sValue = System.Web.HttpContext.Current.Request.QueryString[sKey].ToString().Trim();
					sValue = sValue.Trim(sCharsToTrim);

					cDicTemp.Add(sKey, sValue);
				}
			}

			// POST requests
			foreach (String sKey in System.Web.HttpContext.Current.Request.Form.Keys)
			{
				String sValue = System.Web.HttpContext.Current.Request.Form[sKey].ToString().Trim();
				sValue = sValue.Trim(sCharsToTrim);

				cDicTemp.Add(sKey, sValue);
			}

			if (bOnlyEncrypt)
				DecryptDic(cDicTemp);
			else
			{
				if (cDicTemp.ContainsKey("*"))
					DecryptDic(cDicTemp);
				else
					_cDicRequest = cDicTemp;
			}

			cDicTemp = null;
		}

		public string GetQueryString(string sKey)
		{
			if (_cDicRequest.ContainsKey(sKey))
				return _cDicRequest[sKey];
			return "";
		}

		private void DecryptDic(Dictionary<string, string> cDicTemp)
		{
			if (!cDicTemp.ContainsKey("*") || !cDicTemp.ContainsKey("*"))
			{
				_cDicRequest.Clear();
				cDicTemp.Clear();
				return;
			}

			string sInstanceKey = cDicTemp["*"];
			_sEncryptKey = Player.Manager.GetEncryptKey(sInstanceKey);
			if ("" == _sEncryptKey)
			{
				_cDicRequest.Clear();
				cDicTemp.Clear();
				return;
			}

			dynamic c = JsonFxUtil.StringToDynamic(EncrypUtil.Decrypt(cDicTemp["*"], _sEncryptKey));
			IDictionary<string, Object> cDic = (IDictionary<string, Object>)c;

			char[] sCharsToTrim = { ' ', '\'', '\"' };

			string sValue = "";
			foreach (string s in cDic.Keys)
			{
				sValue = cDic[s].ToString();
				sValue = sValue.Trim(sCharsToTrim);
				_cDicRequest.Add(s, sValue);
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////////
	// Manager
	////////////////////////////////////////////////////////////////////////////////
	public static class Manager
	{
		////////////////////////////////////////
		// variables
		////////////////////////////////////////
		private static bool		_bForDebug	= false;

		////////////////////////////////////////
		// get, set
		////////////////////////////////////////
		public static bool		IsDebugMode	{ get { return _bForDebug; } }

		////////////////////////////////////////
		// functions
		////////////////////////////////////////
		public static void Init()
		{
			_bForDebug = FileUtil.ExistFile(HttpContext.Current.Server.MapPath("/log/fordebug.txt"));				
		}
	}
}