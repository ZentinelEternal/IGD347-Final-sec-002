using UnityEngine;

public class DoorPuzzle : MonoBehaviour
{
	[Header("UI & Item")]
	public GameObject notePanel;    
	public GameObject keyObject;    

	private bool isNoteOpen = false;

	
	public void InteractWithDoor()
	{
		if (!isNoteOpen)
		{
			OpenNote();
		}
	}

	void OpenNote()
	{
		isNoteOpen = true;

		
		if (notePanel != null) notePanel.SetActive(true);

		
		Time.timeScale = 0f;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	
	public void CloseNote()
	{
		isNoteOpen = false;

		
		if (notePanel != null) notePanel.SetActive(false);

		
		if (keyObject != null) keyObject.SetActive(true);

		
		Time.timeScale = 1f;
		Cursor.lockState = CursorLockMode.Locked; 
		Cursor.visible = false;

		
		Destroy(this); 
	}
}