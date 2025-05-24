using UnityEngine;

using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartButton()
    {
        Debug.Log("Game Started");
        // 关卡1
        SceneManager.LoadScene("Level1");

    }

    // Update is called once per frame
}
