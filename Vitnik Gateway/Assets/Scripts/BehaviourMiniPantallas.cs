using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourMiniPantallas : MonoBehaviour
{
    [SerializeField] private GameObject pantallaGameOver;
    [SerializeField] private GameObject pantallaPausa;
    [SerializeField] private GameManager gameManager;


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

    public void IrAMenuPrincipal()
    {
        BehaviourSceneManager.IrAEscena(TipoEscena.MENUPRINCIPAL);
    }
}
