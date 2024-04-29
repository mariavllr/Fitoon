using UnityEngine;
using System.IO;
using UnityEngine.XR.ARFoundation;

public class WriteDataToCSV : MonoBehaviour
{
    ARFaceManager faceManager;
    ARFace face;
    private bool detectado = false;
   // private StreamWriter writer;
    // Ruta donde guardaremos el archivo CSV
    string filePath;

    private void OnEnable()
    {
        faceManager = FindFirstObjectByType<ARFaceManager>();
        filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "FaceData.csv";
        faceManager.facesChanged += CaraDetectada;
        // Verifica si el archivo ya existe y lo elimina si es así
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine("Posicion X;Posicion Y;Posicion Z;Rotacion X;Rotacion Y;Rotacion Z");
        }
    }

    private void OnDisable()
    {
        faceManager.facesChanged -= CaraDetectada;
    }

    void Update()
    {
        if (detectado)
        {
            EscribirCSV();
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
            Debug.Log("Datos del transform guardandose en " + filePath);
        }

        if (aRFacesChangedEventArgs.removed.Count > 0)
        {
            detectado = false;
        }
    }

    void EscribirCSV()
    {
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            // Escribimos cada dato en una línea separada
            writer.WriteLine(string.Format("{0};{1};{2};{3};{4};{5}",
            face.transform.position.x, face.transform.position.y, face.transform.position.z, face.transform.rotation.x, face.transform.rotation.y, face.transform.rotation.z));
        }
    }
}
