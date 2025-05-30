using BepInEx;
using RoR2;
using System.Diagnostics;
using System.IO;
using EntityStates.FalseSonBoss;
using EntityStates.MeridianEvent;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CoolerFalseSonSpawn
{
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class Main : BaseUnityPlugin
  {
    public const string PluginGUID = PluginAuthor + "." + PluginName;
    public const string PluginAuthor = "Nuxlar";
    public const string PluginName = "CoolerFalseSonSpawn";
    public const string PluginVersion = "1.0.0";

    internal static Main Instance { get; private set; }
    public static string PluginDirectory { get; private set; }

    public void Awake()
    {
      Instance = this;

      Log.Init(Logger);

      SetAddressableEntityStateField("RoR2/DLC2/meridian/RoR2.MeridianEventPhase1.asset", "endStateDelay", "1.5");
      SetAddressableEntityStateField("RoR2/DLC2/meridian/RoR2.MeridianEventPhase1.asset", "durationBeforeEnablingCombatEncounter", "0");
      SetAddressableEntityStateField("RoR2/DLC2/meridian/RoR2.MeridianEventPhase2.asset", "endStateDelay", "1.5");
      SetAddressableEntityStateField("RoR2/DLC2/meridian/RoR2.MeridianEventPhase2.asset", "durationBeforeEnablingCombatEncounter", "0");

      On.EntityStates.FalseSonBoss.CrystalDeathState.OnEnter += ChangeP2SpawnPos;
      On.EntityStates.FalseSonBoss.BrokenCrystalDeathState.OnEnter += ChangeP3SpawnPos;
    }

    private void ChangeP2SpawnPos(On.EntityStates.FalseSonBoss.CrystalDeathState.orig_OnEnter orig, CrystalDeathState self)
    {
      if (MeridianEventTriggerInteraction.instance)
      {
        Transform encounterLogic = MeridianEventTriggerInteraction.instance.transform.Find("Boss EncounterLogic");
        if (encounterLogic)
        {
          Transform spawnTransform = encounterLogic.GetChild(1).GetComponent<ScriptedCombatEncounter>().spawns[0].explicitSpawnPosition;
          spawnTransform.rotation = self.characterBody.transform.rotation;
          spawnTransform.position = new Vector3(self.characterBody.footPosition.x, self.characterBody.footPosition.y, self.characterBody.footPosition.z);
        }
      }
      orig(self);
    }

    private void ChangeP3SpawnPos(On.EntityStates.FalseSonBoss.BrokenCrystalDeathState.orig_OnEnter orig, BrokenCrystalDeathState self)
    {
      if (MeridianEventTriggerInteraction.instance)
      {
        Transform encounterLogic = MeridianEventTriggerInteraction.instance.transform.Find("Boss EncounterLogic");
        if (encounterLogic)
        {
          Transform spawnTransform = encounterLogic.GetChild(2).GetComponent<ScriptedCombatEncounter>().spawns[0].explicitSpawnPosition;
          spawnTransform.rotation = self.characterBody.transform.rotation;
          spawnTransform.position = new Vector3(self.characterBody.footPosition.x, self.characterBody.footPosition.y, self.characterBody.footPosition.z);
        }
      }
      orig(self);
    }

    public static bool SetAddressableEntityStateField(string fullEntityStatePath, string fieldName, string value)
    {
      EntityStateConfiguration esc = Addressables.LoadAssetAsync<EntityStateConfiguration>(fullEntityStatePath).WaitForCompletion();
      for (int i = 0; i < esc.serializedFieldsCollection.serializedFields.Length; i++)
      {
        if (esc.serializedFieldsCollection.serializedFields[i].fieldName == fieldName)
        {
          esc.serializedFieldsCollection.serializedFields[i].fieldValue.stringValue = value;
          return true;
        }
      }
      return false;
    }

  }
}