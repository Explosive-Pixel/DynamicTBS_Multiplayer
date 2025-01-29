using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SetupMapHandler : MonoBehaviour, ISetupHandler
{
    [SerializeField] private MapLayout mapPreview;

    private bool mapSelected = false;

    public bool SetupCompleted { get { return mapSelected; } }

    private void Awake()
    {
        MapClass[] maps = gameObject.GetComponentsInChildren<MapClass>(true);
        for (int i = 0; i < maps.Length; i++)
        {
            MapClass map = maps[i];
            Button button = map.GetComponent<Button>();
            button.onClick.AddListener(() => ChooseBoardDesign(button, map.mapType));

            if (GameSetup.MapSetup != null && GameSetup.MapSetup.MapType == map.mapType)
            {
                SetMap(button, map.mapType);
            }
        }
    }

    private void ChooseBoardDesign(Button button, MapType mapType)
    {
        AudioEvents.PressingButton();

        GameSetup.SetupMap(new MapSetup(mapType));
        SetMap(button, mapType);
    }

    private void SetMap(Button button, MapType mapType)
    {
        GameObject parent = button.gameObject.transform.parent.gameObject;
        if (!parent.activeSelf)
            gameObject.GetComponent<BaseActiveHandler>().SetActive(parent);

        mapPreview.Init(mapType);
        mapSelected = true;

        gameObject.GetComponentsInChildren<Button>(true).ToList().ForEach(button => button.interactable = true);
        button.interactable = false;
    }

    private void OnEnable()
    {
        if (mapSelected)
            mapPreview.SetActive(true);
    }

    private void OnDisable()
    {
        if (mapPreview != null && mapSelected)
            mapPreview.SetActive(false);
    }
}
