using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 상태를 클래스화, 객체화하여 상태분기에 따른 조건문포화의 횡포를 막는다. 

 * 상태마다 클래스가있고 스테이트매니저와 각 상태를 싱글톤으로 하여 객체화 시켜놓는다.

 * SetState(상태클래스.GetStateInstance());  == 각상황에따른 상태의 객체를 가져와서 스테이트매니저 대가리객체에 씌운다. 
   그러면 그 대가리객체가 내현재상태가 되는것. 저함수를 호출한 클래스가 이전상태인것이고 함수에들어가는 인자가 바뀌는 상태이다.

 * 마침내 상태가 자동적으로 조건문없이 지속적으로 갱신된다.
 
 * 스테이트 매니저의 아래 상태함수가 호출되는것은 현재상태(현재상태클래스까지 타고가서 오버라이딩된 같은이름)의 
   상태함수가 호출되는것임. 
 * 예) 셋스테이트(아이들상태) 인 상황에서 StateManager.instance.상태함수() 를 호출하면 아이들클래스에 오버라이딩된
 * 같은이름의 상태함수가 호출됨. 그럼 아이들상태로 바뀐상태. 그럼 그상태로 또 예를들어 StateManager.instance.Circuit() 써킷이면.
 * 셋스테이트(써킷)으로 바꿔주고 써킷함수내용을 실행. 그러면 써킷상태가 된상황.
 * 여기서또 StateManager.instance.상태함수()를 호출하면 써킷기준의 그상태가 되는 거라 생각하면됨.
 * StateManager.instance.에임() 이면 움직이면서 조준하는상태니까 에임() 함수안에 셋스테이트(에임써킷)후 내용실행. 상태는 움직이며 조준.
 * 상태의 무한반복.
 */
public interface IActionState
{
    void Idle(bool _value);
    void Circuit(bool _value);
    void Aim(bool _value);
    void Shot(bool _value);
}

public class StateManager : MonoBehaviour, IActionState
{
    public static StateManager instance;
    public GameObject obj_stateList;
    public IActionState myActionState { get; set; }
    
    private void Update()
    {
        //Debug.Log(myActionState);
    }
    private void Awake()
    {
        if (instance == null)
            instance = this;

        SetState(State_Idle.GetStateInstance());
    }

    public void SetState(IActionState _actionState) {  myActionState = _actionState;  }

    public void Idle(bool _value) {  myActionState.Idle(_value);  }

    public void Circuit(bool _value) {  myActionState.Circuit(_value);  }

    public void Aim(bool _value) {  myActionState.Aim(_value);  }

    public void Shot(bool _value) {  myActionState.Shot(_value);  }

}
