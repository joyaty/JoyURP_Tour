using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotaAnimation : MonoBehaviour
{
    /// <summary>
    /// 每轮动画时长
    /// </summary>
    public const float kPerAnimationTime = 3.0f;

    /// <summary>
    /// 动画时长
    /// </summary>
    private float m_AnimationTime;

    /// <summary>
    /// 随机旋转方向
    /// </summary>
    private Vector3 m_AnimationDir;
    
    [Range(0, 1)]
    public float scaleValue = 0.1f;

    void Start()
    {
        ResetAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        m_AnimationTime += Time.deltaTime;
        transform.Rotate(m_AnimationDir);
        if (m_AnimationTime >= kPerAnimationTime)
        {
            ResetAnimation();
        }
    }

    void ResetAnimation()
    {
        m_AnimationTime = 0;
        float yaw = Random.Range(-180f, 180f) * Mathf.Deg2Rad * scaleValue;
        float roll = Random.Range(-180f, 180f) * Mathf.Deg2Rad * scaleValue;
        float pitch = Random.Range(-180f, 180f) * Mathf.Deg2Rad * scaleValue;
        m_AnimationDir = new Vector3(roll, yaw, pitch);
    }
}
