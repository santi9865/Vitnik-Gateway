using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourListaCarriles : MonoBehaviour
{
    [SerializeField] private List<GameObject> carriles;

    public List<GameObject> Carriles 
    {
        get
        {
            return carriles;
        }
    }
}
