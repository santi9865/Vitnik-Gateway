using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LogrosManager : MonoBehaviour
{
    [SerializeField] private ConexionDatabase databaseLogros;
    [SerializeField] private ConexionDatabase databaseStatsJugador;

    [SerializeField] private LogrosPresenter presenter;

    private List<Logro> logros;

    void Start()
    {
        CargarLogros();
        
        if(presenter != null)
        {
            presenter.PresentarLogros(logros);
        }
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
                System.Convert.ToInt32(lecturaLogro[5]),
                System.Convert.ToInt32(lecturaLogro[6]),
                (string)lecturaLogro[7],
                System.Convert.ToInt32(lecturaLogro[8]),
                this
            );

            logros.Add(nuevoLogro);
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

    public void NotificarLogro(int id, int valor)
    {
        foreach(Logro logro in logros)
        {
            if(logro.ID == id)
            {
                if(logro.CambiarProgreso(valor))
                {
                    databaseLogros.ModificarValor("Progreso", logro.Progreso, "ID", logro.ID);
                }
            }
        }
    }

    public void ImpartirRecompensa(string tipoRecompensa, int valorRecompensa)
    {
        switch(tipoRecompensa)
        {
            case "Monedas":
                AgregarMonedasAJugador(valorRecompensa);
                break;
            case "Item":
                AgregarItemAJugador(valorRecompensa);
                break;
        }
    }

    private void AgregarItemAJugador(int item)
    {
        string itemsAdquiridosJugador = databaseStatsJugador.ObtenerPrimerValor("IDItemsAdquiridos").ToString();

        itemsAdquiridosJugador += item.ToString() + ",";

        databaseStatsJugador.ModificarValor("IDItemsAdquiridos", itemsAdquiridosJugador, "ID", 0);
    }

    private void AgregarMonedasAJugador(int monedas)
    {
        int monedasJugador = System.Convert.ToInt32(databaseStatsJugador.ObtenerPrimerValor("Monedas"));

        monedasJugador += monedas;

        databaseStatsJugador.ModificarValor("Monedas", monedasJugador, "ID", 0);
    }
}
