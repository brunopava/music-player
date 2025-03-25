using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayerManager : Singleton<MusicPlayerManager>
{
    public string musicLibraryPath;
    public string fullLibraryPath;

    public AudioSource audioSource;

    public List<Track> allTracks = new List<Track>();
    public Playlist currentPlaylist;

    public List<Playlist> playlists = new List<Playlist>();
    private Playlist _defaultPL = new Playlist();

    /**
     * 
     * TODO: 
     * CREATE PLAYLISTS, SAVE PLAYLISTS
     * CREATE LINK LISTS, SAVE LINK LISTS (TO DOWNLOAD) 
     * 
     * **/
     // Create a field for the save file.
    public JSONObject musicLibrary = new JSONObject();
    public string savePath;

    private int currentTrackIndex = 0;
    private int nextSongIndex = 1;
    private int prevSongIndex;

    private float _currentVolume = 0.5f;

    public int MusicID;

    private void Awake()
    {
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        fullLibraryPath = Application.persistentDataPath +'/'+ musicLibraryPath;
        savePath = Application.persistentDataPath + "/gamedata.json";

        Initialize();
        RefreshLibrary();

        //DeBUG;
        
        _defaultPL.playlistName = "All Tracks";
        _defaultPL.tracks = allTracks;

        SetPlayList(_defaultPL);
    }

    private void Start()
    {
        UIManager.Instance.GoToLibrary();
        AndroidNativeAudio.makePool(1);
        RemoveDuplicatesInLibrary();
    }

    public bool isMinimized;
    private void OnApplicationPause(bool minimized)
    {
        isMinimized = minimized;
    }

    public void PlaySong(Track track)
    {
        for(int i = 0; i < currentPlaylist.tracks.Count; i++)
        {
            if(track.trackName == currentPlaylist.tracks[i].trackName)
            {
                currentTrackIndex = i;
                break;
            }
        }

        if (ANAMusic.isPlaying(MusicID))
        {
            ANAMusic.pause(MusicID);
            ANAMusic.release(MusicID);
        }

        MusicID = ANAMusic.load(track.songPath, true, true, SongLoaded, true);
        ANAMusic.setVolume(MusicID, _currentVolume);

        UIManager.Instance.GoToPlayer(currentPlaylist.tracks[currentTrackIndex]);

        SetPrevAndNext();
    }

    public void SondEnded(int musicID)
    {
        NextSong();
    }

    void SongLoaded(int musicID)
    {
        ANAMusic.play(MusicID, SondEnded);

        // // Get music duration
        // DurationString = "Duration: " + ANAMusic.getDuration(musicID);

        // IsLoaded = true;
        // PlayPauseButton = "Play";
        // IsPlayingString = "Is Playing: False";
        // LoopUnloopButton = "Loop";
        // IsLoopingString = "Is Looping: False";
        // IsMute = false;
        // VolumeMuteButton = "Mute";
        // VolumeString = "Volume: 1.0";
        // IsPlayInBackground = false;
        // PlayInBackgroundString = "Play In Background: False";
    }

    public void PlayCurrentSong()
    {
        UIManager.Instance.player.PlayPause();
    }

    public void PlayPause()
    {
        // Check playing state
        if (ANAMusic.isPlaying(MusicID))
        {
            // Pause
            ANAMusic.pause(MusicID);
        }
        else
        {
            ANAMusic.play(MusicID, SondEnded);
        }
    }

    private void SetPrevAndNext()
    {
        prevSongIndex = currentTrackIndex-1;
        if(prevSongIndex < 0)
        {
            prevSongIndex = currentPlaylist.tracks.Count-1;
        }

        nextSongIndex = currentTrackIndex+1;
        if(nextSongIndex >= currentPlaylist.tracks.Count)
        {
            nextSongIndex = 0;
        }
    }

    public void NextSong()
    {   
        currentTrackIndex++;
        if(currentTrackIndex >= currentPlaylist.tracks.Count)
        {
            currentTrackIndex = 0;
        }

        SetPrevAndNext();

        ANAMusic.pause(MusicID);
        ANAMusic.release(MusicID);

        MusicID = ANAMusic.load(currentPlaylist.tracks[currentTrackIndex].songPath, true, true, SongLoaded, true);
        ANAMusic.setVolume(MusicID, _currentVolume);

        if(Application.isFocused)
        {
            UIManager.Instance.player.RefreshTrackInfo(currentPlaylist.tracks[currentTrackIndex]);
        }
    }

    public void PrevSong()
    {
        currentTrackIndex--;
        if(currentTrackIndex < 0)
        {
            currentTrackIndex = currentPlaylist.tracks.Count-1;
        }

        ANAMusic.pause(MusicID);
        ANAMusic.release(MusicID);

        MusicID = ANAMusic.load(currentPlaylist.tracks[currentTrackIndex].songPath, true, true, SongLoaded, true);
        ANAMusic.setVolume(MusicID, _currentVolume);

        SetPrevAndNext();

        UIManager.Instance.player.RefreshTrackInfo(currentPlaylist.tracks[currentTrackIndex]);
    }

    public void ShufflePlayList(Track currentTrack = null)
    {
        List<Track> mylist = currentPlaylist.tracks;
        var rnd = new System.Random();
        currentPlaylist.tracks = mylist.OrderBy(item => rnd.Next()).ToList();
        currentTrackIndex = 0;

        if(currentTrack != null)
        {
            for(int i = 0; i < currentPlaylist.tracks.Count; i++)
            {
                if(currentTrack.trackName == currentPlaylist.tracks[i].trackName)
                {
                    currentTrackIndex = i;
                    break;
                }
            }
        }
    }

    private void Initialize()
    {
        string fullPath = fullLibraryPath;

        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
            // Debug.Log("Library Directory Created: "+fullPath);
        }else{
            Debug.Log("Already Exists at: "+fullPath);
        }
    }

    public void RefreshLibrary()
    {
        allTracks = new List<Track>();
        if (File.Exists(savePath))
        {
            string fileContents = File.ReadAllText(savePath);
            musicLibrary = new JSONObject(fileContents);

            if(musicLibrary.HasField("songs"))
            {
                for(int i = 0; i < musicLibrary["songs"].Count; i++)
                {
                    Track track = new Track(musicLibrary["songs"][i]);
                    allTracks.Add(track);
                }
            }

            if(musicLibrary.HasField("playlists"))
            {
                playlists = new List<Playlist>();

                for(int j = 0; j < musicLibrary["playlists"].Count; j++)
                {
                    Playlist pl = new Playlist();
                    List<Track> tracks = new List<Track>();

                    pl.playlistName = PUtils.RemoveQuotes(musicLibrary["playlists"][j]["name"].ToString());

                    for(int k = 0; k < musicLibrary["playlists"][j]["songs"].Count; k++)
                    {
                        string songID = PUtils.RemoveQuotes(musicLibrary["playlists"][j]["songs"][k]["song_id"].ToString());
                        Track current  = GetTrackById(songID);
                        tracks.Add(current);
                    }

                    pl.tracks = tracks;
                    playlists.Add(pl);
                }
            }
        }
    }

    public Track GetTrackById(string id)
    {
        for(int i = 0; i < musicLibrary["songs"].Count; i++)
        {
            JSONObject current = musicLibrary["songs"][i];
            string songID = PUtils.RemoveQuotes(current["song_id"].ToString());
            
            if(songID == id)
            {
                return new Track(current);
            }
        }

        return null;
    }

    public void SaveLibrary()
    {
        string jsonString = musicLibrary.ToString();
        File.WriteAllText(savePath, jsonString);
    }

    public void AddToLibrary(JSONObject track)
    {
        if(musicLibrary["songs"] == null)
        {
            musicLibrary.AddField("songs", new JSONObject());
        }

        bool isDuplicate = false;
        for(int i = 0; i < musicLibrary["songs"].Count; i ++)
        {
            string songID = PUtils.RemoveQuotes(musicLibrary["songs"][i]["song_id"].ToString());
            string receivingID = PUtils.RemoveQuotes(track["song_id"].ToString());
            if(songID == receivingID)
            {
                isDuplicate = true;
            }
        }

        if(!isDuplicate)
        {
            musicLibrary["songs"].Add(track);
            SaveLibrary(); 
            RefreshLibrary();
        }        
    }

    public bool IsTrackOnLibrary(Track target)
    {
        string targetID = PUtils.RemoveQuotes(target.trackInfo["song_id"].ToString());
        if(musicLibrary != null && musicLibrary.HasField("songs"))
        {
            for(int i = 0; i < musicLibrary["songs"].Count; i++)
            {
                string currentID = PUtils.RemoveQuotes(musicLibrary["songs"][i]["song_id"].ToString());

                if(currentID == targetID)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void RemoveFromLibrary(JSONObject target)
    {
        string targetID = PUtils.RemoveQuotes(target["song_id"].ToString());

        if(musicLibrary.HasField("songs"))
        {
            JSONObject songs = new JSONObject();

            for(int i = 0; i < musicLibrary["songs"].Count; i++)
            {
                JSONObject current = musicLibrary["songs"][i];
                string id = PUtils.RemoveQuotes(current["song_id"].ToString());
                
                if(id != targetID)
                {
                    songs.Add(current);
                }
            }
            
            if(songs.Count > 0)
            {
                musicLibrary["songs"] = songs;
            }else{
                musicLibrary.RemoveField("songs");
            }
        }

        if(musicLibrary.HasField("playlists"))
        {
            JSONObject playlists = new JSONObject();

            for(int j = 0; j < musicLibrary["playlists"].Count; j++)
            {
                JSONObject current  = musicLibrary["playlists"][j]["songs"];
                JSONObject freshPlaylist = new JSONObject();
                for(int k = 0; k < current.Count; k++)
                {
                    string id  = PUtils.RemoveQuotes(current[k]["song_id"].ToString());
                    if(targetID != id)
                    {
                        freshPlaylist.Add(current[k]);
                    }
                }
                
                musicLibrary["playlists"][j]["songs"] = freshPlaylist;
            }
        }

        SaveLibrary();
        RefreshLibrary();
    }

    private void RemoveDuplicatesInLibrary()
    {
        JSONObject songs = new JSONObject();

        List<int> duplicates = new List<int>();

        if(musicLibrary != null && musicLibrary.HasField("songs"))
        {
            for(int scanIndex = 0; scanIndex < musicLibrary["songs"].Count; scanIndex ++)
            {
                for(int i = 0; i < musicLibrary["songs"].Count; i ++)
                {
                    JSONObject scanned = musicLibrary["songs"][scanIndex];
                    if(scanIndex != i)
                    {
                        if(PUtils.RemoveQuotes(scanned["song_id"].ToString()) == PUtils.RemoveQuotes(musicLibrary["songs"][i]["song_id"].ToString()))
                        {
                            duplicates.Add(i);
                        }
                    }
                }
            }

            for(int j = 0; j < musicLibrary["songs"].Count; j++)
            {
                if(!duplicates.Contains(j))
                {
                    songs.Add(musicLibrary["songs"][j]);
                }
            }

            musicLibrary["songs"] = songs;
            SaveLibrary();
            RefreshLibrary();
        }
    }

    public void CreatePlaylist(JSONObject playlist)
    {
        if(musicLibrary["playlists"] == null)
        {
            musicLibrary.AddField("playlists", new JSONObject());
        }  

        musicLibrary["playlists"].Add(playlist);
        SaveLibrary(); 
        RefreshLibrary();
    }

    public void SetPlayListFromTracks(List<Track> tracks)
    {
        Playlist pl = new Playlist();
        pl.tracks = tracks;
        pl.playlistName = tracks[0].artistName;
        currentPlaylist = pl;
    }

    public void SetDefaultPlaylist()
    {
        currentPlaylist = _defaultPL;
    }

    public void SetPlayList(Playlist playlist)
    {
        currentPlaylist = playlist;
    } 

    public void DeleteSongFile(Track track, bool removeFromLibrary = false)
    {
        if(System.IO.File.Exists(track.songPath))
        {
            System.IO.File.Delete(track.songPath);
        }

        for(int i = 0; i < musicLibrary["songs"].Count; i++)
        {
            if(track.trackName == PUtils.RemoveQuotes(musicLibrary["songs"][i]["name"].ToString()))
            {
                musicLibrary["songs"][i].RemoveField("song_path");
            }
        }

        if(removeFromLibrary)
        {
            RemoveFromLibrary(track.trackInfo);
        }else{
            SaveLibrary();
        }
    }

    public List<Artist> GetAllArtists()
    {
        List<Artist> artists = new List<Artist>();

        foreach(Track track in allTracks)
        {
            bool added = false;
            foreach(Artist artist in artists)
            {
                if(artist.artistName == track.artistName)
                {
                    artist.tracks.Add(track);
                    added = true;
                }
            }
            if(!added)
            {
                Artist newArtist = new Artist();
                newArtist.artistName = track.artistName;
                newArtist.tracks.Add(track);
                artists.Add(newArtist);
            }
        }

        return artists;
    }
}


public class Playlist{
    public string playlistName;
    private List<Track> _tracks;
    public List<Track> tracks{
        set {
            List<Track> t = new List<Track>();
            foreach(Track current in value)
            {
                string id = PUtils.RemoveQuotes(current.trackInfo["song_id"].ToString());
                t.Add(MusicPlayerManager.Instance.GetTrackById(id));
            }
            _tracks = t;
        }
        get{
            return _tracks;
        }
    }
}

public class Artist{
    public string artistName;
    public List<Track> tracks = new List<Track>();

    public Artist()
    {
        
    }
}

public class Track
{
    private JSONObject _trackInfo;
    public JSONObject trackInfo{
        get {
            return _trackInfo;
        }
    }

    public string trackName{
        get {
            return PUtils.RemoveQuotes(_trackInfo["name"].ToString());
        }
    }
    public string artistName{
        get {
            return PUtils.RemoveQuotes(_trackInfo["artist"].ToString());
        }
    }

    public string urlImage{
        get {
            return PUtils.RemoveQuotes(_trackInfo["cover_url"].ToString());
        }
    }

    public string songPath{
        get {
            if(_trackInfo["song_path"] != null)
                return PUtils.RemoveQuotes(_trackInfo["song_path"].ToString());
            else 
                return null;
        }
    }

    public Track(JSONObject json)
    {
        _trackInfo = json;
    }
}
