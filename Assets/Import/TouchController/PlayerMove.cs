using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;
using System;

public class PlayerMove : MonoBehaviourPunCallbacks
{
    public static PlayerMove instance;
    public FixedJoystick joystick;
    public float SpeedMove = 5f;
    public CharacterController controller;

    private float Gravity = -9.81f;
    public float GroundDistance = 0.3f;

    public float jumpHeight = 3f;
    public Transform Ground;

    public LayerMask layerMask;

    public bool isGrounded = true;
    public bool Pressed;

    public Animator anim;

    Vector3 velocity;

    public GameObject playerModel;

    public Transform modelGunPoint;
    public Transform gunHolder;

    public Material[] allSkins;

    public int moneyCollected;

    [Header("Detect Player Implementation")]

    public Camera camera;

    public GameObject crosshairA;
    public GameObject crosshairB;

    [Header("Name On Player Implementations Test")]
    [SerializeField] public string nickname;
    [SerializeField] public TMP_Text nicknameUIText;
    [SerializeField] public Vector3 nicknameOffset = new Vector3(0f, 2f, 0f);

    [Header("Sway")]
    public float step = 0.01f;
    public float maxStepDistance = 0.06f;
    Vector3 swayPos;

    [Header("Sway Rotation")]
    public float rotationStep = 4f;
    public float maxRotationStep = 5f;
    Vector3 swayEulerRot;

    public float smooth = 10f;
    float smoothRot = 12f;

    [Header("Bobbing")]
    public float speedCurve;
    float curveSin { get => Mathf.Sin(speedCurve); }
    float curveCos { get => Mathf.Cos(speedCurve); }

    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;
    Vector3 bobPosition;

    public float bobExaggeration;

    [Header("Bob Rotation")]
    public Vector3 multiplier;
    Vector3 bobEulerRotation;

    Vector2 walkInput;
    Vector2 lookInput;

    // Add this variable
    private bool sway = true;

    public void Awake()
    {
        instance = this;

    }

    void Start()
    {
        if (photonView.IsMine)
        {
            controller = GetComponent<CharacterController>();
            playerModel.SetActive(true);
            photonView.RPC("SetNicknameUI", RpcTarget.All, PlayerPrefs.GetString("USERNAME"));
            nicknameUIText.transform.position = transform.position + nicknameOffset;
            moneyCollected = 0;
            crosshairB.SetActive(false);
            crosshairA.SetActive(true);
        }
        else
        {
            gunHolder.parent = modelGunPoint;
            gunHolder.localPosition = Vector3.zero;
            gunHolder.localRotation = Quaternion.identity;
        }

        playerModel.GetComponent<Renderer>().material = allSkins[photonView.Owner.ActorNumber % allSkins.Length];
    }

    void Update()
    {

        if (photonView.IsMine)
        {
            isGrounded = Physics.CheckSphere(Ground.position, GroundDistance, layerMask);
            Debug.Log($"Is grounded is {isGrounded}");
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            // Use joystick input for movement
            walkInput = new Vector2(joystick.Horizontal, joystick.Vertical);
            Vector3 Move = transform.right * walkInput.x + transform.forward * walkInput.y;
            controller.Move(Move * SpeedMove * Time.deltaTime);

            if (isGrounded && Pressed)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * Gravity);
                isGrounded = false;
            }

            velocity.y += Gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            anim.SetBool("grounded", isGrounded);
            anim.SetFloat("speed", Move.magnitude);

            // Add input for look direction (you might need to implement this based on your input system)
            // For example: lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            Sway();
            SwayRotation();
            BobOffset();
            BobRotation();

            CompositePositionRotation();
            //detect [layer raycast
            DetectPlayer();
        }
    }

    [PunRPC]
    public void SetNicknameUI(string _name)
    {
        nickname = _name;
        nicknameUIText.text = PlayerPrefs.GetString("USERNAME");
    }

    [PunRPC]
    public void CollectMoney(int amount)
    {
        moneyCollected += amount;
        UIController.instance.currentMoneyAmount.text = "Money: $" + moneyCollected.ToString();
        Debug.Log("Successfully added money: " + moneyCollected);
    }

    public void Aim()
    {
        // Implement aiming logic here
    }

    void Sway()
    {
        if (!sway)
        {
            swayPos = Vector3.zero;
            return;
        }

        Vector3 invertLook = lookInput * -step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook;
    }

    void SwayRotation()
    {
        Vector2 invertLook = lookInput * -rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);
        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }

    void CompositePositionRotation()
    {
        gunHolder.localPosition = Vector3.Lerp(gunHolder.localPosition, swayPos + bobPosition, Time.deltaTime * smooth);
        gunHolder.localRotation = Quaternion.Slerp(gunHolder.localRotation, Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation), Time.deltaTime * smoothRot);
    }

    void BobOffset()
    {
        speedCurve += Time.deltaTime * (isGrounded ? (walkInput.magnitude * bobExaggeration) : 1f) + 0.01f;

        bobPosition.x = (curveCos * bobLimit.x * (isGrounded ? 1 : 0)) - (walkInput.x * travelLimit.x);
        bobPosition.y = (curveSin * bobLimit.y) - (walkInput.y * travelLimit.y);
        bobPosition.z = -(walkInput.y * travelLimit.z);
    }

    void BobRotation()
    {
        bobEulerRotation.x = (walkInput != Vector2.zero ? multiplier.x * (Mathf.Sin(2 * speedCurve)) : multiplier.x * (Mathf.Sin(2 * speedCurve) / 2));
        bobEulerRotation.y = (walkInput != Vector2.zero ? multiplier.y * curveCos : 0);
        bobEulerRotation.z = (walkInput != Vector2.zero ? multiplier.z * curveCos * walkInput.x : 0);
    }

    public void DetectPlayer()
    {

        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        ray.origin = camera.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetPhotonView().IsMine)
            {
                UIController.instance.detectPlayerText.text = "Player has been detected";
                crosshairB.SetActive(true);
                crosshairA.SetActive(false);
            }
            else
            {
                UIController.instance.detectPlayerText.text = "Player has not been detected yet";
                crosshairB.SetActive(false);
                crosshairA.SetActive(true);
            }

        }

    }
}
