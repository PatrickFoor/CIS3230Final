using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pause : MonoBehaviour {

    public static Pause Instance;
	
	public Texture2D BatteryIcon;
	public Texture2D BlueprintIcon;
	public Texture2D CageIcon;
	public Texture2D ComicPageIcon;
	public Texture2D KeyIcon;
	public Texture2D PieCrustIcon;
	public Texture2D PieFillingIcon;
	public Texture2D PieTinIcon;

    static int menuChoice = -1;

    private bool showPause = false;
    private List<Animation> animationComponents;
    private List<AudioSource> audioSourceComponents;
    private float menuOn = 0;
    private float lastTime = 0;
    private TP_Camera cameraScript;
	
	private Rect subMenu = new Rect((float) (Screen.width * .125), (float) (Screen.height * .125), 
									(float) (Screen.width * .75), (float) (Screen.height * .75));

    public TP_Camera CameraScript
    {
        get
        {
            return cameraScript;
        }
        set
        {
            cameraScript = value;
        }
    }

	// Use this for initialization
	void Awake() 
    {
        Instance = this;
        Screen.showCursor = false;
	}

    IEnumerator Start()
    {
        AudioListener.volume = 0;
        yield return 0;
        AudioListener.volume = 1;
    }

    void Update()
    {
        // Don't pause in first frame - allow scripts to settle in first
        if (Time.timeSinceLevelLoad == 0)
            return;

        float realDeltaTime = (Time.realtimeSinceStartup - lastTime);
        lastTime = Time.realtimeSinceStartup;
        menuOn = Mathf.Clamp01(menuOn + (Time.timeScale == 0 ? 1 : -1) * realDeltaTime * 5);

        if (Input.GetButton("Pause"))
        {
            showPause = true;
            StartCoroutine(ToggleMenu(showPause));
        }
    }
	
	// OnGUI is called once per frame
	void OnGUI() 
    {
        if (menuOn == 0) 
		{
            return;
		}
		
		Rect rect; 
		
		if (menuChoice == -1)
		{
	        // PLAY button
	        rect = new Rect(0, 0, 150, 75);
	        rect.x = (Screen.width - rect.width) / 2 + (1 - menuOn) * Screen.width;
	        rect.y = (Screen.height - rect.height) / 2;
	        if (GUI.Button(rect, "PLAY"))
	        {
	            showPause = false;
	            StartCoroutine(ToggleMenu(showPause));
	        }
	
	        // option buttons
	        rect = new Rect(rect.x - 200, rect.y + 150, rect.width + 400, 40);
	        string[] menuOptions = new string[] { "Controls", "Inventory", "Game Options", 
													"Audio Options", "Video Options" };
	        menuChoice = GUI.SelectionGrid(rect, menuChoice, menuOptions, menuOptions.Length);
		} 
		else 
		{
			string menuTitle = "";
			
			if (menuChoice == 0) {
				menuTitle = "Controls";
			}
			else if (menuChoice == 1) {
				menuTitle = "Inventory";	
			}
			else if (menuChoice == 2) {
				menuTitle = "Game Options";	
			}
			else if (menuChoice == 3) {
				menuTitle = "Audio Options";	
			}
			else if (menuChoice == 4) {
				menuTitle = "Video Options";	
			}
			
			subMenu = GUI.Window (0, subMenu, SubMenu, menuTitle);	
		}
		
	}
	
	void SubMenu(int windowID) 
	{
		Rect rect; 
		
		// Back button
        rect = new Rect(0, 0, 150, 75);
		rect.x = subMenu.width / 2 - rect.width / 2;
		rect.y = subMenu.height / 2 + rect.height / 2;
        if (GUI.Button(rect, "BACK"))
        {
            menuChoice = -1;
        }
		
		if (menuChoice == 0) 
		{
			
		} 
		else if (menuChoice == 1) 
		{
			//Inventory Information
			GUI.Label (new Rect (subMenu.width/7 - BatteryIcon.width / 2, subMenu.height * .125f,150,75), 
				new GUIContent(InventorySystem.Instance.BatteryCount.ToString() + "/" + Collectible.BatteryTotal.ToString(), 
				BatteryIcon));
			GUI.Label (new Rect ((subMenu.width/7 + (subMenu.width * 3/14)) - BlueprintIcon.width / 2, subMenu.height * .3f,150,75), 
				new GUIContent(InventorySystem.Instance.BlueprintCount.ToString() + "/" + Collectible.BlueprintTotal.ToString(), 
				BlueprintIcon));
			GUI.Label (new Rect ((subMenu.width/7 + (subMenu.width * 6/16)) - ComicPageIcon.width / 2, subMenu.height * .125f,150,75), 
				new GUIContent(InventorySystem.Instance.ComicPageCount.ToString() + "/" + Collectible.ComicPageTotal.ToString(), 
				ComicPageIcon));
			GUI.Label (new Rect ((subMenu.width/7 + (subMenu.width * 7/14)) - PieCrustIcon.width / 2, subMenu.height * .3f,125,75), 
				new GUIContent(InventorySystem.Instance.PieCrustCount.ToString() + "/" + Collectible.PieCrustTotal.ToString(), 
				PieCrustIcon));
			GUI.Label (new Rect ((subMenu.width/7 + (subMenu.width * 9/15)) - PieFillingIcon.width / 2, subMenu.height * .125f,125,75), 
				new GUIContent(InventorySystem.Instance.PieFillingCount.ToString() + "/" + Collectible.PieFillingTotal.ToString(), 
				PieFillingIcon));
			GUI.Label (new Rect ((subMenu.width/7 + (subMenu.width * 85/100)) - PieTinIcon.width / 2, subMenu.height * .3f,125,75), 
				new GUIContent(InventorySystem.Instance.PieTinCount.ToString() + "/" + Collectible.PieTinTotal.ToString(), 
				PieTinIcon));
		}
		else if (menuChoice == 2) 
		{
			
		}
		else if (menuChoice == 3) 
		{
			
		}
	}

    IEnumerator ToggleMenu(bool pause)
    {
        //pause/unpause time
        Time.timeScale = (pause ? 0 : 1);
        //lock/unlock cursor
        Screen.lockCursor = !pause;
        //disable/enable camera script
        cameraScript.enabled = !pause;
		//show/hide cursor
		Screen.showCursor = pause;

        if (Time.timeScale == 0f)
        {
            
        }
        else
        {
            yield return 0;
        }
    }

    public static void UseExistingOrCreatePauseGUI()
    {
        GameObject tempGUI;
        Pause myGUI;

        if (GameObject.Find("PauseGUI") != null)
        {
            tempGUI = GameObject.Find("PauseGUI");
        }
        else
        {
            tempGUI = new GameObject("PauseGUI");
            tempGUI.tag = "PauseGUI";
        }

        tempGUI.AddComponent("Pause");
        
        myGUI = (Pause)tempGUI.GetComponent("Pause");
        myGUI.cameraScript = TP_Camera.Instance;
    }
}
