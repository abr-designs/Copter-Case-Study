using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    protected PassengerArea[] _passengerAreas;

    public Helicopter Helicopter
    {
        get
        {
            if (_helicopter == null)
                _helicopter = FindObjectOfType<Helicopter>();

            return _helicopter;
        }
        set => _helicopter = value;
    }
    private Helicopter _helicopter;

    void Awake()
    {
        Instance = this;
        _passengerAreas = FindObjectsOfType<PassengerArea>();
    }

    public void DesignateDropoffZone(PassengerArea pickupLocation)
    {
      while (true)
      {
          var area = _passengerAreas[Random.Range(0, _passengerAreas.Length)];

          if (area == pickupLocation)
              continue;

          area.SetCurrentAreaState(PassengerArea.AreaState.DROPOFF);
          break;

      }
    }


}
