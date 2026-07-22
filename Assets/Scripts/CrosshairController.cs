using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    public Camera playerCamera;

    public Image crosshair;

    public Sprite dotSprite;
    public Sprite cameraSprite;

    public float checkDistance = 8f;

    void Update()
    {
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, checkDistance))
        {
            if (hit.collider.GetComponent<PhotoTarget>() != null)
            {
                SetCrosshair(cameraSprite);
                return;
            }
        }

        SetCrosshair(dotSprite);
    }

    void SetCrosshair(Sprite sprite)
    {
        if (crosshair.sprite != sprite)
            crosshair.sprite = sprite;
    }
}