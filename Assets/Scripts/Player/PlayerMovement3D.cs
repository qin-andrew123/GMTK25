using Unity.Burst.CompilerServices;
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
    private bool bHasZHitWall = false;
    private bool bHasXHitWall = false;
    private float mTimeLeftGrounded = float.MinValue;
    private float mTimeOfJump = float.MinValue;
    private Vector3 mFrameVelocityVector;
    private FrameInput mFrameInput;
    private float halfHeight = 0.0f;
    private float height = 0.0f;
    // TODO: Slope Handling
    // TODO: Wall Jumping/sliding? Maybe? 

    [SerializeField]
    private AudioEvent mJumpSFX;

    public Animator mAnimator;
    public SpriteRenderer mSpriteRenderer;

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

        height = Mathf.Max(mCapsuleCollider.height, mCapsuleCollider.radius * 2f);
        halfHeight = height / 2.0f;
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
        // Grav First

        HandleJump();
        HandleGravity();
        HandleXDirection();
        HandleZDirection();
        HandleCollision();

        Vector3 calculatedMove = mFrameVelocityVector * Time.fixedDeltaTime;
        mRigidbody.MovePosition(mRigidbody.position + calculatedMove);

        mAnimator.SetFloat("Speed", mFrameInput.mInputVector.magnitude);

        if (mFrameInput.mInputVector.x < 0)
        {
            mSpriteRenderer.flipX = true;
        }
        else
        {
            mSpriteRenderer.flipX = false;
        }
    }

    private void HandleCollision()
    {
        Vector3 DirectionVector = mFrameVelocityVector;
        DirectionVector.Normalize();

        if (DirectionVector == Vector3.zero)
        {
            DirectionVector = Vector3.down;
        }

        Vector3 pointOne = mCapsuleCollider.bounds.center + mCapsuleCollider.transform.up * halfHeight;
        Vector3 pointTwo = mCapsuleCollider.bounds.center - mCapsuleCollider.transform.up * halfHeight;
        Physics.CapsuleCast(
                    pointOne,
                    pointTwo,
                    mCapsuleCollider.radius,
                    DirectionVector,
                    out RaycastHit hit,
                    mPlayerData.RaycastDistance,
                    mPlayerData.HittableLayers);
        if (hit.collider != null)
        {
            HandleIntersection(hit);

            if (Vector3.Dot(hit.normal, Vector3.up) > 0.5f)
            {
                if (!mPlayerData.IsGrounded)
                {
                    mPlayerData.IsGrounded = true;
                    mPlayerData.IsCoyoteTimeUsable = true;
                    mPlayerData.IsBufferedJumpUsable = true;
                    mPlayerData.EndedJumpEarly = false;

                    mFrameVelocityVector.y = 0;
                }
            }
            else
            {
                mFrameVelocityVector = Vector3.ProjectOnPlane(mFrameVelocityVector, hit.normal);
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
    private void HandleIntersection(RaycastHit hit)
    {
        float intersectPenetration = hit.distance - mPlayerData.PlayerCapsuleOffset;
        if (intersectPenetration < 0)
        {
            intersectPenetration = 0;

        }
        Vector3 correction = hit.normal * (intersectPenetration);
        mRigidbody.transform.position += correction;
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
        mJumpSFX?.Play2DSound();
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
            mFrameVelocityVector.x = Mathf.MoveTowards(mFrameVelocityVector.x, mFrameInput.mInputVector.x * mPlayerData.MovementSpeed, acceleration * Time.fixedDeltaTime);
        }
    }

    private void HandleZDirection()
    {
        if (mFrameInput.mInputVector.z == 0)
        {
            float deceleration = mPlayerData.IsGrounded ? mPlayerData.GroundDeceleration : mPlayerData.AirDeceleration;
            mFrameVelocityVector.z = Mathf.MoveTowards(mFrameVelocityVector.z, 0, deceleration * Time.fixedDeltaTime);
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
        else if (!mPlayerData.IsGrounded)
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


