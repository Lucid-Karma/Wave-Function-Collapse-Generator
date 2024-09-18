using System.Collections;
using UnityEngine;

public class ClaimRewardFx : MonoBehaviour
{
    [SerializeField] private AudioSource rewardAdCompleteFx;
    [SerializeField] private AudioSource coinFx;

    void OnEnable()
    {
        RewardedAds.OnRewardedAdComplete.AddListener(() => StartCoroutine(PlayAdWatchedFx()));
    }
    void OnDisable()
    {
        RewardedAds.OnRewardedAdComplete.RemoveListener(() => StartCoroutine(PlayAdWatchedFx()));
    }

    private IEnumerator PlayAdWatchedFx()
    {
        rewardAdCompleteFx.Play();
        yield return new WaitForSeconds(0.25f);
        coinFx.Play();
    }
}
