using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformPath : MonoBehaviour
{
	public enum MoveOnPathType {
		MoveTowards,
		Lerp
	}

	public MoveOnPathType type = MoveOnPathType.MoveTowards;
    public PathPoints path;
	public float speed = 1;
	public float maxDistranceToGoal = .1f;

	private IEnumerator<Transform> currPoint;

	public void Start(){
		if (path == null) {
			Debug.LogError ("Path cannot be null");
			return;
		}
		currPoint = path.GetPathEnumerator ();
		currPoint.MoveNext ();

		if (currPoint.Current == null) {
			return;
		}

		transform.position = currPoint.Current.position;
	}

	public void Update(){
		if (currPoint == null || currPoint.Current == null)
			return;
		if (type == MoveOnPathType.MoveTowards)
			transform.position = Vector3.MoveTowards (transform.position, currPoint.Current.position, Time.deltaTime * speed);
		else if ( type == MoveOnPathType.Lerp )
			transform.position = Vector3.Lerp (transform.position, currPoint.Current.position, Time.deltaTime * speed);

		var distanceSqr = (transform.position - currPoint.Current.position).sqrMagnitude;
		if (distanceSqr < maxDistranceToGoal * maxDistranceToGoal)
			currPoint.MoveNext ();
	}
}
