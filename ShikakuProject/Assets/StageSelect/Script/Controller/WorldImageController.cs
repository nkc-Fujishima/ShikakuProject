using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldImageController : MonoBehaviour
{
    [SerializeField] WorldImageParameter parameter = null;

    float rotateCount = 0;
    public void Construct(WorldImageParameter parameter)
    {
        this.parameter = parameter;
    }

    // Update is called once per frame
    void Update()
    {
        rotateCount += parameter.rotateSpeed * Time.deltaTime;
        this.transform.rotation = Quaternion.Euler(0, rotateCount, 0);
    }
}