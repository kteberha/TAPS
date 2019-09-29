// Modified from SensorToolkit by Micosmo
// purchased source, do not distribute

using UnityEngine;
using UModules;

[RequireComponent(typeof(Rigidbody2D))]
public class Steering : ExtendedMonoBehaviour
{
    [Range(0.1f, 4f)]
    public float AvoidanceSensitivity = 1f;
    [Range(1f, 2f)]
    public float MaxAvoidanceLength = 1f;

    // Rotate the rig towards the target direction before calculating steer vectors. Useful for creating asymetric
    // ray sensor setups. See the example prefabs, they are all RotateTowardsTarget = true.
    public bool RotateTowardsTarget = false;

    // The maximum torque that will be applied to the rigid body.
    public float TurnForce;
    // The maximum force that will be applied to the rigid body in a forwards direction.
    public float MoveForce;
    // The maximum force that will be applied to the rigid body in a sideways or backwards direction.
    public float StrafeForce;
    // The maximum turning speed that will be applied to kinematic rigid bodies.
    public float TurnSpeed;
    // The maximum movement speed that will be applied to kinematic rigid bodies in a forwards direction.
    public float MoveSpeed;
    // The maximum movement speed that will be applied to kinematic rigid bodies in a sideways or backwards direction.
    public float StrafeSpeed;
    // The distance threshold for the rig to arrive at a destination position.
    public float StoppingDistance = 0.5f;

    // The rig will attempt to move towards this transform.
    [Readonly]
    public Transform Target;

    // The rig will face towards this transform, even strafing while moving towards destination.
    public Transform FaceTowardsTransform;

    RaySensor[] sensors;
    Vector2 destination;
    bool trackingToDestinationPosition;
    Vector2 faceDirection;
    bool directionToFaceAssigned;
    Vector2 previousAttractionVector;
    Vector2 previousRepulsionVector;
    Vector2 previousAvoidanceVector;

    private Rigidbody2D _rb;
    public Rigidbody2D RB { get {return Get<Rigidbody2D>(ref _rb);} }

    public Vector2 Destination
    {
        get
        {
            return Target != null ? (Vector2)Target.position : destination;
        }
        set
        {
            if (Target != null)
            {
                Debug.LogWarning("Cannot set Destination while Target is not Null.");
            }
            else
            {
                destination = value;
                trackingToDestinationPosition = true;
            }
        }
    }

    public bool Seeking
    {
        get { return RB != null && (Target != null || trackingToDestinationPosition); }
    }

    public Vector2 DirectionToFace
    {
        get
        {
            if (FaceTowardsTransform != null)
            {
                return (FaceTowardsTransform.position - RB.transform.position).normalized;
            }
            else if (directionToFaceAssigned)
            {
                return faceDirection;
            }
            else
            {
                return Vector2.zero;
            }
        }
        set
        {
            if (FaceTowardsTransform != null)
            {
                Debug.LogWarning("Cannot set DirectionToFace while FaceTowardsTransform is not Null.");
            }
            else if (value == Vector2.zero)
            {
                ClearDirectionToFace();
            }
            else
            {
                directionToFaceAssigned = true;
                faceDirection = value.normalized;
            }
        }
    }

    // Is the rig currently instructed to face a specific direction.
    public bool IsDirectionToFaceAssigned
    {
        get { return FaceTowardsTransform != null || directionToFaceAssigned; }
    }

    // Clears the assigned direction to face, stopping the rig from strafing.
    public void ClearDirectionToFace()
    {
        FaceTowardsTransform = null;
        directionToFaceAssigned = false;
    }

    // Takes a direction as parameter and pushes it depending on the obstacles nearby the rig.
    // Returns a vector that is in the general direction of 'targetDirection' but moved so that
    // nearby obstacles are avoided.
    public Vector2 GetSteeredDirection(Vector2 targetDirection)
    {
        if (RotateTowardsTarget)
        {
            var rot_z = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        }

        return AccumForces(targetDirection);
    }

    public override void Initialize()
    {
        sensors = GetComponentsInChildren<RaySensor>();
        trackingToDestinationPosition = false;
    }

    void Update()
    {
        if (RB == null || !RB.isKinematic) return;

        if (IsDirectionToFaceAssigned) FaceDirectionKinematic(DirectionToFace);

        if (!Seeking) return;

        if (ReachedDestination)
        {
            trackingToDestinationPosition = false;
            return;
        }

        Vector2 targetMoveDirection = (Destination - (Vector2)RB.transform.position).normalized;
        Vector2 avoidanceMoveDirection = GetSteeredDirection(targetMoveDirection);

        if (!IsDirectionToFaceAssigned) FaceDirectionKinematic(avoidanceMoveDirection);

        float forwardDotVel = Vector3.Dot(RB.transform.up, avoidanceMoveDirection.normalized);
        avoidanceMoveDirection = Mathf.Lerp(StrafeSpeed, MoveSpeed, Mathf.Clamp(forwardDotVel, 0f, 1f)) * avoidanceMoveDirection;
        RB.transform.position = (Vector2)RB.transform.position + avoidanceMoveDirection * Time.deltaTime;
    }

    private void FaceDirectionKinematic(Vector2 direction)
    {
        var deltaAngle = RB.transform.up.SignedAngleXY(direction);
        var maxDelta = (TurnSpeed * Time.deltaTime) * Mathf.Min(1f, Mathf.Abs(deltaAngle)/20f);
        deltaAngle = Mathf.Clamp(deltaAngle, -maxDelta, maxDelta);
        RB.transform.rotation = Quaternion.Euler(0f, 0f, RB.transform.rotation.eulerAngles.z - deltaAngle);
    }

    void FixedUpdate()
    {
        // Only seek with forces if we are seeking and we have a non-kinematic rigid body
        if (RB == null || RB.isKinematic) return;

        // If we have been assigned a direction to face, then turn to face it even if we aren't currently seeking
        if (IsDirectionToFaceAssigned) FaceDirectionForces(DirectionToFace);

        // Only seek with forces if we are seeking and we have a non-kinematic rigid body
        if (!Seeking) return;

        if (ReachedDestination)
        {
            trackingToDestinationPosition = false;
            return;
        }

        var targetMoveDirection = Destination - (Vector2)RB.transform.position;
        var avoidanceMoveDirection = GetSteeredDirection(targetMoveDirection);

        // If we haven't been assigned a direction to face then turn to face the direction we will move in.
        if (!IsDirectionToFaceAssigned) FaceDirectionForces(avoidanceMoveDirection);

        // Lerp the dot product of the direction I'm facing to the direction I'm moving,
        // this will interpolate between the strafing force and the moving force.
        float forwardDotMove = Vector3.Dot(RB.transform.up, avoidanceMoveDirection);
        Vector3 moveForce = Mathf.Lerp(StrafeForce, MoveForce, Mathf.Clamp(forwardDotMove, 0f, 1f)) * avoidanceMoveDirection;
        RB.AddForce(moveForce);
    }

    private void FaceDirectionForces(Vector2 direction)
    {
        var angle = RB.transform.up.SignedAngleXY(direction);
        var torque = Mathf.Clamp(angle / 20f, -1f, 1f) * TurnForce;
        RB.AddTorque(-torque);
    }

    private Vector3 AccumForces(Vector3 targetDirection)
    {
        previousAttractionVector = AttractionForce(targetDirection);
        previousRepulsionVector = RepulsionForce;
        var f = previousAttractionVector + previousRepulsionVector;
        if (f.sqrMagnitude > 0.01f)
        {
            previousAvoidanceVector = f.normalized;
            return previousAvoidanceVector;
        }
        else
        {
            previousAvoidanceVector = f * 100f;
            return previousAvoidanceVector;
        }
    }

    private Vector3 AttractionForce(Vector3 targetDirection)
    {
        var dest = targetDirection;
        if (dest.sqrMagnitude > 1f)
        {
            return dest.normalized;
        }
        else
        {
            return dest;
        }
    }

    private Vector3 RepulsionForce
    {
        get
        {
            var rf = Vector2.zero;
            for (int i = 0; i < sensors.Length; i++)
            {
                var s = sensors[i];
                s.Pulse();
                if (!s.IsObstructed) continue;
                var obsRatio = Mathf.Pow(1f - (s.ObstructionRayHit.distance / s.Length), 1f / AvoidanceSensitivity); // 0 when unobstructed, 1 when touching
                rf += obsRatio * s.ObstructionRayHit.normal;
            }
            var rfMag = rf.magnitude;
            if (rfMag > MaxAvoidanceLength)
            {
                return rf * MaxAvoidanceLength;
            }
            return rf * rfMag;
        }
    }

    private bool ReachedDestination => ((Vector2)RB.transform.position - Destination).magnitude <= StoppingDistance;

    protected static readonly Color AttractionVectorColor = new Color(51 / 255f, 255 / 255f, 255 / 255f);
    protected static readonly Color RepulsionVectorColor = Color.yellow;
    protected static readonly Color AvoidanceVectorColor = Color.green;
    public void OnDrawGizmosSelected()
    {
        if (!isActiveAndEnabled) return;

        var attractionPoint = (Vector2)transform.position + previousAttractionVector * 2f;
        var repulsionPoint = attractionPoint + previousRepulsionVector * 2f;
        var avoidancePoint = (Vector2)transform.position + previousAvoidanceVector * 2f;

        Gizmos.color = AttractionVectorColor;
        Gizmos.DrawLine(transform.position, attractionPoint);
        Gizmos.color = RepulsionVectorColor;
        Gizmos.DrawLine(attractionPoint, repulsionPoint);
        Gizmos.color = AvoidanceVectorColor;
        Gizmos.DrawLine(transform.position, avoidancePoint);
    }
}
