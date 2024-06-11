using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PunteroUI : MonoBehaviour
{
    ARFaceManager faceManager;
    ARFace face;
    bool detectado = false;
    Vector3 direction;

    public RectTransform canvasRect; // Referencia al RectTransform del Canvas
    public RectTransform puntero; // Referencia al RectTransform del punto

    void Awake()
    {
        faceManager = FindFirstObjectByType<ARFaceManager>();
    }

    private void OnEnable()
    {
        faceManager.facesChanged += CaraDetectada;
    }

    private void OnDisable()
    {
        faceManager.facesChanged -= CaraDetectada;
    }

    private void Update()
    {
        if (detectado)
        {
            if (face != null)
            {
                
                direction = face.transform.position + face.transform.forward *2;

                Vector2 screenPoint = Camera.main.WorldToScreenPoint(direction);

                // Convertimos el punto de pantalla al espacio del Canvas
                Vector2 canvasPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, Camera.main, out canvasPos);

                // Movemos el punto en el Canvas
                puntero.localPosition = canvasPos;

                Debug.Log(puntero.localPosition);
            }
        }
    }

    void CaraDetectada(ARFacesChangedEventArgs aRFacesChangedEventArgs)
    {
        //Si existe una cara en "updated" (lista de caras detectadas)
        if (aRFacesChangedEventArgs.updated != null && aRFacesChangedEventArgs.updated.Count > 0 && !detectado)
        {
            //Guardar el objeto de la cara
            face = aRFacesChangedEventArgs.updated[0];
            detectado = true;
            Debug.Log("detectada");
        }

        if (aRFacesChangedEventArgs.removed.Count > 0)
        {
            detectado = false;
            Debug.Log("no detectada");
        }
    }
}
