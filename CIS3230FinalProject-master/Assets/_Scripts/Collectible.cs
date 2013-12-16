using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour {

    private InventorySystem.CollectibleType type;
	
	public static Collectible Instance;
	public static int BatteryTotal;
	public static int BlueprintTotal;
	public static int ComicPageTotal;
	public static int PieCrustTotal;
	public static int PieFillingTotal;
	public static int PieTinTotal;
	public static int CitizenTotal;
	
    void Awake()
    {
		Instance = this;
		BatteryTotal = GameObject.FindGameObjectsWithTag("Battery").Length;
		BlueprintTotal = GameObject.FindGameObjectsWithTag("Blueprint").Length;
		ComicPageTotal = GameObject.FindGameObjectsWithTag("ComicPage").Length;
		PieCrustTotal = GameObject.FindGameObjectsWithTag("PieCrust").Length;
		PieFillingTotal = GameObject.FindGameObjectsWithTag("PieFilling").Length;
		PieTinTotal = GameObject.FindGameObjectsWithTag("PieTin").Length;
		CitizenTotal = GameObject.FindGameObjectsWithTag("Citizen").Length;
		
        if (this.gameObject.tag.Equals(InventorySystem.CollectibleType.Battery.ToString()))
        {
            type = InventorySystem.CollectibleType.Battery;
        } else if (this.gameObject.tag.Equals(InventorySystem.CollectibleType.Blueprint.ToString())) {
            type = InventorySystem.CollectibleType.Blueprint;
        } else if (this.gameObject.tag.Equals(InventorySystem.CollectibleType.ComicPage.ToString())) {
            type = InventorySystem.CollectibleType.ComicPage;
        } else if (this.gameObject.tag.Equals(InventorySystem.CollectibleType.PieCrust.ToString())) {
            type = InventorySystem.CollectibleType.PieCrust;
        } else if (this.gameObject.tag.Equals(InventorySystem.CollectibleType.PieFilling.ToString())) {
            type = InventorySystem.CollectibleType.PieFilling;
        } else if (this.gameObject.tag.Equals(InventorySystem.CollectibleType.PieTin.ToString())) {
            type = InventorySystem.CollectibleType.PieTin;
        } else if (this.gameObject.tag.Equals(InventorySystem.CollectibleType.Citizen.ToString())) {
            type = InventorySystem.CollectibleType.Citizen;
        }
    }
	
    void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.tag.Equals("Player"))
        {
            InventorySystem.Instance.CollectObject(type);

            Destroy(gameObject);
        }
    }
}
