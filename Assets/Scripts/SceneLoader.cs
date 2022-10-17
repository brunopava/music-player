using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using DG.Tweening;

public class SceneLoader : Singleton<SceneLoader> 
{
    private bool _currentlyLoading = false;

    [SerializeField]
    private int scene;
    [SerializeField]
    private Text loadingText;

    public Image circleOutline;

    public GameObject panel;

    private Image _loaderBackground;
    private RectTransform _iconRectTransform;
    private GameObject _loaderIcon;
    private GameObject _loaderText;

    private string _lastSceneLoaded = "";
    private string _currentSceneToLoad = "";
    private string _currentLoadedScene = "";


    public List<string> sceneHistory;

    private Vector3 _initialIconPosition;

    private void Awake()
    {
        // _loaderBackground = panel.GetComponent<Image>();
        // _loaderIcon = panel.transform.GetChild(0).gameObject;
        // _loaderText = panel.transform.GetChild(1).gameObject;

        // _iconRectTransform = _loaderIcon.GetComponent<RectTransform>();
        // _initialIconPosition = _iconRectTransform.localPosition;

        panel.SetActive(false);

        sceneHistory = new List<string>();

        LoadScene("Main");
    }

    public void NavigateToScene(string sceneName)
    {
        NotificationCenter.DefaultCenter.AddObserver(this, "OnSceneUnloadComplete");
        _currentSceneToLoad = sceneName;
        UnloadLastScene();
    }

    private void OnSceneUnloadComplete()
    {
        StartCoroutine(RemoveNotification());
        LoadScene(_currentSceneToLoad);
    }

    private IEnumerator RemoveNotification()
    {
        yield return new WaitForSeconds(0.1f);
        
        NotificationCenter.DefaultCenter.RemoveObserver(this, "OnSceneUnloadComplete");
    }

    public void LoadScene(string sceneName, bool isOverlay = false)
    {
        NotificationCenter.DefaultCenter.PostNotification(this, "SelfDestructOnSceneChange");

        if(_currentlyLoading)
        {
            Debug.LogWarning("SceneLoader: Tried to load a scene before another operation finish");
            return;
        }

        _currentlyLoading = true;
        // loadingText.text = "Loading...";
        
        StartCoroutine(LoadNewScene(sceneName, isOverlay));
    }

    public void UnloadScene(string sceneName)
    {
        if(_currentlyLoading)
        {
            Debug.LogWarning("SceneLoader: Tried to unload a scene before another operation finish");
            return;
        }

        _currentlyLoading = true;
        // loadingText.text = "Loading...";

        StartCoroutine(UnloadTargetScene(sceneName));
    }

    public void UnloadLastScene()
    {
        if(_currentlyLoading)
        {
            Debug.LogWarning("SceneLoader: Tried to unload a scene before another operation finish");
            return;
        }

        _currentlyLoading = true;
        // loadingText.text = "Loading....";

        StartCoroutine(UnloadTargetScene(_lastSceneLoaded));
    }

    private void Update() 
    {   
        // if(_currentlyLoading)
        // {
        //     loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
        // }
    }

    private IEnumerator LoadNewScene(string sceneName, bool isOverlay = false) 
    {
        _currentLoadedScene = sceneName;
        sceneHistory.Add(sceneName);

        OnOperationStart();        

        if(!isOverlay)
        {
            _lastSceneLoaded = sceneName;
        }

        yield return new WaitForSeconds(0.1f);
        
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!async.isDone) {
            yield return null;
        }

        yield return new WaitForSeconds(1.1f);

        OnOperationComplete();
        NotificationCenter.DefaultCenter.PostNotification(this, "OnSceneLoadComplete");
    }
    
    private IEnumerator UnloadTargetScene(string sceneName)
    {
        OnOperationStart();

        yield return new WaitForSeconds(0.1f);

        AsyncOperation async = SceneManager.UnloadSceneAsync(sceneName);

        while (!async.isDone) {
            yield return null;
        }

        yield return new WaitForSeconds(1.1f);

        OnOperationComplete();
        NotificationCenter.DefaultCenter.PostNotification(this, "OnSceneUnloadComplete");
    }

    public void ActivatePreemptiveLoad()
    {
        panel.SetActive(true);
    }

    public void DeactivatePreemptiveLoad()
    {
        panel.SetActive(false);
    }

    private void OnOperationStart()
    {
        _currentlyLoading = true;
        panel.SetActive(true);

        // Animate the circle outline's color and fillAmount
        circleOutline.DOColor(PUtils.RandomColor(), 1.5f).SetEase(Ease.Linear).Pause();
        circleOutline.DOFillAmount(0, 1.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo)
            .OnStepComplete(()=> {
                circleOutline.fillClockwise = !circleOutline.fillClockwise;
                circleOutline.DOColor(PUtils.RandomColor(), 1.5f).SetEase(Ease.Linear);
            });

        // _loaderBackground.enabled = true;
        // _loaderText.SetActive(true);
        // _loaderIcon.SetActive(true);
        // _iconRectTransform.DORotate(new Vector3(0f,0f,-360f),1f, RotateMode.FastBeyond360);
    }

    private void OnOperationComplete()
    {
        DOTween.Restart(circleOutline);
        DOTween.Kill(circleOutline);
        panel.SetActive(false);
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(0.5f);
        // _iconRectTransform.localPosition = _initialIconPosition;
        // _loaderBackground.enabled = true;
        // _loaderText.SetActive(true);
        // panel.SetActive(false);
    }
}