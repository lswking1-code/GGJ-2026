using UnityEngine;

public class UImanager : MonoBehaviour
{
    public CharacterEventSO characterEventSO;
    public HPUI hpUI; 
    public GameObject GameClearUI;
    public VoidEventSO gameClearEventSO;


    private void OnEnable()
    {
        characterEventSO.OnEventRaised += OnCharacterEventRaised;
        gameClearEventSO.OnEventRaised += OnGameClearEventRaised;
    }
    private void OnDisable()
    {
        characterEventSO.OnEventRaised -= OnCharacterEventRaised;
        gameClearEventSO.OnEventRaised -= OnGameClearEventRaised;
    }
    private void OnCharacterEventRaised(Character character)
    {
        hpUI.OnHealthChange(character.currentHealth);
    }

    private void OnGameClearEventRaised()
    {
        GameClearUI.SetActive(true);
    }
}
