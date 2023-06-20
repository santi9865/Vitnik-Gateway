using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourColliderRama : MonoBehaviour
{
    [SerializeField] private Rama _ramaAsociada;
    public Rama RamaAsociada {get => _ramaAsociada;}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            _ramaAsociada.JugadorEntroColliderRama();
        }
    }
}
