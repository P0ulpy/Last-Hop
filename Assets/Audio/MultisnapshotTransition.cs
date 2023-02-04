using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MultisnapshotTransition : MonoBehaviour
{
    [System.Serializable]
    public class SnapshotTransition
    {
        public AudioMixerSnapshot _snapshot;
        public float _duration;
    }

    [SerializeField] private AudioMixer _filterMixer;
    [SerializeField] private AudioMixerSnapshot _defaultSnapshot;
    [SerializeField] private SnapshotTransition[] _reverbTransitions;

    private AudioMixerSnapshot _currentSnapshot;
    private float[] _weights = { 0, 0 };

    private IEnumerator _CorStartTransition;

    private void Awake()
    {
        _CorStartTransition = CorStartTransition();

        _weights[0] = 0.0f;
        _weights[1] = 1.0f;
    }

    public void StartTransition()
    {
        _currentSnapshot = _defaultSnapshot;

        StartCoroutine(_CorStartTransition);
    }

    public void StopTransition()
    {
        StopCoroutine(_CorStartTransition);
        _CorStartTransition = CorStartTransition();

        AudioMixerSnapshot[] snapshots = { _reverbTransitions[_reverbTransitions.Length - 1]._snapshot, _defaultSnapshot };
        _filterMixer.TransitionToSnapshots(snapshots, _weights, 0);
    }

    private IEnumerator CorStartTransition()
    {
        foreach (var transition in _reverbTransitions)
        {
            AudioMixerSnapshot[] snapshots = { _currentSnapshot, transition._snapshot };

            _filterMixer.TransitionToSnapshots(snapshots, _weights, transition._duration);
            yield return new WaitForSeconds(transition._duration);

            _currentSnapshot = transition._snapshot;
        }
    }
}
