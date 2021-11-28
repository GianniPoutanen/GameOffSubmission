using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject grassHopper;

    public void AddItem(GameObject item)
    {
        switch (item.name)
        {
            case "SpecialFlower":
                DialogueSpeaker dialog =  grassHopper.GetComponent<DialogueSpeaker>();
                dialog.StartNode = "Grasshopper.QuestComplete";
                return;
            default:
                return;
        }
    }
}
