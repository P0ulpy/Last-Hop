using System.Collections;
using UnityEngine;

namespace Core.VFX
{
    public class VFXLifeTime : MonoBehaviour
    {
        [SerializeField] protected float lifeTime = 3f;

        protected virtual void Start()
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