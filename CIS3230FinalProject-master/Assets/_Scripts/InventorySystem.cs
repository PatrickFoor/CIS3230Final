using UnityEngine;
using System.Collections;

public class InventorySystem : MonoBehaviour {

    public static InventorySystem Instance;

    public enum CollectibleType
    {
        Battery, Blueprint, ComicPage, PieCrust, PieFilling, PieTin, Citizen
    }

    private int batteryCount = 0;
    private int blueprintCount = 0;
    private int comicPageCount = 0;
    private int pieCrustCount = 0;
    private int pieFillingCount = 0;
    private int pieTinCount = 0;
    private int citizenCount = 0;

    public int BatteryCount
    {
        get
        {
            return batteryCount;
        }
        set
        {
            batteryCount = value;
        }
    }
    public int BlueprintCount
    {
        get
        {
            return blueprintCount;
        }
        set
        {
            blueprintCount = value;
        }
    }
    public int ComicPageCount
    {
        get
        {
            return comicPageCount;
        }
        set
        {
            comicPageCount = value;
        }
    }
    public int PieCrustCount
    {
        get
        {
            return pieCrustCount;
        }
        set
        {
            pieCrustCount = value;
        }
    }
    public int PieFillingCount
    {
        get
        {
            return pieFillingCount;
        }
        set
        {
            pieFillingCount = value;
        }
    }
    public int PieTinCount
    {
        get
        {
            return pieTinCount;
        }
        set
        {
            pieTinCount = value;
        }
    }
    public int CitizenCount
    {
        get
        {
            return citizenCount;
        }
        set
        {
            citizenCount = value;
        }
    }

    void Awake()
    {
        Instance = this;
    }

    public void CollectObject(CollectibleType type)
    {
        switch (type)
        {
            case CollectibleType.Battery:
                batteryCount++;
                break;
            case CollectibleType.Blueprint:
                blueprintCount++;
                break;
            case CollectibleType.ComicPage:
                comicPageCount++;
                break;
            case CollectibleType.PieCrust:
                pieCrustCount++;
                break;
            case CollectibleType.PieFilling:
                pieFillingCount++;
                break;
            case CollectibleType.PieTin:
                pieTinCount++;
                break;
            case CollectibleType.Citizen:
                citizenCount++;
                break;
        }
    }

    public static void UseExistingOrCreateInventory()
    {
        GameObject tempInventory;
        InventorySystem myInventory;

        if (GameObject.Find("Inventory") != null)
        {
            tempInventory = GameObject.Find("Inventory");
        }
        else
        {
            tempInventory = new GameObject("Inventory");
            tempInventory.tag = "Inventory";
        }

        tempInventory.AddComponent("InventorySystem");
        myInventory = (InventorySystem)tempInventory.GetComponent("InventorySystem");
    }
}
