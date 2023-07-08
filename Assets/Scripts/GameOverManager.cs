using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;

    public void SubmitScore()
    {
        // var playerName = nameInput.text;
        // GameManager.Instance.SaveScore((int) AppManager.Instance.score, playerName);
        // GameManager.Instance.ToMainMenu();
    }
}
