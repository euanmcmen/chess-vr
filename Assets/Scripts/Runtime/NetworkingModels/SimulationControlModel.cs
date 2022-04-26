using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class SimulationControlModel
{
    [RealtimeProperty(1, true, false)]
    private bool _started;
}

/* ----- Begin Normal Autogenerated Code ----- */
public partial class SimulationControlModel : RealtimeModel {
    public bool started {
        get {
            return _startedProperty.value;
        }
        set {
            if (_startedProperty.value == value) return;
            _startedProperty.value = value;
            InvalidateReliableLength();
        }
    }
    
    public enum PropertyID : uint {
        Started = 1,
    }
    
    #region Properties
    
    private ReliableProperty<bool> _startedProperty;
    
    #endregion
    
    public SimulationControlModel() : base(null) {
        _startedProperty = new ReliableProperty<bool>(1, _started);
    }
    
    protected override void OnParentReplaced(RealtimeModel previousParent, RealtimeModel currentParent) {
        _startedProperty.UnsubscribeCallback();
    }
    
    protected override int WriteLength(StreamContext context) {
        var length = 0;
        length += _startedProperty.WriteLength(context);
        return length;
    }
    
    protected override void Write(WriteStream stream, StreamContext context) {
        var writes = false;
        writes |= _startedProperty.Write(stream, context);
        if (writes) InvalidateContextLength(context);
    }
    
    protected override void Read(ReadStream stream, StreamContext context) {
        var anyPropertiesChanged = false;
        while (stream.ReadNextPropertyID(out uint propertyID)) {
            var changed = false;
            switch (propertyID) {
                case (uint) PropertyID.Started: {
                    changed = _startedProperty.Read(stream, context);
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
        _started = started;
    }
    
}
/* ----- End Normal Autogenerated Code ----- */
