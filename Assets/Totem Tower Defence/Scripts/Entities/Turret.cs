﻿namespace TotemTD {
    using UnityEngine;
    using System.Collections.Generic;

    public class Turret : BaseEntity {
        [Header("Data")]
        public TurretBaseData data;

        [Header("Refs")]
        public LookAt lookAt;
        public Shooter shooter;
        public EnemyDetector enemyDetector;
        public Transform modDisplaysContainer;

        public bool freeSlots => modDisplays.Count < data.maxMods;

        List<TurretModDisplay> modDisplays = new List<TurretModDisplay>();

        protected override void CustomSetup () {
            lookAt.turnSpeed = data.turnSpeed;

            shooter.fireRate = data.fireRate;
            shooter.bulletData = data.bullet;

            enemyDetector.range = data.detectionRange;
        }

        public void AddModDisplay ( TurretModDisplay turretModDisplay ) {
            if ( freeSlots ) {
                modDisplays.Add( turretModDisplay );
                AttachMod( turretModDisplay.data );
            }
            UpdateDisplay();
        }

        public void RemoveModDisplay ( TurretModDisplay turretModDisplay ) {
            if ( modDisplays.Contains( turretModDisplay ) ) {
                Destroy( turretModDisplay.gameObject );
                modDisplays.Remove( turretModDisplay );
                DetachMod( turretModDisplay.data );
            }
            UpdateDisplay();
        }

        private void AttachMod ( TurretModData data ) {
            shooter.AddMod( data );
        }

        private void DetachMod ( TurretModData data ) {
            shooter.RemoveMod( data );
        }

        private void UpdateDisplay () {
            for ( int i = 0; i < modDisplays.Count; i++ ) {
                TurretModDisplay t = modDisplays[i];
                if ( t ) {
                    t.transform.SetParent( modDisplaysContainer );
                    t.transform.localScale = Vector3.one;
                    t.transform.localPosition = new Vector3( -i * t.transform.localScale.x, 0, 0 );
                }
            }
        }
    }
}