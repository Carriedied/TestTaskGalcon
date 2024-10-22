using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fleet : MonoBehaviour
{
    private Planet originPlanet;
    private Planet targetPlanet;
    private int shipsCount;
    private float speed = 5f;

    public void Initialize(Planet origin, Planet target, int ships)
    {
        originPlanet = origin;
        targetPlanet = target;
        shipsCount = ships;

        transform.position = originPlanet.transform.position;
    }

    private void Update()
    {
        if (targetPlanet == null) return;

        Vector3 direction = (targetPlanet.transform.position - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, Vector3.Distance(transform.position, targetPlanet.transform.position));
        if (hit.collider != null && hit.collider.GetComponent<Planet>() != targetPlanet)
        {
            Vector3 avoidanceDirection = Vector3.Cross(direction, Vector3.forward).normalized;
            direction += avoidanceDirection * 0.5f;
            direction.Normalize();
        }

        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPlanet.transform.position) < targetPlanet.Radius)
        {
            targetPlanet.ReceiveShips(shipsCount);
            Destroy(gameObject);
        }
    }
}
