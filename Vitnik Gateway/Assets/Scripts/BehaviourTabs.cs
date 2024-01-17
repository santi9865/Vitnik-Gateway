using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BehaviourTabs : MonoBehaviour
{
    [SerializeField] private List<GameObject> tabs;

    public void EnableThisTab(GameObject thisTab)
    {
        DisableTabsExcept(thisTab);

        thisTab.SetActive(true);
    }

    private void DisableTabsExcept(GameObject exception)
    {
        foreach(GameObject toBeDisabled in tabs)
        {
            if(exception != toBeDisabled)
            {
                toBeDisabled.SetActive(false);
            }
        }
    }
}
