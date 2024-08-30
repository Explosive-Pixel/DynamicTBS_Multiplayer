using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class AnimationSystem : MonoBehaviour
{
    [SerializeField] private AnimationClip idle;
    [SerializeField] private AnimationClip movement;
    [SerializeField] private AnimationClip attack;
    [SerializeField] private AnimationClip death;
    [SerializeField] private AnimationClip active;
    [SerializeField] private AnimationClip passive;
    private Animation anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
        if (anim.GetClipCount() == 0)
        {
            anim.AddClip(idle, "idle");
            /*anim.AddClip(movement, "movement");
            anim.AddClip(attack, "attack");
            anim.AddClip(death, "death");
            anim.AddClip(active, "active_ability");
            anim.AddClip(passive, "passive_ability");*/
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
