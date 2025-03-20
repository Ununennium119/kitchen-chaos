using System;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");


    [SerializeField] private Player player;


    private Animator _animator;


    private void Awake() {
        _animator = GetComponent<Animator>();
        _animator.SetBool(IsWalking, player.IsWalking());
    }

    private void Update() {
        _animator.SetBool(IsWalking, player.IsWalking());
    }
}