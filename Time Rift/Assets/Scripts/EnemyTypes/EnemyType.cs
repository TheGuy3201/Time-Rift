using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyType", menuName = "Enemies/EnemyType")]
public class EnemyType : ScriptableObject
{
    [Header("Enemy Details")]
    public new string name;
    //public Sprite characterSprite;
    public enum TypeOfEnemy
    {
        Archer,
        Rifter,
        StormSoldier
    }
    [Tooltip("Select enemy type")]
    public TypeOfEnemy type;

    public Sprite weaponSprite;
    public AnimatorController AnimController;
    public Sprite bulletSprite;
    public Sprite characterSprite;
    public string shootAudioName;

    
}
