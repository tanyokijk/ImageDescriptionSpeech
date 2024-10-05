using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace ImageDescriptionSpeech;

class AzureVisionAnalyzer
{
    private static string subscriptionKey = "c5d19c144d534d8ea53637dc519027d0";
    private static string endpoint = "https://computer-vision-dev-cus.cognitiveservices.azure.com/";



public static async Task<string> AnalyzeImage(string imagePath)
{
    var client = Authenticate(subscriptionKey, endpoint);

    if (!File.Exists(imagePath))
    {
        Console.WriteLine("Файл не знайдено: " + imagePath);
        return null;
    }

    using (Stream imageStream = File.OpenRead(imagePath))
    {
        var analysis = await client.AnalyzeImageInStreamAsync(imageStream, new List<VisualFeatureTypes?>
        {
            VisualFeatureTypes.Description,
            VisualFeatureTypes.Objects
        });

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Image description:");
            foreach (var caption in analysis.Description.Captions)
            {
                stringBuilder.AppendLine($"{caption.Text} (certainty: {caption.Confidence * 100:F2}%)");
            }

            stringBuilder.AppendLine("\nObjects in the image:");
            if (analysis.Objects.Count > 0)
            {
                foreach (var obj in analysis.Objects)
                {
                    stringBuilder.AppendLine($"{obj.ObjectProperty} on the coordinates: {obj.Rectangle.X}, {obj.Rectangle.Y}, {obj.Rectangle.W}, {obj.Rectangle.H}");
                }
            }
            else
            {
                stringBuilder.AppendLine("No objects found.");
            }

            return stringBuilder.ToString();
        }
}


private static ComputerVisionClient Authenticate(string subscriptionKey, string endpoint)
    {
        ComputerVisionClient client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey))
        {
            Endpoint = endpoint
        };
        return client;
    }
}
