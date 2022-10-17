using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILibrary : UIScreen
{
	public GameObject artistCellPrefab;
	public Transform contentPanel;

	public Button createPlaylist;

	public Button showArtists;
	public Button showPlaylists;

	private void Awake()
	{
		createPlaylist.onClick.AddListener(()=>{
			UIPopupLayer.Instance.ShowPopupCreatePlaylist();
		});


		showArtists.onClick.AddListener(()=>{
			ShowAllArtists();
		});

		showPlaylists.onClick.AddListener(()=>{
			ShowAllPlaylists();
		});
	}

	public void ShowAllArtists()
	{
		showArtists.interactable = false;
		showPlaylists.interactable = true;
		PurgeCells();
		StartCoroutine(AddArtists());
	}

	public void ShowAllPlaylists()
	{
		showArtists.interactable = true;
		showPlaylists.interactable = false;
		PurgeCells();
		StartCoroutine(AddPlaylists());
	}

	private IEnumerator AddArtists()
	{
		List<Artist> allArtists = MusicPlayerManager.Instance.GetAllArtists();

		if(allArtists.Count > 0)
		{
			CreatDefaultAllArtistsCell();
		}

		foreach(Artist current in allArtists)
		{
			yield return new WaitForSeconds(0.05f);
			CreateArtistCell(current);
		}

		yield return new WaitForSeconds(0.1f);
		StartCoroutine(Reposition());
	}

	private IEnumerator AddPlaylists()
	{
		foreach(Playlist current in MusicPlayerManager.Instance.playlists)
		{
			yield return new WaitForSeconds(0.05f);
			CreatePlaylistCell(current);
		}

		yield return new WaitForSeconds(0.1f);
		StartCoroutine(Reposition());
	}

	private void CreateArtistCell(Artist artist=null)
    {
        GameObject cell = UIManager.Instance.playlistCell.GetPooledObject();
        cell.SetActive(true);
        cell.GetComponent<UIPlaylistCell>().SetArtist(artist);
    }

    private void CreatePlaylistCell(Playlist pl)
    {
    	GameObject cell = UIManager.Instance.playlistCell.GetPooledObject();
        cell.SetActive(true);
        cell.GetComponent<UIPlaylistCell>().SetPlayList(pl);
    }

    private void CreatDefaultAllArtistsCell()
    {
    	GameObject cell = UIManager.Instance.playlistCell.GetPooledObject();
        cell.SetActive(true);
        cell.GetComponent<UIPlaylistCell>().SetArtist();
    }

	private IEnumerator Reposition()
    {
        yield return new WaitForSeconds(0.05f);
        ScrollRect scroll = contentPanel.parent.GetComponent<ScrollRect>();
        scroll.verticalNormalizedPosition = 1000f;
    }

    private void PurgeCells()
    {
    	UIManager.Instance.playlistCell.ResetPool();
    }
}
