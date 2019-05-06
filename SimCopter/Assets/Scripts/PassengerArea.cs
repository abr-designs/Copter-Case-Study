using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

//TODO This will incorporate both Spawning passengers for Pickup & Being Designated Drop-off
[RequireComponent(typeof(Collider))]
public class PassengerArea : MonoBehaviour
{
    public enum AreaState
    {
        NONE = -1,
        PICKUP = 0,
        DROPOFF
    }
    
    //=====================================================================================================//
    
    [SerializeField, ReadOnly]
    protected bool _helicopterInArea;
    
    [SerializeField]
    protected string LookingForTag;

    public AreaState currentAreaState;
    
    
    protected static Transform _helicopterTransform;

    [SerializeField]
    protected GameObject[] PassengerPrefabs;

    [SerializeField]
    protected GameObject PickupPrefab;
    [SerializeField]
    protected GameObject DropoffPrefab;

    protected GameObject passengerGameObject;
    protected GameObject smokeGameObject;
    
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
        
        SetCurrentAreaState(currentAreaState);
    }

    // Update is called once per frame
    protected void LateUpdate()
    {
        if (_helicopterInArea == false)
            return;

        switch (currentAreaState)
        {
            case AreaState.PICKUP:
                PickupAreaState();
                break;
            case AreaState.DROPOFF:
                DropoffAreaState();
                break;
        }



    }

    protected void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(LookingForTag))
            return;

        _helicopterInArea = true;
    }
    
    protected void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(LookingForTag))
            return;

        _helicopterInArea = false;
    }
    
    //=====================================================================================================//
    
    
    
    //=====================================================================================================//


    public void SetCurrentAreaState(AreaState state)
    {
        currentAreaState = state;
        
        switch (currentAreaState)
        {
            case AreaState.NONE:
                Destroy(smokeGameObject);
                smokeGameObject = null;
                break;
            case AreaState.PICKUP:

                smokeGameObject = Instantiate(PickupPrefab, transform.position, Quaternion.identity).gameObject;
                smokeGameObject.transform.localScale = Vector3.one * 10f;
                var instance = Instantiate(PassengerPrefabs[Random.Range(0, PassengerPrefabs.Length)]).GetComponent<Passenger>();
                instance.waitingToBoard = true;
                instance.transform.position = transform.position;
                passengerGameObject = instance.gameObject;
                
                break;
            case AreaState.DROPOFF:
                smokeGameObject = Instantiate(DropoffPrefab, transform.position, Quaternion.identity).gameObject;
                break;
        }
    }

    protected virtual void PickupAreaState()
    {
        if(passengerGameObject.activeInHierarchy)
            return;

        passengerGameObject = null;
        SetCurrentAreaState(AreaState.NONE);
        GameController.Instance.DesignateDropoffZone(pickupLocation: this);
    }

    protected virtual void DropoffAreaState()
    {
        if (!GameController.Instance.Helicopter.HelicopterController.IsLanded)
            return;

        if (GameController.Instance.Helicopter.OccupiedSeats == 0)
            return;

        GameController.Instance.Helicopter.TestPopTopPassenger();
        SetCurrentAreaState(AreaState.NONE);
    }

    //=====================================================================================================//

}
