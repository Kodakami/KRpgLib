namespace KRpgLib.Utility.ValueCurves
{
    public struct KnownPointExtents<TypeX, TypeY>
    {
        public KnownPointExtents(TypeX firstPositionX, TypeY firstPositionY, TypeX lastPositionX, TypeY lastPositionY)
        {
            FirstPositionX = firstPositionX;
            FirstValueY = firstPositionY;
            LastPositionX = lastPositionX;
            LastValueY = lastPositionY;
        }

        public TypeX FirstPositionX { get; }
        public TypeY FirstValueY { get; }
        public TypeX LastPositionX { get; }
        public TypeY LastValueY { get; }
    }
}
