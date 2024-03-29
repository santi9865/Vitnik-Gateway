using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourPista : MonoBehaviour
{
    [SerializeField] private PistaManager pistaManager;

    public PistaManager PistaManager {get => pistaManager; set => pistaManager = value;}
    [SerializeField] private float longitud;
    public float Longitud {get => longitud;}
    [SerializeField] private TipoPista tipo;
    public TipoPista Tipo {get => tipo;}

    [SerializeField] private GameObject ramaDerecha;
    public GameObject RamaDerecha {get => ramaDerecha;}

    [SerializeField] private GameObject ramaIzquierda;
    public GameObject RamaIzquierda {get => ramaIzquierda;}

    public Eje EjeMovimiento = new Eje(EjeDireccion.Z, EjeSentido.Positivo);
    public List<GrupoObstaculos> gruposObstaculos;
    public List<GameObject> monedas;

    //Estas son pistas que deben desaparecer junto con esta pista, pero que no están en una rama.
    public List<GameObject> pistasAsociadas;

    [SerializeField] private Quaternion rotacionOriginal;

    void Start()
    {
        
    }

    public void ReiniciarEje()
    {
        transform.rotation = rotacionOriginal;
        EjeMovimiento = new Eje(EjeDireccion.Z, EjeSentido.Positivo);
    }

    public void DesactivarObstaculosAsociados()
    {
        if(gruposObstaculos != null)
        {
            foreach(GrupoObstaculos grupo in gruposObstaculos)
            {
                grupo.DesactivarObstaculos();
            }

            gruposObstaculos.Clear();
            gruposObstaculos = null;
        }
    }

    public void DesactivarMonedasAsociadas()
    {
        if(monedas != null)
        {
            foreach(GameObject moneda in monedas)
            {
                moneda.SetActive(false);
                moneda.GetComponent<BehaviourMoneda>().ReiniciarMoneda();
            }

            monedas.Clear();
            monedas = null;
        }
    }

    public void DesactivarPistasAsociadas()
    {
        if(pistasAsociadas != null)
        {
            foreach(GameObject pista in pistasAsociadas)
            {
                BehaviourPista scriptPista = pista.GetComponent<BehaviourPista>();

                pista.SetActive(false);
                scriptPista.DesactivarObstaculosAsociados();
                scriptPista.DesactivarMonedasAsociadas();
                scriptPista.DesactivarPistasAsociadas();
                scriptPista.LimpiarRamas();
                scriptPista.ReiniciarEje();
            }
            
            pistasAsociadas.Clear();
        }

        pistasAsociadas = null;
    }

    public void JugadorEntroColliderInicio()
    {
        pistaManager.CircularPistas(this.gameObject);
    }

    public void RemoverMoneda(GameObject moneda)
    {
        if(monedas.Contains(moneda))
        {
            monedas.Remove(moneda);
        }
    } 

    public void LimpiarRamas()
    {
        if(ramaDerecha != null)
        {
            ramaDerecha.GetComponent<Rama>().LimpiarPistas();
        }

        if(ramaIzquierda != null)
        {
            ramaIzquierda.GetComponent<Rama>().LimpiarPistas();
        }
    }
}

public enum TipoPista
{
    Recta, 
    InterIzquierda, 
    InterDerecha, 
    InterT, 
    InterCruz, 
    RectaRotaDerecha, 
    RectaRotaIzquierda, 
    RectaRotaInicioDerecha, 
    RectaRotaInicioIzquierda,
    RectaRotaFinDerecha, 
    RectaRotaFinIzquierda,
    InterLIzquierda,
    InterLDerecha
}
