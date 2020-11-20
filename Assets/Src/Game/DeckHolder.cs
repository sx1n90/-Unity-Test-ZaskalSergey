using System.Collections.Generic;
using UnityEngine;

public class DeckHolder : MonoBehaviour
{
    [SerializeField]
    DeckData _deckData = default;

    [SerializeField]
    GameObject _cardPrefab = default;

    [SerializeField]
    RectTransform _spawnPoint = default;

    [SerializeField]
    float _arcRadius = default;

    [SerializeField]
    float _arcXOffset = default;

    [SerializeField]
    [Range(0f, 45f)]
    float _arcAngleForOneCard = default;

    [SerializeField]
    float _maxArcAngle = default;

    List<CardView> _cards = new List<CardView>();

    int _lastDmgIdx = 0;

    private void OnDestroy()
    {
        foreach (var item in _cards)
        {
            item.OnStartDragAction -= OnCardDragStart;
            item.OnEndDragAction -= OnCardDragEnd;
        }
    }

    public List<CardView> SpawnCard(int count, bool random = true)
    {
        if(_cards.Count > 0)
        {
            foreach (var item in _cards)
            {
                Destroy(item.gameObject);
            }
            _cards.Clear();
        }

        for (int i = 0; i < count; i++)
        {
            var card = CreateCard(random ? -1 : i);
            if(card != null)
            {
                _cards.Add(card);
            }
        }
        return _cards;
    }

    private CardView CreateCard(int id)
    {
        var cardGo = Instantiate(_cardPrefab, _spawnPoint);
        if (cardGo == null)
        {
            Debug.LogError("Can't instantiate card");
            return null;
        }
        
        var card = cardGo.GetComponent<CardView>();
        if (card == null)
        {
            Debug.LogError("Can't GetComponent from card");
            return null;
        }

        var data = id < 0 ? _deckData.GetCardDataRandom() : _deckData.GetCardDataByID(id);
        if (data == null)
        {
            Debug.LogError($"Can't get data for card id {id}");
            Destroy(cardGo);
            return null;
        }

        card.Init(data);
        card.OnStartDragAction += OnCardDragStart;
        card.OnEndDragAction += OnCardDragEnd;


        return card;
    }

    [ContextMenu("AnimateDeck")]
    public void AnimateDeck()
    {
        float arcAngleForOneCard = _arcAngleForOneCard;
        if (_arcAngleForOneCard * _cards.Count > _maxArcAngle)
        {
            arcAngleForOneCard = _maxArcAngle / _cards.Count;
        }

        float angleOffset = 90f + arcAngleForOneCard * (_cards.Count - 1) * 0.5f;

        for (int i = 0; i < _cards.Count; i++)
        {
            float angle = angleOffset - arcAngleForOneCard * i;
            var pos = new Vector2((_arcRadius + _arcXOffset) * Mathf.Cos(Mathf.Deg2Rad * angle), _arcRadius * Mathf.Sin(Mathf.Deg2Rad * angle));
            _cards[i].AnimateMove(pos, angle);
        }
    }

    public void ApplyDamage(int dmg, int target)
    {
        if(_cards.Count == 0)
        {
            return;
        }
        var card = _cards[_lastDmgIdx];

        if(card.ApplayDamage(dmg, target))
        {
            card.OnStartDragAction -= OnCardDragStart;
            card.OnEndDragAction -= OnCardDragEnd;

            _cards.RemoveAt(_lastDmgIdx);
            AnimateDeck();
        }

        _lastDmgIdx++;
        if (_lastDmgIdx >= _cards.Count)
        {
            _lastDmgIdx = 0;
        }
    }

    void OnCardDragStart(CardView card)
    {
        _cards.Remove(card);
        AnimateDeck();
    }

    void OnCardDragEnd(CardView card)
    {
        if(card.Transform.parent != transform)
        {
            return;
        }
        _cards.Add(card);
        AnimateDeck();
    }
}
