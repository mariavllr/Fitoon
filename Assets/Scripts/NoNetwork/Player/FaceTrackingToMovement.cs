using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;



public class FaceTrackingToMovement : MonoBehaviour
{
    [Header("AR")]
    ARFaceManager faceManager;
    ARFace face;
    Animator animator;
    private bool detectado = false;
    private FadeCamera fadeCamera;

    [Header("Rotation")]
    private Rigidbody rb;
    [SerializeField] int rotationIntensity = 2;

    [Header("Debug")]
    public TMP_InputField enter_intensity;
    public Transform debugCube;
    // [SerializeField] private TextMeshProUGUI uiText;
    private GoalController goal;

    //EVENTOS (Para el movimiento del personaje)
    public delegate void OnCaraDetectada();
    public static event OnCaraDetectada onCaraDetectadaEvent;

    public delegate void OnCaraNoDetectada();
    public static event OnCaraNoDetectada onCaraNoDetectadaEvent;

    


    private void OnEnable()
    {
        faceManager = FindFirstObjectByType<ARFaceManager>();
        fadeCamera = FindFirstObjectByType<FadeCamera>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        faceManager.facesChanged += CaraDetectada;
        //uiText.text = "Mira hacia la cámara con la cara visible para calibrar";
        if(fadeCamera != null) fadeCamera.StartFade(true);

        goal = FindObjectOfType<GoalController>();
        goal.AddPlayerToList(transform);

    }

    private void OnDisable()
    {
        faceManager.facesChanged -= CaraDetectada;
    }


    //----------AR ROTATION--------------------
    void Update()
    {
        if (detectado)
        {
            RotateCharacter(face.transform);
        }

    }

    private void RotateCharacter(Transform otherObject)
    {
        // Face forward rotation
        float angle = GetObjectRotation(otherObject);

        // Rotation with intensity multiplier
        float rotationWithMult = angle * rotationIntensity;

        // Clamp bewteen -90 and 90
        float clampedRotation = Mathf.Clamp(rotationWithMult, -90f, 90f);

        // Rotate the character
        Quaternion desiredRotation = Quaternion.Euler(0f, clampedRotation, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, 1f);
    }

    private float GetObjectRotation(Transform obj)
    {
        if (obj.transform.eulerAngles.y > 180)
        {
            return obj.transform.eulerAngles.y - 360;
        }
        else
        {
            return obj.transform.eulerAngles.y;
        }
    }
    
    //-----------EVENTS---------
    void CaraDetectada(ARFacesChangedEventArgs aRFacesChangedEventArgs)
    {
        //Si existe una cara en "updated" (lista de caras detectadas)
        if (aRFacesChangedEventArgs.updated != null && aRFacesChangedEventArgs.updated.Count > 0 && !detectado)
        {
            //Guardar el objeto de la cara
            face = aRFacesChangedEventArgs.updated[0];
            detectado = true;
            Debug.Log("CARA DETECTADA!");

            if (onCaraDetectadaEvent != null)
            {
                onCaraDetectadaEvent();
            }


            StartCoroutine(ShowUIMessage("Cara detectada"));
            if (fadeCamera != null) fadeCamera.StartFade(false);
        }

        if (aRFacesChangedEventArgs.removed.Count > 0)
        {
            Debug.Log("Cara quitada");
            detectado = false;

            if(onCaraNoDetectadaEvent != null)
            {
                onCaraNoDetectadaEvent();
            }

            StartCoroutine(ShowUIMessage("Cara no reconocida"));
            if (fadeCamera != null) fadeCamera.StartFade(true);
        }
    }

    IEnumerator ShowUIMessage(string message)
    {
        //uiText.text = message;
        yield return new WaitForSeconds(2f);
        //uiText.text = "";
    }



    //-------DEBUG------------
    public void ChangeRotationIntensity()
    {
        string intensity = enter_intensity.text;
        int.TryParse(intensity, out rotationIntensity);
        Debug.Log("Rotacion cambiada a: " + rotationIntensity);
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
            Vector3 moveDirection = transform.forward * Time.deltaTime;

            // Move the character
            rb.AddForce(moveDirection * 700f, ForceMode.Acceleration);
            animator.SetBool("isRunning", true);
        }

        else
        {
            animator.SetBool("isRunning", false);
        }
    }



    
}
