using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourPista : MonoBehaviour
{
    [SerializeField] private PistaManager pistaManager;
    [SerializeField] private float longitud;

    public List<GrupoObstaculos> gruposObstaculos;
    public List<GameObject> monedas;

    public float Longitud {get => longitud;}

    public void OrdenarGruposObstaculos()
    {
        for( int i = 0; i < gruposObstaculos.Count; i++)
        {
            for(int j = i; j < gruposObstaculos.Count; j++)
            {
                if(gruposObstaculos[i].Posicion > gruposObstaculos[j].Posicion)
                {
                    GrupoObstaculos memoria = gruposObstaculos[i];
                    gruposObstaculos[i] = gruposObstaculos[j];
                    gruposObstaculos[j] = memoria;
                }
            }
        }
    }

    public void DesactivarObstaculosAsociados()
    {
        if(gruposObstaculos != null)
        {
            foreach(GrupoObstaculos grupo in gruposObstaculos)
            {
                grupo.DesactivarObstaculos();
            }
        }
    }

    public void DesactivarMonedasAsociadas()
    {
        if(monedas != null)
        {
            foreach(GameObject moneda in monedas)
            {
                moneda.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            pistaManager.CircularPistas();
        }
    }
}
