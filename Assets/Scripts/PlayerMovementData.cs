using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementData", menuName = "Scriptable Objects/PlayerMovementData")]
public class PlayerMovementData : ScriptableObject
{
    [Header("Movement Information")]
    [SerializeField, Tooltip("How fast our character moves")]
    private float mMovementSpeed;
    [SerializeField, Tooltip("How intense is our jump")]
    private float mJumpForce;
    [SerializeField, Tooltip("How much is our jump cut by if we short jump")]
    private float mJumpCutMultiplier;
    [SerializeField]
    private float mGravityStrength;
    private bool bIsGrounded = true;
    private bool bIsJumpBuffered = false;
    private bool bJumpToBeConsumed = false;
    [SerializeField, Tooltip("Grace Period to input a jump")]
    private float mBufferedJumpTime;
    private bool bEndedJumpEarly = false;
    private bool bIsBufferedJumpUsable = false;
    private bool bIsCoyoteTimeUsable = false;
    [SerializeField, Tooltip("Grace Period for Coyote Time")]
    private float mCoyoteTimeFrame;
    [SerializeField, Tooltip("Deceleration for when we are on ground")]
    private float mHorizontalGroundDeceleration;
    [SerializeField, Tooltip("Deceleration for when we are in air")]
    private float mHorizontalAirDeceleration;
    [SerializeField, Tooltip("Acceleration for when we are on ground")]
    private float mHorizontalGroundAcceleration;
    [SerializeField, Tooltip("Acceleration for when we are in air")]
    private float mHorizontalAirAcceleration;
    [SerializeField, Tooltip("Acceleration for when we are falling in air")]
    private float mVerticalAcceleration;
    [SerializeField, Tooltip("Acceleration for when we want to slow our falling in air")]
    private float mVerticalDeceleration;
    [SerializeField, Tooltip("The Terminal Velocity that you can go. This is a velocity not an acceleration")]
    private float mTerminalVelocity;

    // Do we want to modify movement attributes at any point?
    public float MovementSpeed
    {
        get { return mMovementSpeed; }
        set { mJumpForce = value; }
    }
    public float JumpForce
    {
        get { return mJumpForce; }
        set { mJumpForce = value; }
    }
    public float JumpCutMultiplier
    {
        get { return mJumpCutMultiplier; }
        set { mJumpCutMultiplier = value; }
    }
    public bool IsGrounded
    {
        get { return bIsGrounded; }
        set { bIsGrounded = value; }
    }
    public bool JumpToBeConsumed
    {
        get { return bJumpToBeConsumed; }
        set { bJumpToBeConsumed = value; }
    }
    public bool IsJumpbuffered
    {
        get { return bIsJumpBuffered; }
        set { bIsJumpBuffered = value; }
    }
    public float GravityStrength
    {
        get { return mGravityStrength; }
        set { mGravityStrength = value; }
    }
    public float BufferedJumpTime
    {
        get { return mBufferedJumpTime; }
    }
    public bool EndedJumpEarly
    {
        get { return bEndedJumpEarly; }
        set { bEndedJumpEarly = value; }
    }
    public bool IsBufferedJumpUsable
    {
        get { return bIsBufferedJumpUsable; }
        set { bIsBufferedJumpUsable = value; }
    }
    public bool IsCoyoteTimeUsable
    {
        get { return bIsCoyoteTimeUsable; }
        set { bIsCoyoteTimeUsable = value; }
    }
    public float CoyoteTimeFrame
    {
        get { return mCoyoteTimeFrame; }
    }
    public float GroundDeceleration
    {
        get { return mHorizontalGroundDeceleration; }
    }
    public float AirDeceleration
    {
        get { return mHorizontalAirDeceleration; }
    }
    public float GroundAcceleration
    {
        get { return mHorizontalGroundAcceleration; }
    }
    public float AirAcceleration
    {
        get { return mHorizontalAirAcceleration; }
    }
    public float VerticalAcceleration
    {
        get { return mVerticalAcceleration; }
    }
    public float VerticalDeceleration
    {
        get { return mVerticalDeceleration; }
    }
    public float TerminalVelocity
    {
        get { return mTerminalVelocity; }
    }
    [Header("Raycast Information")]
    [SerializeField, Tooltip("How far away we are checking for collision below us")]
    private float mRaycastDistance = 1.0f;
    [SerializeField, Tooltip("What layers do we want to check?")]
    private LayerMask mHittableLayers;
    public float RaycastDistance
    {
        get { return mRaycastDistance; }
    }
    public LayerMask HittableLayers
    {
        get { return mHittableLayers; }
    }
}
