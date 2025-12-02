using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{

	public Dialogue dialogue;
	public Animator npcAnimator;

	// --- [เพิ่มส่วนนี้] ตัวแปรจำว่าคุยหรือยัง ---
	public bool hasTalked = false;
	// ------------------------------------

	void Start()
	{
		if (npcAnimator == null) npcAnimator = GetComponent<Animator>();
	}

	public void TriggerDialogue()
	{
		// ถ้าคุยไปแล้ว ไม่ต้องทำอะไรเลย (จบข่าว)
		if (hasTalked) return;

		// ส่งตัว NPC นี้ (this) ไปให้ Manager จัดการ
		FindObjectOfType<DialogueManager>().StartDialogue(this);
	}
}