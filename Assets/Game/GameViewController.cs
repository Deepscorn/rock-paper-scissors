using System;
using System.Collections;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Utils;
using UnityEngine;
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

        private Logic _logic;
        private CancellationTokenSource _cancelTokenSource;

        private void Awake()
        {
            _difficultyPanel.gameObject.SetActive(!_mediumAiDifficultyToggle.isOn);
            _mediumAiDifficultyToggle.gameObject.SetActive(_mediumAiDifficultyToggle.isOn);
            _cancelTokenSource = new();
        }

        public void Init(Logic logic)
        {
            _logic = logic;
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
            _cancelTokenSource.Cancel();
            RemoveListener(_stoneToggle);
            RemoveListener(_scissorsToggle);
            RemoveListener(_paperToggle);
            _mediumAiDifficultyToggle.onValueChanged.RemoveAllListeners();
        }

        private async UniTask RoundRoutine(HandDecision playerDecision)
        {
            _buttonsLayout.SetInteractable(false);
            
            HandDecision opponentDecision;
            if (_mediumAiDifficultyToggle.isOn)
            {
                opponentDecision = _logic.MakeOpponentDecision();
            }
            else
            {
                opponentDecision = _logic.MakeOpponentDecision(
                    playerDecision, 
                    _aiDifficultySlider.value);
            }

            await ShowOpponentDecision(opponentDecision);

            if (_cancelTokenSource.IsCancellationRequested)
            {
                return;
            }

            var roundResult = _logic.GetRoundResult(playerDecision, opponentDecision);
        
            await UniTask.WaitForSeconds(_animationDuration);
            await ShowRoundResult(roundResult);
            
            if (_cancelTokenSource.IsCancellationRequested)
            {
                return;
            }

            UpdateScore(roundResult);
            
            await UniTask.WaitWhile(() => Input.GetMouseButtonDown(0) == false);
            
            if (_cancelTokenSource.IsCancellationRequested)
            {
                return;
            }
            
            HideRoundResult();
            _opponentDecisionText.text = Localization.SelectYourFigure;
            _buttonsLayout.SetAllTogglesOff();
            _buttonsLayout.SetInteractable(true);
        }

        private void UpdateScore(GameResult roundResult)
        {
            _logic.UpdateScore(roundResult);

            var postfix = Localization.NoOneWin;
            var color = GameConstants.DrawColor;

            var playerScore = _logic.PlayerScore;
            var opponentScore = _logic.OpponentScore;
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

        private async UniTask ShowRoundResult(GameResult roundResult)
        {
            var resultText = GetRoundResultText(roundResult).gameObject.GetOrAddComponent<CanvasGroup>();
            resultText.gameObject.SetActive(true);
            
            resultText.alpha = 0;
            await resultText.DOFade(1, _animationDuration).Play().ToUniTask();
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

        private async UniTask ShowOpponentDecision(HandDecision opponentDecision)
        {
            _opponentDecisionText.text = ".";
            
            await UniTask.WaitForSeconds(_animationDuration);
            _opponentDecisionText.text = "..";
            
            await UniTask.WaitForSeconds(_animationDuration);
            _opponentDecisionText.text = "...";
            
            await UniTask.WaitForSeconds(_animationDuration);
            _opponentDecisionText.text = Localization.Localize(opponentDecision);
        }

        private void AddListener(Toggle decisionToggle, HandDecision decision)
        {
            decisionToggle.onValueChanged.AddListener(on =>
            {
                if (on)
                {
                    _cancelTokenSource.Cancel();
                    _cancelTokenSource = new CancellationTokenSource();
                    RoundRoutine(decision).Forget();
                }
            });
        }

        private void RemoveListener(Toggle decisionToggle) => decisionToggle.onValueChanged.RemoveAllListeners();
    }
}