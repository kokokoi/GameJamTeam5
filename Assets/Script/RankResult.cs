using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class RankResult : MonoBehaviour
{
    [SerializeField]
    public TMP_Text rankText;
    float minutes = 0;

    // Start is called before the first frame update
    void Start()
    {
        minutes = UITimer.GetMinutes();
        if (minutes == 0)
        {
            rankText.SetText("RANK:S");
            rankText.color = new Color(1.0f,0.9f,0.0f,1.0f);
        }
        else if (minutes == 1)
        {
            rankText.SetText("RANK:A");
            rankText.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
        else
        {
            rankText.SetText("RANK:B");
            rankText.color = new Color(0.0f, 0.0f, 1.0f, 1.0f);
        }

        rankText.transform.DOMove(new Vector3(1060, 540, 0.0f), 1.4f).SetEase(Ease.OutQuart);
    }

    
}
