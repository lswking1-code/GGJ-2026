using UnityEngine;

public class UImanager : MonoBehaviour
{
    public CharacterEventSO characterEventSO;
    public HPUI hpUI; 


    private void OnEnable()
    {
        characterEventSO.OnEventRaised += OnCharacterEventRaised;
    }
    private void OnDisable()
    {
        characterEventSO.OnEventRaised -= OnCharacterEventRaised;
    }
    private void OnCharacterEventRaised(Character character)
    {
        hpUI.OnHealthChange(character.currentHealth);
    }

}
