using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Web;

public class BaseClass
{
	////////////////////////////////////////////////////////////////////////////////
	// Instance
	////////////////////////////////////////////////////////////////////////////////
	public class Instance
	{
		////////////////////////////////////////
		// variables
		////////////////////////////////////////
		protected string		_sFieldName		= "";					// DB 필드 이름
		protected string		_sNodeName		= "";					// JsonToken 루트 노드 이름
		protected MySqlDbType	_cMySqlDBType	= MySqlDbType.Text;

		////////////////////////////////////////
		// get, set
		////////////////////////////////////////
		public string			FieldName		{ get { return _sFieldName; } }
		public MySqlDbType		MySqlDBType		{ get { return _cMySqlDBType; } }

		////////////////////////////////////////
		// functions
		////////////////////////////////////////
		public void Init()
		{
			_Init();
		}

		public void Release()
		{
			_Release();
		}

		public void ToExpandoObject(dynamic cRoot)
		{
			_ToExpandoObject(cRoot);
		}

		public void ToExpandoObjectForDB(dynamic cRoot)
		{
			_ToExpandoObjectForDB(cRoot);
		}

		public dynamic ToExpandoObject()
		{
			dynamic c = new ExpandoObject();
			ToExpandoObject(c);
			return c;
		}

		public dynamic ToExpandoObjectForDB()
		{
			dynamic c = new ExpandoObject();
			ToExpandoObjectForDB(c);
			return c;
		}

		public string ToJsonToken()
		{
			return JsonFxUtil.DynamicToString(ToExpandoObject());
		}

		public string ToJsonTokenForDB()
		{
			return JsonFxUtil.DynamicToString(ToExpandoObjectForDB());
		}

		public void FillFromDataRow(DataRow cDataRow)
		{
			if (!cDataRow.Table.Columns.Contains(_sFieldName))
				return;

			_FillFromDataRow(cDataRow);
		}

		public void FillFormJsonToken(string sJsonToken)
		{
			_FillFromJsonToken(sJsonToken);
		}

		public void FillFromExpandoObject(dynamic c)
		{
			_FillFromExpandoObject(c);
		}

		protected virtual void _Init()
		{
		}

		protected virtual void _Release()
		{
		}

		protected virtual void _ToExpandoObject(dynamic cRoot)
		{
		}

		protected virtual void _ToExpandoObjectForDB(dynamic cRoot)
		{
			_ToExpandoObject(cRoot);
		}

		protected virtual void _FillFromDataRow(DataRow cDataRow)
		{
		}

		protected virtual void _FillFromJsonToken(string sJsonToken)
		{
		}

		protected virtual void _FillFromExpandoObject(dynamic c)
		{
		}
	}
}