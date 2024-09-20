using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class TimeMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.DOMove(new Vector3(696,870, 0f), 1.0f).SetEase(Ease.OutQuart);
    }

    
}
