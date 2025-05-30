using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorActivate : MonoBehaviour
{
	public GameObject Door;
	public float delay;

    // Start is called before the first frame update

    public Material activeMaterial; 
    public Material defaultMaterial;   

    private Renderer sensorRenderer;
    private bool isObjectInTrigger;
    private float timer;

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

		Debug.Log("Collision Occured");

        
    }
	void OnTriggerStay(Collider other)
	{
        Debug.Log("interacting wit sensor");
        Door.SetActive(true);

        sensorRenderer.material = activeMaterial;
        isObjectInTrigger = true;
        timer = 0f; 
    }
    void Update()
    {
        if (!isObjectInTrigger)
        {
            timer += Time.deltaTime;

            if (timer >= delay)
            {
                Door.SetActive(false);
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
