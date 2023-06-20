using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instancia {get; private set;}
    [SerializeField] private AudioListener escucha;
    [SerializeField] private AudioSource reproductorMusica;
    [SerializeField] private AudioSource reproductorSonido;
    [SerializeField] private ConexionDatabase databaseOpciones;

    //Sonidos
    [SerializeField] private AudioClip sonidoMoneda;

    //Musica
    [SerializeField] private AudioClip musicaNivel1;

    void Start()
    {
        if(Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //escucha = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioListener>();
        //reproductor = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
        CargarOpciones();
        ReproducirMusica(Musica.Nivel1);
    }

    private void CargarOpciones()
    {
        reproductorSonido.volume = System.Convert.ToSingle(databaseOpciones.ObtenerPrimerValor("VolumenSonido"));
        reproductorMusica.volume = System.Convert.ToSingle(databaseOpciones.ObtenerPrimerValor("VolumenMusica"));
    }

    public void ReproducirMusica(Musica clip)
    {
        AudioClip clipMusica = ObtenerClipMusica(clip);

        if(clipMusica != null)
        {
            reproductorMusica.Stop();
            reproductorMusica.clip = clipMusica;
            reproductorMusica.Play();
        }
        else
        {
            Debug.Log("Clip de música no encontrado.");
        }
    }

    private AudioClip ObtenerClipMusica(Musica clip)
    {
        switch(clip)
        {
            case Musica.Nivel1:
                return musicaNivel1;

            default:
                return null;
        }
    }

    public void ReproducirSonido(Sonido clip)
    {
        AudioClip clipSonido = ObtenerClipSonido(clip);

        if(clipSonido != null)
        {
            reproductorSonido.PlayOneShot(clipSonido, reproductorSonido.volume);
        }
        else
        {
            Debug.Log("Clip de sonido no encontrado.");
        }
    }

    private AudioClip ObtenerClipSonido(Sonido clip)
    {
        switch(clip)
        {
            case Sonido.Moneda:
                return sonidoMoneda;

            default:
                return null;
        }
    }

    public void EstablecerVolumenMusica(float volumen)
    {
        volumen = Mathf.Clamp(volumen, 0 , 1);
        reproductorMusica.volume = volumen;
    }

    
    public void EstablecerVolumenSonido(float volumen)
    {
        volumen = Mathf.Clamp(volumen, 0 , 1);
        reproductorSonido.volume = volumen;
    }

    public float ObtenerVolumenMusica()
    {
        return reproductorMusica.volume;
    }

    public float ObtenerVolumenSonido()
    {
        return reproductorSonido.volume;
    }

    public void GuardarOpciones()
    {
        databaseOpciones.ModificarValor("VolumenMusica", reproductorMusica.volume, "ID", 0);
        databaseOpciones.ModificarValor("VolumenSonido", reproductorSonido.volume, "ID", 0);
    }

}

public enum Sonido
{
    Moneda
}

public enum Musica
{
    Nivel1
}
