using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialPanel : MonoBehaviour
{
    public List<Button> dialButtons = new List<Button>();
    public List<int> dialButtonRefNum = new List<int>();
    public Button deleteButton;
    public Text dialView;
    public int dialLimLength = 10;
    [HideInInspector]
    public List<int> dialedList = new List<int>();
    
    // Start is called before the first frame update
    void OnEnable()
    {
        foreach (var button in dialButtons)
            button.onClick.AddListener(_OnNumButtonClick);
        dialView.text = "";
        if(!ReferenceEquals(deleteButton,null))
            deleteButton.onClick.AddListener(_OnDeleteButtonClick);
        
        Services.eventManager.AddHandler<ResetForPageChange>(_OnReset);
    }

    private void OnDisable()
    {
        foreach (var button in dialButtons)
            button.onClick.RemoveListener(_OnNumButtonClick);
        dialView.text = "";
        if(!ReferenceEquals(deleteButton,null))
            deleteButton.onClick.RemoveListener(_OnDeleteButtonClick);
    }

    private void _OnNumButtonClick()
    {
        
        var index = dialButtons.IndexOf(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>());
        var num = dialButtonRefNum[index];
        if(dialedList.Count < dialLimLength) 
            dialedList.Add(num);
        _UpdateView();
    }

    private void _OnDeleteButtonClick()
    {
        if (dialedList.Count != 0)
            dialedList.Remove(dialedList[dialedList.Count - 1]);
        _UpdateView();
    }

    private void _UpdateView()
    {
        var str = "";
        foreach (var number in dialedList)
            str += number.ToString(); 
        dialView.text = str;
    }

    private void _OnReset(ResetForPageChange e)
    {
        if (string.Equals(e.toPage.name, "Phone_OnCall"))
        {
            dialedList.Clear();
            _UpdateView();
        }
    }
}
