using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourColliderInicioPista : MonoBehaviour
{
    [SerializeField] private BehaviourPista behaviourPistaPadre;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            behaviourPistaPadre.JugadorEntroColliderInicio();
        }
    }
}
