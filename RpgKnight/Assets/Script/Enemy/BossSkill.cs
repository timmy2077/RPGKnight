using System.Collections;
using UnityEngine;

public class BossSkill : MonoBehaviour
{
    [Header("技能设置")]
    public GameObject skillRangePrefab;
    public float skillDelay = 1f;
    public int skillDamage = 5;

    private EnemyMovement enemyMovement;
    private BossEnrage bossEnrage;
    private bool skillOnCooldown = false;

    private void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        bossEnrage = GetComponent<BossEnrage>();
    }

    private void Update()
    {
        if (bossEnrage == null || !bossEnrage.hasEnraged) return;
        if (skillOnCooldown) return;
        if (enemyMovement == null || enemyMovement.enemyState != EnemyState.Tiring) return;
        if (enemyMovement.player == null) return;

        StartCoroutine(CastSkill());
        
    }

    private IEnumerator CastSkill()
    {
        skillOnCooldown = true;

        Vector3 targetPos = enemyMovement.player.position;

        GameObject rangeObj = Instantiate(skillRangePrefab, targetPos, Quaternion.identity);

        BossSkillHitbox hitbox = rangeObj.GetComponent<BossSkillHitbox>();
        if (hitbox == null)
            hitbox = rangeObj.AddComponent<BossSkillHitbox>();
        hitbox.damage = skillDamage;

        Transform skillChild = rangeObj.transform.GetChild(0);
        if (skillChild != null)
            skillChild.gameObject.SetActive(false);

        yield return new WaitForSeconds(skillDelay);

        if (rangeObj != null && skillChild != null)
            skillChild.gameObject.SetActive(true);

        yield return new WaitWhile(() => enemyMovement != null && enemyMovement.enemyState == EnemyState.Tiring);

        skillOnCooldown = false;

        Destroy(rangeObj, 2f);
    }
}
