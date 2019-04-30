using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

//TODO This will incorporate both Spawning passengers for Pickup & Being Designated Drop-off
[RequireComponent(typeof(Collider))]
public class PassengerArea : MonoBehaviour
{
    public enum AreaType
    {
        NONE,
        PICKUP = 0,
        DROPOFF
    }
    
    //=====================================================================================================//
    
    [SerializeField, ReadOnly]
    protected bool _inArea;
    
    [SerializeField]
    protected string LookingForTag;

    public AreaType CurrentAreaType;
    
    
    protected static Transform _helicopterTransform;

    [SerializeField]
    protected GameObject[] PassengerPrefabs;
    
    //=====================================================================================================//

    protected new BoxCollider collider;
    
    //=====================================================================================================//
    
    // Start is called before the first frame update
    protected void Start()
    {
        if(_helicopterTransform == null)
            _helicopterTransform = GameController.Instance.Helicopter.HelicopterController.transform;

        collider = GetComponent<BoxCollider>();
        if(collider.isTrigger == false)
            throw new ArgumentException("Need to set the Collider trigger to true");
        
        SetCurrentAreaState(CurrentAreaType);
    }

    // Update is called once per frame
    protected void LateUpdate()
    {
        if (_inArea == false)
            return;

        switch (CurrentAreaType)
        {
            case AreaType.PICKUP:
                PickupAreaState();
                break;
            case AreaType.DROPOFF:
                DropoffAreaState();
                break;
        }



    }

    protected void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(LookingForTag))
            return;

        _inArea = true;
    }
    
    protected void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(LookingForTag))
            return;

        _inArea = false;
    }
    
    //=====================================================================================================//

    protected void SetCurrentAreaState(AreaType type)
    {
        CurrentAreaType = type;
        
        switch (CurrentAreaType)
        {
            case AreaType.PICKUP:

                var instance = Instantiate(PassengerPrefabs[Random.Range(0, PassengerPrefabs.Length)]).GetComponent<Passenger>();
                instance.waitingToBoard = true;
                instance.transform.position = transform.position;
                
                break;
            case AreaType.DROPOFF:
                break;
        }
    }
    
    protected  virtual void PickupAreaState()
    {}

    protected virtual void DropoffAreaState()
    {
        if (!GameController.Instance.Helicopter.HelicopterController.IsLanded)
            return;

        if (GameController.Instance.Helicopter.OccupiedSeats == 0)
            return;

        GameController.Instance.Helicopter.TestPopTopPassenger();
    }

}
