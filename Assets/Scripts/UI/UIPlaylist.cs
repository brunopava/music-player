using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlaylist : UIScreen
{
    public Transform contentPanel;

    public GameObject trackCellPrefab;

    private List<GameObject> _cells = new List<GameObject>();

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
        GameObject trackCell = GameObject.Instantiate(trackCellPrefab, Vector3.zero, Quaternion.identity, contentPanel) as GameObject;

        trackCell.SetActive(true);

        trackCell.GetComponent<UITrackCell>().SetTrack(track);

        _cells.Add(trackCell); 
    }

    private void PurgeCells()
    {
        if(_cells != null)
        {
            foreach(GameObject current in _cells)
            {
                GameObject.Destroy(current);
            }

        }
        _cells = new List<GameObject>(); 
    }
}
