using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeatherStation {
	public class CabinetShelf : MonoBehaviour
	{
		// Start is called before the first frame update
		public void OnTriggerEnter(Collider c)
		{
			if(c.gameObject.layer == 11 && !gameObject.GetComponent<Rigidbody>().isKinematic) {
				//Debug.Log(gameObject.name + " hit " + c.gameObject.name);
				gameObject.transform.parent = c.gameObject.transform;//.parent = 
				gameObject.GetComponent<Rigidbody>().isKinematic = true;
				gameObject.GetComponent<BoxCollider>().isTrigger = false;
				//gameObject.GetComponent<Rigidbody>().mass = 0f;
			}
			/*Debug.Log("Hit shelf " + c.gameObject.name);
			GameObject g = c.gameObject;
			FixedJoint j = g.GetComponent<FixedJoint>();
			if(j == null) {
				j = g.AddComponent<FixedJoint>();
				j.massScale = 0f;
				j.connectedMassScale = 0f;
				g.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
			} 
			
			if(j.connectedBody == null) {
				j.connectedBody = GetComponent<Rigidbody>();
			}*/
			//GetComponent<BoxCollider>().isTrigger = false;
		}
		
		public void OnCollisionEnter(Collision c)
		{
			if(c.gameObject.layer == 11 && !gameObject.GetComponent<Rigidbody>().isKinematic) {
				//Debug.Log(gameObject.name + " hit " + c.gameObject.name);
				gameObject.transform.parent = c.gameObject.transform;//.parent = 
				gameObject.GetComponent<Rigidbody>().isKinematic = true;
				gameObject.GetComponent<BoxCollider>().isTrigger = false;
				//gameObject.GetComponent<Rigidbody>().mass = 0f;
			}
		}
	}
}