using UnityEngine;

public class AudioBullet : MonoBehaviour
{
    [SerializeField]
    private float MAX_TRAVEL_DIST = 30.0f;
    private float mTravelDistance = 0.0f;
    private Vector3 mStartPosition = Vector3.zero;
    [SerializeField]
    private Rigidbody mRigidBody;
    [SerializeField]
    private float mBulletSpeed = 20.0f;
    private Vector3 moveDirection = Vector3.right;
    public Vector3 MoveDirection { set { moveDirection = value; } }
    private void OnEnable()
    {
        mRigidBody = gameObject.GetComponent<Rigidbody>();
        mStartPosition = mRigidBody.position;
    }
    private void FixedUpdate()
    {
        mRigidBody.linearVelocity = mBulletSpeed * moveDirection;
        mTravelDistance = Vector3.Distance(mStartPosition, mRigidBody.position);
        if(mTravelDistance >= MAX_TRAVEL_DIST)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ObjectInteractable"))
        {
            // Break the thing or whatever
        }
        else if (collision.gameObject.CompareTag("Ground")|| collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
