using UnityEngine;

public class MapSwitch1 : MonoBehaviour
{
   public GameObject[] MapA;
   public GameObject[] MapB;
   public GameObject[] MapC;
   [Header("EventListener")]
   public MaskChangeEventSO maskChangeEventSO;

   private enum MapState
   {
      None,
      Angry,
      Happy,
      Sad
   }

   private MapState currentState = MapState.None;
   private void OnEnable()
   {
      maskChangeEventSO.OnEventRaised += OnMaskChange;
   }
   private void OnDisable()
   {
      maskChangeEventSO.OnEventRaised -= OnMaskChange;
   }

   private void OnMaskChange(int value)
   {
      MapState targetState = MapState.None;
      switch (value)
      {
         case 1:
            targetState = MapState.Angry;
            break;
         case 2:
            targetState = MapState.Happy;
            break;
         case 3:
            targetState = MapState.Sad;
            break;
      }

      if (targetState == currentState)
      {
         return;
      }

      currentState = targetState;

      SetMapsActive(MapA, currentState == MapState.Angry);
      SetMapsActive(MapB, currentState == MapState.Happy);
      SetMapsActive(MapC, currentState == MapState.Sad);
   }

   private void SetMapsActive(GameObject[] maps, bool isActive)
   {
      if (maps == null)
      {
         return;
      }

      foreach (GameObject map in maps)
      {
         if (map != null)
         {
            map.SetActive(isActive);
         }
      }
   }
}
