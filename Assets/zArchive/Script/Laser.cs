using UnityEngine;
using UnityEngine.Events;

public class Laser : MonoBehaviour
{
	private bool isLaserActive = false;
	private int maxBounces = 7;
	public bool meshactive;
	private LineRenderer lr;

	[SerializeField]
	private Transform startPoint;
	[SerializeField]
	private bool reflectionOnlyMirror;

	public void ClearCollider()
	{
		Destroy(gameObject.GetComponent<MeshCollider>());
	}
	private void Start()
	{
		lr = GetComponent<LineRenderer>();
		lr.SetPosition(0, startPoint.position);
		lr.enabled = false;
		meshactive = false;

	}
	private void Update()
	{
		// Toggle laser on and off when pressing the space bar
		if (Input.GetKeyDown(KeyCode.I))
		{
			isLaserActive = !isLaserActive;
			if (meshactive)
			{
				meshactive = false;
				ClearCollider();
			}
			lr.enabled = isLaserActive;
		}
		
		// Only cast the laser if it's active
		if (isLaserActive)
		{
			CastLaser(startPoint.position, startPoint.right);
		}
	}

	private void CastLaser(Vector3 position, Vector3 direction)
	{
		lr.SetPosition(0, startPoint.position);

		for (int i = 0; i < maxBounces; i++)
		{

			Ray ray = new Ray(position, direction);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 1000, 1))
			{
				position = hit.point;
				direction = Vector3.Reflect(direction, hit.normal);
				lr.SetPosition(i + 1, hit.point);
				if(hit.transform.tag == "Sensor") hit.transform.gameObject.GetComponent<Sensor>().setDoor();
				if (hit.transform.tag != "Mirror" && reflectionOnlyMirror)
				{

					for (int j = (i + 1); j <= 7; j++)
					{
						lr.SetPosition(j, hit.point);
						if (hit.transform.tag == "Sensor") hit.transform.gameObject.GetComponent<Sensor>().setDoor();
					}
					break;
				}
				if (hit.transform.tag == "Sensor") hit.transform.gameObject.GetComponent<Sensor>().setDoor();
			}

		}
		if (!meshactive)
		{
			LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();

			MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();

			Mesh mesh = new Mesh();
			lineRenderer.BakeMesh(mesh, false);
			meshCollider.sharedMesh = mesh;
			meshactive = true;
			
		}

	}
}