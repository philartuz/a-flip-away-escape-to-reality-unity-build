using UnityEngine;

public class ElecGate : MonoBehaviour
{
	public int requiredVolt;
	public int correctVolt;
	public int correctAmp;
	public int correctOhm;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		correctAmp = 0;
		correctOhm = 0;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public int getCurrentAmp()
	{
		return correctAmp;
	}
	public int getCurrentOhm()
	{
		return correctOhm;
	}
	public int getRequiredVolt()
	{
		return requiredVolt;
	}

	public int getCurrentVolt()
	{
		correctVolt = correctAmp * correctOhm;
		return correctVolt;
	}
	public void addAmp(int ampvalue)
	{
		correctAmp += ampvalue;
	}
	public void subtractAmp(int ampvalue)
	{
		correctAmp -= ampvalue;
	}
	public void addOhm(int ohmvalue)
	{
		correctOhm += ohmvalue;
	}
	public void subtractOhm(int ohmvalue)
	{
		correctOhm -= ohmvalue;
	}
	public void Deactivate()
	{
		gameObject.SetActive(false);
	}
}
