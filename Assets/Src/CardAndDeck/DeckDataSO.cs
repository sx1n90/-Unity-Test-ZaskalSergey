using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "DeckData", menuName = "ScriptableObjects/DeckData", order = 10)]
public class DeckData : ScriptableObject
{
    [Serializable]
    public class CardData
    {
        public int Id;
        public int ImageId;
        public string Title;
        public string Description;
        public int Mana;
        public int Attack;
        public int HP;
    }

    [SerializeField]
    string _name = default;

    [SerializeField]
    List<CardData> _deck = new List<CardData>();

    public IEnumerable<int> ImageIds { get { return _deck.Select(card => card.ImageId); } }

    public CardData GetCardDataByID(int id)
    {
        return _deck.FirstOrDefault(card => card.Id == id);
    }

    public CardData GetCardDataRandom()
    {
        return _deck[Random.Range(0, _deck.Count)];
    }
}
