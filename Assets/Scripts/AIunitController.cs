using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIState
{
	test,
	hunt,
	fallback,
	recover,
	groupUp,
	support
};

public class AIUnitController : MonoBehaviour {

	public int healthThreshold;
	public int sightRange;
	public unitScript agroTarget;
	public int anger = 1;
	public int wrecklessness = 5;
	public bool deulingPreference = false;
	

	public unitScript target;
	public AIState state = AIState.hunt;

	public bool moved;
	public bool attacked;

	public void endTurn()
	{
		moved = false;
		attacked = false;
	}

}
