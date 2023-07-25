using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LugarObstaculo
{
    public int Carril {get;set;}
    public Altura Altura {get;set;}
    public bool Libre {get;set;}

    private GameObject _obstaculo;
    private Quaternion rotacionInicialObstaculo;
    public GameObject Obstaculo 
    {
        get => _obstaculo;
    
        set
        {
            if(value == null)
            {
                Libre = true;
            }
            else
            {
                rotacionInicialObstaculo = value.transform.rotation;
                Libre = false;
            }
            _obstaculo = value;

        }
    }

    public LugarObstaculo()
    {
        Libre = true;
    }

    public LugarObstaculo(int carril, Altura altura, bool libre, GameObject obstaculo)
    {
        Carril = carril;
        Altura = altura;
        Libre = libre;
        Obstaculo = obstaculo;
    }

    public void DesactivarObstaculo()
    {
        if(!Libre && Obstaculo != null)
        {
            Obstaculo.SetActive(false);
        }
    }

    public void ReiniciarObstaculo()
    {
        if(Obstaculo != null)
        {
            Obstaculo.transform.rotation = rotacionInicialObstaculo;
        }
    }

    public void RotarObstaculo(float anguloY)
    {
        Obstaculo.transform.Rotate(0,anguloY,0);
    }
}


public enum Altura
{
    Arriba,
    Abajo
}
