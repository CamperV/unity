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

    private string FormatMessage(string message) {
        Regex formatRgx = new Regex(@"\S+?\@\[.+?\]");
        Regex tagRgx = new Regex(@"(\S+?)(?<=\@)");
        Regex contentRgx = new Regex(@"(?=\[)(.+?)(?<=\])");

        foreach (Match m in formatRgx.Matches(message)) {
            string _tag = tagRgx.Match(m.Value).Value;
            string tag = _tag.Substring(0, _tag.Length-1);
            string _content = contentRgx.Match(m.Value).Value;
            string content = _content.Substring(1, _content.Length-2);

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
