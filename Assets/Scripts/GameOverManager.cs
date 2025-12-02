using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;

public class GameOverManager : MonoBehaviour
{
	[Header("UI References")]
	public GameObject jumpscarePanel;   

	[Header("Scene Settings")]
	public string deathSceneName = "DeathScene"; 

	[Header("Audio")]
	public AudioSource audioSource;
	public AudioClip screamSound;

	[Header("Settings")]
	public float scareTime = 3f; 

	// ตัวแปรป้องกันการเรียกซ้ำ
	private bool hasTriggered = false;

	void Start()
	{
		
		if (audioSource != null)
		{
			audioSource.ignoreListenerPause = true;
		}
	}

	
	public void CatchPlayer()
	{
		
		if (hasTriggered) return;
		hasTriggered = true;

		StartCoroutine(JumpscareRoutine());
	}

	IEnumerator JumpscareRoutine()
	{
		Debug.Log("Jumpscare Started!");

		
		if (audioSource != null && screamSound != null)
		{
			audioSource.Stop(); 
			audioSource.clip = screamSound; 
			audioSource.Play(); 
		}

		
		if (jumpscarePanel != null)
		{
			jumpscarePanel.SetActive(true);
		}

		 
		Time.timeScale = 0f;

		
		yield return new WaitForSecondsRealtime(scareTime);

		Debug.Log("Loading Death Scene...");

		
		Time.timeScale = 1f;

		
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		
		SceneManager.LoadScene(deathSceneName);
	}
}
