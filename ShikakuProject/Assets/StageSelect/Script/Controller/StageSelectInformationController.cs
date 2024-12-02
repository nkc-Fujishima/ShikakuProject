using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class StageSelectInformationController : MonoBehaviour
{
    [Header("�I�u�W�F�N�g�ݒ�"), Tooltip("���[���h�l�[���I�u�W�F�N�g"), SerializeField]
    GameObject worldNameObject;
    [Tooltip("���[���h�l�[���e�L�X�g"), SerializeField] Text worldNameText;
    [Tooltip("�{�^���C���t�H���[�V�����I�u�W�F�N�g"), SerializeField] GameObject buttonInfoObject;
    [Tooltip("�{�^���C���t�H���[�V�����e�L�X�g"), SerializeField] Text buttonInfoText;

    [Header("���l�ݒ�"), Tooltip("���[���h�l�[���I�u�W�F�N�g�ړ��ʒu"), SerializeField] Vector3[] worldNamePos;
    [Tooltip("�{�^���C���t�H���[�V�����ړ��ʒu"), SerializeField] Vector3[] buttonInfoPos;
    [Tooltip("�ړ�����"), SerializeField] float moveTime;

    [Header("�e�L�X�g�ݒ�"), Tooltip("���ꂼ��̃��[���h�̖��O"), SerializeField] string[] worldName;
    [Tooltip("�X�e�[�g���̃{�^���C���t�H���[�V����"), SerializeField] string[] buttonInfo;


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
