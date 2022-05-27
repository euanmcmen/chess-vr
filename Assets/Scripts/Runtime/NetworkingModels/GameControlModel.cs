using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class GameControlModel
{
    [RealtimeProperty(1, true, true)]
    private int _currentTurnIndex;

    [RealtimeProperty(2, true, true)]
    private int _currentTurnTeamIndex;

    [RealtimeProperty(3, true, true)]
    private int _currentTurnProgressIndex;
}

/* ----- Begin Normal Autogenerated Code ----- */
public partial class GameControlModel : RealtimeModel {
    public int currentTurnIndex {
        get {
            return _currentTurnIndexProperty.value;
        }
        set {
            if (_currentTurnIndexProperty.value == value) return;
            _currentTurnIndexProperty.value = value;
            InvalidateReliableLength();
            FireCurrentTurnIndexDidChange(value);
        }
    }
    
    public int currentTurnTeamIndex {
        get {
            return _currentTurnTeamIndexProperty.value;
        }
        set {
            if (_currentTurnTeamIndexProperty.value == value) return;
            _currentTurnTeamIndexProperty.value = value;
            InvalidateReliableLength();
            FireCurrentTurnTeamIndexDidChange(value);
        }
    }
    
    public int currentTurnProgressIndex {
        get {
            return _currentTurnProgressIndexProperty.value;
        }
        set {
            if (_currentTurnProgressIndexProperty.value == value) return;
            _currentTurnProgressIndexProperty.value = value;
            InvalidateReliableLength();
            FireCurrentTurnProgressIndexDidChange(value);
        }
    }
    
    public delegate void PropertyChangedHandler<in T>(GameControlModel model, T value);
    public event PropertyChangedHandler<int> currentTurnIndexDidChange;
    public event PropertyChangedHandler<int> currentTurnTeamIndexDidChange;
    public event PropertyChangedHandler<int> currentTurnProgressIndexDidChange;
    
    public enum PropertyID : uint {
        CurrentTurnIndex = 1,
        CurrentTurnTeamIndex = 2,
        CurrentTurnProgressIndex = 3,
    }
    
    #region Properties
    
    private ReliableProperty<int> _currentTurnIndexProperty;
    
    private ReliableProperty<int> _currentTurnTeamIndexProperty;
    
    private ReliableProperty<int> _currentTurnProgressIndexProperty;
    
    #endregion
    
    public GameControlModel() : base(null) {
        _currentTurnIndexProperty = new ReliableProperty<int>(1, _currentTurnIndex);
        _currentTurnTeamIndexProperty = new ReliableProperty<int>(2, _currentTurnTeamIndex);
        _currentTurnProgressIndexProperty = new ReliableProperty<int>(3, _currentTurnProgressIndex);
    }
    
    protected override void OnParentReplaced(RealtimeModel previousParent, RealtimeModel currentParent) {
        _currentTurnIndexProperty.UnsubscribeCallback();
        _currentTurnTeamIndexProperty.UnsubscribeCallback();
        _currentTurnProgressIndexProperty.UnsubscribeCallback();
    }
    
    private void FireCurrentTurnIndexDidChange(int value) {
        try {
            currentTurnIndexDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireCurrentTurnTeamIndexDidChange(int value) {
        try {
            currentTurnTeamIndexDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireCurrentTurnProgressIndexDidChange(int value) {
        try {
            currentTurnProgressIndexDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    protected override int WriteLength(StreamContext context) {
        var length = 0;
        length += _currentTurnIndexProperty.WriteLength(context);
        length += _currentTurnTeamIndexProperty.WriteLength(context);
        length += _currentTurnProgressIndexProperty.WriteLength(context);
        return length;
    }
    
    protected override void Write(WriteStream stream, StreamContext context) {
        var writes = false;
        writes |= _currentTurnIndexProperty.Write(stream, context);
        writes |= _currentTurnTeamIndexProperty.Write(stream, context);
        writes |= _currentTurnProgressIndexProperty.Write(stream, context);
        if (writes) InvalidateContextLength(context);
    }
    
    protected override void Read(ReadStream stream, StreamContext context) {
        var anyPropertiesChanged = false;
        while (stream.ReadNextPropertyID(out uint propertyID)) {
            var changed = false;
            switch (propertyID) {
                case (uint) PropertyID.CurrentTurnIndex: {
                    changed = _currentTurnIndexProperty.Read(stream, context);
                    if (changed) FireCurrentTurnIndexDidChange(currentTurnIndex);
                    break;
                }
                case (uint) PropertyID.CurrentTurnTeamIndex: {
                    changed = _currentTurnTeamIndexProperty.Read(stream, context);
                    if (changed) FireCurrentTurnTeamIndexDidChange(currentTurnTeamIndex);
                    break;
                }
                case (uint) PropertyID.CurrentTurnProgressIndex: {
                    changed = _currentTurnProgressIndexProperty.Read(stream, context);
                    if (changed) FireCurrentTurnProgressIndexDidChange(currentTurnProgressIndex);
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
        _currentTurnIndex = currentTurnIndex;
        _currentTurnTeamIndex = currentTurnTeamIndex;
        _currentTurnProgressIndex = currentTurnProgressIndex;
    }
    
}
/* ----- End Normal Autogenerated Code ----- */