using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaker : MonoBehaviour
{
    void Start()
    {
    }

    // �J�������V�F�C�N���郁�\�b�h
    public void ShakeCamera()
    {
        var impulseSource = GetComponent<CinemachineImpulseSource>();
        impulseSource.GenerateImpulse();
        
    }
}
