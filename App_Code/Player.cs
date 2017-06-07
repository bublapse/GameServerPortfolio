using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

public class Player
{
	////////////////////////////////////////////////////////////////////////////////
	// NewbiesGift
	////////////////////////////////////////////////////////////////////////////////
	public class NewbiesGift
	{
		////////////////////////////////////////
		// variables
		////////////////////////////////////////
		private bool					_bLoaded			= false;

		public int						_iCoin				= 0;
		public Dictionary<int, int>		_iDicIngredients	= new Dictionary<int,int>();

		////////////////////////////////////////
		// get, set
		////////////////////////////////////////
		public int						Coin				{ get { return _iCoin; } }
		public Dictionary<int, int>		IngredientsDic		{ get { return _iDicIngredients; } }

		////////////////////////////////////////
		// functions
		////////////////////////////////////////
		public void Load()
		{
			if (_bLoaded)
				return;
			_bLoaded = true;

			dynamic cRoot = JsonFxUtil.Open(HttpContext.Current.Server.MapPath("/Spec/giftfornewbies.json"));

			dynamic cMoney = cRoot.money;
			_iCoin = cMoney.coin;

			foreach (dynamic c in cRoot.ingredients)
				_iDicIngredients.Add(c.iid, c.amount);
		}

		public void Clear()
		{
			if (!_bLoaded)
				return;
			_bLoaded = false;

			_iCoin = 0;

			_iDicIngredients.Clear();
		}
	}

	////////////////////////////////////////////////////////////////////////////////
	// InstanceData
	////////////////////////////////////////////////////////////////////////////////
	public class InstanceData : BaseClass.Instance
	{
		////////////////////////////////////////
		// variables
		////////////////////////////////////////
		private string		_sKey			= "";
		private string		_sSuperBitID	= "";
		private string		_sEncryptKey	= "";

		////////////////////////////////////////
		// get, set
		////////////////////////////////////////
		public string		Key				{ get { return _sKey; }			set { _sKey = value; RefreshEncryptKey(); } }
		public string		SuperBitID		{ get { return _sSuperBitID; }	set { _sSuperBitID = value; } }
		public string		EncryptKey		{ get { return _sEncryptKey; } }

		////////////////////////////////////////
		// functions
		////////////////////////////////////////
		public InstanceData()
		{
		}

		public InstanceData	(string sKey, string sSuperBitID)
		{
			_sKey			= sKey;
			_sSuperBitID	= sSuperBitID;

			RefreshEncryptKey();
		}

		protected override void _Release()
		{
			_sKey			= "";
			_sSuperBitID	= "";
		}

		protected override void _ToExpandoObject(dynamic cRoot)
		{
			dynamic c = new ExpandoObject();
			c.key			= _sKey;
			c.superbitid	= _sSuperBitID;
			c.servertime	= DateTimeUtil.ToClientString(DateTime.Now);
			//c.iapgoogle		= IAPChecker.Google2.IsConnect;
			c.iapgoogle		= false;//IAPChecker.Google3.Available;
			c.iapapple		= IAPChecker.Apple2.IsConnect;
			cRoot.player = c;
		}

		private void RefreshEncryptKey()
		{
			_sEncryptKey = _sKey + _sSuperBitID;
			if (32 < _sEncryptKey.Length)
				_sEncryptKey = _sEncryptKey.Substring(0, 32);
		}
	}

	////////////////////////////////////////////////////////////////////////////////
	// FinishGameResult
	////////////////////////////////////////////////////////////////////////////////
	public class FinishGameResult : BaseClass.Instance
	{
		////////////////////////////////////////
		// variables
		////////////////////////////////////////
		private int					_iScore					= 0;
		private int					_iCoin					= 0;
		private int					_iExp					= 0;

		private bool				_bNextCanShowTreasure	= false;
		private Enums.MoneysType	_eTreatureMoneyType		= Enums.MoneysType.eEnumSize;
		private int					_iTreasureMoneyAmount	= 0;

		////////////////////////////////////////
		// functions
		////////////////////////////////////////
		public FinishGameResult(int iScore, int iCoin, int iExp)
		{
			_iScore = iScore;
			_iCoin	= iCoin;
			_iExp	= iExp;
		}

		public void SetNextCanShowTreasure(bool b)
		{
			_bNextCanShowTreasure = b;
		}

		public void SetTreasureData(Enums.MoneysType e, int i)
		{
			_eTreatureMoneyType		= e;
			_iTreasureMoneyAmount	= i;
		}

		protected override void _ToExpandoObject(dynamic cRoot)
		{
			dynamic c = new ExpandoObject();
			c.score = _iScore;
			c.coin	= _iCoin;
			c.exp	= _iExp;
			c.nst	= _bNextCanShowTreasure;

			if (Enums.MoneysType.eEnumSize != _eTreatureMoneyType)
			{
				c.tmt	= (int)_eTreatureMoneyType;
				c.tma	= _iTreasureMoneyAmount;
			}

			cRoot.finishgameresult = c;
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
		private int									_iAccountID				= 0;
		private InstanceData						_cInstanceData			= new InstanceData();

		//private Enums.VersionCode					_eVersionCode			= Enums.VersionCode.e150;

		private DateTime							_cCreateDate			= DateTime.Now;
		private DateTime							_cLastUpdate			= DateTime.Now;		

		private string								_sFacebookID			= "";
		private string								_sGooglePlusID			= "";
		private string								_sGamecenterID			= "";

		private Account.Instance					_cAccount				= new Account.Instance();
		private Money.Instance						_cMoneys				= new Money.Instance();
		private Score.Instance						_cScore					= new Score.Instance();

		private Achievements.Instance				_cAchievements			= new Achievements.Instance();
		private Mission.Instance					_cMission				= new Mission.Instance();

		private Cooking.Instance					_cCooking				= new Cooking.Instance();
		private CookingIngredients.Instance			_cIngredients			= new CookingIngredients.Instance();
		private CookingLevel.Instance				_cCookingLevel			= new CookingLevel.Instance();

		private MysticCooking.Instance				_cMysticCooking			= new MysticCooking.Instance();
		private MysticCookingLevel.Instance			_cMysticCookingLevel	= new MysticCookingLevel.Instance();

		private PlayerLevel.Instance				_cPlayerLevel			= new PlayerLevel.Instance();
		private PlayerSkill.Instance				_cPlayerSkill			= new PlayerSkill.Instance();
		private Character.Instance					_cCharacter				= new Character.Instance();

		private Shop.Instance						_cShop					= new Shop.Instance();
		private Tutorial.Instance					_cTutorial				= new Tutorial.Instance();

		private GiftBox.Instance					_cGiftBox				= new GiftBox.Instance();
		private MovieAd.Instance					_cMovieAd				= new MovieAd.Instance();

		private SpecialValue.Instance				_cSpecialValue			= new SpecialValue.Instance();

		private BackgroundCompatibility.Instance	_cBC					= new BackgroundCompatibility.Instance();
		private BuffScroll.Instance					_cBuffScroll			= new BuffScroll.Instance();
		private Notice.Instance						_cNotice				= new Notice.Instance();

		private Friends.Instance					_cFriends				= new Friends.Instance();

		private Random								_cRandom				= new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

		////////////////////////////////////////
		// get, set
		////////////////////////////////////////
		public int									AccountID					{ get { return _iAccountID; }			set { _iAccountID = value; } }
		public string								Key							{ get { return _cInstanceData.Key; }	set { _cInstanceData.Key = value; } }
		public string								SuperBitID					{ get { return _cInstanceData.SuperBitID; } }

		public DateTime								CreateDate					{ get { return _cCreateDate; } }
		public DateTime								LastUpdate					{ get { return _cLastUpdate; } }

		public string								FacebookID					{ get { return _sFacebookID; } }
		public string								GooglePlusID				{ get { return _sGooglePlusID; } }
		public string								GamecenterID				{ get { return _sGamecenterID; } }

		public Account.Instance						AccountInstance				{ get { return _cAccount; } }
		public Money.Instance						MoneysInstance				{ get { return _cMoneys; } }
		public Score.Instance						ScoreInstance				{ get { return _cScore; } }

		public Achievements.Instance				AchievementsInstance		{ get { return _cAchievements; } }
		public Mission.Instance						MissionInstance				{ get { return _cMission; } }

		public Cooking.Instance						CookingInstance				{ get { return _cCooking; } }
		public CookingIngredients.Instance			IngredientsInstance			{ get { return _cIngredients; } }
		public CookingLevel.Instance				CookingLevelInstance		{ get { return _cCookingLevel; } }

		public MysticCooking.Instance				MysticCookingInstance		{ get { return _cMysticCooking; } }
		public MysticCookingLevel.Instance			MysticCookingLevelInstance	{ get { return _cMysticCookingLevel; } }

		public PlayerLevel.Instance					PlayerLevelInstance			{ get { return _cPlayerLevel; } }
		public PlayerSkill.Instance					PlayerSkillInstance			{ get { return _cPlayerSkill; } }
		public Character.Instance					CharacterInstance			{ get { return _cCharacter; } }
		
		public Shop.Instance						ShopInstance				{ get { return _cShop; } }
		public Tutorial.Instance					TutorialInstance			{ get { return _cTutorial; } }

		public GiftBox.Instance						GiftBoxInstance				{ get { return _cGiftBox; } }
		public MovieAd.Instance						MovieAdInstance				{ get { return _cMovieAd; } }
		public BackgroundCompatibility.Instance		BackComInstance				{ get { return _cBC; } }
		public BuffScroll.Instance					BuffScrollInstance			{ get { return _cBuffScroll; } }
		public Notice.Instance						NoticeInstance				{ get { return _cNotice; } }

		public Friends.Instance						FriendInstance				{ get { return _cFriends; } }

		public SpecialValue.Instance				SpecialValue				{ get { return _cSpecialValue; } }

		////////////////////////////////////////
		// functions
		////////////////////////////////////////
		public Instance()
		{
			_sFieldName = "id";
			_sNodeName	= "";
		}

		public void ClearFacebookID()
		{
			_sFacebookID = "";
		}

		public string GetEncryptKey()
		{
			return _cInstanceData.EncryptKey;
		}

		public int GetRandomNumber(int iMin, int iMax)
		{
			return _cRandom.Next(iMin, iMax);
		}

		public Enums.ErrorCode CreateNewPlayer(string sSuperBitID, string sFacebookID, string sGoogleID, string sNickName, ref string rError)
		{
			_sFacebookID	= sFacebookID;
			_sGooglePlusID	= sGoogleID;

			_cInstanceData.SuperBitID = sSuperBitID;
					
			_cAccount.CreateNewAccount(sNickName);
			_cMoneys.CreateNewAccount();
			_cIngredients.CreateNewAccount();
			_cCookingLevel.CreateNewAccount();
			_cMission.CreateNewAccount();
			_cCharacter.CreateNewAccount();
			_cShop.CreateNewAccount();
			_cTutorial.CreateNewAccount();
			_cPlayerLevel.CreateNewAccount();
			_cMovieAd.CreateNewAccount();

			Enums.ErrorCode eResult = MySqlUtil.CreateNewRecord(this, ref rError);
			if (Enums.ErrorCode.eNone != eResult)
				return eResult;

			eResult = MySqlUtil.GetAccountID(this, ref rError);
			if (Enums.ErrorCode.eNone != eResult)
				return eResult;

			MySqlUtil.WriteLog(_iAccountID, Enums.LogMainType.Account, Enums.LogSubType.Create, sSuperBitID);

			_cAchievements.SetOwner(this);

			CheckSpecialDateGift();
			

			return Enums.ErrorCode.eNone;
		}

		public Enums.ErrorCode LoadFromDB(string sSuperBitID, string sFacebookID, string sGoogleID, string sGameCenterID)
		{
			_cInstanceData.SuperBitID	= sSuperBitID;
			_sFacebookID				= sFacebookID;
			_sGooglePlusID				= sGoogleID;
			_sGamecenterID				= sGameCenterID;

			string			sError	= "";
			Enums.ErrorCode eResult = MySqlUtil.FillPlayer(this, ref sError);
			if (Enums.ErrorCode.eNone != eResult)
				return eResult;

			_cAchievements.SetOwner(this);

			// check Background Compatibility
			_cBC.Check(this);

			return eResult;
		}

		public string FinishGame(RequestTable.Instance cRequest, ResponseTable.Instance cResponse)
		{
			Debug.Log("    - [Player : FinishGame] " + _cAccount.Nickname);

			JsonFxObject.Instance cJsonFxObject = new JsonFxObject.Instance(cRequest.JsonToken);

			if (!cJsonFxObject.ContainsKey("*"))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonNode, "*");
			if (!cJsonFxObject.ContainsKey("*"))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonNode, "*");
			if (!cJsonFxObject.ContainsKey("*"))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonNode, "*");
			if (!cJsonFxObject.ContainsKey("*"))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonNode, "*");
			if (!cJsonFxObject.ContainsKey("*"))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonNode, "*");
			if (!cJsonFxObject.ContainsKey("*"))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonNode, "*");

			dynamic c = cJsonFxObject.DynamicObject;
			int		iScore		= c.s;
			int		iFloor		= c.f;
			int		iCoin		= c.c;
			int		iExp		= c.e;
			float	fPlayTime	= (float)c.p;

			bool bCanShowTreasure	= _cAccount.CanShowTreasure;
			bool bAbsorbTreasure	= false;
			if (cJsonFxObject.ContainsKey("*"))
				bAbsorbTreasure = c.at;

			Dictionary<int, int> iDicIngredients = new Dictionary<int,int>();
			foreach (dynamic cIngredients in c.i)
				iDicIngredients.Add(cIngredients.id, cIngredients.a);

			// 정보 갱신
			int iOldLevel = _cPlayerLevel.Level;

			_cScore.UpdateValue(iScore, iFloor, _cCharacter.SelectedID, _cCharacter.SelectedID2);
			_cScore.AddPlayCount(fPlayTime);
			_cMoneys.AddMoney(Enums.MoneysType.eCoin, iCoin);
			_cPlayerLevel.AddExp(this, iExp);
			_cIngredients.AddIngredients(iDicIngredients);

			if (iOldLevel < _cPlayerLevel.Level)
				Ranking.Manager.ChangeLevel(_iAccountID, _cPlayerLevel.Level);

			// 보물상자 얻었나?
			FinishGameResult cFinishGameResult = new FinishGameResult(iScore, iCoin, iExp);

			if (bAbsorbTreasure && bCanShowTreasure)
			{
				Enums.MoneysType	eMoneyType		= Enums.MoneysType.eCoin;
				int					iMoneyAmount	= 0;
				_cAccount.GetTreasure(ref eMoneyType, ref iMoneyAmount);

				cFinishGameResult.SetTreasureData(eMoneyType, iMoneyAmount);

				_cMoneys.AddMoney(eMoneyType, iMoneyAmount);

				Debug.Log("    -    - eMoneyType : " + eMoneyType.ToString());
				Debug.Log("    -    - iMoneyAmount : " + iMoneyAmount.ToString());
			}
			
			cFinishGameResult.SetNextCanShowTreasure(_cAccount.CalcNextCanShowTreasure());

			// 랭킹에 정보 전달
			Ranking.Manager.ChangeScore(this);

			// update db
			string			sError	= "";
			Enums.ErrorCode eResult = MySqlUtil.UpdateFinishGame(this, ref sError);
			if (Enums.ErrorCode.eNone != eResult)
				return cResponse.ToString(false, eResult, sError);

			// 새 정보 클라에 알려줌
			cResponse.AddBaseClass(_cInstanceData);
			cResponse.AddBaseClass(cFinishGameResult);
			cResponse.AddBaseClass(_cScore);
			cResponse.AddBaseClass(_cMoneys);
			cResponse.AddBaseClass(_cPlayerLevel);
			cResponse.AddBaseClass(_cIngredients);

			return cResponse.ToString(true);
		}

		public string UpdateTutorial(RequestTable.Instance cRequest, ResponseTable.Instance cResponse)
		{
			JsonFxObject.Instance cJsonFxObject = new JsonFxObject.Instance(cRequest.JsonToken);
			if (!cJsonFxObject.ContainsKey("*"))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonNode, "*");

			_cTutorial.FillFromExpandoObject(cJsonFxObject.DynamicObject.tutorial);

			// update db
			List<BaseClass.Instance> cListField = new List<BaseClass.Instance>();
			cListField.Add(_cTutorial);

			string			sError	= "";
			Enums.ErrorCode eResult = MySqlUtil.UpdateField(_iAccountID, cListField, ref sError);

			if (Enums.ErrorCode.eNone != eResult)
				return cResponse.ToString(false, eResult, sError);

			return cResponse.ToString(true);
		}

		public string UpdateMission(RequestTable.Instance cRequest, ResponseTable.Instance cResponse)
		{
			JsonFxObject.Instance cJsonFxObject = new JsonFxObject.Instance(cRequest.JsonToken);
			if (!cJsonFxObject.ContainsKey("*"))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonNode, "*");

			_cMission.FillFromExpandoObject(cJsonFxObject.DynamicObject.missions);

			// update db
			List<BaseClass.Instance> cListField = new List<BaseClass.Instance>();
			cListField.Add(_cMission);

			string			sError	= "";
			Enums.ErrorCode eResult = MySqlUtil.UpdateField(_iAccountID, cListField, ref sError);

			if (Enums.ErrorCode.eNone != eResult)
				return cResponse.ToString(false, eResult, sError);

			return cResponse.ToString(true);
		}

		public string GetMissionReward(RequestTable.Instance cRequest, ResponseTable.Instance cResponse)
		{
			JsonFxObject.Instance cJsonFxObject = new JsonFxObject.Instance(cRequest.JsonToken);
			if (!cJsonFxObject.ContainsKey("*"))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonNode, "*");

			Enums.ErrorCode eResult = _cMission.GiveMissionReward(this, cJsonFxObject.DynamicObject.missionid);
			if (Enums.ErrorCode.eNone != eResult)
				return cResponse.ToString(false, eResult);

			// update db
			List<BaseClass.Instance> cListField = new List<BaseClass.Instance>();
			cListField.Add(_cMission);
			cListField.Add(_cMoneys);

			string sError = "";
			eResult = MySqlUtil.UpdateField(_iAccountID, cListField, ref sError);
			if (Enums.ErrorCode.eNone != eResult)
				return cResponse.ToString(false, eResult, sError);

			cResponse.AddBaseClass(_cInstanceData);
			cResponse.AddBaseClass(_cMoneys);
			cResponse.AddBaseClass(_cMission);

			return cResponse.ToString(true);
		}

		public string TryCook(RequestTable.Instance cRequest, ResponseTable.Instance cResponse)
		{
			JsonFxObject.Instance cJsonFxObject = new JsonFxObject.Instance(cRequest.JsonToken);
			if (!cJsonFxObject.ContainsKey("*"))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonNode, "*");

			Enums.CookingSuccessType eSuccessType = Enums.CookingSuccessType.eEnumSize;

			Enums.ErrorCode eResult = _cCooking.TryCook(this, cJsonFxObject.DynamicObject.cookingid, ref eSuccessType);
			if (Enums.ErrorCode.eNone != eResult)
				return cResponse.ToString(false, eResult);

			// 돈, 요리, 요리재료, 경험치, 레벨이 변화한다.
			List<BaseClass.Instance> cListField = new List<BaseClass.Instance>();
			cListField.Add(_cMoneys);
			cListField.Add(_cCooking);
			cListField.Add(_cIngredients);
			cListField.Add(_cPlayerLevel);
			cListField.Add(_cCookingLevel);
			cListField.Add(_cMission);
			cListField.Add(_cMysticCookingLevel);

			string sError = "";
			eResult = MySqlUtil.UpdateField(_iAccountID, cListField, ref sError);
			if (Enums.ErrorCode.eNone != eResult)
				return cResponse.ToString(false, eResult, sError);

			// 새 정보 클라에 알려줌
			Cooking.TryCookResult cTryCookResult = new Cooking.TryCookResult(eSuccessType);
			cResponse.AddBaseClass(_cInstanceData);
			cResponse.AddBaseClass(cTryCookResult);
			cResponse.AddBaseClass(_cMoneys);
			cResponse.AddBaseClass(_cCooking);
			cResponse.AddBaseClass(_cIngredients);
			cResponse.AddBaseClass(_cPlayerLevel);
			cResponse.AddBaseClass(_cCookingLevel);
			cResponse.AddBaseClass(_cMysticCookingLevel);

			return cResponse.ToString(true);
		}

		public string TryMysticCook(RequestTable.Instance cRequest, ResponseTable.Instance cResponse)
		{
			JsonFxObject.Instance cJsonFxObject = new JsonFxObject.Instance(cRequest.JsonToken);
			if (!cJsonFxObject.ContainsKey("*"))
				return cResponse.ToString(false,  Enums.ErrorCode.eNeedJsonNode, "*");

			Enums.CookingSuccessType	eSuccessType	= Enums.CookingSuccessType.eEnumSize;
			Enums.ErrorCode				eResult			= _cMysticCooking.TryCook(this, cJsonFxObject.DynamicObject.cookingid, ref eSuccessType);

			if (Enums.ErrorCode.eNone != eResult)
				return cResponse.ToString(false, eResult);

			// db 업데이트
			List<BaseClass.Instance> cListField = new List<BaseClass.Instance>();
			cListField.Add(_cMoneys);
			cListField.Add(_cCooking);
			cListField.Add(_cIngredients);
			cListField.Add(_cPlayerLevel);
			cListField.Add(_cMysticCooking);
			cListField.Add(_cMysticCookingLevel);

			string sError = "";
			eResult = MySqlUtil.UpdateField(_iAccountID, cListField, ref sError);
			if (Enums.ErrorCode.eNone != eResult)
				return cResponse.ToString(false, eResult, sError);

			// 클라로 정보 전송
			Cooking.TryCookResult cTryCookResult = new Cooking.TryCookResult(eSuccessType);

			cResponse.AddBaseClass(_cInstanceData);
			cResponse.AddBaseClass(cTryCookResult);
			cResponse.AddBaseClass(_cMoneys);
			cResponse.AddBaseClass(_cIngredients);
			cResponse.AddBaseClass(_cCooking);
			cResponse.AddBaseClass(_cPlayerLevel);
			cResponse.AddBaseClass(_cMysticCooking);
			cResponse.AddBaseClass(_cMysticCookingLevel);

			return cResponse.ToString(true);
		}

		public string BuyItemInShop(RequestTable.Instance cRequest, ResponseTable.Instance cResponse)
		{
			JsonFxObject.Instance cJsonFxObject = new JsonFxObject.Instance(cRequest.JsonToken);
			if (!cJsonFxObject.ContainsKey("*"))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonNode, "*");
			if (!cJsonFxObject.ContainsKey("*"))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonNode, "*");

			// try buy
			Enums.ErrorCode eResult = _cShop.BuyItem(this, cJsonFxObject.DynamicObject.id, cJsonFxObject.DynamicObject.amount);
			if (Enums.ErrorCode.eNone != eResult)
				return cResponse.ToString(false, eResult);

			// DB 저장
			List<BaseClass.Instance> cListField = new List<BaseClass.Instance>();
			cListField.Add(_cMoneys);
			cListField.Add(_cIngredients);
			cListField.Add(_cShop);

			string sError = "";
			eResult = MySqlUtil.UpdateField(_iAccountID, cListField, ref sError);
			if (Enums.ErrorCode.eNone != eResult)
				return cResponse.ToString(false, eResult, sError);

			// 클라에 새 정보 알려줌
			cResponse.AddBaseClass(_cInstanceData);
			cResponse.AddBaseClass(_cMoneys);
			cResponse.AddBaseClass(_cIngredients);
			cResponse.AddBaseClass(_cShop);

			return cResponse.ToString(true);
		}

		public string AddItemInShop(RequestTable.Instance cRequest, ResponseTable.Instance cResponse)
		{
			JsonFxObject.Instance cJsonFxObject = new JsonFxObject.Instance(cRequest.JsonToken);
			if (!cJsonFxObject.ContainsKey("*"))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonNode, "*");
			if (!cJsonFxObject.ContainsKey("*"))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonNode, "*");

			// try add
			Enums.ErrorCode eResult = _cShop.AddItem(this, cJsonFxObject.DynamicObject.type, cJsonFxObject.DynamicObject.id);
			if (Enums.ErrorCode.eNone != eResult)
				return cResponse.ToString(false, eResult);

			// update database
			List<BaseClass.Instance> cListField = new List<BaseClass.Instance>();
			cListField.Add(_cMoneys);
			cListField.Add(_cShop);

			string sError = "";
			eResult = MySqlUtil.UpdateField(_iAccountID, cListField, ref sError);
			if (Enums.ErrorCode.eNone != eResult)
				return cResponse.ToString(false, eResult, sError);

			// 클라에 새 정보 알려줌
			cResponse.AddBaseClass(_cInstanceData);
			cResponse.AddBaseClass(_cMoneys);
			cResponse.AddBaseClass(_cShop);

			return cResponse.ToString(true);
		}

		public string ChangeNickName(RequestTable.Instance cRequest, ResponseTable.Instance cResponse)
		{
			JsonFxObject.Instance cJsonFxObject = new JsonFxObject.Instance(cRequest.JsonToken);
			if (!cJsonFxObject.ContainsKey("*"))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonNode, "*");

			// check cost
			if (_cTutorial.HaveChangedNickname)
			{
				if (_cMoneys.GetMoney(Enums.MoneysType.eCash) < 10)
					return cResponse.ToString(false, Enums.ErrorCode.eChangeNickname_NotEnoughCash);

				_cMoneys.Pay(Enums.MoneysType.eCash, 10);
			}

			// change
			string sNewNickname = cJsonFxObject.DynamicObject.nn;
			string sOldNickname	= _cAccount.Nickname;

			_cAccount.ChangeNickName(sNewNickname);

			_cTutorial.HaveChangedNickname = true;

			// update db
			List<BaseClass.Instance> cListField = new List<BaseClass.Instance>();
			cListField.Add(_cAccount);
			cListField.Add(_cMoneys);
			cListField.Add(_cTutorial);

			string sError = "";
			Enums.ErrorCode eResult = MySqlUtil.UpdateField(_iAccountID, cListField, ref sError);
			if (Enums.ErrorCode.eNone != eResult)
				return cResponse.ToString(false, eResult, sError);

			// write log
			MySqlUtil.WriteLog(_iAccountID, Enums.LogMainType.Account, Enums.LogSubType.Change_Nickname, "", sOldNickname, sNewNickname);

			// change ranking info
			Ranking.Manager.ChangeNickname(this);

			// response
			cResponse.AddBaseClass(_cInstanceData);
			cResponse.AddBaseClass(_cAccount);
			cResponse.AddBaseClass(_cTutorial);
			cResponse.AddBaseClass(_cMoneys);

			return cResponse.ToString(true);
		}

		protected override void _Release()
		{
			_cInstanceData.Release();
			_cAccount.Release();
			_cMoneys.Release();
			_cScore.Release();
			_cAchievements.Release();
			_cMission.Release();
			_cMysticCookingLevel.Release();
			_cMysticCooking.Release();
			_cCookingLevel.Release();
			_cCooking.Release();
			_cIngredients.Release();
			_cCookingLevel.Release();
			_cPlayerLevel.Release();
			_cPlayerSkill.Release();
			_cCharacter.Release();
			_cShop.Release();
			_cTutorial.Release();
			_cGiftBox.Release();
			_cMovieAd.Release();
			_cBC.Release();
			_cBuffScroll.Release();
			_cNotice.Release();
			_cFriends.Release();

			_sFacebookID	= "";
			_sGooglePlusID	= "";
			_sGamecenterID	= "";
			_iAccountID		= -1;
		}

		protected override void _ToExpandoObject(dynamic cRoot)
		{
			_cInstanceData.ToExpandoObject(cRoot);

			dynamic cSNS = new ExpandoObject();
			cSNS.facebookid		= _sFacebookID;
			cSNS.googleid		= _sGooglePlusID;
			cSNS.gamecenterid	= _sGamecenterID;
			cRoot.sns = cSNS;

			_cAccount.ToExpandoObject(cRoot);
			_cMoneys.ToExpandoObject(cRoot);
			_cScore.ToExpandoObject(cRoot);

			_cAchievements.ToExpandoObject(cRoot);
			_cMission.ToExpandoObject(cRoot);

			_cCooking.ToExpandoObject(cRoot);
			_cIngredients.ToExpandoObject(cRoot);
			_cCookingLevel.ToExpandoObject(cRoot);

			_cMysticCooking.ToExpandoObject(cRoot);
			_cMysticCookingLevel.ToExpandoObject(cRoot);			

			_cPlayerLevel.ToExpandoObject(cRoot);
			_cPlayerSkill.ToExpandoObject(cRoot);
			_cCharacter.ToExpandoObject(cRoot);

			_cShop.ToExpandoObject(cRoot);
			_cTutorial.ToExpandoObject(cRoot);

			_cGiftBox.ToExpandoObject(cRoot);
			_cMovieAd.ToExpandoObject(cRoot);

			_cBC.ToExpandoObject(cRoot);
			_cBuffScroll.ToExpandoObject(cRoot);
			_cNotice.ToExpandoObject(cRoot);

			_cFriends.ToExpandoObject(cRoot);

			if (null != ServerNotice.Manager.InstanceObject)
				ServerNotice.Manager.InstanceObject.ToExpandoObject(cRoot);
		}

		protected override void _FillFromDataRow(System.Data.DataRow cDataRow)
		{
			_iAccountID		= Int32.Parse(cDataRow["*"].ToString());
			_sFacebookID	= cDataRow["*"].ToString();
			_sGooglePlusID	= cDataRow["*"].ToString();
			_sGamecenterID	= cDataRow["*"].ToString();

			_cCreateDate	= DateTimeUtil.ToServerDateTime(cDataRow["*"].ToString());
			_cLastUpdate	= DateTimeUtil.ToServerDateTime(cDataRow["*"].ToString());

			if ("" == _cInstanceData.SuperBitID)
				_cInstanceData.SuperBitID = cDataRow["*"].ToString();

			_cAccount.FillFromDataRow(cDataRow);
			_cMoneys.FillFromDataRow(cDataRow);
			_cScore.FillFromDataRow(cDataRow);

			_cAchievements.FillFromDataRow(cDataRow);
			_cMission.FillFromDataRow(cDataRow);

			_cCooking.FillFromDataRow(cDataRow);
			_cIngredients.FillFromDataRow(cDataRow);
			_cCookingLevel.FillFromDataRow(cDataRow);

			_cMysticCooking.FillFromDataRow(cDataRow);
			_cMysticCookingLevel.FillFromDataRow(cDataRow);

			_cPlayerLevel.FillFromDataRow(cDataRow);
			_cPlayerSkill.FillFromDataRow(cDataRow);
			_cCharacter.FillFromDataRow(cDataRow);

			_cShop.FillFromDataRow(cDataRow);
			_cTutorial.FillFromDataRow(cDataRow);

			_cGiftBox.FillFromDataRow(cDataRow);
			_cMovieAd.FillFromDataRow(cDataRow);

			_cBC.FillFromDataRow(cDataRow);
			_cBuffScroll.FillFromDataRow(cDataRow);
			_cNotice.FillFromDataRow(cDataRow);

			_cFriends.FillFromDataRow(cDataRow);
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
		private static NewbiesGift					_cNewbiesGift				= new NewbiesGift();

		private static Random						_cRandom					= new Random();
		private static string						_sKeyString					= "*";
		private static int							_iKeyCounter				= 0;

		private static Dictionary<int, Instance>	_cDicInstanceWithID			= new Dictionary<int,Instance>();
		private static Dictionary<string, Instance>	_cDicInstanceWithKey		= new Dictionary<string,Instance>();
		private static Dictionary<string, Instance>	_cDicInstanceWithSuperBitID	= new Dictionary<string,Instance>();

		////////////////////////////////////////
		// get, set
		////////////////////////////////////////
		public static NewbiesGift					NewbiesGift					{ get { return _cNewbiesGift; } }

		////////////////////////////////////////
		// functions
		////////////////////////////////////////
		public static void Init()
		{
			_cNewbiesGift.Load();
		}

		public static void Release()
		{
			_cNewbiesGift.Clear();
			ClearPlayerList();
		}

		public static void ClearPlayerList()
		{
			_cDicInstanceWithID.Clear();
			_cDicInstanceWithKey.Clear();
			_cDicInstanceWithSuperBitID.Clear();
		}

		public static Instance GetInstanceWithAccountID(int iAccountID)
		{
			if (_cDicInstanceWithID.ContainsKey(iAccountID))
				return _cDicInstanceWithID[iAccountID];
			return null;
		}

		public static Instance GetInstanceWithSBID(string sSBID, bool bTryLogin)
		{
			if ("" == sSBID)
				return null;

			if (_cDicInstanceWithSuperBitID.ContainsKey(sSBID))
				return _cDicInstanceWithSuperBitID[sSBID];

			if (bTryLogin)
			{
				Instance cInstance = new Instance();
				Enums.ErrorCode eErrorCode = cInstance.LoadFromDB(sSBID, "", "", "");
				if (Enums.ErrorCode.eNone != eErrorCode)
					return null;

				cInstance.Key = GetNewKey();
				_cDicInstanceWithID.Add(cInstance.AccountID, cInstance);
				_cDicInstanceWithKey.Add(cInstance.Key, cInstance);
				_cDicInstanceWithSuperBitID.Add(cInstance.SuperBitID, cInstance);

				if (null != cInstance)
				{
					cInstance.CheckSpecialDateGift();
					cInstance.CheckMovieAdDateTime();
					cInstance.CheckGiftPurple();
					cInstance.CheckMysticCookingUnlock();
					cInstance.FriendInstance.SetDirty();
					cInstance.FriendInstance.CheckFacebookFriendsReward(cInstance);
				}
				return cInstance;
			}

			return null;
		}

		public static string CreateNewPlayer(RequestTable.Instance cRequest)
		{
			ResponseTable.Instance cResponse = new ResponseTable.Instance(cRequest);
			if ("" == cRequest.JsonToken)
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonToken);

			JsonFxObject.Instance cJsonFxObject = new JsonFxObject.Instance(cRequest.JsonToken);
			if (!cJsonFxObject.ContainsKey("*"))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonNode, "*");

			string sFacebookID = "";
			string sGoogleID = "";

			if (cJsonFxObject.ContainsKey("*"))
				sFacebookID = cJsonFxObject.DynamicObject.f;
			if (cJsonFxObject.ContainsKey("*"))
				sGoogleID = cJsonFxObject.DynamicObject.g;

			string			sError		= "";
			Instance		cInstance	= new Instance();
			Enums.ErrorCode eResult		= cInstance.CreateNewPlayer(GetNewSuperBitID(), sFacebookID, sGoogleID, cJsonFxObject.DynamicObject.nickname, ref sError);
			if (Enums.ErrorCode.eNone != eResult)
				return cResponse.ToString(false, eResult, sError);

			cInstance.Key = GetNewKey();
			_cDicInstanceWithID.Add(cInstance.AccountID, cInstance);
			_cDicInstanceWithKey.Add(cInstance.Key, cInstance);
			_cDicInstanceWithSuperBitID.Add(cInstance.SuperBitID, cInstance);

			cResponse.AddBaseClass(cInstance);
			return cResponse.ToString(true);
		}

		public static string RemovePlayer(RequestTable.Instance cRequest)
		{
			ResponseTable.Instance cResponse = new ResponseTable.Instance(cRequest);

			if ("" == cRequest.Key)
				return cResponse.ToString(false, Enums.ErrorCode.eNeedKey);
			if (!_cDicInstanceWithKey.ContainsKey(cRequest.Key))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedLogin);

			Instance cInstance = _cDicInstanceWithKey[cRequest.Key];
			
			// remove from db
			string			sError	= "";
			Enums.ErrorCode	eResult	= MySqlUtil.RemoveRecord(cInstance.AccountID, ref sError);
			if (Enums.ErrorCode.eNone != eResult)
				return cResponse.ToString(false, eResult, sError);

			// remove cache
			if (_cDicInstanceWithKey.ContainsKey(cRequest.Key))
				_cDicInstanceWithKey.Remove(cRequest.Key);
			if (_cDicInstanceWithID.ContainsKey(cInstance.AccountID))
				_cDicInstanceWithID.Remove(cInstance.AccountID);
			if (_cDicInstanceWithSuperBitID.ContainsKey(cInstance.SuperBitID))
				_cDicInstanceWithSuperBitID.Remove(cInstance.SuperBitID);

			// write log
			MySqlUtil.WriteLog(cInstance.AccountID, Enums.LogMainType.Account, Enums.LogSubType.Remove, cInstance.SuperBitID);

			// response
			cInstance = null;
			return cResponse.ToString(true);
		}

		public static string ChangeNickName(RequestTable.Instance cRequest)
		{
			ResponseTable.Instance cResponse = new ResponseTable.Instance(cRequest);

			if ("" == cRequest.Key)
				return cResponse.ToString(false, Enums.ErrorCode.eNeedKey);
			if ("" == cRequest.JsonToken)
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonToken);
			if (!_cDicInstanceWithKey.ContainsKey(cRequest.Key))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedLogin);

			return _cDicInstanceWithKey[cRequest.Key].ChangeNickName(cRequest, cResponse);
		}

		public static string LoginWithSuperBitID(RequestTable.Instance cRequest)
		{
			ResponseTable.Instance cResponse = new ResponseTable.Instance(cRequest);
			
			if ("" == cRequest.SuperBitID)
				return cResponse.ToString(false, Enums.ErrorCode.eNeedSuperBitID);

			Instance cInstance = null;

			if (_cDicInstanceWithSuperBitID.ContainsKey(cRequest.SuperBitID))
			{
				cInstance = _cDicInstanceWithSuperBitID[cRequest.SuperBitID];
				cInstance.ShopInstance.CheckLastResetDateTime();
				
				_cDicInstanceWithKey.Remove(cInstance.Key);
				cInstance.Key = GetNewKey();
				_cDicInstanceWithKey.Add(cInstance.Key, cInstance);
			}
			else
			{
				cInstance = new Instance();
				Enums.ErrorCode eErrorCode = cInstance.LoadFromDB(cRequest.SuperBitID, "", "", "");
				if (Enums.ErrorCode.eNone != eErrorCode)
					return cResponse.ToString(false, eErrorCode);

				cInstance.Key = GetNewKey();
				_cDicInstanceWithID.Add(cInstance.AccountID, cInstance);
				_cDicInstanceWithKey.Add(cInstance.Key, cInstance);
				_cDicInstanceWithSuperBitID.Add(cInstance.SuperBitID, cInstance);
			}

			if (null != cInstance)
			{
				cInstance.CheckSpecialDateGift();
				cInstance.CheckMovieAdDateTime();
				cInstance.CheckGiftPurple();
				cInstance.CheckMysticCookingUnlock();
				cInstance.FriendInstance.SetDirty();
				cInstance.FriendInstance.CheckFacebookFriendsReward(cInstance);
			}

			cResponse.AddBaseClass(cInstance);
			return cResponse.ToString(true);
		}

		public static string LoginWithFacebookID(RequestTable.Instance cRequest)
		{
			ResponseTable.Instance cResponse = new ResponseTable.Instance(cRequest);
			
			if ("" == cRequest.SuperBitID)
				return cResponse.ToString(false, Enums.ErrorCode.eNeedSuperBitID);

			Instance cInstance = new Instance();
			Enums.ErrorCode eResult = cInstance.LoadFromDB("", cRequest.SuperBitID, "", "");
			if (Enums.ErrorCode.eNone != eResult)
				return cResponse.ToString(false, eResult);

			if (_cDicInstanceWithID.ContainsKey(cInstance.AccountID))
			{
				cInstance = _cDicInstanceWithID[cInstance.AccountID];
				cInstance.ShopInstance.CheckLastResetDateTime();
				
				if (_cDicInstanceWithKey.ContainsKey(cInstance.Key))
					_cDicInstanceWithKey.Remove(cInstance.Key);

				cInstance.Key = GetNewKey();
				_cDicInstanceWithKey.Add(cInstance.Key, cInstance);
			}
			else
			{
				cInstance.Key = GetNewKey();
				_cDicInstanceWithID.Add(cInstance.AccountID, cInstance);
				_cDicInstanceWithKey.Add(cInstance.Key, cInstance);
				_cDicInstanceWithSuperBitID.Add(cInstance.SuperBitID, cInstance);
			}

			if (null != cInstance)
			{
				cInstance.CheckSpecialDateGift();
				cInstance.CheckMovieAdDateTime();
				cInstance.CheckGiftPurple();
				cInstance.CheckMysticCookingUnlock();
				cInstance.FriendInstance.SetDirty();
				cInstance.FriendInstance.CheckFacebookFriendsReward(cInstance);
			}

			cResponse.AddBaseClass(cInstance);
			return cResponse.ToString(true);
		}

		public static string LoginWithGoogleID(RequestTable.Instance cRequest)
		{
			ResponseTable.Instance cResponse = new ResponseTable.Instance(cRequest);

			if ("" == cRequest.SParam)
				return cResponse.ToString(false, Enums.ErrorCode.eNeedStringParam);

			Instance cInstance = new Instance();
			Enums.ErrorCode eResult = cInstance.LoadFromDB("", "", cRequest.SParam, "");
			if (Enums.ErrorCode.eNone != eResult)
				return cResponse.ToString(false, eResult);

			if (_cDicInstanceWithID.ContainsKey(cInstance.AccountID))
			{
				cInstance = _cDicInstanceWithID[cInstance.AccountID];
				cInstance.ShopInstance.CheckLastResetDateTime();
				
				if (_cDicInstanceWithKey.ContainsKey(cInstance.Key))
					_cDicInstanceWithKey.Remove(cInstance.Key);

				cInstance.Key = GetNewKey();
				_cDicInstanceWithKey.Add(cInstance.Key, cInstance);
			}
			else
			{
				cInstance.Key = GetNewKey();
				_cDicInstanceWithID.Add(cInstance.AccountID, cInstance);
				_cDicInstanceWithKey.Add(cInstance.Key, cInstance);
				_cDicInstanceWithSuperBitID.Add(cInstance.SuperBitID, cInstance);
			}

			if (null != cInstance)
			{
				cInstance.CheckSpecialDateGift();
				cInstance.CheckMovieAdDateTime();
				cInstance.CheckGiftPurple();
				cInstance.CheckMysticCookingUnlock();
				cInstance.FriendInstance.SetDirty();
				cInstance.FriendInstance.CheckFacebookFriendsReward(cInstance);
			}

			cResponse.AddBaseClass(cInstance);
			return cResponse.ToString(true);
		}

		public static string FinishGame(RequestTable.Instance cRequest)
		{
			ResponseTable.Instance cResponse = new ResponseTable.Instance(cRequest);

			if ("" == cRequest.Key)
				return cResponse.ToString(false, Enums.ErrorCode.eNeedKey);
			if ("" == cRequest.JsonToken)
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonToken);
			if (!_cDicInstanceWithKey.ContainsKey(cRequest.Key))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedLogin);

			return _cDicInstanceWithKey[cRequest.Key].FinishGame(cRequest, cResponse);
		}

		public static string UpdateTutorial(RequestTable.Instance cRequest)
		{
			ResponseTable.Instance cResponse = new ResponseTable.Instance(cRequest);

			if ("" == cRequest.Key)
				return cResponse.ToString(false, Enums.ErrorCode.eNeedKey);
			if ("" == cRequest.JsonToken)
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonToken);
			if (!_cDicInstanceWithKey.ContainsKey(cRequest.Key))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedLogin);

			return _cDicInstanceWithKey[cRequest.Key].UpdateTutorial(cRequest, cResponse);
		}

		public static string UpdateMission(RequestTable.Instance cRequest)
		{
			ResponseTable.Instance cResponse = new ResponseTable.Instance(cRequest);

			if ("" == cRequest.Key)
				return cResponse.ToString(false, Enums.ErrorCode.eNeedKey);
			if ("" == cRequest.JsonToken)
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonToken);
			if (!_cDicInstanceWithKey.ContainsKey(cRequest.Key))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedLogin);

			return _cDicInstanceWithKey[cRequest.Key].UpdateMission(cRequest, cResponse);
		}

		public static string TryCook(RequestTable.Instance cRequest)
		{
			ResponseTable.Instance cResponse = new ResponseTable.Instance(cRequest);

			if ("" == cRequest.Key)
				return cResponse.ToString(false, Enums.ErrorCode.eNeedKey);
			if ("" == cRequest.JsonToken)
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonToken);
			if (!_cDicInstanceWithKey.ContainsKey(cRequest.Key))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedLogin);

			return _cDicInstanceWithKey[cRequest.Key].TryCook(cRequest, cResponse);
		}

		public static string TryMysticCook(RequestTable.Instance cRequest)
		{
			ResponseTable.Instance cResponse = new ResponseTable.Instance(cRequest);

			if ("" == cRequest.Key)
				return cResponse.ToString(false, Enums.ErrorCode.eNeedKey);
			if ("" == cRequest.JsonToken)
				return cResponse.ToString(false, Enums.ErrorCode.eNeedJsonToken);
			if (!_cDicInstanceWithKey.ContainsKey(cRequest.Key))
				return cResponse.ToString(false, Enums.ErrorCode.eNeedLogin);

			return _cDicInstanceWithKey[cRequest.Key].TryMysticCook(cRequest, cResponse);
		}
	}
}