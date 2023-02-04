using System.Collections;
using UnityEngine;

namespace Core.VFX
{
    public class VFXLifeTime : MonoBehaviour
    {
        [SerializeField] private float lifeTime = 3f;

        private void Start()
        {
            StartCoroutine(TuNeLeSaitPasEncoreMaisTuEstDejaMort());
        }

        private IEnumerator TuNeLeSaitPasEncoreMaisTuEstDejaMort()
        {
            yield return new WaitForSeconds(lifeTime);
            Destroy(gameObject);
        }
    }
}