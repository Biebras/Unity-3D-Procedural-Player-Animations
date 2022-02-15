using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This code was made by marmitoTH.
/// The code can be accessed: https://github.com/marmitoTH/Unity-Kinematic-Body
/// </summary>

public class KinematicBody : MonoBehaviour
{
	public float slopeLimit = 45f;
	public float stepOffset = 0.3f;
	public float skinWidth = 0.08f;

	[SerializeField] private Vector3 _center = Vector3.zero;
	[SerializeField] private float _radius = 0.5f;
	[SerializeField] private float _height = 2f;
	[SerializeField] private float smoothAcceleration = 4;

	private Vector3 _position;
	private Vector3 _upDirection;
	private Vector3 _lastVelocity;

	private Rigidbody _rigidbody;
	private CapsuleCollider _collider;

	private readonly Collider[] _overlaps = new Collider[5];
	private readonly List<RaycastHit> _contacts = new List<RaycastHit>();

	private const int MaxSweepSteps = 5;
	private const float MinMoveDistance = 0f;
	private const float MinCeilingAngle = 145;

	public Vector3 Velocity { get; private set; }
	public Vector3 Acceleration { get; private set; }
	public bool IsGrounded { get; private set; }

	public Vector3 Center
	{
		get { return _center; }
		set
		{
			_center = value;
			Collider.center = value;
		}
	}

	public float Radius
	{
		get { return _radius; }
		set
		{
			_radius = value;
			Collider.radius = value;
		}
	}

	public float Height
	{
		get { return _height; }
		set
		{
			_height = value;
			Collider.height = value;
		}
	}

	public Rigidbody Rigidbody
	{
		get
		{
			if (!_rigidbody)
			{
				if (!TryGetComponent(out _rigidbody))
				{
					_rigidbody = gameObject.AddComponent<Rigidbody>();
				}
			}

			return _rigidbody;
		}
	}

	public CapsuleCollider Collider
	{
		get
		{
			if (!_collider)
			{
				if (!TryGetComponent(out _collider))
				{
					_collider = gameObject.AddComponent<CapsuleCollider>();
				}
			}

			return _collider;
		}
	}

	private void Start()
	{
		InitializeRigidbody();
		InitializeCollider();
	}

	private void InitializeRigidbody()
	{
		Rigidbody.isKinematic = true;
		Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
	}

	private void InitializeCollider()
	{
		Collider.center = _center;
		Collider.height = _height;
		Collider.radius = _radius;
	}

	public void Move(Vector3 motion)
	{
		GetState(motion);
		ClearVariables();
		HandleCollision();
		HandleContacts();
		Depenetrate();
		SetState();
		CalculateAcceleration();

		_lastVelocity = Velocity;
	}

	public void Rotate(Quaternion rotation)
	{
		Rigidbody.MoveRotation(rotation);
	}

	private void GetState(Vector3 motion)
	{
		_position = Rigidbody.position;
		_upDirection = transform.up;
		Velocity = motion;
	}

	private void SetState()
	{
		Rigidbody.MovePosition(_position);
	}

	private void ClearVariables()
	{
		_contacts.Clear();
		IsGrounded = false;
	}

	private void HandleCollision()
	{
		if (Velocity.sqrMagnitude > MinMoveDistance)
		{
			Vector3 localVelocity = transform.InverseTransformDirection(Velocity) * Time.deltaTime;
			Vector3 lateralVelocity = new Vector3(localVelocity.x, 0, localVelocity.z);
			Vector3 verticalVelocity = new Vector3(0, localVelocity.y, 0);

			lateralVelocity = transform.TransformDirection(lateralVelocity);
			verticalVelocity = transform.TransformDirection(verticalVelocity);

			CapsuleSweep(lateralVelocity.normalized, lateralVelocity.magnitude, stepOffset, MinCeilingAngle);
			CapsuleSweep(verticalVelocity.normalized, verticalVelocity.magnitude, 0, 0, slopeLimit);
		}
	}

	private void HandleContacts()
	{
		if (_contacts.Count > 0)
		{
			float angle;

			foreach (RaycastHit contact in _contacts)
			{
				angle = Vector3.Angle(_upDirection, contact.normal);

				if (angle <= slopeLimit)
				{
					IsGrounded = true;
				}

				Velocity -= Vector3.Project(Velocity, contact.normal);
			}
		}
	}

	private void CapsuleSweep(Vector3 direction, float distance, float stepOffset, float minSlideAngle = 0, float maxSlideAngle = 360)
	{
		Vector3 origin, top, bottom;
		RaycastHit hitInfo;
		float safeDistance;
		float slideAngle;

		float capsuleOffset = _height * 0.5f - _radius;

		for (int i = 0; i < MaxSweepSteps; i++)
		{
			origin = _position + _center - direction * _radius;
			bottom = origin - _upDirection * (capsuleOffset - stepOffset);
			top = origin + _upDirection * capsuleOffset;

			if (Physics.CapsuleCast(top, bottom, _radius, direction, out hitInfo, distance + _radius))
			{
				slideAngle = Vector3.Angle(_upDirection, hitInfo.normal);
				safeDistance = hitInfo.distance - _radius - skinWidth;
				_position += direction * safeDistance;
				_contacts.Add(hitInfo);

				if ((slideAngle >= minSlideAngle) && (slideAngle <= maxSlideAngle))
				{
					break;
				}

				direction = Vector3.ProjectOnPlane(direction, hitInfo.normal);
				distance -= safeDistance;
			}
			else
			{
				_position += direction * distance;
				break;
			}
		}
	}

	private void Depenetrate()
	{
		float capsuleOffset = _height * 0.5f - _radius;
		Vector3 top = _position + _upDirection * capsuleOffset;
		Vector3 bottom = _position - _upDirection * capsuleOffset;
		int overlapsNum = Physics.OverlapCapsuleNonAlloc(top, bottom, Collider.radius, _overlaps);

		if (overlapsNum > 0)
		{
			for (int i = 0; i < overlapsNum; i++)
			{
				if ((_overlaps[i].transform != transform) && Physics.ComputePenetration(Collider, _position, transform.rotation, 
					_overlaps[i], _overlaps[i].transform.position, _overlaps[i].transform.rotation, out Vector3 direction, out float distance))
				{
					_position += direction * (distance + skinWidth);
					Velocity -= Vector3.Project(Velocity, -direction);
				}
			}
		}
	}

	private void CalculateAcceleration()
    {
		var acc = (Velocity - _lastVelocity) / Time.fixedDeltaTime;
		Acceleration = Vector3.Lerp(Acceleration, acc, Time.deltaTime * smoothAcceleration);
	}
}