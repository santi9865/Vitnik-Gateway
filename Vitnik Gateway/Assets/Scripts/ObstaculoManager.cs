using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaculoManager : MonoBehaviour
{
    //Posición en Y de los obstáculos que están "abajo"
    [SerializeField] private float alturaPrimerPiso;
    //Posición en Y de los obstáculos que están "arriba"
    [SerializeField] private float alturaSegundoPiso;
    //Distancia media entre los obstáculos.
    [SerializeField] private float distancia;
    //Margen sobre el cual puede variar la disntancia media entre obstáculos.
    [SerializeField] private float varianzaPosicion;
    //Unidad sobre la cuál se basa la distancia entre los obstáculos. 
    //Todos los obstáculos están separados en múltiplos de esta unidad.
    [SerializeField] private float unidad;
    //Distancia a partir de la cuál se toman en cuenta los carriles 
    //de la pista siguiente para el posicionamiento de los obstáculos.
    [SerializeField] private float distanciaPistaSiguiente;
    //Distancia a partir de la cuál se toman en cuenta los carriles 
    //de la pista anterior para el posicionamiento de los obstáculos.
    [SerializeField] private float distanciaPistaAnterior;
    //Distancia mínima agregada a la distancia más grande entre salto 
    //o deslizamiento para dar al jugador tiempo para ejecutar una acción.
    [SerializeField] private float margenDeManiobra;

    //Probabilidad de que se ponga otro obstáculo en el grupo luego
    //de que se ha puesto el último. O sea, pueden ser varios.
    [SerializeField] private float probabilidadSpawnMultiple;
    //Posición del último grupo de obstáculos colocado.
    [SerializeField] private Vector3 posicionUltimoGrupo;

    [SerializeField] private GameObject prefabObstaculo;
    [SerializeField] private int tamañoPool;
    [SerializeField] private GameObject contenedorObstaculos;
    private List<GameObject> poolObstaculos;

    [SerializeField] private BehaviourMovimientoJugador scriptJugador;

    private const float MARGENTECHOYPISO = 0.001F;

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
        if(valor % unidad < MARGENTECHOYPISO)
        {
            return valor;
        }
        else
        {
            return valor - (valor % unidad);
        }
    }

    // Devuelve el múltiplo de la unidad más pequeño que es mayor o igual al valor.
    private float TechoEnUnidad(float valor)
    {
        if(valor % unidad < MARGENTECHOYPISO)
        {
            return valor;
        }
        else
        {
            return valor + (unidad - valor % unidad);
        }
    }


    // Verifica que la distancia a partir de la cual se toman los carriles de otras pistas sea mayor a la
    // distancia que el jugador necesita para hacer la acción más lenta más el margen de maniobra extra, esto último para
    // darle un tiempo de reacción. Esto es porque la pista en sí puede ser un obstáculo cuando, por ejemplo, está rota.
    // Por lo tanto, el obstáculo debe proveer una forma de superarlo que permita al jugador evitar a la pista como obstáculo.
    // El jugador podrá ver que la pista está rota en un carril antes de que sea demasiado tarde.
    // Tambien convierte las distancias a múltiplos de la unidad.
    private void VerificarDistancias()
    {
        //Distancia más grande que puede recorrer el jugador si se desliza, salta o cambia de carril.
        float distanciaMaxima = Mathf.Max(scriptJugador.DistanciaDesliz, scriptJugador.DistanciaSalto, scriptJugador.DistanciaCambioCarril);

        //La distancia a la cual se debe considerar la pista siguiente debe ser mayor a la mitad de la distancia más grande que puede recorrer
        //el jugador con alguna acción más cambiar de carril una vez. A fines prácticos se le suma un valor que sirve como acolchado para dejar un tiempo prudente
        //de reacción.
        distanciaPistaSiguiente = TechoEnUnidad(Mathf.Max(distanciaMaxima / 2 + margenDeManiobra + scriptJugador.DistanciaCambioCarril, distanciaPistaSiguiente));

        //Lo mismo se hace con la pista Anterior.
        distanciaPistaAnterior = TechoEnUnidad(Mathf.Max(distanciaMaxima / 2 + margenDeManiobra + scriptJugador.DistanciaCambioCarril, distanciaPistaAnterior));

        //Se convierte a la varianza de la posición en un múltiplo de la unidad elegida para reflejar mejor los valores posibles
        //Como los grupos de obstáculos se ponen en múltiplos de la unidad elegida si la varianza no es un múltiplo entonces
        //el valor es engañoso porque nunca va a haber un obstáculo posicionado con la varianza máxima o mínima.
        varianzaPosicion = TechoEnUnidad(varianzaPosicion);

        //La distancia entre obstáculos debe ser capaz de permitirle al jugador hacer como mínimo la acción más larga una vez
        // y cambiar de carril dos veces. También se agrega un poco de tiempo para pensar.
        if(distancia < distanciaMaxima + scriptJugador.DistanciaCambioCarril * 2 + margenDeManiobra)
        {
            Debug.Log("Distancia mínima entre obstáculos muy pequeña; adaptando distancia mínima.");
            distancia = distanciaMaxima + scriptJugador.DistanciaCambioCarril * 2 + margenDeManiobra;
        }

        //Al igual que con la varianza, la distancia entre los grupos de obstáculos es un múltiplo de la unidad
        //Por lo tanto, la distancia entre dichos grupos debe ser un múltiplo para reflejar mejor los valores posibles.
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

        if(pista == null)
        {
            return;
        }

        BehaviourPista scriptPista = pista.GetComponentInChildren<BehaviourPista>();

        List<GrupoObstaculos> gruposObstaculos = new List<GrupoObstaculos>();

        float numeroRandom;

        float margenAleatorio = PisoEnUnidad(Random.Range(0, varianzaPosicion));

        float distanciaUltimoGrupoAFinalPista = Vector3.Dot(pista.transform.position + (scriptPista.Longitud / 2) * scriptPista.EjeMovimiento.Vectorizado - 
        posicionUltimoGrupo, scriptPista.EjeMovimiento.Vectorizado);

        //Este while calcula la distancia del último grupo de obstáculos spawneado con la posición del final de la pista considerada.

        while(distanciaUltimoGrupoAFinalPista > distancia + margenAleatorio)
        {
            //Checkea si el último grupo fue spawneado a una distancia mayor a distancia + margenAleatorio
            if(distanciaUltimoGrupoAFinalPista > scriptPista.Longitud + distancia + margenAleatorio)
            {
                posicionUltimoGrupo = pista.transform.position - (scriptPista.Longitud /2 + distancia + margenAleatorio) * scriptPista.EjeMovimiento.Vectorizado;
            }

            Vector3 posicionNuevoGrupo = posicionUltimoGrupo + scriptJugador.EjeMovimiento.Vectorizado * (distancia + margenAleatorio);
            
            //Se analiza si considerar los carriles de la pista siguiente o anterior para el posicionamiento de obstáculos
            //en carriles habilitados según la distancia del grupo a dichas pistas.

            List<Carril> carrilesAConsiderar = null;

            if(pistaSiguiente != null)
            {   
                BehaviourPista scriptSiguientePista = pistaSiguiente.GetComponentInChildren<BehaviourPista>();

                //Distancia del grupo de obstáculos al comienzo de la pista siguiente.

                float distanciaGrupoPistaSiguiente = Vector3.Dot(pistaSiguiente.transform.position - posicionNuevoGrupo, scriptJugador.EjeMovimiento.Vectorizado)
                - scriptSiguientePista.Longitud / 2;

                if(distanciaGrupoPistaSiguiente < distanciaPistaSiguiente)
                {
                    carrilesAConsiderar = pistaSiguiente.GetComponentInChildren<BehaviourListaCarriles>().Carriles;
                }
            }
            else if(pistaAnterior != null)
            {
                BehaviourPista scriptPistaAnterior = pistaAnterior.GetComponentInChildren<BehaviourPista>();

                //Distancia del grupo al final de la pista anterior

                float distanciaGrupoPistaAnterior = Vector3.Dot(posicionNuevoGrupo - pistaAnterior.transform.position, scriptJugador.EjeMovimiento.Vectorizado)
                - scriptPistaAnterior.Longitud / 2;

                if(distanciaGrupoPistaAnterior < distanciaPistaAnterior)
                {
                    carrilesAConsiderar = pistaAnterior.GetComponentInChildren<BehaviourListaCarriles>().Carriles;
                }
            }

            //Los obstáculos siempre se ponen relativos a la posicón de los carriles de la pista central ingresada como argumento.

            GrupoObstaculos nuevoGrupo = new GrupoObstaculos(posicionNuevoGrupo, 
            new List<LugarObstaculo>(), pista.GetComponentInChildren<BehaviourListaCarriles>().Carriles, carrilesAConsiderar);

            //Llena el grupo de obstáculos de lugares según la cantidad de carriles.

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

            LugarObstaculo lugarLibreAleatorio =  nuevoGrupo.DevolverLugarLibreAleatorioHabilitado(); 

            if(ObtenerObstaculoDesactivado() == null)
            {
                lugarLibreAleatorio.Obstaculo = AgregarObstaculoAlPool("");
            }
            else
            {
                lugarLibreAleatorio.Obstaculo = ObtenerObstaculoDesactivado();
            }

            lugarLibreAleatorio.Obstaculo.SetActive(true);
            
            for(int i = 0; i < nuevoGrupo.CantidadLugaresLibresHabilitados - 1; i++)
            {
                numeroRandom = Random.Range(0f,1f);

                if(numeroRandom < probabilidadSpawnMultiple)
                {
                    lugarLibreAleatorio = nuevoGrupo.DevolverLugarLibreAleatorioHabilitado();

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

                    PosicionarObstaculo(lugar.Obstaculo, Vector3.Scale(nuevoGrupo.Carriles[lugar.Carril].transform.position,
                     scriptPista.EjeMovimiento.VectorAxisPerpendicular) + posicionVertical * Vector3.up + 
                     Vector3.Scale(nuevoGrupo.Posicion, scriptPista.EjeMovimiento.VectorAxisParalelo), lugar.Obstaculo.transform.rotation);
                }
            }

            posicionUltimoGrupo = nuevoGrupo.Posicion;

            gruposObstaculos.Add(nuevoGrupo);

            margenAleatorio = PisoEnUnidad(Random.Range(-varianzaPosicion,varianzaPosicion));

            distanciaUltimoGrupoAFinalPista = Vector3.Dot(pista.transform.position +
             (scriptPista.Longitud / 2) * scriptPista.EjeMovimiento.Vectorizado - 
            posicionUltimoGrupo, scriptPista.EjeMovimiento.Vectorizado);
        }

        scriptPista.gruposObstaculos = gruposObstaculos;
    }
}
