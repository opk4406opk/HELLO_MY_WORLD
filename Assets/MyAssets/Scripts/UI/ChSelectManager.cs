using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

    private EventDelegate OnClickChCard;
    private void ClickChCard(CharacterData chData)
    {
        SceneManager.LoadSceneAsync("popup_chInfo", LoadSceneMode.Additive);

        GameObject sceneToSceneData = GameObject.Find("SceneToScene_datas");
        sceneToSceneData.GetComponent<SceneToScene_Data>().Init();
        sceneToSceneData.GetComponent<SceneToScene_Data>().SetData("chName", chData.chName);
        sceneToSceneData.GetComponent<SceneToScene_Data>().SetData("chLevel", chData.chLevel.ToString());
        sceneToSceneData.GetComponent<SceneToScene_Data>().SetData("chType", chData.chType);
        sceneToSceneData.GetComponent<SceneToScene_Data>().SetData("detailScript", chData.detailScript);
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
            OnClickChCard = new EventDelegate(this, "ClickChCard");
            OnClickChCard.parameters[0].value = chData;
            newChCard.GetComponent<UIButton>().onClick.Add(OnClickChCard);

            //chCard parenting
            newChCard.transform.parent = uiGridObj.transform;
            newChCard.transform.localScale = new Vector3(1, 1, 1);
            newChCard.transform.localPosition = new Vector3(0, 0, 0);
            uiGridObj.GetComponent<UIGrid>().Reposition();
        }
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
