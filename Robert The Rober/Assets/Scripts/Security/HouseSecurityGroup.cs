using System.Collections.Generic;
using UnityEngine;

public class HouseSecurityGroup : MonoBehaviour
{
    [SerializeField] private string houseId;
    [SerializeField] private PowerBoxController powerBox;
    [SerializeField] private List<SecurityCameraController> cameras = new();

    public string HouseId => houseId;
    public PowerBoxController PowerBox => powerBox;
    public List<SecurityCameraController> Cameras => cameras;
}