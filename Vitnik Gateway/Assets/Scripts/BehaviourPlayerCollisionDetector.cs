using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourPlayerCollisionDetector : MonoBehaviour
{
    [SerializeField] private BehaviourMovimientoJugador scriptMovimientoJugador;
    private BoxCollider boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        scriptMovimientoJugador = gameObject.GetComponentInParent<BehaviourMovimientoJugador>();
        boxCollider = gameObject.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ActualizarCollider(Vector3 centro, Vector3 tamaño)
    {
        boxCollider.center = centro;
        boxCollider.size = tamaño;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.tag)
        {
            case "ObstaculoSolido":
                scriptMovimientoJugador.ColisionObstaculo(ObstacleType.Solid);
                break;
            case "Moneda":
                scriptMovimientoJugador.ColisionObstaculo(ObstacleType.Moneda);
                other.gameObject.GetComponent<BehaviourMoneda>().Agarrar();
                break;
            case "ColliderInicioPista":
                scriptMovimientoJugador.NuevaSeccionPista(other.gameObject.transform.parent.gameObject);
                break;
            case "ColliderRama":
                scriptMovimientoJugador.RamaDisponible(other.gameObject.GetComponent<BehaviourColliderRama>().RamaAsociada);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch(other.gameObject.tag)
        {
            case "ColliderRama":
                scriptMovimientoJugador.InhabilitarDoblar();
                break;
        }
    }


}

public enum ObstacleType
{
    Solid, Moneda
}