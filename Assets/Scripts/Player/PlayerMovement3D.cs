using UnityEngine;

public class PlayerMovement3D : MonoBehaviour
{
    private struct FrameInput
    {
        public Vector3 mInputVector;
        public bool bIsJumpPressedDown;
        public bool bIsJumpHeldDown;
    }
    [SerializeField]
    private PlayerMovementData mPlayerData;
    private Rigidbody mRigidbody;
    private CapsuleCollider mCapsuleCollider;
    private bool bHasHitWall = false;
    private bool bWasLastHorizontalDirectionPositive = false;
    private float mTimeLeftGrounded = float.MinValue;
    private float mTimeOfJump = float.MinValue;
    private Vector3 mFrameVelocityVector;
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

        mRigidbody = GetComponent<Rigidbody>();
        if (!mRigidbody)
        {
            Debug.LogAssertion("There is no Rigidbody attached to Player GameObject.");
            return;
        }

        //if (mRigidbody.bodyType != RigidbodyType2D.Kinematic)
        //{
        //    Debug.LogWarning("Rigidbody2D is not set to kinematic. We Should change this in editor.");
        //    mRigidbody.bodyType = RigidbodyType2D.Kinematic;
        //}

        mCapsuleCollider = GetComponent<CapsuleCollider>();
        if (!mCapsuleCollider)
        {
            Debug.LogAssertion("There is no CapsuleCollider attached to Player GameObject.");
            return;
        }
    }

    void Update()
    {
        float XInput = Input.GetAxisRaw("Horizontal");
        float ZInput = Input.GetAxisRaw("Vertical");

        mFrameInput.mInputVector = new Vector3(XInput, 0.0f, ZInput);
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
        HandleXDirection();
        HandleZDirection();
        HandleJump();
        HandleGravity();

        mRigidbody.linearVelocity = mFrameVelocityVector;
    }

    private void CheckForCollisions()
    {
        RaycastHit groundHit;
        bool hit = Physics.Raycast(
            mCapsuleCollider.bounds.center,
            Vector3.down,
            out groundHit,
            mPlayerData.RaycastDistance,
            mPlayerData.HittableLayers);

        Debug.DrawRay(mCapsuleCollider.bounds.center, Vector3.down, Color.magenta);

        if (hit)
        {
            HandleIntersection(groundHit);

            if (!mPlayerData.IsGrounded && IsVerticalIntersection(groundHit))
            {
                mPlayerData.IsGrounded = true;
                mPlayerData.IsCoyoteTimeUsable = true;
                mPlayerData.IsBufferedJumpUsable = true;
                mPlayerData.EndedJumpEarly = false;
                // TODO: Can hook up events here for vfx etc
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

        RaycastHit ceilingHit;
        hit = Physics.Raycast(
            mCapsuleCollider.bounds.center,
            Vector3.down,
            out ceilingHit,
            mPlayerData.RaycastDistance,
            mPlayerData.HittableLayers);

        Debug.DrawRay(mCapsuleCollider.bounds.center, Vector3.up, Color.yellow);

        if (hit)
        {
            HandleIntersection(ceilingHit);
            if (IsVerticalIntersection(ceilingHit))
            {
                mFrameVelocityVector.y = Mathf.Min(0, mRigidbody.linearVelocity.y);
            }
        }
    }
    private bool IsVerticalIntersection(RaycastHit hit)
    {
        return (hit.normal.x == 0) && (hit.normal.y != 0);
    }
    private bool IsHorizontalIntersection(RaycastHit hit)
    {
        return (hit.normal.x != 0) && (hit.normal.y == 0);
    }
    private void HandleIntersection(RaycastHit hit)
    {
        float intersectPenetration = mPlayerData.RaycastDistance - hit.distance;
        if (intersectPenetration > mPlayerData.PlayerCapsuleOffset)
        {
            Vector2 correction = hit.normal * (intersectPenetration - mPlayerData.PlayerCapsuleOffset);
            mRigidbody.transform.position += (Vector3)correction;
        }

        if (IsHorizontalIntersection(hit))
        {
            bWasLastHorizontalDirectionPositive = mFrameInput.mInputVector.x > 0;
            mFrameInput.mInputVector.x = 0;
            bHasHitWall = true;
        }
        else
        {
            // if we haven't hit a wall and we are going in the opposite direction as to the last horizontal direction, we set bhashitwall false
            bool bIsCurrentInputFramePositiveX = mFrameInput.mInputVector.x > 0;
            if (bIsCurrentInputFramePositiveX != bWasLastHorizontalDirectionPositive)
            {
                bHasHitWall = false;
            }
        }
    }
    private void HandleJump()
    {
        if (!mPlayerData.EndedJumpEarly &&
            !mPlayerData.IsGrounded &&
            !mFrameInput.bIsJumpHeldDown &&
            mRigidbody.linearVelocity.y > 0)
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

    private void HandleZDirection()
    {
        if (mFrameInput.mInputVector.z == 0)
        {
            if (bHasHitWall)
            {
                mFrameVelocityVector.z = 0;
            }
            else
            {
                float deceleration = mPlayerData.IsGrounded ? mPlayerData.GroundDeceleration : mPlayerData.AirDeceleration;
                mFrameVelocityVector.z = Mathf.MoveTowards(mFrameVelocityVector.z, 0, deceleration * Time.fixedDeltaTime);
            }
        }
        else
        {
            float acceleration = mPlayerData.IsGrounded ? mPlayerData.GroundAcceleration : mPlayerData.AirAcceleration;
            mFrameVelocityVector.z = Mathf.MoveTowards(mFrameVelocityVector.z, mFrameInput.mInputVector.z * mPlayerData.MovementSpeed, acceleration * Time.fixedDeltaTime);
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


