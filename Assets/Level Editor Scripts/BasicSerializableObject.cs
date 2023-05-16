using fNbt;

namespace Level_Editor_Scripts
{
    public class BasicSerializableObject : SerializableMapObject
    {
        public override void Serialize(NbtCompound root)
        {
        }

        public override void ReadFrom(NbtCompound compound)
        {
            throw new System.NotImplementedException();
        }
    }
}