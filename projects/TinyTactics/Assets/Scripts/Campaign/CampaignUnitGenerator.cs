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
			if (theGood != "" && theBad != "") {
				description = $"<b>{name}</b>: <color=#AAD481>{theGood}</color>, <color=#F57C7C>{theBad}</color>";
			} else if (theGood != "" && theBad == "") {
				description = $"<b>{name}</b>: <color=#AAD481>{theGood}</color>";
			} else if (theGood == "" && theBad != "") {
				description = $"<b>{name}</b>: <color=#F57C7C>{theBad}</color>";
			} else {
				description = $"<b>{name}</b>";
			}
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
		new NatureData("Normie", "", ""),
		//
		new NatureData("Absolute Unit", "+VIT",  "-MOV", v: 12, m: -1),
		new NatureData("Bulky",  "+VIT", "-REF", v: 6, r: -3),
		new NatureData("Brickish", "+DEF", "-REF", def: 3, r: -6),
		new NatureData("Cautious", "+DEF", "-DEX", def: 2, dex: -3),
		new NatureData("Glass Cannon", "+STR", "-VIT, -REF, -DEF", s: 6, v: -6, r: -3, def: -3),
		new NatureData("Hasty", "+STR", "-DEF", s: 2, def: -2),
		new NatureData("Precise", "+DEX", "-VIT", dex: 6, v: -6),
		new NatureData("Sprinter", "+MOV", "-STR", m: 1, s: -2),
		new NatureData("Wily", "+REF", "-STR", r: 6, s: -2),
		new NatureData("Whiplike", "+DEX, +REF", "-STR", dex: 6, r: 6, s: -3)
	};

	[System.Serializable]
	public struct CampaignUnitData {
		// generation-time only
		public Guid ID;
		public string unitName; 			// Generated randomly
		public string className;			// loadable Prefab located in Assets/Prefabs/Resources/Units/PlayerUnits/*.prefab
		public NatureData nature;			// in-built stat mods to differentiate units between classes
		public ArchetypeData[] archetypes;	// archetypes from which the unit can draw Perks (Assault, Defender, Support, Cunning, Quick)
		public string signaturePerkTypeName;		// gotten from the prefab
		
		// below are modified during Campaign play
		public PerkData[] perks;
		public string[] wounds;

		public CampaignUnitData(PlayerUnit unitPrefab) {
			ID = Guid.NewGuid();

			unitName = NameDB.GetRandomName();

			// from prefab
			className = unitPrefab.gameObject.name;
			signaturePerkTypeName = unitPrefab.GetComponent<Perk>().GetType().Name;	// should only be one
			archetypes = unitPrefab.archetypes;

			// randomly selection
			nature = NatureTypes.SelectRandom<NatureData>();

			perks = new PerkData[0];
			wounds = new string[0];
		}

		// copy-constructor
		public CampaignUnitData(CampaignUnitData otherData) {
			ID = otherData.ID;
			unitName = otherData.unitName;
			className = otherData.className;
			signaturePerkTypeName = otherData.signaturePerkTypeName;
			archetypes = otherData.archetypes;
			nature = otherData.nature;
			//
			perks = otherData.perks;
			wounds = otherData.wounds;			
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
            PlayerUnit loadedPrefab = Resources.Load<PlayerUnit>($"Units/PlayerUnits/{unitData.className}");
			yield return loadedPrefab;
		}
	}
}
