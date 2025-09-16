using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeleiaTransicao : MonoBehaviour
{
    public GameObject teclaBUI;
    public float tempoParaEntrar = 2f;
    private float tempoSegurando = 0f;
    private bool jogadorPerto = false;

    private void Start()
    {
        teclaBUI.SetActive(false);
        //gameObject.SetActive(false); // começa desativado
    }

    private void Update()
    {
        if (jogadorPerto)
        {
            if (Input.GetKey(KeyCode.B))
            {
                tempoSegurando += Time.deltaTime;

                if (tempoSegurando >= tempoParaEntrar)
                {
                    GameManager.Instance.IrParaProximaCena();
                }
            }
            else
            {
                tempoSegurando = 0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = true;
            teclaBUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;
            teclaBUI.SetActive(false);
            tempoSegurando = 0f;
        }
    }
}
