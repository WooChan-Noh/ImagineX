using System.Collections;
using UnityEngine;

/// <summary>
/// check tag : if frame tag is "RAY", start record
/// check tag : if framne tag is "RAYOFF", end record
/// recording time is fixed -> 20sec
/// if you start record, mesure real record time
/// if real record time<20sec, force end record and cut record sample to real record time 
/// </summary>
public class RecordVoice : MonoBehaviour
{
    private AudioSource myVoice;
    private VoiceAPI voiceAPI;
    private FrameUI FrameUI;

    private string recordDevice;
    private float realRecordTime = 0.0f;
    private bool startTimeCheck = false;//record time flag
    private bool youCanStartRecord = true;//record logic flag
    private bool youCanEndRecord = false;//record logic flag 
    private string startRecordTag = "RAY";
    private string endRecordTag = "RAYOFF";
    private string normalTag = "Untagged";//no matter what tags use (except RAY,RAYOFF,PREVENT)
    private int minRecordTime = 1;
    private int maxRecordTime = 20;//ETRI maximum 20 sec
    private int samplingLate = 16000;//ETRI require

    void Start()
    {
        voiceAPI = GetComponent<VoiceAPI>();
        myVoice = GetComponent<AudioSource>();
        FrameUI = GetComponent<FrameUI>();

        recordDevice = Microphone.devices[0];//PC mic index = 0,  Oculus mic index = 1
    }
    void Update()
    {
        realRecordTime = CheckRecordTime(realRecordTime, startTimeCheck);

        if (realRecordTime >= maxRecordTime)// check max record time
            EndRecord();

        if (youCanStartRecord && (gameObject.tag == startRecordTag))
        {
            CheckAnotherRecord();
        }
        else if (gameObject.tag == endRecordTag)
        {
            EndRecord();
        }

    }
    private float CheckRecordTime(float time, bool flag)//check record time
    {
        if (flag == true)
            time += Time.deltaTime;
        else
            return 0.0f;

        return time;
    }
    private int RoundRecordTime(float recordTime)//round record time(exeption handling)
    {
        return Mathf.RoundToInt(recordTime);
    }
    private void RecordControllFlag()//record logic
    {
        startTimeCheck = !startTimeCheck;
        youCanStartRecord = !youCanStartRecord;
        youCanEndRecord = !youCanEndRecord;
    }
    private void CheckAnotherRecord()//record logic
    {
        if (Microphone.IsRecording(recordDevice))//check mic is enabled(another frame)
        {
            youCanEndRecord = false;
            Debug.Log("You can't start another record");
            return;
        }
        else
        {
            StartRecord();
        }
    }
    void StartRecord()
    {
        Debug.Log("! ! START RECORD ! !");
        Debug.Log($"My Device is : <{recordDevice}>");

        //Start(device name,loop check, time(sec), sampling late) : Check ETRI requirement
        myVoice.clip = Microphone.Start(recordDevice, false, maxRecordTime, samplingLate);

        RecordControllFlag(); //startTimeCheck = true   youCanRecordStart = false   youCanRecordDone = true
    }
    private void EndRecord()
    {
        gameObject.tag = normalTag;

        if (youCanEndRecord == false)//check mic is enabled(another frame)
        {
            Debug.Log("You can't end record");
            //youCanRecordDone = true;//check mic is enabled(same frame)
            return;
        }

        float recordTime = RoundRecordTime(realRecordTime);

        if (recordTime < minRecordTime)//check length
        {
            Microphone.End(recordDevice);
            RecordControllFlag(); //startTimeCheck = false   youCanRecord = true  youCanRecordDone = false      
            Debug.Log("Recording time is too short ! ! Try again ");
            realRecordTime = 0.0f;
            return;
        }

        if (recordTime >= maxRecordTime)
        {
            Debug.Log($"You can record only {maxRecordTime} sec. you exceeded {maxRecordTime} sec. your record maybe truncated");
            realRecordTime = maxRecordTime - 0.01f;
        }
        else
        {
            Microphone.End(recordDevice);
        }

        FrameUI.StartUI();
        
        RecordControllFlag(); //startTimeCheck = false   youCanRecord = true   youCanRecordDone = false    

        //**********Process Start**********//
        voiceAPI.StartREST(myVoice.clip, realRecordTime);
        //**********Process Start**********//

        Debug.Log("RECORD DONE ! !");
        Debug.Log($"Recording time is: {RoundRecordTime(realRecordTime)}"); // recording time check log 

        realRecordTime = 0.0f;
    }
  
    #region PC
    private void OnMouseDown()
    {
        Debug.Log("START RECORD ! !");
        Debug.Log($"My Device is : <{recordDevice}>");

        if (Microphone.IsRecording(recordDevice))//check mic is enabled
        {
            Debug.Log("You can't ");
            return;
        }

        StartRecord();
        startTimeCheck = true;
        youCanStartRecord = false;

    }
    private void OnMouseUp()
    {

        gameObject.tag = "Untagged"; //no matter what tags use (except RAY,RAYOFF)
        startTimeCheck = false;
        youCanStartRecord = true;

        if (Mathf.RoundToInt(realRecordTime) < 1)//check length
        {
            Microphone.End(recordDevice);
            Debug.Log("Recording time is too short ! ! Try again ");
            realRecordTime = 0;
            return;
        }

        Microphone.End(recordDevice);
        StopAllCoroutines();
        voiceAPI.StartREST(myVoice.clip, realRecordTime);

        FrameUI.StartUI();

        Debug.Log("RECORD DONE ! !");
        Debug.Log($"Recording time is: {Mathf.RoundToInt(realRecordTime)}"); // recording time check log 
        realRecordTime = 0;
    }
    #endregion
}


