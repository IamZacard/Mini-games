using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New VendorData", menuName = "Vendors/VendorData")]
public class VendorData : ScriptableObject
{
    public string vendorName;
    public Image vendorAvatar;
    public int entryCost;
}
