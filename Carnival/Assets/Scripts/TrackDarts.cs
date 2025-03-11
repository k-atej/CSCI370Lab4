using TMPro;
using UnityEngine;

public class TrackDarts : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    // this should keep track of how many darts have been thrown & show it on the UI

    public TMP_Text dartCounter;

    private int count;

    void Start()
    {
        dartCounter.SetText("0");
        count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Increment(){
        Debug.Log("called increment");
        count += 1;
        dartCounter.SetText(count.ToString());
    }


}