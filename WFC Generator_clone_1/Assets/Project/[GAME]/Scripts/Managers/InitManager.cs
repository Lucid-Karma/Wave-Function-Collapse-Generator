using UnityEngine;
using UnityEngine.SceneManagement;

public class InitManager : MonoBehaviour
{
    void Start()
    {
        //Init Game Here
        SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        SceneManager.LoadScene("Game", LoadSceneMode.Additive);
        Destroy(gameObject);
    }
}
