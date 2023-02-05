using System.Collections;
using UnityEngine;

public class ProgressBarCooldown : ProgressBar
{
    /* 
     * The fill amount of ProgressBarCooldown is in second.
     */

    private bool isInCooldown;
    public bool IsInCooldown => isInCooldown;

    protected override void Start()
    {
        ResetCooldown();
    }

    /* Use this to start the cooldown */
    public void StartCooldown()
    {
        if(!IsInCooldown)
        {
            CurrentVal = MaxValue;

            //barFillImage.enabled = true;
            isInCooldown = true;

            StartCoroutine(StartTimerCooldownCoroutine());
        }
    }

    public void ResetCooldown()
    {
        CurrentVal = 0;

        //barFillImage.enabled = false;
        isInCooldown = false;
    }

    private IEnumerator StartTimerCooldownCoroutine()
    {
        while(CurrentVal > 0)
        {
            CurrentVal -= Time.deltaTime;

            if (CurrentVal <= 0)
            {
                ResetCooldown();
            }

            yield return null;
        }
    }

    protected override void UpdateValue()
    {
        barFillImage.fillAmount = Mathf.Lerp(1, 0, currentVal / maxValue);
    }
}
