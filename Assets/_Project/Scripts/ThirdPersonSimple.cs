using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonSimple : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float turnLerp = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.2f;

    [Header("Camera")]
    public Transform cameraPivot;

    CharacterController cc;
    Vector3 velocity;
    Animator anim;

    // Animator parameter hashes
    int speedHash = Animator.StringToHash("Speed");      // float  (0 when no input, >0 when moving)
    int jumpHash = Animator.StringToHash("Jump");       // trigger (fire exactly when jump starts)
    int groundedHash = Animator.StringToHash("IsGrounded"); // bool   (true when on ground)
    int velYHash = Animator.StringToHash("VelocityY");  // float  (optional, for fall/land blends)

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();  // or GetComponent<Animator>()
    }

    void Update()
    {
        bool grounded = cc.isGrounded;

        // small stick-to-ground
        if (grounded && velocity.y < 0f) velocity.y = -2f;

        // INPUT
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 input = new Vector3(h, 0f, v);
        input = Vector3.ClampMagnitude(input, 1f);

        // CAMERA-RELATIVE MOVE
        Vector3 camF = Vector3.forward, camR = Vector3.right;
        if (cameraPivot)
        {
            camF = cameraPivot.forward; camF.y = 0; camF.Normalize();
            camR = cameraPivot.right; camR.y = 0; camR.Normalize();
        }
        Vector3 moveDir = camF * input.z + camR * input.x;

        // ROTATE toward move
        if (moveDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnLerp);
        }

        // HORIZONTAL MOVE
        Vector3 horiz = moveDir * moveSpeed;
        cc.Move(horiz * Time.deltaTime);

        // JUMP
        if (Input.GetButtonDown("Jump") && grounded && jumpHeight > 0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (anim) anim.SetTrigger(jumpHash); // 🔔 fire jump animation trigger once
        }

        // GRAVITY
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);

        // 🔧 DRIVE ANIMATOR
        if (anim)
        {
            // Run/Idle snap: full speed when input, else 0
            float planarSpeed = input.sqrMagnitude > 0.01f ? moveSpeed : 0f;
            anim.SetFloat(speedHash, planarSpeed);

            // Grounded & vertical velocity (handy for fall/land states)
            anim.SetBool(groundedHash, grounded);
            anim.SetFloat(velYHash, velocity.y);
        }
    }
}
