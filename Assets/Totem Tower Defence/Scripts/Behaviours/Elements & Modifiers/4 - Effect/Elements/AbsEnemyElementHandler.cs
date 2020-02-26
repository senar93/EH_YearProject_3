﻿namespace TotemTD
{
	using Deirin.EB;
    using Deirin.Utilities;
    using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	public abstract class AbsEnemyElementHandler : BaseBehaviour
	{
		[SerializeField] private ElementScriptableEnum _elementType;

		[Header("Generic Effect Params"), Tooltip("Numero massimo di livelli consentiti per l'effetto")]
		public int maxCharge = 3;
		[Space, Tooltip("Durata dell'effetto alla prima applicazione")]
		public float duration = 1f;
		[Tooltip("Tempo extra aggiunto all'effetto se viene riapplicato mentre è ancora in corso")]
		public float extraTime = 1f;

		[SerializeField, Space, ReadOnly] private int currentCharge = 0;

		[Header("Generic Effect Events"), Space]
		public Unity_Int_Event OnCurrentChargeRefresh;
		public Unity_Int_Event OnCurrentChargeIncreases;
		public Unity_Int_Event OnCurrentChargeDecreases;


		public ElementScriptableEnum elementType 
		{
			get => _elementType;
		}

		public void SetCurrentCharge(int value)
		{
			int oldValue = currentCharge;
			currentCharge = value;

			if (oldValue == value)
			{
				OnCurrentChargeRefresh?.Invoke(currentCharge);
			} 
			else if(oldValue < value)
			{
				OnCurrentChargeIncreases?.Invoke(currentCharge);
			}
			else if(oldValue > value)
			{
				OnCurrentChargeDecreases?.Invoke(currentCharge);
			}

		}

		public int GetCurrentCharge()
		{
			return currentCharge;
		}

	}

}