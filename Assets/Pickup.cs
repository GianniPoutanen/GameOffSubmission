using UnityEngine;

public class Pickup : MonoBehaviour
{
    public float reach = 1f;
    public GameObject player;

    Inventory inventory;
    bool canPickup;
    Outline outline;

    public enum QuickEvent
    {
        none,
        increaseStamana,
        increaseRunSpeed,
    }

    [Header("Calls an action after pickup")]
    public QuickEvent quickEvent;

    void Start()
    {
        outline = this.GetComponent<Outline>();
        GameAssets.Instance.AddToPool("Items", this.gameObject);
    }

    void Update()
    {
        if (player == null)
        {
            player = GameAssets.Instance.playerCharacter.gameObject;
            inventory = player.GetComponent<Inventory>();
        }

        if (Vector3.Distance(gameObject.transform.position, player.transform.position) <= reach)
        {
            canPickup = true;
            outline.enabled = true;
        }
        else
        {
            canPickup = false;
            outline.enabled = false;
        }

        if (Input.GetKey(KeyCode.E))
        {
            if (canPickup)
            {
                player.GetComponent<Inventory>().AddItem(gameObject);
                gameObject.SetActive(false);
                HandleQuickEvent();
            }
        }
    }
    void HandleQuickEvent()
    {
        if (quickEvent == QuickEvent.increaseStamana)
        {
            player.GetComponent<PlayerMovementBehaviour>().maxJumps++;
        }
    }
}
