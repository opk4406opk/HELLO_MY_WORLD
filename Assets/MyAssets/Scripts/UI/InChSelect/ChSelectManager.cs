using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임 캐릭터 선택하는 창을 관리하는 클래스.
/// </summary>
public class ChSelectManager : MonoBehaviour
{
    private JSONObject chDatajsonObj;
    private TextAsset jsonFile;
    private List<Dictionary<string, string>> jsonDataSheet;

    [SerializeField]
    private GameObject chCardPrefab;
    private int maxChCard = 0;
    [SerializeField]
    private GameObject uiGridObj;

    public void Start()
    {
        LoadChDatas();
        CreateChCard();
    }

    private EventDelegate Ed_OnClickChCard;
    private void OnClickChCard(CharacterData chData)
    {
        GameObject sceneToSceneData = GameObject.Find("SceneToScene_datas");
        SceneToScene_Data.gameChDatas.Clear();
        SceneToScene_Data.gameChDatas.Add("chName", chData.chName);
        SceneToScene_Data.gameChDatas.Add("chLevel", chData.chLevel.ToString());
        SceneToScene_Data.gameChDatas.Add("chType", chData.chType);
        SceneToScene_Data.gameChDatas.Add("detailScript", chData.detailScript);

        SceneManager.LoadSceneAsync("popup_chInfo", LoadSceneMode.Additive);
    }
    
    private void LoadChDatas()
    {
        jsonDataSheet = new List<Dictionary<string, string>>();
        jsonFile = Resources.Load("TextAsset/ChDatas/characterDatas") as TextAsset;
        chDatajsonObj = new JSONObject(jsonFile.text);
        AccessData(chDatajsonObj);
    }

    private void CreateChCard()
    {
        for(int idx = 0; idx < maxChCard; ++idx)
        {
            GameObject newChCard = Instantiate(chCardPrefab,
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
                //for(int idx = 0; idx < jsonObj.Count; ++idx)
                //    AccessData(jsonObj);
                break;
            case JSONObject.Type.ARRAY:
                maxChCard = jsonObj.Count;
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
