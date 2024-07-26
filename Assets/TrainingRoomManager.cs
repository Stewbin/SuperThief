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
    public GameObject Buttonz;
    public Image PlaySymbol;
    public Image PauseSymbol;
    private bool isPlaying;
    public TMP_Text counter;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        if (isPlaying)
        {

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
