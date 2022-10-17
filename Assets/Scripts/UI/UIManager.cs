using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Keyur.Components.ObjectPooling;

public delegate void BackButtonEvent ();

public class UIManager : Singleton<UIManager>
{
	private List<BackButtonEvent> _eventStack = new List<BackButtonEvent>();

	public UIPlayer player;
	public UIPlaylist playlist;
	public UILibrary library;
	public UISearch search;
	public UISettings settings;
	public UIVault vault;

	public Button vaultButton;
	public Button backButton;
	public Button libraryButton;
	public Button playlistsButton;
	public Button searchButton;
	public Button settingsButton;
	public Button currentSongButton;

	public ObjectPoolerSimple trackCell;
	public ObjectPoolerSimple playlistCell;

	public Text screenTitle;

	private List<UIScreen> _screens;
	private UIScreen lastScreen;

	private void Awake()
	{
		backButton.onClick.AddListener(()=>{
			if(_eventStack.Count>0)
			{
				_eventStack[_eventStack.Count-1]();
				_eventStack.RemoveAt(_eventStack.Count-1);
			}
		});

		vaultButton.onClick.AddListener(()=>{
			GoToVault();
		});

		currentSongButton.onClick.AddListener(()=>{
			ShowPlayer();
		});

		libraryButton.onClick.AddListener(()=>{
			GoToLibrary();
		});

		playlistsButton.onClick.AddListener(()=>{
			GoToPlaylist();
		});

		searchButton.onClick.AddListener(()=>{
			GoToSearch();
		});

		settingsButton.onClick.AddListener(()=>{
			GoToSettings();
		});

		_screens = new List<UIScreen>();
		_screens.Add(library);
		_screens.Add(player);
		_screens.Add(playlist);
		_screens.Add(settings);
		_screens.Add(search);
		_screens.Add(vault);
	}

	private void DisableAllScreens()
	{
		foreach(UIScreen current in _screens)
		{
			if(current.gameObject.activeSelf)
			{
				lastScreen = current;
			}
			current.gameObject.SetActive(false);
		}
	}
	
	public void GoToPlaylist()
	{
		// Debug.Log("playlistsButton");
		DisableAllScreens();
		playlist.gameObject.SetActive(true);
		screenTitle.text = MusicPlayerManager.Instance.currentPlaylist.playlistName;
	}

	public void GoToSettings()
	{
		// Debug.Log("settingsButton");
		DisableAllScreens();
		settings.gameObject.SetActive(true);
		screenTitle.text = "Settings";
		settings.Refresh();
	}

	public void GoToPlayer(Track track)
	{
		// Debug.Log("currentSongButton");
		DisableAllScreens();
		player.gameObject.SetActive(true);
		screenTitle.text = "Now Playing";
		player.SetTrack(track);
	}

	public void ShowPlayer()
	{
		DisableAllScreens();
		player.gameObject.SetActive(true);
		screenTitle.text = "Now Playing";
		UIManager.Instance.player.RefreshTrackInfo();
	}

	public void GoToSearch()
	{
		DisableAllScreens();
		search.gameObject.SetActive(true);
		screenTitle.text = "Search";
	}

	public void GoToLibrary()
	{
		DisableAllScreens();
		library.gameObject.SetActive(true);
		screenTitle.text = "Library";
		library.ShowAllArtists();
	}

	public void GoToVault()
	{
		DisableAllScreens();
		vault.gameObject.SetActive(true);
		screenTitle.text = "Vault";
	}

	public void AddBackButtonEvent(BackButtonEvent backEvent)
	{
		_eventStack.Add(backEvent);
	}
}
