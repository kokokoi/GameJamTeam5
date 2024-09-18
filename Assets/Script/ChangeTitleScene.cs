using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeTitleScene : MonoBehaviour
{
    public void OnClickTitleButton()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
