using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

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
    private bool bHasHitWall = false;
    private float mTimeLeftGrounded = float.MinValue;
    private float mTimeOfJump = float.MinValue;
    private Vector2 mFrameVelocityVector;
    private FrameInput mFrameInput;

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
    }

    void Update()
    {
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
        Vector2 beforeCollision = mFrameVelocityVector;

        CheckForVerticalCollisions();
        HandleJump();
        HandleGravity();

        mRigidbody2D.linearVelocityY = mFrameVelocityVector.y;

        CheckForHorizontalCollisions();
        HandleXDirection();
        mRigidbody2D.linearVelocityX = mFrameVelocityVector.x;
    }
    private void CheckForVerticalCollisions()
    {
        Vector2 verticalVector = new Vector2(0, mRigidbody2D.linearVelocityY);
        verticalVector.Normalize();

        RaycastHit2D hit = Physics2D.CapsuleCast(
            mCapsuleCollider.bounds.center,
            mCapsuleCollider.size,
            mCapsuleCollider.direction,
            0,
            verticalVector,
            mPlayerData.RaycastDistance,
            mPlayerData.HittableLayers);

        if (hit)
        {
            HandleIntersection(hit);

            if (!mPlayerData.IsGrounded && IsVerticalIntersection(hit))
            {
                if (verticalVector.y < 0)
                {
                    mPlayerData.IsGrounded = true;
                    mPlayerData.IsCoyoteTimeUsable = true;
                    mPlayerData.IsBufferedJumpUsable = true;
                    mPlayerData.EndedJumpEarly = false;
                    // TODO: Can hook up events here for vfx etc
                }
                else if (verticalVector.y > 0)
                {
                    mFrameVelocityVector.y = Mathf.Min(0, mRigidbody2D.linearVelocityY);
                }
            }
        }
        else
        {
            if (mPlayerData.IsGrounded)
            {
                mPlayerData.IsGrounded = false;
                mTimeLeftGrounded = Time.time;
                // TODO: Another event here on leaving 
            }
        }
    }
    private void CheckForHorizontalCollisions()
    {
        Vector2 horizontalVector = new Vector2(mRigidbody2D.linearVelocityX, 0);
        horizontalVector.Normalize();

        RaycastHit2D hit = Physics2D.CapsuleCast(
            mCapsuleCollider.bounds.center,
            mCapsuleCollider.size,
            mCapsuleCollider.direction,
            0,
            horizontalVector,
            mPlayerData.RaycastDistance,
            mPlayerData.HittableLayers);

        if (hit)
        {
            HandleIntersection(hit);
            if (IsHorizontalIntersection(hit))
            {
                mFrameInput.mInputVector.x = 0;
                bHasHitWall = true;
            }
        }
        else
        {
            bHasHitWall = false;
        }
    }
    private bool IsVerticalIntersection(RaycastHit2D hit)
    {
        return (hit.normal.x == 0) && (hit.normal.y != 0);
    }
    private bool IsHorizontalIntersection(RaycastHit2D hit)
    {
        return (hit.normal.x != 0) && (hit.normal.y == 0);
    }
    private void HandleIntersection(RaycastHit2D hit)
    {
        float intersectPenetration = mPlayerData.RaycastDistance - hit.distance;
        if (intersectPenetration > mPlayerData.PlayerCapsuleOffset)
        {
            Vector2 correction = hit.normal * (intersectPenetration - mPlayerData.PlayerCapsuleOffset);
            mRigidbody2D.transform.position += (Vector3)correction;
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

        // TODO: Event here for sfx/vfx
    }

    private void HandleXDirection()
    {
        if (mFrameInput.mInputVector.x == 0)
        {
            if (bHasHitWall)
            {
                mFrameVelocityVector.x = 0;
            }
            else
            {
                float deceleration = mPlayerData.IsGrounded ? mPlayerData.GroundDeceleration : mPlayerData.AirDeceleration;
                mFrameVelocityVector.x = Mathf.MoveTowards(mFrameVelocityVector.x, 0, deceleration * Time.fixedDeltaTime);
            }
        }
        else
        {
            float acceleration = mPlayerData.IsGrounded ? mPlayerData.GroundAcceleration : mPlayerData.AirAcceleration;
            mFrameVelocityVector.x = Mathf.MoveTowards(mFrameVelocityVector.x, mFrameInput.mInputVector.x * mPlayerData.MovementSpeed, acceleration * Time.fixedDeltaTime);
        }
    }

    private void HandleGravity()
    {
        if (mPlayerData.IsGrounded && mFrameVelocityVector.y <= 0.0f)
        {
            mFrameVelocityVector.y = mPlayerData.GroundingForce;
        }
        else
        {
            float gravityEffect = mPlayerData.GravityStrength;

            if (mFrameInput.mInputVector.y < 0.0f)
            {
                gravityEffect = mPlayerData.VerticalAcceleration;
            }
            else if (mFrameInput.mInputVector.y > 0.0f)
            {
                gravityEffect = mPlayerData.VerticalDeceleration;
            }

            if (mPlayerData.EndedJumpEarly && mFrameVelocityVector.y > 0)
            {
                gravityEffect *= mPlayerData.JumpCutMultiplier;
            }

            mFrameVelocityVector.y = Mathf.MoveTowards(mFrameVelocityVector.y, -mPlayerData.TerminalVelocity, gravityEffect * Time.fixedDeltaTime);
        }
    }
}
