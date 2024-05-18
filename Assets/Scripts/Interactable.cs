using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string promptMessage;
    public float interactionDuration = 1f;

    public void BaseInteract()
    {
        StartCoroutine(Interact());
    }

    protected virtual IEnumerator Interact()
    {
        // Implement interaction logic here
        yield return new WaitForSeconds(interactionDuration);
    }
}