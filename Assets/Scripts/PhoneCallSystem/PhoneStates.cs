using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneStates
{
    private FSM<PhoneStates> _fsm;

    public void ChangeGameState<T>(T state) where T : PhoneStatesList
    {
        _fsm.TransitionTo<T>();
    }

    public void TransitToPreviousState()
    {
        _fsm.TransitionToPreviousState();
    }


    #region Class state dynamic

    public void Init()
    {
        // Initialize the FSM with the context (in this case the critter whose states we're managing)
        _fsm = new FSM<PhoneStates>(this);

        // Set the initial state. Don't forget to do this!!
        _fsm.TransitionTo<NotInPhone>();

    }

    public void Start()
    {

    }

    public void Update()
    {
        _fsm.Update();
    }

    public void Clear()
    {
    }

    #endregion

    #region States switch situation

    public class PhoneStatesList : FSM<PhoneStates>.State{}

    #endregion

    #region States detail perform

    public class NotInPhone : PhoneStatesList{}

    public class Dial : PhoneStatesList
    {
        public override void OnEnter()
        {
            if (Services.gameStates.GetCurrentState().GetType() != typeof(GameStates.PhoneCallPage))
            {
                Services.gameStates.ChangeGameState(new GameStates.PhoneCallPage());
            }
        }
    }

    public class OnCall : PhoneStatesList
    {
        public override void OnEnter()
        {
            if (Services.gameStates.GetCurrentState().GetType() != typeof(GameStates.PhoneCallPage))
            {
                Services.gameStates.ChangeGameState(new GameStates.PhoneCallPage());
            }
        }
    }

    public class CallIn : PhoneStatesList
    {
        public override void OnEnter()
        {
            if (Services.gameStates.GetCurrentState().GetType() != typeof(GameStates.PhoneCallPage))
            {
                Services.gameStates.ChangeGameState(new GameStates.PhoneCallPage());
            }
        }
    }
    #endregion
}
