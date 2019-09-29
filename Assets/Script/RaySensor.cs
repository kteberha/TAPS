// Modified from SensorToolkit by Micosmo
// purchased source, do not distribute

using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class RaySensor : MonoBehaviour
{
    // Specified whether the ray sensor will pulse automatically each frame or will be updated manually by having its Pulse() method called when needed.
    public enum UpdateMode { EachFrame, Manual }

    // The length of the ray sensor detection range in world units.
    public float Length = 5f;

    // A layermask for colliders that will block the ray sensors path.
    public LayerMask ObstructedByLayers;

    // A layermask for colliders that are detected by the ray sensor.
    public LayerMask DetectsOnLayers;

    // What direction does the ray sensor detect in.
    public Vector2 Direction = Vector2.up;

    // Is the Direction parameter in world space or local space.
    public bool WorldSpace = false;

    // Should the sensor pulse each frame automatically or will it be pulsed manually.
    public UpdateMode SensorUpdateMode;

    // Returns a list of all detected GameObjects in no particular order.
    public IEnumerable<GameObject> DetectedObjects
    {
        get
        {
            var detectedEnumerator = detectedObjects.GetEnumerator();
            while (detectedEnumerator.MoveNext())
            {
                var go = detectedEnumerator.Current;
                if (go != null && go.activeInHierarchy)
                {
                    yield return go;
                }
            }
        }
    }

    // Returns a list of all detected GameObjects in order of distance from the sensor. This distance is given by the RaycastHit.dist for each GameObject.
    public IEnumerable<GameObject> DetectedObjectsOrderedByDistance { get { return DetectedObjects; } }

    // Returns a list of all RaycastHit objects, each one is associated with a GameObject in the detected objects list.
    public IList<RaycastHit2D> DetectedObjectRayHits { get { return new List<RaycastHit2D>(detectedObjectHits.Values); } }

    // Returns the Collider that obstructed the ray sensors path, or null if it wasn't obstructed.
    public Collider2D ObstructedBy { get { return obstructionRayHit.collider; } }

    // Returns the RaycastHit data for the collider that obstructed the rays path.
    public RaycastHit2D ObstructionRayHit { get { return obstructionRayHit; } }

    // Returns true if the ray sensor is being obstructed and false otherwise
    public bool IsObstructed { get { return isObstructed && ObstructedBy != null; } }

    // Event fired at the time the sensor is obstructed when before it was unobstructed
    [SerializeField]
    public UnityEvent OnObstruction;

    // Event fired at the time the sensor is unobstructed when before it was obstructed
    [SerializeField]
    public UnityEvent OnClear;

    [SerializeField]
    public UnityEvent<GameObject> OnDetected;

    [SerializeField]
    public UnityEvent<GameObject> OnLostDetection;

    // Event fired each time the sensor is pulsed. This is used by the editor extension and you shouldn't have to subscribe to it yourself.
    public delegate void SensorUpdateHandler();
    public event SensorUpdateHandler OnSensorUpdate;

    Vector2 direction { get { return WorldSpace ? Direction.normalized : (Vector2)transform.TransformDirection(Direction.normalized); } }
    RayDistanceComparer2D distanceComparer;

    bool isObstructed = false;
    RaycastHit2D obstructionRayHit;
    Dictionary<GameObject, RaycastHit2D> detectedObjectHits;
    HashSet<GameObject> previousDetectedObjects;
    List<GameObject> detectedObjects;

    // Returns true if the passed GameObject appears in the sensors list of detected gameobjects
    public bool IsDetected(GameObject go)
    {
        return detectedObjectHits.ContainsKey(go);
    }

    // Pulse the ray sensor
    public void Pulse()
    {
        if (isActiveAndEnabled) TestRay();
    }

    // detectedGameObject should be a GameObject that is detected by the sensor. In this case it will return
    // the Raycasthit data associated with this object.
    public RaycastHit2D GetRayHit(GameObject detectedGameObject)
    {
        RaycastHit2D val;
        if (!detectedObjectHits.TryGetValue(detectedGameObject, out val))
        {
            Debug.LogWarning("Tried to get the RaycastHit for a GameObject that isn't detected by RaySensor.");
        }
        return val;
    }

    void OnEnable()
    {
        detectedObjects = new List<GameObject>();
        distanceComparer = new RayDistanceComparer2D();
        detectedObjectHits = new Dictionary<GameObject, RaycastHit2D>();
        previousDetectedObjects = new HashSet<GameObject>();
        ClearDetectedObjects();
    }

    void Update()
    {
        if (Application.isPlaying && SensorUpdateMode == UpdateMode.EachFrame) TestRay();
    }

    private bool LayerMaskIsSubsetOf(LayerMask lm, LayerMask subsetOf)
    {
        return ((lm | subsetOf) & (~subsetOf)) == 0;
    }

    private void TestRay()
    {
        ClearDetectedObjects();
        if (LayerMaskIsSubsetOf(DetectsOnLayers, ObstructedByLayers))
        {
            TestRaySingle();
        }
        else
        {
            TestRayMulti();
        }

        ObstructionEvents();
        DetectionEvents();

        if (OnSensorUpdate != null) OnSensorUpdate();
    }

    private void ObstructionEvents()
    {
        if (isObstructed && obstructionRayHit.collider == null)
        {
            isObstructed = false;
            OnClear.Invoke();
        }
        else if (!isObstructed && obstructionRayHit.collider != null)
        {
            isObstructed = true;
            OnObstruction.Invoke();
        }
    }

    private void DetectionEvents()
    {
        // Any GameObjects still in previousDetectedObjects are no longer detected
        var lostDetectionEnumerator = previousDetectedObjects.GetEnumerator();
        while (lostDetectionEnumerator.MoveNext())
        {
            OnLostDetection.Invoke(lostDetectionEnumerator.Current);
        }

        previousDetectedObjects.Clear();
        for (int i = 0; i < detectedObjects.Count; i++)
        {
            previousDetectedObjects.Add(detectedObjects[i]);
        }
    }

    private void TestRaySingle()
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, direction, Length, ObstructedByLayers);

        if (hit.collider != null)
        {
            if ((1 << hit.collider.gameObject.layer & DetectsOnLayers) != 0)
            {
                AddRayHit(hit);
            }
            obstructionRayHit = hit;
        }
    }

    private void TestRayMulti()
    {
        LayerMask combinedLayers = DetectsOnLayers | ObstructedByLayers;
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(transform.position, direction, Length, combinedLayers);

        Array.Sort(hits, distanceComparer);

        for (int i = 0; i < hits.Length; i++)
        {
            var hit = hits[i];
            if ((1 << hit.collider.gameObject.layer & DetectsOnLayers) != 0)
            {
                AddRayHit(hit);
            }
            if ((1 << hit.collider.gameObject.layer & ObstructedByLayers) != 0)
            {
                obstructionRayHit = hit;
                break;
            }
        }
    }

    private void AddRayHit(RaycastHit2D hit)
    {
        GameObject go = hit.collider.gameObject;

        if (!detectedObjectHits.ContainsKey(go))
        {
            detectedObjectHits.Add(go, hit);
            detectedObjects.Add(go);
            if (!previousDetectedObjects.Contains(go))
            {
                OnDetected.Invoke(go);
            }
            else
            {
                previousDetectedObjects.Remove(go);
            }
        }
    }

    private void ClearDetectedObjects()
    {
        obstructionRayHit = new RaycastHit2D();
        detectedObjectHits.Clear();
        detectedObjects.Clear();
    }

    private void Reset()
    {
        ClearDetectedObjects();
        isObstructed = false;
    }

    class RayDistanceComparer2D : IComparer<RaycastHit2D>
    {
        public int Compare(RaycastHit2D x, RaycastHit2D y)
        {
            if (x.distance < y.distance) { return -1; }
            else if (x.distance > y.distance) { return 1; }
            else { return 0; }
        }
    }

    protected static readonly Color GizmoColor = new Color(51 / 255f, 255 / 255f, 255 / 255f);
    protected static readonly Color GizmoBlockedColor = Color.red;

    public void OnDrawGizmosSelected()
    {
        if (!isActiveAndEnabled) return;

        Vector3 endPosition;
        if (IsObstructed)
        {
            Gizmos.color = GizmoBlockedColor;
            endPosition = transform.position + (Vector3)direction * obstructionRayHit.distance;
        }
        else
        {
            Gizmos.color = GizmoColor;
            endPosition = transform.position + (Vector3)direction * Length;
        }

        Gizmos.DrawLine(transform.position, endPosition);
    }
}