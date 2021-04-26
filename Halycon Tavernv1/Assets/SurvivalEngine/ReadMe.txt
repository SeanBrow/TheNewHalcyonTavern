
Thank you for downloading Survival Engine!

To get started, you may do the following:
1. Look at the demo scenes in the Scenes folder
2. Watch the tutorial videos on Youtube: https://www.youtube.com/channel/UC0LYig0AgPT9T5IN5DCUiqQ
3. Read the PDF doc in this folder. (or the latest version here: https://indiemarc.com/demo/survival-engine/SurvivalEngine.pdf)
4. Join the discord to ask questions: https://discord.gg/JpVwUgG
5. Come discuss on the forum page: https://forum.unity.com/threads/released-survival-engine-complete-project-template-for-survival-games.972804/
6. If you still need help, you may contact me at: contact@indiemarc.com


Integration with Dialogue and Quests:
1) Import Survival Engine first
2) Import Dialogue and Quests
3) If there are no compile errors, the Scripting Define Symbol DIALOGUE_QUESTS should be added automatically to the player settings. If not, add it manually. 
4) Make sure the DQManager prefab is in the scene, and that you add the Actor script to the PlayerCharacter! Then everything should work!
5) Optional but strongly recommended: Download the Demo package, it contains a demo scene to show how the two assets work together.
It can be downloaded here: https://indiemarc.com/demo/survival-engine/DialogueQuestsSurvival1.02.unitypackage
6) Open the demo scene in the DialogueQuestsDemo folder and try it!

Integration with Map and Minimap:
1) Import Survival Engine first
2) Import Map and Minimaps
3) If there are no compile errors, the "Scripting Define Symbol" MAP_MINIMAP should be added automatically to the player settings. If not add it manually. 
4) Make sure the MapManager prefab is in the scene, and that you add the MapIcon script to the PlayerCharacter (Set its type to "Player").
5) Add a MapZone to your scene, and setup the zone to contain the map area, then use MapCapture to generate a template map sprite.
6) Add the MapLevelSettings script to your scene, and set it's properties. Then everything should work!

Integration with InControl:
1) Import Survival Engine first
2) Import InControl
3) If there are no compile errors, the Scripting Define Symbol IN_CONTROL should be added automatically to the player settings. If not, add it manually.
4) Open the Managers prefabs (SurvivalEngine/Prefabs), and on PlayerControls, set to true the variable: gamepad_controls
5) Connect a USB gamepad such as Xbox One controller or other, and run the game!

