using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Redirection : MonoBehaviour
{
    private SDORequest sDORequest;
    private GPTRequest gPTRequest;

    public int index;
    public int check = 1;

    public string qNumber;
    public string cmid;
    public string attempt;
    public string sesskey;
    public string cookie;    //сдо

    [SerializeField] private TMP_InputField _qNumInput, _cmidInput, _attemptInput, _sesskeyInput, _cookieInput, _indexInput;

    public void SaveInput()
    {
        qNumber = _qNumInput.text;
        cmid = _cmidInput.text;
        attempt = _attemptInput.text;
        sesskey = _sesskeyInput.text;
        cookie = _cookieInput.text;
        if (_indexInput.text != "")
        {
            index = Convert.ToInt32(_indexInput.text);
        }
    }

    public void StartWork()
    {
        Application.targetFrameRate = 40;
        sDORequest = GetComponent<SDORequest>();
        gPTRequest = GetComponent<GPTRequest>();
        StartCoroutine(TestCoroutine());
    }

    private IEnumerator TestCoroutine()
    {
        while (true)
        {
            UpdateParameters(index.ToString());

            var newPrefix = $"{qNumber}:{index.ToString()}";
            sDORequest.parameters.Remove($"{newPrefix.ToString()}_:sequencecheck");
             
            sDORequest.SendReqest();   //ради условия

            sDORequest.parameters.Add($"{newPrefix}_:sequencecheck", check.ToString());

            yield return new WaitForSeconds(5);
            Debug.Log("Вопрос: " + sDORequest.qestion);

            gPTRequest.userQuestion = "представь себя программистом и реши задачу на python и пришли мне сообщение только с кодом, в задаче условие в перемешку с html, отсеки его и читай только условие (данные даннные в примере не должны быть в коде и колжны получаться с помощью input():" + sDORequest.qestion;
            gPTRequest.SendReqest();           // запрос чату
            yield return new WaitForSeconds(20);
            

            //////////////////////////////////////////////////проверка на правильность ответа
           
            sDORequest.parameters[qNumber + ":" + index.ToString() +"_answer"] = gPTRequest.assistantResponse;
            sDORequest.SendReqest();

            for (int i = 1; i < 5; i++)
            {
                sDORequest.parameters[qNumber + ":" + index.ToString() + "_:sequencecheck"] = (check + i).ToString();
                sDORequest.SendReqest();
            }
            yield return new WaitForSeconds(10);

            if (IsTestPassed(sDORequest.allAnswerHtml) == false)
            {
                gPTRequest.userQuestion = "исправь свой код (пришли мне только python код без комментариев) на эту задачу: " + sDORequest.qestion + "твой старый код:" + gPTRequest.assistantResponse;
                gPTRequest.SendReqest();

                yield return new WaitForSeconds(20);

                sDORequest.parameters[qNumber + ":" + index.ToString() + "_answer"] = gPTRequest.assistantResponse;
                sDORequest.SendReqest();

                for (int i = 1; i < 5; i++)
                {
                    sDORequest.parameters[qNumber + ":" + index.ToString() + "_:sequencecheck"] = (check + i).ToString();
                    sDORequest.parameters["mdlscrollto"] = UnityEngine.Random.Range(200, 1001).ToString();
                    sDORequest.SendReqest();
                }
                yield return new WaitForSeconds(10);
            }


       
            //////////////////////////////////////////////////проверка на правильность ответа



            index++;
        }
    }

    public void UpdateParameters(string index)
    {
     
        var newPrefix = $"{qNumber}:{index}";
        var lastPrefix = $"";

     
        sDORequest.parameters.Remove($"{lastPrefix}_:flagged");
        sDORequest.parameters.Remove($"{lastPrefix}_:sequencecheck");
        sDORequest.parameters.Remove($"{lastPrefix}_answer");
        sDORequest.parameters.Remove($"{lastPrefix}_-submit");

        sDORequest.parameters.Remove($"{newPrefix.Substring(0, newPrefix.Length - 2)}_:flagged");
        sDORequest.parameters.Remove($"{newPrefix.Substring(0, newPrefix.Length - 2)}_:sequencecheck");
        sDORequest.parameters.Remove($"{newPrefix.Substring(0, newPrefix.Length - 2)}_answer");
        sDORequest.parameters.Remove($"{newPrefix.Substring(0, newPrefix.Length - 2)}_-submit");

      
        sDORequest.parameters.Add($"{newPrefix}_:flagged", "0");
        sDORequest.parameters.Add($"{newPrefix}_:sequencecheck", check.ToString());
        sDORequest.parameters.Add($"{newPrefix}_answer", "");
        sDORequest.parameters.Add($"{newPrefix}_-submit", "1");

        sDORequest.parameters["cmid"] = cmid;
        sDORequest.parameters["attempt"] = attempt;
        sDORequest.parameters["sesskey"] = sesskey;

        sDORequest.headers["cookie"] = cookie;           //замена куки сайта сдо

        sDORequest.parameters["thispage"] = (int.Parse(index) - 1).ToString();
        sDORequest.parameters["nextpage"] = index;
        sDORequest.parameters["slots"] = index;
    }

    public bool IsTestPassed(string input)
    {
       
        if (input.Contains("Ваш код не прошел один или несколько скрытых тестов."))
        {
            return false; 
        }
        if (input.Contains("есть неудачи"))
        {
            return false;
        }

        return true; 
    }
}
