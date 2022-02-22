using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public DataFetcher fetcher;
    public int wheelPartsCount = 10;
    public int radius = 2;
    public GameObject WheelPartPrefab;
   
    public GameObject parent;
    public AnimationCurve curve;
    public bool isSpining;
    
    public Key currentKey;
    public WheelResultData currentResult;
    public WheelInfosData currentInfos;
    
    public Action<WheelResultData> onWheelSpinEnd;
    public List<WheelPart> parts = new List<WheelPart>();
    
    private void Start()
    {
        // Initiate a first Wheel infos fetch so the user can already Spin at start
        fetcher.GetWheelInfos(ReceivedWheelInfos);
        
        CreateWheel();
    }
    
    // Wheel creation using a parts count and a radius
    private void CreateWheel()
    {
        var angle = 360f / (float)wheelPartsCount;
        
        for (var i = 0; i < wheelPartsCount; i++)
        {
            var rotation = Quaternion.AngleAxis(i * angle, Vector3.up);
            var direction = rotation * Vector3.forward;

            var position = transform.position + direction * radius;
            var o = Instantiate(WheelPartPrefab, position, rotation, parent.transform);
            var part =   o.GetComponent<WheelPart>();
            parts.Add(part);    
            part.SetValueWheelValue(i.ToString());
        }
        
    }
    
    // Assign values in Euros and color if Jackpot for each parts
    private void AssignInfosToWheel()
    {
        var i = 0;
        foreach (var wheelPartData in currentInfos.data)
        {
            parts[i].SetValueEuros(wheelPartData.euros.ToString(),wheelPartData.isJackpot);
            i++;
        }
    }
    
    // Fetch the data before actually spinning the Wheel
    public void TrySpin()
    {
        if(isSpining) return;
        fetcher.GetWheelResult(ReceivedWheelResult,currentKey);
    }
    
    // Fetch new informations for the wheel setup
    public void NextWheel()
    {
        if(isSpining) return;
        fetcher.GetWheelInfos(ReceivedWheelInfos);
    }
    
    // Fetcher Callbacks for infos and result
    private void ReceivedWheelInfos(WheelInfosData obj)
    {
        var k = new Key { key = obj.key };
        currentKey = k;
        currentInfos = obj;
        
        AssignInfosToWheel();
    }
    
    private void ReceivedWheelResult(WheelResultData obj)
    {
        currentResult = obj;
        Spin();
    }
    
    // Actual Spinning of the wheel targeting the result index for final rotation
    private void Spin()
    {
        if (isSpining) return;
        
        var angle = 360f / (float)wheelPartsCount;
        var targetPart = currentResult.index;
        var force = UnityEngine.Random.Range(4, 8);
        var currentAngle = transform.eulerAngles.y;
        var totalAngle = -(targetPart * angle + 360 * force);
        StartCoroutine(Rotate(currentAngle, totalAngle, 5f));
    }

    private IEnumerator Rotate(float from, float to, float duration)
    {
        isSpining = true;
        var elapsed = 0f;
        
        while (elapsed < duration)
        {
            var lerpFactor = curve.Evaluate(elapsed / duration);
            transform.localEulerAngles = new Vector3(0.0f, Mathf.Lerp(from, to, lerpFactor), 0f);
            elapsed += Time.deltaTime;

            yield return null;
        }

        isSpining = false;
        transform.eulerAngles = new Vector3(0.0f, to, 0f);
        onWheelSpinEnd.Invoke(currentResult);
    }
    
}