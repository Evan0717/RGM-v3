using System;
using UnityEngine;

namespace RGM.API.Components;

public class BoxColliderThingsComponent : MonoBehaviour
{
    public class TriggerEventArgs : System.EventArgs
    {
        public GameObject GameObject { get; set; }
    }

    public event EventHandler<TriggerEventArgs> TriggerEnter;
    public event EventHandler<TriggerEventArgs> TriggerExit;
    
    private float _mass = 1f;
    private float _linearDamping = .2f;
    private float _boostAmount = .8f;
    private CollisionDetectionMode _mode = CollisionDetectionMode.ContinuousDynamic;

    private bool _useGravity = true;
    private bool _freezeRotation = true;
    private bool _canMove = true;
    private bool _enabled = true;
    private bool _isTrigger = true;
    
    private ThingsComponent _objComponent;
    private BoxCollider _boxCollider;
    
    public void Start()
    {
        _objComponent = gameObject.AddComponent<ThingsComponent>();
        _boxCollider = gameObject.AddComponent<BoxCollider>();

        Setter();
    }
    
    public void Initialize(
        float mass = 1f,
        float linearDamping = .2f,
        float boostAmount = .8f,
        CollisionDetectionMode mode = CollisionDetectionMode.ContinuousDynamic,
        bool useGravity = true,
        bool freezeRotation = true,
        bool canMove = false,
        bool isTrigger = true,
        bool collisionEnabled = true)
    {
        _mass = mass;
        _useGravity = useGravity;
        _freezeRotation = freezeRotation;
        _linearDamping = linearDamping;
        _boostAmount = boostAmount;
        _enabled = collisionEnabled;
        _isTrigger = isTrigger;
        _canMove = canMove;
        _mode = mode;

        Setter();
    }
    
    public void OnTriggerEnter(Collider obj)
        => TriggerEnter?.Invoke(this, new TriggerEventArgs { GameObject = obj.gameObject });
    
    public void OnTriggerExit(Collider obj)
         => TriggerExit?.Invoke(this, new TriggerEventArgs {GameObject = obj.gameObject});

    private void Setter()
    {
        _objComponent.Initialize(
            _mass,
            _useGravity,
            _freezeRotation,
            _linearDamping,
            _boostAmount,
            _canMove,
            _mode);
        
        _boxCollider.center = gameObject.transform.position;
        _boxCollider.enabled = _enabled;
        _boxCollider.isTrigger = _isTrigger;
        _boxCollider.size = gameObject.transform.localScale;
    }
}