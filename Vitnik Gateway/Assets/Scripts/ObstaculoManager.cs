using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaculoManager : MonoBehaviour
{
    [SerializeField] private float alturaPrimerPiso;
    [SerializeField] private float alturaSegundoPiso;
    //Distancia media entre los obstáculos.
    [SerializeField] private float distancia;
    //Margen sobre el cual puede variar la disntancia media entre obstáculos.
    [SerializeField] private float varianzaPosicion;
    //Unidad sobre la cuál se basa la distancia entre los obstáculos. Todos los obstáculos están separados
    //en múltiplos de esta unidad.
    [SerializeField] private float unidad;
    //Distancia a partir de la cuál se toman en cuenta los carriles de la pista siguiente para el posicionamiento de los obstáculos.
    [SerializeField] private float distanciaPistaSiguiente;
    //Distancia a partir de la cuál se toman en cuenta los carriles de la pista anterior para el posicionamiento de los obstáculos.
    [SerializeField] private float distanciaPistaAnterior;

    //Distancia mínima agregada a la distancia más grande entre salto o deslizamiento para dar al jugador tiempo para ejecutar una acción.
    [SerializeField] private float margenDeManiobra;

    [SerializeField] private float probabilidadSpawnMultiple;
    [SerializeField] private Vector3 posicionUltimoGrupo;

    [SerializeField] private GameObject prefabObstaculo;
    [SerializeField] private int tamañoPool;
    [SerializeField] private GameObject contenedorObstaculos;

    [SerializeField] private BehaviourMovimientoJugador scriptJugador;

    private List<GameObject> poolObstaculos;


    void Start()
    {
        poolObstaculos = new List<GameObject>();

        for(int i = 0; i < tamañoPool; i++)
        {
            AgregarObstaculoAlPool("caja " + i);            
        }
    }

    private GameObject AgregarObstaculoAlPool(string nombre)
    {
        GameObject obstaculo = Instantiate(prefabObstaculo, Vector3.zero, prefabObstaculo.transform.rotation);
        obstaculo.SetActive(false);
        obstaculo.transform.SetParent(contenedorObstaculos.transform);
        poolObstaculos.Add(obstaculo);

        obstaculo.name = nombre;

        //Debug.Log("Obstaculo creado.");

        return obstaculo;
    }

    private GameObject ObtenerObstaculoDesactivado()
    {
        foreach(GameObject obstaculo in poolObstaculos)
        {
            if(!obstaculo.activeSelf)
            {
                return obstaculo;
            }
        }

        return null;
    }

    private GameObject PosicionarObstaculo(GameObject obstaculo, Vector3 posicion, Quaternion rotacion)
    {
        obstaculo.transform.position = posicion;
        obstaculo.transform.rotation = rotacion;
        obstaculo.SetActive(true);
        return obstaculo;
    }

    // Devuelve el múltiplo de la unidad más grande que es menor o igual al valor.
    private float PisoEnUnidad(float valor)
    {
        return valor - (valor % unidad);
    }

    // Devuelve el múltiplo de la unidad más pequeño que es mayor o igual al valor.
    private float TechoEnUnidad(float valor)
    {
        return (Mathf.Floor(valor / unidad) + 1) * unidad;
    }


    // Verifica que la distancia a partir de la cual se toman los carriles de otras pistas sea mayor a la
    // distancia que el jugador necesita para hacer la acción más lenta más el margen de maniobra extra, esto último para
    // darle un tiempo de reacción. Esto es porque la pista en sí puede ser un obstáculo cuando, por ejemplo, está rota.
    // Por lo tanto, el obstáculo debe proveer una forma de superarlo que permita al jugador evitar a la pista como obstáculo.
    // El jugador podrá ver que la pista está rota en un carril antes de que sea demasiado tarde.
    // Tambien convierte las distancias a múltiplos de la unidad.
    private void VerificarDistancias()
    {
        float distanciaMaxima = Mathf.Max(scriptJugador.DistanciaDesliz, scriptJugador.DistanciaSalto, scriptJugador.DistanciaCambioCarril);

        distanciaPistaSiguiente = TechoEnUnidad(Mathf.Max(distanciaMaxima + margenDeManiobra, distanciaPistaSiguiente));

        distanciaPistaAnterior = TechoEnUnidad(Mathf.Max(distanciaMaxima + margenDeManiobra, distanciaPistaAnterior));

        varianzaPosicion = TechoEnUnidad(varianzaPosicion);

        //La distancia entre obstáculos debe ser capaz de permitirle al jugador hacer como mínimo la acción más larga una vez
        // y cambiar de carril dos veces. Cada una de estas acciones debe también permitir al jugador tiempo para pensar.
        // if(distancia - varianzaPosicion < distanciaMaxima + scriptJugador.DistanciaCambioCarril * 2 + margenDeManiobra * 3)
        // {
        //     Debug.Log("Distancia mínima entre obstáculos muy pequeña; adaptando distancia mínima.");
        //     distancia = distanciaMaxima + scriptJugador.DistanciaCambioCarril * 2 + margenDeManiobra * 3 + varianzaPosicion;
        // }

        distancia = TechoEnUnidad(distancia);
    }

    // Devuelve una lista cuyos carriles están habilitados solamente si ese carril está habilitado en ambas listas.
    // La posición de los carriles se toma de la lista1.
    private List<Carril> TamizarCarriles(List<Carril> lista1, List<Carril> lista2)
    {
        if(lista1.Count != lista2.Count)
        {
            Debug.Log("Las listas de carriles tamizadas tienen una cantidad de carriles diferentes.");
        }

        List<Carril> resultados = new List<Carril>();

        Carril scriptCarrilLista1;
        Carril scriptCarrilLista2;

        for(int i = 0; i < lista1.Count; i++)
        {
            scriptCarrilLista1 = lista1[i].GetComponent<Carril>();
            for(int j = 0; j < lista2.Count; j++)
            {
                scriptCarrilLista2 = lista2[j].GetComponent<Carril>();
                if(scriptCarrilLista1.Tipo == scriptCarrilLista2.Tipo)
                {

                    Carril nuevoCarril;

                    if(!scriptCarrilLista1.Habilitado)
                    {
                        nuevoCarril = scriptCarrilLista1;
                    }
                    else
                    {
                        nuevoCarril = scriptCarrilLista2;
                    }
                    resultados.Add(nuevoCarril);
                }
            }
        }

        return resultados;
    }

    public void SpawnearObstaculos(GameObject pistaAnterior, GameObject pista, GameObject pistaSiguiente)
    {
        VerificarDistancias();

        BehaviourPista scriptPistaAnterior = pistaAnterior.GetComponentInChildren<BehaviourPista>();
        BehaviourPista scriptPista = pista.GetComponentInChildren<BehaviourPista>();
        BehaviourPista scriptSiguientePista = pistaSiguiente.GetComponentInChildren<BehaviourPista>();

        List<GrupoObstaculos> gruposObstaculos = new List<GrupoObstaculos>();

        float numeroRandom;

        float margenAleatorio = PisoEnUnidad(Random.Range(-varianzaPosicion, varianzaPosicion));

        while(Vector3.Dot(pista.transform.position + (scriptPista.Longitud / 2) * scriptPista.EjeMovimiento.Vectorizado - 
        posicionUltimoGrupo, scriptPista.EjeMovimiento.Vectorizado) > distancia + margenAleatorio)
        {
            Vector3 posicionNuevoGrupo = posicionUltimoGrupo + scriptJugador.EjeMovimiento.Vectorizado * (distancia + margenAleatorio);
            GrupoObstaculos nuevoGrupo = new GrupoObstaculos(posicionNuevoGrupo, new List<LugarObstaculo>(), new List<Carril>());

            //Distancia del grupo al comienzo de la pista siguiente.

            float distanciaGrupoPistaSiguiente = Vector3.Dot(pistaSiguiente.transform.position - posicionNuevoGrupo, scriptJugador.EjeMovimiento.Vectorizado)
            - scriptSiguientePista.Longitud / 2;

            float distanciaGrupoPistaAnterior = Vector3.Dot(posicionNuevoGrupo - pistaAnterior.transform.position, scriptJugador.EjeMovimiento.Vectorizado)
            - scriptPistaAnterior.Longitud / 2;

            // float distanciaGrupoPistaSiguiente = Mathf.Abs(-Vector3.Dot(posicionNuevoGrupo, scriptJugador.EjeMovimiento.Vectorizado) 
            // + Vector3.Dot(pistaSiguiente.transform.position, scriptJugador.EjeMovimiento.Vectorizado)) - scriptSiguientePista.Longitud / 2;
            //Distancia del grupo al final de la pista anterior
            // float distanciaGrupoPistaAnterior = Mathf.Abs(Vector3.Dot(posicionNuevoGrupo, scriptJugador.EjeMovimiento.Vectorizado) 
            // - Vector3.Dot(pistaAnterior.transform.position, scriptJugador.EjeMovimiento.Vectorizado)) - scriptPistaAnterior.Longitud / 2;

            List<Carril> carrilesPista = pista.GetComponentInChildren<BehaviourListaCarriles>().Carriles;

            if(distanciaGrupoPistaSiguiente < distanciaPistaSiguiente)
            {
                nuevoGrupo.Carriles = pistaSiguiente.GetComponentInChildren<BehaviourListaCarriles>().Carriles;
            }
            else if(distanciaGrupoPistaAnterior < distanciaPistaAnterior)
            {
                nuevoGrupo.Carriles = pistaAnterior.GetComponentInChildren<BehaviourListaCarriles>().Carriles;
            }
            else
            {
                nuevoGrupo.Carriles = carrilesPista;
            }

            for(int i = 0; i < nuevoGrupo.Carriles.Count * 2; i++)
            {
                LugarObstaculo nuevoLugar = new LugarObstaculo();

                nuevoLugar.Carril = i / 2;

                if( i % 2 == 1)
                {
                    nuevoLugar.Altura = Altura.Arriba;
                }
                else
                {
                    nuevoLugar.Altura = Altura.Abajo;
                }

                nuevoGrupo.Lugares.Add(nuevoLugar);
            }

            LugarObstaculo lugarLibreAleatorio = nuevoGrupo.DevolverLugarLibreAleatorio();

            if(ObtenerObstaculoDesactivado() == null)
            {
                lugarLibreAleatorio.Obstaculo = AgregarObstaculoAlPool("");
            }
            else
            {
                lugarLibreAleatorio.Obstaculo = ObtenerObstaculoDesactivado();
            }

            lugarLibreAleatorio.Obstaculo.SetActive(true);
            
            for(int i = 0; i < nuevoGrupo.Lugares.Count - 1; i++)
            {
                numeroRandom = Random.Range(0f,1f);

                if(numeroRandom < probabilidadSpawnMultiple)
                {
                    lugarLibreAleatorio = nuevoGrupo.DevolverLugarLibreAleatorio();

                    if(ObtenerObstaculoDesactivado() == null)
                    {
                        lugarLibreAleatorio.Obstaculo = AgregarObstaculoAlPool("");
                    }
                    else
                    {
                        lugarLibreAleatorio.Obstaculo = ObtenerObstaculoDesactivado();
                    }

                    lugarLibreAleatorio.Obstaculo.SetActive(true);
                }
            }

            foreach(LugarObstaculo lugar in nuevoGrupo.Lugares)
            {
                if(!lugar.Libre)
                {
                    float posicionVertical;

                    if(lugar.Altura == Altura.Arriba)
                    {
                        posicionVertical = alturaSegundoPiso;
                    }
                    else
                    {
                        posicionVertical = alturaPrimerPiso;
                    }

                    PosicionarObstaculo(lugar.Obstaculo, Vector3.Scale(nuevoGrupo.Carriles[lugar.Carril].transform.position, scriptPista.EjeMovimiento.VectorAxisPerpendicular) + posicionVertical * Vector3.up + Vector3.Scale(nuevoGrupo.Posicion, scriptPista.EjeMovimiento.VectorAxisParalelo), lugar.Obstaculo.transform.rotation);
                }
            }

            posicionUltimoGrupo = nuevoGrupo.Posicion;

            gruposObstaculos.Add(nuevoGrupo);

            margenAleatorio = PisoEnUnidad(Random.Range(-varianzaPosicion,varianzaPosicion));

            //Debug.Log(distancia + margenAleatorio);
        }

        scriptPista.gruposObstaculos = gruposObstaculos;
    }
}
