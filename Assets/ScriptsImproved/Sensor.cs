using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
	public GameObject Door;
	public float delay;

    // Start is called before the first frame update

    public Material activeMaterial; 
    public Material defaultMaterial;   

    private Renderer sensorRenderer;
    private bool isObjectInTrigger;
    private float timer;

    public void setDoor()
    {
        Debug.Log("This Happened");
		isObjectInTrigger = true;
        sensorRenderer.material = activeMaterial;
		Door.SetActive(false);
    }

    void Start()
    {
        sensorRenderer = GetComponent<Renderer>();
        isObjectInTrigger = false;

        // Ensure the sensor starts with its default material
        if (sensorRenderer != null && defaultMaterial != null)
        {
            sensorRenderer.material = defaultMaterial;
        }

    }
	void OnTriggerEnter(Collider other)
	{

        //if (other.CompareTag("ObjectStayInBG"))
        //{
        //    other.GetComponent<WindObstacle>().SetInBG();
        //}

        //else if (other.CompareTag("Pushable"))
        //{
        //    other.GetComponent<WindObstacle>().SetInBG();
        //}


        Debug.Log("Collision Occured");

        
    }
	void OnTriggerStay(Collider other)
	{
        Debug.Log("interacting wit sensor");
        Door.SetActive(false);

        sensorRenderer.material = activeMaterial;
        isObjectInTrigger = true;
        timer = 0f; 
    }
    //void OnTriggerExit(Collider other)
    //{
    //    StartCoroutine(ComeBackDelay());
    //}
    //// Update is called once per frame
    //private IEnumerator ComeBackDelay()
    //{
    //    yield return new WaitForSeconds(delay);
    //    Door.SetActive(true);
    //    sensorRenderer.material = defaultMaterial;
    //}

    void Update()
    {
        if (!isObjectInTrigger)
        {
            timer += Time.deltaTime;

            if (timer >= delay)
            {
                Door.SetActive(true);
                sensorRenderer.material = defaultMaterial;
            }
        }
        else //when object is in sensor (isObjectInTrigger)
        {
            timer = 0f;
        }

        isObjectInTrigger = false;
    }
}
