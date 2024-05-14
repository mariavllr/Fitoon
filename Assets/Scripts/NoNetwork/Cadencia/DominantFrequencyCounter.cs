using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class DominantFrequencyCounter : MonoBehaviour
{
    //Datos reales
    public List<float> data;
    string rutaPrueba = "Assets/Scripts/NoNetwork/Cadencia/datosPrueba.json";


    public float[] inputData; // Datos de la posición en x respecto al tiempo
    float sampleRate; // Frecuencia de muestreo en segundos
    int maxFFTIndex; //El indice en la FFT donde está el pico (la frecuencia)

    float frequency;

    void Start()
    {
        // Lee el archivo JSON y carga los datos
        string jsonString = File.ReadAllText(rutaPrueba);
        DatosJSON datosJSON = JsonUtility.FromJson<DatosJSON>(jsonString);
        data = new List<float>(datosJSON.data);
        inputData = data.ToArray();

        //Determinar segundos de muestra. La camara es 30fps
        sampleRate = data.Count / 30;

        //Calculo del coste
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // Realiza la Transformada Rápida de Fourier
        Complex[] complexData = new Complex[inputData.Length];
        for (int i = 0; i < inputData.Length; i++)
        {
            complexData[i] = new Complex(inputData[i], 0); // Los datos deben ser en forma de números complejos
        }
        FFT(complexData);
        

        // Encuentra la frecuencia dominante
        float maxMagnitude = 0;
        for (int i = 5; i < complexData.Length / 2; i++) //Se empieza en 5 porque la transformada tiene un pico al principio muy alto
        {
            float magnitude = complexData[i].Magnitude;
            if (magnitude > maxMagnitude)
            {
                maxMagnitude = magnitude;
                maxFFTIndex = i;
            }
        }

        frequency = maxFFTIndex / sampleRate;
        stopwatch.Stop();

        // Calcula la frecuencia
        Debug.Log("----FRECUENCIA MEDIA: METODO TRANSFORMADA RÁPIDA DE FOURIER-----");
        Debug.Log("[FFT] Frecuencia dominante: " + frequency);
        Debug.Log("Coste del algoritmo: " + stopwatch.ElapsedMilliseconds + " ms");
    }

    // Implementación de la Transformada Rápida de Fourier (FFT)
    void FFT(Complex[] data)
    {
        int n = data.Length;
        int m = (int)Mathf.Log(n, 2);

        for (int i = 0; i < n; i++)
        {
            int j = ReverseBits(i, m);
            if (j > i)
            {
                Complex temp = data[i];
                data[i] = data[j];
                data[j] = temp;
            }
        }

        for (int i = 1; i <= m; i++)
        {
            int size = 1 << i;
            int halfSize = size / 2;
            Complex twiddleFactorStep = new Complex(Mathf.Cos(Mathf.PI / halfSize), Mathf.Sin(Mathf.PI / halfSize));

            Complex twiddleFactor = new Complex(1, 0);
            for (int j = 0; j < halfSize; j++)
            {
                for (int k = j; k < n; k += size)
                {
                    int rightIndex = k + halfSize;
                    Complex temp = data[rightIndex] * twiddleFactor;
                    data[rightIndex] = data[k] - temp;
                    data[k] += temp;
                }
                twiddleFactor *= twiddleFactorStep;
            }
        }
    }

    // Función para invertir los bits de un número para FFT
    int ReverseBits(int val, int width)
    {
        int result = 0;
        for (int i = 0; i < width; i++)
        {
            result <<= 1;
            result |= val & 1;
            val >>= 1;
        }
        return result;
    }

    // Clase para representar números complejos
    public class Complex
    {
        public float Real { get; set; }
        public float Imaginary { get; set; }

        public Complex(float real, float imaginary)
        {
            Real = real;
            Imaginary = imaginary;
        }

        public float Magnitude
        {
            get { return Mathf.Sqrt(Real * Real + Imaginary * Imaginary); }
        }


        public static Complex operator +(Complex a, Complex b)
        {
            return new Complex(a.Real + b.Real, a.Imaginary + b.Imaginary);
        }

        public static Complex operator -(Complex a, Complex b)
        {
            return new Complex(a.Real - b.Real, a.Imaginary - b.Imaginary);
        }

        public static Complex operator *(Complex a, Complex b)
        {
            return new Complex(a.Real * b.Real - a.Imaginary * b.Imaginary,
                               a.Real * b.Imaginary + a.Imaginary * b.Real);
        }
    }
}
