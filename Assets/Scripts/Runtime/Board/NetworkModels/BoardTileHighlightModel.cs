using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class BoardTileHighlightModel
{
    [RealtimeProperty(1, true, true)]
    private bool _isTileHighlightActive;
}

/* ----- Begin Normal Autogenerated Code ----- */
public partial class BoardTileHighlightModel : RealtimeModel {
    public bool isTileHighlightActive {
        get {
            return _isTileHighlightActiveProperty.value;
        }
        set {
            if (_isTileHighlightActiveProperty.value == value) return;
            _isTileHighlightActiveProperty.value = value;
            InvalidateReliableLength();
            FireIsTileHighlightActiveDidChange(value);
        }
    }
    
    public delegate void PropertyChangedHandler<in T>(BoardTileHighlightModel model, T value);
    public event PropertyChangedHandler<bool> isTileHighlightActiveDidChange;
    
    public enum PropertyID : uint {
        IsTileHighlightActive = 1,
    }
    
    #region Properties
    
    private ReliableProperty<bool> _isTileHighlightActiveProperty;
    
    #endregion
    
    public BoardTileHighlightModel() : base(null) {
        _isTileHighlightActiveProperty = new ReliableProperty<bool>(1, _isTileHighlightActive);
    }
    
    protected override void OnParentReplaced(RealtimeModel previousParent, RealtimeModel currentParent) {
        _isTileHighlightActiveProperty.UnsubscribeCallback();
    }
    
    private void FireIsTileHighlightActiveDidChange(bool value) {
        try {
            isTileHighlightActiveDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    protected override int WriteLength(StreamContext context) {
        var length = 0;
        length += _isTileHighlightActiveProperty.WriteLength(context);
        return length;
    }
    
    protected override void Write(WriteStream stream, StreamContext context) {
        var writes = false;
        writes |= _isTileHighlightActiveProperty.Write(stream, context);
        if (writes) InvalidateContextLength(context);
    }
    
    protected override void Read(ReadStream stream, StreamContext context) {
        var anyPropertiesChanged = false;
        while (stream.ReadNextPropertyID(out uint propertyID)) {
            var changed = false;
            switch (propertyID) {
                case (uint) PropertyID.IsTileHighlightActive: {
                    changed = _isTileHighlightActiveProperty.Read(stream, context);
                    if (changed) FireIsTileHighlightActiveDidChange(isTileHighlightActive);
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
        _isTileHighlightActive = isTileHighlightActive;
    }
    
}
/* ----- End Normal Autogenerated Code ----- */
