using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] Animator animator;
    GameManagement gm;

    private void Start()
    {
        gm = FindFirstObjectByType<GameManagement>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
           gm.ChangeCoin(10);
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.CompareTag("Fance"))
        {
            animator.SetTrigger("hit");
        }
        else if(collision.gameObject.CompareTag("Obstacle"))
        {
            gm.ChangeLife(-1);
            Destroy(collision.gameObject);
            //animator.SetTrigger("hit");
        }
    }
}
