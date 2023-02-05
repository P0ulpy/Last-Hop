using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        private Player _player;

        public Player player => _player;

        [SerializeField] private AudioAssets _audioAssets;
        public AudioAssets _AudioAssets => _audioAssets;

        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            _instance = this;
        }
        
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
                }

                return _instance;
            }
        }
    }
}