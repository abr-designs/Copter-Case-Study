using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    [SerializeField] private float pickupRadius    = 10;
    [SerializeField] private float pickupThreshold = 1;

    [SerializeField] private float moveSpeed = 5;

    private new Transform transform;

    // Start is called before the first frame update
    void Start()
    {
        transform = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        var helicopterPosition = GameController.Instance.Helicopter.HelicopterController.transform.position;
        var Distance           = Vector3.Distance(transform.position, helicopterPosition);
        if (Distance > pickupRadius)
            return;

        if (!GameController.Instance.Helicopter.HelicopterController.IsGrounded)
            return;

        if (Distance > pickupThreshold)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                helicopterPosition, moveSpeed * Time.deltaTime);
            transform.LookAt(helicopterPosition);

        }
        else
            GameController.Instance.Helicopter.Embark(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (!transform)
            transform = gameObject.transform;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);

    }
}
