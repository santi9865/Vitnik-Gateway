using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BehaviourSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static void IrAEscena(TipoEscena tipo)
    {
        if(TipoANombreEscena(tipo) != null)
        {
            SceneManager.LoadScene(TipoANombreEscena(tipo));
            Debug.Log("Cargada escena: "+ TipoANombreEscena(tipo));
        }
        else
        {
            Debug.Log("Tipo de escena desconocido.");
        }


    }

    public void Salir()
    {
        Application.Quit();
    }

    private static string TipoANombreEscena(TipoEscena tipo)
    {
        switch (tipo)
        {
            case TipoEscena.MENUPRINCIPAL:
                return "MenuPrincipal";
            case TipoEscena.LOGROS:
                return "Logros";
            case TipoEscena.TIENDA:
                return "Tienda";
            case TipoEscena.INVENTARIO:
                return "Inventario";
            case TipoEscena.SELECNIVEL:
                return "SelecNivel";
            case TipoEscena.NIVEL1:
                return "Nivel1";
            default:
                return null;
        }
    }
}

public enum TipoEscena
{
    MENUPRINCIPAL, LOGROS, TIENDA, INVENTARIO, SELECNIVEL, NIVEL1
}
