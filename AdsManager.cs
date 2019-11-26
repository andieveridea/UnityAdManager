using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class AdsManager : MonoBehaviour, IUnityAdsListener
{

    [SerializeField]
    private string gameId = "1234567";
    [SerializeField]
    private bool testMode = true;

    [SerializeField]
    private float recallAddTimer;

    private float currentAdRecallTimer;

    [SerializeField]
    private bool recallMode;

    private UnityAction rewardingMethod;
    private UnityAction failedMethod;
    private UnityAction connectionLostMethod;
    private UnityAction errorAdsMethod;

    // Initialize the Ads listener and service:
    void Start()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, testMode);
    }

    public void ShowAds(string placementKey)
    {
        if (testMode)
        {
            Debug.Log("Ad Test mode");
            InitiateAdPlacement(placementKey);
        }
        else
        {
            Debug.Log("Ad Publish mode");
            if (!recallMode)
            {
                recallMode = true;
                InitiateAdPlacement(placementKey);
            }
            else
            {
                 //TODO: Add Being recalling ad indicator

            }
        }
    }

    public void InitiateAdPlacement(string placementKey)
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            if (Advertisement.IsReady(placementKey))
            {
                Advertisement.Show(placementKey);
            }
            else
            {
                FailedReward();
            }
        }
        else
        {
            NotConnectToInternet();
        }
    }

    public void SetRewardingMethod(UnityAction yourRewardingMethod)
    {
        this.rewardingMethod = yourRewardingMethod;
    }

    public void SetFailedMethod(UnityAction yourFailedMethod)
    {
        this.failedMethod = yourFailedMethod;
    }

    public void SetConnectionLostMethod(UnityAction yourConnectionLostMethod)
    {
        this.connectionLostMethod = yourConnectionLostMethod;
    }

    public void SetErrorAdsMethod(UnityAction yourErrorAdsMethod)
    {
        this.errorAdsMethod = yourErrorAdsMethod;
    }

    public void SetAllRequirementMethods(UnityAction onRewardingMethod, UnityAction onFailedMethod, UnityAction onConnectionLostMethod, UnityAction onErrorAdsMethod)
    {
        this.rewardingMethod = onRewardingMethod;
        this.failedMethod = onFailedMethod;
        this.connectionLostMethod = onConnectionLostMethod;
        this.errorAdsMethod = onErrorAdsMethod;
    }

    public float GetRecallAdsTimer()
    {
        return this.currentAdRecallTimer;
    }

    private void ClainReward()
    {
        UnityEvent newRewardingEvent = new UnityEvent();
        if (rewardingMethod != null)
        {
            newRewardingEvent.AddListener(rewardingMethod);
            newRewardingEvent.Invoke();
        }
        else
        {
            Debug.Log("You don't have reward function that assign to AdsManager.class.");
        }
    }

    private void FailedReward()
    {
        UnityEvent newFailedReward = new UnityEvent();
        if (rewardingMethod != null)
        {
            newFailedReward.AddListener(failedMethod);
            newFailedReward.Invoke();
        }
        else
        {
            Debug.Log("You don't have failed function that assign to AdsManager.class.");
        }
    }

    private void NotConnectToInternet()
    {
        UnityEvent newConnectionLost = new UnityEvent();
        if (rewardingMethod != null)
        {
            newConnectionLost.AddListener(connectionLostMethod);
            newConnectionLost.Invoke();
        }
        else
        {
            Debug.Log("You don't have internet connection lost function that assign to AdsManager.class.");
        }
    }

    private void ErrorAds()
    {
        UnityEvent newErrorAdsEvent = new UnityEvent();
        if (rewardingMethod != null)
        {
            newErrorAdsEvent.AddListener(errorAdsMethod);
            newErrorAdsEvent.Invoke();
        }
        else
        {
            Debug.Log("You don't have error ads function that assign to AdsManager.class.");
        }
    }

    public void OnUnityAdsReady(string placementId)
    {
        Debug.Log("You ready to see the ads, to get reward.");
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.Log("Unexpectedly error from 3rd party it says : " + message);
        ErrorAds();
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        Debug.Log("Your ad is being see.");
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        switch (showResult)
        {
            case ShowResult.Skipped:
                //TODO: Add condition while player skipped the ads.
                Debug.Log("You did't get reward, because you skipped the ad.");
                FailedReward();
                break;
            case ShowResult.Finished:
                //TODO: Add reward to player when finish the ads.
                ClainReward();
                break;
            case ShowResult.Failed:
                //TODO: Add anouncment to player when ads being failed by conditional states.
                Debug.Log("Fail occure when see the ad.");
                FailedReward();
                break;
        }

    }

    private void Update()
    {
        if (recallMode)
        {
            currentAdRecallTimer += Time.deltaTime;
            if (currentAdRecallTimer >= recallAddTimer)
            {
                currentAdRecallTimer = 0;
                Debug.Log("Ready to see the ad again.");
                recallMode = false;
            }
        }
    }
}
