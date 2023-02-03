using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] protected Image barFillImage;
    [SerializeField] protected float maxValue = 100;
    [SerializeField] protected bool enableDebug;

    // Set MaxValue to change the max fill amount the bar can go.
    public float MaxValue { get { return maxValue; } set { maxValue = value; } }

    private float currentVal;
    public float CurrentVal
    {
        get { return currentVal; }

        /* 
         * Use this setter to change the bar fill amount. 
         * Code example : healthUI.CurrentVal -= damages ; 
         */
        set
        {
            currentVal = Mathf.Clamp(value, 0, MaxValue);
            UpdateValue();
        }
    }

	protected virtual void Start ()
    {
        CurrentVal = MaxValue;        
	}

    protected virtual void UpdateValue()
    {
        barFillImage.fillAmount = currentVal / maxValue;
	}
}
