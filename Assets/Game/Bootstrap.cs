using UnityEngine;

namespace Game
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private GameViewController _gameViewController;
        
        private Logic _logic;
        
        private void Start()
        {
            _logic = new Logic();
            _gameViewController.Init(_logic);
        }
    }
}
