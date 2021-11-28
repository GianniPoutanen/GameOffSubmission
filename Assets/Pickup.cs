using UnityEngine;

public class Pickup : MonoBehaviour
{
    public float reach = 1f;
    public GameObject player;

    Inventory inventory;

    void Start() {
        inventory = player.GetComponent<Inventory>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            if (Vector3.Distance(gameObject.transform.position, player.transform.position) <= reach)
            {
                inventory.AddItem(gameObject);
                gameObject.SetActive(false);
            }
        }
    }
}
