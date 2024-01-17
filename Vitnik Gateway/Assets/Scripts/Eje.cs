using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eje
{
    public EjeDireccion Direccion {get; private set;}
    public EjeSentido Sentido {get; private set;}
    public Vector3 Vectorizado {get; private set;}

    public static Eje EjeZPositivo {get => new Eje(EjeDireccion.Z, EjeSentido.Positivo);}

    public Vector3 VectorAxisPerpendicular 
    {
        get 
        {
            if(Direccion == EjeDireccion.X)
            {
                return Vector3.forward;       
            }
            else
            {
                return Vector3.right;

            }
        }
    }
    public Vector3 VectorAxisParalelo
    {
        get 
        {
            if(Direccion == EjeDireccion.X)
            {
                return Vector3.right;     
            }
            else
            {
                return Vector3.forward; 
            }
        }
    }

    public Eje(EjeDireccion direccion, EjeSentido sentido)
    {
        Direccion = direccion;
        Sentido = sentido;
        Vectorizar();
    }

    private void Vectorizar()
    {
        Vector3 vector = Vector3.zero;

        switch(Direccion)
        {
            case EjeDireccion.X:
                vector.x = 1;
                break;
            case EjeDireccion.Z:
                vector.z = 1;
                break;
        }

        int signo = 0;

        switch(Sentido)
        {
            case EjeSentido.Positivo:
                signo = 1;
                break;
            case EjeSentido.Negativo:
                signo = -1;
                break;
        }

        vector = new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z)) * signo;

        Vectorizado = vector;
    }

    public void GirarIzquierda() 
    //Cambiar de dirección y sentido como si se viera desde 
    //Y positivo hacia Y negativo, es decir, desde arriba.
    {
        switch(Direccion)
        {
            case EjeDireccion.X:
                Direccion = EjeDireccion.Z;
                break;
            case EjeDireccion.Z:
                Direccion = EjeDireccion.X;
                InvertirSentido();
                break;
        }

        Vectorizar();
    }

    public void GirarDerecha() 
    //Cambiar de dirección y sentido como si se viera desde 
    //Y positivo hacia Y negativo, es decir, desde arriba.
    {
        switch(Direccion)
        {
            case EjeDireccion.X:
                Direccion = EjeDireccion.Z;
                InvertirSentido();
                break;
            case EjeDireccion.Z:
                Direccion = EjeDireccion.X;
                break;
        }

        Vectorizar();
    }

    private void InvertirSentido()
    {
        switch(Sentido)
        {
            case EjeSentido.Positivo:
                Sentido = EjeSentido.Negativo;
                break;
            case EjeSentido.Negativo:
                Sentido = EjeSentido.Positivo;
                break;
        }
    }

    public float AngulosA(Eje nuevoEje) 
    //Rotacion calculada según el eje Y visto desde arriba hacia abajo.
    {
        if(nuevoEje.Direccion == Direccion)
        {
            if(nuevoEje.Sentido != Sentido)
            {
                //Dar la vuelta.
                return 180;
            }
        }
        else
        {
            switch(nuevoEje.Direccion)
            {
                case EjeDireccion.X: //La dirección de ESTE eje es Z
                    if(nuevoEje.Sentido == Sentido)
                    {
                        //90 grados a la derecha.
                        return 90;
                    }
                    else
                    {
                        //90 grados a la izquierda.
                        return -90;
                    }
                case EjeDireccion.Z: //La dirección de ESTE eje es X
                    if(nuevoEje.Sentido == Sentido)
                    {
                        //90 grados a la izquierda.
                        return -90;
                    }
                    else
                    {
                        //90 grados a la derecha.
                        return 90;
                    }
            }
        }

        //  La dirección y sentido son iguales.
        return 0;
    }
}

public enum EjeDireccion
{
    X,Z
}

public enum EjeSentido
{
    Positivo, Negativo
}
