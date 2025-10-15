using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    
    public GameObject pointA;
    public GameObject pointB;
    private Rigidbody2D rb;

    private Transform currentPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentPoint = pointB.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
