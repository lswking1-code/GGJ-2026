using UnityEngine;

public class MapSwitch : MonoBehaviour
{
   public GameObject MapA;
   public GameObject MapB;
   public GameObject MapC;

   [SerializeField] private TopDownPlayerController player;

   private enum MapState
   {
      None,
      Angry,
      Happy,
      Sad
   }

   private MapState currentState = MapState.None;

   private void Awake()
   {
      ApplyFromPlayer();
   }

   private void Update()
   {
      ApplyFromPlayer();
   }

   private void ApplyFromPlayer()
   {
      if (player == null)
      {
         return;
      }

      MapState targetState = MapState.None;
      if (player.Angry)
      {
         targetState = MapState.Angry;
      }
      else if (player.Happy)
      {
         targetState = MapState.Happy;
      }
      else if (player.Sad)
      {
         targetState = MapState.Sad;
      }

      if (targetState == currentState)
      {
         return;
      }

      currentState = targetState;

      if (MapA != null)
      {
         MapA.SetActive(currentState == MapState.Angry);
      }

      if (MapB != null)
      {
         MapB.SetActive(currentState == MapState.Happy);
      }

      if (MapC != null)
      {
         MapC.SetActive(currentState == MapState.Sad);
      }
   }
}
