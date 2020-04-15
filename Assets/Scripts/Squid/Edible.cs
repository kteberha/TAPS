using UnityEngine;
using UnityEngine.Events;
using UModules;

public class Edible : MonoBehaviour
{
	public bool destroyOnEat = true;

	[Header("Components")]
	[ReorderableList]
	public Behaviour[] components;
	new public Rigidbody2D rigidbody;

	[Header("Events")]
	public UnityEvent grabbed = new UnityEvent();
	public UnityEvent released = new UnityEvent();
	public UnityEvent eaten = new UnityEvent();

	public void Grabbed()
	{
		grabbed.Invoke();
		foreach(Behaviour bc in components)
		{
			bc.enabled = false;
			if (rigidbody != null) rigidbody.simulated = false;
		}
	}

	public void Released()
	{
		released.Invoke();
		foreach(Behaviour bc in components)
		{
			bc.enabled = false;
			if (rigidbody != null) rigidbody.simulated = true;
		}
	}

	public void Eaten()
	{
		eaten.Invoke();
		if (destroyOnEat) Destroy(this.gameObject);
	}
}