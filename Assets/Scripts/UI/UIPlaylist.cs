using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlaylist : UIScreen
{
    public Transform contentPanel;

    public GameObject trackCellPrefab;

    private void OnEnable()
    {
        PurgeCells();
        StartCoroutine(CreateTrackCells());
    }

    public IEnumerator CreateTrackCells()
    {
        List<Track> tracks = MusicPlayerManager.Instance.currentPlaylist.tracks;

        for(int i = 0; i < tracks.Count; i++)
        {
            yield return new WaitForSeconds(0.01f);
            CreateCell(tracks[i], i);
        }

        yield return new WaitForSeconds(0.05f);
        ScrollRect scroll = contentPanel.parent.GetComponent<ScrollRect>();
        scroll.verticalNormalizedPosition = 1000f;
    }

    private void CreateCell(Track track, int index)
    {
        GameObject trackCell = UIManager.Instance.trackCell.GetPooledObject();
        trackCell.SetActive(true);
        trackCell.transform.SetParent(contentPanel);

        trackCell.GetComponent<UITrackCell>().SetTrack(track);
    }

    private void PurgeCells()
    {
        UIManager.Instance.trackCell.ResetPool();
        // if(_cells != null)
        // {
        //     foreach(GameObject current in _cells)
        //     {
        //         GameObject.Destroy(current);
        //     }

        // }
        // _cells = new List<GameObject>(); 
    }
}
