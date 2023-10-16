using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UniTools;
using UniTools.Reactive;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class GridSwiper<Data, View> : IDisposable where View : Component
{
    [SerializeField, Header("Optional")] private RectTransform navigationContainer;
    [Space, SerializeField] private RectTransform pagesContainer;
    [SerializeField] private Button prevBtn, nextBtn, goToFirstBtn, goToLastBtn;
    [SerializeField] private int maxCardsShown = 8;
    [SerializeField] private ScrollRect pageScroller;
    [SerializeField] private View prefab;

    private Connections connections = new Connections();
    private IReadOnlyReactiveList<Data> allData;
    private Reactive<int> page = new Reactive<int>();
    private Reactive<int> maxPages = new Reactive<int>();
    private Action<View, Data> onShown;

    public IReadOnlyReactive<int> CurrentPage => page;

    public void Show(IReadOnlyReactiveList<Data> data, Action<View, Data> onShown, int currentPage = 0)
    {
        if (data == null) return;

        page.value = currentPage;

        this.onShown = onShown;
        allData = data;

        connections.DisconnectAll();

        connections += data.SubscribeAndInvoke(list => maxPages.value = (list.Count - 1) / maxCardsShown + 1);
        connections += data.SubscribeAndInvoke(list => PresentCurrentValues());
        connections += nextBtn?.SubscribeWithSound(() => NextPage());
        connections += prevBtn?.SubscribeWithSound(() => PrevPage());
        connections += goToFirstBtn.SubscribeWithSound(GoToFirst);
        connections += goToLastBtn.SubscribeWithSound(GoToLast);
        connections += page.Buffer((oldVal, newVal) =>
        {
            if (oldVal < newVal)
            {
                Swipe(true);
            }
            else
            {
                Swipe(false);
            }
            void Swipe(bool toNext)
            {
                pageScroller.horizontalNormalizedPosition = toNext ? 0 : 1;

                PresentCells(GetCardsOnPage(newVal), onShown, pagesContainer.GetChild(toNext ? 1 : 0));
                PresentCells(GetCardsOnPage(oldVal), onShown, pagesContainer.GetChild(toNext ? 0 : 1));

                pageScroller.DOKill();
                pageScroller.DONormalizedPos(new Vector2(toNext ? 1 : 0, 0), 0.3f);
            }
        });
        connections += page.SubscribeAndInvoke(maxPages, (currentPage, maxPages) =>
        {
            UpdateNavigation();
            nextBtn.SetActive(maxPages > 1 && currentPage < maxPages - 1);
            prevBtn.SetActive(maxPages > 1 && currentPage > 0);
        });
        
        if(page.value >= maxPages.value - 1) GoToLast();

        UpdateNavigation();
    }
    public void PresentCurrentValues()
    {
        PresentCells(GetCardsOnPage(page.value), onShown, pagesContainer.GetChild(0));
        PresentCells(GetCardsOnPage(page.value), onShown, pagesContainer.GetChild(1));
    }
    private List<Data> GetCardsOnPage(int pg) => allData.Skip(maxCardsShown * pg).Take(maxCardsShown).ToList();
    private Presenter<Data, View> PresentCells(List<Data> cards, Action<View, Data> onShown, Transform container)
    {
        return cards.Present(prefab, container.GetComponent<RectTransform>(), (view, data) =>
        {
            onShown.Invoke(view, data);
        });

    }
    private void UpdateNavigation()
    {
        if (navigationContainer == null) return;

        var navigationItem = navigationContainer.GetComponentsInChildren<CollectionNavigationButton>(true);
        navigationItem.ForEach(ni => ni.SetActive(false));

        for (int i = 0; i < navigationItem.Length && i < maxPages.value; i++)
        {
            int pageNumber = 0;

            if (page.value < navigationItem.Length / 2 || maxPages.value < navigationItem.Length)
            {
                pageNumber = i;
            }
            else
            {
                if (page.value > maxPages.value - navigationItem.Length / 2) pageNumber = i + maxPages.value - navigationItem.Length;
                else pageNumber = i + page.value - navigationItem.Length / 2;
            }

            navigationItem[i].Show((pageNumber + 1).ToString(), () =>
            {
                if (pageNumber >= maxPages.value) return;
                page.value = pageNumber;
            });
            navigationItem[i].Highlight(pageNumber == page.value);
            navigationItem[i].SetActive(true);
        }
    }
    public void NextPage()
    {
        if (page.value >= maxPages.value - 1) return;
        page.value++;
    }
    public void PrevPage()
    {
        if (page.value <= 0) return;
        page.value--;
    }
    public void GoToFirst()
    {
        page.value = 0;
    }
    public void GoToLast()
    {
        page.value = maxPages.value - 1;
    }

    public void Dispose()
    {
        connections.DisconnectAll();
    }
}
