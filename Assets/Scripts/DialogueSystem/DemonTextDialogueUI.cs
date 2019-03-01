using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;

namespace Yarn.Unity.Example {
    /// Displays dialogue lines to the player, and sends
    /// user choices back to the dialogue system.

    /** Note that this is just one way of presenting the
     * dialogue to the user. The only hard requirement
     * is that you provide the RunLine, RunOptions, RunCommand
     * and DialogueComplete coroutines; what they do is up to you.
     */
    public class DemonTextDialogueUI : Yarn.Unity.DialogueUIBehaviour
    {
        public IEnumerator DialogueStarted()
        {
            yield break;
        }

        public IEnumerator RunLine(Yarn.Line line)
        {
            
        }

        public IEnumerator RunOptions(Yarn.Options optionsCollection, Yarn.OptionChooser optionChooser)
        {
            
        }

        public IEnumerator RunCommand(Yarn.Command command)
        {
            
        }

        public IEnumerator NodeComplete(string nextNode)
        {
            yield break;
        }

        public IEnumerator DialogueComplete()
        {
            yield break;
        }

    }

}
