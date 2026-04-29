using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

//半山腰太挤，你总得去山顶看看//
public class ObjectPoolSystem : MonoBehaviour
{
    public static ObjectPoolSystem instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject Object = new GameObject(typeof(ObjectPoolSystem).Name);
                Instance = Object.AddComponent<ObjectPoolSystem>();
                DontDestroyOnLoad(Object);
            }
            return Instance;
        }
    }
    private static ObjectPoolSystem Instance;

    [SerializeField] private int poolSize;

    [SerializeField] private GameObject audioSourceElement;

    private Queue<AudioSource> audioPool;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DilatePool(audioPool, audioSourceElement);
    }

    #region GetPoolElement
    public AudioSource GetAudioSourceElement()
    {
        return GetElement(audioPool, audioSourceElement);
    }
    #endregion

    public void RecycleElement<T>(T component) where T : Component
    {
        if (component == null) return;


        if (component is AudioSource audio)
        {
            ResetAudioSource(audio);
            audioPool.Enqueue(audio);
        }

        component.gameObject.SetActive(false);
    }

    private T GetElement<T>(Queue<T> pool,GameObject prefab) where T : Component
    {
        if (pool.Count == 0)
        {
            return CreateNewElement(pool, prefab);
        }

        T element = pool.Dequeue();
        if (element == null)
        {
            return CreateNewElement(pool, prefab);
        }

        element.gameObject.SetActive(true);
        return element;
    }

    private T DilatePool<T>(Queue<T> pool,GameObject prefab) where T : Component
    {
        if (pool == null)
        {
            pool = new Queue<T>();
        }

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewElement(pool, prefab).gameObject.SetActive(false);
        }

        return pool.Dequeue();
    }

    private T CreateNewElement<T>(Queue<T> pool, GameObject prefab) where T : Component
    {
        GameObject obj = Instantiate(prefab, transform);
        T element = obj.GetComponent<T>();
        pool.Enqueue(element);
        return element;
    }

    #region ResetPool
    private void ResetSpriteRenderer(SpriteRenderer sprite)
    {
        sprite.sprite = null;
        sprite.color = Color.white;
        sprite.transform.position = Vector3.zero;
        sprite.transform.rotation = Quaternion.identity;
    }

    private void ResetAnimator(Animator animator)
    {
        animator.runtimeAnimatorController = null;
        animator.ResetTrigger("Play"); // 重置触发器
        animator.transform.position = Vector3.zero;
    }

    private void ResetParticleSystem(ParticleSystem particle)
    {
        particle.Stop();
        particle.Clear(); // 清除残留粒子
        particle.transform.position = Vector3.zero;
    }

    private void ResetAudioSource(AudioSource audio)
    {
        audio.clip = null;
        audio.Stop();
        audio.transform.position = Vector3.zero;
    }
    #endregion
}