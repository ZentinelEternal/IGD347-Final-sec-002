using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // <--- 1. เพิ่มบรรทัดนี้ครับ (สำหรับ URP)

public class HidingScreenEffect : MonoBehaviour
{
	[Header("Setup")]
	public Volume globalVolume;

	[Header("Blur Settings")]
	public float normalFocusDistance = 10f;
	public float hiddenFocusDistance = 0.1f;
	public float transitionSpeed = 5f;

	private DepthOfField dofComponent; // ตอนนี้มันจะรู้จักคำนี้แล้ว
	private float currentFocusDistance;
	private float targetFocusDistance;

	void Start()
	{
		if (globalVolume == null) globalVolume = GetComponent<Volume>();

		// 2. ถ้ามันยัง error ตรง TryGet ให้เปลี่ยนเป็น <DepthOfField> แบบนี้
		if (globalVolume.profile.TryGet<DepthOfField>(out dofComponent))
		{
			currentFocusDistance = normalFocusDistance;
			targetFocusDistance = normalFocusDistance;
			dofComponent.focusDistance.value = currentFocusDistance;
		}
		else
		{
			// ถ้าหาไม่เจอ อาจเป็นเพราะใน Profile ยังไม่ได้ Add Override > Depth Of Field
			Debug.LogWarning("ยังไม่ได้เพิ่ม Depth of Field ใน Global Volume Profile!");
		}
	}

	void Update()
	{
		if (dofComponent == null) return;

		currentFocusDistance = Mathf.Lerp(currentFocusDistance, targetFocusDistance, Time.deltaTime * transitionSpeed);
		dofComponent.focusDistance.value = currentFocusDistance;
	}

	public void SetBlurred(bool isBlurred)
	{
		if (isBlurred)
		{
			targetFocusDistance = hiddenFocusDistance;
		}
		else
		{
			targetFocusDistance = normalFocusDistance;
		}
	}
}