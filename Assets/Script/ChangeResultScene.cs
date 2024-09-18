using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeResultScene : MonoBehaviour
{
    public void OnClickResultButton()
    {
        SceneManager.LoadScene("ResultScene");
    }
}
