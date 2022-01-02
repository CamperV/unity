using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatLog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI log;
    
    public int historySize;
    private List<string> messageHistory;
    private ScrollRect scrollRect;

    private readonly string delim = "@";

    void Awake() {
        messageHistory = new List<string>();
        scrollRect = GetComponent<ScrollRect>();
    }

    void Start() {
        Clear();
    }

    public void Clear() {
        messageHistory.Clear();
        log.SetText("> ...");

        // scroll to bottom
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }

    private string _FormatMessage(string message) {
        string finalMessage = "";

        foreach (string word in message.Split(char.Parse(" "))) {
            if (word.Contains(delim)) {

                string[] tokens = word.Split(char.Parse(delim));
                string tag = tokens[0];
                string content = tokens[1];

                switch (tag) {
                    case "PLAYER_UNIT":
                        finalMessage += $"<color=green><b>{content}</b></color>";
                        break;
                    case "ENEMY_UNIT":
                        finalMessage += $"<color=red><b>{content}</b></color>";
                        break;
                    case "NUMBER":
                        finalMessage += $"<color=yellow><b>{content}</b></color>";
                        break;
                    case "KEYWORD":
                        finalMessage += $"<b>{content}</b>";
                        break;
                    case "YELLOW":
                        finalMessage += $"<color=yellow><b>{content}</b></color>";
                        break;
                    case "RED":
                        finalMessage += $"<color=red><b>{content}</b></color>";
                        break;
                    case "GREEN":
                        finalMessage += $"<color=green><b>{content}</b></color>";
                        break;
                    case "PURPLE":
                        finalMessage += $"<color=purple><b>{content}</b></color>";
                        break;
                    case "BLUE":
                        finalMessage += $"<color=blue><b>{content}</b></color>";
                        break;
                    default:
                        finalMessage += content;
                        break;
                }

            } else {
                finalMessage += word;
            }

            finalMessage += " ";
        }

        return finalMessage.Trim();
    }

    private string FormatMessage(string message) {
        Regex formatRgx = new Regex(@"\S+?\@\[.+?\]");
        Regex tagRgx = new Regex(@"(\S+?)(?<=\@)");
        Regex contentRgx = new Regex(@"(?=\[)(.+?)(?<=\])");

        foreach (Match m in formatRgx.Matches(message)) {
            Debug.Log($"Found match {m.Value}");

            string _tag = tagRgx.Match(m.Value).Value;
            string tag = _tag.Substring(0, _tag.Length-1);
            string _content = contentRgx.Match(m.Value).Value;
            string content = _content.Substring(1, _content.Length-2);

            Debug.Log($"got tag {tag} [{tag.GetType()}] and content {content}");

            switch (tag) {
                case "PLAYER_UNIT":
                    message = message.Replace(m.Value, $"<color=green><b>{content}</b></color>");
                    break;
                case "ENEMY_UNIT":
                    message = message.Replace(m.Value, $"<color=red><b>{content}</b></color>");
                    break;
                case "NUMBER":
                    message = message.Replace(m.Value, $"<color=yellow><b>{content}</b></color>");
                    break;
                case "KEYWORD":
                    message = message.Replace(m.Value, $"<b>{content}</b>");
                    break;
                case "YELLOW":
                    message = message.Replace(m.Value, $"<color=yellow><b>{content}</b></color>");
                    break;
                case "RED":
                    message = message.Replace(m.Value, $"<color=red><b>{content}</b></color>");
                    break;
                case "GREEN":
                    message = message.Replace(m.Value, $"<color=green><b>{content}</b></color>");
                    break;
                case "PURPLE":
                    message = message.Replace(m.Value, $"<color=purple><b>{content}</b></color>");
                    break;
                case "BLUE":
                    message = message.Replace(m.Value, $"<color=blue><b>{content}</b></color>");
                    break;
            }
        }

        return message;
    }

    public void AddEntry(string message) {
        messageHistory.Add(FormatMessage(message));

        while (messageHistory.Count > historySize) {
            messageHistory.RemoveAt(0);
        }

        // now display
        // string fullLog = "> ...\n";
        string fullLog = "";
        foreach (string entry in messageHistory) {
            fullLog += $"> {entry}\n";
        }

        fullLog += $"> ...";
        log.SetText(fullLog);

        // scroll to bottom
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }
}
