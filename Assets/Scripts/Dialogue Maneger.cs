using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{

	public GameObject dialoguePanel;
	public TextMeshProUGUI nameText;
	public TextMeshProUGUI dialogueText;
	public GameObject interactTip;

	private Queue<string> sentences;
	private DialogueTrigger currentNPC; // เปลี่ยนมาเก็บตัว Trigger แทน
	private Animator currentNPCAnimator;

	private bool canContinue = false;
	private Coroutine typingCoroutine;

	void Start()
	{
		sentences = new Queue<string>();
		if (dialoguePanel != null) dialoguePanel.SetActive(false);
		if (interactTip != null) interactTip.SetActive(false);
	}

	// --- [แก้ฟังก์ชันนี้] รับ DialogueTrigger มาตัวเดียวพอ เพราะข้างในมีครบหมดแล้ว ---
	public void StartDialogue(DialogueTrigger trigger)
	{

		currentNPC = trigger; // จำไว้ว่าคุยกับใคร
		currentNPCAnimator = currentNPC.npcAnimator; // ดึง Animator มา

		// แช่แข็ง NPC
		if (currentNPCAnimator != null) currentNPCAnimator.speed = 0;

		// จัดการ UI
		if (interactTip != null) interactTip.SetActive(false);
		if (dialoguePanel != null) dialoguePanel.SetActive(true); // เปิดกล่องข้อความ

		// เริ่มโหลดประโยค
		nameText.text = currentNPC.dialogue.name;
		sentences.Clear();
		foreach (string sentence in currentNPC.dialogue.sentences)
		{
			sentences.Enqueue(sentence);
		}

		StartCoroutine(EnableContinueButton());
		DisplayNextSentence();
	}

	// ... (ฟังก์ชัน EnableContinueButton เหมือนเดิม) ...
	IEnumerator EnableContinueButton()
	{
		canContinue = false;
		yield return new WaitForSeconds(0.2f);
		canContinue = true;
	}

	// ... (ฟังก์ชัน DisplayNextSentence และ TypeSentence เหมือนเดิม) ...
	public void DisplayNextSentence()
	{
		if (sentences.Count == 0)
		{
			EndDialogue();
			return;
		}
		string sentence = sentences.Dequeue();
		if (typingCoroutine != null) StopCoroutine(typingCoroutine);
		typingCoroutine = StartCoroutine(TypeSentence(sentence));
	}

	IEnumerator TypeSentence(string sentence)
	{
		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
			yield return null;
		}
	}

	// --- [แก้ฟังก์ชันนี้] จุดสำคัญที่ทำให้กล่องหายและคุยซ้ำไม่ได้ ---
	void EndDialogue()
	{
		// 1. ปิดกล่องข้อความทันที
		if (dialoguePanel != null) dialoguePanel.SetActive(false);

		// 2. สั่งให้ NPC ตัวนี้จำว่า "คุยแล้วนะ"
		if (currentNPC != null)
		{
			currentNPC.hasTalked = true;

			// คืนค่า Animator ให้ขยับต่อ
			if (currentNPCAnimator != null)
			{
				currentNPCAnimator.speed = 1;
			}

			// ล้างค่าทิ้ง
			currentNPC = null;
			currentNPCAnimator = null;
		}

		canContinue = false;
	}

	// ... (Update และ ShowInteractTip เหมือนเดิม) ...
	void Update()
	{
		if (dialoguePanel.activeSelf)
		{
			if (canContinue && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
			{
				DisplayNextSentence();
			}
		}
	}

	public void ShowInteractTip(bool isShow)
	{
		if (interactTip != null) interactTip.SetActive(isShow);
	}
}