using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourMoneda : MonoBehaviour
{
    public BehaviourPista pistaAsociada;
    private Quaternion rotacionOriginal;

    void Start()
    {
        rotacionOriginal = transform.rotation;
    }

    public void ReiniciarMoneda()
    {
        transform.rotation = rotacionOriginal;
    }

    public void Agarrar()
    {
        SoundManager.Instancia.ReproducirSonido(Sonido.Moneda);
        pistaAsociada.RemoverMoneda(gameObject);
        gameObject.SetActive(false);
        ReiniciarMoneda();
    }
}
