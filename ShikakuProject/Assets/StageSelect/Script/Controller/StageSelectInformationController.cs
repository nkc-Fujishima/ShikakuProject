using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class StageSelectInformationController : MonoBehaviour
{
    [Header("オブジェクト設定"), Tooltip("ワールドネームオブジェクト"), SerializeField]
    GameObject worldNameObject;
    [Tooltip("ワールドネームテキスト"), SerializeField] Text worldNameText;
    [Tooltip("ボタンインフォメーションオブジェクト"), SerializeField] GameObject buttonInfoObject;
    [Tooltip("ボタンインフォメーションテキスト"), SerializeField] Text buttonInfoText;

    [Header("数値設定"), Tooltip("ワールドネームオブジェクト移動位置"), SerializeField] Vector3[] worldNamePos;
    [Tooltip("ボタンインフォメーション移動位置"), SerializeField] Vector3[] buttonInfoPos;
    [Tooltip("移動時間"), SerializeField] float moveTime;

    [Header("テキスト設定"), Tooltip("それぞれのワールドの名前"), SerializeField] string[] worldName;
    [Tooltip("ステート毎のボタンインフォメーション"), SerializeField] string[] buttonInfo;


    public void SetWorldSelectStatePosition()
    {
        worldNameObject.GetComponent<RectTransform>().DOAnchorPos(worldNamePos[0], moveTime).SetLink(gameObject);
        buttonInfoObject.GetComponent<RectTransform>().DOAnchorPos(buttonInfoPos[0], moveTime).SetLink(gameObject);
    }

    public void SetStageSelectStatePosition()
    {
        worldNameObject.GetComponent<RectTransform>().DOAnchorPos(worldNamePos[1], moveTime).SetLink(gameObject);
        buttonInfoObject.GetComponent<RectTransform>().DOAnchorPos(buttonInfoPos[1], moveTime).SetLink(gameObject);
    }

    public void SetWorldName(int worldObjectCount)
    {
        worldNameText.text = worldName[worldObjectCount];
    }

    public void SetWorldSelectButtonInfo()
    {
        buttonInfoText.text = buttonInfo[0];
    }

    public void SetStageSelectButtonInfo()
    {
        buttonInfoText.text = buttonInfo[1];
    }
}
