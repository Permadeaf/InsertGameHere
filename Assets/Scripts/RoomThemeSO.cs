using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomTheme", menuName = "Room/Room Theme")]
public class RoomThemeSO : ScriptableObject
{
    [SerializeField]
    Sprite background;

    [SerializeField]
    Sprite wall;

    [SerializeField]
    Sprite obstacle;

    public Sprite Background { get => background; }
    public Sprite Wall { get => wall; }
    public Sprite Obstacle { get => obstacle; }
}
