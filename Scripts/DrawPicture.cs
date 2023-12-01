using UnityEngine;
using System;
using System.IO;
using ImageMagick;

/// <summary>
/// Image(Base64) -> Decompress -> Draw
/// </summary>
public class DrawPicture : MonoBehaviour
{
    private DrawAPI drawAPI;
    private GameObject thisFrame;
    private FrameUI FrameUI;

    private int imageSizeWidth = 1024;
    private int imageSizeHeight = 1024;
    private string shaderName = "Standard";

    public void DrawPictures(Action callback, string prompt)
    {
        drawAPI = GetComponent<DrawAPI>();
        FrameUI = GetComponent<FrameUI>();
        thisFrame = gameObject;

        drawAPI.DrawComplete += callback;
        drawAPI.StartDraw(prompt);
    }
    public void Base64ToPicture()//Karlo result(string) to Picture
    {
        byte[] imageToByte = Convert.FromBase64String(drawAPI.imageToBase64);
        Texture2D drawImage = new Texture2D(imageSizeWidth, imageSizeHeight, TextureFormat.RGB24, false);

        drawImage.LoadImage(PreprocessImage(imageToByte));
        drawImage.Apply();

        PictureToMeterials(drawImage);
    }

    public byte[] PreprocessImage(byte[] imageData)
    {
        MemoryStream stream = new MemoryStream(imageData);

        stream.Position = 0;
        byte[] buffer = new byte[stream.Length];
        stream.Read(buffer, 0, buffer.Length);

        return buffer;
    }
    private void PictureToMeterials(Texture2D karloImage)
    {
        if (karloImage != null)
        {
            //you can't direct access to drawImage materials
            Material[] myKarloPaintMaterial = thisFrame.GetComponent<Renderer>().materials;

            Material tempMaterial = new Material(Shader.Find(shaderName));
            tempMaterial.mainTexture = karloImage;

            //***CHECK METERIAL ORDER***//
            if (gameObject.scene.name == "02-5_TreeGallery")
            {
                if (this.transform.parent.name == "Museum03_WallDisplay01_Painting"
                 || this.transform.parent.name == "Museum03_WallDisplay05_Painting")
                {
                    myKarloPaintMaterial[1] = tempMaterial;
                }
                else if (this.transform.parent.name == "Museum03_WallDisplay02_Painting")
                {
                    switch (this.name)
                    {
                        case "Museum03_WallDisplay02_Painting_01":
                        case "Museum03_WallDisplay02_Painting_02":
                        case "Museum03_WallDisplay02_Painting_03":
                        case "Museum03_WallDisplay02_Painting_04":
                        case "Museum03_WallDisplay02_Painting_05":
                            myKarloPaintMaterial[0] = tempMaterial;
                            break;
                        case "Museum03_WallDisplay02_Painting_06":
                        case "Museum03_WallDisplay02_Painting_07":
                        case "Museum03_WallDisplay02_Painting_08":
                        case "Museum03_WallDisplay02_Painting_09":
                        case "Museum03_WallDisplay02_Painting_10":
                            myKarloPaintMaterial[1] = tempMaterial;
                            break;
                        default:
                            Debug.Log("Error in DrawPicture_picrtureToMeterials_02-5_TreeGallery");
                            break;
                    }
                }
                else if (this.transform.parent.name == "Museum03_WallDisplay03_Painting")
                {
                    switch (this.name)
                    {
                        case "Museum03_WallDisplay03_Painting01":
                        case "Museum03_WallDisplay03_Painting02":
                        case "Museum03_WallDisplay03_Painting03":
                        case "Museum03_WallDisplay03_Painting04":
                        case "Museum03_WallDisplay03_Painting05":
                        case "Museum03_WallDisplay03_Painting06":
                        case "Museum03_WallDisplay03_Painting07":
                        case "Museum03_WallDisplay03_Painting08":
                            myKarloPaintMaterial[1] = tempMaterial;
                            break;
                        case "Museum03_WallDisplay03_Painting09":
                        case "Museum03_WallDisplay03_Painting10":
                        case "Museum03_WallDisplay03_Painting11":
                            myKarloPaintMaterial[0] = tempMaterial;
                            break;
                        default:
                            Debug.Log("Error in DrawPicture_picrtureToMeterials_02-5_TreeGallery");
                            break;
                    }
                }
                else if (this.transform.parent.name == "Museum03_WallDisplay04_Painting")
                {
                    switch (this.name)
                    {
                        case "Museum03_WallDisplay04_Painting01":
                        case "Museum03_WallDisplay04_Painting02":
                        case "Museum03_WallDisplay04_Painting03":
                        case "Museum03_WallDisplay04_Painting04":
                        case "Museum03_WallDisplay04_Painting05":
                        case "Museum03_WallDisplay04_Painting06":
                        case "Museum03_WallDisplay04_Painting07":
                            myKarloPaintMaterial[1] = tempMaterial;
                            break;
                        case "Museum03_WallDisplay04_Painting08":
                        case "Museum03_WallDisplay04_Painting09":
                        case "Museum03_WallDisplay04_Painting10":
                        case "Museum03_WallDisplay04_Painting11":
                        case "Museum03_WallDisplay04_Painting12":
                        case "Museum03_WallDisplay04_Painting13":
                        case "Museum03_WallDisplay04_Painting14":
                            myKarloPaintMaterial[0] = tempMaterial;
                            break;
                        default:
                            Debug.Log("Error in DrawPicture_picrtureToMeterials_02-5_TreeGallery");
                            break;
                    }
                }
            }

            switch (gameObject.scene.name)
            {
                case "02-2_GlassGallery":
                case "02-3_OperaGallery":
                case "02-4_RedGallery":
                    myKarloPaintMaterial[0] = tempMaterial;
                    if (this.transform.parent.parent.name == "Small Frames")
                    {
                        myKarloPaintMaterial[0].mainTextureScale = new Vector2(1.009f, 1.55f);
                        myKarloPaintMaterial[0].mainTextureOffset = new Vector2(1.0f, 0.46f);
                    }
                    else
                    {
                        myKarloPaintMaterial[0].mainTextureScale = new Vector2(1.058f, 1.058f);
                        myKarloPaintMaterial[0].mainTextureOffset = new Vector2(0.973f, 0.97f);
                    }
                    break;
                default:
                    Debug.Log("Error in DrawPicture_picrtureToMeterials_CheckSceneName");
                    break;
            }
            //***CHECK METERIAL ORDER***//

            thisFrame.GetComponent<Renderer>().materials = myKarloPaintMaterial;

            FrameUI.EndUI();
        }
    }
}


