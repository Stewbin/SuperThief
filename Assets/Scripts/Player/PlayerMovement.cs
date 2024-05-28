
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    public bool isGrounded;
    public float gravity = -9.8f;
    public Joystick joystick;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 3f;

    // Start is called before the first frame update
    void Start()
    {

        //Cursor.lockState = CursorLockMode.Locked; 
        controller = GetComponent<CharacterController>();

        Transform newTrans = SpawnManager.instance.GetSpawnPoints();
        transform.position = newTrans.position;
        transform.rotation = newTrans.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;

        // Get the input from the joystick
        Vector2 input = joystick.Direction;

        // Move the player
        Move(input);
    }

    // This function is responsible to receive inputs for our InputManager.cs and apply them to our character controller
    public void Move(Vector2 input)
    {
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);

        playerVelocity.y += gravity * Time.deltaTime;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        controller.Move(playerVelocity * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    public void Jump()
    {
        playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        print("Jumping");
    }
}