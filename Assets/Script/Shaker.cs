using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaker : MonoBehaviour
{
    void Start()
    {
    }

    // カメラをシェイクするメソッド
    public void ShakeCamera()
    {
        var impulseSource = GetComponent<CinemachineImpulseSource>();
        impulseSource.GenerateImpulse();
        
    }
}
