using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourListaCarriles : MonoBehaviour
{
    [SerializeField] private List<Transform> carriles;

    public List<Transform> Carriles 
    {
        get
        {
            return carriles;
        }
    }
}
