using UnityEngine;
using System.Collections;

/// <summary>
/// 게임 캐릭터 선택창에 등록되어 있는 캐릭터 카드들의 상태를 저장하고 있는 클래스.
/// </summary>
public class CharacterData : MonoBehaviour {

    public string CharacterName { set; get; }
    public int CharacterLevel { set; get; }
    public string DetailScript { set; get; }
    public string CharacterType { set; get; }
    
    private Texture faceTexture;

    [SerializeField]
    private UILabel lbl_name;
    [SerializeField]
    private UITexture Texture_Character;

    public void InitData()
    {
        lbl_name.text = CharacterName;
        SetTexture(CharacterName);
    }

    private void SetTexture(string textureName)
    {
        string filePath = string.Format("{0}texture2D_{1}", ConstFilePath.CHAR_TEXTURE2D_RESOURCES_PATH, textureName);
		faceTexture = Resources.Load(filePath) as Texture;
        Texture_Character.mainTexture = faceTexture;
    }
}
