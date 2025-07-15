using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyTier", menuName = "Enemies/EnemyTier")]
public class EnemyTier : ScriptableObject
{
    [Header("Enemy Details")]
    public new string name;

    public enum TierOfEnemy
    {
        Light,
        Medium,
        Heavy,
        SUPERHEAVY
    }
    [Tooltip("Select enemy tier")]
    public TierOfEnemy tier;
}
