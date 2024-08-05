using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrainingRoomManager : MonoBehaviour
{
    public DummyBehaviour DummyPrefab;
    public List<DummyBehaviour> DummiesInRoom = new();
    [Header("Bounds")]
    public float UpperBound;
    public float LowerBound;
    public float LeftBound;
    public float RightBound;
    [Header("Play Buttons")]
    public Image PlayButton;
    public Sprite PlaySymbol;
    public Sprite PauseSymbol;
    private bool _isPlaying = false;
    [SerializeField] private TMP_Text _screenCounter;
    [Header("Other buttons")]
    public GameObject PlusButton;
    public GameObject MinusButton;

    #region Play button
    // Will go on UI button
    public void TogglePlay()
    {
        // Respawn all dummies
        DummiesInRoom.ForEach(dummy => dummy.gameObject.SetActive(true));

        if (_isPlaying) // Pause
        {
            _isPlaying = false;
            DummiesInRoom.ForEach(dummy => dummy.StopMoving());
            PlayButton.sprite = PlaySymbol;

            // Clear vanquish counter
            _screenCounter.text = "_";
        }
        else // Play
        {
            _isPlaying = true;
            DummiesInRoom.ForEach(dummy => dummy.StartMoving());

            PlayButton.sprite = PauseSymbol;

            // Init vanquish counter
            _screenCounter.text = "0";
        }

        // Shut off +/- buttons when playing
        PlusButton.SetActive(!_isPlaying);
        MinusButton.SetActive(!_isPlaying);
    }

    public void SpawnDummy()
    {
        float x = Random.Range(-LeftBound, RightBound);
        float z = Random.Range(-LowerBound, UpperBound);

        Vector3 spawnPt = transform.position;
        spawnPt.x += x;
        spawnPt.z += z;
        spawnPt.y += 0.05f;

        DummyBehaviour dummy = Instantiate(DummyPrefab, spawnPt, Quaternion.identity);
        Debug.Assert(dummy.transform.position.Equals(spawnPt), "Not spawning at proper spot");
        // Register with manager
        dummy.Manager = this;
        DummiesInRoom.Add(dummy);
    }

    public void DespawnDummy()
    {
        if (DummiesInRoom.Count > 0)
        {
            int i = Random.Range(0, DummiesInRoom.Count - 1);
            Destroy(DummiesInRoom[i].gameObject);
            // Should only deregister dummies on despawn
            DummiesInRoom.RemoveAt(i);
        }
    }
    #endregion

    #region Record score
    private int _dummiesVanquished = 0;
    public void IncrementVanquishCounter()
    {
        Debug.Assert(DummiesInRoom.Count >= _dummiesVanquished, "Negative dummies in room");

        _dummiesVanquished++;
        _screenCounter.text = _dummiesVanquished.ToString();
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.forward * UpperBound);
        Gizmos.DrawRay(transform.position, Vector3.back * LowerBound);
        Gizmos.DrawRay(transform.position, Vector3.right * RightBound);
        Gizmos.DrawRay(transform.position, Vector3.left * LeftBound);
    }
}
