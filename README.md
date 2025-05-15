# GameDevProject
Inside is 6 sections with 16 scripts:

# Audio
BackgroundMusic.cs Background music

# Player
PlayerMovement.cs: Controlling animation and movement

Health.cs: Containing the players health and UI

SimpleJoystick.cs Handles joystick for mobile

StartCamerca.cs Triggers cut scene when player loads new level

CameraController.cs Handles the camera movement third person

# Game scripts
LevelLoader.cs Used in menu to transition between the scences

LevelLocking.cs Handles datastore for previous levels and unlocks levels as player progresses

MenuLoader.cs Loads the menu

TreasureFlash.cs Triggers a flash in the treasure at the end of a level

# EnemyAi
AiPlayerRotater.cs Rotates stuck archer AI enemy 

AnimalAiBehaviour.cs Uses meshnav to move Animals and attack when close enough

Sawblade.cs Moves sawblade and deals damage

TakeDamage.cs Deals damage, used for boulders

# Projectiles
Cannon.cs: This clones the trajetories and injects them into the game

HomingMissile.cs This handles the trajectories movements

# Models
All models were imported from the Unity Asset Store
Players character: https://www.mixamo.com/#/?page=1&type=Character
