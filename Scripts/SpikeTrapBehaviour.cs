using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapBehaviour : MonoBehaviour
{
    float waitTime;
    WaitForSecondsRealtime waitForSecondsRealtime;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("repeatingSpikes", 0f, 3.7f);
    }

    IEnumerator Spikes()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        gameObject.GetChild("SpikesFrame1").SetActive(false);
        gameObject.GetChild("SpikesFrame2").SetActive(true);

        yield return new WaitForSecondsRealtime(0.01f);
        gameObject.GetChild("SpikesFrame2").SetActive(false);
        gameObject.GetChild("SpikesFrame3").SetActive(true);

        yield return new WaitForSecondsRealtime(0.01f);
        gameObject.GetChild("SpikesFrame3").SetActive(false);
        gameObject.GetChild("SpikesFrame4").SetActive(true);

        yield return new WaitForSecondsRealtime(1.5f);
        gameObject.GetChild("SpikesFrame4").SetActive(false);
        gameObject.GetChild("SpikesFrame3").SetActive(true);

        yield return new WaitForSecondsRealtime(0.3f);
        gameObject.GetChild("SpikesFrame3").SetActive(false);
        gameObject.GetChild("SpikesFrame2").SetActive(true);

        yield return new WaitForSecondsRealtime(0.3f);
        gameObject.GetChild("SpikesFrame2").SetActive(false);
        gameObject.GetChild("SpikesFrame1").SetActive(true);
    }
    void repeatingSpikes()
    {
        StartCoroutine(Spikes());
    }
}
