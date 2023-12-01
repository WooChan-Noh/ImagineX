using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Threading.Tasks;
/// <summary>
/// Prompt -> Karlo Drawing -> Karlo Protocol(Base64)
/// </summary>
public class DrawAPI : MonoBehaviour
{
    private string karloKey;
    private string karloURL = "https://api.kakaobrain.com/v2/inference/karlo/t2i";

    //***************************************Karlo PARAMETERS*********************************************//

    private string prompt;
    private string negativePrompt = "nigga";//exclude words
    private string upscale = "true";//check zoom (default:false)
    private string nsfwChecker = "true";//NSFW chcek (dafault:false)
    private string imageFormat = "png";//webp, jpeg, png
    private string returnType = "base64_string";//base64, url
    private string scheduler = "decoder_ddim_v_prediction";
    //decoder_ddim_v_prediction or decoder_ddpm_v_prediction
    private double priorGuidanceScale = 5.0;//denoise measures : 1.0~20.0
    private double guidanceScale = 5.0;//decoder denoise measures : 1.0~20.0
    private int width = 512;//384~640 (multiple of 8)
    private int height = 512;//384~640 (multiple of 8)
    private int scale = 2;//zoom ratio (2 or 4) (max : 2048)
    private int imageQuality = 70;//1~100
    private int priorNumInferenceSteps = 25;//denoise step : 10~100
    private int numInferenceSteps = 50;//decoder denoise step : 10~100
    private int samples = 1;//image num 1~8
    private int[] seed = null;//if not use seed[], random seed (default:null)
                              //if use seed[], seed[] length must be samples num.
                              //if use same seed and same parameter, always draw same picture

    //***************************************Karlo PARAMETERS*********************************************//

    public string imageToBase64;// use this variable in DrawPicture.cs
    private int promptMaxLength = 255;//karlo max 256

    public event Action DrawComplete;

    public async void StartDraw(string longPrompt)
    {
        prompt = CheckPromptLength(longPrompt);

        await RequestPostToDrawing();
    }
    private string CheckPromptLength(string longPrompt)
    {
        if (longPrompt.Length > promptMaxLength)//check length
        {
            longPrompt = longPrompt.Substring(0, promptMaxLength);
            Debug.Log("Too Long ! ! your prompt was trucated ! !");
        }

        return longPrompt;
    }

    async Task RequestPostToDrawing()
    {
        UnityWebRequest request = new UnityWebRequest(karloURL, "POST");

        string seedParameter;
        if (seed != null)//JSON format 
        {
            seedParameter = $"\"seed\": {seed},";
        }
        else
        {
            seedParameter = $"\"seed\": null,";
        }

        string requestBody =
            $"{{" +
                $"\"prompt\": \"{prompt}\"," +
                $"\"negative_prompt\": \"{negativePrompt}\"," +
                $"\"width\": {width}," +
                $"\"height\": {height}," +
                $"\"upscale\": \"{upscale}\"," +
                $"\"scale\": {scale}," +
                $"\"image_format\": \"{imageFormat}\"," +
                $"\"image_quality\": {imageQuality}," +
                $"\"samples\": {samples}," +
                $"\"return_type\": \"{returnType}\"," +
                $"\"prior_num_inference_steps\": {priorNumInferenceSteps}," +
                $"\"prior_guidance_scale\": {priorGuidanceScale}," +
                $"\"num_inference_steps\": {numInferenceSteps}," +
                $"\"guidance_scale\": {guidanceScale}," +
                $"\"scheduler\": \"{scheduler}\"," +
                seedParameter +
                $"\"nsfw_checker\": \"{nsfwChecker}\"" +
            $"}}";

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Authorization", $"KakaoAK {karloKey}");
        request.SetRequestHeader("Content-Type", "application/json");

        var asyncOperation = request.SendWebRequest();

        while (!asyncOperation.isDone)
        {
            await Task.Yield();
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseBase64 = request.downloadHandler.text;
            ResponseData response = JsonUtility.FromJson<ResponseData>(responseBase64);

            // if imageNum > 1, change this part
            if (response.images.Length > 0)
            {
                imageToBase64 = response.images[0].image;
                DrawComplete.Invoke();
            }
            else
            {
                Debug.Log("Error: " + request.error);
            }
        }
        else
        {
            Debug.LogError($"Error: {request.error}");
        }
        request.Dispose();
    }
}

//JSON STRUCT

[System.Serializable]
public class ResponseData
{
    public string id;//work ID
    public string model_version;
    public Image[] images;//image info 
    [System.Serializable]
    public class Image
    {
        public string id;//image ID
        public int seed;//image seed. use Re-request. make same picture
        public string image;//base64 or URL(URL access time: 10 min)
        public bool nsfw_content_detected;//check image has NSFW content
        public double nsfw_score;//NSFW content probability
    }
}


