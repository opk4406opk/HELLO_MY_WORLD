using UnityEngine;
using System.Collections;

public class CharacterData : MonoBehaviour {

    private string _chName;
    public string chName
    {
        set { _chName = value; }
        //get { return _chName; }
    }
    private int _chLevel;
    public int chLevel
    {
        set { _chLevel = value; }
        //get { return _chLevel; }
    }

    private string _detailScript;
    public string detailScript
    {
        set { _detailScript = value; }
        //get { return _detailScript; }
    }

    private string _chType;
    public string chType
    {
        set { _chType = value; }
        //get { return _chType; }
    }

    private string _chFaceName;
    public string chFaceName
    {
        set { _chFaceName = value; }
        //get { return _chFace; }
    }
    private Texture faceTexture;

    [SerializeField]
    private UILabel lbl_name;
    [SerializeField]
    private UITexture txt_chFace;

    private string loadPath = "Texture(RT)/";


    public void InitData()
    {
        lbl_name.text = _chName;
        SetTexture(_chFaceName);
        txt_chFace.mainTexture = faceTexture;
    }

    private void SetTexture(string _textureName)
    {
        switch(_textureName)
        {
            case "RT_FireFighter_00":
                faceTexture = Resources.Load(loadPath + _textureName) as Texture;
                break;
            case "RT_Gang_01":
                faceTexture = Resources.Load(loadPath + _textureName) as Texture;
                break;
            case "RT_Police_02":
                faceTexture = Resources.Load(loadPath + _textureName) as Texture;
                break;
            case "RT_Sheriff_03":
                faceTexture = Resources.Load(loadPath + _textureName) as Texture;
                break;
            case "RT_Trucker_04":
                faceTexture = Resources.Load(loadPath + _textureName) as Texture;
                break;
        }
    }


}
