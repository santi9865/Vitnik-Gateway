using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class LogrosManager : MonoBehaviour
{
    [SerializeField] private ConexionDatabase databaseLogros;
    [SerializeField] private ConexionDatabase databaseLogrosJugador;

    [SerializeField] private GameObject contenedorLogros;

    [SerializeField] private GameObject prefabPanelLogro;

    private List<Logro> logros;


    // Start is called before the first frame update
    void Start()
    {
        CargarLogros();
        CargarProgresoLogros();
        CrearPanelesLogros();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CargarLogros()
    {
        Debug.Log("Cargando Logros.");

        List<object> lecturaLogros = databaseLogros.ObtenerValoresColumna("ID");

        List<int> IDsLogros = new List<int>();

        foreach(object lectura in lecturaLogros)
        {
            IDsLogros.Add(System.Convert.ToInt32(lectura));
        }

        logros = new List<Logro>();

        Logro nuevoLogro;

        foreach(int ID in IDsLogros)
        {
            List<object> lecturaLogro = databaseLogros.ObtenerValoresSegunID(ID);

            nuevoLogro = new Logro
            (
                System.Convert.ToInt32(lecturaLogro[0]),
                (string)lecturaLogro[1],
                (string)lecturaLogro[2],
                (string)lecturaLogro[3],
                System.Convert.ToInt32(lecturaLogro[4]),
                System.Convert.ToInt32(lecturaLogro[5])
            );

            logros.Add(nuevoLogro);
        }
    }

    private void CargarProgresoLogros()
    {
        Debug.Log("Cargando progreso.");

        List<object> lecturaLogros = databaseLogrosJugador.ObtenerValoresColumna("IDAchievement");

        List<int> IDsProgresoJugador = new List<int>();

        foreach(object lectura in lecturaLogros)
        {
            IDsProgresoJugador.Add(System.Convert.ToInt32(lectura));
        }
        
        if(IDsProgresoJugador.Count == 0)
        {
            Debug.Log("El lugador no ha hecho progreso en ningún logro todavía.");
            return;
        }

        Logro logroAModificar;
        int progreso;

        foreach(int IDLogro in IDsProgresoJugador)
        {
            logroAModificar = ObtenerLogroPorID(IDLogro);
            progreso = System.Convert.ToInt32(databaseLogrosJugador.ObtenerPrimerValorSegunColumna(IDLogro, "IDAchievement", "Progreso"));

            logroAModificar.CambiarProgreso(progreso);

            Debug.Log("Cambiado progreso a " + progreso + " de " + logroAModificar.Nombre);
        }

    }

    private Logro ObtenerLogroPorID(int IDLogro)
    {
        foreach(Logro logro in logros)
        {
            if(logro.ID == IDLogro)
            {
                return logro;
            }
        }

        return null;
    }

    private void CrearPanelesLogros()
    {
        Debug.Log("Creando paneles de Logros.");

        GameObject nuevoPanel;
        PanelLogro scriptNuevoPanel;

        foreach(Logro logro in logros)
        {
            nuevoPanel = Instantiate(prefabPanelLogro, contenedorLogros.transform);
            scriptNuevoPanel = nuevoPanel.GetComponent<PanelLogro>();

            scriptNuevoPanel.Actualizar(logro.Nombre, logro.Descripcion, logro.Progreso / (float)logro.Meta, ObtenerSpriteLogroSegunID(logro.ID));
        }
    }

    private Sprite ObtenerSpriteLogroSegunID(int ID)
    {
        return Resources.Load<Sprite>($"Placeholders/IconosLogros/{ID}");
    }

}
