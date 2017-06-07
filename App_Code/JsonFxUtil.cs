using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

public class JsonFxUtil
{
	public static string DynamicToString(dynamic c)
	{
		JsonFx.Json.JsonWriter cJsonWriter = new JsonFx.Json.JsonWriter();
		cJsonWriter.Settings.PrettyPrint = false;

		return cJsonWriter.Write(c);
	}

	public static dynamic StringToDynamic(string s)
	{
		if ("" == s)
			return null;

		JsonFx.Json.JsonReader cReader = new JsonFx.Json.JsonReader();
		dynamic c = cReader.Read(s);
		return c;
	}

	public static dynamic Open(string sFilePath)
	{
		if (!File.Exists(sFilePath))
			return null;

		using (FileStream cFileStream = File.Open(sFilePath, FileMode.Open))
		{
			byte[] sBytes = new byte[cFileStream.Length];
			cFileStream.Read(sBytes, 0, (int)cFileStream.Length);

			String sJsonToken = new UTF8Encoding(false).GetString(sBytes);
			return StringToDynamic(sJsonToken);
		}
	}

	public static bool Open(String sFilePath, ref dynamic rOut)
	{
		rOut = Open(sFilePath);
		if (null == rOut)
			return false;

		return true;
	}

	public static bool Write(String sFileName, ref dynamic rRoot)
	{
		String sTemp = DynamicToString(rRoot);

		using (FileStream cStream = File.Open(sFileName, FileMode.Create))
		{
			StreamWriter cWriter = new StreamWriter(cStream, new UTF8Encoding(false));
			cWriter.Write(sTemp);
			cWriter.Close();
		}

		return true;
	}
}