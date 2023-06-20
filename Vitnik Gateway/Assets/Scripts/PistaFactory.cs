using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistaFactory : MonoBehaviour
{
    [SerializeField] private GameObject pistaRecta;
    [SerializeField] private GameObject pistaInterIzquierda;
    [SerializeField] private GameObject pistaInterDerecha;
    [SerializeField] private GameObject pistaInterT;
    [SerializeField] private GameObject pistaInterCruz;
    [SerializeField] private GameObject pistaRotaDerecha;
    [SerializeField] private GameObject pistaRotaIzquierda;
    [SerializeField] private GameObject pistaRecupDerecha;
    [SerializeField] private GameObject pistaRecupIzquierda;

    [SerializeField] private GameObject contenedorPistas;

    public GameObject CrearPista(TipoPista tipo = TipoPista.Recta)
    {
        GameObject pista;

        switch(tipo)
        {
            case TipoPista.Recta:
                pista = pistaRecta;
                break;
            case TipoPista.InterIzquierda:
                pista = pistaInterIzquierda;
                break;
            case TipoPista.InterDerecha:
                pista = pistaInterDerecha;
                break;
            case TipoPista.InterT:
                pista = pistaInterT;
                break;
            case TipoPista.InterCruz:
                pista = pistaInterCruz;
                break;
            case TipoPista.RectaRotaDerecha:
                pista = pistaRotaDerecha;
                break;
            case TipoPista.RectaRotaIzquierda:
                pista = pistaRotaIzquierda;
                break;
            case TipoPista.RectaRecupDerecha:
                pista = pistaRecupDerecha;
                break;
            case TipoPista.RectaRecupIzquierda:
                pista = pistaRecupIzquierda;
                break;
            default:
                pista = pistaRecta;
                break;
        }

        return Instantiate(pista,contenedorPistas.transform);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
