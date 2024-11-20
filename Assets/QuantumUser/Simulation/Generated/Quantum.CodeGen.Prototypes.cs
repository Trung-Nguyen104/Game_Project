// <auto-generated>
// This code was auto-generated by a tool, every time
// the tool executes this code will be reset.
//
// If you need to extend the classes generated to add
// fields or methods to them, please create partial
// declarations in another file.
// </auto-generated>
#pragma warning disable 0109
#pragma warning disable 1591


namespace Quantum.Prototypes {
  using Photon.Deterministic;
  using Quantum;
  using Quantum.Core;
  using Quantum.Collections;
  using Quantum.Inspector;
  using Quantum.Physics2D;
  using Quantum.Physics3D;
  using Byte = System.Byte;
  using SByte = System.SByte;
  using Int16 = System.Int16;
  using UInt16 = System.UInt16;
  using Int32 = System.Int32;
  using UInt32 = System.UInt32;
  using Int64 = System.Int64;
  using UInt64 = System.UInt64;
  using Boolean = System.Boolean;
  using String = System.String;
  using Object = System.Object;
  using FlagsAttribute = System.FlagsAttribute;
  using SerializableAttribute = System.SerializableAttribute;
  using MethodImplAttribute = System.Runtime.CompilerServices.MethodImplAttribute;
  using MethodImplOptions = System.Runtime.CompilerServices.MethodImplOptions;
  using FieldOffsetAttribute = System.Runtime.InteropServices.FieldOffsetAttribute;
  using StructLayoutAttribute = System.Runtime.InteropServices.StructLayoutAttribute;
  using LayoutKind = System.Runtime.InteropServices.LayoutKind;
  #if QUANTUM_UNITY //;
  using TooltipAttribute = UnityEngine.TooltipAttribute;
  using HeaderAttribute = UnityEngine.HeaderAttribute;
  using SpaceAttribute = UnityEngine.SpaceAttribute;
  using RangeAttribute = UnityEngine.RangeAttribute;
  using HideInInspectorAttribute = UnityEngine.HideInInspector;
  using PreserveAttribute = UnityEngine.Scripting.PreserveAttribute;
  using FormerlySerializedAsAttribute = UnityEngine.Serialization.FormerlySerializedAsAttribute;
  using MovedFromAttribute = UnityEngine.Scripting.APIUpdating.MovedFromAttribute;
  using CreateAssetMenu = UnityEngine.CreateAssetMenuAttribute;
  using RuntimeInitializeOnLoadMethodAttribute = UnityEngine.RuntimeInitializeOnLoadMethodAttribute;
  #endif //;
  
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.BulletInfo))]
  public unsafe partial class BulletInfoPrototype : ComponentPrototype<Quantum.BulletInfo> {
    public PlayerRef OwnerPlayer;
    public FP Damage;
    public FP BulletTimeOut;
    partial void MaterializeUser(Frame frame, ref Quantum.BulletInfo result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.BulletInfo component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.BulletInfo result, in PrototypeMaterializationContext context = default) {
        result.OwnerPlayer = this.OwnerPlayer;
        result.Damage = this.Damage;
        result.BulletTimeOut = this.BulletTimeOut;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.GameSession))]
  public unsafe partial class GameSessionPrototype : ComponentPrototype<Quantum.GameSession> {
    public Quantum.QEnum32<GameState> GameState;
    partial void MaterializeUser(Frame frame, ref Quantum.GameSession result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.GameSession component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.GameSession result, in PrototypeMaterializationContext context = default) {
        result.GameState = this.GameState;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.Input))]
  public unsafe partial class InputPrototype : StructPrototype {
    public FPVector2 mousePosition;
    public Button PickUpItem;
    public Button DropItem;
    public Button InteracAction;
    public Button RightMouseClick;
    public Int32 SelectItem;
    partial void MaterializeUser(Frame frame, ref Quantum.Input result, in PrototypeMaterializationContext context);
    public void Materialize(Frame frame, ref Quantum.Input result, in PrototypeMaterializationContext context = default) {
        result.mousePosition = this.mousePosition;
        result.PickUpItem = this.PickUpItem;
        result.DropItem = this.DropItem;
        result.InteracAction = this.InteracAction;
        result.RightMouseClick = this.RightMouseClick;
        result.SelectItem = this.SelectItem;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.ItemInfo))]
  public unsafe partial class ItemInfoPrototype : ComponentPrototype<Quantum.ItemInfo> {
    public Quantum.Prototypes.ItemProfilePrototype Item;
    [Header("If Item Is Gun")]
    public Int32 GunAmmo;
    public AssetRef<EntityPrototype> BulletPrototype;
    partial void MaterializeUser(Frame frame, ref Quantum.ItemInfo result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.ItemInfo component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.ItemInfo result, in PrototypeMaterializationContext context = default) {
        this.Item.Materialize(frame, ref result.Item, in context);
        result.GunAmmo = this.GunAmmo;
        result.BulletPrototype = this.BulletPrototype;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.ItemProfile))]
  public unsafe partial class ItemProfilePrototype : StructPrototype {
    public AssetRef<ItemData> ItemData;
    public AssetRef<EntityPrototype> ItemPrototype;
    partial void MaterializeUser(Frame frame, ref Quantum.ItemProfile result, in PrototypeMaterializationContext context);
    public void Materialize(Frame frame, ref Quantum.ItemProfile result, in PrototypeMaterializationContext context = default) {
        result.ItemData = this.ItemData;
        result.ItemPrototype = this.ItemPrototype;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.ItemSpawn))]
  public unsafe partial class ItemSpawnPrototype : StructPrototype {
    public Quantum.Prototypes.ItemProfilePrototype ItemProfile;
    public Int32 ItemQuantity;
    partial void MaterializeUser(Frame frame, ref Quantum.ItemSpawn result, in PrototypeMaterializationContext context);
    public void Materialize(Frame frame, ref Quantum.ItemSpawn result, in PrototypeMaterializationContext context = default) {
        this.ItemProfile.Materialize(frame, ref result.ItemProfile, in context);
        result.ItemQuantity = this.ItemQuantity;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.ItemSpawner))]
  public unsafe partial class ItemSpawnerPrototype : ComponentPrototype<Quantum.ItemSpawner> {
    public AssetRef<ItemSpawnPosition> ItemSpawnPosition;
    [ArrayLengthAttribute(30)]
    public Quantum.Prototypes.PositionsPrototype[] Positions = new Quantum.Prototypes.PositionsPrototype[30];
    [ArrayLengthAttribute(3)]
    public Quantum.Prototypes.ItemSpawnPrototype[] Item = new Quantum.Prototypes.ItemSpawnPrototype[3];
    partial void MaterializeUser(Frame frame, ref Quantum.ItemSpawner result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.ItemSpawner component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.ItemSpawner result, in PrototypeMaterializationContext context = default) {
        result.ItemSpawnPosition = this.ItemSpawnPosition;
        for (int i = 0, count = PrototypeValidator.CheckLength(Positions, 30, in context); i < count; ++i) {
          this.Positions[i].Materialize(frame, ref *result.Positions.GetPointer(i), in context);
        }
        for (int i = 0, count = PrototypeValidator.CheckLength(Item, 3, in context); i < count; ++i) {
          this.Item[i].Materialize(frame, ref *result.Item.GetPointer(i), in context);
        }
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.MousePointerInfo))]
  public unsafe partial class MousePointerInfoPrototype : ComponentPrototype<Quantum.MousePointerInfo> {
    public FPVector2 targetPosition;
    public FP AimAngle;
    partial void MaterializeUser(Frame frame, ref Quantum.MousePointerInfo result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.MousePointerInfo component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.MousePointerInfo result, in PrototypeMaterializationContext context = default) {
        result.targetPosition = this.targetPosition;
        result.AimAngle = this.AimAngle;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.PlayerInfo))]
  public unsafe partial class PlayerInfoPrototype : ComponentPrototype<Quantum.PlayerInfo> {
    public PlayerRef PlayerRef;
    public Int32 PlayerSkinColor;
    [Header("Inventory")]
    [ArrayLengthAttribute(4)]
    public Quantum.Prototypes.ItemInfoPrototype[] Inventory = new Quantum.Prototypes.ItemInfoPrototype[4];
    public QBoolean HadWeapon;
    public Int32 CurrSelectItem;
    partial void MaterializeUser(Frame frame, ref Quantum.PlayerInfo result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.PlayerInfo component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.PlayerInfo result, in PrototypeMaterializationContext context = default) {
        result.PlayerRef = this.PlayerRef;
        result.PlayerSkinColor = this.PlayerSkinColor;
        for (int i = 0, count = PrototypeValidator.CheckLength(Inventory, 4, in context); i < count; ++i) {
          this.Inventory[i].Materialize(frame, ref *result.Inventory.GetPointer(i), in context);
        }
        result.HadWeapon = this.HadWeapon;
        result.CurrSelectItem = this.CurrSelectItem;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.PlayerRoleManager))]
  public unsafe partial class PlayerRoleManagerPrototype : ComponentPrototype<Quantum.PlayerRoleManager> {
    [ArrayLengthAttribute(8)]
    public Quantum.Prototypes.RoleProfilePrototype[] RoleProfiles = new Quantum.Prototypes.RoleProfilePrototype[8];
    partial void MaterializeUser(Frame frame, ref Quantum.PlayerRoleManager result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.PlayerRoleManager component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.PlayerRoleManager result, in PrototypeMaterializationContext context = default) {
        for (int i = 0, count = PrototypeValidator.CheckLength(RoleProfiles, 8, in context); i < count; ++i) {
          this.RoleProfiles[i].Materialize(frame, ref *result.RoleProfiles.GetPointer(i), in context);
        }
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.Positions))]
  public unsafe partial class PositionsPrototype : StructPrototype {
    public FPVector2 Position;
    public QBoolean isSpawned;
    partial void MaterializeUser(Frame frame, ref Quantum.Positions result, in PrototypeMaterializationContext context);
    public void Materialize(Frame frame, ref Quantum.Positions result, in PrototypeMaterializationContext context = default) {
        result.Position = this.Position;
        result.isSpawned = this.isSpawned;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.RoleProfile))]
  public unsafe partial class RoleProfilePrototype : StructPrototype {
    public Quantum.QEnum32<PlayerRole> PlayerRole;
    public Int32 RoleQuantity;
    partial void MaterializeUser(Frame frame, ref Quantum.RoleProfile result, in PrototypeMaterializationContext context);
    public void Materialize(Frame frame, ref Quantum.RoleProfile result, in PrototypeMaterializationContext context = default) {
        result.PlayerRole = this.PlayerRole;
        result.RoleQuantity = this.RoleQuantity;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.TaskInfo))]
  public unsafe partial class TaskInfoPrototype : ComponentPrototype<Quantum.TaskInfo> {
    public Quantum.QEnum32<TaskType> TaskType;
    public QBoolean IsTaskCompleted;
    partial void MaterializeUser(Frame frame, ref Quantum.TaskInfo result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.TaskInfo component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.TaskInfo result, in PrototypeMaterializationContext context = default) {
        result.TaskType = this.TaskType;
        result.IsTaskCompleted = this.IsTaskCompleted;
        MaterializeUser(frame, ref result, in context);
    }
  }
}
#pragma warning restore 0109
#pragma warning restore 1591
