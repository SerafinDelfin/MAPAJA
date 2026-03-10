using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneButton : MonoBehaviour
{
    [SerializeField]
    private int sceneIndex;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ChangeScene);
    }

    void ChangeScene()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(sceneIndex);
    }
}
