using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class PieceMovementModel
{
    [RealtimeProperty(1, true, true)]
    private int _moveSequenceIndex;

    [RealtimeProperty(2, true, true)]
    private float _currentLerpTime;
}

/* ----- Begin Normal Autogenerated Code ----- */
public partial class PieceMovementModel : RealtimeModel {
    public int moveSequenceIndex {
        get {
            return _moveSequenceIndexProperty.value;
        }
        set {
            if (_moveSequenceIndexProperty.value == value) return;
            _moveSequenceIndexProperty.value = value;
            InvalidateReliableLength();
            FireMoveSequenceIndexDidChange(value);
        }
    }
    
    public float currentLerpTime {
        get {
            return _currentLerpTimeProperty.value;
        }
        set {
            if (_currentLerpTimeProperty.value == value) return;
            _currentLerpTimeProperty.value = value;
            InvalidateReliableLength();
            FireCurrentLerpTimeDidChange(value);
        }
    }
    
    public delegate void PropertyChangedHandler<in T>(PieceMovementModel model, T value);
    public event PropertyChangedHandler<int> moveSequenceIndexDidChange;
    public event PropertyChangedHandler<float> currentLerpTimeDidChange;
    
    public enum PropertyID : uint {
        MoveSequenceIndex = 1,
        CurrentLerpTime = 2,
    }
    
    #region Properties
    
    private ReliableProperty<int> _moveSequenceIndexProperty;
    
    private ReliableProperty<float> _currentLerpTimeProperty;
    
    #endregion
    
    public PieceMovementModel() : base(null) {
        _moveSequenceIndexProperty = new ReliableProperty<int>(1, _moveSequenceIndex);
        _currentLerpTimeProperty = new ReliableProperty<float>(2, _currentLerpTime);
    }
    
    protected override void OnParentReplaced(RealtimeModel previousParent, RealtimeModel currentParent) {
        _moveSequenceIndexProperty.UnsubscribeCallback();
        _currentLerpTimeProperty.UnsubscribeCallback();
    }
    
    private void FireMoveSequenceIndexDidChange(int value) {
        try {
            moveSequenceIndexDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireCurrentLerpTimeDidChange(float value) {
        try {
            currentLerpTimeDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    protected override int WriteLength(StreamContext context) {
        var length = 0;
        length += _moveSequenceIndexProperty.WriteLength(context);
        length += _currentLerpTimeProperty.WriteLength(context);
        return length;
    }
    
    protected override void Write(WriteStream stream, StreamContext context) {
        var writes = false;
        writes |= _moveSequenceIndexProperty.Write(stream, context);
        writes |= _currentLerpTimeProperty.Write(stream, context);
        if (writes) InvalidateContextLength(context);
    }
    
    protected override void Read(ReadStream stream, StreamContext context) {
        var anyPropertiesChanged = false;
        while (stream.ReadNextPropertyID(out uint propertyID)) {
            var changed = false;
            switch (propertyID) {
                case (uint) PropertyID.MoveSequenceIndex: {
                    changed = _moveSequenceIndexProperty.Read(stream, context);
                    if (changed) FireMoveSequenceIndexDidChange(moveSequenceIndex);
                    break;
                }
                case (uint) PropertyID.CurrentLerpTime: {
                    changed = _currentLerpTimeProperty.Read(stream, context);
                    if (changed) FireCurrentLerpTimeDidChange(currentLerpTime);
                    break;
                }
                default: {
                    stream.SkipProperty();
                    break;
                }
            }
            anyPropertiesChanged |= changed;
        }
        if (anyPropertiesChanged) {
            UpdateBackingFields();
        }
    }
    
    private void UpdateBackingFields() {
        _moveSequenceIndex = moveSequenceIndex;
        _currentLerpTime = currentLerpTime;
    }
    
}
/* ----- End Normal Autogenerated Code ----- */
