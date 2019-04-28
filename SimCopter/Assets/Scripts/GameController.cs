using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

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


    }

    // Start is called before the first frame update
    void Start()
    {

    }

}
