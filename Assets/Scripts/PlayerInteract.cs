using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerInteract : MonoBehaviour
{
    public Camera camera;
    [SerializeField] public float distance = 5f;
    [SerializeField] public LayerMask mask;
    private PlayerUI playerUI;

    void Start()
    {
        camera = GetComponent<CameraLook>().GetComponent<Camera>();
        playerUI = GetComponent<PlayerUI>();
    }

    // Update is called once per frame
    void Update()
    {
        playerUI.UpdateText(string.Empty);
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                playerUI.UpdateText(interactable.promptMessage);
                Debug.Log(interactable.promptMessage);
            }
        }
    }
}