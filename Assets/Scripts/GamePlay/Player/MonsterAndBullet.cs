using UnityEngine;


public class MonsterAndBullet : MonoBehaviour
{
    public static void BulletAttackMonster(Bullet bullet, MonsterController monster)
    {
        monster.SetHPServerRpc(monster.GetHP().Value - bullet.GetDamage());
        Destroy(bullet.gameObject);
    }
}
