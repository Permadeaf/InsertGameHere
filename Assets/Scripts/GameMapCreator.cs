using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMapCreator : MonoBehaviour
{
    [SerializeField]
    ObjectPool roomPool;

    [SerializeField]
    MapGenerator generator;

    [SerializeField]
    Vector2 dimensions;

    [SerializeField]
    Vector2 spacing;

    [SerializeField]
    Player player;

    [SerializeField]
    GameEvent onSetPlayerStartingPosition;

    [Header("Rooms")]

    [SerializeField]
    List<RoomDataSO> codedRooms;

    [SerializeField]
    List<DesignerRoom> designedRooms;

    List<EverythingARoomNeedsForSpawn> randomRooms = new List<EverythingARoomNeedsForSpawn>();

    [SerializeField]
    RoomDataSO coinRoom;

    [SerializeField]
    RoomDataSO cabinetRoom;

    [SerializeField]
    RoomDataSO cartridgeRoom;

    [Header("Themes")]
    [SerializeField]
    RoomThemeSO specialRoomTheme;

    [SerializeField]
    List<RoomThemeSO> otherThemes;

    MapGenerator.MapData mapData;

    Dictionary<Vector2Int, Room> roomDictionary = new Dictionary<Vector2Int, Room>();

    RoomThemeSO chosenTheme;

    List<Vector2Int> specialRoomLocations = new List<Vector2Int>();

    int currentMapChallengeRating;

    // Start is called before the first frame update
    void Start()
    {
        AddRandomRooms();
        GenerateGameMap();
    }

    void Initialize()
    {
        chosenTheme = otherThemes.RandomItem();
        roomPool.DisableAll();
        mapData = generator.GenerateMap();
        specialRoomLocations.Clear();
        roomDictionary.Clear();
        currentMapChallengeRating = mapData.TotalChallengeRating;
    }

    public void GenerateGameMap()
    {
        Initialize();
        for (int x = 0; x < mapData.Map.GetLength(0); x++)
        {
            for (int y = 0; y < mapData.Map.GetLength(1); y++)
            {
                if (mapData.Map[x, y])
                {
                    Vector2 position = new Vector2(x * dimensions.x + x * spacing.x, y * dimensions.y + y * spacing.y);
                    var room = roomPool.GetObject();
                    room.transform.position = position;
                    var roomData = room.GetComponent<Room>();
                    if (roomData != null)
                    {
                        roomData.RoomLocation = new Vector2Int(x, y);
                        roomData.CanUpOpen = mapData.HasNeighborUp(roomData.RoomLocation);
                        roomData.CanDownOpen = mapData.HasNeighborDown(roomData.RoomLocation);
                        roomData.CanLeftOpen = mapData.HasNeighborLeft(roomData.RoomLocation);
                        roomData.CanRightOpen = mapData.HasNeighborRight(roomData.RoomLocation);
                        roomData.IsOpen = false;
                        roomData.IsOpen = true;
                        if (roomData.RoomLocation == mapData.StartingPosition)
                        {
                            player.transform.position = roomData.CenterPlayerSpawn.transform.position;
                            player.CurrentRoomLocation = roomData.RoomLocation;
                        }
                        roomDictionary.Add(roomData.RoomLocation, roomData);
                    }
                    room.SetActive(true);

                }
            }
        }

        CreateSpecialRooms();
        CreateGeneralRooms();
        SetThemeForSpecialRooms();

        onSetPlayerStartingPosition.Invoke();

        // player.transform.position = new Vector3(mapData.StartingPosition.x * dimensions.x + spacing.x * mapData.StartingPosition.x,
        //     mapData.StartingPosition.y * dimensions.y + spacing.y + mapData.StartingPosition.y);
    }

    void AddRandomRooms()
    {
        foreach (var room in designedRooms)
        {
            randomRooms.Add(room);
        }
        foreach (var room in codedRooms)
        {
            randomRooms.Add(room);
        }
    }

    void CreateGeneralRooms()
    {
        foreach (var keyVal in roomDictionary)
        {
            //TODO: Should we restrict to the used special rooms?
            keyVal.Value.OnSetTheme.Invoke(chosenTheme);
            if (keyVal.Key == mapData.StartingPosition || mapData.DeadEnds.Contains(keyVal.Key))
            {
                continue;
            }

            int challengeRating = 0;

            if (currentMapChallengeRating > 0)
            {
                challengeRating = UnityEngine.Random.Range(1, mapData.MaxChallengeRatingPerRoom + 1);
                currentMapChallengeRating -= challengeRating;
            }

            keyVal.Value.GenerateRoomStuff(randomRooms.RandomItem(), challengeRating);
        }
    }


    void CreateSpecialRooms()
    {
        mapData.DeadEnds.Shuffle();
        Vector2Int coinRoomLocation, cartridgeRoomLocation, cabinetRoomLocation;

        coinRoomLocation = mapData.DeadEnds[0];
        cartridgeRoomLocation = mapData.DeadEnds[1];
        cabinetRoomLocation = mapData.DeadEnds[2];

        specialRoomLocations.Add(coinRoomLocation);
        specialRoomLocations.Add(cartridgeRoomLocation);
        specialRoomLocations.Add(cabinetRoomLocation);

        roomDictionary[coinRoomLocation].GenerateRoomStuff(coinRoom, 0);
        roomDictionary[cartridgeRoomLocation].GenerateRoomStuff(cartridgeRoom, 0);
        roomDictionary[cabinetRoomLocation].GenerateRoomStuff(cabinetRoom, 0);
    }

    void SetThemeForSpecialRooms()
    {
        foreach (var location in specialRoomLocations)
        {
            roomDictionary[location].OnSetTheme.Invoke(specialRoomTheme);
        }
    }

    public Room GetRoomAtLocation(Vector2Int location)
    {
        if (!roomDictionary.ContainsKey(location))
        {
            return null;
        }
        return roomDictionary[location];
    }
}
