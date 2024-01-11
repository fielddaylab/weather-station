using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryCoverAnim : MonoBehaviour
{
	Animator CoverAnim = null;
    // Start is called before the first frame update
    void Awake()
    {
        CoverAnim = GetComponent<Animator>();
    }

	public void OpenCover()
	{
		CoverAnim.SetBool("Open", true);
	}
	
	public void CloseCover()
	{
		CoverAnim.SetBool("Open", false);
	}
	
    // Update is called once per frame
    void Update()
    {
        
    }
}
