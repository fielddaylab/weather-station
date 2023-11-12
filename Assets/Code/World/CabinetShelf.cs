using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FieldDay.Components;
using FieldDay.SharedState;

namespace WeatherStation {
	
	public class CabinetShelf : BatchedComponent {
	
		public LayerMask GripMask;
		
		private BoxCollider CachedBox;
		
		private readonly Collider[] OverlapCache = new Collider[32];
		private int OverlapCount = 0;
		
		private void Awake() {
			CachedBox = GetComponent<BoxCollider>();
        }
		
		public void QueryColliders() {
			//Debug.Log("Query Colliders");
			OverlapCount = Physics.OverlapBoxNonAlloc(transform.TransformPoint(CachedBox.center), CachedBox.size*0.5f, OverlapCache, transform.rotation, GripMask, QueryTriggerInteraction.Ignore);
			//Debug.Log(gameObject.name + " Overlap Count: " + OverlapCount);
			for(int i = 0; i < OverlapCount; ++i) {
				//Debug.Log(OverlapCache[i].gameObject.name);
				OverlapCache[i].gameObject.transform.parent = gameObject.transform;
				OverlapCache[i].gameObject.GetComponent<Rigidbody>().isKinematic = true;
				OverlapCache[i].gameObject.GetComponent<Rigidbody>().detectCollisions = false;
			}
			
			StartCoroutine("SwitchBack");
		}
		
		IEnumerator SwitchBack() {
			yield return new WaitForSeconds(2.5f);
			for(int i = 0; i < OverlapCount; ++i) {
				OverlapCache[i].transform.SetParent(OverlapCache[i].gameObject.GetComponent<Grabbable>().OriginalParent, true);
				OverlapCache[i].gameObject.GetComponent<Rigidbody>().isKinematic = false;
				OverlapCache[i].gameObject.GetComponent<Rigidbody>().detectCollisions = true;
			}
			
		}
		
		// Start is called before the first frame update
		/*public void OnTriggerEnter(Collider c)
		{
			if(c.gameObject.layer == 11 && !gameObject.GetComponent<Rigidbody>().isKinematic) {
				//Debug.Log(gameObject.name + " hit " + c.gameObject.name);
				gameObject.transform.parent = c.gameObject.transform;//.parent = 
				gameObject.GetComponent<Rigidbody>().isKinematic = true;
				gameObject.GetComponent<BoxCollider>().isTrigger = false;
				//gameObject.GetComponent<Rigidbody>().mass = 0f;
			}
			Debug.Log("Hit shelf " + c.gameObject.name);
			//GameObject g = c.gameObject;
			//FixedJoint j = g.GetComponent<FixedJoint>();
			//if(j == null) {
			//	j = g.AddComponent<FixedJoint>();
			//	j.massScale = 0f;
			//	j.connectedMassScale = 0f;
			//	g.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
			//} 
			
			//if(j.connectedBody == null) {
			//	j.connectedBody = GetComponent<Rigidbody>();
			//}
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
		}*/
	}
}