using UnityEngine;
using UnityEngine.Events;
using UModules;

public class Collider2DEvent : UnityEvent<Collider2D> {}

[RequireComponent(typeof(Collider2D))]
public class TentacleHand : MonoBehaviour
{
	[Readonly]
	new public Collider2D collider;
	[Readonly]
	public Transform heldObject;

	public Collider2DEvent triggerEnter = new Collider2DEvent();
	
	public bool Holding => heldObject!=null;
	public void ToggleCollider() => collider.enabled = !collider.enabled;
	
	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		collider = GetComponent<Collider2D>();
	}

	public void HandAttach(Transform t)
	{
		if (t != null)
		{
			heldObject = t;
			heldObject.SetParent(this.transform);
			heldObject.SendMessage("Grabbed",SendMessageOptions.DontRequireReceiver);
		}
	}

	public void HandDetach()
	{
		if (heldObject != null)
		{
			heldObject.SendMessage("Released",SendMessageOptions.DontRequireReceiver);
			this.transform.DetachChildren();
			heldObject = null;
		}
	}

	/// <summary>
	/// Sent when another object enters a trigger collider attached to this
	/// object (2D physics only).
	/// </summary>
	/// <param name="other">The other Collider2D involved in this collision.</param>
	void OnTriggerEnter2D(Collider2D other) => triggerEnter.Invoke(other);

}