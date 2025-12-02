using UnityEngine;

public class HidingSpot : MonoBehaviour
{

	// ตัวแปรเก็บตัวควบคุมเอฟเฟกต์
	private HidingScreenEffect screenEffect;

	void Start()
	{
		// ค้นหาตัวควบคุมเอฟเฟกต์ในฉากเตรียมไว้
		screenEffect = FindObjectOfType<HidingScreenEffect>();
		if (screenEffect == null) Debug.LogWarning("หา HidingScreenEffect ไม่เจอในฉาก!");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerInteract player = other.GetComponent<PlayerInteract>();
			if (player != null)
			{
				player.isHidden = true;
				Debug.Log("Player is hiding!");

				// --- สั่งให้จอเบลอ ---
				if (screenEffect != null) screenEffect.SetBlurred(true);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerInteract player = other.GetComponent<PlayerInteract>();
			if (player != null)
			{
				player.isHidden = false;
				Debug.Log("Player is visible!");

				// --- สั่งให้จอหายเบลอ ---
				if (screenEffect != null) screenEffect.SetBlurred(false);
			}
		}
	}
}