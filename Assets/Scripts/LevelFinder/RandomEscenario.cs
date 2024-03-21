using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomEscenario : MonoBehaviour
{
    [SerializeField] List<EscenarioItem> escenarios;
    [SerializeField] GameObject container;
    [SerializeField] TextMeshProUGUI countdownText;
    [SerializeField] int countdown;
    Image image;
    TextMeshProUGUI text;
    EscenarioItem escenarioElegido;

    public float secondsToChange;
    public float totalSeconds;
    float timer = 0;
    bool timerActive = true;
    void Start()
    {
        image = container.GetComponentInChildren<Image>();
        text = container.GetComponentInChildren<TextMeshProUGUI>();
        StartCoroutine(CambiarImagenAleatoria());
    }

    private void Update()
    {
        if(timerActive) timer += Time.deltaTime;

    }

    IEnumerator CambiarImagenAleatoria()
    {
        while (timer < totalSeconds) 
        {

            yield return new WaitForSeconds(secondsToChange);

            escenarioElegido = escenarios[Random.Range(0, escenarios.Count)];
            image.sprite = escenarioElegido.imagenEscenario;
            text.text = escenarioElegido.nombreEscenario;
        }

        //timer = 0;
        timerActive = false;
        countdownText.text = countdown.ToString();
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {       
        while (countdown != 0)
        {
            yield return new WaitForSeconds(1f);
            countdown--;
            if (countdown == 0)
            {
                GetComponent<ButtonFunctions>().LoadScene(escenarioElegido.nombreEscenario);
            }
            else
            {
                countdownText.text = countdown.ToString();
            }
        }

    }
}
