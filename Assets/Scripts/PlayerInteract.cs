using UnityEngine;

public class PlayerInteract : MonoBehaviour
{

	[Header("Status")]
	public bool isHidden = false; // ตัวแปรให้ Enemy มาเช็คว่าเราแอบอยู่ไหม

	// ตัวแปรเก็บ Object ที่เรายืนใกล้ๆ
	private DialogueTrigger currentNPC;
	private DoorPuzzle currentDoor; // [ใหม่] เก็บประตู

	private DialogueManager manager;

	void Start()
	{
		manager = FindObjectOfType<DialogueManager>();
	}

	void Update()
	{
		// --- ตรวจจับการกดปุ่ม E ---
		if (Input.GetKeyDown(KeyCode.E))
		{

			// กรณีที่ 1: ยืนหน้าประตู (อ่านโน้ต)
			if (currentDoor != null)
			{
				currentDoor.InteractWithDoor();
				// สั่งซ่อนปุ่ม [E] ทันทีที่กด
				if (manager != null) manager.ShowInteractTip(false);
				return; // จบการทำงานรอบนี้ (กันไม่ให้กดซ้อน)
			}

			// กรณีที่ 2: ยืนหน้า NPC (คุย)
			// เช็คเพิ่มด้วยว่า NPC ตัวนี้ยังไม่เคยคุย (hasTalked == false)
			if (currentNPC != null && !currentNPC.hasTalked)
			{
				currentNPC.TriggerDialogue();
				return;
			}
		}
	}

	// --- เมื่อเดิน "เข้า" โซน ---
	private void OnTriggerEnter(Collider other)
	{

		// 1. เช็คว่าเป็น NPC ไหม?
		if (other.CompareTag("NPC"))
		{
			DialogueTrigger npc = other.GetComponent<DialogueTrigger>();

			// ถ้ามีสคริปต์ และ ยังไม่เคยคุย -> ให้ขึ้นปุ่ม E
			if (npc != null && !npc.hasTalked)
			{
				currentNPC = npc;
				if (manager != null) manager.ShowInteractTip(true);
			}
		}

		// 2. [ใหม่] เช็คว่าเป็น ประตู (DoorPuzzle) ไหม?
		DoorPuzzle door = other.GetComponent<DoorPuzzle>();
		if (door != null)
		{
			currentDoor = door;
			if (manager != null) manager.ShowInteractTip(true); // ขึ้นปุ่ม E
		}
	}

	// --- เมื่อเดิน "ออก" จากโซน ---
	private void OnTriggerExit(Collider other)
	{

		// 1. เดินออกจาก NPC
		if (other.CompareTag("NPC"))
		{
			// เช็คว่าเป็นตัวเดียวกับที่เราจ้องอยู่ไหม (กันบั๊กกรณี NPC ยืนติดกัน)
			if (currentNPC == other.GetComponent<DialogueTrigger>())
			{
				currentNPC = null;
				if (manager != null) manager.ShowInteractTip(false); // ซ่อนปุ่ม E
			}
		}

		// 2. [ใหม่] เดินออกจาก ประตู
		DoorPuzzle door = other.GetComponent<DoorPuzzle>();
		if (door != null)
		{
			if (currentDoor == door)
			{
				currentDoor = null;
				if (manager != null) manager.ShowInteractTip(false); // ซ่อนปุ่ม E
			}
		}
	}

	// --- เช็คสถานะตลอดเวลา (กันเหนียว) ---
	private void OnTriggerStay(Collider other)
	{
		// กรณีพิเศษ: ถ้าคุยกับ NPC จบแล้ว ให้ปุ่ม E หายไปทันที
		if (other.CompareTag("NPC"))
		{
			DialogueTrigger npc = other.GetComponent<DialogueTrigger>();

			if (npc != null && npc.hasTalked && currentNPC == npc)
			{
				currentNPC = null;
				if (manager != null) manager.ShowInteractTip(false);
			}
		}
	}
}