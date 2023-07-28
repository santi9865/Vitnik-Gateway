using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rama : MonoBehaviour
{
    public List<GameObject> Pistas {get; set;} = new List<GameObject>();
    public Eje EjeMovimiento {get; private set;}
    [SerializeField] private TipoRama tipoRama;

    public TipoRama TipoRama{get {return tipoRama;}}

    [SerializeField] private GameObject _pistaPadre;

    public GameObject PistaPadre {get => _pistaPadre;}

    public void AlinearSegunPadre(Eje ejeMovimientoPadre)
    {
        EjeMovimiento = new Eje(ejeMovimientoPadre.Direccion, ejeMovimientoPadre.Sentido);

        switch(tipoRama)
        {
            case TipoRama.Izquierda:
                EjeMovimiento.GirarIzquierda();
                break;
            case TipoRama.Derecha:
                EjeMovimiento.GirarDerecha();
                break;
        }
    }

    public void LimpiarPistas()
    {
        if(Pistas == null)
        {
            return;
        }

        foreach(GameObject pista in Pistas)
        {
            pista.SetActive(false);
            BehaviourPista scriptPista = pista.GetComponent<BehaviourPista>();
            scriptPista.DesactivarMonedasAsociadas();
            scriptPista.DesactivarObstaculosAsociados();
            scriptPista.DesactivarPistasAsociadas();
            scriptPista.LimpiarRamas();
            scriptPista.ReiniciarEje();
        }

        Pistas.Clear();
    }

    public void JugadorEntroColliderRama()
    {
        
    }
}

public enum TipoRama
{
    Izquierda, Derecha
}
