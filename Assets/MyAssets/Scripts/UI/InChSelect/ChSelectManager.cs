using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임 캐릭터 선택하는 창을 관리하는 클래스.
/// </summary>
public class ChSelectManager : MonoBehaviour
{
    private JSONObject charDatajsonObj;
    private TextAsset jsonFile;
    private List<Dictionary<string, string>> jsonDataSheet;

    [SerializeField]
    private GameObject charCardPrefab;
    private int maxCharCard = 0;
    [SerializeField]
    private GameObject uiGridObj;

    private static ChSelectManager _singleton = null;
    public static ChSelectManager singleton
    {
        get
        {
            if (_singleton == null) KojeomLogger.DebugLog("ChSelectManager 제대로 초기화 되지 않았습니다", LOG_TYPE.ERROR);
            return _singleton;
        }
    }

    private CharacterData lastestSelectChar;

    public void Start()
    {
        _singleton = this;
        LoadCharDatas();
        CreateChCard();
    }

    public CharacterData GetSelectCharData()
    {
        return lastestSelectChar;
    }

    private EventDelegate Ed_OnClickChCard;
    private void OnClickChCard(CharacterData chData)
    {
        lastestSelectChar = chData;
        UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.charInfo);
    }
    
    private void LoadCharDatas()
    {
        jsonDataSheet = new List<Dictionary<string, string>>();
        jsonFile = Resources.Load(ConstFilePath.TXT_CHARACTER_DATAS) as TextAsset;
        charDatajsonObj = new JSONObject(jsonFile.text);
        AccessData(charDatajsonObj);
    }

    private void CreateChCard()
    {
        for(int idx = 0; idx < maxCharCard; ++idx)
        {
            GameObject newChCard = Instantiate(charCardPrefab,
                new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)) as GameObject;
            CharacterData chData = newChCard.GetComponent<CharacterData>();
            string tmpStr;
            jsonDataSheet[idx].TryGetValue("chName", out tmpStr);
            chData.chName = tmpStr;
            jsonDataSheet[idx].TryGetValue("chType", out tmpStr);
            chData.chType = tmpStr;
            jsonDataSheet[idx].TryGetValue("chLevel", out tmpStr);
            chData.chLevel = int.Parse(tmpStr);
            jsonDataSheet[idx].TryGetValue("detailScript", out tmpStr);
            chData.detailScript = tmpStr;
            jsonDataSheet[idx].TryGetValue("chFaceTextureName", out tmpStr);
            chData.chFaceName = tmpStr;
            chData.InitData();

            //chCard set OnClick Event
            Ed_OnClickChCard = new EventDelegate(this, "OnClickChCard");
            Ed_OnClickChCard.parameters[0].value = chData;
            newChCard.GetComponent<UIButton>().onClick.Add(Ed_OnClickChCard);

            //chCard parenting
            newChCard.transform.parent = uiGridObj.transform;
            newChCard.transform.localScale = new Vector3(1, 1, 1);
            newChCard.transform.localPosition = new Vector3(0, 0, 0);
        }
        uiGridObj.GetComponent<UIGrid>().Reposition();
    }

    private void AccessData(JSONObject jsonObj)
    {
        switch (jsonObj.type)
        {
            case JSONObject.Type.OBJECT:
                break;
            case JSONObject.Type.ARRAY:
                maxCharCard = jsonObj.Count;
                for (int idx = 0; idx < jsonObj.Count; ++idx)
                {
                    jsonDataSheet.Add(jsonObj.list[idx].ToDictionary());
                }
                break;
            default:
                Debug.Log("Json Level Data Sheet Access ERROR");
                break;
        }

    }
}
