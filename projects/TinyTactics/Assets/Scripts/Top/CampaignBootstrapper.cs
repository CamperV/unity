using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CampaignBootstrapper : MonoBehaviour
{
    public Campaign campaignPrefab;
    //
    public void CreateNewCampaign() => Instantiate(campaignPrefab);
    public void StartActiveCampaign() => Campaign.active.BeginLevelSequence();

    public void TriggerNextCampaignEvent() {
        Campaign.active.waitForBootstrapper = true;
    }

    public void DestroyCampaign() {
        if (Campaign.active != null) Destroy(Campaign.active.gameObject);
    }
}