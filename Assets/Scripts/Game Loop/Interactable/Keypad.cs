using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keypad : Interactable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected override IEnumerator Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);

        // Implement keypad interaction logic here
        yield return StartCoroutine(base.Interact());
    }
}