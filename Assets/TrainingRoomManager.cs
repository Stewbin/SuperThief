using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrainingRoomManager : MonoBehaviour
{
    public GameObject DummyPrefab;
    public int DummyCount { get; private set; }
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
    public TMP_Text counter;


    public void TogglePlay()
    {
        if (_isPlaying) // Pause
        {
            _isPlaying = false;
            DummyBehaviour.Dummies.ForEach(dummy => dummy.StopMoving());

            PlayButton.sprite = PlaySymbol;
        }
        else // Play
        {
            _isPlaying = true;
            DummyBehaviour.Dummies.ForEach(dummy => dummy.StartMoving(true));

            PlayButton.sprite = PauseSymbol;
        }
    }

    private void SpawnDummy()
    {
        float x = Random.Range(-LeftBound, RightBound);
        float z = Random.Range(-LowerBound, UpperBound);

        Vector3 spawnPt = transform.position;
        spawnPt.x += x;
        spawnPt.z += z;
        spawnPt.y += 0.05f;

        Instantiate(DummyPrefab, spawnPt, Quaternion.identity);
    }

    private void DestroyDummy()
    {
        var instances = DummyBehaviour.Dummies;
        if (instances.Count > 0)
        {
            int i = Random.Range(0, instances.Count - 1);
            Destroy(instances[i].gameObject);
            instances.RemoveAt(i);
        }
    }

    public void IncrementDummyCounter()
    {
        if (DummyCount < 21)
        {
            DummyCount++;
            SpawnDummy();
        }
    }

    public void DecrementDummyCounter()
    {
        if (DummyCount > 0)
        {
            DummyCount--;
            DestroyDummy();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.forward * UpperBound);
        Gizmos.DrawRay(transform.position, Vector3.back * LowerBound);
        Gizmos.DrawRay(transform.position, Vector3.right * RightBound);
        Gizmos.DrawRay(transform.position, Vector3.left * LeftBound);
    }
}
