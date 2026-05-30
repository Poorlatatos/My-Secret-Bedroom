using UnityEngine;

public class LaundryItemCollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LaundryBasket"))
        {
            // Make this object disappear
            gameObject.SetActive(false); // Or Destroy(gameObject);
        }
    }
}