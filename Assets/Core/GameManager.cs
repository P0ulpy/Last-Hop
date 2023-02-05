using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        private Player _player;

        public Player player => _player;

        private AudioAssets _audioAssets;

        [SerializeField] private AudioEventAsset aeNormalExplosion;
        [SerializeField] private AudioEventAsset aeBigExplosion;
        [SerializeField] private AudioEventAsset aeRacineUnderground;
        [SerializeField] private AudioEventAsset aeRacineUp;
        [SerializeField] private AudioEventAsset aeFeuillesRacines;
        [SerializeField] private AudioEventAsset aeEnnemiTirBulet;
        [SerializeField] private AudioEventAsset aeEnnemiTirBomb;
        [SerializeField] private AudioEventAsset aeSwoosh;
        [SerializeField] private AudioEventAsset aeRenvoieProjectile;


        private void Awake()
        {
            _instance = this;

            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

            _audioAssets = FindObjectOfType<AudioAssets>();
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

        public void PlaySoundRacineUp()
        {
            _audioAssets.PlayAudioEvent(aeRacineUp);
            _audioAssets.PlayAudioEvent(aeFeuillesRacines);
        }

        public void PlaySoundNormalExplosion()   { _audioAssets.PlayAudioEvent(aeNormalExplosion); }
        public void PlaySoundBigExplosion()      { _audioAssets.PlayAudioEvent(aeBigExplosion); }
        public void PlaySoundRacineUnderground() { _audioAssets.PlayAudioEvent(aeRacineUnderground); }
        public void PlaySoundTirBullet()         { _audioAssets.PlayAudioEvent(aeEnnemiTirBomb); }
        public void PlaySoundTirBombe()          { _audioAssets.PlayAudioEvent(aeEnnemiTirBulet); }
        public void PlaySoundSwoosh()            {  }
        public void PlaySoundRenvoieProjectile() 
        {
            _audioAssets.PlayAudioEvent(aeRenvoieProjectile); }
    }
}