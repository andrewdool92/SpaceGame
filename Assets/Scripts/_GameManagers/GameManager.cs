using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void OnApplicationStart()
    {
        var activeScene = SceneManager.GetActiveScene();
        if (activeScene.name != "PersistentScene")
        {
            SceneManager.LoadScene("PersistentScene", LoadSceneMode.Additive);
        }
    }
}
