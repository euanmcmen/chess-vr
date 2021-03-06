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

    [RealtimeProperty(4, true, true)]
    private int _lastPlayedSequenceId;
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
    
    public int lastPlayedSequenceId {
        get {
            return _lastPlayedSequenceIdProperty.value;
        }
        set {
            if (_lastPlayedSequenceIdProperty.value == value) return;
            _lastPlayedSequenceIdProperty.value = value;
            InvalidateReliableLength();
            FireLastPlayedSequenceIdDidChange(value);
        }
    }
    
    public delegate void PropertyChangedHandler<in T>(GameControlModel model, T value);
    public event PropertyChangedHandler<int> currentTurnIndexDidChange;
    public event PropertyChangedHandler<int> currentTurnTeamIndexDidChange;
    public event PropertyChangedHandler<int> currentTurnProgressIndexDidChange;
    public event PropertyChangedHandler<int> lastPlayedSequenceIdDidChange;
    
    public enum PropertyID : uint {
        CurrentTurnIndex = 1,
        CurrentTurnTeamIndex = 2,
        CurrentTurnProgressIndex = 3,
        LastPlayedSequenceId = 4,
    }
    
    #region Properties
    
    private ReliableProperty<int> _currentTurnIndexProperty;
    
    private ReliableProperty<int> _currentTurnTeamIndexProperty;
    
    private ReliableProperty<int> _currentTurnProgressIndexProperty;
    
    private ReliableProperty<int> _lastPlayedSequenceIdProperty;
    
    #endregion
    
    public GameControlModel() : base(null) {
        _currentTurnIndexProperty = new ReliableProperty<int>(1, _currentTurnIndex);
        _currentTurnTeamIndexProperty = new ReliableProperty<int>(2, _currentTurnTeamIndex);
        _currentTurnProgressIndexProperty = new ReliableProperty<int>(3, _currentTurnProgressIndex);
        _lastPlayedSequenceIdProperty = new ReliableProperty<int>(4, _lastPlayedSequenceId);
    }
    
    protected override void OnParentReplaced(RealtimeModel previousParent, RealtimeModel currentParent) {
        _currentTurnIndexProperty.UnsubscribeCallback();
        _currentTurnTeamIndexProperty.UnsubscribeCallback();
        _currentTurnProgressIndexProperty.UnsubscribeCallback();
        _lastPlayedSequenceIdProperty.UnsubscribeCallback();
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
    
    private void FireLastPlayedSequenceIdDidChange(int value) {
        try {
            lastPlayedSequenceIdDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    protected override int WriteLength(StreamContext context) {
        var length = 0;
        length += _currentTurnIndexProperty.WriteLength(context);
        length += _currentTurnTeamIndexProperty.WriteLength(context);
        length += _currentTurnProgressIndexProperty.WriteLength(context);
        length += _lastPlayedSequenceIdProperty.WriteLength(context);
        return length;
    }
    
    protected override void Write(WriteStream stream, StreamContext context) {
        var writes = false;
        writes |= _currentTurnIndexProperty.Write(stream, context);
        writes |= _currentTurnTeamIndexProperty.Write(stream, context);
        writes |= _currentTurnProgressIndexProperty.Write(stream, context);
        writes |= _lastPlayedSequenceIdProperty.Write(stream, context);
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
                case (uint) PropertyID.LastPlayedSequenceId: {
                    changed = _lastPlayedSequenceIdProperty.Read(stream, context);
                    if (changed) FireLastPlayedSequenceIdDidChange(lastPlayedSequenceId);
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
        _lastPlayedSequenceId = lastPlayedSequenceId;
    }
    
}
/* ----- End Normal Autogenerated Code ----- */
