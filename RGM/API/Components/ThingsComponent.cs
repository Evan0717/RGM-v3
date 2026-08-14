using UnityEngine;

namespace RGM.API.Components;

public class ThingsComponent : MonoBehaviour
{
    private Rigidbody _rigidBody;
    
    private float _mass = 1f;
    private bool _useGravity = true;
    private bool _freezeRotation = true;
    private bool _canMove = true;
    private float _linearDamping = .2f;
    private float _boostAmount = .8f;
    private CollisionDetectionMode _mode = CollisionDetectionMode.ContinuousDynamic;

    public void Start()
    {
        _rigidBody = gameObject.GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
        
        Setter();
    }

    public void FixedUpdate()
    {
        if (!_canMove) return;
        _rigidBody.AddForce(_rigidBody.linearVelocity * (_boostAmount * Time.fixedDeltaTime), ForceMode.Impulse);
    }
    
    public void Initialize(float mass = 1f,
        bool useGravity = true,
        bool freezeRotation = true,
        float linearDamping = .2f,
        float boostAmount = .8f,
        bool canMove = true,
        CollisionDetectionMode mode = CollisionDetectionMode.ContinuousDynamic)
    {
        _mode = mode;
        _mass = mass;
        _useGravity = useGravity;
        _freezeRotation = freezeRotation;
        _linearDamping = linearDamping;
        _boostAmount = boostAmount;
        _canMove = canMove;

        Setter();
    }

    private void Setter()
    {
        _rigidBody.mass = _mass;
        _rigidBody.collisionDetectionMode = _mode;
        _rigidBody.useGravity = _useGravity;
        _rigidBody.freezeRotation = _freezeRotation;
        _rigidBody.linearDamping = _linearDamping;
        _rigidBody.collisionDetectionMode = _mode;
    }
}
