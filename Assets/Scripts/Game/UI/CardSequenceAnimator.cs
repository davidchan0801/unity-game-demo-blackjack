using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSequenceAnimator : MonoBehaviour
{
    [SerializeField] private List<CardFrame> _sprites = new List<CardFrame>();
    [SerializeField] private float _frameRate = 1f/24f;
    [SerializeField] private int _frame = 0;

    [SerializeField] private bool _loop = false;

    private float _time = 0f;
    private bool pause1 = false;

    private int _direction = 1;

    public System.Action onFinish;

    void Awake()
    {
        if (_sprites.Count > 0) {
            
        }
        pause1 = true;
    }

    void Update()
    {
        _time += Time.deltaTime;

        if(_time > _frameRate)
        {
            _time = 0f;
            if (!pause1)
                NextFrame();
        }
    }

    public void AddFrame(CardFrame obj) {
        _sprites.Add(obj);
    }

    public void Reset() {
        if (_sprites.Count > 0) {
            for (int i=0;i<_sprites.Count;i++) {
                _sprites[i].ShowFrontSprite();
            }
        }
    }

    public void Play() {
        pause1 = false;
        _frame = 0;
        _time = 0f;
    }

    public void Pause() {
        pause1 = true;
    }

    private void NextFrame()
    {
        if (_sprites != null && _sprites.Count > 0) {
            _sprites[_frame].gameObject.SetActive(!_sprites[_frame].gameObject.activeSelf);
            _frame += _direction;

            if (_frame == _sprites.Count) {
                if (_loop)
                    _frame = 0;
                else {
                    pause1 = true;
                    _frame = 0;
                    onFinish?.Invoke();
                }
            }
        }
    }
}