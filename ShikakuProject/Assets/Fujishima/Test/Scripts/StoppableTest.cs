using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoppableTest : MonoBehaviour,IStoppable
{
    public void OnStop()
    {
        Debug.Log("動きが止まったよ");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
