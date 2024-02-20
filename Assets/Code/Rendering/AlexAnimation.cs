using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlexAnimation : MonoBehaviour
{
	[SerializeField]
	Animator _animator;
	
	public GameObject Notebook;
	
	public GameObject Pen;
	
	public GameObject WalkPointNW;
	
	public GameObject WalkPointS;
	
	[SerializeField]
	List<Transform> _startLocations = new List<Transform>(5);
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void SetStartingLocation(int index)
	{
		if(index < _startLocations.Count)
		{
			transform.position = _startLocations[index].position;
			transform.rotation = _startLocations[index].rotation;
		}
	}
	
	public void StopAllAnimations()
	{
		if(_animator != null)
		{
			_animator.SetBool("writing", false);
			//_animator.ResetTrigger("kneel00");
		}	
	}
	
	public void StartWriting()
	{
		if(_animator != null)
		{
			_animator.SetBool("writing", true);
		}
	}
	
	public void StartWalkLoop()
	{
		if(_animator != null)
		{
			_animator.SetBool("walking", true);
		}
	}
	
	public void StartKneeling()
	{
		if(_animator != null)
		{
			_animator.SetTrigger("kneel00");
		}
	}
}
