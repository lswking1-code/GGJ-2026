using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth;
    public float currentHealth;
    
    [Header("Invulnerable")]
    public float invulnerableDuration;
    private float invulnerableCounter;//倒计时
    public bool invulnerable;//是否无敌
    [Header("Damage")]
    public int damage;
    [Header("Events")]
    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;

    private void Start()
    {
        currentHealth = maxHealth;
    }
    private void Update()
    {
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;//����ʱ������(�޵�ʱ������)
            if(invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }
    }
    public void TakeDamage(Attack attacker)
    {
        if (invulnerable)
            return;
        //Debug.Log(attacker.damage);
        if(currentHealth-attacker.damage > 0)
        {
            currentHealth -= attacker.damage;
            triggerInvulnerable();
            OnTakeDamage?.Invoke(attacker.transform);
        }
        else 
        {
            currentHealth = 0;
            OnDie?.Invoke();
        }

        OnHealthChange?.Invoke(this);
    }
     private void triggerInvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}
