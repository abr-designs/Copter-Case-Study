using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(HelicopterController))]
public class Helicopter : MonoBehaviour
{
    public HelicopterController HelicopterController
    {
        get
        {
            if (_helicopterController == null)
                _helicopterController = GetComponent<HelicopterController>();
            return _helicopterController;
        }
    }
    private HelicopterController _helicopterController;

    public int AvailableSeats
    {
        get { return Passengers.Count(x => x == null); }
    }
    
    public int OccupiedSeats
    {
        get { return Passengers.Count(x => x != null); }
    }

    [SerializeField, Range(0,10)]
    private int Seats;

    [ReadOnly] public Passenger[] Passengers;

    // Start is called before the first frame update
    void Start()
    {
        Passengers = new Passenger[Seats];
    }

    public void Embark(Passenger passenger)
    {
        if (AvailableSeats <= 0)
        {
            Debug.LogError($"Not enough room to embark {passenger.Name}");
            return;
        }

        for (int i = 0; i < Seats; i++)
        {
            if (Passengers[i] != null)
                continue;

            Passengers[i] = passenger;
            Passengers[i].SetActive(false);
            break;
        }
    }

    public void Disembark(Passenger passenger)
    {
        int index = -1;
        for (int i = 0; i < Seats; i++)
        {
            if (Passengers[i] != passenger)
                continue;

            index = i;
            break;
        }

        if (index < 0)
        {
            Debug.LogError($"No such passenger {passenger.Name} exists");
            return;
        }

        Disembark(index);
    }
    public void Disembark(GameObject passenger)
    {
        int index = -1;
        for (int i = 0; i < Seats; i++)
        {
            if (Passengers[i].gameObject != passenger)
                continue;

            index = i;
            break;
        }

        if (index < 0)
        {
            Debug.LogError($"No such gameObject {passenger.name} exists");
            return;
        }

        Disembark(index);
    }
    public void Disembark(int index)
    {
        if (index < 0 || index >= Passengers.Length)
        {
            throw new IndexOutOfRangeException();
        }

        if (Passengers[index] == null)
        {
            Debug.LogError($"No such passenger [{index}] exists");
            return;
        }

        Passengers[index].transform.position = transform.position + (transform.right * 2f);
        Passengers[index].waitingToBoard = false;
        Passengers[index].SetActive(true);
        Passengers[index] = null;
    }

    public void TestPopTopPassenger()
    {
        for (int i = 0; i < Seats; i++)
        {
            if (Passengers[i] == null)
                continue;

            Debug.Log($"Disembarking {Passengers[i].name}");
            
            Disembark(i);
            return;

        }
    }

}
