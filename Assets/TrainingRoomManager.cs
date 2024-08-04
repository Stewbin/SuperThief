using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrainingRoomManager : MonoBehaviour
{
    public DummyBehaviour DummyPrefab;
    public List<DummyBehaviour> DummiesInRoom;
    [Header("Bounds")]
    public float UpperBound;
    public float LowerBound;
    public float LeftBound;
    public float RightBound;
    [Header("Play Buttons")]
    public Image PlayButton;
    public Sprite PlaySymbol;
    public Sprite PauseSymbol;
    private bool _isPlaying;
    public TMP_Text ScreenCounter {get; private set;}
    [Header("Other buttons")]
    public GameObject PlusButton;
    public GameObject MinusButton;

    #region Play button
    // Will go on UI button
    public void TogglePlay()
    {
        if (_isPlaying) // Pause
        {
            _isPlaying = false;
            DummiesInRoom.ForEach(dummy => dummy.StopMoving());

            PlayButton.sprite = PlaySymbol;

            // Clear vanquish counter
            ScreenCounter.text = "_";
        }
        else // Play
        {
            _isPlaying = true;
            DummiesInRoom.ForEach(dummy => dummy.StartMoving(true));

            PlayButton.sprite = PauseSymbol;

            // Init vanquish counter
            ScreenCounter.text = "0";
        }

        // Shut off +/- buttons when playing
        PlusButton.SetActive(!_isPlaying);
        MinusButton.SetActive(!_isPlaying);
    }

    private void SpawnDummy()
    {
        float x = Random.Range(-LeftBound, RightBound);
        float z = Random.Range(-LowerBound, UpperBound);

        Vector3 spawnPt = transform.position;
        spawnPt.x += x;
        spawnPt.z += z;
        spawnPt.y += 0.05f;

        DummyBehaviour dummy = Instantiate(DummyPrefab, spawnPt, Quaternion.identity);
        // Register with manager
        dummy.Manager = this;
        DummiesInRoom.Add(dummy);
    }

    private void DespawnDummy()
    {
        if (DummiesInRoom.Count > 0)
        {
            int i = Random.Range(0, DummiesInRoom.Count - 1);
            Destroy(DummiesInRoom[i].gameObject);
            DummiesInRoom.RemoveAt(i);
        }
    }
    #endregion

    #region Record score
    public int DummiesVanquished = 0;
    public void IncrementVanquishCounter()
    {
        Debug.Assert(DummiesInRoom.Count <= DummiesVanquished, "Negative dummies in room");
        
        DummiesVanquished++;
        ScreenCounter.text = DummiesVanquished.ToString();
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
