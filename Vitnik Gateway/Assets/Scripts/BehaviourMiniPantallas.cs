using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourMiniPantallas : MonoBehaviour
{
    [SerializeField] private BehaviourMiniPantallaGameOver pantallaGameOver;
    [SerializeField] private BehaviourMiniPantallaPausa pantallaPausa;
    [SerializeField] private BehaviourMiniPantallaOpciones pantallaOpciones;

    void Start()
    {
        GameManager.Instancia.GameOver += GameOver;
    }

    void OnDestroy()
    {
        GameManager.Instancia.GameOver -= GameOver;
    }

    public void PantallaGameOverSetActive(bool nuevoEstado)
    {
        if(nuevoEstado)
        {
            pantallaGameOver.Activar();
            GameManager.Instancia.PauseGame();
        }   
        else
        {
            pantallaGameOver.Desactivar();
        }

        pantallaGameOver.Actualizar();
    }

    public void PantallaPausaSetActive(bool nuevoEstado)
    {
        if(nuevoEstado)
        {
            GameManager.Instancia.PauseGame();
            pantallaPausa.Activar();
        }
        else
        {
            GameManager.Instancia.UnPauseGame();
            pantallaPausa.Desactivar();
        }
    }

    public void PantallaOpcionesSetActive(bool nuevoEstado)
    {
        if(nuevoEstado)
        {
            pantallaPausa.Desactivar();
            pantallaOpciones.Activar();
            pantallaOpciones.Actualizar();
        }
        else
        {
            SoundManager.Instancia.GuardarOpciones();
            pantallaOpciones.Desactivar();
            pantallaPausa.Activar();
        }
    }

    public void CerrarTodo()
    {
        SoundManager.Instancia.GuardarOpciones();
        pantallaOpciones.Desactivar();
        pantallaPausa.Desactivar();
        GameManager.Instancia.UnPauseGame();
    }

    public void IrAMenuPrincipal()
    {
        BehaviourSceneManager.IrAEscena(TipoEscena.MENUPRINCIPAL);
    }

    public void GameOver()
    {
        PantallaGameOverSetActive(true);
    }
}
