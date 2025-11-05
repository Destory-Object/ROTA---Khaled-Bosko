using UnityEngine;

public class shootingScript : MonoBehaviour
{
    [SerializeField] Transform ShootingPoint;
    [SerializeField] GameObject bombaclat;
    [SerializeField] GameObject DeflectiveBombaclat;
    [SerializeField] float Enemyposition;

    
    void Start()
    {
        float randomYValue = Random.Range(ShootingPoint.position.y - 2f, ShootingPoint.position.y + 2f);

        Instantiate(bombaclat, new Vector2(ShootingPoint.position.x, randomYValue), Quaternion.identity);

       

    }


    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Bökjjar skjuta sätt en bool till "canShoot"
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Sätt samma boolm till "false"
    }
}
