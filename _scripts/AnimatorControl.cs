using UnityEngine;
using System.Collections;

public class AnimatorControl : MonoBehaviour {

	// Use this for initialization
    private Animator animator;
	void Start ()
	{
	    animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetNormalTrue()
    {
        animator.SetBool("isNormal",true);
    }

    public void SetNormalFalse()
    {
        animator.SetBool("isNormal",false);
    }

    public void SetClickTrue()
    {
        animator.SetBool("isIntoScene",true);
        Invoke("SetIntoScene",0.1f);
    }

    public void SetIntoScene()
    {
        animator.SetBool("isIntoScene",false);
    }

    public void SetNextTrue()
    {
        animator.SetBool("isNext",true);
        Invoke("SetNextFalse",0.1f);
    }

    public void SetNextFalse()
    {
        animator.SetBool("isNext", false);
    }
}//YZ
