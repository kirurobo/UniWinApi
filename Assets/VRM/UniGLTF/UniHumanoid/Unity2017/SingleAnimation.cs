#if UNITY_2017_3_OR_NEWER
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;


namespace UniHumanoid
{
    public class SingleAnimation : MonoBehaviour
    {
        [SerializeField]
        AnimationClip m_clip;

        [SerializeField]
        Animator m_animator;

        private void Reset()
        {
            m_animator = GetComponent<Animator>();
        }

        PlayableGraph m_graph;

        void Awake()
        {
            m_graph = PlayableGraph.Create();
        }

        private void Start()
        {
            var output = AnimationPlayableOutput.Create(m_graph, m_animator.name, m_animator);

            var clipPlayable = AnimationClipPlayable.Create(m_graph, m_clip);
            output.SetSourcePlayable(clipPlayable);

            m_graph.Play();
        }

        private void OnDestroy()
        {
            m_graph.Destroy();
        }
    }
}
#endif