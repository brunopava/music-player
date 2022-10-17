using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlaylistCell : MonoBehaviour
{
    public Text playlistName;
    public Image coverImage;

    public void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
    }

    public void SetArtist(Artist artist=null)
    {
        if(artist != null)
        {
            playlistName.text = artist.artistName;

            Davinci.get().load(artist.tracks[0].urlImage).into(coverImage).start();
            
            GetComponent<Button>().onClick.AddListener(()=>{
                MusicPlayerManager.Instance.SetPlayListFromTracks(artist.tracks);
                UIManager.Instance.GoToPlaylist();
            });
        }else{
            playlistName.text = "All Artists";
            Davinci.get().load(MusicPlayerManager.Instance.allTracks[0].urlImage).into(coverImage).start();
            //TODO: BETTER IMAGING

            GetComponent<Button>().onClick.AddListener(()=>{
                MusicPlayerManager.Instance.SetDefaultPlaylist();
                UIManager.Instance.GoToPlaylist();
                // MusicPlayerManager.Instance.PlaySong(artist.tracks[0]);
            });
        }
    }

    public void SetPlayList(Playlist pl)
    {
       if(pl != null)
        {
            playlistName.text = pl.playlistName;

            if(pl.tracks.Count > 0)
            {
                Davinci.get().load(pl.tracks[0].urlImage).into(coverImage).start();

                GetComponent<Button>().onClick.AddListener(()=>{
                    MusicPlayerManager.Instance.SetPlayList(pl);
                    UIManager.Instance.GoToPlaylist();
                });
            }
        } 
    }
}
