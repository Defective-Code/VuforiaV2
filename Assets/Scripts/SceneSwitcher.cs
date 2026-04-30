using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System.Collections;

[RequireComponent(typeof(Button))]
public class SceneSwitcher : MonoBehaviour
{
    private Button sceneSwitchButton;
    public string SceneToLoad;
    public GameObject loadingPanel;
    public Slider loadingBar;

    //private AsyncOperation loadingScene;

    void Awake()
    {
        sceneSwitchButton = GetComponent<Button>();
    }

    void Start()
    {
        sceneSwitchButton.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        Debug.Log($"Switching to {SceneToLoad}");

        // reset any countdown timers in ObserverManager when we change scenes
        ObserverManager.instance.OnSceneChange();

        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        //Debug.Log("Loading the scene");

        yield return null;

        loadingPanel.SetActive(true);

        AsyncOperation loadingScene = SceneManager.LoadSceneAsync(SceneToLoad);
        loadingScene.allowSceneActivation = false;

        // loops while the scene is loading, and updates the loadingbar
        while (!loadingScene.isDone)
        {
            //Debug.Log("Looping isDone");

            loadingBar.value = Mathf.Clamp01(loadingScene.progress / 0.9f);

            if (loadingScene.progress >= 0.9f)
            {
                // The scene is done loading so we can fill the 
                loadingScene.allowSceneActivation = true;
            }

            yield return null;
        }

        
        //yield return loadingScene; // wait for loading scene to conclude
        ObserverManager.instance.isSceneChanging = false; // then set the isSceneChanging flag to false
        

    }

    //void Update()
    //{

    //    // stop exection if there is no scene that is loading
    //    if (loadingScene == null)
    //    {
    //        return;
    //    }

        
        
    //}

    
}