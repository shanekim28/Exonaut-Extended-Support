using UnityEngine;

public class LevelAttributes : MonoBehaviour
{
	public Rect bounds = default(Rect);

	public float fallOutBuffer = 5f;

	public float colliderThickness = 10f;

	private Color sceneViewDisplayColor = new Color(0.2f, 0.74f, 0.27f, 0.5f);

	public GameObject[] pickups = new GameObject[4];

	private static LevelAttributes instance;

	public static LevelAttributes GetInstance()
	{
		if (!instance)
		{
			instance = (Object.FindObjectOfType(typeof(LevelAttributes)) as LevelAttributes);
			if (!instance)
			{
				Logger.traceError("There needs to be one active LevelAttributes script on a GameObject in your scene.");
			}
		}
		return instance;
	}

	private void OnDisable()
	{
		if (base.enabled)
		{
			instance = null;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = sceneViewDisplayColor;
		Vector3 vector = new Vector3(bounds.xMin, bounds.yMax, 0f);
		Vector3 vector2 = new Vector3(bounds.xMin, bounds.yMin, 0f);
		Vector3 vector3 = new Vector3(bounds.xMax, bounds.yMax, 0f);
		Vector3 vector4 = new Vector3(bounds.xMax, bounds.yMin, 0f);
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector2, vector4);
		Gizmos.DrawLine(vector4, vector3);
		Gizmos.DrawLine(vector3, vector);
	}

	private void Awake()
	{
	}

	private void Start()
	{
	}

	public void SetBoundaries()
	{
		GameObject gameObject = new GameObject("Created Boundaries");
		gameObject.transform.parent = base.transform;
		GameObject gameObject2 = new GameObject("Left Boundary");
		gameObject2.transform.parent = gameObject.transform;
		BoxCollider boxCollider = gameObject2.AddComponent<BoxCollider>();
		boxCollider.size = new Vector3(colliderThickness, bounds.height + colliderThickness * 2f + fallOutBuffer, colliderThickness);
		boxCollider.center = new Vector3(bounds.xMin - colliderThickness * 0.5f, bounds.y + bounds.height * 0.5f - fallOutBuffer * 0.5f, 0f);
		GameObject gameObject3 = new GameObject("Right Boundary");
		gameObject3.transform.parent = gameObject.transform;
		boxCollider = (gameObject3.AddComponent<BoxCollider>());
		boxCollider.size = new Vector3(colliderThickness, bounds.height + colliderThickness * 2f + fallOutBuffer, colliderThickness);
		boxCollider.center = new Vector3(bounds.xMax + colliderThickness * 0.5f, bounds.y + bounds.height * 0.5f - fallOutBuffer * 0.5f, 0f);
		GameObject gameObject4 = new GameObject("Top Boundary");
		gameObject4.transform.parent = gameObject.transform;
		boxCollider = (gameObject4.AddComponent<BoxCollider>());
		boxCollider.size = new Vector3(bounds.width + colliderThickness * 2f, colliderThickness, colliderThickness);
		boxCollider.center = new Vector3(bounds.x + bounds.width * 0.5f, bounds.yMax + colliderThickness * 0.5f, 0f);
		GameObject gameObject5 = new GameObject("Bottom Boundary (Including Fallout Buffer)");
		gameObject5.transform.parent = gameObject.transform;
		boxCollider = (gameObject5.AddComponent<BoxCollider>());
		boxCollider.size = new Vector3(bounds.width + colliderThickness * 2f, colliderThickness, colliderThickness);
		boxCollider.center = new Vector3(bounds.x + bounds.width * 0.5f, bounds.yMin - colliderThickness * 0.5f - fallOutBuffer, 0f);
	}
}
