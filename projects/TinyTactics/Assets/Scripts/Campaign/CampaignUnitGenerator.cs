using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public class CampaignUnitGenerator
{
	[System.Serializable]
	public struct NatureData {
		public string name;
		public string description;

		public int m_VITALITY;
		public int m_STRENGTH;
		public int m_DEXTERITY;
		public int m_REFLEX;
		public int m_DEFENSE;
		public int m_MOVE;

		public NatureData(string n, string theGood, string theBad, int v=0, int s=0, int dex=0, int r=0, int def=0, int m=0) {
			name = n;
			description = $"<b>{name}</b>: <color=#AAD481>{theGood}</color>, <color=#F57C7C>{theBad}</color>";
			//
			m_VITALITY = v;
			m_STRENGTH = s;
			m_DEXTERITY = dex;
			m_REFLEX = r;
			m_DEFENSE = def;
			m_MOVE = m;
		}
	}

	// for random generation
	public static NatureData[] NatureTypes = new NatureData[]{
		new NatureData("Normie", "n/a",  "n/a"),
		new NatureData("Bulky",  "+VIT", "-REF", v: 5, r: -3),
		new NatureData("Noodly", "+MOV", "-STR", s: -3, m: 1),
		new NatureData("Godly",  "+ALL", "n/a", v: 50, s: 50, dex: 50, m: 10)
	};

	[System.Serializable]
	public struct CampaignUnitData {
		// generation-time only
		public Guid ID;
		public string prefabID;			// loadable Prefab located in Assets/Prefabs/Resources/Units/PlayerUnits/*.prefab
		public NatureData nature;		// in-built stat mods to differentiate units between classes
		public string[] archetypes;		// archetypes from which the unit can draw Perks (Assault, Defender, Support, Cunning, Quick)
		public string signaturePerk;	// gotten from the prefab
		
		// below are modified during Campaign play
		public string[] perks;
		public string[] wounds;

		public CampaignUnitData(PlayerUnit unitPrefab) {
			ID = new Guid();

			// from prefab
			prefabID = unitPrefab.gameObject.name;
			signaturePerk = unitPrefab.GetComponent<Perk>().name;	// should only be one
			archetypes = unitPrefab.archetypes;

			// randomly selection
			nature = NatureTypes.SelectRandom<NatureData>();

			perks = new string[0];
			wounds = new string[0];
		}
	}

	public struct CampaignUnitPackage {
		public PlayerUnit unitPrefab;
		public CampaignUnitData unitData;

		public CampaignUnitPackage(PlayerUnit _unitPrefab) {
			unitPrefab = _unitPrefab;
			unitData = new CampaignUnitData(_unitPrefab);
		}
	}

	public static IEnumerable<PlayerUnit> DeserializeUnits(ICollection<CampaignUnitData> serializedUnits) {
		foreach (CampaignUnitData unitData in serializedUnits) {
            PlayerUnit loadedPrefab = Resources.Load<PlayerUnit>($"Units/PlayerUnits/{unitData.prefabID}");
			loadedPrefab.GetComponent<UnitStats>().ApplyNature(unitData.nature);

			yield return loadedPrefab;
		}
	}
}
