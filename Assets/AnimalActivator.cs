using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalActivator : MonoBehaviour
{
    public GameObject[] animals;
    // Start is called before the first frame update
    void Start()
    {
        animals[GameManager.Instance.CurrentLevel - 1].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
