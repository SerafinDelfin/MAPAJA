using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(QuitApp);
    }

    void QuitApp()
    {
        Application.Quit();
    }
}
