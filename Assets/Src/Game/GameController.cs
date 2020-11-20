using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    ImageLoader _imgLoader = default;

    [SerializeField]
    GameObject _preloader = default;

    [SerializeField]
    DeckData _deckData = default;

    [SerializeField]
    DeckHolder _userDeck = default;

    [SerializeField]
    int _cardCount = default;

    [SerializeField]
    int _dmgMin = default;

    [SerializeField]
    int _dmgMax = default;

    [SerializeField]
    int _targetMin = default;

    [SerializeField]
    int _targetMax = default;

    IEnumerator Start()
    {
        _preloader.SetActive(true);
        yield return StartCoroutine(_imgLoader.StartLoading(_deckData.ImageIds));
        _preloader.SetActive(false);

        CreateDesk();
    }

    [ContextMenu("RecreateDesk")]
    void CreateDesk()
    {
        _userDeck.SpawnCard(_cardCount);
        _userDeck.AnimateDeck();
    }

    public void OnApplyDamageCklick()
    {
        _userDeck.ApplyDamage(Random.Range(_dmgMin, _dmgMax + 1), Random.Range(_targetMin, _targetMax + 1));
    }
}
