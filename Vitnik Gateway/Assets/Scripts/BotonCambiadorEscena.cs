using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BotonCambiadorEscena : MonoBehaviour
{
    [SerializeField] private TipoEscena tipoEscena;
    private Button esteBoton;

    private void Awake()
    {
        esteBoton = GetComponent<Button>();
        esteBoton.onClick.AddListener(BotonClick);
    }

    private void BotonClick()
    {
        BehaviourSceneManager.IrAEscena(tipoEscena);
    }
}
