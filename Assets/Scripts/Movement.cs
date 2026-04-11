using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 15f;
    [SerializeField] Vector2 clamp;
    Rigidbody rb;
    Vector2 move;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    public void Move(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
        Debug.Log(move);

    }
    public void HandleMovement()
    {
        Vector3 currentPosition = rb.position;
        Vector3 moveDirection = new Vector3(move.x, 0, move.y);
        Vector3 newPostion = currentPosition + moveDirection * (moveSpeed *Time.fixedDeltaTime) ;

        newPostion.x = Mathf.Clamp(newPostion.x, -clamp.x, clamp.x);
        newPostion.z = Mathf.Clamp(newPostion.z, -clamp.y, clamp.y);
        rb.MovePosition(newPostion);
    }
}
