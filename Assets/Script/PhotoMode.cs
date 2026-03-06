using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class PhotoMode : MonoBehaviour
{
    [Header("Camera")]
    public Camera playerCamera;
    public Transform cameraHolder;
    public GameObject photoUI; 

    [Header("Photo Mode Settings")]
    public float normalFOV = 60f;
    public float photoFOV = 30f;
    public Vector3 normalPos;
    public Vector3 photoPos;
    public float transitionTime = 0.25f;

    [Header("Flash Settings")]
    public Image flashImage;       // L'image blanche fullscreen
    public float flashDuration = 0.05f;

    [Header("Raycast Settings")]
    public float sphereRadius = 0.5f; 
    public float maxDistance = 100f;

    public TextMeshProUGUI scoreText;

    int score = 0;
    bool photoMode = false;
    Coroutine currentTransition;
    PlayerInput playerInput;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        playerInput.actions["aim"].performed += DetectPhotoMode;
        playerInput.actions["aim"].canceled += DetectPhotoMode;
        playerInput.actions["click"].performed += TakePhoto;
    }

    private void OnDisable()
    {
        playerInput.actions["aim"].performed -= DetectPhotoMode;
        playerInput.actions["aim"].canceled -= DetectPhotoMode;
        playerInput.actions["click"].performed -= TakePhoto;
    }
    public void DetectPhotoMode(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            photoMode = true;
            StartTransition(EnterPhotoMode());
        }

        if (ctx.canceled)
        {
            photoMode = false;
            StartTransition(ExitPhotoMode());
        }
    }

    public void TakePhoto(InputAction.CallbackContext ctx)
    {
        if (!photoMode || !ctx.performed) return;
        StartCoroutine(FlashCoroutine());
        ShootSphereRay();
    }


    void ShootSphereRay()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        // SphereCast pour détecter un seul objet
        if (Physics.SphereCast(ray, sphereRadius, out hit, maxDistance))
        {
            PhotoTarget target = hit.collider.GetComponent<PhotoTarget>();

            if (target != null)
            {
                score += target.PhotoValue;
                scoreText.text = "Score: " + score;
            }
        }
    }


    IEnumerator FlashCoroutine()
    {
        flashImage.gameObject.SetActive(true);
        flashImage.color = new Color(1f, 1f, 1f, 1f); // alpha = 1

        float t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / flashDuration);
            flashImage.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        flashImage.gameObject.SetActive(false);
    }

    void StartTransition(IEnumerator routine)
    {
        if (currentTransition != null)
            StopCoroutine(currentTransition);

        currentTransition = StartCoroutine(routine);
    }

    IEnumerator EnterPhotoMode()
    {
        photoUI.SetActive(true);

        float startFOV = playerCamera.fieldOfView;
        Vector3 startPos = cameraHolder.localPosition;

        float t = 0;

        while (t < transitionTime)
        {
            t += Time.deltaTime;
            float lerp = t / transitionTime;

            playerCamera.fieldOfView = Mathf.Lerp(startFOV, photoFOV, lerp);
            cameraHolder.localPosition = Vector3.Lerp(startPos, photoPos, lerp);

            yield return null;
        }
    }

    IEnumerator ExitPhotoMode()
    {
        float startFOV = playerCamera.fieldOfView;
        Vector3 startPos = cameraHolder.localPosition;

        float t = 0;

        while (t < transitionTime)
        {
            t += Time.deltaTime;
            float lerp = t / transitionTime;

            playerCamera.fieldOfView = Mathf.Lerp(startFOV, normalFOV, lerp);
            cameraHolder.localPosition = Vector3.Lerp(startPos, normalPos, lerp);

            yield return null;
        }

        photoUI.SetActive(false);
    }
}
