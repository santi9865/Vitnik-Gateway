using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourAchievementScroll : MonoBehaviour
{
    [SerializeField] private GameObject scrollContent;
    [SerializeField] private GameObject scrollItemPrefab;
    [SerializeField] private List<GameObject> scrollItems = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        AddItem();
        AddItem();
        AddItem();
        
        foreach(GameObject item in scrollItems)
        {
            item.transform.parent = scrollContent.transform; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AddItem()
    {
        scrollItems.Add(Instantiate(scrollItemPrefab));

    }
}
