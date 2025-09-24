using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "VendorData", menuName = "Vendors/VendorData")]
public class VendorData : ScriptableObject
{
    public string vendorName;
    public Image vendorAvatar;
    public int entryCost;

    [Header("MiniGame")]
    public MiniGameDefinition miniGame;
}
