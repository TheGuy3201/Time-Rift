using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyType", menuName = "Enemies/EnemyType")]
public class EnemyType : ScriptableObject
{
    [Header("Enemy Details")]
    public new string name;
    //public Sprite characterSprite;
    //public Sprite weaponSprite;

    public enum TypeOfEnemy
    {
        Archer,
        Rifter,
        StormSoldier
    }
    [Tooltip("Select enemy type")]
    public TypeOfEnemy type;
}
