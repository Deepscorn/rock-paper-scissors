using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using Game.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game
{
    public class GameViewController : MonoBehaviour
    {
        [SerializeField] 
        private Toggle _mediumAiDifficultyToggle;

        [SerializeField] 
        private Slider _aiDifficultySlider;

        [SerializeField] 
        private Text _scoreText;

        [SerializeField] 
        private Text _opponentDecisionText;

        [SerializeField] 
        private Text _roundResultWin;
        
        [SerializeField] 
        private Text _roundResultLose;
        
        [SerializeField] 
        private Text _roundResultDraw;
        
        [SerializeField] 
        private Toggle _stoneToggle;
        
        [SerializeField] 
        private Toggle _scissorsToggle;
        
        [SerializeField] 
        private Toggle _paperToggle;

        [SerializeField] 
        private ToggleGroup _buttonsLayout;
        
        [SerializeField] 
        private GameObject _difficultyPanel;
        
        [SerializeField] 
        private float _animationDuration = 0.1f;

        private Logic logic;
        private int playerScore;
        private int opponentScore;

        private void Awake()
        {
            logic = Logic.Instance;
            _difficultyPanel.gameObject.SetActive(!_mediumAiDifficultyToggle.isOn);
            _mediumAiDifficultyToggle.gameObject.SetActive(_mediumAiDifficultyToggle.isOn);
        }

        private void Start()
        {
            AddListener(_stoneToggle, HandDecision.Stone);
            AddListener(_scissorsToggle, HandDecision.Scissors);
            AddListener(_paperToggle, HandDecision.Paper);
            _mediumAiDifficultyToggle.onValueChanged.AddListener(on =>
            {
                _difficultyPanel.gameObject.SetActive(!on);
                _mediumAiDifficultyToggle.gameObject.SetActive(on);
            });
        }

        private void OnDestroy()
        {
            RemoveListener(_stoneToggle);
            RemoveListener(_scissorsToggle);
            RemoveListener(_paperToggle);
            _mediumAiDifficultyToggle.onValueChanged.RemoveAllListeners();
        }

        private IEnumerator RoundRoutine(HandDecision playerDecision)
        {
            _buttonsLayout.SetInteractable(false);
            
            HandDecision opponentDecision;
            if (_mediumAiDifficultyToggle.isOn)
            {
                opponentDecision = logic.MakeOpponentDecision();
            }
            else
            {
                opponentDecision = logic.MakeOpponentDecision(
                    playerDecision, 
                    _aiDifficultySlider.value, 
                    playerScore, 
                    opponentScore);
            }

            yield return ShowOpponentDecision(opponentDecision);

            var roundResult = logic.GetRoundResult(playerDecision, opponentDecision);
        
            yield return new WaitForSeconds(_animationDuration);
            yield return ShowRoundResult(roundResult);

            UpdateScore(roundResult);
            
            yield return new WaitWhile(() => Input.GetMouseButtonDown(0) == false);
            
            HideRoundResult();
            _opponentDecisionText.text = Localization.SelectYourFigure;
            _buttonsLayout.SetAllTogglesOff();
            _buttonsLayout.SetInteractable(true);
        }

        private void UpdateScore(GameResult roundResult)
        {
            playerScore += roundResult != GameResult.PlayerLose ? 1 : 0;
            opponentScore += roundResult != GameResult.PlayerWins ? 1 : 0;

            var postfix = Localization.NoOneWin;
            var color = GameConstants.DrawColor;
            if (playerScore != opponentScore)
            {
                postfix = playerScore > opponentScore ? Localization.YouWin : Localization.OpponentWin;
                color = playerScore > opponentScore ? GameConstants.WinColor : GameConstants.DrawColor;
            }

            _scoreText.text = $"{playerScore}:{opponentScore}. {postfix}";
            _scoreText.color = color;
        }

        private void HideRoundResult()
        {
            var allResults = EnumExt.GetValues<GameResult>();
            foreach (GameResult gameResult in allResults)
            {
                GetRoundResultText(gameResult).gameObject.SetActive(false);
            }
        }

        private IEnumerator ShowRoundResult(GameResult roundResult)
        {
            var resultText = GetRoundResultText(roundResult).gameObject.GetOrAddComponent<CanvasGroup>();
            resultText.gameObject.SetActive(true);
            
            resultText.alpha = 0;
            yield return resultText.DOFade(1, _animationDuration).WaitForCompletion();
        }

        private Text GetRoundResultText(GameResult roundResult)
        {
            switch (roundResult)
            {
                case GameResult.Draw:
                    return _roundResultDraw;
                case GameResult.PlayerLose:
                    return _roundResultLose;
                case GameResult.PlayerWins:
                    return _roundResultWin;
                default:
                    throw new NotImplementedException();
            }
        }

        private IEnumerator ShowOpponentDecision(HandDecision opponentDecision)
        {
            _opponentDecisionText.text = ".";
            
            yield return new WaitForSeconds(_animationDuration);
            _opponentDecisionText.text = "..";
            
            yield return new WaitForSeconds(_animationDuration);
            _opponentDecisionText.text = "...";
            
            yield return new WaitForSeconds(_animationDuration);
            _opponentDecisionText.text = Localization.Localize(opponentDecision);
        }

        private void AddListener(Toggle decisionToggle, HandDecision decision)
        {
            decisionToggle.onValueChanged.AddListener(on =>
            {
                if (on)
                {
                    StartCoroutine(RoundRoutine(decision));
                }
            });
        }

        private void RemoveListener(Toggle decisionToggle) => decisionToggle.onValueChanged.RemoveAllListeners();
    }
}