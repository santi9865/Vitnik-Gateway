using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiendaPresenter : MonoBehaviour
{
    private List<ItemTienda> itemsTienda = new List<ItemTienda>();
    private ConexionDatabase conexionDatabase;
    [SerializeField] private List<string> nombresColumnas = new List<string>();

    void Start()
    {
        conexionDatabase = gameObject.GetComponent<ConexionDatabase>();
    }

    private void ObtenerItemsTienda()
    {
        List<int> IDs = new List<int>();

        List<string> lecturaDatabase = conexionDatabase.ObtenerValoresColumna("ID");

        foreach(string entrada in lecturaDatabase)
        {
            IDs.Add(System.Convert.ToInt32(entrada));
        }

        foreach(int ID in IDs)
        {
            ItemTienda item;

            List<string> lecturaItem = conexionDatabase.ObtenerValoresFila(ID, nombresColumnas);

            item = new ItemTienda
            (
                System.Convert.ToInt32(lecturaItem[0]),
                lecturaItem[1],
                System.Convert.ToInt32(lecturaItem[2]),
                System.Convert.ToInt32(lecturaItem[3]),
                lecturaItem[4],
                lecturaItem[5],
                System.Convert.ToInt32(lecturaItem[6])            
            );

            itemsTienda.Add(item);
        }
    }
}
