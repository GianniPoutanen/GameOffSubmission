using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("QuestGivers")]
    public GameObject grassHopper;
    public GameObject grassHopperBrother;
    public int collectibleCount = 0;
    public int totalCollectibles = 4;

    public void AddItem(GameObject item)
    {
        switch (item.name.Split(' ')[0])
        {
            case "SpecialNectar":
                DialogueSpeaker dialog =  grassHopper.GetComponent<DialogueSpeaker>();
                dialog.StartNode = "Grasshopper.QuestComplete";
                DialogueSpeaker brotherDialog =  grassHopperBrother.GetComponent<DialogueSpeaker>();
                brotherDialog.StartNode = "GrasshopperBrother.FoundOrb";
                if (++collectibleCount == totalCollectibles) {
                    brotherDialog.StartNode = "GrasshopperBrother.FoundAllOrbs";
                }
                return;
            default:
                return;
        }
    }
}
