using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SDORequest : MonoBehaviour
{
    private string url = "https://sdo24.1580.ru/mod/quiz/processattempt.php";

    public string allAnswerHtml;
    public string qestion;

    // Параметры запроса
    public Dictionary<string, string> parameters = new Dictionary<string, string>
    {
        { "cmid", "3419" },//<<<change
        { "_:flagged", "0" },    //q91428
        { "_:sequencecheck", "1" },// <<<fack
        { "q_answer", "" },
        { "_-submit", "1" },
        { "attempt", "87128" },//<<<change
        { "thispage", "0" },
        { "nextpage", "1" },
        { "timeup", "0" },
        { "sesskey", "1Dho32kjaD" },//<<<change
        { "mdlscrollto", "415" },  
        { "slots", "1" }
    };

    // Заголовки запроса
    public Dictionary<string, string> headers = new Dictionary<string, string>
    {
       // { "accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7" },
      //  { "accept-encoding", "gzip, deflate, br, zstd" },
      //  { "accept-eanguage", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7" },
       // { "cache-control", "max-age=0" },
       // { "content-type", "multipart/form-data; boundary=----WebKitFormBoundaryv7Rw2cstzRBRNKX5" },
        { "cookie", "MoodleSession=0rjbijudgh69k3ocj9qto2nkdq; MOODLEID1_=%25F4%25E1%25C1%25FDd2B%25D5%2525c" },
       // { "origin", "https://sdo23.1580.ru" },
       // { "priority", "u=0, i" },
       // { "referer", "https://sdo23.1580.ru/mod/quiz/attempt.php?attempt=87109&cmid=5193&page=14&mdlscrollto=208" },
      //  { "Sec-CH-UA", "\"Not)A;Brand\";v=\"99\", \"Google Chrome\";v=\"127\", \"Chromium\";v=\"127\"" },
      //  { "Sec-CH-UA-Mobile", "?0" },
      //  { "Sec-CH-UA-Platform", "\"Windows\"" },
      //  { "Sec-Fetch-Dest", "document" },
      //  { "Sec-Fetch-Mode", "navigate" },
      //  { "Sec-Fetch-Site", "same-origin" },
     //   { "Sec-Fetch-User", "?1" },
     //   { "upgrade-insecure-requests", "1" },
     //   { "user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.0.0 Safari/537.36" }
    };

    //void Start()
    //{
    //    SendReqest();
    //}

    public void SendReqest()
    {


        StartCoroutine(SendPostRequest());
       // StartCoroutine(SendGetRequest());
    }

    private IEnumerator SendPostRequest()
    {
        WWWForm form = new WWWForm();
        foreach (var param in parameters)
        {
            form.AddField(param.Key, param.Value);
        }

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            foreach (var header in headers)
            {
                www.SetRequestHeader(header.Key, header.Value);
            }

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("Error: " + www.error);
                Debug.Log("Response: " + www.downloadHandler.text);
            }
            else
            {
                Debug.Log("Response: " + www.downloadHandler.text);
                qestion = TrimString(www.downloadHandler.text);
                allAnswerHtml = www.downloadHandler.text;
                Debug.Log(qestion);
            }
        }
    }

    private string TrimString(string input)          // обрезание html ответа для примерного запроса gpt
    {
        int startIndex = input.IndexOf("Текст вопроса");
        int endIndex = input.IndexOf("штрафной");


        if (startIndex != -1 && endIndex != -1)
        {

            return input.Substring(startIndex, endIndex - startIndex).Trim();
        }


        return input;
    }


      private IEnumerator SendGetRequest()
      {
          // Формирование URL с параметрами
          string url = $"https://sdo23.1580.ru/mod/quiz/attempt.php?attempt={parameters["attempt"]}&cmid={parameters["cmid"]}&page={parameters["thispage"]}&mdlscrollto={parameters["mdlscrollto"]}";

          // Создание запроса
          UnityWebRequest request = UnityWebRequest.Get(url);

          // Добавление заголовка cookie
          request.SetRequestHeader("Cookie", $"MoodleSession={headers["cookie"]}");

          // Отправка запроса и ожидание ответа
          yield return request.SendWebRequest();

          // Проверка на ошибки
          if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
          {
              Debug.LogWarning($"GET:::::::::::Request error: {request.error}");
          }
          else
          {
              // Вывод ответа на консоль
              Debug.Log($"GET:::::::::Response: {request.downloadHandler.text}");
          }
      }
}
