using System.Collections;
using System.Collections.Generic;
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

    private string FormatMessage(string message) {
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
