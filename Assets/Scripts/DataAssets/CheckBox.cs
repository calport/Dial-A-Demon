using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBox : MonoBehaviour
{

    [SerializeField]private string dataName;
    private Data_Contract dataFile;
    [SerializeField]private GameObject onIcon;
    [SerializeField]private GameObject offIcon;
    
    private void OnEnable()
    {
        dataFile = Services.dataContract;
        if (dataFile.DataBool.ContainsKey(dataName))
        {
            bool data;
            dataFile.DataBool.TryGetValue(dataName, out data);
            SwitchOnOff(data);
        }
    }


    // Update is called once per frame
    public void Click()
    {
        if (dataFile.DataBool.ContainsKey(dataName))
        {
            bool data;
            dataFile.DataBool.TryGetValue(dataName, out data);
            data = !data;
            dataFile.ChangeData(dataName,data);
            SwitchOnOff(data);
        }
    }

    void SwitchOnOff(bool data)
    {
        if (data)
        {
            onIcon.SetActive(true);
            offIcon.SetActive(false);
        }
        else
        {
            onIcon.SetActive(false);
            offIcon.SetActive(true);
        }
    }
}
