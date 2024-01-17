using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public event Action GameOver;
    public static GameManager Instancia {get; private set;}
    public int Monedas { get; private set;} = 0;
    public float Distancia {get; private set;} = 0;
    public bool IsPaused {get; private set;} = false;

    [SerializeField] private StatsJugador statsJugador;
    [SerializeField] private BehaviourCamera scriptCamara;
    [SerializeField] private MonedaManager monedaManager;
    [SerializeField] private PistaManager pistaManager;

    private void Awake()
    {
        Instancia = this;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        IsPaused = true;
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1;
        IsPaused = false;
    }

    public void ResetLevel()
    {
        UnPauseGame();
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
    }

    public void AddDistancia(float desplazamiento)
    {
        if(desplazamiento > 0)
        {
            Distancia += desplazamiento;
        }
    }

    public void InvocarGameOver()
    {
        GameOver.Invoke();
    }

    public void JugadorDobloDerecha(GameObject pista, Rama rama)
    {
        pistaManager.CambiarSendaPrincipal(pista, rama);

        rama.AlinearSegunPadre(pista.GetComponent<BehaviourPista>().EjeMovimiento);

        scriptCamara.AcomodarCamara(rama.EjeMovimiento);

        monedaManager.JugadorDoblo();
    }

    public void JugadorDobloIzquierda(GameObject pista, Rama rama)
    {
        pistaManager.CambiarSendaPrincipal(pista, rama);

        rama.AlinearSegunPadre(pista.GetComponent<BehaviourPista>().EjeMovimiento);

        scriptCamara.AcomodarCamara(rama.EjeMovimiento);

        monedaManager.JugadorDoblo();
    }
}
