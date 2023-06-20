using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourMiniPantallaOpciones : MonoBehaviour
{
    [SerializeField] Slider sliMusica;
    [SerializeField] Slider sliSonido;

    // Start is called before the first frame update
    void Start()
    {
        Actualizar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Actualizar()
    {
        sliMusica.value = SoundManager.Instancia.ObtenerVolumenMusica();
        sliSonido.value = SoundManager.Instancia.ObtenerVolumenSonido();
    }

    public void ModificarVolumenMusica()
    {
        SoundManager.Instancia.EstablecerVolumenMusica(sliMusica.value);
    }

    public void ModificarVolumenSonido()
    {
        SoundManager.Instancia.EstablecerVolumenSonido(sliSonido.value);
    }
}
