﻿namespace TotemTD {
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;
    using Deirin.Utilities;
    using System.Collections.Generic;

    public class WaveManager : MonoBehaviour {
        [Header("Refs")]
        public Enemy enemyPrefab;

        [Header("Params")]
        public WaveData waveData;
        public Vector3ArrayVariable pathPoints;

        [Header("Events")]
        public UnityFloatEvent OnPlaceTimeStart;
        public UnityEvent OnPlaceTimeEnd;

        private bool counting;

        public void Setup () {
            GameObject modulesContainer = new GameObject("Turret Modules Container");
            modulesContainer.transform.SetParent( transform );

            RuntimeLevelData.turretModules = new List<TurretModule>();

            for ( int i = 0; i < waveData.turretModules.Count; i++ ) {
                TurretModule t = Instantiate( waveData.turretModules[i], modulesContainer.transform );
                t.gameObject.SetActive( false );
                RuntimeLevelData.turretModules.Add( t );
            }
        }

        public void StartPlaceTime () {
            StartCoroutine( PlaceTimeRoutine() );
        }

        public void StartWave () {
            StartCoroutine( WaveRoutine() );
        }

        public void SkipPlaceTime () {
            counting = false;
        }

        IEnumerator PlaceTimeRoutine () {
            float timer = 0;
            counting = true;

            OnPlaceTimeStart.Invoke( waveData.placeTime );

            while ( counting == true && timer < waveData.placeTime ) {
                timer += Time.deltaTime;
                yield return null;
            }

            OnPlaceTimeEnd.Invoke();
        }

        IEnumerator WaveRoutine () {
            foreach ( var item in waveData.enemies ) {
                Enemy e = Instantiate( enemyPrefab, pathPoints.Value[0], Quaternion.identity );
                yield return new WaitForSeconds( waveData.spawnInterval );
            }
        }
    }
}