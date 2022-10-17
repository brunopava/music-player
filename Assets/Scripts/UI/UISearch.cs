using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UISearch : UIScreen
{
    public InputField searchbar;
    public Button search;
    public Image iconLoader;
    public Image iconError;

    public Transform resultContent;
    public GameObject trackCellPrefab;

    private List<GameObject> _cells = new List<GameObject>();


    private void Start()
    {
        iconError.gameObject.SetActive(false);
        iconLoader.gameObject.SetActive(false);
    	search.onClick.AddListener(()=>{
            PurgeCells();
    		WebRequestsManager.Instance.SearchSong(searchbar.text,OnSearchSuccess,OnSearchError, OnProgressUpdate);
            // Animate the circle outline's color and fillAmount
            iconLoader.gameObject.SetActive(true);
            iconLoader.DOColor(PUtils.RandomColor(), 1.5f).SetEase(Ease.Linear).Pause();
            iconLoader.DOFillAmount(0, 1.5f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo)
            .OnStepComplete(()=> {
                iconLoader.fillClockwise = !iconLoader.fillClockwise;
                iconLoader.DOColor(PUtils.RandomColor(), 1.5f).SetEase(Ease.Linear);
            });
    	});
    }

    private void OnProgressUpdate(float progress)
    {
        // Debug.Log(progress);
    }

    private void ResetIconLoader()
    {
        iconLoader.gameObject.SetActive(false);
        DOTween.Restart(iconLoader);
        DOTween.Kill(iconLoader);
    }

    private void OnSearchSuccess(JSONObject json)
    {
        ResetIconLoader();
    	// Debug.Log(json);
        _cells = new List<GameObject>();

        StartCoroutine(CreateCells(json));        
    }

    private IEnumerator CreateCells(JSONObject json)
    {
        for(int i = 0; i< json.Count; i++)
        {
            Track track = new Track(json[i]);

            yield return new WaitForSeconds(0.1f);
            CreateTrackCell(track);
        }
        
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(Reposition());
    }

    private IEnumerator Reposition()
    {
        yield return new WaitForSeconds(0.05f);
        ScrollRect scroll = resultContent.parent.GetComponent<ScrollRect>();
        scroll.verticalNormalizedPosition = 1000f;
    }

    private void CreateTrackCell(Track track)
    {
        GameObject trackCell = GameObject.Instantiate(trackCellPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        trackCell.transform.SetParent(resultContent);

        trackCell.GetComponent<UITrackCell>().SetTrack(track);

        _cells.Add(trackCell);  
    }

    private void OnSearchError(JSONObject json)
    {
        Debug.Log(json);
        ResetIconLoader();
        iconError.gameObject.SetActive(true);
        iconError.DOFade(1f, 0f);
        iconError.DOFade(0f, 0.5f).SetDelay(5f).OnStepComplete(()=>{
            iconError.gameObject.SetActive(false);
            iconError.DOFade(0f, 0f);
            iconError.DOFade(0f, 0.5f);
        });
    }

    private void PurgeCells()
    {
        foreach(GameObject current in _cells)
        {
            GameObject.Destroy(current);
        }

        _cells = new List<GameObject>();
    }
}
