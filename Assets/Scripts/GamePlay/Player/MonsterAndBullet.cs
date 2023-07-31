using UnityEngine;


public class MonsterAndBullet : MonoBehaviour
{
    public static void BulletAttackMonster(Bullet bullet, MonsterController monster)
    {
        monster.SetHP(monster.GetHP().Value - bullet.GetDamage());
        Destroy(bullet.gameObject);
    }


    public static void BulletAttackBoss(Bullet bullet, bossAction boss)
    {
        boss.TakeDamage(bullet.GetDamage());
        // Debug.Log(boss.GetHealth());
        Destroy(bullet.gameObject);

    }


}
