public interface ISignalSource
{
    float Value { get; }
    float SnapToAchievable(float value) => value;

}