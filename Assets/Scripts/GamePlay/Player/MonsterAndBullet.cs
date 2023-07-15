using UnityEngine;


public class MonsterAndBullet : MonoBehaviour
{
    public static void BulletAttackMonster(NormalAttack bullet, MonsterController monster)
    {
        monster.SetHP(monster.GetHP().Value - bullet.GetDamage());
        Destroy(bullet.gameObject);
    }
}
