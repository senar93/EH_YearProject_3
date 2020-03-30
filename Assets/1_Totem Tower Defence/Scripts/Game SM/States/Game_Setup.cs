﻿namespace SweetRage {
    public class Game_Setup : Game_BaseState {
        public override void Enter () {
            base.Enter();

            //wave manager setup
            gameData.waveManager.Setup();

            //turret menu UI update
            gameData.modulesMenuUI.Setup();
            gameData.modulesMenuUI.UpdateUI();
            gameData.modulesMenuUI.Activate( false );

            //phase UI setup
            gameData.phaseUI.Setup();

            //HACK
            //should call an animation or smtn
            gameData.placeTimeUI.gameObject.SetActive( false );
            //HACK

            gameData.GoNext();
        }
    }
}