using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

public static class TempDataExtensions
{
    public static void AddErrorMessage(this ITempDataDictionary tempData, string message)
    {
        var existingErrors = tempData.ContainsKey("ErrorMessages")
            ? JsonConvert.DeserializeObject<List<string>>(tempData["ErrorMessages"].ToString())
            : new List<string>();

        existingErrors.Add(message);
        tempData["ErrorMessages"] = JsonConvert.SerializeObject(existingErrors);
    }

    public static List<string> GetErrorMessages(this ITempDataDictionary tempData)
    {
        if (!tempData.ContainsKey("ErrorMessages")) return new List<string>();
        return JsonConvert.DeserializeObject<List<string>>(tempData["ErrorMessages"].ToString());
    }
}

