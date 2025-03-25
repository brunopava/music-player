using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIPlayer : UIScreen
{
    private Track _currentTrack;

    public Text artistName;
    public Text trackName;
    public Text currentPlayingTime;
    public Text totalDuration;
    public Image coverImage;

    private GameObject playIcon;
    private GameObject pauseIcon;

    public Button playButton;
    public Button forwardButton;
    public Button backwardButton;
    public Button shuffleButton;
    public Button repeatButton;

    public Slider progressionSlider;

    private bool isSeeking = false;

    private void Awake()
    {
        playButton.onClick.AddListener(()=>{
            PlayPause();

            RectTransform buttonRect = playButton.GetComponent<RectTransform>();
            PUtils.AnimateButtonClick(buttonRect);
        });

        forwardButton.onClick.AddListener(()=>{
            MusicPlayerManager.Instance.NextSong();

            RectTransform buttonRect = forwardButton.GetComponent<RectTransform>();
            PUtils.AnimateButtonClick(buttonRect);
        });

        backwardButton.onClick.AddListener(()=>{
            MusicPlayerManager.Instance.PrevSong();

            RectTransform buttonRect = backwardButton.GetComponent<RectTransform>();
            PUtils.AnimateButtonClick(buttonRect);
        });

        shuffleButton.onClick.AddListener(()=>{
            MusicPlayerManager.Instance.ShufflePlayList();

            RectTransform buttonRect = shuffleButton.GetComponent<RectTransform>();
            PUtils.AnimateButtonClick(buttonRect);
        });

        repeatButton.onClick.AddListener(()=>{
            RectTransform buttonRect = repeatButton.GetComponent<RectTransform>();
            PUtils.AnimateButtonClick(buttonRect);
        });

        EventTrigger trigger = progressionSlider.GetComponent<EventTrigger>();
        EventTrigger.Entry entryDown = new EventTrigger.Entry();

        entryDown.eventID = EventTriggerType.PointerDown;
        entryDown.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data); });
        
        trigger.triggers.Add(entryDown);

        EventTrigger.Entry entryUp = new EventTrigger.Entry();

        entryUp.eventID = EventTriggerType.PointerUp;
        entryUp.callback.AddListener((data) => { OnPointerUpDelegate((PointerEventData)data); });
        
        trigger.triggers.Add(entryUp);


        playIcon = playButton.transform.GetChild(0).gameObject;
        pauseIcon = playButton.transform.GetChild(1).gameObject;

        pauseIcon.SetActive(false);
        playIcon.SetActive(true);
    }

    private void Update()
    {
        if(MusicPlayerManager.Instance.isMinimized)
            return;
            
        if(gameObject.activeSelf)
        {
            if (ANAMusic.isPlaying(MusicPlayerManager.Instance.MusicID))
            {
                int current = ANAMusic.getCurrentPosition(MusicPlayerManager.Instance.MusicID);
                int duration = ANAMusic.getDuration(MusicPlayerManager.Instance.MusicID);

                currentPlayingTime.text = PUtils.FormatTime(current);
                totalDuration.text = PUtils.FormatTime(duration);

                pauseIcon.SetActive(true);
                playIcon.SetActive(false);

                if(!isSeeking)
                    progressionSlider.value = PUtils.GetProgress(current, duration);            
            }else{
                pauseIcon.SetActive(false);
                playIcon.SetActive(true);
            }
        }
    }

    public void PlayPause()
    {
        MusicPlayerManager.Instance.PlayPause();
    }

    public void OnPointerUpDelegate(PointerEventData data)
    {
        // MusicPlayerManager.Instance.player.OnSelectProgress(false);
    }

    public void OnPointerDownDelegate(PointerEventData data)
    {
        // MusicPlayerManager.Instance.player.OnSelectProgress(true);
    }

    public void SetImageByURL(string imageUrl)
    {
        Davinci.get().load(imageUrl).into(coverImage).start();
    }

    public void SetTrack(Track track)
    {
        RefreshTrackInfo(track);

        pauseIcon.SetActive(false);
        playIcon.SetActive(true);
    }

    public void RefreshTrackInfo(Track track=null)
    {
        if(track == null)
        {
            if(_currentTrack != null)
            {
                track = _currentTrack;
            }else{
                track = MusicPlayerManager.Instance.allTracks[0];
            }
        }

        artistName.text = track.artistName;
        trackName.text = track.trackName;
        _currentTrack = track;

        SetImageByURL(_currentTrack.urlImage);

        int current = ANAMusic.getCurrentPosition(MusicPlayerManager.Instance.MusicID);
        int duration = ANAMusic.getDuration(MusicPlayerManager.Instance.MusicID);

        currentPlayingTime.text = current.ToString();
        totalDuration.text = current.ToString();
    }


    public void OnProgressionSliderDown()
    {
        isSeeking = true;
    }

    public void OnProgressionSliderUp()
    {
        int current = ANAMusic.getCurrentPosition(MusicPlayerManager.Instance.MusicID);
        int duration = ANAMusic.getDuration(MusicPlayerManager.Instance.MusicID);

        float pointer = duration * progressionSlider.value;

        ANAMusic.seekTo(MusicPlayerManager.Instance.MusicID, (int)pointer);

        isSeeking = false;
    }
}

