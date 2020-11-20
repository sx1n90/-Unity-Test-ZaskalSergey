using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static DeckData;

public class CardView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    Transform _tr = default;

    [SerializeField]
    RawImage _image = default;

    [SerializeField]
    Text _title = default;

    [SerializeField]
    Text _description = default;

    [SerializeField]
    Text _manaTxt = default;

    [SerializeField]
    Text _attackTxt = default;

    [SerializeField]
    Text _hpTxt = default;

    [SerializeField]
    GameObject _shine = default;

    [SerializeField]
    CanvasGroup _canvasGroup = default;

    public Action<CardView> OnStartDragAction;
    public Action<CardView> OnEndDragAction;

    public Transform Transform { get; private set; }

    CardData _cardData = default;

    int _mana;
    int _attack;
    int _hp;

    Transform _oldParrent;

    public void Init(CardData data)
    {
        Transform = _tr;

        _cardData = data;

        var texture = ImageLoader.GetImage(_cardData.ImageId);
        if(texture != null)
        {
            _image.texture = texture;
        }

        _mana = _cardData.Mana;
        _attack = _cardData.Attack;
        _hp = _cardData.HP;

        _title.text = _cardData.Title;
        _description.text = _cardData.Description;
        _manaTxt.text = _mana.ToString();
        _attackTxt.text = _attack.ToString();
        _hpTxt.text = _hp.ToString();
    }

    public bool ApplayDamage(int dmg, int target = 0)
    {
        int newValue;
        switch (target)
        {
            case 0:
                newValue = Mathf.Max(_hp - dmg, 0);
                StartCoroutine(AnimateDamage(_hpTxt, _hp, newValue, newValue == 0));
                _hp = newValue;
                return _hp == 0;

            case 1:
                newValue = Mathf.Max(_mana - dmg, 0);
                StartCoroutine(AnimateDamage(_manaTxt, _mana, newValue));
                _mana = newValue;
                return false;

            case 2:
                newValue = Mathf.Max(_attack - dmg, 0);
                StartCoroutine(AnimateDamage(_attackTxt, _attack, newValue));
                _attack = newValue;
                return false;


            default:
                Debug.LogError($"Wrong target {target}");
                return false;
        }
    }

    public void AnimateMove(Vector2 pos, float angle)
    {
        _tr.DOLocalMove(pos, 0.3f);
        _tr.DOLocalRotateQuaternion(Quaternion.AngleAxis(angle - 90f, Vector3.forward), 0.3f);
    }
    public void AnimateDeath()
    {
        _tr.DOLocalMove(new Vector3(0, 600f), 0.3f);
        _tr.DOLocalRotateQuaternion(Quaternion.identity, 0.3f);
        Destroy(gameObject, 1.0f);
    }

    IEnumerator AnimateDamage(Text txtField, int from, int to, bool death = false)
    {
        var pos = _tr.localPosition;
        var rot = _tr.localRotation;
        var sindex = _tr.GetSiblingIndex();

        _tr.DOLocalMove(new Vector3(0, 400f), 0.1f);
        _tr.DOLocalRotateQuaternion(Quaternion.identity, 0.1f);
        _tr.SetSiblingIndex(int.MaxValue);

        yield return new WaitForSeconds(0.2f);

        var idx = from;
        var add = from < to ? 1 : -1;
        while(idx != to)
        {
            idx += add;
            txtField.text = idx.ToString();
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.2f);

        if (death)
        {
            _tr.DOScale(0, 2f).OnComplete(()=> Destroy(gameObject));
        }
        else
        {
            _tr.DOLocalMove(pos, 0.1f);
            _tr.DOLocalRotateQuaternion(rot, 0.1f);
            _tr.SetSiblingIndex(sindex);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnStartDragAction?.Invoke(this);
        _shine.SetActive(true);
        _oldParrent = _tr.parent;
        _tr.SetParent(MainCanvas.CanvasTransform);
        _tr.localRotation = Quaternion.identity;
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _tr.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(_tr.parent == MainCanvas.CanvasTransform)
        {
            _tr.SetParent(_oldParrent);
            _canvasGroup.blocksRaycasts = true;
        }
        OnEndDragAction?.Invoke(this);
        _shine.SetActive(false);
    }
}
