using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class UITrackCell : MonoBehaviour
{
    public Toggle toggler;
    public Text artistName;
    public Text trackName;
    public Image cover;

    public Image downloadProgress;
    public Image iconError;

    public Text progressText;

    public Button downloadButton;
    public Button optionsButton;

    private bool _readyToPlay;

    private Track _track;

    public GameObject optionsPanel;

    public Button removeFromPlaylist;
    public Button addToPlaylist;
    public Button deleteFile;
    public Button removeFromLibrary;

    private void Awake()
    {
        optionsButton.gameObject.SetActive(false);
        downloadProgress.gameObject.SetActive(false);
        iconError.gameObject.SetActive(false);
        progressText.gameObject.SetActive(false);
        optionsPanel.SetActive(false);
        toggler.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        optionsButton.onClick.RemoveAllListeners();
        downloadButton.onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.RemoveAllListeners();

        removeFromPlaylist.onClick.RemoveAllListeners();
        addToPlaylist.onClick.RemoveAllListeners();
        deleteFile.onClick.RemoveAllListeners();
        removeFromLibrary.onClick.RemoveAllListeners();

        optionsPanel.GetComponent<Button>().onClick.RemoveAllListeners();

        optionsPanel.SetActive(false);
        _track = null;
    }

    public void SetTrack(Track track)
    {   
        _track = track;
        foreach(Track current in MusicPlayerManager.Instance.allTracks)
        {
            if(current.trackName == track.trackName)
            {
                _track = current;
                break;
            }
        }

        trackName.text = _track.trackName;
        artistName.text = _track.artistName;

        Davinci.get().load(_track.urlImage).into(cover).start();

        if(_track.trackInfo.HasField("song_path"))
        {
            _readyToPlay = true;
            optionsButton.gameObject.SetActive(true);
            downloadButton.gameObject.SetActive(false);
        }else{
            bool isInLibrary = MusicPlayerManager.Instance.IsTrackOnLibrary(_track);
            _readyToPlay = false;

            if(!isInLibrary)
            {
                optionsButton.gameObject.SetActive(false);
                downloadButton.gameObject.SetActive(true);
            }else{
                optionsButton.gameObject.SetActive(true);
                downloadButton.gameObject.SetActive(true);
            }
        }

        optionsButton.onClick.AddListener(()=>
        {
            optionsPanel.SetActive(true);
            SetupOptions();
        });

        GetComponent<Button>().onClick.AddListener(()=>{
            if(_readyToPlay)
            {
                Debug.Log(_track.trackName);
                MusicPlayerManager.Instance.PlaySong(_track);
            }else{
                //TODO: DOWNLOAD AND PLAY
                //WebRequestsManager.Instance.DownloadSong(_track.trackInfo, OnDownloadComplete, OnDownloadError, OnProgressUpdate);
            }
        });

        downloadButton.onClick.AddListener(()=>{
            isDownloading = true;
            ShowProgress();
            WebRequestsManager.Instance.DownloadSong(_track.trackInfo, OnDownloadComplete, OnDownloadError, OnProgressUpdate);
        });
    }

    private bool isDownloading = false;

    public void SetupOptions()
    {
        removeFromPlaylist.onClick.AddListener(()=>{
            optionsPanel.SetActive(false);
        }); 
        addToPlaylist.onClick.AddListener(()=>{
            optionsPanel.SetActive(false);
        }); 
        deleteFile.onClick.AddListener(()=>{
            optionsPanel.SetActive(false);
            MusicPlayerManager.Instance.DeleteSongFile(_track);
        });

        removeFromLibrary.onClick.AddListener(()=>{
            optionsPanel.SetActive(false);
            MusicPlayerManager.Instance.DeleteSongFile(_track, true);
        });

        optionsPanel.GetComponent<Button>().onClick.AddListener(()=>{
            optionsPanel.SetActive(false);
        });
    }

    private void ShowProgress()
    {
        downloadButton.gameObject.SetActive(false);
        downloadProgress.gameObject.SetActive(true);
        progressText.gameObject.SetActive(true);
        progressText.text = "0%";

        // Animate the circle outline's color and fillAmount
        downloadProgress.DOColor(PUtils.RandomColor(), 1.5f).SetEase(Ease.Linear).Pause();
            downloadProgress.DOFillAmount(0, 1.5f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo)
            .OnStepComplete(()=> {
                downloadProgress.fillClockwise = !downloadProgress.fillClockwise;
            downloadProgress.DOColor(PUtils.RandomColor(), 1.5f).SetEase(Ease.Linear);
        }); 
    }

    private void OnDownloadComplete(JSONObject json)
    {
        isDownloading = false;
        MusicPlayerManager.Instance.AddToLibrary(json);
        DOTween.Kill(downloadProgress);
        downloadProgress.DOFade(0f, 0.5f);
        optionsButton.gameObject.SetActive(true);
        progressText.gameObject.SetActive(false);
    }

    private void OnDownloadError(JSONObject json)
    {
        isDownloading = false;
        Debug.Log(json);
        DOTween.Kill(downloadProgress);
        downloadProgress.gameObject.SetActive(false);
        iconError.gameObject.SetActive(true);
        progressText.gameObject.SetActive(false);
        iconError.DOFade(1f, 0f);

        downloadProgress.DOFillAmount(1, 0);
        downloadProgress.DOColor(Color.white, 0);

        iconError.DOFade(0f, 0.5f).SetDelay(5f).OnStepComplete(()=>{
            iconError.gameObject.SetActive(false);
            downloadButton.gameObject.SetActive(true);
            iconError.DOFade(0f, 0f);
            iconError.DOFade(0f, 0.5f);
        });
    }

    private void OnProgressUpdate(float progress)
    {
        if(!isMinimized)
        {
            progressText.text = progress.ToString("n2") + "%";
        }
    }

    public bool isMinimized;
    private void OnApplicationPause(bool minimized)
    {
        isMinimized = minimized;
        Debug.Log(isMinimized);
        if(isMinimized)
        {
            DOTween.KillAll();
        }else{
            if(isDownloading)
            {
                downloadProgress.DOFillAmount(1, 0);
                downloadProgress.DOColor(Color.white, 0);
                downloadProgress.fillClockwise = true;

                // Animate the circle outline's color and fillAmount
                downloadProgress.DOColor(PUtils.RandomColor(), 1.5f).SetEase(Ease.Linear).Pause();
                    downloadProgress.DOFillAmount(0, 1.5f)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Yoyo)
                    .OnStepComplete(()=> {
                        downloadProgress.fillClockwise = !downloadProgress.fillClockwise;
                        downloadProgress.DOColor(PUtils.RandomColor(), 1.5f).SetEase(Ease.Linear);
                }); 
            }
        }
    }
}
