using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Net.Mail;
using UnityEngine.Windows;

public class GPTRequest : MonoBehaviour
{
    private string url = "https://vip.easychat.work/api/openai/v1/chat/completions";   //для получения токена капчи сайт: https://vip.easychat.work/

    // Параметры запроса
    public string captchaToken = "P1_eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.hadwYXNza2V5xQUXWMCDNunZjCHjpwmQS1RN2c9UN5N-tup9FkPi8C_nQXZz1_oiD9wicatFi4lnwIQhmZGD7avq5mybHd3YinNEyBCox7JAVQkXh-FqCyiSgxqiMdIeQsaYRxLDWAsCbccrMvMvG1kHoHNQmVVQPmiEmS5KjhUDtYd0s2oWsWb9-0qfMkOtJdIpc9S8kaPM2glMJ8xbMFllsXQgsxBkP5MJDeWE6iXWe14oSJgUHeEGSspQ3BootGaznr97unMcSEAbu1FssxeGIDTkF3yysPfkudX5qM8K1Zpd2VPAcyBz25aMTi3JIh0mikwRXfrY71d3pVufI7iO2HoSxRISrFjlgkcoDwMlhTFd23SpCAE0n4jWYD9IaFMgBSlG1scsmLYlolRiqCF4d9XT0VGPcXA9HUlmX8SDjF0Uvud4S1QxfTAxpmg3xnjJn799WpJINci2wyVuMt1n8GOATNWWJ1D5vVUrSsgVk0U_Gxom71JytyurNLWq9q1pOOo--VOFb6Te75ZebnyIhajKDeCYXTMsfF95KAqK_Z4XDqthk7q8e6tw-59Dh-CzmIMqnfy1cvvSv4YDrisUfBpGpZsQKarj70f0CsmfZHpzjfl-iwSWlhoCLP4T0lqICSJUk-qlnT8JYHYOH9xsh4mqs6KkLVAur5fz9pkgTvsOaDSivB1P6rERIbGT7T33Y4TC4HfxIAEbHQBXx4AmBozzG54b2jHwCWs4DI2lKfryq7dyPV42P3vdj-EHIb53CsuQzZ4aVQF14X5qb-HBAn_dLJllwkvC00-qsSpKjVBKA-1nV1sSLJnLtWE9QP2Tval5waPpH3BdSPDTAT2ZzX--RAJiZjl5s2RDohHEUnrmvCKJB81KJ4sUODQIkcGrkfAiXNwQnzWCvPVviId5RPrLHol-d3Dh9cHLWHBViHrjZUSZyprSJYZ0C_u-tFMGV92Y0bvkdMPqnqLKqo1zfIiagzmYrTUgPbrDeXAdGPtflM3HJPd0F3hsGAPFp1ye-OCasSV7-sjR99W4lR3utyhzlgaZcMalym5vjXQqzUc7H7YNWNHl3j0k4T2AYaOBtFqFck_Udh7vlwXSUp4JPx_NY7l9CQyA8T1jm_x75A8_zTKPNw3GlKKQm5WUCiOf-3FfhOZPV3_D2HCpOug5RcJsin3BcAC5irVi_Bs0ewXXxSiiqgKEGm6VYj_133P_R6ouduct-Ap0xEeLx86ytV78NJWVlRnO-nnA0Kxqs9rPmiLHFyiaOq_cRe9P49-IdyFYOCksCp02hsGQdISFcXNvGpNjsY-mOyZ23gkaeaZvW3O3G_-rZnS37TL2jzCLIjaBKzxlxisF8ssFQkH27Bz1T986uaDbVx6XaJ7Wpqe6pBdckRBHZd9tAjonvEKO7uo8whgfxIP5RU5VC30IMQ2wwH8rsUrhX1rCPftCSYH4p1CpADeMceWjceefGC9BehRJZ8G7wvfh7pM0TkdgKsmBrBDvPZTuEf8rjnaymKE_hNynu8o3rjcXSBxnukq5H79VWNIvpKuzhq9bBIDC8kZ3vJKTdPSo3syeSWIS4gm3ga9ojhZj4lRHBZu-6-ES7TicVFWKEnOgF0Rwl5E6JkAyU49bRguTlcgwqGZ1nmyJbg_ulJkxxDJblZOan-xnW37fpI4z_LhygH-8jW8mzw3Ol8tmIq0goGbh6aQ97iLxGlwAPECZxhGn4qYs6F-6MVOxPtNpu3NII4ogKhAjdaNleHDOZsWm36hzaGFyZF9pZM4UPIQfomtyqDFhMWVjMGEyonBkAA.ovNLTiHluil6Xh2lPtEEo2x60DJVBhK5mNdze_rC0P4";
    public string userQuestion = "У пети было 2 яблока, вова сьел у него 1 яблоко, сколько яблок осталось у пети?";

    public string assistantResponse;  

    //private void Start()
    //{
    //    SendReqest();
    //}

    public void SendReqest()
    {
        StartCoroutine(SendPostRequest(userQuestion, captchaToken));
    }

    private IEnumerator SendPostRequest(string question, string captcha)
    {
        
        string jsonBody = JsonUtility.ToJson(new RequestData
        {
            messages = new Message[]
            {
                new Message { role = "system", content = "\nCurrent model: gpt-4o-mini\nCurrent time: 20.08.2024, 22:15:37\nLatex inline: $ x^2 $ \nLatex block: $$ e=mc^2 $$\n\n" },
                new Message { role = "user", content = question }
            },
            stream = false,
            model = "gpt-4o-mini",
            temperature = 0.5f,
            presence_penalty = 0,
            frequency_penalty = 0,
            top_p = 1,
            chat_token = 66,
            captchaToken = captcha
        });

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            
            yield return www.SendWebRequest();

           
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("Error: " + www.error);              
            }
            else
            {
                
                ResponseData responseData = JsonUtility.FromJson<ResponseData>(www.downloadHandler.text);

                string cleaned = responseData.choices[0].message.content.Replace("```", "").Replace("python", "").Trim();

                assistantResponse = cleaned;
                Debug.Log("Assistant Response: " + assistantResponse);
            }
        }
    }

  
    [System.Serializable]
    public class RequestData
    {
        public Message[] messages;
        public bool stream;
        public string model;
        public float temperature;
        public float presence_penalty;
        public float frequency_penalty;
        public float top_p;
        public int chat_token;
        public string captchaToken;
    }

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    public class ResponseData
    {
        public string id;
        public string @object;
        public int created;
        public string model;
        public Choice[] choices;
        public Usage usage;
    }

    [System.Serializable]
    public class Choice
    {
        public int index;
        public Message message;
    }

    [System.Serializable]
    public class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }
}


/*
 * https://api.deepinfra.com/v1/openai/chat/completion
 * 
 * 
 * {
    "model": "meta-llama/Meta-Llama-3.1-70B-Instruct",
    "messages": [
        {
            "role": "system",
            "content": "Be a helpful assistant"
        },
        {
            "role": "user",
            "content": "с новым годом"
        }
    ],
    "stream": false
}


answer:
{
    "id": "chatcmpl-6b9712dfde094d2b9620e1eedacb2d88",
    "object": "chat.completion",
    "created": 1724419648,
    "model": "meta-llama/Meta-Llama-3.1-70B-Instruct",
    "choices": [
        {
            "index": 0,
            "message": {
                "role": "assistant",
                "content": "С Новым Годом! Пусть этот год принесет вам радость, счастье, успех и много новых возможностей!",
                "name": null,
                "tool_calls": []
            },
            "finish_reason": "stop"
        }
    ],
    "usage": {
        "prompt_tokens": 24,
        "total_tokens": 56,
        "completion_tokens": 32,
        "estimated_cost": 3.6479999999999996e-05
    }
}
 
 
 */
