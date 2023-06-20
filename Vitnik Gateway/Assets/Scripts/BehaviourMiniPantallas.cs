using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourMiniPantallas : MonoBehaviour
{
    [SerializeField] private GameObject pantallaGameOver;
    [SerializeField] private GameObject pantallaPausa;
    [SerializeField] private GameObject pantallaOpciones;
    [SerializeField] private GameManager gameManager;

    public bool pantallaPausaActive {get {return pantallaPausa.activeSelf;}}
    public bool pantallaOpcionesActive {get {return pantallaOpciones.activeSelf;}}

    private bool pantallaPausaActivaAnterior = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PantallaGameOverSetActive(bool nuevoEstado)
    {
        pantallaGameOver.SetActive(nuevoEstado);

        pantallaGameOver.GetComponent<BehaviourMiniPantallaGameOver>().Actualizar();

        if(nuevoEstado) 
        {
            gameManager.PauseGame();
        }
    }

    public void PantallaPausaSetActive(bool nuevoEstado)
    {
        pantallaPausa.SetActive(nuevoEstado);

        if(nuevoEstado)
        {
            gameManager.PauseGame();
        }
    }

    public void PantallaOpcionesSetActive(bool nuevoEstado)
    {
        pantallaOpciones.SetActive(nuevoEstado);

        if(pantallaPausa.activeSelf)
        {
            pantallaPausaActivaAnterior = pantallaPausa.activeSelf;
            PantallaPausaSetActive(false);
        }

        if(nuevoEstado)
        {
            pantallaOpciones.GetComponent<BehaviourMiniPantallaOpciones>().Actualizar();
            gameManager.PauseGame();
        }
        else
        {
            SoundManager.Instancia.GuardarOpciones();

            if(pantallaPausaActivaAnterior)
            {
                PantallaPausaSetActive(true);
                pantallaPausaActivaAnterior = false;
            }
        }
    }

    public void IrAMenuPrincipal()
    {
        BehaviourSceneManager.IrAEscena(TipoEscena.MENUPRINCIPAL);
    }
}
