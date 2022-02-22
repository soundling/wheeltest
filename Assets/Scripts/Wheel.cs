using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Wheel : MonoBehaviour
{
    public DataFetcher fetcher;
    
    [SerializeField] private int wheelPartsCount = 10;
    [SerializeField] private int radius = 2;
    private bool _isSpinning;

    [SerializeField] private GameObject wheelPartPrefab;
    [SerializeField] private   GameObject parent;
    
    public AnimationCurve curve;

    private Key _currentKey;
    private WheelResultData _currentResult;
    private WheelInfosData _currentInfos;
    
    public Action<WheelResultData> onWheelSpinEnd;
    private List<WheelPart> _parts = new List<WheelPart>();
    
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
            var o = Instantiate(wheelPartPrefab, position, rotation, parent.transform);
            var part =   o.GetComponent<WheelPart>();
            _parts.Add(part);    
            part.SetValueWheelValue(i.ToString());
        }
    }
    
    // Assign values in Euros and color if Jackpot for each parts
    private void AssignInfosToWheel()
    {
        var i = 0;
        foreach (var wheelPartData in _currentInfos.data)
        {
            _parts[i].SetValueEuros(wheelPartData.euros.ToString(),wheelPartData.isJackpot);
            i++;
        }
    }
    
    // Fetch the data before actually spinning the Wheel
    public void TrySpin()
    {
        if(_isSpinning) return;
        fetcher.GetWheelResult(ReceivedWheelResult,_currentKey);
    }
    
    // Fetch new informations for the wheel setup
    public void NextWheel()
    {
        if(_isSpinning) return;
        fetcher.GetWheelInfos(ReceivedWheelInfos);
    }
    
    // Fetcher Callbacks for infos and result
    private void ReceivedWheelInfos(WheelInfosData obj)
    {
        var k = new Key { key = obj.key };
        _currentKey = k;
        _currentInfos = obj;
        
        AssignInfosToWheel();
    }
    
    private void ReceivedWheelResult(WheelResultData obj)
    {
        _currentResult = obj;
        Spin();
    }
    
    // Actual Spinning of the wheel targeting the result index for final rotation
    private void Spin()
    {
        if (_isSpinning) return;
        
        var angle = 360f / (float)wheelPartsCount;
        var targetPart = _currentResult.index;
        var force = UnityEngine.Random.Range(4, 8);
        var currentAngle = transform.eulerAngles.y;
        var totalAngle = -(targetPart * angle + 360 * force);
        StartCoroutine(Rotate(currentAngle, totalAngle, 5f));
    }

    private IEnumerator Rotate(float from, float to, float duration)
    {
        _isSpinning = true;
        var elapsed = 0f;
        
        while (elapsed < duration)
        {
            var lerpFactor = curve.Evaluate(elapsed / duration);
            transform.localEulerAngles = new Vector3(0.0f, Mathf.Lerp(from, to, lerpFactor), 0f);
            elapsed += Time.deltaTime;

            yield return null;
        }

        _isSpinning = false;
        transform.eulerAngles = new Vector3(0.0f, to, 0f);
        onWheelSpinEnd.Invoke(_currentResult);
    }
    
}