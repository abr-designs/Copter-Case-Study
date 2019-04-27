using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class HelicopterController : MonoBehaviour
{
    [SerializeField] protected HelicopterControls test;

    //General
    //============================================================//

    [SerializeField, FoldoutGroup("General")]
    private float takeOffTime;

    [SerializeField, FoldoutGroup("General"), Range(0f, 0.1f)]
    private float decelerationPrecision;

    [SerializeField, FoldoutGroup("General"), Range(0.1f, 1f)]
    private float naturalDecelerationMultiplier;

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
    protected bool isGrounded = false;

    [SerializeField, FoldoutGroup("Ground Check")]
    private LayerMask groundLayerMask;

    [SerializeField, FoldoutGroup("Ground Check")]
    private float checkDistance = 1f;

    [SerializeField, FoldoutGroup("Ground Check")]
    private Vector3 originOffset;
    //============================================================//
[SerializeField, ReadOnly,]
    private Vector3 _accelerations = Vector3.zero;

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

    private new Transform transform;

    // Start is called before the first frame update
    void Start()
    {
        transform = gameObject.transform;

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

    // Update is called once per frame
    protected void Update()
    {
        CheckGrounded();

        if (isGrounded && !_canTakeOff)
        {
            ProccessTakeoff();

            rotorTransform.localEulerAngles += Vector3.up * rotorRotationSpeed *
                                               rotorTakeoffSpinRate.Evaluate(_takeOffTimer / takeOffTime) *
                                               Time.deltaTime;
        }
        else
        {
            
            Vector3    position = transform.position;
            Quaternion rotation = transform.rotation;

            ProcessRotation(ref rotation);
            ProcessMovement(ref position);
            ProcessClimb(ref position);

            transform.rotation = rotation;
            transform.position = position;
            
            rotorTransform.localEulerAngles += Vector3.up * rotorRotationSpeed * Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        ProcessVisualRotation();
        UpdateCamera();
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

        _canTakeOff = (_takeOffTimer >= takeOffTime);
        //_accelerations.z = Mathf.Clamp(_accelerations.z, -1f, 1f);

    }

    protected void ProcessVisualRotation()
    {
        var final = (translateRotation * _accelerations.x) + (turnRotation * _accelerations.z * -_accelerations.x);

        bodyTransform.localEulerAngles = final;
    }
    
    protected virtual void UpdateCamera()
    {
        cameraTransform.position = transform.position + cameraOffsetPosition;

        cameraTransform.forward = transform.position - cameraTransform.position;
    }

    protected void CheckGrounded()
    {
        //Check speed when grounded checked
        //TODO Need to account for crashing here

        isGrounded = Physics.Raycast(transform.position + originOffset, -transform.up, checkDistance,
            groundLayerMask.value);
    }

    #region OnDrawGizmosSelected

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        if (!transform)
            transform = gameObject.transform;

        Gizmos.DrawWireSphere(transform.position + cameraOffsetPosition, 0.2f);

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position + originOffset,
            transform.position + (-transform.up * checkDistance) + originOffset);

    }

#endif

    #endregion
}
