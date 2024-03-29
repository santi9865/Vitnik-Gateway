using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia {get; private set;}
    public int Monedas { get; private set;} = 0;
    public float Distancia {get; private set;} = 0;

    [SerializeField] private HUDManager hudManager;
    [SerializeField] private StatsJugador statsJugador;
    [SerializeField] private BehaviourCamera scriptCamara;
    [SerializeField] private MonedaManager monedaManager;

    private void Awake()
    {
        Instancia = this;
        // if(Instancia == null)
        // {
        //     Instancia = this;
        //     DontDestroyOnLoad(gameObject);
        // }
        // else
        // {
        //     Destroy(gameObject);
        // }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1;
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IntentoTerminado()
    {
        statsJugador.AgregarMonedas(Monedas);
        Monedas = 0;
    }

    public void AddMonedas(int cantidad)
    {
        if(Monedas + cantidad > 0)
        {
            Monedas += cantidad;
        }

        hudManager.ActualizarMonedas();
    }

    public void AddDistancia(float desplazamiento)
    {
        if(desplazamiento > 0)
        {
            Distancia += desplazamiento;
        }

        hudManager.ActualizarDistancia();
    }

    public void JugadorDobloDerecha(GameObject pista, Rama rama)
    {
        PistaManager pistaManager = GetComponent<PistaManager>();
        pistaManager.CambiarSendaPrincipal(pista, rama);

        rama.AlinearSegunPadre(pista.GetComponent<BehaviourPista>().EjeMovimiento);

        scriptCamara.AcomodarCamara(rama.EjeMovimiento);

        monedaManager.JugadorDoblo();
    }

    public void JugadorDobloIzquierda(GameObject pista, Rama rama)
    {
        PistaManager pistaManager = GetComponent<PistaManager>();
        pistaManager.CambiarSendaPrincipal(pista, rama);

        rama.AlinearSegunPadre(pista.GetComponent<BehaviourPista>().EjeMovimiento);

        scriptCamara.AcomodarCamara(rama.EjeMovimiento);

        monedaManager.JugadorDoblo();
    }
}
