using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourMiniPantallas : MonoBehaviour
{
    [SerializeField] private BehaviourMiniPantallaGameOver pantallaGameOver;
    [SerializeField] private BehaviourMiniPantallaPausa pantallaPausa;
    [SerializeField] private BehaviourMiniPantallaOpciones pantallaOpciones;

    private bool pGameOverOculta = false;
    private bool pPausaOculta = false;

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
            if(pantallaPausa.gameObject.activeSelf)
            {
                pantallaPausa.Desactivar();
                pPausaOculta = true;
            }

            if(pantallaGameOver.gameObject.activeSelf)
            {
                pantallaGameOver.Desactivar();
                pGameOverOculta = true;
            }

            pantallaOpciones.Activar();
            pantallaOpciones.Actualizar();
        }
        else
        {
            SoundManager.Instancia.GuardarOpciones();
            pantallaOpciones.Desactivar();
            
            if(pPausaOculta)
            {
                pantallaPausa.Activar();
                pPausaOculta = false;
            }

            if(pGameOverOculta)
            {
                pantallaGameOver.Activar();
                pGameOverOculta = false;
            }
        }
    }

    public void CerrarTodo()
    {
        if(!pantallaGameOver.gameObject.activeSelf)
        {
            SoundManager.Instancia.GuardarOpciones();
            pantallaOpciones.Desactivar();
            pantallaPausa.Desactivar();
            GameManager.Instancia.UnPauseGame();
        }
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
