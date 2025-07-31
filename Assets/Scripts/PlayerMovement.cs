using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private struct FrameInput
    {
        public Vector2 mInputVector;
        public bool bIsJumpPressedDown;
        public bool bIsJumpHeldDown;
    }
    [SerializeField]
    private PlayerMovementData mPlayerData;
    private Rigidbody2D mRigidbody2D;
    private CapsuleCollider2D mCapsuleCollider;
    private bool bStartQueriesInColliders;
    private float mTimeLeftGrounded = float.MinValue;
    private float mTimeOfJump = float.MinValue;
    private Vector2 mFrameVelocityVector;
    private FrameInput mFrameInput;
    // TODO: Coyote Time
    // TODO: Jump buffering
    // TODO: Slope Handling
    // TODO: Wall Jumping/sliding? Maybe? 
    void Start()
    {
        if (!mPlayerData)
        {
            Debug.LogAssertion("PlayerMovementData is not attached to PlayerMovement.");
            return;
        }

        mRigidbody2D = GetComponent<Rigidbody2D>();
        if (!mRigidbody2D)
        {
            Debug.LogAssertion("There is no Rigidbody2D attached to Player GameObject.");
            return;
        }

        if (mRigidbody2D.bodyType != RigidbodyType2D.Kinematic)
        {
            Debug.LogWarning("Rigidbody2D is not set to kinematic. We Should change this in editor.");
            mRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        }

        mCapsuleCollider = GetComponent<CapsuleCollider2D>();
        if (!mCapsuleCollider)
        {
            Debug.LogAssertion("There is no CapsuleCollider2D attached to Player GameObject.");
            return;
        }
        bStartQueriesInColliders = Physics2D.queriesStartInColliders;
    }

    // Update is called once per frame
    void Update()
    {
        // Handle Input
        float XInput = Input.GetAxisRaw("Horizontal");
        float YInput = Input.GetAxisRaw("Vertical");

        mFrameInput.mInputVector = new Vector2(XInput, YInput);
        mFrameInput.bIsJumpPressedDown = Input.GetButtonDown("Jump");
        mFrameInput.bIsJumpHeldDown = Input.GetButton("Jump");

        if (mFrameInput.bIsJumpPressedDown)
        {
            mPlayerData.JumpToBeConsumed = true;
            mTimeOfJump = Time.time;
        }
    }

    private void FixedUpdate()
    {
        CheckForCollisions();
        HandleJump();
        HandleXDirection();
        HandleGravity();

        mRigidbody2D.linearVelocity = mFrameVelocityVector;
    }

    private void CheckForCollisions()
    {
        //So we don't hit ourselves
        Physics2D.queriesStartInColliders = false;

        bool groundHit = false;
        RaycastHit2D hit = Physics2D.CapsuleCast(
            mCapsuleCollider.bounds.center,
            mCapsuleCollider.size,
            mCapsuleCollider.direction,
            0,
            Vector2.down,
            mPlayerData.RaycastDistance,
            mPlayerData.HittableLayers);
        DebugDrawCapsuleCast2D(mCapsuleCollider.bounds.center, mCapsuleCollider.size, mCapsuleCollider.direction, 0, Vector2.down, mPlayerData.RaycastDistance, hit, Color.red);
        if (hit.collider != null)
        {
            groundHit = true;
        }

        bool ceilingHit = false;
        hit = Physics2D.CapsuleCast(
            mCapsuleCollider.bounds.center,
            mCapsuleCollider.size,
            mCapsuleCollider.direction,
            0,
            Vector2.up,
            mPlayerData.RaycastDistance,
            ~mPlayerData.HittableLayers);
        DebugDrawCapsuleCast2D(mCapsuleCollider.bounds.center, mCapsuleCollider.size, mCapsuleCollider.direction, 0, Vector2.up, mPlayerData.RaycastDistance, hit, Color.red);
        if (hit.collider != null)
        {
            ceilingHit = true;
        }

        if (ceilingHit)
        {
            mFrameVelocityVector.y = Mathf.Min(0, mRigidbody2D.linearVelocityY);
        }

        if (!mPlayerData.IsGrounded && groundHit)
        {
            mPlayerData.IsGrounded = true;
            mPlayerData.IsCoyoteTimeUsable = true;
            // Coyote could be useable here
            mPlayerData.IsBufferedJumpUsable = true;
            mPlayerData.EndedJumpEarly = false;
            // Can hook up events here for vfx etc
        }
        else if (mPlayerData.IsGrounded && !groundHit)
        {
            mPlayerData.IsGrounded = false;
            mTimeLeftGrounded = Time.time;
            // Another event here on leaving 
        }
        Physics2D.queriesStartInColliders = bStartQueriesInColliders;
    }

    void DebugDrawCapsuleCast2D(Vector2 origin, Vector2 size, CapsuleDirection2D direction, float angle, Vector2 castDirection, float distance, RaycastHit2D hit, Color color, int segments = 16)
    {
        // Normalize cast direction
        Vector2 dir = castDirection.normalized;

        // Calculate start and end positions
        Vector2 start = origin;
        Vector2 end = origin + dir * distance;

        // Draw start capsule
        DrawCapsule2D(start, size, direction, color, segments);

        // Draw cast line
        Debug.DrawLine(start, end, color);

        // Draw end capsule (full extent)
        DrawCapsule2D(end, size, direction, color * 0.5f, segments);

        // Draw hit capsule if any
        if (hit.collider != null)
        {
            DrawCapsule2D(hit.point, size, direction, Color.red, segments);
        }
    }
    void DrawCapsule2D(Vector2 center, Vector2 size, CapsuleDirection2D direction, Color color, int segments = 8)
    {
        float radius, height;
        if (direction == CapsuleDirection2D.Vertical)
        {
            radius = size.x / 2f;
            height = size.y;
        }
        else
        {
            radius = size.y / 2f;
            height = size.x;
        }

        float sideLength = Mathf.Max(0f, height - 2 * radius);

        Vector2 up = direction == CapsuleDirection2D.Vertical ? Vector2.up : Vector2.right;
        Vector2 right = direction == CapsuleDirection2D.Vertical ? Vector2.right : Vector2.up;

        Vector2 topCenter = center + up * (sideLength / 2);
        Vector2 bottomCenter = center - up * (sideLength / 2);

        // Straight lines
        Debug.DrawLine(bottomCenter - right * radius, topCenter - right * radius, color);
        Debug.DrawLine(bottomCenter + right * radius, topCenter + right * radius, color);

        // Semi-circles
        DrawSemiCircle(topCenter, radius, 0, 180, direction, color, segments);
        DrawSemiCircle(bottomCenter, radius, 180, 360, direction, color, segments);
    }

    void DrawSemiCircle(Vector2 center, float radius, float startAngle, float endAngle, CapsuleDirection2D direction, Color color, int segments)
    {
        float angleStep = (endAngle - startAngle) / segments;
        for (int i = 0; i < segments; i++)
        {
            float angle1 = Mathf.Deg2Rad * (startAngle + i * angleStep);
            float angle2 = Mathf.Deg2Rad * (startAngle + (i + 1) * angleStep);

            Vector2 dir1 = new Vector2(Mathf.Cos(angle1), Mathf.Sin(angle1));
            Vector2 dir2 = new Vector2(Mathf.Cos(angle2), Mathf.Sin(angle2));

            if (direction == CapsuleDirection2D.Horizontal)
            {
                // Swap X and Y for horizontal
                dir1 = new Vector2(dir1.y, dir1.x);
                dir2 = new Vector2(dir2.y, dir2.x);
            }

            Vector2 point1 = center + dir1 * radius;
            Vector2 point2 = center + dir2 * radius;

            Debug.DrawLine(point1, point2, color);
        }
    }
    private void HandleJump()
    {
        if (!mPlayerData.EndedJumpEarly &&
            !mPlayerData.IsGrounded &&
            !mFrameInput.bIsJumpHeldDown &&
            mRigidbody2D.linearVelocityY > 0)
        {
            mPlayerData.EndedJumpEarly = true;
        }

        if (!mPlayerData.JumpToBeConsumed &&
            !HasBufferedJump())
        {
            return;
        }

        if (mPlayerData.IsGrounded || CanUseCoyoteTime())
        {
            Jump();
        }

        // We have consumed our jump
        mPlayerData.JumpToBeConsumed = false;
    }

    private bool HasBufferedJump()
    {
        return (mPlayerData.IsBufferedJumpUsable && (Time.time < mTimeOfJump + mPlayerData.BufferedJumpTime));
    }

    private bool CanUseCoyoteTime()
    {
        return (mPlayerData.IsCoyoteTimeUsable && (Time.time < mTimeLeftGrounded + mPlayerData.CoyoteTimeFrame));
    }

    private void Jump()
    {
        mPlayerData.EndedJumpEarly = false;
        mTimeOfJump = 0; // Technically idk if we rlly need to reset this but to be safe
        mPlayerData.IsBufferedJumpUsable = false;
        mPlayerData.IsCoyoteTimeUsable = false;
        mFrameVelocityVector.y = mPlayerData.JumpForce;

        // Event here for sfx/vfx
    }

    private void HandleXDirection()
    {
        if (mFrameInput.mInputVector.x == 0)
        {
            float deceleration = mPlayerData.IsGrounded ? mPlayerData.GroundDeceleration : mPlayerData.AirDeceleration;
            mFrameVelocityVector.x = Mathf.MoveTowards(mFrameVelocityVector.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            float acceleration = mPlayerData.IsGrounded ? mPlayerData.GroundAcceleration : mPlayerData.AirAcceleration;
            mFrameVelocityVector.x = Mathf.MoveTowards(0, mFrameInput.mInputVector.x * mPlayerData.MovementSpeed, acceleration * Time.fixedDeltaTime);
        }
    }

    private void HandleGravity()
    {
        if (mPlayerData.IsGrounded && mFrameVelocityVector.y <= 0.0f)
        {
            mFrameVelocityVector.y = 0;
        }
        else
        {
            float gravityEffect = mPlayerData.GravityStrength;
            // Fast fall
            if (mFrameInput.mInputVector.y < 0.0f)
            {
                Debug.Log("Accelerating fall " + mPlayerData.VerticalAcceleration);
                gravityEffect = mPlayerData.VerticalAcceleration;
            }
            else if (mFrameInput.mInputVector.y > 0.0f) // "Slower fall"
            {
                Debug.Log("Slowing fall " + mPlayerData.VerticalDeceleration);
                gravityEffect = mPlayerData.VerticalDeceleration;
            }

            if (mPlayerData.EndedJumpEarly && mFrameVelocityVector.y > 0)
            {
                Debug.Log("Jump Ended Early, half jump");
                gravityEffect *= mPlayerData.JumpCutMultiplier;
            }

            Debug.Log("GravityEffect: " + gravityEffect);
            mFrameVelocityVector.y = Mathf.MoveTowards(mFrameVelocityVector.y, -mPlayerData.TerminalVelocity, gravityEffect * Time.fixedDeltaTime);
        }
    }
}
