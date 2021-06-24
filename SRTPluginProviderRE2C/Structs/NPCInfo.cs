﻿using SRTPluginProviderRE2C.Enumerations;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace SRTPluginProviderRE2C.Structs
{
    [DebuggerDisplay("{_DebuggerDisplay,nq}")]
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public unsafe struct NPCInfo
    {
        // In-memory values
        [FieldOffset(0x08)]
        public NPCModelTypeEnumeration ModelType;

        [FieldOffset(0x38)]
        public int x;

        [FieldOffset(0x3C)]
        public int z;

        [FieldOffset(0x40)]
        public int y;

        [FieldOffset(0x76)]
        public uint roomID;

        [FieldOffset(0x156)]
        public ushort currentHP;

        public static NPCInfo AsStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(NPCInfo*)pb;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string _DebuggerDisplay
        {
            get
            {
                return string.Format("Model: {0} | HP: {1}", ModelType, CurrentHP);
            }
        }
        public NPCModelTypeEnumeration EnemyType { get => ModelType; }
        public string EnemyTypeString => ModelType.ToString();
        public uint RoomID { get => roomID; }
        public ushort CurrentHP { get => currentHP; }
        public ushort MaximumHP => Enemy.GetHitPoints(this.ModelType);
        public bool IsBoss => Enemy.IsBoss(this.ModelType);
        public bool IsDead => CurrentHP == 0xFFFF;
        public bool IsAlive => !IsDead && CurrentHP > 0 && CurrentHP < 30000;
    }

    public class Enemy
    {
        public static Dictionary<NPCModelTypeEnumeration, ushort[]> MaxHitPoints = new Dictionary<NPCModelTypeEnumeration, ushort[]>() {
            { NPCModelTypeEnumeration.ZombieBrad, new ushort[] { 0, 250, 1250 } },
            { NPCModelTypeEnumeration.Croc, new ushort[] { 0, 300, 300 } },
            { NPCModelTypeEnumeration.MrX1, new ushort[] { 0, 400, 600 } },
            { NPCModelTypeEnumeration.MrX2, new ushort[] { 0, 200, 200 } },
            { NPCModelTypeEnumeration.GEmbryo, new ushort[] { 400, 600, 1000 } },
            { NPCModelTypeEnumeration.BirkinG1, new ushort[] { 0, 500, 800 } },
            { NPCModelTypeEnumeration.BirkinG2, new ushort[] { 0, 700, 1200 } },
            { NPCModelTypeEnumeration.BirkinG3, new ushort[] { 0, 900, 1400 } },
            { NPCModelTypeEnumeration.BirkinG4, new ushort[] { 0, 1100, 1300 } },
            { NPCModelTypeEnumeration.BirkinG5, new ushort[] { 0, 600, 1000 } }
        };

        public static ushort GetHitPoints(NPCModelTypeEnumeration npc)
        {
            var diff = GameMemoryRE2CScanner.GetCurrentDifficulty();
            if (MaxHitPoints.ContainsKey(npc))
            {
                return MaxHitPoints[npc][(byte)diff];
            }
            return 0;
        }

        public static bool IsBoss(NPCModelTypeEnumeration npc)
        {
            return MaxHitPoints.ContainsKey(npc);
        }
    }
}