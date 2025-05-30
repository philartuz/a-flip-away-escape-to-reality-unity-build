using UnityEngine;

public class BatterySlot : MonoBehaviour
{
	public bool activated;
	public GameObject ElecGate;
	public GameObject activator;
	public ElecGate elecgateRef;
    public int slottype; //0 = amp, 1 = ohm
	public int value;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
	void OnTriggerStay(Collider other)
	{
		if (!activated)
		{
			if (other.gameObject.tag == "Battery")
			{
				if(slottype == other.gameObject.GetComponent<Battery>().GetBatteryType())
				{
					value = other.gameObject.GetComponent<Battery>().GetValue();
					if (slottype == 0)
					{
						elecgateRef.addAmp(value);
					}
					else
					{
						elecgateRef.addOhm(value);
					}
					activator = other.gameObject;
					activated = true;
					if (elecgateRef.getCurrentVolt() == elecgateRef.getRequiredVolt())
					{
						elecgateRef.Deactivate();
					}
				}
			}
		}
			
	}
	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Battery")
		{
			if(activated)
			{
				if (GameObject.ReferenceEquals(other.gameObject, activator))
				{
					if (slottype == 0)
					{
						elecgateRef.subtractAmp(value);
						activated = false;
						activator = null;
					}
					else
					{
						elecgateRef.subtractOhm(value);
						activated = false;
						activator = null;
					}
				}
			}

		}
	}
	// Update is called once per frame
	void Update()
    {
        
    }
}
