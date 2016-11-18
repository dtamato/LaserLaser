using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class BlinkText : MonoBehaviour {

	[SerializeField] float blinkSpeed;

	Text text;
	float timer;

	void Awake () {

		text = this.GetComponentInChildren<Text> ();
		timer = 0;
	}

	void Update () {

		timer += Time.deltaTime;

		if (timer >= blinkSpeed) {

			text.enabled = !text.enabled;
			timer = 0;
		}
	}
}
