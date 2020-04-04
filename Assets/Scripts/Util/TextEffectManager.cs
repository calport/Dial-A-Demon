using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.Experimental.XR;

public abstract class Tag
{
    public Dictionary<string, string> attribute { get; set; }

    public virtual void Init(Dictionary<string, string> attribute)
    {
        this.attribute = attribute;
    }
}
public interface ITextEffectTag
{
    string DoTextEffect(string operateString);
}
public interface IGameEffectTag
{
    void DoGameEffect();
}

public class Delete : Tag, ITextEffectTag
{
    public string DoTextEffect(string operateString)
    {
        if (attribute.ContainsKey("match"))
        {
            operateString = operateString.Replace(attribute["match"], "");
            return operateString;
        }
        return "";
    }
}

public class GirlName : Tag, ITextEffectTag
{
    public string DoTextEffect(string operateString)
    {
        return TextEffectManager.Instance.girlName;
    }
}
public class TextEffectManager : MonoBehaviour
{
    public static TextEffectManager Instance;
    public enum TagName
    {
        Default,
        Delete,
        GirlName,
    }
    public string a = "<she starts <tag1>to behavior in </tag1>a pretty wired <tag2>way. However, other people are</tag2> apparently not realizing this.";
    public string girlName;
    

    void Start()
    {
        Instance = this;
        Debug.Log(a);
        Debug.Log(ProcessingTags(a));
    }

    public string ProcessingTags(string sentence)
    {
        var splitSentenceList = Regex.Split(sentence, @"(?=[<])|(?<=[>])").ToList();
        while (_GetPairTag(splitSentenceList,out KeyValuePair<Tag,string> startTag, out KeyValuePair<Tag,string> endTag))
        {
            splitSentenceList = _DoTagBehaviorAndCleanTag(startTag, endTag, splitSentenceList);
            //_PrintString(splitSentenceList);
        }
        return ListToString(splitSentenceList);
    }


    private bool _GetPairTag(List<string> stringList,out KeyValuePair<Tag,string> startTag, out KeyValuePair<Tag,string> endTag)
    {
        for (int i = 0; i < stringList.Count; i++)
        {
            if (!_GetActiveTag(stringList[i], out bool isStart, out Tag tagInstance)) continue;
            if (isStart)
            {
                startTag = new KeyValuePair<Tag, string>(tagInstance,stringList[i]);
                continue;
            }
            //is end
            endTag = new KeyValuePair<Tag, string>(tagInstance,stringList[i]);
            Debug.Assert(endTag.Key.GetType() == startTag.Key.GetType(),"tag "+ endTag.Key + "overlay with tag " + startTag.Key);
            return true;
        }

        return false;
    }
    private bool _GetActiveTag(string testString, out bool isStart, out Tag tagInstance)
    {
        isStart = false;
        tagInstance = null;
        
        if (!testString.StartsWith("<") || !testString.EndsWith(">")) return false;
        
        var tagStr = testString.Substring(1, testString.Length - 2).Trim();
        var tagElement = tagStr.Split(" "[0]).ToList();
        
        var tagName = tagElement[0].Trim();
        tagElement.Remove(tagElement[0]);
        if (tagName.StartsWith("/"))
        {
            tagName = tagName.Substring(1);
            Type type = Type.GetType(tagName);
            Debug.Assert(Type.GetType(tagName) != null && 
                         Type.GetType(tagName).IsSubclassOf(typeof(Tag)), tagName + " is not a defined tag class");
            tagInstance = (Tag) Activator.CreateInstance(Type.GetType(tagName));
            isStart = false;
        }
        else
        {
            var attribute = new Dictionary<string,string>();
            foreach (var element in tagElement)
            {
                element.Trim();
                var pair = element.Split("="[0]);
                Debug.Assert(pair[1].Trim().StartsWith("\"") && pair[1].Trim().EndsWith("\""),pair[1] + " is not a correct input parameter");
                var parameter = pair[1].Trim().Substring(1, pair[1].Trim().Length - 2).Trim();
                if (attribute.ContainsKey(pair[0])) attribute[pair[0]] = parameter;
                else attribute.Add(pair[0].Trim(),parameter);
            }
            Type type = Type.GetType(tagName);
            Debug.Assert(Type.GetType(tagName) != null && 
                         Type.GetType(tagName).IsSubclassOf(typeof(Tag)), tagName + " is not a defined tag class");
            tagInstance = (Tag) Activator.CreateInstance(Type.GetType(tagName));
            tagInstance.Init(attribute);
            isStart = true;
        }

        return true;
    }
    private List<string> _DoTagBehaviorAndCleanTag(KeyValuePair<Tag,string> startTag, KeyValuePair<Tag,string> endTag, List<string> operateString)
    {
        var startPos = operateString.IndexOf(startTag.Value);
        var endPos = operateString.IndexOf(endTag.Value);
        var operateTarget = "";
        for (int i = startPos + 1; i < endPos; i++)
        {
            operateTarget += operateString[i];
            operateString[i] = "";
        }
        if (startTag.Key.GetType().GetInterfaces().Contains(typeof(ITextEffectTag)))
        {
            var textEffectTag = (ITextEffectTag) startTag.Key;
            operateString[startPos+1] = textEffectTag.DoTextEffect(operateTarget);
        }

        if (startTag.Key.GetType().GetInterfaces().Contains(typeof(IGameEffectTag)))
        {
            var textEffectTag = (IGameEffectTag) startTag.Key;
            textEffectTag.DoGameEffect();
        }

        operateString.Remove(startTag.Value);
        operateString.Remove(endTag.Value);
        
        return operateString;
    }
    private void _PrintString(IEnumerable b)
    {
        foreach (var sentence in b)
        {
            Debug.Log(sentence);
        }

    }
    public string ListToString(List<string> list)
    {
        var str = "";
        foreach (var item in list)
            str += item;
        return str;
    }
}
