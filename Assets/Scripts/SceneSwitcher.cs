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

    private AsyncOperation loadingScene;

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

        loadingPanel.SetActive(true);

        loadingScene = SceneManager.LoadSceneAsync(SceneToLoad);
    }

    void Update()
    {

        // stop exection if there is no scene that is loading
        if (loadingScene == null)
        {
            return;
        }

        loadingBar.value = Mathf.Clamp01(loadingScene.progress / 0.9f);
        
    }

    
}