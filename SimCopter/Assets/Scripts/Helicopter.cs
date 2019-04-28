using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(HelicopterController))]
public class Helicopter : MonoBehaviour
{
    public HelicopterController HelicopterController { get; private set; }

    public int AvailableSeats
    {
        get { return Passengers.Count(x => x == null); }
    }

    [SerializeField, Range(0,10)]
    private int Seats;

    [ReadOnly] public GameObject[] Passengers;

    // Start is called before the first frame update
    void Start()
    {
        HelicopterController = GetComponent<HelicopterController>();
        Passengers = new GameObject[Seats];
    }

    public void Embark(GameObject passenger)
    {
        if (AvailableSeats <= 0)
        {
            Debug.LogError($"Not enough room to embark {gameObject.name}");
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

    public void Disembark(GameObject passenger)
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
            Debug.LogError($"No such passenger {passenger.name} exists");
            return;
        }
        
        Passengers[index].SetActive(true);
        Passengers[index] = null;
    }

}
