using UnityEngine;

// Game Specific - Replace For New Game
using UniverseLib.Input;

namespace RMMBY.Editable
{
    // Input Handler Used By RMMBY
    // Example Built For RWBY: Arrowfell
    // Modify This Script To Use New Games Input System
    public class InputHandler : MonoBehaviour
    {
        // Required Variables - Do Not Delete
        public bool up;
        public bool down;
        public bool left;
        public bool right;
        public bool select;
        public bool back;
        public bool active;
        public bool refresh;
        public bool update;
        public bool toggleConsole;
        // End Of Required Variables

        // Game Specific Input System
        

        // Runs The First Frame After This Input Handler Is Created
        private void Start()
        {
            // Example Code For RWBY: Arrowfell
            // Replace Or Remove For New Game's Input System
            
        }

        // Only Runs When The RMMBY Mod Menu Is Loaded
        public void OnSceneLoaded()
        {
            // Example Code For RWBY: Arrowfell
            // Replace Or Remove For New Game's Input System
        }

        // Will Run Every Frame
        private void Update()
        {
            // Used To Show/Hide The MelonLoader Console
            // Should Always Run - Run Before Checking If The Input Handler Is Active
            // Modify For Your Input System
            if (InputManager.GetKeyDown(KeyCode.P))
            {
                toggleConsole = true;
            }
            else
            {
                toggleConsole = false;
            }

            // Active Should Only True While The Mod Menu Is Loaded - Handled By RMMBY
            // All Inputs Should Be Set To False When Not In The Mod Menu
            // Don't Modify This
            if (!active)
            {
                up = false;
                down = false;
                left = false;
                right = false;
                select = false;
                back = false;

                return;
            }

            // RMMBY's Mod Menu Will Only Use This Input Handler's Booleans
            // The Rest Of This Script Converts The Input System From RWBY: Arrowfell To RMMBY's Booleans
            // Modify Below For Your Input System

            // Moves Selection Up When Pressed
            if (Singleton<GameInput>.Instance.GetAxis(GameInput.GameActions.MoveVertical, GameInput.InputType.Menu) > 0.2f)
            {
                up = true;
            }
            else
            {
                up = false;
            }

            // Moves Selection Down When Pressed
            if (Singleton<GameInput>.Instance.GetAxis(GameInput.GameActions.MoveVertical, GameInput.InputType.Menu) < -0.2f)
            {
                down = true;
            }
            else
            {
                down = false;
            }

            // Moves Selection Left When Pressed
            if (Singleton<GameInput>.Instance.GetAxis(GameInput.GameActions.MoveHorizontal, GameInput.InputType.Menu) < -0.2f)
            {
                left = true;
            }
            else
            {
                left = false;
            }

            // Moves Selection Right When Pressed
            if (Singleton<GameInput>.Instance.GetAxis(GameInput.GameActions.MoveHorizontal, GameInput.InputType.Menu) > 0.2f)
            {
                right = true;
            }
            else
            {
                right = false;
            }

            if (InputManager.GetKey(KeyCode.O))
            {
                update = true;
            }
            else
            {
                update = false;
            }

            if (InputManager.GetKey(KeyCode.R))
            {
                refresh = true;
            }
            else
            {
                refresh = false;
            }

            // Returns To Mod List When In Individual Mod Settings, And Saves Mod Settings
            // Loads ListenToLoadMenu.sceneToReturnTo When In Mod List Or On Mod Menu Error Display
            back = Singleton<GameInput>.Instance.GetButton(GameInput.GameActions.UICancel, GameInput.InputType.Menu);

            // Loads Individual Mod Settings When In Mod List
            select = Singleton<GameInput>.Instance.GetButton(GameInput.GameActions.UISubmit, GameInput.InputType.Menu);
        }
    }
}