using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;

/// <summary>
/// My Voice(Float Data : Original) -> My Voice(Short Data) -> My Voice(Byte Data) -> My Voice(Base64 String) ->
/// ETRI(STT) : korean sentence -> Papago(translate) : enlish sentence -> Drawing Start(prompt)
/// </summary>

public class VoiceAPI : MonoBehaviour
{
    private string urlETRI = "http://aiopen.etri.re.kr:8000/WiseASR/Recognition";
    private string accessKeyETRI ;
    private string languageCodeETRI = "korean";

    private string urlPapago = "https://openapi.naver.com/v1/papago/n2mt";
    private string clientIdPapago ;
    private string clientSecretPapago ;

    private string voiceToBase64;//for ETRI
    private string korSTT;//ETRI result
    private string prompt;//Papago result

    private DrawPicture drawPicture; //send a prompt to Drawing Script
    private FrameUI FrameUI;

    private int maxRecordTime = 20;//ETRI maximum 20 sec

    public void StartREST(AudioClip myVoice, float recordTime)
    {
        drawPicture = GetComponent<DrawPicture>();
        FrameUI = GetComponent<FrameUI>();

        voiceToBase64 = VoiceToBase64(myVoice, recordTime);//include nomalize process

        StartCoroutine(RequestPostToETRI());
    }
    private string VoiceToBase64(AudioClip myVoice, float realRecordTime)
    {
        int recordTime = roundRecordTime(realRecordTime);

        if (recordTime < maxRecordTime)//prevent voice truncated
        {
            recordTime = roundRecordTime(realRecordTime) + 1;
        }

        float[] originalVoice = new float[myVoice.samples / maxRecordTime * recordTime * myVoice.channels];

        myVoice.GetData(originalVoice, 0); // [-1.0, 1.0]

        //normalized to short int
        short[] shortVoice = new short[originalVoice.Length];
        float min = float.MaxValue;
        float max = float.MinValue;
        for (int i = 0; i < originalVoice.Length; i++)
        {
            if (originalVoice[i] < min)
                min = originalVoice[i];
            if (originalVoice[i] > max)
                max = originalVoice[i];
        }
        for (int i = 0; i < originalVoice.Length; i++)
        {
            float normalizedValue = (originalVoice[i] - min) / (max - min);
            shortVoice[i] = (short)Math.Round(normalizedValue * 32767); // short MAX = 32767
        }

        //convert voice to Byte
        byte[] byteVoice = new byte[shortVoice.Length * 2]; //short = 2Byte
        int index = 0;
        for (int i = 0; i < shortVoice.Length; i++)
        {
            byte[] bytes = BitConverter.GetBytes(shortVoice[i]);
            byteVoice[index + 0] = bytes[0];
            byteVoice[index + 1] = bytes[1];
            index += 2;
        }

        return Convert.ToBase64String(byteVoice);
    }
    IEnumerator RequestPostToETRI()//
    {
        string requestBody =
            $"{{\"argument\": " +
                $"{{\"language_code\": \"{languageCodeETRI}\"," +
                  $"\"audio\": \"{voiceToBase64}\"}}" +
            $"}}";

        UnityWebRequest request = new UnityWebRequest(urlETRI, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", accessKeyETRI);


        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ResponseDataSTT dataSTT = JsonUtility.FromJson<ResponseDataSTT>(request.downloadHandler.text);
            korSTT = dataSTT.return_object.recognized;

            if (korSTT == "")//check empty sentence
            {
                Debug.Log("Empty Sentence ! try again");
                FrameUI.EndUI();
                request.Dispose();
                yield break;
            }

            Debug.Log(korSTT);//check korean sentence
            //**************Translate Start***************//
            StartCoroutine(RequestPostToPapago());
            //**************Translate Start***************//
        }
        else
        {
            Debug.Log("Error: " + request.error);
        }
        request.Dispose();
    }
    IEnumerator RequestPostToPapago()
    {
        string query = korSTT;
        string requestUrl = $"{urlPapago}?source=ko&target=en&text={UnityWebRequest.EscapeURL(query)}";

        UnityWebRequest request = UnityWebRequest.PostWwwForm(requestUrl, "");

        request.SetRequestHeader("X-Naver-Client-Id", clientIdPapago);
        request.SetRequestHeader("X-Naver-Client-Secret", clientSecretPapago);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ResponsDataPapago dataPapago = JsonUtility.FromJson<ResponsDataPapago>(request.downloadHandler.text);
            Result resultPapago = dataPapago.message.result;

            prompt = resultPapago.translatedText;
            //Debug.Log(translateText);//check english sentence

            /*****************Drawing Start***************/
            drawPicture.DrawPictures(drawPicture.Base64ToPicture, prompt);
            /*****************Drawing Start***************/

        }
        else
        {
            Debug.LogError($"Error: {request.error}");
        }
        request.Dispose();
    }
    private int roundRecordTime(float recordTime)
    {
        return Mathf.RoundToInt(recordTime);
    }

}
#region JSON STRUCT
//ETRI(STT) JSON STRUCT
[System.Serializable]
public class ResponseDataSTT
{
    public int result;
    public string return_type;
    public ReturnObject return_object;
}
[System.Serializable]
public class ReturnObject
{
    public string recognized;
}
//Papago JSON STRUCT
[System.Serializable]
public class ResponsDataPapago
{
    public Message message;
}
[System.Serializable]
public class Message
{
    public Result result;
}
[System.Serializable]
public class Result
{
    public string srcLangType;
    public string tarLangType;
    public string translatedText;
}
#endregion
