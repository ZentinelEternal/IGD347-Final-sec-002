using UnityEngine;

public class CursorControl : MonoBehaviour
{
    private void Start()
    {
        // ทำให้เมาส์ขยับที่หน้าเมนูได้
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
