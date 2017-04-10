using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Area { Speed, Accuracy, Rhythm };

[DisallowMultipleComponent]
public class ThreeKeysController : MonoBehaviour {

	[Header("Obstacles")]
	[SerializeField] GameObject[] speedObstacles;
	[SerializeField] GameObject[] accuracyObstacles;
	[SerializeField] GameObject[] rhythmObstacles;

	[Header("End Door")]
	[SerializeField] GameObject door;
	[SerializeField] GameObject speedLock;
	[SerializeField] GameObject accuracyLock;
	[SerializeField] GameObject rhythmLock;

	[Header("Doors")]
	[SerializeField] GameObject speedDoor;
	[SerializeField] GameObject rhythmDoor;
	[SerializeField] GameObject accuracyDoor;

	bool speedAreaComplete = false;
	bool accuracyAreaComplete = false;
	bool rhythmAreaComplete = false;


	void Update () {

		speedLock.transform.RotateAround(door.transform.position, Vector3.forward, 50 * Time.deltaTime);
		accuracyLock.transform.RotateAround(door.transform.position, Vector3.forward, 50 * Time.deltaTime);
		rhythmLock.transform.RotateAround(door.transform.position, Vector3.forward, 50 * Time.deltaTime);
	}

	public void FinishArea(Area finishedArea) {

		switch (finishedArea) {
			case Area.Speed:
			
				speedAreaComplete = true;
				speedLock.GetComponent<SpriteRenderer>().color = Color.green;

				for(int i = 0; i < speedObstacles.Length; i++) {

					speedObstacles[i].SetActive(false);
				}
				break;
			case Area.Accuracy:
			
				accuracyAreaComplete = true;
				accuracyLock.GetComponent<SpriteRenderer>().color = Color.green;
				speedDoor.SetActive(false);

				for(int i = 0; i < accuracyObstacles.Length; i++) {

					accuracyObstacles[i].SetActive(false);
				}
				break;
			case Area.Rhythm:
			
				rhythmAreaComplete = true;
				rhythmLock.GetComponent<SpriteRenderer>().color = Color.green;
				accuracyDoor.SetActive(false);

				for(int i = 0; i < rhythmObstacles.Length; i++) {

					rhythmObstacles[i].SetActive(false);
				}
				break;
			default:
				break;
		}

		CheckDoorCompletion();
	}

	void CheckDoorCompletion () {

		if(speedAreaComplete && accuracyAreaComplete && rhythmAreaComplete) {

			door.GetComponentInChildren<SpriteRenderer>().color = Color.green;
			door.GetComponentInChildren<Light>().color = Color.green;
			door.GetComponentInChildren<Collider2D>().isTrigger = true;

			speedLock.SetActive(false);
			accuracyLock.SetActive(false);
			rhythmLock.SetActive(false);
		}
	}
}
