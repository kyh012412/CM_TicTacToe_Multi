using System;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour {
    [SerializeField] private GameObject crossArrowGameObject;
    [SerializeField] private GameObject circleArrowGameObject;
    [SerializeField] private GameObject crossYouTextGameObject;
    [SerializeField] private GameObject circleYouTextGameObject;
    [SerializeField] private TextMeshProUGUI playerCrossScoreTextMesh;
    [SerializeField] private TextMeshProUGUI playerCircleScoreTextMesh;

    private void Awake() {
        crossArrowGameObject.SetActive(false);
        circleArrowGameObject.SetActive(false);
        crossYouTextGameObject.SetActive(false);
        circleYouTextGameObject.SetActive(false);

        playerCrossScoreTextMesh.text = "";
        playerCircleScoreTextMesh.text = "";
    }

    private void Start() {
        GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;
        GameManager.Instance.OnCurrentPlayablePlayerTypeChanged += GameManager_OnCurrentPlayablePlayerTypeChanged;
        GameManager.Instance.OnScoreChanged += GameManager_OnScoreChanged;
    }

    private void GameManager_OnScoreChanged(object sender, EventArgs e) {
        GameManager.Instance.GetScores(out int playerCrossScore, out int playerCircleScore);

        playerCrossScoreTextMesh.text = playerCircleScore.ToString();
        playerCircleScoreTextMesh.text = playerCrossScore.ToString();
    }

    private void GameManager_OnCurrentPlayablePlayerTypeChanged(object sender, EventArgs e) {
        UpdateCurrentArrow();
    }

    private void GameManager_OnGameStarted(object sender, EventArgs e) {
        if (GameManager.Instance.GetLocalPlayerType() == GameManager.PlayerType.Cross) {
            crossYouTextGameObject.SetActive(true);
        } else {
            circleYouTextGameObject.SetActive(true);
        }

        playerCrossScoreTextMesh.text = "0";
        playerCircleScoreTextMesh.text = "0";

        UpdateCurrentArrow();
    }

    /// <summary>
    /// 현재 플레이 가능한 사람 표시 UI (당사자 무관)
    /// </summary>
    private void UpdateCurrentArrow() {
        Debug.Log("update Current Arrow");
        if (GameManager.Instance.GetCurrentPlayablePlayerType() == GameManager.PlayerType.Cross) {
            crossArrowGameObject.SetActive(true);
            circleArrowGameObject.SetActive(false);
        } else {
            circleArrowGameObject.SetActive(true);
            crossArrowGameObject.SetActive(false);
        }
    }
}
