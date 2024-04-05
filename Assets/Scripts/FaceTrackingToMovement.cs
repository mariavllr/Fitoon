using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class FaceTrackingToMovement : MonoBehaviour
{
    ARFaceManager faceManager;
    ARFace face;
    Animator animator;
    private bool detectado = false;
    private Vector3 forwardDirection;
    private Rigidbody rb;
    private FadeCamera fadeCamera;

    [SerializeField] private TextMeshProUGUI uiText;

    private void OnEnable()
    {
        faceManager = FindFirstObjectByType<ARFaceManager>();
        fadeCamera = FindFirstObjectByType<FadeCamera>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        faceManager.facesChanged += CaraDetectada;
        uiText.text = "Mira hacia la cámara con la cara visible para calibrar";
        if(fadeCamera != null) fadeCamera.StartFade(true);

    }

    private void OnDisable()
    {
        faceManager.facesChanged -= CaraDetectada;
    }



    void Update()
    {
        if (detectado)
        {
            //Forward Direction of the character
            forwardDirection = new Vector3(face.transform.forward.x, face.transform.forward.y, face.transform.forward.z);
            forwardDirection.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(forwardDirection);
            transform.rotation = targetRotation;

            //Movement
            Vector3 moveDirection = forwardDirection * 2f * Time.deltaTime;
            rb.MovePosition(rb.position + moveDirection);
        }

        else
        {
           // DebugMovement(); //para probar en el editor
        }

    }


    private void DebugMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Rotate the character based on horizontal input
        transform.Rotate(Vector3.up, horizontalInput * 100f * Time.deltaTime);

        // Move the character forward based on vertical input
        if (verticalInput != 0)
        {
            // Get the forward vector of the character
            Vector3 moveDirection = transform.forward * verticalInput * 2f * Time.deltaTime;

            // Move the character
            rb.MovePosition(rb.position + moveDirection);
            animator.SetBool("isRunning", true);
        }

        else
        {
            animator.SetBool("isRunning", false);
        }
    }



    //Cuando una cara es detectada, se lanza un evento y se ejecuta esta función
    void CaraDetectada(ARFacesChangedEventArgs aRFacesChangedEventArgs)
    {
        //Si existe una cara en "updated" (lista de caras detectadas)
        if(aRFacesChangedEventArgs.updated != null && aRFacesChangedEventArgs.updated.Count > 0 && !detectado)
        {
            //Guardar el objeto de la cara
            face = aRFacesChangedEventArgs.updated[0];
            detectado = true;
            Debug.Log("CARA DETECTADA!");
            animator.SetBool("isRunning", true);

            StartCoroutine(ShowUIMessage("Cara detectada"));
            if (fadeCamera != null) fadeCamera.StartFade(false);
        }

        if (aRFacesChangedEventArgs.removed.Count > 0)
        {
            Debug.Log("Cara quitada");
            detectado = false;
            animator.SetBool("isRunning", false);

            StartCoroutine(ShowUIMessage("Cara no reconocida"));
            if (fadeCamera != null) fadeCamera.StartFade(true);
        }
    }

    IEnumerator ShowUIMessage(string message)
    {
        uiText.text = message;
        yield return new WaitForSeconds(2f);
        uiText.text = "";
    }
}
