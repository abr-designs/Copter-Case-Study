using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    public string Name;
    public bool waitingToBoard = true;
    [SerializeField] private float pickupRadius    = 10;
    [SerializeField] private float pickupThreshold = 1;

    [SerializeField] private float moveSpeed = 5;

    private Transform _helicopterTransform;

    private new Transform transform;

    // Start is called before the first frame update
    void Start()
    {
        transform = gameObject.transform;
        _helicopterTransform = GameController.Instance.Helicopter.HelicopterController.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //TODO Want to walk away from helicopter if this is the case
        if (!waitingToBoard)
            return;
        
        if (_helicopterTransform == null)
            return;
        
        var helicopterPosition = _helicopterTransform.position;
        var Distance           = Vector3.Distance(transform.position, helicopterPosition);
        if (Distance > pickupRadius)
            return;

        if (!GameController.Instance.Helicopter.HelicopterController.IsLanded)
            return;

        if (Distance > pickupThreshold)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                helicopterPosition, moveSpeed * Time.deltaTime);
            transform.LookAt(helicopterPosition);

        }
        else
            GameController.Instance.Helicopter.Embark(this);
    }

    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }

    private void OnDrawGizmosSelected()
    {
        if (!transform)
            transform = gameObject.transform;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);

    }
}
