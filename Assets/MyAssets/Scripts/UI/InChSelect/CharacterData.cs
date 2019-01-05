using UnityEngine;
using System.Collections;

/// <summary>
/// 게임 캐릭터 선택창에 등록되어 있는 캐릭터 카드들의 상태를 저장하고 있는 클래스.
/// </summary>
public class CharacterData : MonoBehaviour {

    public string chName { set; get; }
    public int chLevel { set; get; }
    public string detailScript { set; get; }
    public string chType { set; get; }

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

    public void InitData()
    {
        lbl_name.text = chName;
        SetTexture(_chFaceName);
        txt_chFace.mainTexture = faceTexture;
    }

    private void SetTexture(string _textureName)
    {
        string filePath = string.Format("{0}texture2D_{1}", ConstFilePath.CHAR_TEXTURE2D_RESOURCES_PATH, _textureName);
		faceTexture = Resources.Load(filePath) as Texture;
	}
}
