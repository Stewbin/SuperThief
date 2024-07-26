using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrainingRoomManager : MonoBehaviour
{
    public GameObject DummyPrefab;
    public int DummyCount {get; private set;}
    [Header("Bounds")]
    public float UpperBound;
    public float LowerBound;
    public float LeftBound;
    public float RightBound;
    [Header("Play Buttons")]
    public Image PlayButton;
    public Sprite PlaySymbol;
    public Sprite PauseSymbol;
    private bool isPlaying;
    public TMP_Text counter;


    public void TogglePlay()
    {
        if (isPlaying) // Pause
        {
            isPlaying = false;
            DummyBehaviour.Dummies.ForEach(dummy => StopCoroutine(dummy.MoveLeftAndRight()));

            PlayButton.sprite = PauseSymbol;
        }
        else // Play
        {
            isPlaying = true;
            DummyBehaviour.Dummies.ForEach(dummy => StartCoroutine(dummy.MoveLeftAndRight()));

            PlayButton.sprite = PlaySymbol;
        }
    }

    private void SpawnDummy()
    {
        float x = Random.Range(LeftBound, RightBound);
        float z = Random.Range(UpperBound, LowerBound);

        Vector3 spawnPt = new(x, transform.position.y, z);
        Instantiate(DummyPrefab, spawnPt, Quaternion.identity);
    }

    public void IncrementDummyCount()
    {
        if (DummyCount < 21)
        {
            DummyCount++;
        }
    }

    public void DecrementDummyCount()
    {
        if(DummyCount > 0)
        {
            DummyCount--;
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
