using UnityEngine;

public class Battery : MonoBehaviour
{
    public int batterytype; //0 = amp, 1 = ohm
    public int value;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int GetBatteryType()
    {
        return batterytype; 
    }
	public int GetValue()
	{
		return value;
	}
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
