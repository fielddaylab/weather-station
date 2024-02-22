using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlexAnimation : MonoBehaviour
{
	[SerializeField]
	Animator _animator;
	
	public GameObject Notebook;
	
	public GameObject Pen;
	
	public GameObject StartPointNW;
	public GameObject WalkPointNW;
	
	public GameObject StartPointS;	
	public GameObject WalkPointS;
	
	[SerializeField]
	List<Transform> _startLocations = new List<Transform>(5);
	
	private bool ToggleSpot = false;
	private bool Walking = false;
	private bool WalkingBackAndForthS = false;
	private bool WalkingBackAndForthNW = false;
	private bool TurnedAround = true;
	private bool TurningAround = false;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public IEnumerator Walk(Transform location, float duration)
	{
		if(!Walking)
		{
			_animator.SetBool("walking", true);
			yield return new WaitForSeconds(1f);
			
			Walking = true;
			float t = 0f;
			Vector3 startPos = transform.position;
			while(t < duration && (WalkingBackAndForthS || WalkingBackAndForthNW))
			{
				Vector3 newPos = Vector3.Lerp(startPos, location.position, t/duration);
				transform.position = newPos;
				t += UnityEngine.Time.unscaledDeltaTime;
				yield return null;
			}
			
			_animator.SetBool("walking", false);
			
			yield return new WaitForSeconds(2f);
		}
		
		ToggleSpot = !ToggleSpot;
		Walking = false;
		TurnedAround = false;
	}
	
	public IEnumerator TurnAround(bool doPoint=true)
	{
		if(!TurnedAround)
		{
			TurningAround = true;
			
			//do a point here...
			if(doPoint)
			{
				_animator.SetTrigger("point00");
			}
			
			yield return new WaitForSeconds(2f);
			
			_animator.SetTrigger("turnaround");
			
			yield return new WaitForSeconds(2f);
			
		}
		
		TurnedAround = true;
		
	}
	
	public IEnumerator WalkBackAndForthS()
	{
		while(WalkingBackAndForthS)
		{
			if(!Walking)
			{
				if(TurnedAround)
				{
					TurningAround = false;
					
					if(ToggleSpot)
					{
						StartCoroutine(Walk(StartPointS.transform, 15f));
					}
					else
					{
						StartCoroutine(Walk(WalkPointS.transform, 15f));
					}
				}
				else
				{
					if(!TurningAround)
					{
						StartCoroutine(TurnAround());
					}
				}
			}
			else
			{
				if(!TurningAround)
				{
					StartCoroutine(TurnAround());
				}		
			}

			yield return null;
		}
	}
	
	public IEnumerator WalkBackAndForthNW()
	{
		while(WalkingBackAndForthNW)
		{
			_animator.SetBool("kneeling", true);
			_animator.SetBool("tinkering", true);
			
			yield return new WaitForSeconds(1f);
			
			_animator.SetTrigger("stand");
			
			/*_animator.SetBool("kneeling", false);
			_animator.SetBool("tinkering", false);
			
			TurnedAround = false;
			
			StartCoroutine(TurnAround(false));
			
			StartCoroutine(Walk(WalkPointNW.transform, 3f));
			
			TurnedAround = false;
			
			StartCoroutine(TurnAround(false));
			
			StartCoroutine(Walk(StartPointNW.transform, 3f));*/
			
			//_animator.ResetTrigger("stand");
		}
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
		WalkingBackAndForthS = false;
		WalkingBackAndForthNW = false;
		ToggleSpot = false;
		Walking = false;
		TurnedAround = true;
		TurningAround = false;
		
		if(Notebook != null)
		{
			Notebook.SetActive(false);
		}
		
		if(Pen != null)
		{
			Pen.SetActive(false);
		}
		
		if(_animator != null)
		{
			_animator.SetBool("writing", false);
			_animator.SetBool("walking", false);
			_animator.SetBool("tinkering", false);
			_animator.SetBool("standtinker", false);
			_animator.SetBool("kneeling", false);
			_animator.ResetTrigger("turnaround");
			_animator.ResetTrigger("stand");
			
			_animator.Play("Idle", 0);
		}	
	}
	
	public void StartWriting()
	{
		if(_animator != null)
		{
			_animator.SetBool("writing", true);
		}
	}
	
	public void StartTinkering()
	{
		if(_animator != null)
		{
			_animator.SetBool("standtinker", true);
		}
	}
	
	public void StartWalkLoopNW()
	{
		if(_animator != null)
		{
			WalkingBackAndForthNW = true;
			StartCoroutine(WalkBackAndForthNW());
		}
	}
	
	public void StartWalkLoopS()
	{
		if(_animator != null)
		{
			WalkingBackAndForthS = true;
			StartCoroutine(WalkBackAndForthS());
		}
	}
	
	public void StartKneeling()
	{
		if(_animator != null)
		{
			_animator.SetBool("kneeling", true);
			_animator.SetBool("tinkering", true);
		}
	}
}
