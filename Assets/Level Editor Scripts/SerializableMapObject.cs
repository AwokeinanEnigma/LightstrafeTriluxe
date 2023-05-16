using fNbt;
using UnityEngine;

namespace Level_Editor_Scripts
{
    public abstract class SerializableMapObject : MonoBehaviour
    {
        public int CatalogIndex;
        public abstract void Serialize(NbtCompound root);
        public abstract void ReadFrom(NbtCompound compound);
    }
}