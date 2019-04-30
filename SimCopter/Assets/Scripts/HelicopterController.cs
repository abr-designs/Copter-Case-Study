using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class HelicopterController : MonoBehaviour
{

    public bool IsLanded
    {
        get { return (IsGrounded && !_canTakeOff); }
    }
    
    [SerializeField] protected HelicopterControls test;

    //General
    //============================================================//

    [SerializeField, FoldoutGroup("General")]
    private float takeOffTime;

    [SerializeField, FoldoutGroup("General"), Range(0.001f, 0.1f)]
    private float decelerationPrecision;

    [SerializeField, FoldoutGroup("General"), Range(0.1f, 1f)]
    private float naturalDecelerationMultiplier;
    
    [SerializeField, FoldoutGroup("General"), Range(0f, 1f)]
    private float crashReverseMultiplier;
    
    [FormerlySerializedAs("crashSpeedValue")] [SerializeField, FoldoutGroup("General"), Range(0f, 3f)]
    private float crashSpeedThreshold;

    //Rotation
    //============================================================//
    
    [SerializeField, FoldoutGroup("Rotation")]
    private Transform bodyTransform;

    
    [SerializeField, FoldoutGroup("Rotation")]
    private Vector3 translateRotation;
    
    [SerializeField, FoldoutGroup("Rotation")]
    private Vector3 turnRotation;
    
    //Movement
    //============================================================//

    [FormerlySerializedAs("riseSpeed")] [SerializeField, FoldoutGroup("Movement")]
    private float climbSpeed;

    [SerializeField, FoldoutGroup("Movement")]
    private float riseAcceleration;

    [SerializeField, FoldoutGroup("Movement")]
    private float translateSpeed;

    [SerializeField, FoldoutGroup("Movement")]
    private float translateAcceleration;

    [SerializeField, FoldoutGroup("Movement")]
    private float rotationSpeed;

    [SerializeField, FoldoutGroup("Movement")]
    private float rotationAcceleration;


    //Rotor
    //============================================================//

    [SerializeField, FoldoutGroup("Rotor"), Required]
    private Transform rotorTransform;

    [SerializeField, FoldoutGroup("Rotor")]
    private AnimationCurve rotorTakeoffSpinRate;

    [SerializeField, FoldoutGroup("Rotor")]
    private float rotorRotationSpeed;


    //Camera
    //============================================================//

    [SerializeField, FoldoutGroup("Camera"), Required]
    private Transform cameraTransform;

    [SerializeField, FoldoutGroup("Camera")]
    private Vector3 cameraOffsetPosition;

    //Ground Check
    //============================================================//

    [SerializeField, ReadOnly, FoldoutGroup("Ground Check")]
    public bool IsGrounded { get; protected set; }


    [SerializeField, FoldoutGroup("Ground Check")]
    private LayerMask groundLayerMask;

    [SerializeField, FoldoutGroup("Ground Check")]
    private float checkDistance = 1f;

    [SerializeField, FoldoutGroup("Ground Check")]
    private Vector3 originOffset;
    //============================================================//
    [SerializeField, ReadOnly,BoxGroup("Debug"),]
    private Vector3 _accelerations = Vector3.zero;
    [SerializeField, ReadOnly,BoxGroup("Debug"),]
    private float speed;

    private float _takeOffTimer = 0f;
    private bool  _canTakeOff;

    //[SerializeField, ReadOnly]
    //private float takeOffAccel = 0f;
    //
    //[SerializeField, ReadOnly]
    //private float translateAccel = 0f;
    //[SerializeField, ReadOnly]
    //private float rotationAccel = 0f;
    //
    //private float takeOffTimer = 0f;
    //
    //private bool canTakeOff = false;



    //============================================================//

    [SerializeField, BoxGroup("Debug", Order = -100), ReadOnly]
    protected float mMove;

    [SerializeField, BoxGroup("Debug", Order = -100), ReadOnly]
    protected float mTurn;

    [SerializeField, BoxGroup("Debug", Order = -100), ReadOnly]
    protected float mClimb;

    //============================================================//

    private Vector3 _CurrentPosition;
    private Quaternion _CurrentRotation;

    public new Transform transform
    {
        get
        {
            if (_transform == null)
                _transform = GetComponent<Transform>();

            return _transform;
        }
    }

    private Transform _transform;

    private new Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _CurrentPosition = rigidbody.position;
        _CurrentRotation = rigidbody.rotation;

        InitControls();

    }

    protected virtual void InitControls()
    {
        test.Helicopter.Move.Enable();
        test.Helicopter.Move.performed += ctx => { mMove = ctx.ReadValue<float>(); };

        test.Helicopter.Turn.Enable();
        test.Helicopter.Turn.performed += ctx => { mTurn = ctx.ReadValue<float>(); };

        test.Helicopter.Climb.Enable();
        test.Helicopter.Climb.performed += ctx => { mClimb = ctx.ReadValue<float>(); };

    }

    private void FixedUpdate()
    {
        rigidbody.position = _CurrentPosition;
        rigidbody.rotation = _CurrentRotation;
    }

    // Update is called once per frame
    protected void Update()
    {
        CheckGrounded();

        if (IsGrounded && !_canTakeOff)
        {
            ProccessTakeoff();

            rotorTransform.localEulerAngles += Vector3.up * rotorRotationSpeed *
                                               rotorTakeoffSpinRate.Evaluate(_takeOffTimer / takeOffTime) *
                                               Time.deltaTime;
        }
        else
        {
            if (IsGrounded && speed < crashSpeedThreshold)
            {
                Debug.Log($"Landing Speed {speed}");
                Land();
                return;
            }
            

            _CurrentPosition = rigidbody.position;
            _CurrentRotation = rigidbody.rotation;

            ProcessRotation(ref _CurrentRotation);
            ProcessMovement(ref _CurrentPosition);
            ProcessClimb(ref _CurrentPosition);

            //transform.rotation = _CurrentRotation;
            //transform.position = _CurrentPosition;

            rotorTransform.localEulerAngles += Vector3.up * rotorRotationSpeed * Time.deltaTime;
        }

        speed = _accelerations.sqrMagnitude;
    }

    private void LateUpdate()
    {
        ProcessVisualRotation();
        UpdateCamera();
    }

    protected void Takeoff()
    {
        IsGrounded = false;
        ignoreGroundCheck = true;
        _canTakeOff = true;
        _takeOffTimer = takeOffTime - Time.deltaTime;
        
    }
    protected void Land()
    {
        _canTakeOff = false;
        _accelerations = Vector3.zero;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (speed < crashSpeedThreshold)
            return;
        
        _accelerations *= -crashReverseMultiplier;
        
        
        Debug.Log($"Collision with {other.gameObject.name}. Speed {speed}");

    }

    protected void ProcessMovement(ref Vector3 currentPosition)
    {
        //Vector3 transMove, ClimbMove;

        if (mMove == 0f)
        {
            if (_accelerations.x > decelerationPrecision)
                _accelerations.x += translateAcceleration * -naturalDecelerationMultiplier * Time.deltaTime;
            else if (_accelerations.x < -decelerationPrecision)
                _accelerations.x += translateAcceleration * naturalDecelerationMultiplier * Time.deltaTime;
            else
            {
                _accelerations.x = 0f;
            }
        }
        else
        {
            _accelerations.x += translateAcceleration * mMove * Time.deltaTime;
        }

        _accelerations.x = Mathf.Clamp(_accelerations.x, -1f, 1f);

        currentPosition += (transform.forward * translateSpeed * _accelerations.x) * Time.deltaTime;

    }

    protected void ProcessClimb(ref Vector3 currentPosition)
    {
        //TODO I need to add some sort of natural sink to the movement of the helicopter
        if (mClimb == 0f)
        {
            if (_accelerations.y > decelerationPrecision)
                _accelerations.y += riseAcceleration * -naturalDecelerationMultiplier * Time.deltaTime;
            else if (_accelerations.y < -decelerationPrecision)
                _accelerations.y += riseAcceleration * naturalDecelerationMultiplier * Time.deltaTime;
            else
                _accelerations.y = 0f;
        }
        else
        {
            _accelerations.y += riseAcceleration * mClimb * Time.deltaTime;
        }

        _accelerations.y = Mathf.Clamp(_accelerations.y, -1f, 1f);

        currentPosition += (transform.up * climbSpeed * _accelerations.y) * Time.deltaTime;
    }

    protected void ProcessRotation(ref Quaternion currentRotation)
    {
        if (mTurn == 0f)
        {
            if (_accelerations.z > decelerationPrecision)
                _accelerations.z += rotationAcceleration * -naturalDecelerationMultiplier * Time.deltaTime;
            else if (_accelerations.z < -decelerationPrecision)
                _accelerations.z += rotationAcceleration * naturalDecelerationMultiplier * Time.deltaTime;
            else
            {
                _accelerations.z = 0f;
            }
        }
        else
        {
            _accelerations.z += rotationAcceleration * mTurn * Time.deltaTime;
        }

        _accelerations.z = Mathf.Clamp(_accelerations.z, -1f, 1f);

        var rotation = Vector3.up * rotationSpeed * _accelerations.z * Time.deltaTime;

        currentRotation = Quaternion.Euler(rotation) * currentRotation;
    }

    protected void ProccessTakeoff()
    {
        if (mClimb == 0f)
        {
            if (_takeOffTimer > 0f)
                _takeOffTimer -= Time.deltaTime;
            else
            {
                _takeOffTimer = 0f;
            }
        }
        else if (mClimb >= 1f)
        {
            _takeOffTimer += Time.deltaTime;
        }

        if(_takeOffTimer >= takeOffTime)
            Takeoff();
        
        //_canTakeOff = (_takeOffTimer >= takeOffTime);
        //_accelerations.z = Mathf.Clamp(_accelerations.z, -1f, 1f);

    }

    protected void ProcessVisualRotation()
    {
        var final = (translateRotation * _accelerations.x) + (turnRotation * _accelerations.z * -_accelerations.x);

        bodyTransform.localEulerAngles = final;
    }
    
    protected virtual void UpdateCamera()
    {
        //cameraTransform.position = _CurrentPosition + (cameraOffsetPosition);

        //cameraTransform.LookAt(_CurrentPosition);
    }

    private bool ignoreGroundCheck = false;
    private float ignoreTimer = 0f;
    protected void CheckGrounded()
    {
        if (ignoreGroundCheck && ignoreTimer < 0.1f)
        {
            ignoreTimer += Time.deltaTime;
            return;
        }
        else
        {
            ignoreTimer = 0f;
            ignoreGroundCheck = false;
        }
        //Check speed when grounded checked
        //TODO Need to account for crashing here

        IsGrounded = Physics.Raycast(transform.position + originOffset, -transform.up, checkDistance,
            groundLayerMask.value);
    }

    #region OnDrawGizmosSelected

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {

        Gizmos.DrawWireSphere(transform.position + cameraOffsetPosition, 0.2f);

        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position + originOffset,
            transform.position + (-transform.up * checkDistance) + originOffset);

    }

#endif

    #endregion
}
