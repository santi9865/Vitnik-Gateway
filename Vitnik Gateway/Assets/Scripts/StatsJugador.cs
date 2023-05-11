using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsJugador : MonoBehaviour
{

    private ConexionDatabase conexionDatabase;

    // Start is called before the first frame update
    void Start()
    {
        conexionDatabase = gameObject.GetComponent<ConexionDatabase>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AgregarMonedas(int monedasNuevas)
    {
        int monedasPrevias = System.Convert.ToInt32(conexionDatabase.ObtenerPrimerValor("Monedas"));

        monedasPrevias += monedasNuevas;

        conexionDatabase.ModificarValor("Monedas", monedasPrevias, "ID", 0);

        Debug.Log($"Se han agregado {monedasNuevas} monedas.");
    }
}
