using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleOn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void DelayOn(Renderer ps, float fTime) {
		StartCoroutine(DelayEnable(ps, fTime));
	}
	
	IEnumerator DelayEnable(Renderer ps, float fTime) {
		yield return new WaitForSeconds(fTime);
		ps.enabled = true;
	}
}
