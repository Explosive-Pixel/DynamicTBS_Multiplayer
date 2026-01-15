using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollableLobbyList : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    public RectTransform viewport;
    public RectTransform content;
    public GameObject lobbyItemPrefab;

    [Header("Layout")]
    public float itemHeight = 150f;
    public float itemWidth = 430f;
    public float paddingLeft = 5f;
    public float paddingRight = 15f;
    public float spacing = 10f;

    [Header("Scroll")]
    public float scrollSpeed = 5000;
    public float dragSpeed = 10f;

    private float contentHeight;
    private Vector2 dragStartPos;
    private Vector2 contentStartPos;

    private readonly List<GameObject> items = new();

    // ----------------------------
    // PUBLIC API
    // ----------------------------

    public void SetLobbies(LobbyInfo[] lobbies, Lobby selectedLobby)
    {
        Clear();

        OnlineMenuScreenHandler onlineMenuScreenHandler = gameObject.GetComponentInParent<OnlineMenuScreenHandler>(true);
        foreach (LobbyInfo lobbyInfo in lobbies)
        {
            GameObject lobbyInfoGO = GameObject.Instantiate(lobbyItemPrefab);
            lobbyInfoGO.transform.SetParent(content.transform);
            lobbyInfoGO.GetComponent<LobbyInfoHandler>().Init(lobbyInfo, selectedLobby);
            ChangeActiveGameObjectOnClickHandler clickHandler = lobbyInfoGO.GetComponent<ChangeActiveGameObjectOnClickHandler>();
            clickHandler.activeHandler = onlineMenuScreenHandler.MidActiveHandler;
            clickHandler.activeOnClickGameObject = onlineMenuScreenHandler.LobbyInfoMenu;
            lobbyInfoGO.GetComponent<Button>().onClick.AddListener(() =>
            {
                AudioEvents.PressingButton();
                MenuEvents.ChangeLobbySelection(lobbyInfoGO.GetComponent<LobbyInfoHandler>().Lobby);
            });

            items.Add(lobbyInfoGO);
        }

        RebuildLayout();
    }

    public void Clear()
    {
        foreach (var item in items)
            Destroy(item);

        items.Clear();
        content.anchoredPosition = Vector2.zero;
    }

    // ----------------------------
    // LAYOUT
    // ----------------------------

    void RebuildLayout()
    {
        float viewportWidth = viewport.rect.width;
        float aspectRatio = itemHeight / itemWidth; // Höhe / Breite der Item-Grafik

        float yOffset = paddingLeft; // oben lassen wir das alte padding für vertikale Richtung, falls du möchtest, kannst du ein separates topPadding einbauen

        // Content korrekt horizontal stretchen
        content.anchorMin = new Vector2(0, 1);
        content.anchorMax = new Vector2(1, 1);
        content.pivot = new Vector2(0.5f, 1);
        content.anchoredPosition = Vector2.zero;

        for (int i = 0; i < items.Count; i++)
        {
            RectTransform rt = items[i].GetComponent<RectTransform>();
            rt.SetParent(content, false);
            rt.localScale = Vector3.one;

            // Item korrekt verankern
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);

            // Breite und Höhe
            float width = viewportWidth - paddingLeft - paddingRight;
            float height = width * aspectRatio;

            rt.sizeDelta = new Vector2(0, height); // width = 0 → Stretch horizontal
            rt.offsetMin = new Vector2(paddingLeft, rt.offsetMin.y);   // links
            rt.offsetMax = new Vector2(-paddingRight, rt.offsetMax.y); // rechts

            // Vertikale Position
            rt.anchoredPosition = new Vector2(0, -yOffset);

            yOffset += height + spacing;
        }

        contentHeight = yOffset;
        content.sizeDelta = new Vector2(0, contentHeight);
    }



    // ----------------------------
    // SCROLL INPUT
    // ----------------------------

    void Update()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            Scroll(scroll * scrollSpeed * Time.deltaTime);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPos = eventData.position;
        contentStartPos = content.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float deltaY = (eventData.position.y - dragStartPos.y) * dragSpeed;
        Scroll(deltaY, true);
    }

    public void OnEndDrag(PointerEventData eventData) { }

    // ----------------------------
    // CORE SCROLL LOGIC
    // ----------------------------

    void Scroll(float delta, bool absolute = false)
    {
        float newY = absolute
            ? contentStartPos.y + delta
            : content.anchoredPosition.y + delta;

        float maxScroll = Mathf.Max(0, contentHeight - viewport.rect.height);
        newY = Mathf.Clamp(newY, 0, maxScroll);

        content.anchoredPosition = new Vector2(0, newY);
    }

}

