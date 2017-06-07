using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

public class Cooking
{
	////////////////////////////////////////////////////////////////////////////////
	// TryCookResult
	////////////////////////////////////////////////////////////////////////////////
	public class TryCookResult : BaseClass.Instance
	{
		////////////////////////////////////////
		// variables
		////////////////////////////////////////
		private Enums.CookingSuccessType	_eSuccessType	= Enums.CookingSuccessType.eEnumSize;

		////////////////////////////////////////
		// functions
		////////////////////////////////////////
		public TryCookResult(Enums.CookingSuccessType eType)
		{
			_eSuccessType = eType;
		}

		protected override void _ToExpandoObject(dynamic cRoot)
		{
			dynamic c = new ExpandoObject();
			c.successtype = (int)_eSuccessType;

			cRoot.cookresult = c;
		}
	}

	////////////////////////////////////////////////////////////////////////////////
	// Palette
	////////////////////////////////////////////////////////////////////////////////
	public class Palette
	{
		////////////////////////////////////////
		// variables
		////////////////////////////////////////
		private int					_iID						= -1;
		private List<int>			_iListIngredientsIDs		= new List<int>();
		private List<int>			_iListIngredientsAmounts	= new List<int>();
		private int					_iCookingCost				= 0;

		////////////////////////////////////////
		// get, set
		////////////////////////////////////////
		public int					ID							{ get { return _iID; } }
		public List<int>			IngredientsIDList			{ get { return _iListIngredientsIDs; } }
		public List<int>			IngredientsAmountList		{ get { return _iListIngredientsAmounts; } }
		public int					CookingCost					{ get { return _iCookingCost; } }

		////////////////////////////////////////
		// functions
		////////////////////////////////////////
		public void Load(dynamic c)
		{
			_iID			= c.id;
			_iCookingCost	= c.cookingcost;

			StringUtil.StringToList(c.ingredientsids, ref _iListIngredientsIDs);
			StringUtil.StringToList(c.ingredientsamounts, ref _iListIngredientsAmounts);
		}

		public bool CheckIngredientsList(CookingIngredients.Instance cIngredientsInst)
		{
			for (int i = 0, iSize = _iListIngredientsIDs.Count; i < iSize; ++i)
			{
				if (cIngredientsInst.GetIngredientsAmount(_iListIngredientsIDs[i]) < _iListIngredientsAmounts[i])
					return false;
			}

			return true;
		}

		public void RemoveIngredientsByCook(CookingIngredients.Instance cIngredientsInst)
		{
			for (int i = 0, iSize = _iListIngredientsIDs.Count; i < iSize; ++i)
				cIngredientsInst.RemoveIngredients(_iListIngredientsIDs[i], _iListIngredientsAmounts[i]);
		}
	}

	////////////////////////////////////////////////////////////////////////////////
	// InstanceData
	////////////////////////////////////////////////////////////////////////////////
	public class InstanceData
	{
		////////////////////////////////////////
		// variables
		////////////////////////////////////////
		private int						_iPaletteID			= 0;
		private int						_iAmount			= 0;
		private int						_iGreatAmount		= 0;
		private bool					_bHas				= false;
		private bool					_bGreatSuccess		= false;
		private int						_iGreatType			= 0;

		////////////////////////////////////////
		// get, set
		////////////////////////////////////////
		public int						PaletteID			{ get { return _iPaletteID; } }
		public int						Amount				{ get { return _iAmount; } }
		public int						GreatAmount			{ get { return _iGreatAmount; } }
		public bool						Has					{ get { return _bHas; } }
		public bool						HasBeenGreatSuccess	{ get { return _bGreatSuccess; } }
		public Enums.CookingGreatType	GreateTypeEnum		{ get { return (Enums.CookingGreatType)_iGreatType; } }

		////////////////////////////////////////
		// functions
		////////////////////////////////////////
		public InstanceData()
		{
		}

		public InstanceData(int iPaletteID)
		{
			_iPaletteID		= iPaletteID;
			_iAmount		= 0;
			_iGreatAmount	= 0;
			_bHas			= false;
			_bGreatSuccess	= false;
			_iGreatType		= 0;
		}

		public void AddAmount(Enums.CookingSuccessType eSuccessType)
		{
			_bHas = true;

			if (Enums.CookingSuccessType.eGreatSuccess == eSuccessType)
			{
				++_iGreatAmount;
				_bGreatSuccess = true;

				++_iGreatType;
				if ((int)Enums.CookingGreatType.eGold < _iGreatType)
					_iGreatType = (int)Enums.CookingGreatType.eGold;
			}
			else
				++_iAmount;
		}

		public void RemoveAmount(int iAmount)
		{
			if (_iAmount < iAmount)
				return;
			_iAmount -= iAmount;
		}

		public void RemoveGreatAmount(int iAmount)
		{
			if (_iGreatAmount < iAmount)
				return;
			_iGreatAmount -= iAmount;
		}

		public void ToExpandoObject(List<ExpandoObject> cList)
		{
			dynamic c = new ExpandoObject();
			c.pi	= _iPaletteID;
			c.a		= _iAmount;
			c.ga	= _iGreatAmount;
			c.has	= _bHas;
			c.gs	= _bGreatSuccess;
			c.gt	= _iGreatType;

			cList.Add(c);
		}

		public void FromExpandoObject(dynamic c)
		{
			_iPaletteID		= c.pi;
			_iAmount		= c.a;
			_bHas			= c.has;

			IDictionary<string, Object> cDic = (IDictionary<String, Object>)c;
			if (cDic.ContainsKey("*"))
				_iGreatAmount = c.ga;

			if (cDic.ContainsKey("*"))
				_bGreatSuccess = c.gs;

			if (cDic.ContainsKey("*"))
				_iGreatType = c.gt;
			else
			{
				int iGreatType = _iGreatAmount;
				if ((int)Enums.CookingGreatType.eGold < iGreatType)
					iGreatType = (int)Enums.CookingGreatType.eGold;
				_iGreatType = iGreatType;
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////////
	// Instance
	////////////////////////////////////////////////////////////////////////////////
	public class Instance : BaseClass.Instance
	{
		////////////////////////////////////////
		// variables
		////////////////////////////////////////
		private Dictionary<int, InstanceData>	_cDicInstanceData	= new Dictionary<int,InstanceData>();

		////////////////////////////////////////
		// functions
		////////////////////////////////////////
		public Instance()
		{
			_sFieldName = "cooking";
			_sNodeName	= "cooking";
		}

		public Enums.ErrorCode TryCook(Player.Instance cOwner, int iCookingID, ref Enums.CookingSuccessType eSuccessType)
		{
			// check Palette
			Palette cPalette = Manager.GetPalette(iCookingID);
			if (null == cPalette)
				return Enums.ErrorCode.eCooking_InvalidID;

			// check InstanceData
			InstanceData cData = null;
			if (_cDicInstanceData.ContainsKey(iCookingID))
				cData = _cDicInstanceData[iCookingID];
			else
			{
				cData = new InstanceData(iCookingID);
				_cDicInstanceData.Add(iCookingID, cData);
			}

			// check ingredients
			if (!cPalette.CheckIngredientsList(cOwner.IngredientsInstance))
				return Enums.ErrorCode.eCooking_NotEnoughIngredients;

			// cook - change informations
			eSuccessType = CalcSuccessType(cOwner, iCookingID);

			// exp
			int		iExp		= CookingLevel.Manager.GetCookingData(iCookingID).CookingExp;
			float	fScaleSkill	= (cOwner.PlayerSkillInstance.GetValue(Enums.PlayerSkillID.eExpCooking) * 0.01f);
			float	fScaleGreat	= Enums.CookingSuccessType.eGreatSuccess == eSuccessType ? 0.15f : 0.0f;
			
			float fScaleMedal = 0.0f;
			switch (cData.GreateTypeEnum)
			{
				case Enums.CookingGreatType.eBronze:	fScaleMedal = 0.05f; break;
				case Enums.CookingGreatType.eSilver:	fScaleMedal = 0.1f; break;
				case Enums.CookingGreatType.eGold:		fScaleMedal = 0.15f; break;

				case Enums.CookingGreatType.eNone:
				default:
					fScaleMedal = 0.0f;	break;
			}

			float fExp = (float)iExp;
			fExp *= (1.0f + fScaleSkill + fScaleGreat + fScaleMedal);

			iExp = (int)Math.Round(fExp, 0);

			int iOldLevel = cOwner.PlayerLevelInstance.Level;

			cData.AddAmount(eSuccessType);												// 요리 갯수 1 올려주고
			//cOwner.MoneysInstance.Pay(Enums.MoneysType.eCoin, cPalette.CookingCost);	// 돈 줄여주고
			cPalette.RemoveIngredientsByCook(cOwner.IngredientsInstance);				// 요리재료 줄여주고
			cOwner.PlayerLevelInstance.AddExp(cOwner, iExp);							// 경험치 채워주고
			cOwner.CookingLevelInstance.AddCooking(cData.PaletteID, cOwner);			// 요리 레벨 정보 변경해 준다.

			if (iOldLevel < cOwner.PlayerLevelInstance.Level)
				Ranking.Manager.ChangeLevel(cOwner.AccountID, cOwner.PlayerLevelInstance.Level);

			// check mission
			cOwner.MissionInstance.UpCookingAmount(iCookingID);

			if (Enums.CookingSuccessType.eGreatSuccess == eSuccessType)
				cOwner.MissionInstance.UpGreatCookingAmount(iCookingID);

			// write log
			//int iCurCoin = cOwner.MoneysInstance.GetMoney(Enums.MoneysType.eCoin);
			//MySqlUtil.WriteLog(cOwner.AccountID, Enums.LogMainType.Coin_Remove, Enums.LogSubType.Cooking, iCookingID.ToString(), (iCurCoin + cPalette.CookingCost).ToString(), iCurCoin.ToString());

			return Enums.ErrorCode.eNone;
		}

		public bool HasCooking(int iCookingID)
		{
			if (_cDicInstanceData.ContainsKey(iCookingID))
				return _cDicInstanceData[iCookingID].Has;
			return false;
		}

		public int GetCookingAmount(int iCookingID)
		{
			if (_cDicInstanceData.ContainsKey(iCookingID))
				return _cDicInstanceData[iCookingID].Amount;
			return 0;
		}

		public int GetGreatCookingAmount(int iCookingID)
		{
			if (_cDicInstanceData.ContainsKey(iCookingID))
				return _cDicInstanceData[iCookingID].GreatAmount;
			return 0;
		}

		public bool RemoveCooking(int iID, int iAmount)
		{
			if (!_cDicInstanceData.ContainsKey(iID))
				return false;

			if (_cDicInstanceData[iID].Amount < iAmount)
				return false;

			_cDicInstanceData[iID].RemoveAmount(iAmount);
			return true;
		}

		public bool RemoveGreatCooking(int iID, int iAmount)
		{
			if (!_cDicInstanceData.ContainsKey(iID))
				return false;

			if (_cDicInstanceData[iID].GreatAmount < iAmount)
				return false;

			_cDicInstanceData[iID].RemoveGreatAmount(iAmount);
			return true;
		}

		protected override void _Release()
		{
			_cDicInstanceData.Clear();
		}
		
		protected override void _ToExpandoObject(dynamic cRoot)
		{
			List<ExpandoObject> cList = new List<ExpandoObject>();
			foreach (int iPaletteID in _cDicInstanceData.Keys)
				_cDicInstanceData[iPaletteID].ToExpandoObject(cList);

			dynamic c = new ExpandoObject();
			c.instances = cList;

			cRoot.cooking = c;
		}

		protected override void _FillFromDataRow(System.Data.DataRow cDataRow)
		{
			JsonFxObject.Instance cJsonInst = new JsonFxObject.Instance(cDataRow[_sFieldName].ToString(), _sNodeName);

			if (cJsonInst.ContainsKey("instances"))
			{
				int i = 0;

				foreach (dynamic c in cJsonInst.DynamicObject.instances)
				{
					i = c.pi;

					if (_cDicInstanceData.ContainsKey(i))
						_cDicInstanceData[i].FromExpandoObject(c);
					else
					{
						InstanceData cData = new InstanceData();
						cData.FromExpandoObject(c);
						_cDicInstanceData.Add(i, cData);
					}
				}
			}
		}

		private Enums.CookingSuccessType CalcSuccessType(Player.Instance cOwner, int iCookingID)
		{
			// 기본 10% + 플레이어 스킬 %
			int iBasicProb			= 10;
			int iPlayerSkillProb	= (int)cOwner.PlayerSkillInstance.GetValue(Enums.PlayerSkillID.eUpCookingGreatProb);

			int iRandomNumber = cOwner.GetRandomNumber(1, 101);//new Random().Next(1, 101);
			//FileLogUtil.Log("Cooking Prob : " + iRandomNumber.ToString() + " <= " + (iBasicProb + iPlayerSkillProb).ToString());

			if (iRandomNumber <= iBasicProb + iPlayerSkillProb)
				return Enums.CookingSuccessType.eGreatSuccess;
			return Enums.CookingSuccessType.eSuccess;
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
		private static bool							_bLoadPalettes	= false;
		private static Dictionary<int, Palette>		_cDicPalette	= new Dictionary<int,Palette>();
		private static List<Palette>				_cListPalette	= new List<Palette>();
		private static Random						_cRandom		= new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

		////////////////////////////////////////
		// functions
		////////////////////////////////////////
		public static void LoadPalettes()
		{
			if (_bLoadPalettes)
				return;
			_bLoadPalettes = true;

			dynamic cRoot = JsonFxUtil.Open(HttpContext.Current.Server.MapPath("/Spec/cooking.json"));
			if (null == cRoot)
				return;

			foreach (dynamic c in cRoot.cookings)
			{
				Palette cPalette = new Palette();
				cPalette.Load(c);

				_cDicPalette.Add(cPalette.ID, cPalette);
				_cListPalette.Add(cPalette);
			}
		}

		public static void ClearPalettes()
		{
			_bLoadPalettes = false;
			_cDicPalette.Clear();
			_cListPalette.Clear();
		}

		public static Palette GetPalette(int iPaletteID)
		{
			if (_cDicPalette.ContainsKey(iPaletteID))
				return _cDicPalette[iPaletteID];
			return null;
		}

		public static Palette GetRandomPalette()
		{
			return _cListPalette[_cRandom.Next(0, _cListPalette.Count)];
		}
	}
}