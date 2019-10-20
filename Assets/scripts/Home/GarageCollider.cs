using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageCollider : MonoBehaviour
{

    //Slow car on enter
    private Dot_Truck_Controller dotTruckController;

    // Start is called before the first frame update
    void Start()
    {
        dotTruckController = GameObject.FindWithTag("Player").GetComponent<Dot_Truck_Controller>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnTriggerEnter(Collider other) {

        //playButton.interactable = false;
        dotTruckController.slowCar(true);
        if(other.gameObject.tag == "Player") {
            
        }

    }

}
