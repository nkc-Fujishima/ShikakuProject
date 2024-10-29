using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class StageSelectManager : MonoBehaviour
{
    [Header("オブジェクト設定"),Tooltip("ワールドイメージオブジェクト"),SerializeField]GameObject[] stageObjects;
    [Tooltip("ワールドイメージのパラメータ"), SerializeField] WorldImageParameter parameter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
