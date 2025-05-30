using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
public abstract class Weapons : ScriptableObject
{
    [field: SerializeField] public float attackSpeed { get; private set; }
    [SerializeField] Animator animator;
    [SerializeField] public Image image { get; private set; }
    [SerializeField] GameObject item;
    [SerializeField] string itemDisc;
   protected abstract void PlayAttack();

}
