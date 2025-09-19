using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Dialog", menuName = "Dialog")]
public class Dialog : ScriptableObject
{
    [System.Serializable]
    public class DialogLine
    {
        public string speakerName;
        public Sprite speakerAvatar;
        [TextArea] public string text;
        public List<string> choices;

        public VendorData vendorData;
    }

    public List<DialogLine> lines;
}