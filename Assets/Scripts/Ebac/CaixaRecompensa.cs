using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaixaRecompensa : MonoBehaviour
{
    public GameObject prefabRecompensa;

    private bool jogadorProximo = false;
    private float tempoSegurando = 0f;
    private bool recompensaEntregue = false;

    private GameObject textoTeclaB;
    private Image circleLoader;

    // Mudar a cor do circleloader
    public Color corInicial = Color.white;
    public Color corFinal = Color.green;

    private void OnEnable()
    {
        textoTeclaB = transform.Find("Texto_Tecla_B")?.gameObject;
        circleLoader = transform.Find("UI_Recompensa/Canvas/CircleLoader")?.GetComponent<Image>();

        if (textoTeclaB != null)
            textoTeclaB.SetActive(false);

        if (circleLoader != null)
        {
            circleLoader.fillAmount = 0f;
            circleLoader.color = corInicial;
            circleLoader.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (jogadorProximo && !recompensaEntregue)
        {
            if (Input.GetKey(KeyCode.B))
            {
                tempoSegurando += Time.deltaTime;

                if (circleLoader != null)
                {
                    float progresso = Mathf.Clamp01(tempoSegurando / 2f);
                    circleLoader.fillAmount = progresso;
                    circleLoader.color = Color.Lerp(corInicial, corFinal, progresso);
                }

                if (tempoSegurando >= 2f)
                {
                    EntregarRecompensa();
                }
            }
            else
            {
                tempoSegurando = 0f;

                if (circleLoader != null)
                {
                    circleLoader.fillAmount = 0f;
                    circleLoader.color = corInicial;
                }
            }
        }
    }

    private void EntregarRecompensa()
    {
        recompensaEntregue = true;
        GameManager.Instance.SelecionarVisual(prefabRecompensa);
        Debug.Log("Prefab escolhido: " + prefabRecompensa.name);

        foreach (var caixa in FindObjectsOfType<CaixaRecompensa>())
        {
            if (caixa != this)
                Destroy(caixa.gameObject);
        }

        if (textoTeclaB != null)
            textoTeclaB.SetActive(false);

        if (circleLoader != null)
            circleLoader.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorProximo = true;

            if (textoTeclaB != null)
                textoTeclaB.SetActive(true);

            if (circleLoader != null)
            {
                circleLoader.fillAmount = 0f;
                circleLoader.color = corInicial;
                circleLoader.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorProximo = false;
            tempoSegurando = 0f;

            if (textoTeclaB != null)
                textoTeclaB.SetActive(false);

            if (circleLoader != null)
            {
                circleLoader.fillAmount = 0f;
                circleLoader.color = corInicial;
                circleLoader.gameObject.SetActive(false);
            }
        }
    }
}
