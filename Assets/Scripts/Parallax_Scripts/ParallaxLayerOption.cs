using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ParallaxLayerOption : MonoBehaviour {
	public float speedX;
	public float speedY;
	public bool moveInOppositeDirection;

	private Transform cameraTransform;
	private Vector3 previousCameraPosition;
	private bool previousMoveParallax;
	public ParallaxOption options;

	void OnEnable() {
		cameraTransform = options.transform;
		previousCameraPosition = cameraTransform.position;
	}

	void Update () {
		if(options.moveParallax && !previousMoveParallax)
			previousCameraPosition = cameraTransform.position;

		previousMoveParallax = options.moveParallax;

		if(!Application.isPlaying && !options.moveParallax)
			return;

		Vector3 distance = cameraTransform.position - previousCameraPosition;
		float direction = (moveInOppositeDirection) ? -1f : 1f;
		transform.position += Vector3.Scale(distance, new Vector3(speedX, speedY)) * direction;

		previousCameraPosition = cameraTransform.position;
	}
}
